using ConviBLL.Interfaces;
using ConviBLL.ViewModels;
using ConviDAL.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ConviBLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<Customer> _userManager;
        private readonly SignInManager<Customer> _signInManager;

        public AccountService(UserManager<Customer> userManager, SignInManager<Customer> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
        }

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
        {
            var user = new Customer
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Age = model.Age,
                Gender = model.Gender,
                Address = model.Address
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            return result;
        }


        public async Task LogoutAsync() => await _signInManager.SignOutAsync();

        public async Task<ExternalLoginInfo> GetExternalLoginInfoAsync() => await _signInManager.GetExternalLoginInfoAsync();

        public async Task<SignInResult> ExternalLoginSignInAsync(ExternalLoginInfo info)
        {
            return await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
        }

        public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl)
        {
            return _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        }
        public async Task<bool> UserExistsAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }

        public async Task<IdentityResult> RegisterExternalUserAsync(ExternalLoginInfo info)
        {
            // Get email and other details from the external login provider
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);

            // Create a new user instance with the information from the external provider
            var user = new Customer
            {
                UserName = email,
                Email = email,
                // Populate other properties if available from the provider
                FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                LastName = info.Principal.FindFirstValue(ClaimTypes.Surname)
            };

            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                // Link this user with the external login provider
                result = await _userManager.AddLoginAsync(user, info);
            }

            return result;
        }

    }

}

