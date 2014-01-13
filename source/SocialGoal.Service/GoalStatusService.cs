using System.Collections.Generic;
using SocialGoal.Data.Repository;
using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;

namespace SocialGoal.Service
{
    public interface IGoalStatusService
    {
        IEnumerable<GoalStatus> GetGoalStatus();
        GoalStatus GetGoalStatus(int id);
        void CreateGoalStatus(GoalStatus GoalStatus);
        void DeleteGoalStatus(int id);
        void SaveGoalStatus();
    }

    public class GoalStatusService : IGoalStatusService
    {
        private readonly IGoalStatusRepository goalStatusRepository;
        private readonly IUnitOfWork unitOfWork;

        public GoalStatusService(IGoalStatusRepository goalStatusRepository, IUnitOfWork unitOfWork)
        {
            this.goalStatusRepository = goalStatusRepository;
            this.unitOfWork = unitOfWork;
        }

        #region IGoalStatusService Members

        public IEnumerable<GoalStatus> GetGoalStatus()
        {
            var goalStatus = goalStatusRepository.GetAll();
            return goalStatus;
        }

        public GoalStatus GetGoalStatus(int id)
        {
            var goalStatus = goalStatusRepository.GetById(id);
            return goalStatus;
        }

        public void CreateGoalStatus(GoalStatus goalStatus)
        {
            goalStatusRepository.Add(goalStatus);
            SaveGoalStatus();
        }

        public void DeleteGoalStatus(int id)
        {
            var goalStatus = goalStatusRepository.GetById(id);
            goalStatusRepository.Delete(goalStatus);
            SaveGoalStatus();
        }

        public void SaveGoalStatus()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
