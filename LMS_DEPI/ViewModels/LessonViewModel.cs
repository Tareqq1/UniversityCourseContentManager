using System;
using System.ComponentModel.DataAnnotations;

namespace LMS_DEPI.APP.ViewModels
{
    public class LessonViewModel
    {
        public int CourseId { get; set; } // To bind to the CourseId in the hidden input

        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Due date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; } = DateTime.Now; // Set default to current date

        public string FilePath { get; set; }  // Property for the file path
    }
}
