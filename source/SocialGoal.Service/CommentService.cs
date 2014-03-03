using System.Collections.Generic;
using System.Linq;
using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
using SocialGoal.Data.Repository;

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
        Comment GetComment(int id);
        Comment GEtUpdateByCommentId(int id);
        int GetCommentcount(int id);

        void CreateComment(Comment comment, string userId);
        void DeleteComment(int id);
        void SaveComment();
    }

    public class CommentService : ICommentService
    {
        private readonly ICommentUserRepository _commentUserRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IFollowUserRepository _followUserRepository;
        private readonly IUnitOfWork _unitOfWork;


        public CommentService(ICommentRepository commentRepository,ICommentUserRepository commentUserRepository, IUnitOfWork unitOfWork, IFollowUserRepository followUserRepository)
        {
            _commentRepository = commentRepository;
            _commentUserRepository = commentUserRepository;
            _followUserRepository = followUserRepository;
            _unitOfWork = unitOfWork;
        }

        #region ICommentService Members

        public IEnumerable<Comment> GetComments()
        {
            var comment = _commentRepository.GetAll();
            return comment;
        }
        public IEnumerable<Comment> GetCommentsOfPublicGoals()
        {
            var comment = _commentRepository.GetMany(c => c.Update.Goal.GoalType == false).ToList();
            return comment;
        }
        public IEnumerable<Comment> GetCommentsByUser(string userid)
        {
            var comments = from c in _commentRepository.GetMany(c => c.Update.Goal.GoalType == false).OrderByDescending(c => c.CommentDate)
                          join com in _commentUserRepository.GetMany(cu => cu.UserId == userid) on c.CommentId equals com.CommentId
                          select c;

            return comments;
        }
        public IEnumerable<Comment> GetTop20CommentsOfPublicGoalFollwing(string userId)
        {
            //var comment = from c in commentRepository.GetMany(c => c.Update.Goal.GoalType == false) where (from f in followUserRepository.GetMany(fol => fol.FromUserId == userId) select f.ToUserId).ToList().Contains(c.Update.Goal.UserId) select c;
            var commentUsers=(from f in _followUserRepository.GetMany(fol=>fol.FromUserId==userId)
                              join com in (from cu in _commentUserRepository.GetAll()
                                           select cu) on f.ToUserId equals com.UserId select com).ToList();

            var comments = from c in _commentRepository.GetMany(c => c.Update.Goal.GoalType == false)
                           join com in commentUsers on c.CommentId equals com.CommentId
                           select c;
                           
                         
            return comments;
        }
        public IEnumerable<Comment> GetTop20CommentsOfPublicGoals(IEnumerable<int> commentIds)
        {
            var allComments = new List<Comment>();
            foreach (int item in commentIds)
            {
                var comment = _commentRepository.Get(c => (c.Update.Goal.GoalType == false) && (c.CommentId == item));
                allComments.Add(comment);
            }
           var comments=allComments.Take(20);
            return comments;
        }
        public IEnumerable<Comment> GetTop20CommentsOfPublicGoals(string userid)
        {
            var comment = GetCommentsByUser(userid).Take(20).ToList();
            return comment;
        }

        public IEnumerable<Comment> GetCommentsByUpdate(int updateid)
        {
            var comments = _commentRepository.GetMany(c => c.UpdateId == updateid).ToList();
            return comments;
        }


        public Comment GetComment(int id)
        {
            var comment = _commentRepository.GetById(id);
            return comment;
        }

        public Comment GEtUpdateByCommentId(int id)
        {
            var comment = _commentRepository.Get(u => u.CommentId == id);
            return comment;
        }

        public void CreateComment(Comment comment, string userId)
        {
            _commentRepository.Add(comment);
            SaveComment();
            var commentUser = new CommentUser { UserId = userId, CommentId = comment.CommentId };
            _commentUserRepository.Add(commentUser);
            SaveComment();
        }

        public int GetCommentcount(int id)
        {
            return _commentRepository.GetMany(c => c.UpdateId == id).Count();
        }

        public void DeleteComment(int id)
        {
            var comment = _commentRepository.GetById(id);
            _commentRepository.Delete(comment);
            var commentUser=_commentUserRepository.Get(cu=>cu.CommentId==id);
            _commentUserRepository.Delete(commentUser);
            SaveComment();
        }

        public void SaveComment()
        {
            _unitOfWork.Commit();
        }

        #endregion
    }
}
