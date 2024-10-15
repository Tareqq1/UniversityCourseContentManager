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
        public IActionResult Index(int lessonId)
        {
            var resources = _context.CourseResources.Where(cr => cr.LessonId == lessonId).ToList();
            ViewBag.LessonId = lessonId; // Pass the lessonId to the view
            return View(resources);
        }

        // GET: CourseResources/Create
        public IActionResult Create(int lessonId, int courseId)
        {
            var viewModel = new CourseResourceViewModel
            {
                LessonId = lessonId,
                CourseId = courseId // Set the CourseId in the ViewModel
            };
            return View(viewModel);
        }

        // POST: CourseResources/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CourseResourceViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Check if the CourseId exists
                var courseExists = _context.Courses.Any(c => c.Id == viewModel.CourseId);
                if (!courseExists)
                {
                    ModelState.AddModelError("CourseId", "The selected course does not exist.");
                    return View(viewModel);
                }

                var courseResource = new CourseResource
                {
                    LessonId = viewModel.LessonId,
                    CourseId = viewModel.CourseId, // Ensure CourseId is set
                    ResourceType = viewModel.ResourceType,
                    FileName = viewModel.FileName,
                    FilePath = viewModel.FilePath
                };

                _context.CourseResources.Add(courseResource);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index), new { lessonId = viewModel.LessonId });
            }
            return View(viewModel);
        }

        // GET: CourseResources/Delete/5
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
        public IActionResult DeleteConfirmed(int id)
        {
            var courseResource = _context.CourseResources.Find(id);
            _context.CourseResources.Remove(courseResource);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
