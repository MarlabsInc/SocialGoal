using System.Collections.Generic;
using System.Linq;
using SocialGoal.Model.Models;
using SocialGoal.Data.Repository;
using SocialGoal.Data.Infrastructure;
using SocialGoal.Core.Common;
using SocialGoal.Service.Properties;

namespace SocialGoal.Service
{
    public interface IGroupUpdateSupportService
    {
        IEnumerable<GroupUpdateSupport> GetSupports();
        GroupUpdateSupport GetSupport(int id);
        IEnumerable<GroupUpdateSupport> GetSupportForUpdate(int updateId);
        int GetSupportcount(int id);
        void CreateSupport(GroupUpdateSupport Support);
        void DeleteSupport(int id);
        bool IsUpdateSupported(int updateid, string userid, IGroupUserService groupUserService);
        void DeleteSupport(int updateid, int userid);
        void SaveSupport();

        void CreateUserSupport(GroupUpdateSupport support);
      
        IEnumerable<ApplicationUser> GetSupportersOfUpdate(int id, IUserService userService,IGroupUserService groupUserService);
    }

    public class GroupUpdateSupportService : IGroupUpdateSupportService
    {
        private readonly IGroupUpdateSupportRepository GroupUpdateSupportRepository;
        //  private readonly IFollowUserRepository followUserRepository;
        private readonly IUnitOfWork unitOfWork;

        public GroupUpdateSupportService(IGroupUpdateSupportRepository GroupUpdateSupportRepository, IUnitOfWork unitOfWork)
        {
            this.GroupUpdateSupportRepository = GroupUpdateSupportRepository;
            //this.followUserRepository = followUserRepository;
            this.unitOfWork = unitOfWork;
        }

        #region IGroupUpdateSupportService Members

        public IEnumerable<GroupUpdateSupport> GetSupports()
        {
            var UpdateSupports = GroupUpdateSupportRepository.GetAll();
            return UpdateSupports;
        }

        public IEnumerable<GroupUpdateSupport> GetSupportForUpdate(int updateId)
        {
            return GroupUpdateSupportRepository.GetMany(s => s.GroupUpdateId == updateId).OrderByDescending(s => s.UpdateSupportedDate);
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
        public void CreateUserSupport(GroupUpdateSupport support)//, ISupportInvitationService supportInvitationService)
        {
            var oldUser = GroupUpdateSupportRepository.GetMany(g => g.GroupUserId == support.GroupUserId && g.GroupUpdateSupportId == support.GroupUpdateSupportId);
            if (oldUser.Count() == 0)
            {
                GroupUpdateSupportRepository.Add(support);
                SaveSupport();
            }
            // supportInvitationService.AcceptInvitation(support.GoalId, support.UserId);
        }

        public GroupUpdateSupport GetSupport(int id)
        {
            var Support = GroupUpdateSupportRepository.GetById(id);
            return Support;
        }

        public void CreateSupport(GroupUpdateSupport Support)
        {
            GroupUpdateSupportRepository.Add(Support);
            SaveSupport();
        }

        public void DeleteSupport(int id)
        {
            var Support = GroupUpdateSupportRepository.GetById(id);
            GroupUpdateSupportRepository.Delete(Support);
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
        public void DeleteSupport(int updateid, int userid)
        {
            var support = GroupUpdateSupportRepository.Get(f => (f.GroupUpdateId == updateid && f.GroupUserId == userid));
            GroupUpdateSupportRepository.Delete(support);
            SaveSupport();
            //int id = (from s in GetSupports() where s.UpdateId == updateid && s.UserId == userid select s.UpdateSupportId).FirstOrDefault();
            //if (id != 0) DeleteSupport(id);
        }

        public bool IsUpdateSupported(int updateid, string userid, IGroupUserService groupUserService)
        {
            var groupuserid = groupUserService.GetGroupUserByuserId(userid).GroupUserId;
            return GroupUpdateSupportRepository.Get(g => g.GroupUpdateId == updateid && g.GroupUserId == groupuserid) != null;
        }

        public IEnumerable<ApplicationUser> GetSupportersOfUpdate(int id, IUserService userService, IGroupUserService groupUserService)
        {

            List<int> users = new List<int> { };
            var supports = GroupUpdateSupportRepository.GetMany(f => f.GroupUpdateId == id);
            foreach (var item in supports)
            {
                var user = item.GroupUserId;
                users.Add(user);
            }
            var userids= groupUserService.GetUserIdByGroupUserId(users);
            var userlist = userService.GetUserByUserId(userids);
            return userlist;
               
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
            return GroupUpdateSupportRepository.GetMany(c => c.GroupUpdateId == id).Count();
        }

        public void SaveSupport()
        {
            unitOfWork.Commit();
        }


        #endregion
    }
}
