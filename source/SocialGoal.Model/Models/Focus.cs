using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SocialGoal.Model.Models
{
    public class Focus
    {
        public int FocusId { get; set; }

        [StringLength(50)]
        public string FocusName { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        public int GroupId { get; set; }

        public virtual Group Group { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual ICollection<GroupGoal> GroupGoals { get; set; }

        public Focus()
        {
            CreatedDate = DateTime.Now;
        }
    }
}
