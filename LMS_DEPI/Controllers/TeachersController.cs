using Microsoft.AspNetCore.Mvc;
using LMS_DEPI.Entities.Models;
using LMS.Data;
using LMS_DEPI.APP.ViewModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace LMS_DEPI.APP.Controllers
{
    [Authorize(Roles = "Teacher")]

    public class TeacherController : Controller
    {
        private readonly LMSContext _context;

        public TeacherController(LMSContext context)
        {
            _context = context;
        }

        // Action to view courses taught by the teacher
        public IActionResult MyCourses(int? teacherId)
        {
            // If teacherId is null, show a message to enter the ID
            if (teacherId == null)
            {
                ViewBag.Message = "Please enter a Teacher ID to view courses.";
                return View(new List<Course>()); // Empty list initially
            }

            // Log the entered Teacher ID
            Console.WriteLine($"Entered Teacher ID: {teacherId}");

            // Retrieve courses associated with the entered teacher ID
            var courses = _context.Courses
                .Where(c => c.TeacherId == teacherId)
                .ToList();

            // Log the number of courses found
            Console.WriteLine($"Courses Retrieved: {courses.Count}");

            foreach (var course in courses)
            {
                Console.WriteLine($"Course ID: {course.Id}, Title: {course.Title}, Teacher ID: {course.TeacherId}");
            }

            // Pass the retrieved courses to the view
            return View(courses);
        
  return View(courses);
        }





        // Action to view students in a specific course and assign grades
        public IActionResult ManageStudents(int courseId)
        {
            // Find the course with the associated enrollments and students
            var course = _context.Courses
                .Include(c => c.Enrollments) // Include enrollments to access students
                .ThenInclude(e => e.Student) // Assuming Enrollment has a Student property
                .FirstOrDefault(c => c.Id == courseId);

            if (course == null)
            {
                return NotFound(); // Return 404 if the course is not found
            }

            // Create a view model to pass the course and its enrollments to the view
            var model = new ManageStudentsViewModel
            {
                Course = course,
                Enrollments = course.Enrollments.ToList() // List of students enrolled
            };

            return View(model); // Pass the model to the view
        }


        // Action to update grades
        [HttpPost]
        public IActionResult UpdateGrades(int courseId, List<Enrollment> enrollments)
        {
            foreach (var enrollment in enrollments)
            {
                var existingEnrollment = _context.Enrollments.Find(enrollment.Id);
                if (existingEnrollment != null)
                {
                    existingEnrollment.Grade = enrollment.Grade;  // Update the grade
                }
            }
            _context.SaveChanges();
            return RedirectToAction("ManageStudents", new { courseId });
        }

        // Action to add an extra lesson
        public IActionResult AddExtraLesson(int courseId)
        {
            var course = _context.Courses.Find(courseId);
            if (course == null)
            {
                return NotFound(); // Return 404 if the course is not found
            }
            return View(new Lesson { CourseId = courseId }); // Pass a new Lesson with the CourseId
        }

        [HttpPost]
        public IActionResult AddExtraLesson(Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                _context.Lessons.Add(lesson);
                _context.SaveChanges();
                return RedirectToAction("MyCourses");
            }
            return View(lesson); // Return to the view with the lesson data for corrections
        }

        private int GetCurrentUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine($"Retrieved User ID: {userIdString}"); // Debug output
            return Convert.ToInt32(userIdString);
        }

    }
}
