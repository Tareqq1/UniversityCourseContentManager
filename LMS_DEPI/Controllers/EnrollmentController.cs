using LMS.Data;
using LMS_DEPI.APP.ViewModels;
using LMS_DEPI.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
namespace LMS_DEPI.Controllers
{
    public class EnrollmentController : Controller
    {
        private LMSContext _context;

        public EnrollmentController(LMSContext context)
        {
            _context = context;
        }

        // GET: Enrollment
        public ActionResult Index()
        {
            var enrollments = _context.Enrollments.ToList();
            return View(enrollments);
        }

        // GET: Enrollment/Details/5
        public ActionResult Details(int id)
        {
            var enrollment = _context.Enrollments
   .Include(e => e.Course)
   .Include(e => e.Student)
   .FirstOrDefault(e => e.Id == id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        private ActionResult HttpNotFound()
        {
            return NotFound();
        }

        // GET: Enrollment/Create
        public ActionResult Create()
        {
            ViewBag.CourseId = new SelectList(_context.Courses, "Id", "Title");
            ViewBag.StudentId = new SelectList(_context.Students, "Id", "Name");
            return View();
        }

        // POST: Enrollment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind] EnrollmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var enrollment = new Enrollment
                {
                    CourseId = model.CourseId,                
                    StudentId = model.StudentId,         
                    EnrolledAt = model.EnrolledAt,
                    CompletedAt = model.CompletedAt,
                    Grade = model.Grade
                };

                _context.Enrollments.Add(enrollment);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseId = new SelectList(_context.Courses, "Id", "Title", model.CourseId);
            ViewBag.StudentId = new SelectList(_context.Students, "Id", "Name", model.StudentId);
            return View(model);
        }

        // GET: Enrollment/Edit/5
        public ActionResult Edit(int id)
        {
            var enrollment = _context.Enrollments
            .Include(e => e.Course)
            .Include(e => e.Student)
            .FirstOrDefault(e => e.Id == id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(_context.Courses, "Id", "Title", enrollment.CourseId);
            ViewBag.StudentId = new SelectList(_context.Students, "Id", "Name", enrollment.StudentId);
            return View(new EnrollmentViewModel(enrollment));
        }

        // POST: Enrollment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind] EnrollmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var enrollment = _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefault(e => e.Id == model.Id);
                if (enrollment != null)
                {
                    enrollment.CourseId = model.CourseId;
                    enrollment.StudentId = model.StudentId;
                    enrollment.EnrolledAt = model.EnrolledAt;
                    enrollment.CompletedAt = model.CompletedAt;
                    enrollment.Grade = model.Grade;

                    _context.Entry(enrollment).State = EntityState.Modified;
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.CourseId = new SelectList(_context.Courses, "Id", "Title", model.CourseId);
            ViewBag.StudentId = new SelectList(_context.Students, "Id", "Name", model.StudentId);
            return View(model);
        }


        // GET: Enrollment/Delete/5
        public ActionResult Delete(int id)
        {
            var enrollment = _context.Enrollments
    .Include(e => e.Course)
    .Include(e => e.Student)
    .FirstOrDefault(e => e.Id == id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        // POST: Enrollment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var enrollment = _context.Enrollments
    .Include(e => e.Course)
    .Include(e => e.Student)
    .FirstOrDefault(e => e.Id == id);
            _context.Enrollments.Remove(enrollment);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}