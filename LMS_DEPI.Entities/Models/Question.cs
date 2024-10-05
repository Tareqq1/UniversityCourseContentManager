using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using LMS_DEPI.Entities.Models;

namespace LMS.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }
        [Required]
        public string Text { get; set; }
        public string Type { get; set; } 

        public ICollection<Answer> Answers { get; set; }
    }
}