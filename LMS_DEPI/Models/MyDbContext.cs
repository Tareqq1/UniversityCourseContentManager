using Microsoft.EntityFrameworkCore;
using LMS.Models;
using LMS_DEPI.Entities.Models;
using LMS_DEPI.Models;

namespace LMS.Data
{
    public class LMSContext : DbContext
    {
        public LMSContext(DbContextOptions<LMSContext> options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CourseResource> CourseResources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Enrollment>()
               .HasKey(e => new { e.CourseId, e.StudentId });

            modelBuilder.Entity<Enrollment>()
               .HasOne(e => e.Course)
               .WithMany(c => c.Enrollments)
               .HasForeignKey(e => e.CourseId);

            modelBuilder.Entity<Enrollment>()
               .HasOne(e => e.Student)
               .WithMany(s => s.Enrollments)
               .HasForeignKey(e => e.StudentId);

            modelBuilder.Entity<Quiz>()
               .HasOne(q => q.Lesson)
               .WithMany(l => l.Quizzes)
               .HasForeignKey(q => q.LessonId);

            modelBuilder.Entity<Question>()
               .HasOne(q => q.Quiz)
               .WithMany(q => q.Questions)
               .HasForeignKey(q => q.QuizId);

            modelBuilder.Entity<Answer>()
               .HasOne(a => a.Question)
               .WithMany(q => q.Answers)
               .HasForeignKey(a => a.QuestionId);

            // Add this part for the Course and User relationship
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Teacher) // Navigation property
                .WithMany() // Assuming a Teacher can have many Courses
                .HasForeignKey(c => c.TeacherId); // Foreign key property
        }
    }
}