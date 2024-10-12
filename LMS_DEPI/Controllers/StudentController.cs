using LMS.Data;
using LMS_DEPI.APP.Database;
using LMS_DEPI.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using LMS_DEPI.APP.ViewModels;

namespace LMS_DEPI.Controllers
{
    [Authorize(Roles = "User")]
    public class StudentController : Controller
    {
        private readonly LMSContext _context;
        private readonly UserManager<UserIdentity> _userManager; // Inject UserManager

        public StudentController(LMSContext context, UserManager<UserIdentity> userManager)
        {
            _context = context;
            _userManager = userManager; // Initialize UserManager
        }

        // Search, filter, and display courses
        public async Task<IActionResult> Index(string searchString, string filter)
        {
            var courses = from c in _context.Courses.Include(c => c.Teacher) select c;

            // Search functionality
            if (!string.IsNullOrEmpty(searchString))
            {
                courses = courses.Where(c => c.Title.Contains(searchString) || c.Description.Contains(searchString));
                ViewData["CurrentSearch"] = searchString;
            }

            // Filter functionality
            if (!string.IsNullOrEmpty(filter) && int.TryParse(filter, out int filterValue))
            {
                courses = courses.Where(c => c.Credits == filterValue);
                ViewData["CurrentFilter"] = filter;
            }

            // Populate the filter dropdown (Credits example)
            var creditFilterOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "2", Text = "2" },
                new SelectListItem { Value = "3", Text = "3" },
                new SelectListItem { Value = "4", Text = "4" },
                new SelectListItem { Value = "5", Text = "5" }
            };
            ViewBag.Filter = new SelectList(creditFilterOptions, "Value", "Text");

            return View(await courses.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Enroll(int courseId)
        {
            // Find the course by its Id
            var course = await _context.Courses.FindAsync(courseId);

            if (course == null)
            {
                return NotFound();
            }

            // Get the Identity user (this represents the logged-in user)
            var userIdentity = await _userManager.GetUserAsync(User);

            if (userIdentity == null)
            {
                return NotFound("User not found");
            }

            // Find the user in the Students table by matching the Identity Username
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == userIdentity.Email);

            if (student == null)
            {
                return NotFound("Student not found");
            }

            // Check if the student is already enrolled in the course
            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.StudentId == student.Id);

            if (existingEnrollment != null)
            {
                TempData["Message"] = "You are already enrolled in this course.";
                return RedirectToAction("Index");
            }

            // Create a new enrollment
            var enrollment = new Enrollment
            {
                CourseId = courseId,
                StudentId = student.Id,  // Use the student Id from the Students table
                EnrolledAt = DateTime.Now
            };

            // Add the enrollment to the database
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyCourses");
        }










        // View lessons for a specific course
        public async Task<IActionResult> CourseLessons(int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.Lessons)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
            {
                return NotFound();
            }

            return View(course.Lessons);  // Display the lessons for the course
        }
        public async Task<IActionResult> MyCourses()
        {
            // Get the currently logged-in user from Identity
            var userIdentity = await _userManager.GetUserAsync(User);

            if (userIdentity == null)
            {
                return NotFound("User not found");
            }

            // Find the student in the Students table by matching the Identity Email
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == userIdentity.Email);

            if (student == null)
            {
                return NotFound("Student not found");
            }

            // Ensure that the enrollments are being fetched based on the correct StudentId
            var enrolledCourses = await _context.Enrollments
                .Where(e => e.StudentId == student.Id)  // Use the Student Id from the Students table
                .Include(e => e.Course)  // Include the course details
                .ToListAsync();

            return View(enrolledCourses);  // Pass the enrolled courses to the view
        }




        public async Task<IActionResult> Profile()
        {
            // Get the currently logged-in user from Identity
            var userIdentity = await _userManager.GetUserAsync(User);

            if (userIdentity == null)
            {
                return NotFound("User not found");
            }

            // Find the custom user in the Users table by Identity Username (not Id)
            var customUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == userIdentity.UserName);

            if (customUser == null)
            {
                return NotFound("Custom user not found");
            }

            // Find the student in the Students table by matching the Identity Email
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == userIdentity.Email);

            if (student == null)
            {
                return NotFound("Student not found");
            }

            // Get the courses the student is enrolled in using StudentId
            var enrolledCourses = await _context.Enrollments
                .Where(e => e.StudentId == student.Id)  // Use the Student Id from the Students table
                .Include(e => e.Course)
                .ToListAsync();

            // Create the ViewModel
            var viewModel = new StudentProfileViewModel
            {
                CustomUser = customUser,
                IdentityUser = userIdentity,  // Use IdentityUser to get email
                EnrolledCourses = enrolledCourses
            };

            return View(viewModel);
        }




    }
}
