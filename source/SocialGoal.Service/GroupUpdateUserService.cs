using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
using SocialGoal.Data.Repository;

namespace SocialGoal.Service
{

    public interface IGroupUpdateUserService
    {
        void CreateGroupUpdateUser(string userId, int groupUpdateId);
        void DeleteGroupUpdateUser(string userId, int groupUpdateId);

        ApplicationUser GetGroupUpdateUser(int groupUpdateId);

        void DeleteGroupUpdateUser(int id);
        void SaveGroupUpdateUser();
       
    }

    public class GroupUpdateUserService : IGroupUpdateUserService
    {
        private readonly IGroupUpdateUserRepository _groupUpdateUserRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GroupUpdateUserService(IGroupUpdateUserRepository groupUpdateUserRepository, IUnitOfWork unitOfWork, IUserRepository userRepository)
        {
            _groupUpdateUserRepository = groupUpdateUserRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;

        }

        #region IGroupUpdateUserService Members

        public void DeleteGroupUpdateUser(string userId, int groupUpdateId)
        {
            var groupUpdateUser = _groupUpdateUserRepository.Get(cu => cu.UserId == userId && cu.GroupUpdateId == groupUpdateId);
            _groupUpdateUserRepository.Delete(groupUpdateUser);
            SaveGroupUpdateUser();
        }

        public void DeleteGroupUpdateUser(int id)
        {
            var groupUpdateUser = _groupUpdateUserRepository.GetById(id);
            _groupUpdateUserRepository.Delete(groupUpdateUser);
            SaveGroupUpdateUser();
        }


        public void SaveGroupUpdateUser()
        {
            _unitOfWork.Commit();
        }

       
        public ApplicationUser GetGroupUpdateUser(int groupUpdateId)
        {
            var groupUpdateUserId = _groupUpdateUserRepository.Get(g => g.GroupUpdateId == groupUpdateId).UserId;
            var user = _userRepository.Get(u => u.Id == groupUpdateUserId);
            return user;

        }


        public void CreateGroupUpdateUser(string userId, int groupUpdateId)
        {
            var groupUpdateUser = new GroupUpdateUser { UserId = userId, GroupUpdateId = groupUpdateId };
            _groupUpdateUserRepository.Add(groupUpdateUser);
            SaveGroupUpdateUser();
        }
        #endregion
    }
}
