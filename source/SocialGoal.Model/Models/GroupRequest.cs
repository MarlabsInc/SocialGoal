
using System;
namespace SocialGoal.Model.Models
{
    public class GroupRequest
    {
        public int GroupRequestId { get; set; }

        public int GroupId { get; set; }

        public string UserId { get; set; }

        public virtual Group Group { get; set; }

        public virtual ApplicationUser User { get; set; }

        public bool Accepted { get; set; }
    }
}
