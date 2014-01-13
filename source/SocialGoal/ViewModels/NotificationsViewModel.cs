using System;
using SocialGoal.Model.Models;

namespace SocialGoal.Web.ViewModels
{
    public class NotificationsViewModel
    {
        //public NotificationsViewModel()
        //{
        //    if (CreatedDate == DateTime.Now)
        //    {
        //        NotificationDate = "Today";
        //    }
        //    else if (CreatedDate == System.DateTime.Today.AddDays(-1))
        //    {
        //        NotificationDate = "Yesterday";
        //    }
        //    else{
        //        NotificationDate = CreatedDate.ToShortDateString();
        //    }

       // }
        public int NotificationId { get; set; }
        public int NotificationType { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ProfilePicUrl { get; set; }

        public int NumberOfComments { get; set; }
        
        public int? UpdateId { get; set; }
        public string Updatemsg { get; set; }
        public float? status { get; set; }
        public DateTime UpdateDate { get; set; }
        public virtual Goal Goal { get; set; }
        
        public int? CommentId { get; set; }
        public string CommentText { get; set; }
        public DateTime CommentDate { get; set; }
        public virtual Update Update { get; set; }

        public int? GoalId { get; set; }
        public string GoalName { set; get; }
        public string Desc { get; set; }
        public float? Target { get; set; }
        public int MetricId { get; set; }
        public int? FocusId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Metric Metric { get; set; }
        public virtual Focus Focus { get; set; }

        public int? GroupId { get; set; }
        public string GroupName { get; set; }

        public int? GroupUserId { get; set; }
        public virtual GroupUser GroupUser { get; set; }

        public int? SupportId { get; set; }
        public Support Support { get; set; }

        public int? GroupGoalId { get; set; }
        public string GroupGoalName { get; set; }
        public string GoalDesc { get; set; }
        public virtual GroupGoal GroupGoal { get; set; }
       
        //public bool SupportLink { get; set; }
        public bool IsAMember { get; set; }

        public int? GroupUpdateId { get; set; }
        public string GroupUpdatemsg { get; set; }
        public float? Groupstatus { get; set; }
        public DateTime GroupUpdateDate { get; set; }


        public int? GroupCommentId { get; set; }
        public string GroupCommentText { get; set; }
        public DateTime GroupCommentDate { get; set; }
        public virtual GroupUpdate GroupUpdate { get; set; }

        public int? FollowUserId { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public virtual ApplicationUser FromUser { get; set; }
        public virtual ApplicationUser ToUser { get; set; }

        public string NotificationDate { get; set; }
   
    }
}