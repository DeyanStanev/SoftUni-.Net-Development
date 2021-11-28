using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeisterMask.Data.Models
{
    public class Employee
    {
        public Employee()
        {
            this.EmployeesTasks = new List<EmployeeTask>();
        }

     
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string Username { get; set; }

        
        [Required]
        [MaxLength(320)]
        public string Email { get; set; }

        [Required]
        [MaxLength(15)]
        public string  Phone { get; set; }

        public virtual ICollection<EmployeeTask> EmployeesTasks { get; set; }

    }
}
