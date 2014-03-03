using System.Collections.Generic;
using System.Linq;
using SocialGoal.Model.Models;
using SocialGoal.Data.Repository;
using SocialGoal.Data.Infrastructure;
using SocialGoal.Core.Common;
using SocialGoal.Service.Properties;

namespace SocialGoal.Service
{
    public interface IGroupGoalService
    {
        IEnumerable<GroupGoal> GetGroupGoals();
        IEnumerable<GroupGoal> GetGroupGoalsByFocus(int focusid);
        GroupGoal GetGroupGoal(int id);
        void CreateGroupGoal(GroupGoal groupGoal);
        IEnumerable<GroupGoal> GetGroupGoals(int groupId);
        void DeleteGroupGoal(int id);
        void SaveGroupGoal();
        IEnumerable<ValidationResult> CanAddGoal(GroupGoal goal, IGroupUpdateService groupUpdateService);
        void EditGroupGoal(GroupGoal groupGoalToEdit);
        int GroupGoals(int groupId);
        IEnumerable<GroupGoal> GetTop20GroupsGoals(string userid, IGroupUserService groupUserService);
        IEnumerable<GroupGoal> GetAssignedGoalsToOthers(int userid);
        IEnumerable<GroupGoal> GetAssignedGoalsToMe(int userid);
    }

    public class GroupGoalService : IGroupGoalService
    {
        private readonly IGroupGoalRepository _groupGoalRepository;
        private readonly IUnitOfWork _unitOfWork;
        public GroupGoalService(IGroupGoalRepository groupGoalRepository, IUnitOfWork unitOfWork)
        {
            _groupGoalRepository = groupGoalRepository;
            _unitOfWork = unitOfWork;
        }
        #region IGroupGoalService Members

        public IEnumerable<GroupGoal> GetGroupGoals()
        {
            var groupGoal = _groupGoalRepository.GetAll();
            return groupGoal;
        }
        public IEnumerable<GroupGoal> GetGroupGoalsByFocus(int focusid)
        {
            var goals = _groupGoalRepository.GetMany(g => g.FocusId == focusid);
            return goals;
        }

        public IEnumerable<GroupGoal> GetGroupGoals(int groupId)
        {
            return _groupGoalRepository.GetMany(g => g.GroupUser.GroupId == groupId);
        }

       
        public GroupGoal GetGroupGoal(int id)
        {
            var groupGoal = _groupGoalRepository.GetById(id);
            return groupGoal;
        }

        public IEnumerable<GroupGoal> GetTop20GroupsGoals(string userid, IGroupUserService groupUserService)
        {
            var goals = from g in _groupGoalRepository.GetAll() where (from gu in groupUserService.GetGroupUsers() where gu.UserId == userid select gu.GroupId).ToList().Contains(g.GroupUser.GroupId) select g;
            return goals;
        }
        public void CreateGroupGoal(GroupGoal groupGoal)
        {
            _groupGoalRepository.Add(groupGoal);
            SaveGroupGoal();
        }

        public void DeleteGroupGoal(int id)
        {
            var groupGoal = _groupGoalRepository.GetById(id);
            _groupGoalRepository.Delete(groupGoal);
            SaveGroupGoal();
        }

        public IEnumerable<ValidationResult> CanAddGoal(GroupGoal newGoal,IGroupUpdateService groupUpdateService)
        {
            GroupGoal goal;
            if (newGoal.GroupGoalId == 0)
                goal = _groupGoalRepository.Get(g => (g.GroupId == newGoal.GroupId) && (g.GoalName == newGoal.GoalName));
            else
                goal = _groupGoalRepository.Get(g => (g.GroupId == newGoal.GroupId) && (g.GoalName == newGoal.GoalName) && g.GroupGoalId != newGoal.GroupGoalId);
            if (goal != null)
            {
                yield return new ValidationResult("GoalName", Resources.GoalExists);
            }
            if (newGoal.StartDate.Subtract(newGoal.EndDate).TotalSeconds > 0)
            {
                yield return new ValidationResult("EndDate", Resources.EndDate);
            }
            int flag = 0;
            int status = 0;
            if (newGoal.GroupGoalId != 0)
            {
                var updates = groupUpdateService.GetUpdatesByGoal(newGoal.GroupGoalId).OrderByDescending(g => g.UpdateDate).ToList();

                if (updates.Count() > 0)
                {
                    if (updates[0].UpdateDate.Subtract(newGoal.EndDate).TotalSeconds > 0)
                    {
                        flag = 1;
                    }
                    if (newGoal.StartDate.Subtract(updates[0].UpdateDate).TotalSeconds > 0)
                    {
                        status = 1;
                    }
                    if (flag == 1)
                    {
                        
                        yield return new ValidationResult("EndDate", Resources.EndDateNotValid + " " + updates[0].UpdateDate.ToString("dd-MMM-yyyy"));
                    }
                    else if (status == 1)
                    {                        
                        yield return new ValidationResult("StartDate", Resources.StartDate + " " + updates[0].UpdateDate.ToString("dd-MMM-yyyy"));
                    }

                }
            }
        }

        public void EditGroupGoal(GroupGoal groupGoalToEdit)
        {
            _groupGoalRepository.Update(groupGoalToEdit);
            SaveGroupGoal();
        }


        public int GroupGoals(int groupId)
        {
            return _groupGoalRepository.GetMany(g => g.GroupUser.GroupId == groupId).Count();
        }


        public IEnumerable<GroupGoal> GetAssignedGoalsToOthers(int userid)
        {

            var goals =  _groupGoalRepository.GetMany(f =>((f.GroupUserId == userid) && (f.AssignedGroupUserId != null))).ToList();
            return goals;
        }

        public IEnumerable<GroupGoal> GetAssignedGoalsToMe(int userid)
        {

            var goals = _groupGoalRepository.GetMany(f =>f.AssignedGroupUserId == userid).ToList();
            return goals;
        }


        public void SaveGroupGoal()
        {
            _unitOfWork.Commit();
        }

        #endregion
    }
}
