using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LMS_DEPI.Entities.Models;
using LMS_DEPI.Models;
using LMS_DEPI.APP.Database;

namespace LMS_DEPI.Controllers
{
    public class SettingsController : Controller
    {
        private readonly UserManager<UserIdentity> _userManager;
        private readonly SignInManager<UserIdentity> _signInManager;

        public SettingsController(UserManager<UserIdentity> userManager, SignInManager<UserIdentity> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            var model = new SettingsViewModel();
            return View(model);
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
