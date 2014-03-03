using System.Collections.Generic;
using System.Linq;
using SocialGoal.Model.Models;
using SocialGoal.Data.Repository;
using SocialGoal.Data.Infrastructure;

namespace SocialGoal.Service
{
    public interface IGroupUpdateSupportService
    {
        IEnumerable<GroupUpdateSupport> GetSupports();
        GroupUpdateSupport GetSupport(int id);
        IEnumerable<GroupUpdateSupport> GetSupportForUpdate(int updateId);
        int GetSupportcount(int id);
        void CreateSupport(GroupUpdateSupport support);
        void DeleteSupport(int id);
        bool IsUpdateSupported(int updateid, string userid, IGroupUserService groupUserService);
        void DeleteSupport(int updateid, int userid);
        void SaveSupport();

        void CreateUserSupport(GroupUpdateSupport support);
      
        IEnumerable<ApplicationUser> GetSupportersOfUpdate(int id, IUserService userService,IGroupUserService groupUserService);
    }

    public class GroupUpdateSupportService : IGroupUpdateSupportService
    {
        private readonly IGroupUpdateSupportRepository _groupUpdateSupportRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GroupUpdateSupportService(IGroupUpdateSupportRepository groupUpdateSupportRepository, IUnitOfWork unitOfWork)
        {
            _groupUpdateSupportRepository = groupUpdateSupportRepository;
            _unitOfWork = unitOfWork;
        }

        #region IGroupUpdateSupportService Members

        public IEnumerable<GroupUpdateSupport> GetSupports()
        {
            var updateSupports = _groupUpdateSupportRepository.GetAll();
            return updateSupports;
        }

        public IEnumerable<GroupUpdateSupport> GetSupportForUpdate(int updateId)
        {
            return _groupUpdateSupportRepository.GetMany(s => s.GroupUpdateId == updateId).OrderByDescending(s => s.UpdateSupportedDate);
        }

        public void CreateUserSupport(GroupUpdateSupport support)//, ISupportInvitationService supportInvitationService)
        {
            var oldUser = _groupUpdateSupportRepository.GetMany(g => g.GroupUserId == support.GroupUserId && g.GroupUpdateSupportId == support.GroupUpdateSupportId);
            if (oldUser.Count() == 0)
            {
                _groupUpdateSupportRepository.Add(support);
                SaveSupport();
            }
        }

        public GroupUpdateSupport GetSupport(int id)
        {
            var support = _groupUpdateSupportRepository.GetById(id);
            return support;
        }

        public void CreateSupport(GroupUpdateSupport support)
        {
            _groupUpdateSupportRepository.Add(support);
            SaveSupport();
        }

        public void DeleteSupport(int id)
        {
            var support = _groupUpdateSupportRepository.GetById(id);
            _groupUpdateSupportRepository.Delete(support);
            SaveSupport();
        }

        public void DeleteSupport(int updateid, int userid)
        {
            var support = _groupUpdateSupportRepository.Get(f => (f.GroupUpdateId == updateid && f.GroupUserId == userid));
            _groupUpdateSupportRepository.Delete(support);
            SaveSupport();
            //int id = (from s in GetSupports() where s.UpdateId == updateid && s.UserId == userid select s.UpdateSupportId).FirstOrDefault();
            //if (id != 0) DeleteSupport(id);
        }

        public bool IsUpdateSupported(int updateid, string userid, IGroupUserService groupUserService)
        {
            var groupuserid = groupUserService.GetGroupUserByuserId(userid).GroupUserId;
            return _groupUpdateSupportRepository.Get(g => g.GroupUpdateId == updateid && g.GroupUserId == groupuserid) != null;
        }

        public IEnumerable<ApplicationUser> GetSupportersOfUpdate(int id, IUserService userService, IGroupUserService groupUserService)
        {

            var users = new List<int>();
            var supports = _groupUpdateSupportRepository.GetMany(f => f.GroupUpdateId == id);
            foreach (var item in supports)
            {
                var user = item.GroupUserId;
                users.Add(user);
            }
            var userids= groupUserService.GetUserIdByGroupUserId(users);
            var userlist = userService.GetUserByUserId(userids);
            return userlist;
               
        }

        public int GetSupportcount(int id)
        {
            return _groupUpdateSupportRepository.GetMany(c => c.GroupUpdateId == id).Count();
        }

        public void SaveSupport()
        {
            _unitOfWork.Commit();
        }


        #endregion
    }
}
