using System;
using System.Collections.Generic;

namespace SocialGoal.Model.Models
{
    public class GroupUser
    {
        public GroupUser()
        {
            AddedDate = DateTime.Now;
        }
        public int GroupUserId { get; set; }

        public int GroupId { get; set; }

        public string UserId { get; set; }

        public bool Admin { get; set; }

        public DateTime AddedDate { get; set; }

        public virtual ICollection<GroupGoal> GroupGoals { get; set; }
    }
}
