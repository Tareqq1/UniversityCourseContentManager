using LMS.Data;
using LMS_DEPI.APP.Database;
using LMS_DEPI.APP.ViewModels;
using LMS_DEPI.Entities.Models;
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
        private readonly LMSContext _lmsContext;


        public AccountController(UserManager<UserIdentity> userManager, SignInManager<UserIdentity> signInManager, ApplicationDbContext context, LMSContext lmsContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _lmsContext = lmsContext;
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

        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to sign in the user
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Find the signed-in user
                    var user = await _userManager.FindByNameAsync(model.Username);

                    // Check the user's roles and redirect accordingly
                    if (await _userManager.IsInRoleAsync(user, "Teacher"))
                    {
                        return RedirectToAction("Index", "Teacher");
                    }
                    else if (await _userManager.IsInRoleAsync(user, "User"))
                    {
                        return RedirectToAction("Index", "Student");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home"); // Default fallback
                    }
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
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Step 1: Create the Identity user (UserIdentity)
                var userIdentity = new UserIdentity { UserName = model.Username, Email = model.Email };
                var result = await _userManager.CreateAsync(userIdentity, model.Password);

                if (result.Succeeded)
                {
                    // Step 2: Assign roles (Teacher, Admin, or User/Student)
                    if (model.IsTeacher)
                    {
                        await _userManager.AddToRoleAsync(userIdentity, "Teacher");
                    }
                    else if (model.IsAdmin)
                    {
                        await _userManager.AddToRoleAsync(userIdentity, "Admin");
                    }
                    else // If registering as a User (Student)
                    {
                        await _userManager.AddToRoleAsync(userIdentity, "User");

                        // Create and save the Student record if the user is a student
                        var student = new Student
                        {
                            Name = model.Username,
                            Email = model.Email // Assuming you use email for the student
                        };

                        _lmsContext.Students.Add(student);  // Add to Students table
                    }

                    // Step 3: Manually create and save the custom user in LMSContext (Users table)
                    var customUser = new User
                    {
                        Username = model.Username,
                        Password = model.Password,  // Storing plain passwords is not recommended, consider hashing
                        Role = model.IsTeacher ? "Teacher" : model.IsAdmin ? "Admin" : "User"
                    };

                    _lmsContext.Users.Add(customUser); // Add to the custom Users table
                    await _lmsContext.SaveChangesAsync();  // Save to the LMSContext database

                    // Step 4: Sign the user in after registration
                    await _signInManager.SignInAsync(userIdentity, isPersistent: false);

                    return RedirectToAction("Index", "Home");
                }

                // Handle errors if registration failed
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Return the view with the model to display errors
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
