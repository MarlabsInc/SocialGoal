using System.Collections.Generic;

namespace SocialGoal.Model.Models
{
    public class GoalStatus
    {
        public int GoalStatusId { get; set; }

        public string GoalStatusType { get; set; }

        public virtual ICollection<Goal> Goals { get; set; }

        public virtual ICollection<GroupGoal> GroupGoals { get; set; }

    }
}
