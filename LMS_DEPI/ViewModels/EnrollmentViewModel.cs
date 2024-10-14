using LMS_DEPI.Entities.Models;

namespace LMS_DEPI.APP.ViewModels
{
    public class EnrollmentViewModel
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public DateTime EnrolledAt { get; set; }
        public DateTime CompletedAt { get; set; }
        public int Grade { get; set; }

        public EnrollmentViewModel() { }

        public EnrollmentViewModel(Enrollment enrollment)
        {
            Id = enrollment.Id;
            CourseId = enrollment.CourseId;
            StudentId = enrollment.StudentId;
            EnrolledAt = enrollment.EnrolledAt;
            CompletedAt = enrollment.CompletedAt;
        }
    }
}
