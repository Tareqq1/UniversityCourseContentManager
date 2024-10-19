using LMS.Data;
using LMS_DEPI.APP.Database;
using LMS_DEPI.APP.ViewModels;
using LMS_DEPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using LMS_DEPI.Entities.Models;

namespace LMS_DEPI.Controllers
{
    public class SettingsController : Controller
    {
        private readonly UserManager<UserIdentity> _userManager;
        private readonly SignInManager<UserIdentity> _signInManager;
        private readonly ApplicationDbContext _context; // Include if you need database access
        private readonly LMSContext _lmsContext;

        public SettingsController(UserManager<UserIdentity> userManager, SignInManager<UserIdentity> signInManager, ApplicationDbContext context, LMSContext lmscontext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context; // Initialize context if needed
            _lmsContext = lmscontext;
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
            // Get the currently logged-in user from ASP.NET Identity
            var user = await _userManager.GetUserAsync(User);

            if (user != null && !string.IsNullOrEmpty(model.NewUsername))
            {
                // Find the corresponding custom user in dbo.Users using the email
                var customUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

                if (customUser == null)
                {
                    // Handle case when custom user is not found
                    ModelState.AddModelError("", "Custom user not found.");
                    return View("Index", model);
                }

                // Update the username in the aspnetusers table (Identity user)
                user.UserName = model.NewUsername;
                user.NormalizedUserName = model.NewUsername.ToUpper();
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Ensure the custom user entity is being tracked
                    _context.Attach(customUser);

                    // Now, update the username (Name) in the custom dbo.Students table
                    var student = await _lmsContext.Students.FirstOrDefaultAsync(s => s.Email == user.Email); // Find the student by email
                    if (student != null)
                    {
                        student.Name = model.NewUsername; // Update the student's Name
                        await _lmsContext.SaveChangesAsync(); // Save changes to dbo.Students
                    }

                    // Refresh the sign-in session with the updated username
                    await _signInManager.RefreshSignInAsync(user);

                    TempData["SuccessMessage"] = "Username changed successfully!";
                    return RedirectToAction("Index", "Settings");
                }
                else
                {
                    // Handle errors from updating the Identity user
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

            if (user == null || string.IsNullOrEmpty(model.NewEmail))
            {
                ModelState.AddModelError("NewEmail", "Invalid user or email cannot be empty.");
                return View("Index", model);
            }

            // Check if the new email already exists
            var studentEmailExists = await _lmsContext.Students.AnyAsync(u => u.Email == model.NewEmail);
            if (studentEmailExists)
            {
                ModelState.AddModelError("NewEmail", "This email is already in use.");
                return View("Index", model);
            }

            // Generate email change token and attempt to change the email in the Identity table
            var token = await _userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);
            var result = await _userManager.ChangeEmailAsync(user, model.NewEmail, token);

            if (result.Succeeded)
            {
                // Find the corresponding student record using the username as the key
                var student = await _lmsContext.Students.FirstOrDefaultAsync(s => s.Name == user.UserName);

                if (student != null)
                {
                    student.Email = model.NewEmail; // Update the email in the student record
                    await _lmsContext.SaveChangesAsync(); // Save changes to the Students table
                }

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

            return View("Index", model);
        }



    }
}
