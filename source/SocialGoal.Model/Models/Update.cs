using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SocialGoal.Model.Models
{
    public class Update
    {
        [ScaffoldColumn(false)]
        public int UpdateId { get; set; }

        public string Updatemsg { get; set; }

        public double? status { get; set; }

        public int GoalId { get; set; }

        public DateTime UpdateDate { get; set; }

        public virtual Goal Goal { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<UpdateSupport> UpdateSupports { get; set; }

        public Update()
        {
            UpdateDate = DateTime.Now;

        }

    }
}
