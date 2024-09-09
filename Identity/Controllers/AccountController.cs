using Identity.Core.Application.Contracts.Identity;
using Identity.Core.Application.DTOs.Account;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
    public class AccountController : Controller
    {
        private readonly IIdentityService _identityService;
        public AccountController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        #region Register

        [HttpGet]
        public ActionResult Register()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction(actionName: "Index", controllerName: "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterAccountDto account)
        {
            try
            {
                if (User.Identity!.IsAuthenticated)
                {
                    return RedirectToAction(actionName: "Index", controllerName: "Home");
                }

                var result = await _identityService.RgisterAccount(account);

                if (result.IsSuccess)
                {
                    return RedirectToAction(actionName: "Index", controllerName: "Home");
                }
                foreach (var error in result.ErrorMessages)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return View(account);
            }
            catch
            {
                return View();
            }
        }

        #endregion

        #region Login

        [HttpGet]
        public ActionResult Login(string? returnUrl = null)
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction(actionName: "Index", controllerName: "Home");
            }
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginAccountDto account, string? returnUrl = null)
        {
            try
            {
                if (User.Identity!.IsAuthenticated)
                {
                    return RedirectToAction(actionName: "Index", controllerName: "Home");
                }

                var result = await _identityService.LoginAccount(account);

                if (result.IsSuccess)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction(actionName: "Index", controllerName: "Home");
                }
                foreach (var error in result.ErrorMessages)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return View(account);
            }
            catch
            {
                return View();
            }
        }

        #endregion

        #region Logout

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Logout()
        {
            await _identityService.Logout();
            return RedirectToAction(actionName: "Index", controllerName: "Home");
        }

        #endregion
    }
}
