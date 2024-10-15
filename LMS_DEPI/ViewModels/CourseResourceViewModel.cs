namespace LMS_DEPI.APP.ViewModels
{
    public class CourseResourceViewModel
    {
        public int LessonId { get; set; }
        public int CourseId { get; set; } // Add CourseId
        public string ResourceType { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
