using Identity.Core.Application.Contracts.Acccount;
using Identity.Core.Application.DTOs.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
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

        // GET: AccountController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AccountController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AccountController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AccountController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AccountController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AccountController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
