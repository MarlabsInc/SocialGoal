using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SocialGoal.Model.Models
{
    public class GroupGoal
    {
        public int GroupGoalId { get; set; }

        public string GoalName { set; get; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public double? Target { get; set; }

        public int? MetricId { get; set; }

        public int? FocusId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int GoalStatusId { get; set; }

        public int GroupUserId { get; set; }

        public int? AssignedGroupUserId { get; set; }

        public string AssignedTo { get; set; }

        public virtual GroupUser GroupUser { get; set; }

        public int GroupId { get; set; }

        public virtual Group Group { get; set; }


        public virtual Metric Metric { get; set; }

        public virtual Focus Focus { get; set; }

        public virtual GoalStatus GoalStatus { get; set; }

        public virtual ICollection<GroupUpdate> Updates { get; set; }

        public GroupGoal()
        {
            CreatedDate = DateTime.Now;
            GoalStatusId = 1;
        }
    }
}
