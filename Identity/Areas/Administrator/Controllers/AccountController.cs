using Identity.Core.Application.Contracts.Acccount;
using Identity.Core.Application.DTOs.Account;
using IdentitySample.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;

namespace Identity.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IUtilities _Utilities;
        private readonly IMemoryCache _memoryCache;
        public AccountController(IAccountService accountService, IUtilities utilities, IMemoryCache memoryCache)
        {
            _accountService = accountService;
            _Utilities = utilities;
            _memoryCache = memoryCache;
        }

        // GET: AccountController
        [Authorize(Policy = "AccountsList")]
        public async Task<ActionResult> Index()
        {
            var model = await _accountService.GetList();
            return View(model);
        }

        [Authorize(Policy = "AddClaims")]
        public async Task<ActionResult> AddClaims(string id)
        {
            var model = await _accountService.GetClaimsForAddClaims(id);
            if (model == null)
            {
                return NotFound();
            }
            ViewData["UserName"] = await _accountService.ReturnUserNameBy(id);
            return View(model);
        }

        [Authorize(Policy = "AddClaims")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddClaims(ManageClaimsDto claims)
        {
            try
            {
                var result = await _accountService.AddClaimsToUser(claims);
                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.ErrorMessages)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View(claims);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
        }

        [Authorize(Policy = "RemoveClaims")]
        public async Task<ActionResult> RemoveClaims(string id)
        {
            var model = await _accountService.GetClaimsForRemoveClaims(id);
            if (model == null)
            {
                return NotFound();
            }
            ViewData["UserName"] = await _accountService.ReturnUserNameBy(id);
            return View(model);
        }

        [Authorize(Policy = "RemoveClaims")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveClaims(ManageClaimsDto claims)
        {
            try
            {
                var result = await _accountService.RemoveClaimsFromUser(claims);
                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.ErrorMessages)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View(claims);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
        }

        // GET: AccountController/Roles
        [Authorize(Policy = "Roles")]
        public async Task<ActionResult> Roles()
        {
            var model = await _accountService.GetRoles();
            return View(model);
        }

        [Authorize(Policy = "Roles")]
        public ActionResult CreateRole()
        {
            var allSitePath = _Utilities.ActionAndControllerNamesList();
            _memoryCache.GetOrCreate("ActionAndControllerNamesList", p =>
            {
                p.AbsoluteExpiration = DateTimeOffset.MaxValue;
                return allSitePath;
            });

            var model = new CreateRoleDto()
            {
                SitePath = allSitePath
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateRole(CreateRoleDto role)
        {
            var result = await _accountService.CreateRole(role);
            if (result.IsSuccess)
            {
                return RedirectToAction(nameof(Roles));
            }

            foreach (var error in result.ErrorMessages)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(role);
        }

        [HttpGet]
        public async Task<ActionResult> AddClaimsToRole(string id)
        {
            var allPath = _Utilities.ActionAndControllerNamesList();
            var model = await _accountService.GetRoleClaimsForAddRoleClaims(id, allPath);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> AddClaimsToRole(ManageRoleClaimsDto ciams)
        {
            try
            {
                var result = await _accountService.AddCliamsToRole(ciams);
                if (!result.IsSuccess)
                {
                    foreach (var error in result.ErrorMessages)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    return View(ciams);
                }

                return RedirectToAction(nameof(Roles));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
        }

        // GET: AccountController/ManageUserRoles
        [Authorize(Policy = "ManageUserRole")]
        public async Task<ActionResult> ManageUserRoles(string id)
        {
            var model = await _accountService.GetUserRoles(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: AccountController/ManageUserRoles
        [Authorize(Policy = "ManageUserRole")]
        [HttpPost]
        public async Task<ActionResult> ManageUserRoles(AddRoleToUserDto addRoles)
        {
            try
            {
                var result = await _accountService.AddRoleToUser(addRoles);
                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.ErrorMessages)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View(addRoles);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
        }

        // GET: AccountController/Details/5
        [Authorize(Policy = "DetailAccount")]
        public async Task<ActionResult> Details(string id)
        {
            var model = await _accountService.Getby(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        public async Task<ActionResult> UpdateSecurityStamp(string id)
        {
            await _accountService.UpdateSecurityStamp(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
