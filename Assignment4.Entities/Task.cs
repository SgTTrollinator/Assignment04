using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Assignment4.Entities
{
    public class Task
    {
        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }
        public User AssignedTo { get; set; }
        public string Description { get; set; }

        [Required]
        public State State { get; set; }
        public ICollection<Tag> Tags { get; set; }
    }
}

public enum State
{
    New,
    Active,
    Resolved,
    Closed,
    Removed
}
