using System.Collections.Generic;
using System.Linq;
using SocialGoal.Model.Models;
using SocialGoal.Data.Repository;
using SocialGoal.Data.Infrastructure;

namespace SocialGoal.Service
{
    public interface ISupportInvitationService
    {
        IEnumerable<SupportInvitation> GetSupportInvitations();
        SupportInvitation GetSupportInvitation(int id);
        void CreateSupportInvitation(SupportInvitation supportInvitation);
        void DeleteSupportInvitation(int id);
        void SaveSupportInvitation();
        bool IsUserInvited(int goalId, string userId);
        void AcceptInvitation(int id, string userid);

        IEnumerable<SupportInvitation> GetSupportInvitationsForUser(string userId);
    }
    public class SupportInvitationService : ISupportInvitationService
    {
        private readonly ISupportInvitationRepository _supportInvitationRepository;
        private readonly IUnitOfWork _unitOfWork;
        
        public SupportInvitationService(ISupportInvitationRepository supportInvitationRepository, IUnitOfWork unitOfWork)
        {
            _supportInvitationRepository = supportInvitationRepository;
            _unitOfWork = unitOfWork;
        }

        #region ISupportInvitationService Members

        public IEnumerable<SupportInvitation> GetSupportInvitations()
        {
            var supportInvitation = _supportInvitationRepository.GetAll();
            return supportInvitation;
        }

        public void AcceptInvitation(int id, string userid)
        {
            var supportInvitation = _supportInvitationRepository.Get(g => (g.GoalId == id && g.ToUserId == userid));
            if (supportInvitation != null)
            {
                _supportInvitationRepository.Delete(supportInvitation);
                SaveSupportInvitation();
            }
        }

        public SupportInvitation GetSupportInvitation(int id)
        {
            var supportInvitation = _supportInvitationRepository.GetById(id);
            return supportInvitation;
        }

        public void CreateSupportInvitation(SupportInvitation supportInvitation)
        {
            _supportInvitationRepository.Add(supportInvitation);
            SaveSupportInvitation();
        }

        public void DeleteSupportInvitation(int id)
        {
            var supportInvitation = _supportInvitationRepository.GetById(id);
            _supportInvitationRepository.Delete(supportInvitation);
            SaveSupportInvitation();
        }

        public IEnumerable<SupportInvitation> GetSupportInvitationsForUser(string userId)
        {
            return from s in GetSupportInvitations() where s.ToUserId == userId && s.Accepted == false select s;
        }

        public bool IsUserInvited(int goalId, string userId)
        {
            return _supportInvitationRepository.Get(s => s.ToUserId == userId && s.GoalId == goalId) != null;
        }

        public void SaveSupportInvitation()
        {
            _unitOfWork.Commit();
        }

        #endregion
    }
}
