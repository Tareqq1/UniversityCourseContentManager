using LMS_DEPI.Entities.Models;

namespace LMS_DEPI.APP.ViewModels
{
    public class ManageStudentsViewModel
    {
        public Course Course { get; set; }
        public List<Enrollment> Enrollments { get; set; }
    }

}
