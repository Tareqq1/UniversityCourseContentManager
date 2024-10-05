using LMS.Data;
using LMS_DEPI.APP.ViewModels;
using LMS_DEPI.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;

namespace LMS_DEPI.APP.Controllers
{
    public class AuthController : Controller
    {
        private readonly LMSContext _context;

        public AuthController(LMSContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the username already exists
                if (_context.Users.Any(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Username is already taken.");
                    return View(model); // Return view with validation error
                }

                // Validate password strength
                if (!IsPasswordStrong(model.Password))
                {
                    ModelState.AddModelError("Password", "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one number.");
                    return View(model); // Return view with validation error
                }

                // Hash password (this should be done before saving password to DB)
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                // Create and save new user
                var newUser = new User
                {
                    Username = model.Username,
                    Password = hashedPassword, // Save hashed password
                    Role = "User"
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                // Redirect to Login after successful registration
                return RedirectToAction("Login", "Auth");
            }

            // If model state is not valid, return the view with errors
            return View(model);
        }

        private bool IsPasswordStrong(string password)
        {
            // Example of password strength criteria
            if (password.Length < 8)
                return false;
            if (!password.Any(char.IsDigit))
                return false;
            if (!password.Any(char.IsUpper))
                return false;
            if (!password.Any(char.IsLower))
                return false;

            return true;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.SingleOrDefault(u => u.Username == model.Username);
                if (user != null && user.Password == model.Password) // Direct comparison
                {
                    // Set up your session or authentication token here
                    // Redirect based on user role
                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("AdminDashboard", "Home");
                    }
                    else if (user.Role == "Teacher")
                    {
                        return RedirectToAction("TeacherDashboard", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Courses");
                    }
                }
                ModelState.AddModelError("", "Invalid username or password.");
            }
            return View(model);
        }



    }
}
