using Microsoft.AspNetCore.Mvc;
using LMS.Models;
using LMS.Data;
using LMS_DEPI.Entities.Models;
using Microsoft.EntityFrameworkCore;
using LMS_DEPI.APP.ViewModels;

namespace LMS.Controllers
{
    public class CoursesController : Controller
    {
        private readonly LMSContext _context;

        public CoursesController(LMSContext context)
        {
            _context = context;
        }

        // GET: Courses
        public IActionResult Index()
        {
            var courses = _context.Courses.ToList(); // Assuming _context is your DbContext
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
        public IActionResult Create()
        {
            var model = new CourseViewModel
            {
                StartDate = DateTime.Now, // Set StartDate to the current date
                EndDate = DateTime.Now.AddDays(30) // Set EndDate to 30 days from now
            };

            return View(model);
        }



        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                    TeacherName = User.Identity.Name // Set the TeacherName from the logged-in user
                };

                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model); // Return the view with the invalid model
        }



        // GET: Courses/Edit/5
        public IActionResult Edit(int? id)
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

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Title,Description,StartDate,EndDate,Credits")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        // GET: Courses/Delete/5
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

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var course = _context.Courses.Find(id);
            _context.Courses.Remove(course);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
        public IActionResult Lessons(int id)
        {
            // Retrieve lessons for the given course ID
            var lessons = _context.Lessons.Where(l => l.CourseId == id).ToList();
            ViewBag.CourseId = id; // Pass the Course ID to the view
            return View(lessons); // Return the lessons to the view
        }
    }
}
