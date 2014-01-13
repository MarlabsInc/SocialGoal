using System;

namespace SocialGoal.Model.Models
{
    public class SupportInvitation
    {
        public int SupportInvitationId { get; set; }

        public string FromUserId { get; set; }

        public int GoalId { get; set; }

        public string ToUserId { get; set; }

        public DateTime SentDate { get; set; }

        public virtual Goal Goal { get; set; }

        public bool Accepted { get; set; }
    }
}
