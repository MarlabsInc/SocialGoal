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
    public interface IUpdateSupportService
    {
        IEnumerable<UpdateSupport> GetSupports();
        //IEnumerable<Support> GetSupports(IEnumerable<int> id);
        UpdateSupport GetSupport(int id);
        IEnumerable<UpdateSupport> GetSupportForUpdate(int updateId);
        //IEnumerable<Goal> GetUserSupportedGoals(int userid, IGoalService goalService);
        //IEnumerable<Goal> GetUserSupportedGoalsBYPopularity(int userid, IGoalService goalService);
        //IEnumerable<ValidationResult> CanInviteUser(int userId, int goalId);
        //IEnumerable<Support> GetTop20SupportsOfFollowings(int userId);
       // IEnumerable<Support> GetTop20Support(int userid);
        int GetSupportcount(int id);
        void CreateSupport(UpdateSupport Support);
        void DeleteSupport(int id);
        bool IsUpdateSupported(int updateid, string userid);
        void DeleteSupport(int updateid, string userid);
        void SaveSupport();
        //IEnumerable<User> SearchUserToSupport(string searchString, int goalId, IUserService userService, ISupportInvitationService supportInvitationService, int userid);

        void CreateUserSupport(UpdateSupport support);//, ISupportInvitationService supportInvitationService);

        IEnumerable<ApplicationUser> GetSupportersOfUpdate(int id, IUserService userService);
    }

    public class UpdateSupportService : IUpdateSupportService
    {
        private readonly IUpdateSupportRepository UpdateSupportRepository;
      //  private readonly IFollowUserRepository followUserRepository;
        private readonly IUnitOfWork unitOfWork;

        public UpdateSupportService(IUpdateSupportRepository UpdateSupportRepository, IUnitOfWork unitOfWork)
        {
            this.UpdateSupportRepository = UpdateSupportRepository;
            //this.followUserRepository = followUserRepository;
            this.unitOfWork = unitOfWork;
        }

        #region IUpdateSupportService Members

        public IEnumerable<UpdateSupport> GetSupports()
        {
            var UpdateSupports = UpdateSupportRepository.GetAll();
            return UpdateSupports;
        }

        public IEnumerable<UpdateSupport> GetSupportForUpdate(int updateId)
        {
            return UpdateSupportRepository.GetMany(s => s.UpdateId == updateId).OrderByDescending(s => s.UpdateSupportedDate);
        }

        //public IEnumerable<Support> GetSupports(IEnumerable<int> id)
        //{
        //    List<Support> Supports = new List<Support> { };
        //    Support Support;
        //    foreach (int item in id)
        //    {
        //        Support = GetSupport(item);
        //        yield return Support;
        //        //Supports.Add(Support);
        //    }
        //    // return Supports;

        //}

        //public IEnumerable<Support> GetTop20SupportsOfFollowings(int userId)
        //{

        //    //var supports = SupportRepository.GetMany(s => s.Goal.GoalType == false).OrderByDescending(s => s.SupportedDate).Take(20).ToList();
        //    var supports = (from s in SupportRepository.GetMany(s => s.Goal.GoalType == false) where (from f in followUserRepository.GetMany(fol => fol.FromUserId == userId) select f.ToUserId).ToList().Contains(s.UserId) select s).OrderByDescending(s => s.SupportedDate).Take(20);
        //    return supports;
        //}
        //public IEnumerable<Support> GetTop20Support(int userid)
        //{

        //    var supports = SupportRepository.GetMany(s => (s.Goal.GoalType == false) && (s.UserId == userid)).OrderByDescending(s => s.SupportedDate).Take(20).ToList();
        //    return supports;
        //}
        public void CreateUserSupport(UpdateSupport support)//, ISupportInvitationService supportInvitationService)
        {
            var oldUser = UpdateSupportRepository.GetMany(g => g.UserId == support.UserId && g.UpdateSupportId == support.UpdateSupportId);
            if (oldUser.Count() == 0)
            {
                UpdateSupportRepository.Add(support);
                SaveSupport();
            }
           // supportInvitationService.AcceptInvitation(support.GoalId, support.UserId);
        }

        public UpdateSupport GetSupport(int id)
        {
            var Support = UpdateSupportRepository.GetById(id);
            return Support;
        }

        public void CreateSupport(UpdateSupport Support)
        {
            UpdateSupportRepository.Add(Support);
            SaveSupport();
        }

        public void DeleteSupport(int id)
        {
            var Support = UpdateSupportRepository.GetById(id);
            UpdateSupportRepository.Delete(Support);
            SaveSupport();
        }

        //public IEnumerable<Goal> GetUserSupportedGoals(int userid, IGoalService goalService)
        //{
        //    return GetSupports().Where(s => s.UserId == userid).Join(goalService.GetGoals(), s => s.GoalId, g => g.GoalId, (s, g) => g).OrderByDescending(g => g.CreatedDate).ToList();
        //}


        //public IEnumerable<Goal> GetUserSupportedGoalsBYPopularity(int userid, IGoalService goalService)
        //{
        //    return GetSupports().Where(s => s.UserId == userid).Join(goalService.GetGoals(), s => s.GoalId, g => g.GoalId, (s, g) => g).OrderByDescending(g => g.Supports.Count()).ToList();

        //}
        public void DeleteSupport(int updateid, string userid)
        {
            var support = UpdateSupportRepository.Get(f => (f.UpdateId == updateid && f.UserId == userid));
            UpdateSupportRepository.Delete(support);
            SaveSupport();
            //int id = (from s in GetSupports() where s.UpdateId == updateid && s.UserId == userid select s.UpdateSupportId).FirstOrDefault();
            //if (id != 0) DeleteSupport(id);
        }

        public bool IsUpdateSupported(int updateid, string userid)
        {
            return UpdateSupportRepository.Get(g => g.UpdateId == updateid && g.UserId == userid) != null;
        }

        public IEnumerable<ApplicationUser> GetSupportersOfUpdate(int id, IUserService userService)
        {
            return userService.GetUsers().Join(UpdateSupportRepository.GetMany(g => g.UpdateId == id),
                 u => u.Id,
                 s => s.UserId,
                 (u, s) => u);
        }

        //public IEnumerable<User> SearchUserToSupport(string searchString, int goalId, IUserService userService, ISupportInvitationService supportInvitationService, int userId)
        //{
        //    var users = from u in userService.GetUsers(searchString)
        //                where u.UserId != userId && !(from g in GetSupporForGoal(goalId) select g.UserId).Contains(u.UserId) &&
        //                !(supportInvitationService.IsUserInvited(goalId, u.UserId))
        //                select u;
        //    return users;
        //}

        //public IEnumerable<ValidationResult> CanInviteUser(int userId, int goalId)
        //{
        //    var supportingUser = SupportRepository.Get(s => s.UserId == userId && s.GoalId == goalId);
        //    if (supportingUser != null)
        //    {
        //        yield return new ValidationResult("", Resources.PersonSupporting);
        //    }
        //}
        public int GetSupportcount(int id)
        {
            return UpdateSupportRepository.GetMany(c => c.UpdateId == id).Count();
        }

        public void SaveSupport()
        {
            unitOfWork.Commit();
        }


        #endregion
    }
}
