using Microsoft.AspNetCore.Mvc;
using LMS.Models;
using LMS.Data;
using LMS_DEPI.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS_DEPI.Controllers
{
    public class CourseResourcesController : Controller
    {
        private readonly LMSContext _context;

        public CourseResourcesController(LMSContext context)
        {
            _context = context;
        }

        // GET: CourseResources
        public IActionResult Index(int courseId)
        {
            var course = _context.Courses.Find(courseId);
            var resources = _context.CourseResources.Where(cr => cr.CourseId == courseId).ToList();
            return View(resources);
        }

        // GET: CourseResources/Create
        public IActionResult Create(int courseId)
        {
            return View();
        }

        // POST: CourseResources/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int courseId, [Bind("ResourceType,FileName,FilePath")] CourseResource courseResource)
        {
            if (ModelState.IsValid)
            {
                courseResource.CourseId = courseId;
                _context.CourseResources.Add(courseResource);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index), new { courseId });
            }
            return View(courseResource);
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

        public IActionResult ViewResource(int id)
        {
            var courseResource = _context.CourseResources.Find(id);
            if (courseResource.ResourceType == null)
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