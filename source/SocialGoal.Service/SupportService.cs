using System.Collections.Generic;
using System.Linq;
using SocialGoal.Model.Models;
using SocialGoal.Data.Repository;
using SocialGoal.Data.Infrastructure;

namespace SocialGoal.Service
{
    public interface ISupportService
    {
        IEnumerable<Support> GetSupports();
        IEnumerable<Support> GetSupports(IEnumerable<int> id);
        Support GetSupport(int id);
        IEnumerable<Support> GetSupporForGoal(int goalId);
        IEnumerable<Goal> GetUserSupportedGoals(string userid, IGoalService goalService);
        IEnumerable<Goal> GetUserSupportedGoalsByPopularity(string userid, IGoalService goalService);
        bool CanInviteUser(string userId, int goalId);
        IEnumerable<Support> GetTop20SupportsOfFollowings(string userId);
        IEnumerable<Support> GetTop20Support(string userid);
        void CreateSupport(Support support);
        void DeleteSupport(int id);
        bool IsGoalSupported(int goalid, string userid);
        void DeleteSupport(int goalid, string userid);
        void SaveSupport();
        IEnumerable<ApplicationUser> SearchUserToSupport(string searchString, int goalId, IUserService userService, ISupportInvitationService supportInvitationService, string userid);

        void CreateUserSupport(Support support, ISupportInvitationService supportInvitationService);

        IEnumerable<ApplicationUser> GetSupportersOfGoal(int id, IUserService userService);
    }

    public class SupportService : ISupportService
    {
        private readonly ISupportRepository _supportRepository;
        private readonly IFollowUserRepository _followUserRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SupportService(ISupportRepository supportRepository, IFollowUserRepository followUserRepository, IUnitOfWork unitOfWork)
        {
            _supportRepository = supportRepository;
            _followUserRepository = followUserRepository;
            _unitOfWork = unitOfWork;
        }

        #region ISupportService Members

        public IEnumerable<Support> GetSupports()
        {
            var supports = _supportRepository.GetAll();
            return supports;
        }

        public IEnumerable<Support> GetSupporForGoal(int goalId)
        {
            return _supportRepository.GetMany(s => s.GoalId == goalId).OrderByDescending(s => s.SupportedDate);
        }

        public IEnumerable<Support> GetSupports(IEnumerable<int> id)
        {
            foreach (int item in id)
            {
                Support support = GetSupport(item);
                yield return support;
            }
        }

        public IEnumerable<Support> GetTop20SupportsOfFollowings(string userId)
        {

            //var supports = SupportRepository.GetMany(s => s.Goal.GoalType == false).OrderByDescending(s => s.SupportedDate).Take(20).ToList();
            var supports = (from s in _supportRepository.GetMany(s => s.Goal.GoalType == false) where (from f in _followUserRepository.GetMany(fol => fol.FromUserId == userId) select f.ToUserId).ToList().Contains(s.UserId) select s).OrderByDescending(s => s.SupportedDate).Take(20);
            return supports;
        }
        public IEnumerable<Support> GetTop20Support(string userid)
        {

            var supports = _supportRepository.GetMany(s => (s.Goal.GoalType == false) && (s.UserId == userid)).OrderByDescending(s => s.SupportedDate).Take(20).ToList();
            return supports;
        }
        public void CreateUserSupport(Support support, ISupportInvitationService supportInvitationService)
        {
            var oldUser = _supportRepository.GetMany(g => g.UserId == support.UserId && g.SupportId == support.SupportId);
            if (oldUser.Count() == 0)
            {
                _supportRepository.Add(support);
                SaveSupport();
            }
            supportInvitationService.AcceptInvitation(support.GoalId, support.UserId);
        }

        public Support GetSupport(int id)
        {
            var support = _supportRepository.GetById(id);
            return support;
        }

        public void CreateSupport(Support support)
        {
            _supportRepository.Add(support);
            SaveSupport();
        }

        public void DeleteSupport(int id)
        {
            var support = _supportRepository.GetById(id);
            _supportRepository.Delete(support);
            SaveSupport();
        }

        public IEnumerable<Goal> GetUserSupportedGoals(string userid, IGoalService goalService)
        {
            return GetSupports().Where(s => s.UserId == userid).Join(goalService.GetGoals(), s => s.GoalId, g => g.GoalId, (s, g) => g).OrderByDescending(g => g.CreatedDate).ToList();
        }


        public IEnumerable<Goal> GetUserSupportedGoalsByPopularity(string userid, IGoalService goalService)
        {
            return GetSupports().Where(s => s.UserId == userid).Join(goalService.GetGoals(), s => s.GoalId, g => g.GoalId, (s, g) => g).OrderByDescending(g => g.Supports.Count()).ToList();

        }
        public void DeleteSupport(int goalid, string userid)
        {
            int id = (from s in GetSupports() where s.GoalId == goalid && s.UserId == userid select s.SupportId).FirstOrDefault();
            if (id != 0) DeleteSupport(id);
        }

        public bool IsGoalSupported(int goalid, string userid)
        {
            return _supportRepository.Get(g => g.GoalId == goalid && g.UserId == userid) != null;
        }

        public IEnumerable<ApplicationUser> GetSupportersOfGoal(int id, IUserService userService)
        {
            return userService.GetUsers().Join(_supportRepository.GetMany(g => g.GoalId == id),
                 u => u.Id,
                 s => s.UserId,
                 (u, s) => u);
        }

        public IEnumerable<ApplicationUser> SearchUserToSupport(string searchString, int goalId, IUserService userService, ISupportInvitationService supportInvitationService, string userId)
        {
            var users = from u in userService.GetUsers(searchString)
                        where u.Id != userId && !(from g in GetSupporForGoal(goalId) select g.UserId).Contains(u.Id) &&
                        !(supportInvitationService.IsUserInvited(goalId, u.Id))
                        select u;
            return users;
        }

        public bool CanInviteUser(string userId, int goalId)
        {
            var supportingUser = _supportRepository.Get(s => s.UserId == userId && s.GoalId == goalId);
            if (supportingUser != null)
                return false;
            return true;
        }


        public void SaveSupport()
        {
            _unitOfWork.Commit();
        }

        #endregion
    }
}
