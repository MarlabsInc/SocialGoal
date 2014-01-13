using System.Collections.Generic;
using System.Linq;
using SocialGoal.Model.Models;
using SocialGoal.Data.Repository;
using SocialGoal.Data.Infrastructure;
using SocialGoal.Core.Common;
using SocialGoal.Service.Properties;
using System;

namespace SocialGoal.Service
{
    public interface IGroupService
    {
        IEnumerable<Group> GetGroups();
        IEnumerable<Group> GetGroups(IEnumerable<int> id);
        IEnumerable<Group> GetGroupsForUser(IEnumerable<int> groupIds);
        IEnumerable<Group> GetTop20Groups(IEnumerable<int> id);
        IEnumerable<Group> SearchGroup(string group);
        Group GetGroup(string groupname);
        Group GetGroup(int id);
        Group CreateGroup(Group group, string userId);
        void UpdateGroup(Group group);
        void DeleteGroup(int id);
        void SaveGroup();
        IEnumerable<ValidationResult> CanAddGroup(Group group);

        /// <summary>
        /// Getting groups by page
        /// </summary>
        /// <param name="filterBy"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        IEnumerable<Group> GetGroupsByPage(string userId, string filterBy, int noOfrecods, int page);
    }

    public class GroupService : IGroupService
    {
        private readonly IGroupRepository groupRepository;
        private readonly IFollowUserRepository followUserrepository;
        private readonly IGroupUserRepository groupUserrepository;
        private readonly IUnitOfWork unitOfWork;

        public GroupService(IGroupRepository groupRepository, IFollowUserRepository followUserrepository, IGroupUserRepository groupUserrepository, IUnitOfWork unitOfWork)
        {
            this.groupRepository = groupRepository;
            this.groupUserrepository = groupUserrepository;
            this.followUserrepository = followUserrepository;
            this.unitOfWork = unitOfWork;
        }

        #region IGroupService Members

        public IEnumerable<Group> GetGroups()
        {
            var groups = groupRepository.GetAll().OrderByDescending(g => g.CreatedDate);
            return groups;
        }

        public IEnumerable<Group> GetGroups(IEnumerable<int> id)
        {
            List<Group> groups = new List<Group> { };
            foreach (int item in id)
            {
                var group = GetGroup(item);
                if(!groups.Contains(group))
                {
                groups.Add(group);
                }
            }
            return groups;

        }


        public IEnumerable<Group> GetGroupsForUser(IEnumerable<int> groupIds)
        {
            List<Group> groups=new List<Group>{};
            foreach(var item in groupIds)
            {
                var group=groupRepository.GetById(item);
                groups.Add(group);
            }
            return groups;
        }

        public Group GetGroup(string groupname)
        {
            var group = groupRepository.Get(g => g.GroupName == groupname);
            return group;
        }


        public Group GetGroup(int id)
        {
            var group = groupRepository.GetById(id);
            return group;
        }

        public IEnumerable<Group> GetTop20Groups(IEnumerable<int> id)
        {
            List<Group> groups = new List<Group> { };
            foreach (int item in id)
            {
                var group = GetGroup(item);
                groups.Add(group);
            }
            //var goals = groupRepository.GetAll().OrderByDescending(g => g.CreatedDate).Take(20).ToList();
            return groups.OrderByDescending(g => g.CreatedDate).Take(20).ToList();
        }

        public IEnumerable<Group> SearchGroup(string group)
        {
            return groupRepository.GetMany(g => g.GroupName.ToLower().Contains(group.ToLower())).OrderBy(g => g.GroupName);
        }

        public Group CreateGroup(Group group, string userId)
        {
            groupRepository.Add(group);
            SaveGroup();

            var groupUser = new GroupUser { GroupId = group.GroupId, UserId = userId,Admin=true };
            try
            {
                groupUserrepository.Add(groupUser);
                SaveGroup();
            }
            catch
            {
                groupRepository.Delete(group);
                SaveGroup();
            }            
            return group;
        }

        public void UpdateGroup(Group group)
        {
            groupRepository.Update(group);
            SaveGroup();
        }

        public void DeleteGroup(int id)
        {
            var group = groupRepository.GetById(id);
            groupRepository.Delete(group);
            groupUserrepository.Delete(gu => gu.GroupId == id);
            SaveGroup();
        }

        public IEnumerable<ValidationResult> CanAddGroup(Group newGroup)
        {
            Group group;
            if (newGroup.GroupId == 0)
                group = groupRepository.Get(g => g.GroupName == newGroup.GroupName);
            else
                group = groupRepository.Get(g => g.GroupName == newGroup.GroupName && g.GroupId != newGroup.GroupId);
            if (group != null)
            {
                yield return new ValidationResult("GroupName", Resources.GroupExists);
            }
        }



        /// <summary>
        /// For getting groups by page
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="filterBy"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public IEnumerable<Group> GetGroupsByPage(string userId, string filterBy, int noOfrecods, int page)
        {
            var skipGroup = noOfrecods * page;
            var groupList = GetGroups();


            if (filterBy =="My Followings Groups")
            {
                List<string> userIds = new List<string> { };
                var followings = followUserrepository.GetMany(g => g.FromUserId == userId).ToList();
                foreach (var item in followings)
                {
                    var ids = item.ToUserId;
                    userIds.Add(ids);
                }
                List<int> groupsIds = new List<int> { };
                foreach (var item in userIds)
                {
                    var groupUsers = groupUserrepository.GetMany(g => g.UserId == item);
                    foreach (GroupUser gruser in groupUsers)
                    {
                        groupsIds.Add(gruser.GroupId);
                    }
                }
                groupList = this.GetGroups(groupsIds);
            }
            else if (filterBy =="My Groups")
            {                

                groupList=from g in groupRepository.GetAll()
                           join gu in groupUserrepository.GetMany(gru=>gru.UserId==userId && gru.Admin==true) on g.GroupId equals gu.GroupId
                           orderby g.CreatedDate
                           select g ;            

            }
            else if (filterBy == "My Followed Groups")
            {

                List<int> groupIds = new List<int> { };
                var groupUsers = groupUserrepository.GetMany(g => (g.UserId == userId) && (g.Admin == false)).OrderByDescending(g => g.GroupUserId).ToList();
                foreach (GroupUser item in groupUsers)
                {
                    var groupId = item.GroupId;
                    groupIds.Add(groupId);
                }

                groupList = this.GetGroups(groupIds);
            }

            groupList = groupList.Skip(skipGroup).Take(noOfrecods);
            return groupList;
        }


        public void SaveGroup()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
