using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace SocialGoal.Model.Models
{
    public class Goal
    {
        public int GoalId { get; set; }

        [StringLength(50)]
        public string GoalName { set; get; }

        [StringLength(100)]
        public string Desc { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime EndDate { get; set; }

        public double? Target { get; set; }

        public bool GoalType { get; set; }

        public int? MetricId { get; set; }

        public int GoalStatusId { get; set; }

        public string UserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual Metric Metric { get; set; }

        public virtual GoalStatus GoalStatus { get; set; }

        public virtual ICollection<Support> Supports { get; set; }

        public virtual ICollection<Update> Updates { get; set; }

        public virtual ICollection<SupportInvitation> SupportInvitations { get; set; }


        public Goal()
        {
            CreatedDate = DateTime.Now;
            GoalStatusId = 1;
        }
    }
}
