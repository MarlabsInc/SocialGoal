using SocialGoal.Data.Repository;
using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
using System;

namespace SocialGoal.Service
{
    public interface IUserProfileService
    {
        UserProfile GetProfile(int id);
        UserProfile GetUser(string userid);
        UserProfile GetUserByEmail(string email);
        UserProfile Register(string email, string firstname, string lastname, string id);
        void UpdateUserProfile(UserProfile user);
        void SaveUserProfile();
    }
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository userProfileRepository;
        private readonly IUnitOfWork unitOfWork;

        public UserProfileService(IUserProfileRepository userProfileRepository, IUnitOfWork unitOfWork)
        {
            this.userProfileRepository = userProfileRepository;
            this.unitOfWork = unitOfWork;
        }

        public UserProfile GetProfile(int id)
        {
            var userprofile = userProfileRepository.Get(u => u.UserProfileId == id);
            return userprofile;
        }
        public UserProfile GetUser(string userid)
        {
            var userprofile = userProfileRepository.Get(u => u.UserId == userid);
            return userprofile;
        }

        public UserProfile GetUserByEmail(string email)
        {
            var userProfile = userProfileRepository.Get(u => u.Email == email);
            return userProfile;
        }

        public UserProfile Register(string email, string firstname, string lastname, string id)
        {
                UserProfile newUser = new UserProfile();
                newUser.UserId = id;
                newUser.Email = email;
                newUser.FirstName = firstname;
                newUser.LastName = lastname;
                userProfileRepository.Add(newUser);
                SaveUserProfile();
                return userProfileRepository.Get(u => u.Email == newUser.Email);
            
        }

        public void UpdateUserProfile(UserProfile user)
        {
            userProfileRepository.Update(user);
            SaveUserProfile();
        }
        public void SaveUserProfile()
        {
            unitOfWork.Commit();
        }
    }
}
