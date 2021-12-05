using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Theatre.Data.Models
{
    public class Theatre
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [Range(1,10)]
        public sbyte NumberOfHalls { get; set; }

        [Required]
        [MaxLength(30)]
        public string Director { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
        public Theatre()
        {
            this.Tickets = new List<Ticket>();
        }
    }

}
