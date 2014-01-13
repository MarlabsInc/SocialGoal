using System.Collections.Generic;
using System.Linq;
using SocialGoal.Model.Models;
using SocialGoal.Data.Repository;
using SocialGoal.Data.Infrastructure;
using System;

namespace SocialGoal.Service
{
    public interface IFollowRequestService
    {
        IEnumerable<FollowRequest> GetFollowRequests();
        FollowRequest GetFollowRequest(int id);
        IEnumerable<FollowRequest> GetFollowRequests(string userId);
        void CreateFollowRequest(FollowRequest followRequest);
        IEnumerable<FollowRequest> GetFollowRequestsForUser(string userid);
        bool RequestSent(string fromuserId, string touserid);
        void DeleteFollowRequest(string fromuserId, string touserid);
        void ApproveRequest(string id, string userid);
        void DeleteFollowRequest(int id);
       void SaveFollowRequest();
    }

    public class FollowRequestService : IFollowRequestService
    {
        private readonly IFollowRequestRepository followRequestRepository;
        private readonly IUnitOfWork unitOfWork;
        public FollowRequestService(IFollowRequestRepository followRequestRepository, IUnitOfWork unitOfWork)
        {
            this.followRequestRepository = followRequestRepository;
            this.unitOfWork = unitOfWork;
        }
        #region IGroupRequestService Members

        public IEnumerable<FollowRequest> GetFollowRequests()
        {
            var followRequest = followRequestRepository.GetAll();
            return followRequest;
        }

        public FollowRequest GetFollowRequest(int id)
        {
            var followRequest = followRequestRepository.GetById(id);
            return followRequest;
        }

        public void CreateFollowRequest(FollowRequest followRequest)
        {
            var oldfollow = GetFollowRequests().Where(g => g.ToUserId == followRequest.ToUserId && g.FromUserId == followRequest.FromUserId);
            if (oldfollow.Count() == 0)
            {
                followRequestRepository.Add(followRequest);
                SaveFollowRequest();
            }
        }

        public void DeleteFollowRequest(int id)
        {
            var followRequest = followRequestRepository.GetById(id);
            followRequestRepository.Delete(followRequest);
            SaveFollowRequest();
        }

        public bool RequestSent(string fromuserId, string touserid)
        {
            var followRequests = followRequestRepository.GetMany(g =>( g.FromUserId == fromuserId && g.ToUserId == touserid && g.Accepted == false));
            if (followRequests.Count() == 1)
            {
                return true;
            }
            else return false;

        }
        public IEnumerable<FollowRequest> GetFollowRequests(string followId)
        {
            var followRequests = followRequestRepository.GetMany(g => g.ToUserId == followId && g.Accepted == false);
            return followRequests;
        }

        public void ApproveRequest(string id, string userid)
        {
            var followRequest = followRequestRepository.Get(g => (g.ToUserId == id && g.FromUserId == userid));
            if (followRequest != null)
            {
                followRequestRepository.Delete(followRequest);
                //groupInvitation.Accepted = true;
                //GroupInvitationRepository.Update(groupInvitation);
                SaveFollowRequest();
            }
        }

        public void DeleteFollowRequest(string fromuserId, string toUserId)
        {
            var FollowRequest = followRequestRepository.Get(g => (g.FromUserId == fromuserId && g.ToUserId == toUserId));
            DeleteFollowRequest(FollowRequest.FollowRequestId);

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

        public IEnumerable<FollowRequest> GetFollowRequestsForUser(string userid)
        {
            return from g in GetFollowRequests() where g.ToUserId == userid && g.Accepted == false select g;
        }

        //public bool IsUserInvited(int groupId, int userId)
        //{
        //    return GroupInvitationRepository.Get(g => g.ToUserId == userId && g.GroupId == groupId) != null;
        //}

        public void SaveFollowRequest()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
