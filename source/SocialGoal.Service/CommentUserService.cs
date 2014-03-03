using System.Collections.Generic;
using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
using SocialGoal.Data.Repository;

namespace SocialGoal.Service
{

    public interface ICommentUserService
    {

        IEnumerable<int> GetCommentIdsByUser(string userId);
        ApplicationUser GetUser(int commentId);
        void CreateCommentUser(string userId, int commentId);
        void DeleteCommentUser(string userId, int commentId);

        void DeleteCommentUser(int id);
        void SaveCommentUser();
       
    }

    public class CommentUserService : ICommentUserService
    {
        private readonly ICommentUserRepository _commentUserRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CommentUserService(ICommentUserRepository commentUserRepository,IUserRepository userRepository,IUnitOfWork unitOfWork)
        {
            _commentUserRepository = commentUserRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;

        }
       
        #region ICommentUserService Members

        public ApplicationUser GetUser(int commentId)
        {
            var userId = _commentUserRepository.Get(cu => cu.CommentId == commentId).UserId;
            return _userRepository.GetById(userId);
        }

        public IEnumerable<int> GetCommentIdsByUser(string userId)
        {
            List<int> commentIds = new List<int>();
            var commentUsers = _commentUserRepository.GetMany(c => c.UserId == userId);
            foreach (var item in commentUsers)
            {
                commentIds.Add(item.CommentId);
            }
            return commentIds;

        }

        public void DeleteCommentUser(string userId, int commentId)
        {
            var commentUser = _commentUserRepository.Get(cu => cu.UserId == userId && cu.CommentId == commentId);
            _commentUserRepository.Delete(commentUser);
            SaveCommentUser();
        }

        public void DeleteCommentUser(int id)
        {
            var commentUser = _commentUserRepository.GetById(id);
            _commentUserRepository.Delete(commentUser);
            SaveCommentUser();
        }

       
        public void SaveCommentUser()
        {
            _unitOfWork.Commit();
        }

        #endregion


        public void CreateCommentUser(string userId, int commentId)
        {
            var commentUser = new CommentUser { UserId = userId, CommentId = commentId };
            _commentUserRepository.Add(commentUser);
            SaveCommentUser();
        }
    }
}
