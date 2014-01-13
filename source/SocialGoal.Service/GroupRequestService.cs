using System.Collections.Generic;
using System.Linq;
using SocialGoal.Model.Models;
using SocialGoal.Data.Repository;
using SocialGoal.Data.Infrastructure;
using System;

namespace SocialGoal.Service
{
    public interface IGroupRequestService
    {
        IEnumerable<GroupRequest> GetGroupRequests();
        GroupRequest GetGroupRequest(int id);
        IEnumerable<GroupRequest>GetGroupRequests(int groupId);
        void CreateGroupRequest(GroupRequest groupRequest);
        IEnumerable<GroupRequest> GetGroupRequestsForGroup(int groupId);
        bool RequestSent(string userId, int groupId);
        void DeleteGroupRequest(string userId, int groupId);
        void ApproveRequest(int id, string userid);
        void DeleteGroupRequest(int id);
        void SaveGroupRequest();
    }

    public class GroupRequestService : IGroupRequestService
    {
        private readonly IGroupRequestRepository groupRequestRepository;
        private readonly IUnitOfWork unitOfWork;
        public GroupRequestService(IGroupRequestRepository groupRequestRepository, IUnitOfWork unitOfWork)
        {
            this.groupRequestRepository = groupRequestRepository;
            this.unitOfWork = unitOfWork;
        }
        #region IGroupRequestService Members

        public IEnumerable<GroupRequest> GetGroupRequests()
        {
            var groupRequest = groupRequestRepository.GetAll();
            return groupRequest;
        }

        public GroupRequest GetGroupRequest(int id)
        {
            var groupRequest = groupRequestRepository.GetById(id);
            return groupRequest;
        }

        public void CreateGroupRequest(GroupRequest groupRequest)
        {
            var oldgroup = GetGroupRequests().Where(g => g.UserId == groupRequest.UserId && g.GroupId == groupRequest.GroupId);
            if (oldgroup.Count() == 0)
            {
                groupRequestRepository.Add(groupRequest);
                SaveGroupRequest();
            }
        }

        public void DeleteGroupRequest(int id)
        {
            var groupRequest = groupRequestRepository.GetById(id);
            groupRequestRepository.Delete(groupRequest);
            SaveGroupRequest();
        }

        public bool RequestSent(string userId, int groupId)
        {
            var groupRequests = groupRequestRepository.GetMany(g => g.UserId == userId && g.GroupId == groupId && g.Accepted==false);
            if (groupRequests.Count() == 1)
            {
                return true;
            }
            else return false;

        }
        public IEnumerable<GroupRequest>GetGroupRequests(int groupId)
        {
            var groupRequests = groupRequestRepository.GetMany(g => g.GroupId == groupId && g.Accepted == false);
            return groupRequests;
        }

        public void ApproveRequest(int id, string userid)
        {
            var groupRequest = groupRequestRepository.Get(g => (g.GroupId == id && g.UserId == userid));
            if (groupRequest != null)
            {
                groupRequestRepository.Delete(groupRequest);
                //groupInvitation.Accepted = true;
                //GroupInvitationRepository.Update(groupInvitation);
                SaveGroupRequest();
            }
        }

        public void DeleteGroupRequest(string userId, int groupId)
        {
            var groupRequest=groupRequestRepository.Get(g => g.UserId == userId && g.GroupId == groupId);
            DeleteGroupRequest(groupRequest.GroupRequestId);

        }
        //public void AcceptInvitation(int id, int userid)
        //{
        //    var groupInvitation = GroupInvitationRepository.Get(g => (g.GroupId == id && g.ToUserId == userid));
        //    if (groupInvitation != null)
        //    {
        //        GroupInvitationRepository.Delete(groupInvitation);
        //        //groupInvitation.Accepted = true;
        //        //GroupInvitationRepository.Update(groupInvitation);
        //        SaveGroupInvitation();
        //    }
        //}
        //public IEnumerable<GroupInvitation> GetGroupInvitationsForGroup(int groupId)
        //{
        //    return from g in GetGroupInvitations() where g.GroupId == groupId select g;
        //}

        public IEnumerable<GroupRequest> GetGroupRequestsForGroup(int groupId)
        {
            return from g in GetGroupRequests() where g.GroupId==groupId && g.Accepted == false select g;
        }

        //public bool IsUserInvited(int groupId, int userId)
        //{
        //    return GroupInvitationRepository.Get(g => g.ToUserId == userId && g.GroupId == groupId) != null;
        //}

        public void SaveGroupRequest()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
