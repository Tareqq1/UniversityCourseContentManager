using Microsoft.AspNetCore.Mvc;
using LMS.Models;
using LMS.Data;
using LMS_DEPI.Entities.Models;
using Microsoft.EntityFrameworkCore;

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
            return View(_context.Courses.ToList());
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
            return View();
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Title,Description,StartDate,EndDate,Credits")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Courses.Add(course);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(course);
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
    }
}