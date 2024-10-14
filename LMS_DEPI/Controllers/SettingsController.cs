using LMS.Data;
using LMS_DEPI.APP.Database;
using LMS_DEPI.APP.ViewModels;
using LMS_DEPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LMS_DEPI.Controllers
{
    public class SettingsController : Controller
    {
        private readonly UserManager<UserIdentity> _userManager;
        private readonly SignInManager<UserIdentity> _signInManager;
        private readonly ApplicationDbContext _context; // Include if you need database access

        public SettingsController(UserManager<UserIdentity> userManager, SignInManager<UserIdentity> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context; // Initialize context if needed
        }

        public IActionResult Index()
        {
            var model = new SettingsViewModel();
            return View(model);
        }

        // New DeleteAccount methods

        [HttpGet]
        [Authorize]
        public IActionResult DeleteAccount()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccountConfirmed()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Sign the user out before deleting the account
            await _signInManager.SignOutAsync();

            // Delete the user account
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("AccountDeleted"); // Redirect to a confirmation page
            }

            // If there was an error, stay on the same page
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("DeleteAccount"); // Show the confirmation page again in case of an error
        }

        [HttpGet]
        public IActionResult AccountDeleted()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUsername(SettingsViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null && !string.IsNullOrEmpty(model.NewUsername))
            {
                user.UserName = model.NewUsername;
                user.NormalizedUserName = model.NewUsername.ToUpper();

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(user); // Refresh sign-in to update session
                    TempData["SuccessMessage"] = "Username changed successfully!";
                    return RedirectToAction("Index", "Settings");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("NewUsername", error.Description);
                    }
                }
            }
            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(SettingsViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null && !string.IsNullOrEmpty(model.CurrentPassword) && !string.IsNullOrEmpty(model.NewPassword))
            {
                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(user);
                    TempData["SuccessMessage"] = "Password changed successfully!";
                    return RedirectToAction("Index", "Settings");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                }
            }
            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(SettingsViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null && !string.IsNullOrEmpty(model.NewEmail))
            {
                var token = await _userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);
                var result = await _userManager.ChangeEmailAsync(user, model.NewEmail, token);

                if (result.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(user);
                    TempData["SuccessMessage"] = "Email changed successfully!";
                    return RedirectToAction("Index", "Settings");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("NewEmail", error.Description);
                    }
                }
            }
            return View("Index", model);
        }
    }
}
