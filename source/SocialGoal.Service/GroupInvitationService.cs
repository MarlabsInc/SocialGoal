using System.Collections.Generic;
using System.Linq;
using SocialGoal.Model.Models;
using SocialGoal.Data.Repository;
using SocialGoal.Data.Infrastructure;

namespace SocialGoal.Service
{
    public interface IGroupInvitationService
    {
        IEnumerable<GroupInvitation> GetGroupInvitations();
        GroupInvitation GetGroupInvitation(int id);
        void CreateGroupInvitation(GroupInvitation groupInvitation);
        void DeleteGroupInvitation(int id);
        void AcceptInvitation(int id, string userid);
        void SaveGroupInvitation();
        IEnumerable<GroupInvitation> GetGroupInvitationsForUser(string userid);
        IEnumerable<GroupInvitation> GetGroupInvitationsForGroup(int groupId);
        bool IsUserInvited(int groupId, string userId);
    }

    public class GroupInvitationService : IGroupInvitationService
    {
        private readonly IGroupInvitationRepository _groupInvitationRepository;
        private readonly IUnitOfWork _unitOfWork;
        public GroupInvitationService(IGroupInvitationRepository groupInvitationRepository, IUnitOfWork unitOfWork)
        {
            _groupInvitationRepository = groupInvitationRepository;
            _unitOfWork = unitOfWork;
        }
        #region IGroupInvitationService Members

        public IEnumerable<GroupInvitation> GetGroupInvitations()
        {
            var groupInvitation = _groupInvitationRepository.GetAll();
            return groupInvitation;
        }

        public GroupInvitation GetGroupInvitation(int id)
        {
            var groupInvitation = _groupInvitationRepository.GetById(id);
            return groupInvitation;
        }

        public void CreateGroupInvitation(GroupInvitation groupInvitation)
        {
            var oldgroup = GetGroupInvitations().Where(g => g.ToUserId == groupInvitation.ToUserId && g.GroupId == groupInvitation.GroupId);
            if (oldgroup.Count() == 0)
            {
                _groupInvitationRepository.Add(groupInvitation);
                SaveGroupInvitation();
            }
        }

        public void DeleteGroupInvitation(int id)
        {
            var groupInvitation = _groupInvitationRepository.GetById(id);
            _groupInvitationRepository.Delete(groupInvitation);
            SaveGroupInvitation();
        }



        public void AcceptInvitation(int id, string userid)
        {
            var groupInvitation = _groupInvitationRepository.Get(g => (g.GroupId == id && g.ToUserId == userid));
            if (groupInvitation != null)
            {
                _groupInvitationRepository.Delete(groupInvitation);
                //groupInvitation.Accepted = true;
                //GroupInvitationRepository.Update(groupInvitation);
                SaveGroupInvitation();
            }
        }
        public IEnumerable<GroupInvitation> GetGroupInvitationsForGroup(int groupId)
        {
            return from g in GetGroupInvitations() where g.GroupId == groupId select g;
        }

        public IEnumerable<GroupInvitation> GetGroupInvitationsForUser(string userid)
        {
            return from g in GetGroupInvitations() where g.ToUserId == userid && g.Accepted == false select g;
        }

        public bool IsUserInvited(int groupId, string userId)
        {
            return _groupInvitationRepository.Get(g => g.ToUserId == userId && g.GroupId == groupId) != null;
        }

        public void SaveGroupInvitation()
        {
            _unitOfWork.Commit();
        }

        #endregion
    }
}
