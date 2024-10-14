using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LMS_DEPI.Entities.Models
{
    public class Enrollment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int CourseId { get; set; }
        public Course Course { get; set; }
        [Required]
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public DateTime EnrolledAt { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}