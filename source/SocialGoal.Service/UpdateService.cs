using System.Collections.Generic;
using System.Linq;
using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
using SocialGoal.Data.Repository;

namespace SocialGoal.Service
{
    public interface IUpdateService
    {
        IEnumerable<Update> GetUpdates();
        IEnumerable<Update> GetUpdatesByGoal(int goalid);
        IEnumerable<Update> GetUpdatesOfPublicGoals();
        IEnumerable<Update> GetUpdatesForaUser(string userid);
        IEnumerable<Update> GetTop20UpdatesOfFollowing(string userid);
        IEnumerable<Update> GetTop20Updates(string userid);
        IEnumerable<Update> GetUpdatesWithStatus(int goalid);
        Update GetLastUpdate(string userid);
        Update GetUpdate(int id);
        double Progress(int id);
        void CreateUpdate(Update update);
        void EditUpdate(Update update);
        void DeleteUpdate(int id);
        void SaveUpdate();

    }
    public class UpdateService : IUpdateService
    {
        private readonly IGoalRepository _goalRepository;
        private readonly IUpdateRepository _updateRepository;
        private readonly IFollowUserRepository _followUserRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateService(IUpdateRepository updateRepository, IGoalRepository goalRepository, IUnitOfWork unitOfWork, IFollowUserRepository followUserRepository)
        {
            _updateRepository = updateRepository;
            _goalRepository = goalRepository;
            _followUserRepository = followUserRepository;
            _unitOfWork = unitOfWork;
        }
        #region IUpdateService Members

        public IEnumerable<Update> GetUpdates()
        {
            var update = _updateRepository.GetAll();
            return update;
        }
        public Update GetLastUpdate(string userid)
        {
            var updates = _updateRepository.GetMany(g => g.Goal.UserId == userid).Last();
            return updates;
        }
        public IEnumerable<Update> GetUpdatesForaUser(string userid)
        {

            var updates = _updateRepository.GetMany(u => (u.Goal.UserId == userid && u.Goal.GoalType == false)).OrderByDescending(u => u.UpdateDate).ToList(); ;
            return updates;
        }

        public IEnumerable<Update> GetTop20UpdatesOfFollowing(string userid)
        {
            var updates = from u in _updateRepository.GetMany(u => (u.Goal.GoalType == false)) where (from f in _followUserRepository.GetMany(fol => fol.FromUserId == userid) select f.ToUserId).ToList().Contains(u.Goal.UserId) select u;
            return updates;
        }
        public IEnumerable<Update> GetTop20Updates(string userid)
        {

            var updates = _updateRepository.GetMany(u => (u.Goal.GoalType == false) && (u.Goal.UserId == userid)).OrderByDescending(u => u.UpdateDate).Take(20).ToList();
            return updates;
        }

        public IEnumerable<Update> GetUpdatesByGoal(int goalid)
        {
            var updates = _updateRepository.GetMany(u => u.GoalId == goalid).OrderByDescending(u => u.UpdateId).ToList();
            return updates;
        }
        public IEnumerable<Update> GetUpdatesWithStatus(int goalid)
        {
            var updates = _updateRepository.GetMany(u => (u.GoalId == goalid) && (u.status != null)).OrderByDescending(u => u.UpdateId).ToList();
            return updates;
        }
        public IEnumerable<Update> GetUpdatesOfPublicGoals()
        {
            var updates = _updateRepository.GetMany(u => u.Goal.GoalType == false).ToList();
            return updates;
        }
        
        public Update GetUpdate(int id)
        {
            var update = _updateRepository.GetById(id);
            return update;
        }

        //public IEnumerable<ValidationResult> CanAddUpdate(Update newUpdate)
        //{
        //    if (newUpdate.status.GetType().Name != "Double")
        //        yield return new ValidationResult("Status", "Not a valid update");
     
        //}


        public void CreateUpdate(Update update)
        {
            _updateRepository.Add(update);
            SaveUpdate();
        }

        public void EditUpdate(Update update)
        {
            _updateRepository.Update(update);
            SaveUpdate();
        }

        public void DeleteUpdate(int id)
        {
            var update = _updateRepository.GetById(id);
            _updateRepository.Delete(update);
            SaveUpdate();
        }

        public double Progress(int id)
        {
            var status = _updateRepository.GetById(id).status;
            var target = _goalRepository.GetById(_updateRepository.GetById(id).GoalId).Target;
            var progress = (status / target) * 100;
            return (double)progress;

        }

        public void SaveUpdate()
        {
            _unitOfWork.Commit();
        }

        #endregion
    }
}
