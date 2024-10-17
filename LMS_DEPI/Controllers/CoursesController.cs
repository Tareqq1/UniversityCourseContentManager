using Microsoft.AspNetCore.Mvc;
using LMS.Models;
using LMS.Data;
using LMS_DEPI.Entities.Models;
using Microsoft.EntityFrameworkCore;
using LMS_DEPI.APP.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace LMS.Controllers
{
    public class CoursesController : Controller
    {
        private readonly LMSContext _context;

        public CoursesController(LMSContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Teacher")]

        public IActionResult Index()
        {
            var courses = _context.Courses.ToList();
            return View(courses);
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Lessons)
                .Include(c => c.Quizzes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        [Authorize(Roles = "Teacher")]
        public IActionResult Create()
        {
            var model = new CourseViewModel
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30)
            };

            return View(model);
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create(CourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                var course = new Course
                {
                    Title = model.Title,
                    Description = model.Description,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Credits = model.Credits,
                    TeacherName = User.Identity.Name
                };

                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }


        // GET: Courses/Delete/5
        [Authorize(Roles = "Teacher")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = _context.Courses.Find(id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                try
                {
                    // Remove related lessons first
                    var lessons = _context.Lessons.Where(l => l.CourseId == id).ToList();
                    if (lessons.Any())
                    {
                        _context.Lessons.RemoveRange(lessons);
                    }

                    _context.Courses.Remove(course);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log the exception (consider using a logging library)
                    ModelState.AddModelError("", "An error occurred while deleting the course: " + ex.Message);
                    return View(course);
                }
            }

            ModelState.AddModelError("", "Course not found.");
            return RedirectToAction(nameof(Index));
        }


        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }

        public IActionResult Lessons(int id)
        {
            var lessons = _context.Lessons.Where(l => l.CourseId == id).ToList();
            ViewBag.CourseId = id;
            return View(lessons);
        }
    }
}
