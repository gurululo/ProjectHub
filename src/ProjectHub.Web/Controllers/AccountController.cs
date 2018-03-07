using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectHub.Web.Models.ViewModels;
using ProjectHub.Web.Services;

namespace ProjectHub.Web.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly AdminAccountService _adminAccountService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(AdminAccountService adminAccountService, ILogger<AccountController> logger)
        {
            _adminAccountService = adminAccountService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel
            {
                ReturnUrl = NormalizeReturnUrl(returnUrl)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!_adminAccountService.ValidateCredentials(model.UserName, model.Password))
            {
                _logger.LogWarning("管理员登录失败，用户名：{UserName}。", model.UserName);

                // 增加失败响应时间，降低暴力尝试的效率。
                await Task.Delay(300);

                ModelState.AddModelError(string.Empty, "用户名或密码不正确，请重试。");
                return View(model);
            }

            var userName = model.UserName.Trim();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, "Administrator")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var properties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe
            };

            if (model.RememberMe)
            {
                properties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7);
            }

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                properties);

            _adminAccountService.RecordLogin(userName);
            _logger.LogInformation("管理员 {UserName} 登录成功。", userName);

            var returnUrl = NormalizeReturnUrl(model.ReturnUrl);
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        private string NormalizeReturnUrl(string returnUrl)
        {
            return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
                ? returnUrl
                : null;
        }
    }
}
