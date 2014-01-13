using System;

namespace SocialGoal.Model.Models
{
    public class GroupInvitation
    {
        public int GroupInvitationId { get; set; }

        public string FromUserId { get; set; }

        public int GroupId { get; set; }

        public string ToUserId { get; set; }

        public DateTime SentDate { get; set; }

        public virtual Group Group { get; set; }

        public bool Accepted { get; set; }
    }
}
