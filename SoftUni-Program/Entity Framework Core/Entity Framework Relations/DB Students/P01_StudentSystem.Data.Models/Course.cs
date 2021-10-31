using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Data.Models
{
    public class Course
    {
        public Course()
        {
            Resources = new HashSet<Resource>();
            StudentsEnrolled = new HashSet<StudentCourse>();
            HomeworkSubmissions = new HashSet<Homework>();
        }
        [Key]
        public int CourseId { get; set; }

        [Required]
        [MaxLength(80)]
        public string Name { get; set; }
        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public decimal Price { get; set; }

        public virtual ICollection<Resource> Resources { get; set; }
        public virtual ICollection<StudentCourse> StudentsEnrolled { get; set; }
        public virtual ICollection<Homework> HomeworkSubmissions { get; set; }

    }
}
