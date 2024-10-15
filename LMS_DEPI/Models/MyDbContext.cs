using Microsoft.EntityFrameworkCore;
using LMS.Models; // Ensure this namespace contains all your models
using LMS_DEPI.Entities.Models; // Ensure this namespace contains CourseResource, Lesson, etc.
using LMS_DEPI.Models; // Ensure this namespace contains other related models

namespace LMS.Data
{
    public class LMSContext : DbContext
    {
        public LMSContext(DbContextOptions<LMSContext> options) : base(options)
        {
            // You may uncomment the line below to specify a migration command context
            // Add-Migration InitialCreate -Context LMSContext -OutputDir Migrations\SqliteMigrations
        }

        // DbSets for your entities
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
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict); // Disable cascading delete

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict); // Disable cascading delete

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Quiz)
                .WithMany(q => q.Questions)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Restrict); // Disable cascading delete

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict); // Disable cascading delete

            modelBuilder.Entity<CourseResource>()
                .HasOne(cr => cr.Lesson)
                .WithMany(l => l.CourseResources)
                .HasForeignKey(cr => cr.LessonId)
                .OnDelete(DeleteBehavior.Restrict); // Disable cascading delete

            modelBuilder.Entity<CourseResource>()
                .HasOne(cr => cr.Course)
                .WithMany(c => c.CourseResources) // Assuming you have a collection of CourseResources in Course
                .HasForeignKey(cr => cr.CourseId)
                .OnDelete(DeleteBehavior.Restrict); // Disable cascading delete

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Teacher) // Navigation property
                .WithMany() // Assuming a Teacher can have many Courses
                .HasForeignKey(c => c.TeacherId) // Foreign key property
                .OnDelete(DeleteBehavior.Restrict); // Disable cascading delete
        }

    }
}
