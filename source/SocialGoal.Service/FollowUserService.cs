using System.Collections.Generic;
using System.Linq;
using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
using SocialGoal.Data.Repository;
using SocialGoal.Core.Common;
using SocialGoal.Service.Properties;


namespace SocialGoal.Service
{
    public interface IFollowUserService
    {
        void CreateFollowUserFromRequest(FollowUser followUser, IFollowRequestService followRequestService);
        bool IsFollowing(string fromuserid, string touserid);
        IEnumerable<string> GetFollowingUsers(string userid);
        IEnumerable<ApplicationUser> GetFollowers(string userId);
        IEnumerable<ApplicationUser> GetFollowings(string userId);
        IEnumerable<FollowUser> GetTop20Followers(string userid);
        void DeleteFollowUser(string toid, string fromid);
        IEnumerable<FollowUser> Following(string userid);
        void SaveFollowUser();

        /// <summary>
        /// Get follower users by page
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="currentPage"></param>
        /// <param name="noofRecords"></param>
        /// <returns></returns>
        IEnumerable<ApplicationUser> GetFollowers(string userId, int currentPage, int noofRecords);

        /// <summary>
        /// get follwing users by page
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="currentPage"></param>
        /// <param name="noofRecords"></param>
        /// <returns></returns>
        IEnumerable<ApplicationUser> GetFollowings(string userId, int currentPage, int noofRecords);
    }

    public class FollowUserService : IFollowUserService
    {

        private readonly IFollowUserRepository followUserRepository;
        private readonly IUnitOfWork unitOfWork;

        public FollowUserService(IFollowUserRepository followUserRepository, IUnitOfWork unitOfWork)
        {
            this.followUserRepository = followUserRepository;
            this.unitOfWork = unitOfWork;
        }

        #region IGroupUserService Members

        public void CreateFollowUserFromRequest(FollowUser followUser, IFollowRequestService groupRequestService)
        {
            var oldUser = followUserRepository.GetMany(g => g.FromUserId == followUser.FromUserId && g.ToUserId == followUser.ToUserId);
            if (oldUser.Count() == 0)
            {
                followUserRepository.Add(followUser);
                SaveFollowUser();
            }
            groupRequestService.ApproveRequest(followUser.ToUserId, followUser.FromUserId);
        }
        public bool IsFollowing(string fromuserid, string touserid)
        {
            var user = followUserRepository.GetMany(f => (f.FromUserId == fromuserid && f.ToUserId == touserid));
            if (user.Count() == 1)
            {
                return true;
            }
            else return false;


        }
        public IEnumerable<string> GetFollowingUsers(string userid)
        {
            List<string> followingsids = new List<string> { };
            var followings = followUserRepository.GetMany(g => g.FromUserId == userid).ToList();
            foreach (var item in followings)
            {
                var ids = item.ToUserId;
                followingsids.Add(ids);
            }

            return followingsids;
        }
        public IEnumerable<FollowUser> GetTop20Followers(string userid)
        {
            var user = from f in followUserRepository.GetAll() where (from g in followUserRepository.GetMany(fol => fol.FromUserId == userid) select g.ToUserId).ToList().Contains(f.FromUserId) select f;
            return user;
        }
        public IEnumerable<FollowUser> Following(string userid)
        {
            var user = followUserRepository.GetMany(f => f.FromUserId == userid);
            return user;
        }


        public IEnumerable<ApplicationUser> GetFollowers(string userId)
        {
            var followers = from u in followUserRepository.GetMany(f => f.ToUserId == userId) select u.FromUser;
            return followers;
        }

        /// <summary>
        /// Get followers by page
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="currentPage"></param>
        /// <param name="noofRecords"></param>
        /// <returns></returns>
        public IEnumerable<ApplicationUser> GetFollowers(string userId, int currentPage, int noofRecords)
        {
            var skipFollowers = noofRecords * currentPage;
            var followers = from u in followUserRepository.GetMany(f => f.ToUserId == userId) select u.FromUser;

            followers = followers.Skip(skipFollowers).Take(noofRecords);
            return followers;
        }





        public IEnumerable<ApplicationUser> GetFollowings(string userId)
        {
            var followings = from u in followUserRepository.GetMany(f => f.FromUserId == userId) select u.ToUser;
            return followings;
        }

        /// <summary>
        /// Get following users by page
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="currentPage"></param>
        /// <param name="noofRecords"></param>
        /// <returns></returns>
        public IEnumerable<ApplicationUser> GetFollowings(string userId, int currentPage, int noofRecords)
        {
            var skipFollowings = noofRecords * currentPage;
            var followings = from u in followUserRepository.GetMany(f => f.FromUserId == userId) select u.ToUser;

            followings = followings.Skip(skipFollowings).Take(noofRecords);
            return followings;
        }


        public void DeleteFollowUser(string toid, string fromid)
        {
            var followUser = followUserRepository.Get(f => (f.FromUserId == fromid && f.ToUserId == toid));
            followUserRepository.Delete(followUser);
            SaveFollowUser();
        }
        public void SaveFollowUser()
        {
            unitOfWork.Commit();
        }
        #endregion
    }
}
