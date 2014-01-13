using System.Collections.Generic;
using SocialGoal.Data.Repository;
using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
using SocialGoal.Core.Common;
using SocialGoal.Service.Properties;

namespace SocialGoal.Service
{
    public interface IFocusService
    {
        IEnumerable<Focus> GetFocuss();
        IEnumerable<Focus> GetFocussOFGroup(int id);
        Focus GetFocus(int id);
        Focus GetFocus(string focusname);
        void CreateFocus(Focus focus);
        void DeleteFocus(int id);
        void SaveFocus();
        Focus GetGroup(int focusid);
        void UpdateFocus(Focus focus);
        IEnumerable<ValidationResult> CanAddFocus(Focus newFocus);
    }
   
    public class FocusService : IFocusService
    {
        private readonly IFocusRepository focusRepository;
        private readonly IUnitOfWork unitOfWork;
        public FocusService(IFocusRepository focusRepository, IUnitOfWork unitOfWork)
        {
            this.focusRepository = focusRepository;
            this.unitOfWork = unitOfWork;
        }
        #region IFocusService Members

        public IEnumerable<Focus> GetFocuss()
        {
            var focus = focusRepository.GetAll();
            return focus;
        }

        public Focus GetFocus(int id)
        {
            var focus = focusRepository.GetById(id);
            return focus;
        }

        public Focus GetFocus(string focusname)
        {
            var focus = focusRepository.Get(f => f.FocusName == focusname);

            return focus;
        }

        public Focus GetGroup(int focusid)
        {
            var group = focusRepository.Get(f => f.FocusId == focusid);
            return group;
        }
        public IEnumerable<Focus> GetFocussOFGroup(int id)
        {
            var focus = focusRepository.GetMany(f =>f.GroupId == id);
            return focus;
        }
        public void CreateFocus(Focus focus)
        {
            focusRepository.Add(focus);
            SaveFocus();
        }

        public IEnumerable<ValidationResult> CanAddFocus(Focus newFocus)
        {
            Focus focus;
            if (newFocus.FocusId == 0)
            {
                focus = focusRepository.Get(f => f.FocusName == newFocus.FocusName && f.GroupId == newFocus.GroupId);
            }
            else
            {
                focus = focusRepository.Get(f => f.FocusName == newFocus.FocusName && f.GroupId == newFocus.GroupId && f.FocusId != newFocus.FocusId);
            }
            if (focus != null)
            {
                yield return new ValidationResult("FocusName", Resources.FocusExists);
            }
        }

        public void DeleteFocus(int id)
        {
            var focus = focusRepository.GetById(id);
            focusRepository.Delete(focus);
            SaveFocus();
        }
        public void UpdateFocus(Focus focus)
        {
            focusRepository.Update(focus);
            //SaveFocus();
        }

        public void SaveFocus()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
