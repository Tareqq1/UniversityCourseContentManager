using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LMS_DEPI.Entities.Models
{   public class Quiz
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}