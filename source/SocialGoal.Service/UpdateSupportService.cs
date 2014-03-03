using System.Collections.Generic;
using System.Linq;
using SocialGoal.Model.Models;
using SocialGoal.Data.Repository;
using SocialGoal.Data.Infrastructure;

namespace SocialGoal.Service
{
    public interface IUpdateSupportService
    {
        IEnumerable<UpdateSupport> GetSupports();
        UpdateSupport GetSupport(int id);
        IEnumerable<UpdateSupport> GetSupportForUpdate(int updateId);

        int GetSupportcount(int id);
        void CreateSupport(UpdateSupport support);
        void DeleteSupport(int id);
        bool IsUpdateSupported(int updateid, string userid);
        void DeleteSupport(int updateid, string userid);
        void SaveSupport();

        void CreateUserSupport(UpdateSupport support);

        IEnumerable<ApplicationUser> GetSupportersOfUpdate(int id, IUserService userService);
    }

    public class UpdateSupportService : IUpdateSupportService
    {
        private readonly IUpdateSupportRepository _updateSupportRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSupportService(IUpdateSupportRepository updateSupportRepository, IUnitOfWork unitOfWork)
        {
            _updateSupportRepository = updateSupportRepository;
            _unitOfWork = unitOfWork;
        }

        #region IUpdateSupportService Members

        public IEnumerable<UpdateSupport> GetSupports()
        {
            var updateSupports = _updateSupportRepository.GetAll();
            return updateSupports;
        }

        public IEnumerable<UpdateSupport> GetSupportForUpdate(int updateId)
        {
            return _updateSupportRepository.GetMany(s => s.UpdateId == updateId).OrderByDescending(s => s.UpdateSupportedDate);
        }

        public void CreateUserSupport(UpdateSupport support)
        {
            var oldUser = _updateSupportRepository.GetMany(g => g.UserId == support.UserId && g.UpdateSupportId == support.UpdateSupportId);
            if (!oldUser.Any())
            {
                _updateSupportRepository.Add(support);
                SaveSupport();
            }
        }

        public UpdateSupport GetSupport(int id)
        {
            var support = _updateSupportRepository.GetById(id);
            return support;
        }

        public void CreateSupport(UpdateSupport support)
        {
            _updateSupportRepository.Add(support);
            SaveSupport();
        }

        public void DeleteSupport(int id)
        {
            var support = _updateSupportRepository.GetById(id);
            _updateSupportRepository.Delete(support);
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
            var support = _updateSupportRepository.Get(f => (f.UpdateId == updateid && f.UserId == userid));
            _updateSupportRepository.Delete(support);
            SaveSupport();
            //int id = (from s in GetSupports() where s.UpdateId == updateid && s.UserId == userid select s.UpdateSupportId).FirstOrDefault();
            //if (id != 0) DeleteSupport(id);
        }

        public bool IsUpdateSupported(int updateid, string userid)
        {
            return _updateSupportRepository.Get(g => g.UpdateId == updateid && g.UserId == userid) != null;
        }

        public IEnumerable<ApplicationUser> GetSupportersOfUpdate(int id, IUserService userService)
        {
            return userService.GetUsers().Join(_updateSupportRepository.GetMany(g => g.UpdateId == id),
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
            return _updateSupportRepository.GetMany(c => c.UpdateId == id).Count();
        }

        public void SaveSupport()
        {
            _unitOfWork.Commit();
        }


        #endregion
    }
}
