using Identity.Core.Application.Contracts.Acccount;
using Identity.Core.Application.Contracts.Identity;
using Identity.Infrastructure.Idnetity.Services;
using IdentitySample.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Identity.Security.DynamicRole
{
    public class DynamicRoleRequirementHandler : AuthorizationHandler<DynamicRoleRequirement>
    {
        private readonly IUtilities _utilities;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMemoryCache _memoryCache;
        private readonly IDataProtector _dataProtectorToken;
        private readonly IAccountService _accountService;
        private readonly IIdentityService _identityService;

        public DynamicRoleRequirementHandler(IMemoryCache memoryCache, IHttpContextAccessor contextAccessor,
            IUtilities utilities, IAccountService accountService, IDataProtectionProvider dataProtectionProvider,
            IIdentityService identityService)
        {
            _memoryCache = memoryCache;
            _contextAccessor = contextAccessor;
            _utilities = utilities;
            _accountService = accountService;
            _dataProtectorToken = dataProtectionProvider.CreateProtector("RvgGuid");
            _identityService = identityService;
        }
        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, DynamicRoleRequirement requirement)
        {
            var httpContext = _contextAccessor.HttpContext;
            var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            var dbRoleValidationGuid = await _memoryCache.GetOrCreate("RoleValidationGuid", async p =>
            {
                p.AbsoluteExpiration = DateTimeOffset.MaxValue;
                return await _utilities.DataBaseRoleValidationGuid();
            });


            var allAreasName = _memoryCache.GetOrCreate("allAreasName", p =>
            {
                p.AbsoluteExpiration = DateTimeOffset.MaxValue;
                return _utilities.GetAllAreasNames();
            });


            //Generate ClaimsType
            SplitUserRequestedUrl(httpContext.Request.Path.ToString(), allAreasName,
                out var areaAndControllerAndActionName);

            //Get Unprotected User Cookie value
            UnprotectRvgCookieData(httpContext, out var unprotectedRvgCookie);

            if (!IsRvgCookieDataValid(unprotectedRvgCookie, userId, dbRoleValidationGuid))
            {
                var user = await _accountService.Getby(userId);
                if (user == null)
                    return;

                AddOrUpdateRvgCookie(httpContext, dbRoleValidationGuid, userId);
                await _identityService.RefreshCookie(userId);
            }
            else if (httpContext.User.HasClaim(areaAndControllerAndActionName, true.ToString()))
            {
                context.Succeed(requirement);
            }
            return;
        }


        #region Methods

        private void SplitUserRequestedUrl(string url, List<string> areaNames,
            out string areaAndControllerAndActionName)
        {
            var requestedUrl = url.Split('/')
                .Where(t => !string.IsNullOrEmpty(t)).ToList();
            var urlCount = requestedUrl.Count;
            if (urlCount != 0 &&
                areaNames.Any(t => t.Equals(requestedUrl[0], StringComparison.CurrentCultureIgnoreCase)))
            {
                var areaName = requestedUrl[0];
                var controllerName = (urlCount == 1) ? "HomeController" : requestedUrl[1] + "Controller";
                var actionName = (urlCount > 2) ? requestedUrl[2] : "Index";
                areaAndControllerAndActionName = $"{areaName}|{controllerName}|{actionName}".ToUpper();
            }
            else
            {
                var areaName = "NoArea";
                var controllerName = (urlCount == 0) ? "HomeController" : requestedUrl[0] + "Controller";
                var actionName = (urlCount > 1) ? requestedUrl[1] : "Index";
                areaAndControllerAndActionName = $"{areaName}|{controllerName}|{actionName}".ToUpper();
            }
        }

        private void SplitUserRequestedUrl(HttpContext httpContext,
           out string areaAndControllerAndActionName)
        {
            var areaName = httpContext.Request.RouteValues["area"]?.ToString() ?? "NoArea";
            var controllerName = httpContext.Request.RouteValues["controller"].ToString();
            var actionName = httpContext.Request.RouteValues["action"].ToString();

            areaAndControllerAndActionName = $"{areaName}|{controllerName}|{actionName}".ToUpper();
        }

        private void UnprotectRvgCookieData(HttpContext httpContext, out string unprotectedRvgCookie)
        {
            var protectedRvgCookie = httpContext.Request.Cookies
                .FirstOrDefault(t => t.Key == "RVG").Value;
            unprotectedRvgCookie = null;
            if (!string.IsNullOrEmpty(protectedRvgCookie))
            {
                try
                {
                    unprotectedRvgCookie = _dataProtectorToken.Unprotect(protectedRvgCookie);
                }
                catch (CryptographicException)
                {
                }
            }
        }

        private bool IsRvgCookieDataValid(string rvgCookieData, string validUserId, string validRvg)
        {
            return !string.IsNullOrEmpty(rvgCookieData) &&
                       SplitUserIdFromRvgCookie(rvgCookieData) == validUserId &&
                       SplitRvgFromRvgCookie(rvgCookieData) == validRvg;
        }

        private string SplitUserIdFromRvgCookie(string rvgCookieData)
        {
            return rvgCookieData.Split("|||")[1];
        }

        private string SplitRvgFromRvgCookie(string rvgCookieData)
        {
            return rvgCookieData.Split("|||")[0];
        }

        private string CombineRvgWithUserId(string rvg, string userId)
        {
            return rvg + "|||" + userId;
        }

        private void AddOrUpdateRvgCookie(HttpContext httpContext, string validRvg, string validUserId)
        {
            var rvgWithUserId = CombineRvgWithUserId(validRvg, validUserId);
            var protectedRvgWithUserId = _dataProtectorToken.Protect(rvgWithUserId);
            httpContext.Response.Cookies.Append("RVG", protectedRvgWithUserId,
                new CookieOptions
                {
                    MaxAge = TimeSpan.FromDays(90),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax
                });
        }

        #endregion
    }
}
