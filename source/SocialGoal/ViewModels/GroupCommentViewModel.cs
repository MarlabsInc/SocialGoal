using System;
using SocialGoal.Model.Models;

namespace SocialGoal.Web.ViewModels
{
    public class GroupCommentsViewModel
    {
        public int GroupCommentId { get; set; }

        public string CommentText { get; set; }

        public int GroupUpdateId { get; set; }

        public string UserId { get; set; }

        public DateTime CommentDate { get; set; }

        public ApplicationUser User { get; set; }
    }
}