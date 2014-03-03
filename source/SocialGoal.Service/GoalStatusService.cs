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
        void CreateGoalStatus(GoalStatus goalStatus);
        void DeleteGoalStatus(int id);
        void SaveGoalStatus();
    }

    public class GoalStatusService : IGoalStatusService
    {
        private readonly IGoalStatusRepository _goalStatusRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GoalStatusService(IGoalStatusRepository goalStatusRepository, IUnitOfWork unitOfWork)
        {
            _goalStatusRepository = goalStatusRepository;
            _unitOfWork = unitOfWork;
        }

        #region IGoalStatusService Members

        public IEnumerable<GoalStatus> GetGoalStatus()
        {
            var goalStatus = _goalStatusRepository.GetAll();
            return goalStatus;
        }

        public GoalStatus GetGoalStatus(int id)
        {
            var goalStatus = _goalStatusRepository.GetById(id);
            return goalStatus;
        }

        public void CreateGoalStatus(GoalStatus goalStatus)
        {
            _goalStatusRepository.Add(goalStatus);
            SaveGoalStatus();
        }

        public void DeleteGoalStatus(int id)
        {
            var goalStatus = _goalStatusRepository.GetById(id);
            _goalStatusRepository.Delete(goalStatus);
            SaveGoalStatus();
        }

        public void SaveGoalStatus()
        {
            _unitOfWork.Commit();
        }

        #endregion
    }
}
