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
    public interface IGroupGoalService
    {
        IEnumerable<GroupGoal> GetGroupGoals();
        IEnumerable<GroupGoal> GetGroupGoalsByFocus(int focusid);
        GroupGoal GetGroupGoal(int id);
        void CreateGroupGoal(GroupGoal GroupGoal);
        IEnumerable<GroupGoal> GetGroupGoals(int GroupId);
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
        private readonly IGroupGoalRepository GroupGoalRepository;
        private readonly IUnitOfWork unitOfWork;
        public GroupGoalService(IGroupGoalRepository GroupGoalRepository, IUnitOfWork unitOfWork)
        {
            this.GroupGoalRepository = GroupGoalRepository;
            this.unitOfWork = unitOfWork;
        }
        #region IGroupGoalService Members

        public IEnumerable<GroupGoal> GetGroupGoals()
        {
            var GroupGoal = GroupGoalRepository.GetAll();
            return GroupGoal;
        }
        public IEnumerable<GroupGoal> GetGroupGoalsByFocus(int focusid)
        {
            var goals = GroupGoalRepository.GetMany(g => g.FocusId == focusid);
            return goals;
        }

        public IEnumerable<GroupGoal> GetGroupGoals(int GroupId)
        {
            return GroupGoalRepository.GetMany(g => g.GroupUser.GroupId == GroupId);
        }

       
        public GroupGoal GetGroupGoal(int id)
        {
            var GroupGoal = GroupGoalRepository.GetById(id);
            return GroupGoal;
        }

        public IEnumerable<GroupGoal> GetTop20GroupsGoals(string userid, IGroupUserService groupUserService)
        {
            var goals = from g in GroupGoalRepository.GetAll() where (from gu in groupUserService.GetGroupUsers() where gu.UserId == userid select gu.GroupId).ToList().Contains(g.GroupUser.GroupId) select g;
            return goals;
        }
        public void CreateGroupGoal(GroupGoal GroupGoal)
        {
            GroupGoalRepository.Add(GroupGoal);
            SaveGroupGoal();
        }

        public void DeleteGroupGoal(int id)
        {
            var GroupGoal = GroupGoalRepository.GetById(id);
            GroupGoalRepository.Delete(GroupGoal);
            SaveGroupGoal();
        }

        public IEnumerable<ValidationResult> CanAddGoal(GroupGoal newGoal,IGroupUpdateService groupUpdateService)
        {
            GroupGoal goal;
            if (newGoal.GroupGoalId == 0)
                goal = GroupGoalRepository.Get(g => (g.GroupId == newGoal.GroupId) && (g.GoalName == newGoal.GoalName));
            else
                goal = GroupGoalRepository.Get(g => (g.GroupId == newGoal.GroupId) && (g.GoalName == newGoal.GoalName) && g.GroupGoalId != newGoal.GroupGoalId);
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
                var Updates = groupUpdateService.GetUpdatesByGoal(newGoal.GroupGoalId).OrderByDescending(g => g.UpdateDate).ToList();

                if (Updates.Count() > 0)
                {
                    if (Updates[0].UpdateDate.Subtract(newGoal.EndDate).TotalSeconds > 0)
                    {
                        flag = 1;
                    }
                    if (newGoal.StartDate.Subtract(Updates[0].UpdateDate).TotalSeconds > 0)
                    {
                        status = 1;
                    }
                    if (flag == 1)
                    {
                        
                        yield return new ValidationResult("EndDate", Resources.EndDateNotValid + " " + Updates[0].UpdateDate.ToString("dd-MMM-yyyy"));
                    }
                    else if (status == 1)
                    {                        
                        yield return new ValidationResult("StartDate", Resources.StartDate + " " + Updates[0].UpdateDate.ToString("dd-MMM-yyyy"));
                    }

                }
            }
        }

        public void EditGroupGoal(GroupGoal groupGoalToEdit)
        {
            GroupGoalRepository.Update(groupGoalToEdit);
            SaveGroupGoal();
        }


        public int GroupGoals(int groupId)
        {
            return GroupGoalRepository.GetMany(g => g.GroupUser.GroupId == groupId).Count();
        }


        public IEnumerable<GroupGoal> GetAssignedGoalsToOthers(int userid)
        {

            var goals =  GroupGoalRepository.GetMany(f =>((f.GroupUserId == userid) && (f.AssignedGroupUserId != null))).ToList();
            return goals;
        }

        public IEnumerable<GroupGoal> GetAssignedGoalsToMe(int userid)
        {

            var goals = GroupGoalRepository.GetMany(f =>f.AssignedGroupUserId == userid).ToList();
            return goals;
        }


        public void SaveGroupGoal()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
