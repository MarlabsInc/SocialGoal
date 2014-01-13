using System;

namespace SocialGoal.Model.Models
{
    public class GroupComment
    {
        public int GroupCommentId { get; set; }

        public string CommentText { get; set; }

        public int GroupUpdateId { get; set; }

        public DateTime CommentDate { get; set; }

        public virtual GroupUpdate GroupUpdate { get; set; }

        public GroupComment()
        {
            CommentDate = DateTime.Now;
        }
    }
}
