using LMS.Data;
using LMS_DEPI.APP.Database;
using LMS_DEPI.APP.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LMS_DEPI.APP.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<UserIdentity> _userManager;
        private readonly ApplicationDbContext _context; // Add the context here
        private readonly SignInManager<UserIdentity> _signInManager;


        public AccountController(UserManager<UserIdentity> userManager, SignInManager<UserIdentity> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
        public async Task<IActionResult> Dashboard()
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Assuming your user model has a FullName property
                ViewBag.FullName = user.UserName;
            }
            else
            {
                ViewBag.FullName = "Guest"; // In case the user is not logged in
            }

            return View();
        }

        // Display the login form
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Handle user login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to sign in the user
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // On successful login, redirect to the Home/Index page
                    return RedirectToAction("Index", "Home");
                }

                // If login fails, display an error message
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            // Return the view with the model so that errors are shown
            return View(model);
        }

        [HttpGet]
        public IActionResult DeleteAccount()
        {
            return View();
        }

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

        // Optional: Show confirmation page after account deletion
        [HttpGet]
        public IActionResult AccountDeleted()
        {
            return View();
        }

        // Display the registration form
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Handle user registration
        [HttpPost]
        [HttpPost]
        [HttpPost]
        // Handle user registration
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                // Log all validation errors to the console
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage); // Log to console
                }

                // Return the view with the model to display errors
                return View(model);
            }

            var user = new UserIdentity { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Assign the appropriate role based on the checkbox
                if (model.IsTeacher)
                {
                    await _userManager.AddToRoleAsync(user, "Teacher");
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, "User");
                }

                // Sign the user in after registration
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            // Handle errors and add them to the model state
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                Console.WriteLine(error.Description); // Log to console or a file
            }

            // Return the view with the model so that errors are shown
            return View(model);
        }



        public async Task<IActionResult> TestUserCreation()
        {
            var user = new UserIdentity { UserName = "testuser", Email = "test@example.com" };
            var hashedPassword = _userManager.PasswordHasher.HashPassword(user, "Password123!");

            user.PasswordHash = hashedPassword;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User created");
        }



        // Handle user logout
        [HttpPost]
        [ValidateAntiForgeryToken] // For CSRF protection
        [Authorize] // Ensure only authenticated users can access this
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home"); // Redirect to home or wherever you want
        }
    }
}
