using System.Drawing.Drawing2D;
using AutoMapper;
using PagedList;
using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
using SocialGoal.Service;
using SocialGoal.Web.Core.Models;
using SocialGoal.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SocialGoal.Web.Core.Extensions;
using SocialGoal.Web.Mailers;
using SocialGoal.Properties;
using Microsoft.AspNet.Identity;

namespace SocialGoal.Web.Controllers
{
    [Authorize]
    public class GroupController : Controller
    {
        private readonly IGroupService groupService;
        private readonly IGroupUserService groupUserService;
        private readonly IMetricService metricService;
        private readonly IFocusService focusService;
        private readonly IGroupGoalService groupGoalService;
        private readonly IUserService userService;
        private readonly IGroupInvitationService groupInvitationService;
        private readonly ISecurityTokenService securityTokenService;
        private readonly IGroupUpdateService groupUpdateService;
        private readonly IGroupCommentService groupCommentService;
        private readonly IGoalStatusService goalStatusService;
        public readonly IGroupRequestService groupRequestService;
        private readonly IFollowUserService followUserService;
        private readonly IGroupCommentUserService groupCommentUserService;
        private readonly IGroupUpdateSupportService groupUpdateSupportService;
        private readonly IGroupUpdateUserService groupUpdateUserService;

        private IUserMailer userMailer = new UserMailer();
        public IUserMailer UserMailer
        {
            get { return userMailer; }
            set { userMailer = value; }
        }

        public GroupController(IGroupService groupService, IGroupUserService groupUserService, IUserService userService, IMetricService metricService, IFocusService focusService, IGroupGoalService groupgoalService, IGroupInvitationService groupInvitationService, ISecurityTokenService securityTokenService, IGroupUpdateService groupUpdateService, IGroupCommentService groupCommentService, IGoalStatusService goalStatusService, IGroupRequestService groupRequestService, IFollowUserService followUserService, IGroupCommentUserService groupCommentUserService, IGroupUpdateSupportService groupUpdateSupportService, IGroupUpdateUserService groupUpdateUserService)
        {
            this.groupService = groupService;
            this.groupInvitationService = groupInvitationService;
            this.userService = userService;
            this.groupUserService = groupUserService;
            this.metricService = metricService;
            this.focusService = focusService;
            this.groupGoalService = groupgoalService; ;
            this.securityTokenService = securityTokenService;
            this.groupUpdateService = groupUpdateService;
            this.groupCommentService = groupCommentService;
            this.goalStatusService = goalStatusService;
            this.groupRequestService = groupRequestService;
            this.followUserService = followUserService;
            this.groupCommentUserService = groupCommentUserService;
            this.groupUpdateSupportService = groupUpdateSupportService;
            this.groupUpdateUserService = groupUpdateUserService;
        }
        //
        // GET: /Group/

        public ViewResult Index(int id)
        {
            GroupViewModel group = Mapper.Map<Group, GroupViewModel>(groupService.GetGroup(id));
            group.Goals = Mapper.Map<IEnumerable<GroupGoal>, IEnumerable<GroupGoalViewModel>>(groupGoalService.GetGroupGoals(id));

            foreach (var item in group.Goals)
            {
                var user = userService.GetUser(item.GroupUser.UserId);

                item.UserId = user.Id;
                item.User = user;
            }
            var assignedgroupuser = groupUserService.GetGroupUser(User.Identity.GetUserId(), id);
            if (assignedgroupuser != null)
            {
                group.GoalsAssignedToOthers = Mapper.Map<IEnumerable<GroupGoal>, IEnumerable<GroupGoalViewModel>>(groupGoalService.GetAssignedGoalsToOthers(assignedgroupuser.GroupUserId));
                group.GoalsAssignedToMe = Mapper.Map<IEnumerable<GroupGoal>, IEnumerable<GroupGoalViewModel>>(groupGoalService.GetAssignedGoalsToMe(assignedgroupuser.GroupUserId));
            }
            group.Focus = focusService.GetFocussOFGroup(id);

            //group.GroupUserId = groupUserService.GetGroupUserByuserId(((SocialGoalUser)(User.Identity)).UserId).GroupUserId;
            group.Users = groupUserService.GetMembersOfGroup(id);
            //if (group.GroupUser.UserId == ((SocialGoalUser)(User.Identity)).UserId)
            if (groupUserService.GetAdminId(id) == User.Identity.GetUserId())
                group.Admin = true;
            var status = 0;
            foreach (var item in group.Users)
            {
                if (item.Id == (User.Identity.GetUserId()))
                    status = 1;
            }
            if (status == 1)
                group.IsAMember = true;
            if (groupRequestService.RequestSent((User.Identity.GetUserId()), id))
                group.RequestSent = true;
            if (groupInvitationService.IsUserInvited(id, (User.Identity.GetUserId())))
                group.InvitationSent = true;
            return View("Index", group);
        }

        public ViewResult MyGroups()
        {
            var groupids = groupUserService.GetGroupAdminUsers(User.Identity.GetUserId());
            var allGroups = groupService.GetGroups(groupids);
            return View(allGroups);
        }

        public ViewResult Following()
        {
            var groupids = groupUserService.GetFollowedGroups(User.Identity.GetUserId());
            var allGroups = groupService.GetGroups(groupids);
            return View(allGroups);
        }

        public ViewResult JoinedUsers(int id)
        {
            var members = groupUserService.GetMembersOfGroup(id);
            GroupMemberViewModel gmvm = new GroupMemberViewModel() { GroupId = id, Group = groupService.GetGroup(id), Users = members };
            return View(gmvm);
        }

        //
        // GET: /Group/Create
        public PartialViewResult CreateGroup()
        {
            var groupFormViewModel = new GroupFormModel();
            return PartialView(groupFormViewModel);
        }

        public ViewResult ShowAllRequests(int id)
        {
            var groupRequests = groupRequestService.GetGroupRequests(id);
            var groupRequestViewModel = Mapper.Map<IEnumerable<GroupRequest>, IEnumerable<GroupRequestViewModel>>(groupRequests);
            ViewBag.CurrentGroupID = id;
            return View("_RequestsView", groupRequestViewModel);
        }

        public ViewResult Members(int id)
        {
            var members = groupUserService.GetMembersOfGroup(id);
            GroupMemberViewModel gmvm = new GroupMemberViewModel() { GroupId = id, Group = groupService.GetGroup(id), Users = members };
            return View(gmvm);
        }

        public ActionResult DeleteMember(string userId, int groupId)
        {
            var membertodelete = groupUserService.GetGroupUser(userId, groupId);
            groupUserService.DeleteGroupUser(membertodelete.GroupUserId);
            return RedirectToAction("Index", new { id = membertodelete.GroupId });
        }

        //
        // POST: /Group/Create

        [HttpPost]
        public ActionResult CreateGroup(GroupFormModel newGroup)
        {
            var userId = User.Identity.GetUserId();
            Group group = Mapper.Map<GroupFormModel, Group>(newGroup);
            var errors = groupService.CanAddGroup(group).ToList();
            ModelState.AddModelErrors(errors);
            if (ModelState.IsValid)
            {
                //group.UserId = ((SocialGoalUser)(User.Identity)).UserId;
                var createdGroup = groupService.CreateGroup(group, userId);
                //var createdGroup = groupService.GetGroup(newGroup.GroupName);
                //var groupAdmin = new GroupUser { GroupId = createdGroup.GroupId, UserId = ((SocialGoalUser)(User.Identity)).UserId, Admin = true };
                //groupUserService.CreateGroupUser(groupAdmin, groupInvitationService);
                return RedirectToAction("Index", new { id = createdGroup.GroupId });
            }
            return View("CreateGroup", newGroup);
        }

        //
        // GET: /Group/Edit
        public ActionResult EditGroup(int id)
        {
            var group = groupService.GetGroup(id);
            GroupFormModel editGroup = Mapper.Map<Group, GroupFormModel>(group);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View("_EditGroup", editGroup);
        }

        public ViewResult CreateFocus(int id)
        {
            return View(new FocusFormModel() { GroupId = id, Group = groupService.GetGroup(id) });
        }

        [HttpPost]
        public ActionResult CreateFocus(FocusFormModel focus)
        {
            var errors = focusService.CanAddFocus(Mapper.Map<FocusFormModel, Focus>(focus)).ToList();

            ModelState.AddModelErrors(errors);
            if (ModelState.IsValid)
            {
                Focus newFocus = Mapper.Map<FocusFormModel, Focus>(focus);
                focusService.CreateFocus(newFocus);
                //var createdfocus = focusService.GetFocus(focus.FocusName);
                return RedirectToAction("Focus", new { id = newFocus.FocusId });
            }
            return View("CreateFocus", focus);
        }

        public ActionResult Focus(int id)
        {
            FocusViewModel Focus = Mapper.Map<Focus, FocusViewModel>(focusService.GetFocus(id));
            Focus.GroupGoal = Mapper.Map<IEnumerable<GroupGoal>, IEnumerable<GroupGoalViewModel>>(groupGoalService.GetGroupGoalsByFocus(id));
            foreach (var item in Focus.GroupGoal)
            {
                var user = userService.GetUser(item.GroupUser.UserId);
                item.UserId = user.Id;
                item.User = user;
            }
            //Focus.GroupGoal = groupGoalService.GetGroupGoalsByFocus(id);
            Focus.Users = groupUserService.GetMembersOfGroup(Focus.GroupId);
            var status = 0;
            foreach (var item in Focus.Users)
            {
                if (item.Id == (User.Identity.GetUserId()))
                {
                    status = 1;
                }
            }
            if (status == 1)
            {
                Focus.IsAMember = true;
            }
            return View("Focus", Focus);
        }

        public ActionResult EditFocus(int id)
        {
            var Focus = focusService.GetFocus(id);
            FocusFormModel editFocus = Mapper.Map<Focus, FocusFormModel>(Focus);
            if (Focus == null)
            {
                return HttpNotFound();
            }
            return View("EditFocus", editFocus);
        }

        public ActionResult DeleteFocus(int id)
        {
            var focus = focusService.GetFocus(id);
            if (focus == null)
            {
                return HttpNotFound();
            }
            return View(focus);
        }

        [HttpPost, ActionName("DeleteFocus")]
        public ActionResult DeleteConfirmedFocus(int id)
        {
            var Focus = focusService.GetFocus(id);
            if (Focus == null)
            {
                return HttpNotFound();
            }
            focusService.DeleteFocus(id);
            return RedirectToAction("Index", "Group", new { id = Focus.GroupId });
        }

        [HttpPost]
        public ActionResult EditFocus(FocusFormModel focusFormViewModel)
        {
            Focus focus = Mapper.Map<FocusFormModel, Focus>(focusFormViewModel);
            focus.Group = groupService.GetGroup(focus.GroupId);
            var errors = focusService.CanAddFocus(focus).ToList();
            ModelState.AddModelErrors(errors);
            if (ModelState.IsValid)
            {
                focusService.UpdateFocus(focus);
                focusService.SaveFocus();
                return RedirectToAction("Index", new { id = focus.GroupId });
            }
            return View("EditFocus", focusFormViewModel);
        }

        // POST: /Group/Edit/5
        [HttpPost]
        public ActionResult EditGroup(GroupFormModel groupFormViewModel)
        {
            Group group = Mapper.Map<GroupFormModel, Group>(groupFormViewModel);
            var errors = groupService.CanAddGroup(group).ToList();
            ModelState.AddModelErrors(errors);
            if (ModelState.IsValid)
            {
                groupService.UpdateGroup(group);
                return RedirectToAction("Index", new { id = group.GroupId });
            }
            return View("_EditGroup", groupFormViewModel);
        }

        //
        // GET: /Group/Delete/5

        public ActionResult DeleteGroup(int id)
        {
            var group = groupService.GetGroup(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View("_DeleteGroup", group);
        }

        [HttpPost, ActionName("DeleteGroup")]
        public ActionResult DeleteConfirmedGroup(int id)
        {
            var group = groupService.GetGroup(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            groupService.DeleteGroup(id);
            groupUserService.DeleteGroupUserByGroupId(id);
            return RedirectToAction("Index", "Home");
        }

        //[ActionName("CreateGoal")]
        public ViewResult CreateGoal(int id)
        {
            var metrics = metricService.GetMetrics();
            var focuss = focusService.GetFocussOFGroup(id);
            var groupGoal = new GroupGoalFormModel() { GroupId = id };
            groupGoal.Metrics = metrics.ToSelectListItems(-1);
            groupGoal.Foci = focuss.ToSelectListItems(-1);
            return View(groupGoal);
        }

        [HttpPost]
        public ActionResult CreateGoal(GroupGoalFormModel goal)
        {
            GroupGoal groupgoal = Mapper.Map<GroupGoalFormModel, GroupGoal>(goal);
            var groupUser = groupUserService.GetGroupUser(User.Identity.GetUserId(), goal.GroupId);
            groupgoal.GroupUserId = groupUser.GroupUserId;
            //groupgoal.GroupUser = groupUser;
            //if (groupgoal.AssignedTo == null)
            //{
            //    groupgoal.AssignedGroupUserId = null;
            //}
            var errors = groupGoalService.CanAddGoal(groupgoal, groupUpdateService).ToList();
            ModelState.AddModelErrors(errors);
            if (ModelState.IsValid)
            {
                //groupgoal.GroupUser.UserId = ((SocialGoalUser)(User.Identity)).UserId;
                groupGoalService.CreateGroupGoal(groupgoal);
                return RedirectToAction("Index", new { id = goal.GroupId });
            }
            //goal.Group = groupService.GetGroup(goal.GroupUser.GroupId);
            var metrics = metricService.GetMetrics();
            var focuss = focusService.GetFocussOFGroup(goal.GroupId);
            goal.Metrics = metrics.ToSelectListItems(-1);
            goal.Foci = focuss.ToSelectListItems(-1);
            return View(goal);
        }

        public ViewResult EditGoal(int id)
        {
            var goal = groupGoalService.GetGroupGoal(id);
            GroupGoalFormModel editGoal = Mapper.Map<GroupGoal, GroupGoalFormModel>(goal);
            var metrics = metricService.GetMetrics();
            var focuss = focusService.GetFocussOFGroup(goal.GroupUser.GroupId);
            if (goal.Metric != null)
                editGoal.Metrics = metrics.ToSelectListItems(goal.Metric.MetricId);
            else
                editGoal.Metrics = metrics.ToSelectListItems(-1);
            if (goal.Focus != null)
                editGoal.Foci = focuss.ToSelectListItems(goal.Focus.FocusId);
            else
                editGoal.Foci = focuss.ToSelectListItems(-1);
            return View(editGoal);
        }

        [HttpPost]
        public ActionResult EditGoal(GroupGoalFormModel editGoal)
        {
            GroupGoal groupGoal = Mapper.Map<GroupGoalFormModel, GroupGoal>(editGoal);
            var errors = groupGoalService.CanAddGoal(groupGoal, groupUpdateService);
            ModelState.AddModelErrors(errors);
            if (ModelState.IsValid)
            {
                groupGoalService.EditGroupGoal(groupGoal);
                return RedirectToAction("GroupGoal", new { id = editGoal.GroupGoalId }); ;

            }
            else
            {
                var metrics = metricService.GetMetrics();
                var focuss = focusService.GetFocuss();
                var groupGoalToEdit = groupGoalService.GetGroupGoal(editGoal.GroupGoalId);
                editGoal.Group = groupService.GetGroup(editGoal.GroupId);
                if (groupGoalToEdit.Metric != null)
                    editGoal.Metrics = metrics.ToSelectListItems(groupGoalToEdit.Metric.MetricId);
                else
                    editGoal.Metrics = metrics.ToSelectListItems(-1);
                if (groupGoalToEdit.Focus != null)
                    editGoal.Foci = focuss.ToSelectListItems(groupGoalToEdit.Focus.FocusId);
                else
                    editGoal.Foci = focuss.ToSelectListItems(-1);
                //editGoal.Group = groupGoalToEdit.GroupUser.Group;
                return View(editGoal);
            }
        }

        public ActionResult DeleteGoal(int id)
        {
            var goal = groupGoalService.GetGroupGoal(id);
            if (goal == null)
            {
                return HttpNotFound();
            }
            return View(goal);
        }

        [HttpPost, ActionName("DeleteGoal")]
        public ActionResult DeleteConfirmed(int id)
        {
            var goal = groupGoalService.GetGroupGoal(id);
            if (goal == null)
            {
                return HttpNotFound();
            }

            groupGoalService.DeleteGroupGoal(id);
            return RedirectToAction("Index", new { id = goal.GroupId });
        }

        public ActionResult GroupGoal(int id)
        {
            var goal = groupGoalService.GetGroupGoal(id);
            if (goal == null)
            {
                return HttpNotFound();
            }
            var goalDetails = Mapper.Map<GroupGoal, GroupGoalViewModel>(goal);
            var user = userService.GetUser(goalDetails.GroupUser.UserId);
            goalDetails.UserId = user.Id;
            goalDetails.User = user;
            var assignedGroupUser = groupUserService.GetGroupUser(goalDetails.AssignedGroupUserId);

            if (goalDetails.AssignedGroupUserId == 0)
            {

                // var assigneduser = userService.GetUser(assignedGroupUser.UserId);
                goalDetails.AssignedUserId = null;
            }
            else
            {
                goalDetails.AssignedUserId = assignedGroupUser.UserId;
            }
            var goalstatus = goalStatusService.GetGoalStatus();
            goalDetails.GoalStatuses = goalstatus.ToSelectListItems(goal.GoalStatusId);

            goalDetails.Users = groupUserService.GetMembersOfGroup(goal.GroupUser.GroupId);
            var status = 0;
            foreach (var item in goalDetails.Users)
            {
                if (item.Id == User.Identity.GetUserId())
                {
                    status = 1;
                }
            }
            if (status == 1)
            {
                goalDetails.IsAMember = true;
            }
            return View(goalDetails);
        }

        // [HttpPost]
        public ActionResult SaveUpdate(GroupUpdateFormModel newupdate)
        {

            if (ModelState.IsValid)
            {
                GroupUpdate update = Mapper.Map<GroupUpdateFormModel, GroupUpdate>(newupdate);
                var userId = User.Identity.GetUserId();
                groupUpdateService.CreateUpdate(update, userId);
                var Updates = Mapper.Map<IEnumerable<GroupUpdate>, IEnumerable<GroupUpdateViewModel>>(groupUpdateService.GetUpdatesByGoal(newupdate.GroupGoalId));
                foreach (var item in Updates)
                {
                    item.IsSupported = groupUpdateSupportService.IsUpdateSupported(item.GroupUpdateId, User.Identity.GetUserId(), groupUserService);
                    item.UserId = groupUpdateUserService.GetGroupUpdateUser(item.GroupUpdateId).Id;
                }
                GroupUpdateListViewModel updates = new GroupUpdateListViewModel()
                {
                    GroupUpdates = Updates,
                    Metric = groupGoalService.GetGroupGoal(newupdate.GroupGoalId).Metric,
                    Target = groupGoalService.GetGroupGoal(newupdate.GroupGoalId).Target
                };
                return PartialView("_UpdateView", updates);

            }
            return null;
        }

        public PartialViewResult DisplayUpdates(int id)
        {
            var Updates = Mapper.Map<IEnumerable<GroupUpdate>, IEnumerable<GroupUpdateViewModel>>(groupUpdateService.GetUpdatesByGoal(id));

            foreach (var item in Updates)
            {


                item.IsSupported = groupUpdateSupportService.IsUpdateSupported(item.GroupUpdateId, User.Identity.GetUserId(), groupUserService);
                item.UserId = groupUpdateUserService.GetGroupUpdateUser(item.GroupUpdateId).Id;
            }
            GroupUpdateListViewModel updates = new GroupUpdateListViewModel()
            {
                GroupUpdates = Updates,
                Metric = groupGoalService.GetGroupGoal(id).Metric,
                Target = groupGoalService.GetGroupGoal(id).Target
            };
            return PartialView("_UpdateView", updates);
        }

        public PartialViewResult DisplayComments(int id)
        {
            var comments = groupCommentService.GetCommentsByUpdate(id);
            IEnumerable<GroupCommentsViewModel> commentsView = Mapper.Map<IEnumerable<GroupComment>, IEnumerable<GroupCommentsViewModel>>(comments);
            foreach (var item in commentsView)
            {
                var groupCommentUser = groupCommentUserService.GetCommentUser(item.GroupCommentId);
                var user = userService.GetUser(groupCommentUser.UserId);
                item.UserId = user.Id;
                item.User = user;
            }
            return PartialView("_CommentView", commentsView);
        }

        [HttpPost]
        public ActionResult SaveComment(GroupCommentFormModel newcomment)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var comment = Mapper.Map<GroupCommentFormModel, GroupComment>(newcomment);
                groupCommentService.CreateComment(comment, userId);

            }
            return RedirectToAction("DisplayComments", new { id = newcomment.GroupUpdateId });
        }

        [HttpGet]
        public JsonResult DisplayCommentCount(int id)
        {
            int commentcount = groupCommentService.GetCommentsByUpdate(id).Count();
            return Json(commentcount, JsonRequestBehavior.AllowGet);
        }


        public int NoOfComments(int id)
        {
            var no = groupCommentService.GetCommentcount(id);
            return (no);
        }
        public JsonResult SearchUserForGroup(string username, int groupId)
        {
            return Json(from g in groupUserService.SearchUserForGroup(username, groupId, userService, groupInvitationService)
                        select new { label = g.UserName + ", " + g.Email, value = g.UserName, id = g.Id }, JsonRequestBehavior.AllowGet);
        }

        public int NoOfUsers(int id)
        {
            return groupUserService.GetGroupUsersCount(id);
        }

        public PartialViewResult InviteUser(int id, string UserId)
        {
            GroupInvitation newInvitation = new GroupInvitation()
            {
                GroupId = id,
                FromUserId = User.Identity.GetUserId(),
                ToUserId = UserId,
                SentDate = DateTime.Now
            };
            groupInvitationService.CreateGroupInvitation(newInvitation);
            return PartialView(userService.GetUser(UserId));
        }

        [ActionName("InviteUsers")]
        public ViewResult InviteUsers(int id)
        {
            var group = groupService.GetGroup(id);
            GroupViewModel invGroup = Mapper.Map<Group, GroupViewModel>(group);
            return View("_InviteUsers", invGroup);
        }

        public ViewResult UsersList(int id)
        {
            var users = groupUserService.GetGroupUsersList(id).ToList();
            return View(users);
        }

        public ActionResult JoinGroup(int id)
        {
            var newGroupUser = new GroupUser()
            {
                Admin = false,
                UserId = User.Identity.GetUserId(),
                GroupId = id
            };
            groupUserService.CreateGroupUser(newGroupUser, groupInvitationService);
            if (groupRequestService.RequestSent(User.Identity.GetUserId(), id))
                groupRequestService.DeleteGroupRequest(User.Identity.GetUserId(), id);
            return RedirectToAction("Index", new { id = id });
        }

        public ActionResult GroupJoinRequest(int id)
        {
            var groupRequestFormModel = new GroupRequestFormModel()
            {
                UserId = User.Identity.GetUserId(),
                GroupId = id
            };
            var groupRequest = Mapper.Map<GroupRequestFormModel, GroupRequest>(groupRequestFormModel);
            groupRequestService.CreateGroupRequest(groupRequest);
            return RedirectToAction("Index", new { id = groupRequestFormModel.GroupId });
        }

        public int GetNumberOfRequests(int id)
        {
            return groupRequestService.GetGroupRequestsForGroup(id).Count();
        }

        public ActionResult AcceptRequest(int groupId, string userId)
        {
            var newGroupUser = new GroupUser()
            {
                Admin = false,
                UserId = userId,
                GroupId = groupId
            };
            groupUserService.CreateGroupUserFromRequest(newGroupUser, groupRequestService);
            if (groupInvitationService.IsUserInvited(groupId, userId))
                groupInvitationService.AcceptInvitation(groupId, userId);
            return RedirectToAction("ShowAllRequests", new { id = groupId });
        }

        public ActionResult RejectRequest(int groupId, string userId)
        {
            groupRequestService.DeleteGroupRequest(userId, groupId);
            return RedirectToAction("ShowAllRequests", new { id = groupId });
        }

        public PartialViewResult GroupsView()
        {
            var groupIds = groupUserService.GetGroupAdminUsers(User.Identity.GetUserId());
            var groups = groupService.GetGroupsForUser(groupIds);
            var groupsList = Mapper.Map<IEnumerable<Group>, IEnumerable<GroupsItemViewModel>>(groups);
            return PartialView("_GroupView", groupsList);
        }

        public ViewResult GroupViewOfUser()
        {
            return View();
        }

        public PartialViewResult FollowedGroups()
        {
            List<Group> groups = new List<Group> { };
            var groupids = groupUserService.GetFollowedGroups(User.Identity.GetUserId());
            foreach (var item in groupids)
            {
                var group = groupService.GetGroup(item);
                groups.Add(group);
            }
            return PartialView("FollowedGroups", groups);
        }

        [HttpPost]
        public string InviteEmail(InviteEmailFormModel inviteUser)
        {
            var user = userService.GetUsersByEmail(inviteUser.Email);
            if (user != null)
            {
                if (!groupUserService.CanInviteUser(user.Id, inviteUser.GrouporGoalId))
                    return Resources.PersonJoined;
            }
            if (ModelState.IsValid)
            {
                Guid groupIdToken = Guid.NewGuid();
                SecurityToken groupIdSecurity = new SecurityToken()
                {
                    Token = groupIdToken,
                    ActualID = inviteUser.GrouporGoalId
                };
                securityTokenService.CreateSecurityToken(groupIdSecurity);
                UserMailer.Invite(inviteUser.Email, groupIdToken).Send();
                return Resources.InvitationSent;
            }
            else
                return Resources.WrongEmail;
        }

        public PartialViewResult Reportpage(int id)
        {
            return PartialView();
        }

        public JsonResult GetGoalReport(int id)
        {
            var goal = groupGoalService.GetGroupGoal(id);
            return Json(new
            {
                Data = (from g in groupUpdateService.GetUpdatesWithStatus(id).OrderBy(u => u.UpdateDate)
                        select new { Date = g.UpdateDate.ToString(), Value = g.status }),
                Target = new { EndDate = goal.EndDate.ToString(), Target = (goal.Target != null) ? goal.Target : 100 }
            }, JsonRequestBehavior.AllowGet);
        }


        public string GoalStatus(int id, int goalid)
        {
            var goal = groupGoalService.GetGroupGoal(goalid);
            goal.GoalStatusId = id;
            groupGoalService.SaveGroupGoal();
            string msg = goal.GoalStatus.GoalStatusType;
            return msg;
        }

        public double GoalProgress(int id)
        {
            return groupUpdateService.Progress(id);
        }

        public ActionResult ListOfGroups()
        {
            var groups = groupService.GetGroups();
            return View(groups);
        }

        public ActionResult Groupslist(int filter)
        {
            if (filter == 0)
            {
                var group = groupService.GetGroups();
                return PartialView("_Groupslist", group);
            }
            else if (filter == 1)
            {
                var userIds = followUserService.GetFollowingUsers(User.Identity.GetUserId());
                var groupIds = groupUserService.GetGroupUsers(userIds);
                var groups = groupService.GetGroups(groupIds);
                return PartialView("_Groupslist", groups);
            }
            else if (filter == 2)
            {
                var groupIds = groupUserService.GetGroupAdminUsers(User.Identity.GetUserId());
                var group = groupService.GetGroupsForUser(groupIds);
                return PartialView("_Groupslist", group);
            }
            else if (filter == 3)
            {
                var groupids = groupUserService.GetFollowedGroups(User.Identity.GetUserId());
                var allGroups = groupService.GetGroups(groupids);
                return PartialView("_Groupslist", allGroups);
            }
            return PartialView();
        }

        /// <summary>
        /// Action to load groups list 
        /// </summary>
        /// <param name="sortBy"></param>
        /// <param name="filterBy"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult GroupList(string filterBy = "All", int page = 1)
        {
            // Get a paged list of groups
            var groups = groupService.GetGroups(User.Identity.GetUserId(), filterBy, new Page(page,8));
            
            // map it to a paged list of models.
            var groupsViewModel = Mapper.Map<IPagedList<Group>, IPagedList<GroupsItemViewModel>>(groups);

            foreach (var item in groupsViewModel)
            {
                var groupAdminId = groupUserService.GetAdminId(item.GroupId);
                var groupUserAdmin = userService.GetUser(groupAdminId);
                item.UserId = groupUserAdmin.Id;
                item.UserName = groupUserAdmin.UserName;
            }
            var groupsList = new GroupsPageViewModel(filterBy) {GroupList = groupsViewModel};

            // If its an ajax request, just return the table
            if (Request.IsAjaxRequest())
            {
                return PartialView("_GroupsTable", groupsList);
            }
            return View("ListOfGroups", groupsList);
        }

        public ActionResult SearchMemberForGoalAssigning(int id)
        {
            var currentUserId = User.Identity.GetUserId();
            var users = groupUserService.GetMembersOfGroup(id);
            var groupusers = groupUserService.GetGroupUsersListToAssign(id, currentUserId);
            var result = from u in users
                         join gu in groupusers on u.Id equals gu.UserId
                         where gu.GroupId == id
                         select new MemberViewModel
                         {
                             GroupId = id,
                             UserId = u.Id,
                             GroupUserId = gu.GroupUserId,
                             UserName = u.UserName,
                             User = u
                         };
            //MemberViewModel gmvm = new MemberViewModel() { GroupId = id, Group = groupService.GetGroup(id), Users = members , GroupUser = users};
            return PartialView("_MembersToSearch", result);
        }

        public ActionResult EditUpdate(int id)
        {
            var update = groupUpdateService.GetUpdate(id);
            GroupUpdateFormModel editUpdate = Mapper.Map<GroupUpdate, GroupUpdateFormModel>(update);
            if (update == null)
            {
                return HttpNotFound();
            }
            return PartialView("_EditUpdate", editUpdate);
        }

        [HttpPost]
        public ActionResult EditUpdate(GroupUpdateFormModel newupdate)
        {
            GroupUpdate update = Mapper.Map<GroupUpdateFormModel, GroupUpdate>(newupdate);
            if (ModelState.IsValid)
            {

                groupUpdateService.EditUpdate(update);
                var Updates = Mapper.Map<IEnumerable<GroupUpdate>, IEnumerable<GroupUpdateViewModel>>(groupUpdateService.GetUpdatesByGoal(newupdate.GroupGoalId));
                foreach (var item in Updates)
                {
                    item.IsSupported = groupUpdateSupportService.IsUpdateSupported(item.GroupUpdateId, User.Identity.GetUserId(), groupUserService);
                    item.UserId = groupUpdateUserService.GetGroupUpdateUser(item.GroupUpdateId).Id;
                }
                GroupUpdateListViewModel updates = new GroupUpdateListViewModel()
                {
                    GroupUpdates = Updates,
                    Metric = groupGoalService.GetGroupGoal(newupdate.GroupGoalId).Metric,
                    Target = groupGoalService.GetGroupGoal(newupdate.GroupGoalId).Target
                };
                return PartialView("_UpdateView", updates);
            }
            return null;
        }

        public ActionResult DeleteUpdate(int id)
        {
            var update = groupUpdateService.GetUpdate(id);

            if (update == null)
            {
                return HttpNotFound();
            }
            return PartialView("_DeleteUpdate", update);
        }

        [HttpPost, ActionName("DeleteUpdate")]
        public ActionResult DeleteConfirmedUpdate(int id)
        {
            var update = groupUpdateService.GetUpdate(id);
            if (update == null)
            {
                return HttpNotFound();
            }

            groupUpdateService.DeleteUpdate(id);
            return RedirectToAction("GroupGoal", new { id = update.GroupGoalId });
        }

        public void SupportUpdate(int id)
        {
            var groupuser = groupUserService.GetGroupUserByuserId(User.Identity.GetUserId());
            groupUpdateSupportService.CreateSupport(new GroupUpdateSupport() { GroupUserId = groupuser.GroupUserId, GroupUpdateId = id, UpdateSupportedDate = DateTime.Now });
        }


        public int NoOfSupports(int id)
        {
            return groupUpdateSupportService.GetSupportcount(id);
        }


        public void UnSupportUpdate(int id)
        {
            var groupuser = groupUserService.GetGroupUserByuserId(User.Identity.GetUserId());
            groupUpdateSupportService.DeleteSupport(id, groupuser.GroupUserId);
        }

        [HttpGet]
        public JsonResult DisplayUpdateSupportCount(int id)
        {
            int supportcount = groupUpdateSupportService.GetSupportcount(id);
            return Json(supportcount, JsonRequestBehavior.AllowGet);
        }


        public PartialViewResult SupportersOfUpdate(int id)
        {
            var supporters = groupUpdateSupportService.GetSupportersOfUpdate(id, userService, groupUserService);
            GroupUpdateSupportersViewModel usvm = new GroupUpdateSupportersViewModel() { GroupUpdateId = id, GroupUpdate = groupUpdateService.GetUpdate(id), Users = supporters };
            return PartialView(usvm);
        }
    }
}
