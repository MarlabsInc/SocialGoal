
using System;
namespace SocialGoal.Model.Models
{
    public class FollowRequest
    {
        public int FollowRequestId { get; set; }

        public string FromUserId { get; set; }

        public string ToUserId { get; set; }

        public bool Accepted { get; set; }
    }
}
