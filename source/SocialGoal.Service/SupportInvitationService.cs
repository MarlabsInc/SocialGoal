using System.Collections.Generic;
using System.Linq;
using SocialGoal.Model.Models;
using SocialGoal.Data.Repository;
using SocialGoal.Data.Infrastructure;
using System;

namespace SocialGoal.Service
{
    public interface ISupportInvitationService
    {
        IEnumerable<SupportInvitation> GetSupportInvitations();
        SupportInvitation GetSupportInvitation(int id);
        void CreateSupportInvitation(SupportInvitation SupportInvitation);
        void DeleteSupportInvitation(int id);
        void SaveSupportInvitation();
        bool IsUserInvited(int goalId, string userId);
        void AcceptInvitation(int id, string userid);

        IEnumerable<SupportInvitation> GetSupportInvitationsForUser(string userId);
    }
    public class SupportInvitationService : ISupportInvitationService
    {
        private readonly ISupportInvitationRepository SupportInvitationRepository;
        private readonly IUnitOfWork unitOfWork;
        
        public SupportInvitationService(ISupportInvitationRepository SupportInvitationRepository, IUnitOfWork unitOfWork)
        {
            this.SupportInvitationRepository = SupportInvitationRepository;
            this.unitOfWork = unitOfWork;
        }

        #region ISupportInvitationService Members

        public IEnumerable<SupportInvitation> GetSupportInvitations()
        {
            var SupportInvitation = SupportInvitationRepository.GetAll();
            return SupportInvitation;
        }

        public void AcceptInvitation(int id, string userid)
        {
            var SupportInvitation = SupportInvitationRepository.Get(g => (g.GoalId == id && g.ToUserId == userid));
            if (SupportInvitation != null)
            {
                SupportInvitationRepository.Delete(SupportInvitation);
                //SupportInvitation.Accepted = true;
                //SupportInvitationRepository.Update(SupportInvitation);
                SaveSupportInvitation();
            }
        }

        public SupportInvitation GetSupportInvitation(int id)
        {
            var SupportInvitation = SupportInvitationRepository.GetById(id);
            return SupportInvitation;
        }

        public void CreateSupportInvitation(SupportInvitation SupportInvitation)
        {
            SupportInvitationRepository.Add(SupportInvitation);
            SaveSupportInvitation();
        }

        public void DeleteSupportInvitation(int id)
        {
            var SupportInvitation = SupportInvitationRepository.GetById(id);
            SupportInvitationRepository.Delete(SupportInvitation);
            SaveSupportInvitation();
        }

        public IEnumerable<SupportInvitation> GetSupportInvitationsForUser(string userId)
        {
            return from s in GetSupportInvitations() where s.ToUserId == userId && s.Accepted == false select s;
        }

        public bool IsUserInvited(int goalId, string userId)
        {
            return SupportInvitationRepository.Get(s => s.ToUserId == userId && s.GoalId == goalId) != null;
        }

        public void SaveSupportInvitation()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
