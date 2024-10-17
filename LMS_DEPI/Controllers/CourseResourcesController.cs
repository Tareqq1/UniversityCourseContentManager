using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LMS.Models;
using LMS.Data;
using LMS_DEPI.Entities.Models;
using Microsoft.EntityFrameworkCore;
using LMS_DEPI.APP.ViewModels;

namespace LMS_DEPI.Controllers
{
    [Authorize] // Restrict access to authenticated users
    public class CourseResourcesController : Controller
    {
        private readonly LMSContext _context;

        public CourseResourcesController(LMSContext context)
        {
            _context = context;
        }

        // GET: CourseResources
        public IActionResult Index(int courseId, int lessonId)
        {
            // Fetch resources where both CourseId and LessonId match
            var resources = _context.CourseResources
                                    .Where(cr => cr.CourseId == courseId && cr.LessonId == lessonId)
                                    .ToList();

            ViewBag.LessonId = lessonId;
            ViewBag.CourseId = courseId; // Pass CourseId to the view as well

            return View(resources);
        }


        // GET: CourseResources/Create
        [Authorize(Roles = "Teacher")]
        public IActionResult Create(int lessonId, int courseId)
        {
            var viewModel = new CourseResourceViewModel
            {
                LessonId = lessonId,
                CourseId = courseId // Pass both LessonId and CourseId to the view
            };
            return View(viewModel);
        }


        // POST: CourseResources/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public IActionResult Create(CourseResourceViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Ensure CourseId and LessonId are valid
                var courseExists = _context.Courses.Any(c => c.Id == viewModel.CourseId);
                var lessonExists = _context.Lessons.Any(l => l.Id == viewModel.LessonId);

                if (!courseExists || !lessonExists)
                {
                    ModelState.AddModelError("", "The selected course or lesson does not exist.");
                    return View(viewModel);
                }

                var courseResource = new CourseResource
                {
                    LessonId = viewModel.LessonId,
                    CourseId = viewModel.CourseId,
                    ResourceType = viewModel.ResourceType,
                    FileName = viewModel.FileName,
                    FilePath = viewModel.FilePath
                };

                _context.CourseResources.Add(courseResource);
                _context.SaveChanges();

                // Redirect back to Index with both CourseId and LessonId
                return RedirectToAction(nameof(Index), new { courseId = viewModel.CourseId, lessonId = viewModel.LessonId });
            }

            return View(viewModel);
        }


        // GET: CourseResources/Delete/5
        [Authorize(Roles = "Teacher")]
        public IActionResult Delete(int id)
        {
            var courseResource = _context.CourseResources.Find(id);
            if (courseResource == null)
            {
                return NotFound();
            }
            return View(courseResource);
        }

        // GET: CourseResources/ViewResource/5
        public IActionResult ViewResource(int id)
        {
            var courseResource = _context.CourseResources.Find(id);
            if (courseResource == null)
            {
                return NotFound();
            }

            // Return the view with the course resource details
            return View(courseResource);
        }

        // POST: CourseResources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public IActionResult DeleteConfirmed(int id)
        {
            var courseResource = _context.CourseResources.Find(id);
            if (courseResource != null)
            {
                _context.CourseResources.Remove(courseResource);
                _context.SaveChanges();

                // Add a success message
                TempData["SuccessMessage"] = "Course resource deleted successfully.";
            }

            return RedirectToAction(nameof(Index), new { courseId = courseResource.CourseId, lessonId = courseResource.LessonId });
        }

    }
}
