using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
using SocialGoal.Data.Repository;

namespace SocialGoal.Service
{

    public interface IGroupCommentUserService
    {
        void CreateGroupCommentUser(string userId, int groupCommentId);
        void DeleteGroupCommentUser(string userId, int groupCommentId);
        GroupCommentUser GetCommentUser(int commentId);

        ApplicationUser GetGroupCommentUser(int groupCommentId);
        void DeleteGroupCommentUser(int id);
        void SaveGroupCommentUser();
       
    }

    public class GroupCommentUserService : IGroupCommentUserService
    {
        private readonly IGroupCommentUserRepository _groupCommentUserRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GroupCommentUserService(IGroupCommentUserRepository groupCommentUserRepository,IUnitOfWork unitOfWork,IUserRepository userRepository)
        {
            _groupCommentUserRepository = groupCommentUserRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }


        #region IGroupCommentUserService Members

        public void DeleteGroupCommentUser(string userId, int groupCommentId)
        {
            var groupCommentUser = _groupCommentUserRepository.Get(cu => cu.UserId == userId && cu.GroupCommentId == groupCommentId);
            _groupCommentUserRepository.Delete(groupCommentUser);
            SaveGroupCommentUser();
        }

        public GroupCommentUser GetCommentUser(int commentId)
        {
            return _groupCommentUserRepository.Get(gcu => gcu.GroupCommentId == commentId);
        }

        public void DeleteGroupCommentUser(int id)
        {
            var groupCommentUser = _groupCommentUserRepository.GetById(id);
            _groupCommentUserRepository.Delete(groupCommentUser);
            SaveGroupCommentUser();
        }

       
        public void SaveGroupCommentUser()
        {
            _unitOfWork.Commit();
        }




        public void CreateGroupCommentUser(string userId, int groupCommentId)
        {
            var groupCommentUser = new GroupCommentUser { UserId = userId, GroupCommentId = groupCommentId };
            _groupCommentUserRepository.Add(groupCommentUser);
            SaveGroupCommentUser();
        }


        public ApplicationUser GetGroupCommentUser(int groupCommentId)
        {
            var groupCommentUserId = _groupCommentUserRepository.Get(g => g.GroupCommentId == groupCommentId).UserId;
            return _userRepository.GetById(groupCommentUserId);
        }


        #endregion
    }
}
