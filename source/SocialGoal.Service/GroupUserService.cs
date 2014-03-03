﻿using System.Collections.Generic;
using System.Linq;
using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
using SocialGoal.Data.Repository;

namespace SocialGoal.Service
{

    public interface IGroupUserService
    {
        IEnumerable<GroupUser> GetGroupUsers();
        IEnumerable<int> GetGroupUsers(string userid);
        IEnumerable<int> GetGroupUsers(IEnumerable<string> userid);
        IEnumerable<GroupUser> GetTop20GroupsUsersForProfile(string userid);
        IEnumerable<GroupUser> GetTop20GroupsUsers(string userid);
        bool CanInviteUser(string userId, int groupId);
        GroupUser GetGroupUser(int id);
        GroupUser GetGroupUserByuserId(string id);
        GroupUser GetGroupUser(string userId, int groupId);
        void CreateGroupUserFromRequest(GroupUser groupUser, IGroupRequestService groupRequestService);
        IEnumerable<int> GetFollowedGroups(string userid);
        //IEnumerable<Group> GetGroups(string email);
        IEnumerable<GroupUser> GetGroupUsersByGroup(int groupId);
        IEnumerable<int> GetGroupAdminUsers(string userid);
        IEnumerable<GroupUser> GetGroupUsersList(int groupId);
        IEnumerable<GroupUser> GetGroupUsersListToAssign(int groupId, string currentUserId);
        IEnumerable<ApplicationUser> GetMembersOfGroup(int groupId);
        IEnumerable<string> GetUserIdByGroupUserId(IEnumerable<int> groupuserid);
        int GetGroupUsersCount(int groupId);
        string GetAdminId(int groupId);

        void CreateGroupUser(GroupUser groupUser, IGroupInvitationService groupInvitationService);
        void DeleteGroupUserByGroupId(int groupid);
        void DeleteGroupUser(int id);
        void SaveGroupUser();
        IEnumerable<ApplicationUser> SearchUserForGroup(string searchString, int groupId, IUserService userService, IGroupInvitationService groupInvitationService);
    }

    public class GroupUserService : IGroupUserService
    {
        private readonly IGroupUserRepository _groupUserRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GroupUserService(IGroupUserRepository groupUserRepository,IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _groupUserRepository = groupUserRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        #region IGroupUserService Members

        public IEnumerable<GroupUser> GetGroupUsers()
        {
            var groupUser = _groupUserRepository.GetAll();
            return groupUser;
        }

        public GroupUser GetGroupUser(string userId, int groupId)
        {
            return _groupUserRepository.Get(gu => gu.UserId == userId && gu.GroupId == groupId);

        }

        public IEnumerable<int> GetGroupUsers(string userid)
        {
            var groupids = new List<int>();
            var groupUsers = _groupUserRepository.GetMany(g => g.UserId == userid).OrderByDescending(g => g.GroupId).ToList();
            foreach (var item in groupUsers)
            {
                var ids = item.GroupId;
                groupids.Add(ids);
            }

            return groupids;
        }

        public IEnumerable<int> GetGroupAdminUsers(string userid)
        {
            var groupIds = new List<int>();
            var groupUsers = _groupUserRepository.GetMany(g => (g.UserId == userid) && g.Admin).OrderByDescending(g => g.GroupUserId).ToList();
            foreach (GroupUser item in groupUsers)
            {
                var groupId = item.GroupId;
                groupIds.Add(groupId);
            }
            return groupIds;
        }
        public IEnumerable<GroupUser> GetTop20GroupsUsers(string userid)
        {
            var groupids = GetGroupUsers(userid);
            var userids = GetGroupMembers(groupids);

            //var users = groupUserRepository.GetAll().OrderByDescending(g => g.GroupUserId).Take(20).ToList();
            return userids;
        }

        public IEnumerable<GroupUser> GetTop20GroupsUsersForProfile(string userid)
        {

            var users = _groupUserRepository.GetMany(g => g.UserId == userid).OrderByDescending(g => g.GroupUserId).Take(20).ToList();

            return users;
        }

        public int GetGroupUsersCount(int groupId)
        {
            return _groupUserRepository.GetMany(g => g.GroupId == groupId).Count();
        }

        public IEnumerable<GroupUser> GetGroupUsersList(int groupId)
        {
            var groupUsers = _groupUserRepository.GetMany(g => g.GroupId == groupId).OrderByDescending(g => g.GroupUserId).ToList();
            return groupUsers;
        }

        public IEnumerable<GroupUser> GetGroupUsersListToAssign(int groupId, string currentUserId)
        {
            var groupUsers = _groupUserRepository.GetMany(g => g.GroupId == groupId && g.UserId!=currentUserId).OrderByDescending(g => g.GroupUserId).ToList();
            return groupUsers;
        }

        public IEnumerable<ApplicationUser> SearchUserForGroup(string searchString, int groupId, IUserService userService, IGroupInvitationService groupInvitationService)
        {
            var users = from u in userService.GetUsers(searchString)
                        where !(from g in GetGroupUsersByGroup(groupId) select g.UserId).Contains(u.Id) &&
                        !(groupInvitationService.IsUserInvited(groupId, u.Id))
                        select u;
            return users;
        }

        public IEnumerable<GroupUser> GetGroupUsersByGroup(int groupId)
        {
            var groupUsers = _groupUserRepository.GetMany(g => g.GroupId == groupId).OrderBy(g => g.UserId).ToList();
            return groupUsers;
        }

        
        public GroupUser GetGroupUser(int id)
        {
            var groupUser = _groupUserRepository.GetById(id);
            return groupUser;
        }

        public GroupUser GetGroupUserByuserId(string id)
        {
            var user = _groupUserRepository.Get(g => g.UserId == id);
            return user;
        }
        public IEnumerable<int> GetFollowedGroups(string userid)
        {
            var groupIds = new List<int>();
            var groupUsers = _groupUserRepository.GetMany(g => (g.UserId == userid) && (g.Admin == false)).OrderByDescending(g => g.GroupUserId).ToList();
            foreach (GroupUser item in groupUsers)
            {
                var groupId = item.GroupId;
                groupIds.Add(groupId);
            }
            return groupIds;
        }

        public IEnumerable<ApplicationUser> GetMembersOfGroup(int groupId)
        {
            var users = from u in _userRepository.GetAll()
                        join gu in
                            (from g in _groupUserRepository.GetMany(gr=>gr.GroupId==groupId) select g) on u.Id equals gu.UserId
                        select u;
            return users;
        }

        public void CreateGroupUser(GroupUser groupUser, IGroupInvitationService groupInvitationService)
        {
            var oldUser = _groupUserRepository.GetMany(g => g.UserId == groupUser.UserId && g.GroupId == groupUser.GroupId);
            if (oldUser.Count() == 0)
            {
                _groupUserRepository.Add(groupUser);
                SaveGroupUser();
            }
            groupInvitationService.AcceptInvitation(groupUser.GroupId, groupUser.UserId);
        }

        public void CreateGroupUserFromRequest(GroupUser groupUser, IGroupRequestService groupRequestService)
        {
            var oldUser = _groupUserRepository.GetMany(g => g.UserId == groupUser.UserId && g.GroupId == groupUser.GroupId);
            if (oldUser.Count() == 0)
            {
                _groupUserRepository.Add(groupUser);
                SaveGroupUser();
            }
            groupRequestService.ApproveRequest(groupUser.GroupId, groupUser.UserId);
        }

        public IEnumerable<int> GetGroupUsers(IEnumerable<string> userid)
        {
            var groupsIds = new List<int>();
            foreach (var item in userid)
            {
                var groupUsers = _groupUserRepository.GetMany(g => g.UserId == item);
                foreach (GroupUser gruser in groupUsers)
                {
                    groupsIds.Add(gruser.GroupId);
                }
                
            }
            return groupsIds;

        }

        public IEnumerable<GroupUser> GetGroupMembers(IEnumerable<int> groupid)
        {
            var users = new List<GroupUser>();
            foreach (int item in groupid)
            {
                var groupUsers = _groupUserRepository.Get(g => g.GroupId == item);
                users.Add(groupUsers);

            }
            return users;

        }

        public string GetAdminId(int groupId)
        {
            return _groupUserRepository.Get(g => g.GroupId == groupId && g.Admin).UserId;
        }


        public bool CanInviteUser(string userId, int groupId)
        {
            var groupUser = _groupUserRepository.Get(g => g.GroupId == groupId && g.UserId == userId);
            if (groupUser != null)
                return false;
            return true;
        }


        public IEnumerable<string>GetUserIdByGroupUserId(IEnumerable<int> groupuserid)
        {
            var users = new List<string>();
            foreach (int item in groupuserid)
            {
                var groupUsers = _groupUserRepository.Get(g => g.GroupUserId == item).UserId;
                users.Add(groupUsers);

            }
            return users;
        }
        public void DeleteGroupUser(int id)
        {
            var groupUser = _groupUserRepository.GetById(id);
            _groupUserRepository.Delete(groupUser);
            SaveGroupUser();
        }

        public void DeleteGroupUserByGroupId(int groupid)
        {
            var groupuser = GetGroupUsersByGroup(groupid);
            foreach (var item in groupuser)
            {
                DeleteGroupUser(item.GroupUserId);
            }
        }
        public void SaveGroupUser()
        {
            _unitOfWork.Commit();
        }

        #endregion
    }
}
