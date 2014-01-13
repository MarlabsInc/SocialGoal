using System;
using SocialGoal.Model.Models;

namespace SocialGoal.Web.ViewModels
{
    public class CommentsViewModel
    {
        public int CommentId { get; set; }

        public string CommentText { get; set; }

        public int UpdateId { get; set; }

        public string UserId { get; set; }

        public DateTime CommentDate { get; set; }

        public ApplicationUser User { get; set; }
    }
}