using System;
using SocialGoal.Model.Models;

namespace SocialGoal.Web.ViewModels
{
    public class NotificationViewModel
    {
        public int GroupInvitationId { get; set; }

        public int SupportInvitationId { get; set; }

        public string FromUserId { get; set; }

        public int GroupId { get; set; }

        public string ToUserId { get; set; }

        public DateTime SentDate { get; set; }

        public ApplicationUser FromUser { get; set; }

        public ApplicationUser ToUser { get; set; }

        public virtual Group Group { get; set; }

        public bool Accepted { get; set; }

        public int GoalId { get; set; }

        public virtual Goal Goal { get; set; }
    }
}