using SocialGoal.Data.Repository;
using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;

namespace SocialGoal.Service
{
    public interface IUserProfileService
    {
        UserProfile GetProfile(int id);
        UserProfile GetUser(string userid);
        UserProfile GetUserByEmail(string email);
        
        void CreateUserProfile(string userId);
        void UpdateUserProfile(UserProfile user);
        void SaveUserProfile();
    }
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserProfileService(IUserProfileRepository userProfileRepository, IUnitOfWork unitOfWork)
        {
            _userProfileRepository = userProfileRepository;
            _unitOfWork = unitOfWork;
        }

        public UserProfile GetProfile(int id)
        {
            var userprofile = _userProfileRepository.Get(u => u.UserProfileId == id);
            return userprofile;
        }
        public UserProfile GetUser(string userid)
        {
            var userprofile = _userProfileRepository.Get(u => u.UserId == userid);
            return userprofile;
        }

        public UserProfile GetUserByEmail(string email)
        {
            var userProfile = _userProfileRepository.Get(u => u.Email == email);
            return userProfile;
        }

        public void CreateUserProfile(string userId)
        {

            UserProfile newUserProfile = new UserProfile();
            newUserProfile.UserId = userId;
            _userProfileRepository.Add(newUserProfile);
            SaveUserProfile();
        }
        public void UpdateUserProfile(UserProfile user)
        {
            _userProfileRepository.Update(user);
            SaveUserProfile();
        }
        public void SaveUserProfile()
        {
            _unitOfWork.Commit();
        }
    }
}
