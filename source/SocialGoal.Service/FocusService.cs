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
        IEnumerable<Focus> GetFocus();
        IEnumerable<Focus> GetFocussOfGroup(int id);
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
        private readonly IFocusRepository _focusRepository;
        private readonly IUnitOfWork _unitOfWork;
        public FocusService(IFocusRepository focusRepository, IUnitOfWork unitOfWork)
        {
            _focusRepository = focusRepository;
            _unitOfWork = unitOfWork;
        }
        #region IFocusService Members

        public IEnumerable<Focus> GetFocus()
        {
            var focus = _focusRepository.GetAll();
            return focus;
        }

        public Focus GetFocus(int id)
        {
            var focus = _focusRepository.GetById(id);
            return focus;
        }

        public Focus GetFocus(string focusname)
        {
            var focus = _focusRepository.Get(f => f.FocusName == focusname);

            return focus;
        }

        public Focus GetGroup(int focusid)
        {
            var group = _focusRepository.Get(f => f.FocusId == focusid);
            return group;
        }
        public IEnumerable<Focus> GetFocussOfGroup(int id)
        {
            var focus = _focusRepository.GetMany(f =>f.GroupId == id);
            return focus;
        }
        public void CreateFocus(Focus focus)
        {
            _focusRepository.Add(focus);
            SaveFocus();
        }

        public IEnumerable<ValidationResult> CanAddFocus(Focus newFocus)
        {
            Focus focus;
            if (newFocus.FocusId == 0)
            {
                focus = _focusRepository.Get(f => f.FocusName == newFocus.FocusName && f.GroupId == newFocus.GroupId);
            }
            else
            {
                focus = _focusRepository.Get(f => f.FocusName == newFocus.FocusName && f.GroupId == newFocus.GroupId && f.FocusId != newFocus.FocusId);
            }
            if (focus != null)
            {
                yield return new ValidationResult("FocusName", Resources.FocusExists);
            }
        }

        public void DeleteFocus(int id)
        {
            var focus = _focusRepository.GetById(id);
            _focusRepository.Delete(focus);
            SaveFocus();
        }
        public void UpdateFocus(Focus focus)
        {
            _focusRepository.Update(focus);
        }

        public void SaveFocus()
        {
            _unitOfWork.Commit();
        }

        #endregion
    }
}
