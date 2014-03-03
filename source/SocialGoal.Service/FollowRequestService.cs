using System.Collections.Generic;
using System.Linq;
using SocialGoal.Model.Models;
using SocialGoal.Data.Repository;
using SocialGoal.Data.Infrastructure;

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
        private readonly IFollowRequestRepository _followRequestRepository;
        private readonly IUnitOfWork _unitOfWork;
        public FollowRequestService(IFollowRequestRepository followRequestRepository, IUnitOfWork unitOfWork)
        {
            _followRequestRepository = followRequestRepository;
            _unitOfWork = unitOfWork;
        }
        #region IGroupRequestService Members

        public IEnumerable<FollowRequest> GetFollowRequests()
        {
            var followRequest = _followRequestRepository.GetAll();
            return followRequest;
        }

        public FollowRequest GetFollowRequest(int id)
        {
            var followRequest = _followRequestRepository.GetById(id);
            return followRequest;
        }

        public void CreateFollowRequest(FollowRequest followRequest)
        {
            var oldfollow = GetFollowRequests().Where(g => g.ToUserId == followRequest.ToUserId && g.FromUserId == followRequest.FromUserId);
            if (oldfollow.Count() == 0)
            {
                _followRequestRepository.Add(followRequest);
                SaveFollowRequest();
            }
        }

        public void DeleteFollowRequest(int id)
        {
            var followRequest = _followRequestRepository.GetById(id);
            _followRequestRepository.Delete(followRequest);
            SaveFollowRequest();
        }

        public bool RequestSent(string fromuserId, string touserid)
        {
            var followRequests = _followRequestRepository.GetMany(g =>( g.FromUserId == fromuserId && g.ToUserId == touserid && g.Accepted == false));
            if (followRequests.Count() == 1)
            {
                return true;
            }
            return false;
        }
        public IEnumerable<FollowRequest> GetFollowRequests(string followId)
        {
            var followRequests = _followRequestRepository.GetMany(g => g.ToUserId == followId && g.Accepted == false);
            return followRequests;
        }

        public void ApproveRequest(string id, string userid)
        {
            var followRequest = _followRequestRepository.Get(g => (g.ToUserId == id && g.FromUserId == userid));
            if (followRequest != null)
            {
                _followRequestRepository.Delete(followRequest);
                SaveFollowRequest();
            }
        }

        public void DeleteFollowRequest(string fromuserId, string toUserId)
        {
            var followRequest = _followRequestRepository.Get(g => (g.FromUserId == fromuserId && g.ToUserId == toUserId));
            DeleteFollowRequest(followRequest.FollowRequestId);

        }

        public IEnumerable<FollowRequest> GetFollowRequestsForUser(string userid)
        {
            return from g in GetFollowRequests() where g.ToUserId == userid && g.Accepted == false select g;
        }

        public void SaveFollowRequest()
        {
            _unitOfWork.Commit();
        }

        #endregion
    }
}
