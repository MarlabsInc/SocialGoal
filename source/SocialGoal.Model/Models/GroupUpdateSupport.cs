using System;

namespace SocialGoal.Model.Models
{
    public class GroupUpdateSupport
    {
        public int GroupUpdateSupportId { get; set; }

        public int GroupUpdateId { get; set; }

        public int GroupUserId { get; set; }

        public virtual GroupUpdate GroupUpdate { get; set; }

        public DateTime UpdateSupportedDate { get; set; }

    }
}
