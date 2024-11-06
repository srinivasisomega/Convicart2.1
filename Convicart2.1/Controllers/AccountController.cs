using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ConviBLL.ViewModels;
using ConviBLL.Interfaces;
namespace Convicart2._1.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("login")]
        public IActionResult Login() => View();

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _accountService.LoginAsync(model);
            if (result.Succeeded) return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        [HttpGet("register")]
        public IActionResult Register() => View();

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _accountService.RegisterAsync(model);
            if (result.Succeeded) return RedirectToAction("Login");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        
        [HttpGet("external-login-callback")]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            var info = await _accountService.GetExternalLoginInfoAsync();
            if (info == null) return RedirectToAction("Login");

            // Try to sign in the user with the external login provider.
            var result = await _accountService.ExternalLoginSignInAsync(info);
            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            // If the user does not have an account, create one.
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (email != null)
            {
                var userExists = await _accountService.UserExistsAsync(email);
                if (!userExists)
                {
                    // Register the user
                    var registrationResult = await _accountService.RegisterExternalUserAsync(info);
                    if (registrationResult.Succeeded)
                    {
                        // Log in the new user
                        await _accountService.ExternalLoginSignInAsync(info);
                        return RedirectToAction("Index", "Home");
                    }

                    // If there were errors during registration, add them to ModelState
                    foreach (var error in registrationResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Email claim not received from external login provider.");
            }

            return View("ExternalLoginCallback", new ExternalLoginViewModel { Email = email });
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("external-login")]
        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
            var properties = _accountService.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }
    }

}
