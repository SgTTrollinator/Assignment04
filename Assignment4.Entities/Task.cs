using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Assignment4.Core;
using System;

namespace Assignment4.Entities
{
    public class Task
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }
        public User AssignedTo { get; set; }
        public string Description { get; set; }

        [Required]
        public State State { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public DateTime Created { get; set; }
        public DateTime StatusUpdated { get; set; }

    }
}
