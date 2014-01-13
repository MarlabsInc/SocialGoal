using System.Collections.Generic;
using System.Linq;
using SocialGoal.Data;
using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
using SocialGoal.Web.Core.Models;
using SocialGoal.Data.Repository;
using System;

namespace SocialGoal.Service
{
    public interface IGroupUpdateService
    {
        IEnumerable<GroupUpdate> GetUpdates();
        IEnumerable<GroupUpdate> GetUpdatesByGoal(int goalid);
       // IEnumerable<GroupUpdate> GetUpdatesOfPublicGoals();
      //IEnumerable<GroupUpdate> GetUpdatesForaUser(int userid);
        IEnumerable<GroupUpdate> GetTop20Updates(string userid, IGroupUserService groupUserService);
        IEnumerable<GroupUpdate> GetUpdatesWithStatus(int goalid);
        GroupUpdate GetLastUpdate(string userid);
        GroupUpdate GetUpdate(int id);
        double Progress(int id);
        void CreateUpdate(GroupUpdate update, string userId);
        void EditUpdate(GroupUpdate update);
        void DeleteUpdate(int id);
        void SaveUpdate();
        //IEnumerable<Update> GetSupportGoalsUpdates(int UserId);
    }
    public class GroupUpdateService : IGroupUpdateService
    {
        private readonly IGroupUpdateRepository groupUpdateRepository;
        private readonly IGroupUpdateUserRepository groupUpdateUserRepository;
        private readonly IGroupGoalRepository groupGoalRepository;
        private readonly IUnitOfWork unitOfWork;
        public GroupUpdateService(IGroupUpdateRepository updateRepository,IGroupUpdateUserRepository groupUpdateUserRepository,IGroupGoalRepository groupGoalRepository, IUnitOfWork unitOfWork)
        {
            this.groupUpdateRepository = updateRepository;
            this.groupUpdateUserRepository = groupUpdateUserRepository;
            this.groupGoalRepository = groupGoalRepository;
            this.unitOfWork = unitOfWork;
        }
        #region IGroupUpdateService Members

        public IEnumerable<GroupUpdate> GetUpdates()
        {
            var update = groupUpdateRepository.GetAll();
            return update;
        }
        public GroupUpdate GetLastUpdate(string userid)
        {
            var updates = groupUpdateRepository.GetMany(g => g.GroupGoal.GroupUser.UserId == userid).Last();
            return updates;
        }
        //public IEnumerable<GroupUpdate> GetUpdatesForaUser(int userid)
        //{
        //    var updates = groupUpdateRepository.GetMany(u => (u.GroupGoal.UserId == userid && u.GroupGoal.GoalType == false)).OrderByDescending(u => u.UpdateDate).ToList(); ;
        //    return updates;
        //}

        public IEnumerable<GroupUpdate> GetTop20Updates(string userid, IGroupUserService groupUserService)
        {
            var updates = from u in groupUpdateRepository.GetAll() where (from g in groupUserService.GetGroupUsers() where g.UserId == userid select g.GroupId).ToList().Contains(u.GroupGoal.GroupUser.GroupId) select u;
            return updates;
        }

        public IEnumerable<GroupUpdate> GetUpdatesByGoal(int goalid)
        {
            var updates = groupUpdateRepository.GetMany(u => u.GroupGoalId == goalid).OrderByDescending(u => u.GroupUpdateId).ToList();
            return updates;
        }
        public IEnumerable<GroupUpdate> GetUpdatesWithStatus(int goalid)
        {
            var updates = groupUpdateRepository.GetMany(u => (u.GroupGoalId == goalid) && (u.status != null)).OrderByDescending(u => u.GroupUpdateId).ToList();
            return updates;
        }
        
        public GroupUpdate GetUpdate(int id)
        {
            var update = groupUpdateRepository.GetById(id);
            return update;
        }

        public void CreateUpdate(GroupUpdate update, string userId)
        {
            groupUpdateRepository.Add(update);
            SaveUpdate();
            var groupUpdateUser = new GroupUpdateUser { UserId = userId, GroupUpdateId = update.GroupUpdateId };
            groupUpdateUserRepository.Add(groupUpdateUser);
            SaveUpdate();
        }

        public double Progress(int id)
        {
            var status = groupUpdateRepository.GetById(id).status;
            var target = groupGoalRepository.GetById(groupUpdateRepository.GetById(id).GroupGoalId).Target;
            var progress = (status / target) * 100;
            return (double)progress;

        }
        public void EditUpdate(GroupUpdate update)
        {
            groupUpdateRepository.Update(update);
            SaveUpdate();
        }

        public void DeleteUpdate(int id)
        {
            var update = groupUpdateRepository.GetById(id);
            groupUpdateRepository.Delete(update);
            groupUpdateUserRepository.Delete(gu => gu.GroupUpdateId == id);
            SaveUpdate();
        }

        public void SaveUpdate()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}


