using System.Collections.Generic;
using System.Linq;
using SocialGoal.Data;
using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
using SocialGoal.Data.Repository;
using System;

namespace SocialGoal.Service
{

    public interface ICommentService
    {
        IEnumerable<Comment> GetComments();
        IEnumerable<Comment> GetCommentsByUpdate(int updateid);
        IEnumerable<Comment> GetCommentsOfPublicGoals();
        IEnumerable<Comment> GetCommentsByUser(string userid);
        IEnumerable<Comment> GetTop20CommentsOfPublicGoalFollwing(string userId);
        IEnumerable<Comment> GetTop20CommentsOfPublicGoals(string userid);
        //Comment GetLastComment(int userid);
        Comment GetComment(int id);
        Comment GEtUpdateByCommentId(int id);
        int GetCommentcount(int id);

        void CreateComment(Comment comment, string userId);
        void DeleteComment(int id);
        void SaveComment();
    }

    public class CommentService : ICommentService
    {
        private readonly ICommentUserRepository commentUserRepository;
        private readonly ICommentRepository commentRepository;
        private readonly IFollowUserRepository followUserRepository;
        private readonly IUnitOfWork unitOfWork;


        public CommentService(ICommentRepository commentRepository,ICommentUserRepository commentUserRepository, IUnitOfWork unitOfWork, IFollowUserRepository followUserRepository)
        {
            this.commentRepository = commentRepository;
            this.commentUserRepository = commentUserRepository;
            this.followUserRepository = followUserRepository;
            this.unitOfWork = unitOfWork;
        }

        #region ICommentService Members

        public IEnumerable<Comment> GetComments()
        {
            var comment = commentRepository.GetAll();
            return comment;
        }
        public IEnumerable<Comment> GetCommentsOfPublicGoals()
        {
            var comment = commentRepository.GetMany(c => c.Update.Goal.GoalType == false).ToList();
            return comment;
        }
        public IEnumerable<Comment> GetCommentsByUser(string userid)
        {
            var comments = from c in commentRepository.GetMany(c => c.Update.Goal.GoalType == false).OrderByDescending(c => c.CommentDate)
                          join com in commentUserRepository.GetMany(cu => cu.UserId == userid) on c.CommentId equals com.CommentId
                          select c;

            return comments;
        }
        public IEnumerable<Comment> GetTop20CommentsOfPublicGoalFollwing(string userId)
        {
            //var comment = from c in commentRepository.GetMany(c => c.Update.Goal.GoalType == false) where (from f in followUserRepository.GetMany(fol => fol.FromUserId == userId) select f.ToUserId).ToList().Contains(c.Update.Goal.UserId) select c;
            var commentUsers=(from f in followUserRepository.GetMany(fol=>fol.FromUserId==userId)
                              join com in (from cu in commentUserRepository.GetAll()
                                           select cu) on f.ToUserId equals com.UserId select com).ToList();

            var comments = from c in commentRepository.GetMany(c => c.Update.Goal.GoalType == false)
                           join com in commentUsers on c.CommentId equals com.CommentId
                           select c;
                           
                         
            return comments;
        }
        public IEnumerable<Comment> GetTop20CommentsOfPublicGoals(IEnumerable<int> commentIds)
        {
            List<Comment> allComments = new List<Comment> { };
            foreach (int item in commentIds)
            {
                var comment = commentRepository.Get(c => (c.Update.Goal.GoalType == false) && (c.CommentId == item));
                allComments.Add(comment);
            }
           var comments=allComments.Take(20);
            return comments;
        }
        public IEnumerable<Comment> GetTop20CommentsOfPublicGoals(string userid)
        {
            var comment = GetCommentsByUser(userid).Take(20).ToList();
            //var comment = commentRepository.GetMany(c => (c.Update.Goal.GoalType == false) && (c.UserId == userid)).Take(20).ToList();
            return comment;
        }
        //public Comment GetLastComment(int userid)
        //{
        //    var comments = commentRepository.GetMany(c => c.UserId == userid).Last();
        //    return comments;
        //}

        public IEnumerable<Comment> GetCommentsByUpdate(int updateid)
        {
            var comments = commentRepository.GetMany(c => c.UpdateId == updateid).ToList();
            return comments;
        }


        public Comment GetComment(int id)
        {
            var comment = commentRepository.GetById(id);
            return comment;
        }

        public Comment GEtUpdateByCommentId(int id)
        {
            var comment = commentRepository.Get(u => u.CommentId == id);
            return comment;
        }

        public void CreateComment(Comment comment, string userId)
        {
            commentRepository.Add(comment);
            SaveComment();
            var commentUser = new CommentUser { UserId = userId, CommentId = comment.CommentId };
            commentUserRepository.Add(commentUser);
            SaveComment();
        }

        public int GetCommentcount(int id)
        {
            return commentRepository.GetMany(c => c.UpdateId == id).Count();
        }

        public void DeleteComment(int id)
        {
            var comment = commentRepository.GetById(id);
            commentRepository.Delete(comment);
            var commentUser=commentUserRepository.Get(cu=>cu.CommentId==id);
            commentUserRepository.Delete(commentUser);
            SaveComment();
        }

        public void SaveComment()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
