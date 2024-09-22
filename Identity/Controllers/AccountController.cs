using AutoMapper;
using Identity.Core.Application.Contracts.Identity;
using Identity.Core.Application.DTOs.Account;
using Identity.Infrastructure.Idnetity.Models;
using Identity.Models.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;
using System.Text.Json;

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
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
        }

        #endregion

        #region Login

        [HttpGet]
        public async Task<ActionResult> Login(string? returnUrl = null)
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction(actionName: "Index", controllerName: "Home");
            }
            var model = new LoginViewModel()
            {
                ReturnUrl = returnUrl,
                ExternalLogin = await _identityService.GetExternalLogins()
            };
            ViewData["returnUrl"] = returnUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel account, string? returnUrl = null)
        {
            try
            {
                if (User.Identity!.IsAuthenticated)
                {
                    return RedirectToAction(actionName: "Index", controllerName: "Home");
                }
                account.ReturnUrl = returnUrl;
                account.ExternalLogin = await _identityService.GetExternalLogins();
                ViewData["returnUrl"] = returnUrl;
                var loginDto = _mapper.Map<LoginAccountDto>(account);
                var result = await _identityService.LoginAccount(loginDto);

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
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IsEmailAlredyExist(string email)
        {
            var isExist = await _identityService.IsEmailAlreadyExist(email);

            if (isExist)
            {
                return Json("The email entered has already been registered");
            }

            return Json(true);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

        #region Confirm Email

        public async Task<ActionResult> ConfirmEmail(string username, string token)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }
            var result = await _identityService.ConfirmEmail(username, token);

            return Content(result.Message);
        }

        #endregion

        #region External Login

        [HttpPost]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action(action: "ExternalLoginCallBack", controller: "Account",
                values: new { ReturnUrl = returnUrl });

            var properties = _identityService.ConfigureExternalLoginProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        public async Task<ActionResult> ExternalLoginCallBack(string? returnUrl = null, string? remoteError = null)
        {

            returnUrl = (returnUrl != null && Url.IsLocalUrl(returnUrl)) ? returnUrl : Url.Content("~/");

            if (remoteError != null)
            {
                return BadRequest(remoteError);
            }

            var exLoginInfo = await _identityService.GetExternalLoginInfo();

            if (exLoginInfo == null)
            {
                return BadRequest();
            }

            var result = await _identityService.ExternalLogin(exLoginInfo);

            if (result.IsSuccess)
            {
                return Redirect(returnUrl);
            }

            var email = exLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                return BadRequest();
            }

            return View();
            //await _identityService.RegisterUserWithExternalLogin(email, exLoginInfo);
            //return Redirect(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginCallBack(RegisterWithExternalLoginDto account, string? returnUrl = null)
        {
            returnUrl = (returnUrl != null && Url.IsLocalUrl(returnUrl)) ? returnUrl : Url.Content("~/");
            ViewData["returnUrl"] = returnUrl;

            var exLoginInfo = await _identityService.GetExternalLoginInfo();

            if (exLoginInfo == null)
            {
                return BadRequest();
            }

            var email = exLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                return BadRequest();
            }

            await _identityService.RegisterUserWithExternalLogin(email, account, exLoginInfo);
            return Redirect(returnUrl);
        }

        #endregion

        #region Forgot Password

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordDto forgotPassword)
        {
            try
            {
                var result = await _identityService.SendResetPasswordEmail(forgotPassword);
                if (result.IsSuccess)
                {
                    ViewData["Result"] = "Email send";
                }
                else
                {
                    foreach (var error in result.ErrorMessages)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
        }

        [HttpGet]
        public ActionResult ResetPassword(string username, string token)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token))
            {
                return BadRequest();
            }
            var model = new ResetPasswordDto
            {
                Token = token,
                UserName = username
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(ResetPasswordDto password)
        {
            try
            {
                var result = await _identityService.ResetPassword(password);
                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(Login));
                }

                foreach (var error in result.ErrorMessages)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return View(password);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
        }


        #endregion

        #region Login With Phone Number

        [HttpGet]
        public ActionResult LoginWithPhoneNumber()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction(actionName: "Index", controllerName: "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> LoginWithPhoneNumber(LoginWithPhoneNumberDto phobeNumber)
        {
            try
            {
                if (User.Identity!.IsAuthenticated)
                {
                    return RedirectToAction(actionName: "Index", controllerName: "Home");
                }

                if (TempData.ContainsKey("pttpc"))
                {
                    var tempData =
                        JsonSerializer.Deserialize<PhoneTotpTempDataModel>(TempData["pttpc"].ToString());
                    if (tempData.ExpirtionTime > DateTime.Now)
                    {
                        var differenceInSeconds = (int)(tempData.ExpirtionTime - DateTime.Now).TotalSeconds;
                        ModelState.AddModelError(string.Empty, $"Please try again in {differenceInSeconds} seconds to resend the code");
                        TempData.Keep("pttpc");
                        return View();
                    }

                }

                var result = await _identityService.SenTotpCode(phobeNumber);
                TempData["pttpc"] = JsonSerializer.Serialize(result);
                return RedirectToAction(nameof(VerifyLoginWithPhoneNumber));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
        }

        [HttpGet]
        public ActionResult VerifyLoginWithPhoneNumber()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction(actionName: "Index", controllerName: "Home");
            }

            if (!TempData.ContainsKey("pttpc"))
            {
                return BadRequest();
            }

            var tempData =
                       JsonSerializer.Deserialize<PhoneTotpTempDataModel>(TempData["pttpc"].ToString());
            if (tempData.ExpirtionTime <= DateTime.Now)
            {
                ViewData["TotpMessages"] = "Code has expired, get a new code again";
                return RedirectToAction(nameof(LoginWithPhoneNumber));
            }
            TempData.Keep("pttpc");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> VerifyLoginWithPhoneNumber(VerifyTotpCodeDto code)
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction(actionName: "Index", controllerName: "Home");
            }

            if (!TempData.ContainsKey("pttpc"))
            {
                return BadRequest();
            }

            var tempData =
                     JsonSerializer.Deserialize<PhoneTotpTempDataModel>(TempData["pttpc"].ToString());
            if (tempData.ExpirtionTime <= DateTime.Now)
            {
                ViewData["TotpMessages"] = "Code has expired, get a new code again";
                return RedirectToAction(nameof(LoginWithPhoneNumber));
            }
            //code.PhoneNumber = tempData.PhoneNumber;
            //code.SecretKey = tempData.SecretKey;
            var result = await _identityService.LoginWithPhoneNumber(code, tempData.SecretKey, tempData.PhoneNumber);
            if (result.IsSuccess)
            {
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.ErrorMessages)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            return View(code);
        }

        #endregion

        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}
