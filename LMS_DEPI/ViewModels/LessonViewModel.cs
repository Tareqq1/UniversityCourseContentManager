using System;
using System.ComponentModel.DataAnnotations;
namespace LMS_DEPI.APP.ViewModels

{
    public class LessonViewModel
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string FilePath { get; set; }
        public bool HasResources { get; set; } // New property
    }

}

