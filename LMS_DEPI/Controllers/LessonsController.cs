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
                .Select(l => new LessonViewModel
                {
                    Id = l.Id,
                    CourseId = l.CourseId,
                    Title = l.Title,
                    Description = l.Description,
                    DueDate = l.DueDate,
                    FilePath = l.FilePath,
                    HasResources = _context.CourseResources.Any(cr => cr.LessonId == l.Id) // Check if there are resources for the lesson
                })
                .ToListAsync();

            var course = await _context.Courses.FindAsync(courseId);
            ViewBag.CourseName = course?.Title ?? "Unknown Course";
            ViewBag.CourseId = courseId;
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

        // GET: Lessons/Edit/5
        [Authorize(Roles = "Teacher")] // Only allow teachers to edit lessons
        public async Task<IActionResult> Edit(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null)
            {
                return NotFound();
            }

            var lessonViewModel = new LessonViewModel
            {
                Id = lesson.Id,
                CourseId = lesson.CourseId,
                Title = lesson.Title,
                Description = lesson.Description,
                DueDate = lesson.DueDate,
                FilePath = lesson.FilePath
            };

            return View(lessonViewModel); // Pass the lesson data to the view
        }

        // POST: Lessons/Edit/5
        [HttpPost]
        [Authorize(Roles = "Teacher")] // Only allow teachers to edit lessons
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LessonViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var lesson = await _context.Lessons.FindAsync(id);
                if (lesson == null)
                {
                    return NotFound();
                }

                // Update lesson properties with data from the model
                lesson.Title = model.Title;
                lesson.Description = model.Description;
                lesson.DueDate = model.DueDate;
                lesson.FilePath = model.FilePath;

                try
                {
                    _context.Lessons.Update(lesson);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LessonExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("Index", new { courseId = model.CourseId });
            }

            return View(model); // Return the view with validation errors, if any
        }

        // GET: Lessons/Delete/5
        [Authorize(Roles = "Teacher")] // Only allow teachers to delete lessons
        public async Task<IActionResult> Delete(int id)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Course) // Optionally include course information
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson); // Show the delete confirmation page
        }

        // POST: Lessons/Delete/5
        [HttpPost, ActionName("Delete")] // This tells the form to post to this method, but keep the action name as "Delete"
        [Authorize(Roles = "Teacher")] // Only allow teachers to delete lessons
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson != null)
            {
                _context.Lessons.Remove(lesson);
                await _context.SaveChangesAsync();
            }

            // Redirect to the list of lessons for the course
            return RedirectToAction("Index", new { courseId = lesson.CourseId });
        }




        private bool LessonExists(int id)
        {
            return _context.Lessons.Any(e => e.Id == id);
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
