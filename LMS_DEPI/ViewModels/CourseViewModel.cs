using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LMS_DEPI.APP.ViewModels
{
    public class CourseViewModel
    {
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        public string Description { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Start Date is required.")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "End Date is required.")]
        public DateTime EndDate { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Credits must be a non-negative number.")]
        public int Credits { get; set; }

    }
}
