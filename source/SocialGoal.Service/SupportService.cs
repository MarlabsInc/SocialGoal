using System.Collections.Generic;
using System.Linq;
using SocialGoal.Model.Models;
using SocialGoal.Data.Repository;
using SocialGoal.Data.Infrastructure;
using SocialGoal.Core.Common;
using SocialGoal.Service.Properties;
using System;

namespace SocialGoal.Service
{
    public interface ISupportService
    {
        IEnumerable<Support> GetSupports();
        IEnumerable<Support> GetSupports(IEnumerable<int> id);
        Support GetSupport(int id);
        IEnumerable<Support> GetSupporForGoal(int goalId);
        IEnumerable<Goal> GetUserSupportedGoals(string userid, IGoalService goalService);
        IEnumerable<Goal> GetUserSupportedGoalsBYPopularity(string userid, IGoalService goalService);
        bool CanInviteUser(string userId, int goalId);
        IEnumerable<Support> GetTop20SupportsOfFollowings(string userId);
        IEnumerable<Support> GetTop20Support(string userid);
        void CreateSupport(Support Support);
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
        private readonly ISupportRepository SupportRepository;
        private readonly IFollowUserRepository followUserRepository;
        private readonly IUnitOfWork unitOfWork;

        public SupportService(ISupportRepository SupportRepository, IFollowUserRepository followUserRepository, IUnitOfWork unitOfWork)
        {
            this.SupportRepository = SupportRepository;
            this.followUserRepository = followUserRepository;
            this.unitOfWork = unitOfWork;
        }

        #region ISupportService Members

        public IEnumerable<Support> GetSupports()
        {
            var Supports = SupportRepository.GetAll();
            return Supports;
        }

        public IEnumerable<Support> GetSupporForGoal(int goalId)
        {
            return SupportRepository.GetMany(s => s.GoalId == goalId).OrderByDescending(s => s.SupportedDate);
        }

        public IEnumerable<Support> GetSupports(IEnumerable<int> id)
        {
            List<Support> Supports = new List<Support> { };
            Support Support;
            foreach (int item in id)
            {
                Support = GetSupport(item);
                yield return Support;
                //Supports.Add(Support);
            }
            // return Supports;

        }

        public IEnumerable<Support> GetTop20SupportsOfFollowings(string userId)
        {

            //var supports = SupportRepository.GetMany(s => s.Goal.GoalType == false).OrderByDescending(s => s.SupportedDate).Take(20).ToList();
            var supports = (from s in SupportRepository.GetMany(s => s.Goal.GoalType == false) where (from f in followUserRepository.GetMany(fol => fol.FromUserId == userId) select f.ToUserId).ToList().Contains(s.UserId) select s).OrderByDescending(s => s.SupportedDate).Take(20);
            return supports;
        }
        public IEnumerable<Support> GetTop20Support(string userid)
        {

            var supports = SupportRepository.GetMany(s => (s.Goal.GoalType == false) && (s.UserId == userid)).OrderByDescending(s => s.SupportedDate).Take(20).ToList();
            return supports;
        }
        public void CreateUserSupport(Support support, ISupportInvitationService supportInvitationService)
        {
            var oldUser = SupportRepository.GetMany(g => g.UserId == support.UserId && g.SupportId == support.SupportId);
            if (oldUser.Count() == 0)
            {
                SupportRepository.Add(support);
                SaveSupport();
            }
            supportInvitationService.AcceptInvitation(support.GoalId, support.UserId);
        }

        public Support GetSupport(int id)
        {
            var Support = SupportRepository.GetById(id);
            return Support;
        }

        public void CreateSupport(Support Support)
        {
            SupportRepository.Add(Support);
            SaveSupport();
        }

        public void DeleteSupport(int id)
        {
            var Support = SupportRepository.GetById(id);
            SupportRepository.Delete(Support);
            SaveSupport();
        }

        public IEnumerable<Goal> GetUserSupportedGoals(string userid, IGoalService goalService)
        {
            return GetSupports().Where(s => s.UserId == userid).Join(goalService.GetGoals(), s => s.GoalId, g => g.GoalId, (s, g) => g).OrderByDescending(g => g.CreatedDate).ToList();
        }


        public IEnumerable<Goal> GetUserSupportedGoalsBYPopularity(string userid, IGoalService goalService)
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
            return SupportRepository.Get(g => g.GoalId == goalid && g.UserId == userid) != null;
        }

        public IEnumerable<ApplicationUser> GetSupportersOfGoal(int id, IUserService userService)
        {
            return userService.GetUsers().Join(SupportRepository.GetMany(g => g.GoalId == id),
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
            var supportingUser = SupportRepository.Get(s => s.UserId == userId && s.GoalId == goalId);
            if (supportingUser != null)
                return false;
            else
                return true;
        }


        public void SaveSupport()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
