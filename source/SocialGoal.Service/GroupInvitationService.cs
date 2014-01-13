using System.Collections.Generic;
using System.Linq;
using SocialGoal.Model.Models;
using SocialGoal.Data.Repository;
using SocialGoal.Data.Infrastructure;
using System;

namespace SocialGoal.Service
{
    public interface IGroupInvitationService
    {
        IEnumerable<GroupInvitation> GetGroupInvitations();
        GroupInvitation GetGroupInvitation(int id);
        void CreateGroupInvitation(GroupInvitation GroupInvitation);
        void DeleteGroupInvitation(int id);
        void AcceptInvitation(int id, string userid);
        void SaveGroupInvitation();
        IEnumerable<GroupInvitation> GetGroupInvitationsForUser(string userid);
        IEnumerable<GroupInvitation> GetGroupInvitationsForGroup(int groupId);
        bool IsUserInvited(int groupId, string userId);
    }

    public class GroupInvitationService : IGroupInvitationService
    {
        private readonly IGroupInvitationRepository GroupInvitationRepository;
        private readonly IUnitOfWork unitOfWork;
        public GroupInvitationService(IGroupInvitationRepository GroupInvitationRepository, IUnitOfWork unitOfWork)
        {
            this.GroupInvitationRepository = GroupInvitationRepository;
            this.unitOfWork = unitOfWork;
        }
        #region IGroupInvitationService Members

        public IEnumerable<GroupInvitation> GetGroupInvitations()
        {
            var GroupInvitation = GroupInvitationRepository.GetAll();
            return GroupInvitation;
        }

        public GroupInvitation GetGroupInvitation(int id)
        {
            var GroupInvitation = GroupInvitationRepository.GetById(id);
            return GroupInvitation;
        }

        public void CreateGroupInvitation(GroupInvitation GroupInvitation)
        {
            var oldgroup = GetGroupInvitations().Where(g => g.ToUserId == GroupInvitation.ToUserId && g.GroupId == GroupInvitation.GroupId);
            if (oldgroup.Count() == 0)
            {
                GroupInvitationRepository.Add(GroupInvitation);
                SaveGroupInvitation();
            }
        }

        public void DeleteGroupInvitation(int id)
        {
            var GroupInvitation = GroupInvitationRepository.GetById(id);
            GroupInvitationRepository.Delete(GroupInvitation);
            SaveGroupInvitation();
        }



        public void AcceptInvitation(int id, string userid)
        {
            var groupInvitation = GroupInvitationRepository.Get(g => (g.GroupId == id && g.ToUserId == userid));
            if (groupInvitation != null)
            {
                GroupInvitationRepository.Delete(groupInvitation);
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
            return GroupInvitationRepository.Get(g => g.ToUserId == userId && g.GroupId == groupId) != null;
        }

        public void SaveGroupInvitation()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
