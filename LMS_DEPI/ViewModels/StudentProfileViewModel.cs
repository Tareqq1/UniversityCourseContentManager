using LMS_DEPI.Entities.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace LMS_DEPI.APP.ViewModels
{
    public class StudentProfileViewModel
    {
        public IdentityUser IdentityUser { get; set; }  // No need for CustomUser anymore
        public List<Enrollment> EnrolledCourses { get; set; }
        public bool IsEnrolled { get; set; }
    }
}
