﻿using LMS_DEPI.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace LMS_DEPI.Entities.Models
{
    public class CourseResource
    {
        [Key]
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string ResourceType { get; set; } // e.g. video, document
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public Course Course { get; set; }
    }
}