using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LMS_DEPI.Entities.Models
{
    public class Lesson
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }
        public Course Course { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime ReleaseDate { get; set; } = DateTime.Now; // Set default value to now

        [Required(ErrorMessage = "Due date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; } // Keep this as required if you want to enforce user input

        public ICollection<Quiz> Quizzes { get; set; }
        public string FilePath { get; set; }  // New property for PDF path
    }
}
