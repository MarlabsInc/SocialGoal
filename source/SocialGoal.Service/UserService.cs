using System.Collections.Generic;
using System.Linq;
using SocialGoal.Data.Repository;
using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
using SocialGoal.Core.Common;
using SocialGoal.Service.Properties;

namespace SocialGoal.Service
{

    public interface IUserService
    {
        ApplicationUser GetUser(string userId);
        IEnumerable<ApplicationUser> GetUsers();
        IEnumerable<ApplicationUser> GetUsers(string username);
        ApplicationUser GetUserProfile(string userid);
        ApplicationUser GetUsersByEmail(string email);
        IEnumerable<ApplicationUser> GetInvitedUsers(string username, int groupId, IGroupInvitationService groupInvitationService);
        IEnumerable<ApplicationUser> GetUserByUserId(IEnumerable<string> userid);
        IEnumerable<ApplicationUser> SearchUser(string searchString);        
      
        IEnumerable<ValidationResult> CanAddUser(string email);      
        void UpdateUser(ApplicationUser user);
       void SaveUser();
       void EditUser(string id, string firstname, string lastname, string email);


       void SaveImageURL(string userId, string imageUrl);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork,IUserProfileRepository userProfileRepository)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _userProfileRepository = userProfileRepository;
        }
        public ApplicationUser GetUserProfile(string userid)
        {
            var userprofile = _userRepository.Get(u=>u.Id==userid);
            return userprofile;
        }


        #region IUserService Members

        public ApplicationUser GetUser(string userId)
        {
            return _userRepository.Get(u => u.Id == userId);
        }

        public IEnumerable<ApplicationUser> GetUsers()
        {
            var users = _userRepository.GetAll();
            return users;
        }
        public ApplicationUser GetUsersByEmail(string email)
        {
            var users = _userRepository.Get(u => u.Email.Contains(email));
            return users;

        }
        public IEnumerable<ApplicationUser> GetUsers(string username)
        {
            var users = _userRepository.GetMany(u => (u.FirstName + " " + u.LastName).Contains(username) || u.Email.Contains(username)).OrderBy(u => u.FirstName).ToList();

            return users;
        }
        public IEnumerable<ApplicationUser> GetInvitedUsers(string username, int groupId, IGroupInvitationService groupInvitationService)
        {
            return GetUsers(username).Join(groupInvitationService.GetGroupInvitationsForGroup(groupId).Where(inv => inv.Accepted == false), u => u.Id, i => i.ToUserId, (u, i) => u);
        }

        public IEnumerable<ValidationResult> CanAddUser(string email)
        {

            var user = _userRepository.Get(u => u.Email == email);
            if (user != null)
            {
                yield return new ValidationResult("Email", Resources.EmailExixts);


            }
        }

        



        public void SaveImageURL(string userId, string imageUrl)
        {
            var user = GetUser(userId);
            user.ProfilePicUrl = imageUrl;
            UpdateUser(user);
        }
        public void EditUser(string id,string firstname, string lastname,string email)
        {
            var user = GetUser(id);
            user.FirstName = firstname;
            user.LastName = lastname;
            user.Email = email;
            UpdateUser(user);
        }
        public void UpdateUser(ApplicationUser user)
        {
            _userRepository.Update(user);
            SaveUser();
        }

        public IEnumerable<ApplicationUser> SearchUser(string searchString)
        {
            var users = _userRepository.GetMany(u=>u.UserName.Contains(searchString)|| u.FirstName.Contains(searchString) || u.LastName.Contains(searchString) || u.Email.Contains(searchString)).OrderBy(u=>u.DisplayName);
            return users;
        }

       public void SaveUser()
        {
            _unitOfWork.Commit();
        }
      
        public IEnumerable<ApplicationUser> GetUserByUserId(IEnumerable<string> userid)
        {
            var users = new List<ApplicationUser>();
            foreach (string item in userid)
            {
                var Users = _userRepository.GetById(item);
                users.Add(Users);

            }
            return users;
        }

        #endregion
    }
}
