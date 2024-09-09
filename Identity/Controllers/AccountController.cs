using AutoMapper;
using Identity.Core.Application.Contracts.Identity;
using Identity.Core.Application.DTOs.Account;
using Identity.Models.Account;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
    public class AccountController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;
        public AccountController(IIdentityService identityService, IMapper mapper)
        {
            _identityService = identityService;
            _mapper = mapper;
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
        public async Task<ActionResult> Register(RegisterViewModel account)
        {
            try
            {
                if (User.Identity!.IsAuthenticated)
                {
                    return RedirectToAction(actionName: "Index", controllerName: "Home");
                }

                var registerAccountDto = _mapper.Map<RegisterAccountDto>(account);
                var result = await _identityService.RgisterAccount(registerAccountDto);

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

                ViewData["returnUrl"] = returnUrl;
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

        #region Remote Validations

        public async Task<ActionResult> IsEmailAlredyExist(string email)
        {
            var isExist = await _identityService.IsEmailAlreadyExist(email);

            if (isExist)
            {
                return Json("The email entered has already been registered");
            }

            return Json(true);
        }

        public async Task<ActionResult> IsUserNameAlredyExist(string username)
        {
            var isExist = await _identityService.IsUserNameAlreadyExist(username);

            if (isExist)
            {
                return Json("The username entered has already been registered");
            }

            return Json(true);
        }

        #endregion
    }
}
