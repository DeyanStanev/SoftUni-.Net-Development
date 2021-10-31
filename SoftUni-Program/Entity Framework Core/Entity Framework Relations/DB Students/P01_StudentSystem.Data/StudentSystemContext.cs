using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;
using System;

namespace P01_StudentSystem.Data
{
    public class StudentSystemContext: DbContext
    {
        public StudentSystemContext()
        {

        }
        public StudentSystemContext(DbContextOptions options)
            :base(options)
        {
            
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<StudentCourse>().HasKey(k => new { k.StudentId, k.CourseId });
            modelBuilder.Entity<Student>().Property(p => p.PhoneNumber).IsUnicode(false);
            modelBuilder.Entity<Resource>().Property(p => p.Url).IsUnicode(false);
            modelBuilder.Entity<Homework>().Property(p => p.Content).IsUnicode(false);

        }
        public DbSet<Course> Courses { get; set; }

        public DbSet<Homework> HomeworkSubmissions{ get; set; }

        public DbSet<Resource> Resources { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<StudentCourse> StudentCourses { get; set; }

    }
}
