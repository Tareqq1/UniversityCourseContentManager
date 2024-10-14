using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS_DEPI.Entities.Models;
using LMS.Data;
using System.Linq;
using System.Threading.Tasks;
using LMS_DEPI.APP.ViewModels;

namespace LMS_DEPI.Controllers
{
    public class LessonsController : Controller
    {
        private readonly LMSContext _context;

        public LessonsController(LMSContext context)
        {
            _context = context;
        }

        // GET: Lessons/Index
        public async Task<IActionResult> Index(int courseId)
        {
            var lessons = await _context.Lessons
                .Where(l => l.CourseId == courseId)
                .ToListAsync();

            // Retrieve the course name using the courseId
            var course = await _context.Courses.FindAsync(courseId);
            ViewBag.CourseName = course?.Title ?? "Unknown Course"; // Default to "Unknown Course" if not found
            ViewBag.CourseId = courseId; // Pass the course ID to the view
            return View(lessons);
        }

        // GET: Lessons/Create
        [Authorize(Roles = "Teacher")] // Only allow teachers to create lessons
        public IActionResult Create(int courseId)
        {
            var lessonViewModel = new LessonViewModel
            {
                CourseId = courseId, // Set the CourseId
                DueDate = DateTime.Now // Optional: set a default value
            };
            return View(lessonViewModel); // Return the LessonViewModel to the view
        }


        // POST: Lessons/Create
        [HttpPost]
        [Authorize(Roles = "Teacher")] // Only allow teachers to create lessons
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LessonViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the CourseId exists
                var courseExists = await _context.Courses.AnyAsync(c => c.Id == model.CourseId);
                if (!courseExists)
                {
                    ModelState.AddModelError("CourseId", "The selected course does not exist.");
                    return View(model);
                }

                // Create a new Lesson object
                var lesson = new Lesson
                {
                    CourseId = model.CourseId, // Use the CourseId from the model
                    Title = model.Title,
                    Description = model.Description,
                    DueDate = model.DueDate,
                    ReleaseDate = DateTime.Now,
                    FilePath = model.FilePath
                };

                _context.Lessons.Add(lesson); // Add lesson to the context
                await _context.SaveChangesAsync(); // Save changes
                return RedirectToAction("Index", new { courseId = model.CourseId }); // Redirect to lessons index
            }

            return View(model); // Return the view with the invalid model
        }

    }
}
