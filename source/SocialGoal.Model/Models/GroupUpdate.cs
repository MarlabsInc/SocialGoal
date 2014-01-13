using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SocialGoal.Model.Models
{
    public class GroupUpdate
    {
        [ScaffoldColumn(false)]
        public int GroupUpdateId { get; set; }

        public string Updatemsg { get; set; }

        public double? status { get; set; }

        public int GroupGoalId { get; set; }

        public DateTime UpdateDate { get; set; }

        public virtual GroupGoal GroupGoal { get; set; }

        public virtual ICollection<GroupComment> GroupComments { get; set; }

        public GroupUpdate()
        {
            UpdateDate = DateTime.Now;

        }

    }
}
