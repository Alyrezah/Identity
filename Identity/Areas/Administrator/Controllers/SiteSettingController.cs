using Identity.Core.Application.Contracts.Identity;
using Identity.Core.Application.DTOs.SiteSetting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Identity.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    [Authorize(Policy = "DynamicRole")]
    public class SiteSettingController : Controller
    {
        private readonly ISiteSettingService _siteSettingService;
        private readonly IMemoryCache _memoryCache;
        public SiteSettingController(ISiteSettingService siteSettingService, IMemoryCache memoryCache)
        {
            _siteSettingService = siteSettingService;
            _memoryCache = memoryCache;
        }

        // GET: SiteSettingController
        public async Task<ActionResult> Index()
        {
            var model = await _siteSettingService.GetList();
            return View(model);
        }

        // GET: SiteSettingController/RoleValidationGuid
        [HttpGet]
        public async Task<ActionResult> RoleValidationGuid()
        {
            var roleValidationGuid = await _siteSettingService.GetRoleValidationGuid();
            var value = roleValidationGuid?.Value;

            var model = new GenerateNewGuidDto()
            {
                Value = value,
                LastModifiedDate = roleValidationGuid?.LastModifiedDate
            };

            return View(model);
        }

        // POST: SiteSettingController/RoleValidationGuid
        [HttpPost]
        public async Task<ActionResult> RoleValidationGuid(GenerateNewGuidDto guid)
        {
            var roleValidationGuid = await _siteSettingService.GetRoleValidationGuid();

            if (roleValidationGuid == null)
            {
                await _siteSettingService.Create();
            }
            else
            {
                await _siteSettingService.Update(roleValidationGuid);
            }
            _memoryCache.Remove("RoleValidationGuid");
            return RedirectToAction(nameof(Index));
        }
    }
}
