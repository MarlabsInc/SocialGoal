using System.Collections.Generic;
using System.Linq;
using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
using SocialGoal.Data.Repository;

namespace SocialGoal.Service
{

    public interface IGroupCommentService
    {
        IEnumerable<GroupComment> GetComments();
        IEnumerable<GroupComment> GetCommentsByUpdate(int updateid);
        IEnumerable<GroupComment> GetTop20CommentsOfPublicGoals(string userid, IGroupUserService groupUserService);
        GroupComment GetLastComment(string userid);
        GroupComment GetComment(int id);
        GroupComment GEtUpdateByCommentId(int id);
        int GetCommentcount(int id);

        void CreateComment(GroupComment comment, string useId);
        void DeleteComment(int id);
        void SaveComment();
    }

    public class GroupCommentService : IGroupCommentService
    {
        private readonly IGroupCommentRepository _groupCommentRepository;
        private readonly IGroupCommentUserRepository _groupCommentUserRepository;
        private readonly IUnitOfWork _unitOfWork;


        public GroupCommentService(IGroupCommentRepository groupCommentRepository,IGroupCommentUserRepository groupCommentUserRepository, IUnitOfWork unitOfWork)
        {
            _groupCommentRepository = groupCommentRepository;
            _groupCommentUserRepository = groupCommentUserRepository;
            _unitOfWork = unitOfWork;
        }

        #region IGroupCommentService Members

        public IEnumerable<GroupComment> GetComments()
        {
            var comment = _groupCommentRepository.GetAll();
            return comment;
        }
        //public IEnumerable<GroupComment> GetCommentsOfPublicGoals()
        //{
        //    var comment = groupCommentRepository.GetMany(c => c.Goal.GoalType == false).ToList();
        //    return comment;
        //}
        //public IEnumerable<GroupComment> GetCommentsByUser(int userid)
        //{
        //    var comment = groupCommentRepository.GetMany(c => (c.UserId == userid && c.Update.Goal.GoalType == false)).OrderByDescending(c => c.CommentDate).ToList();
        //    return comment;
        //}
        public IEnumerable<GroupComment> GetTop20CommentsOfPublicGoals(string userid, IGroupUserService groupUserService)
        {
             var comment = from u in _groupCommentRepository.GetAll() where (from g in groupUserService.GetGroupUsers() where g.UserId == userid select g.GroupId).ToList().Contains(u.GroupUpdate.GroupGoal.GroupUser.GroupId) select u;
          
            return comment;
        }
        public GroupComment GetLastComment(string userid)
        {
            //var comments = groupCommentRepository.GetMany(c => c.UserId == userid).Last();
            var comments = new GroupComment();
            return comments;
        }

        public IEnumerable<GroupComment> GetCommentsByUpdate(int updateid)
        {
            var comments = _groupCommentRepository.GetMany(c => c.GroupUpdateId == updateid).ToList();
            return comments;
        }


        public GroupComment GetComment(int id)
        {
            var comment = _groupCommentRepository.GetById(id);
            return comment;
        }

        public GroupComment GEtUpdateByCommentId(int id)
        {
            var comment = _groupCommentRepository.Get(u => u.GroupCommentId == id);
            return comment;
        }

        public void CreateComment(GroupComment comment, string userId)
        {
            _groupCommentRepository.Add(comment);
            SaveComment();
            var groupCommentUser = new GroupCommentUser { UserId = userId, GroupCommentId = comment.GroupCommentId };
            _groupCommentUserRepository.Add(groupCommentUser);
            SaveComment();
        }

        public int GetCommentcount(int id)
        {
            return _groupCommentRepository.GetMany(c => c.GroupUpdateId == id).Count();
        }

        public void DeleteComment(int id)
        {
            var comment = _groupCommentRepository.GetById(id);
            _groupCommentRepository.Delete(comment);
            _groupCommentUserRepository.Delete(gc => gc.GroupCommentId == id);
            SaveComment();
        }

        public void SaveComment()
        {
            _unitOfWork.Commit();
        }

        #endregion
    }
}
