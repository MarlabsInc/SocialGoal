using AutoMapper;
using SocialGoal.Model.Models;
using SocialGoal.Service;
using SocialGoal.Web.Core.Models;
using SocialGoal.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace SocialGoal.Web.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        IGoalService goalService;
        IUpdateService updateService;
        ICommentService commentService;
        IGroupInvitationService groupInvitationService;
        ISupportInvitationService supportInvitationService;
        IFollowRequestService followRequestService;
        IUserService userService;
        public NotificationController(IGoalService goalService, IUpdateService updateService, ICommentService commentService, IGroupInvitationService groupInvitationService, ISupportInvitationService supportInvitationService, IFollowRequestService followRequestService, IUserService userService)
        {
            this.goalService = goalService;
            this.supportInvitationService = supportInvitationService;
            this.updateService = updateService;
            this.groupInvitationService = groupInvitationService;
            this.commentService = commentService;
            this.followRequestService = followRequestService;
            this.userService = userService;
        }


        /// <summary>
        /// Action that returns invitations
        /// </summary>
        /// <param name="page">current page no will be bere</param>
        /// <returns></returns>
        public ActionResult Index(int page = 0)
        {
            int noOfrecords = 10;
            var notifications = GetNotifications(page, noOfrecords);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_NotificationList", notifications);
            }
            return View("Index", notifications);
        }



        public int GetNumberOfInvitations()
        {
            return groupInvitationService.GetGroupInvitationsForUser(User.Identity.GetUserId()).Count() + supportInvitationService.GetSupportInvitationsForUser(User.Identity.GetUserId()).Count() + followRequestService.GetFollowRequestsForUser(User.Identity.GetUserId()).Count();
        }

        /// <summary>
        /// Method returns paged notifications
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public IEnumerable<NotificationViewModel> GetNotifications(int page, int noOfRecords)
        {
            var groupInv = groupInvitationService.GetGroupInvitationsForUser(User.Identity.GetUserId());
            var supportInv = supportInvitationService.GetSupportInvitationsForUser(User.Identity.GetUserId());
            var followRequests = followRequestService.GetFollowRequests(User.Identity.GetUserId());
            IEnumerable<NotificationViewModel> Invitations = Mapper.Map<IEnumerable<GroupInvitation>, IEnumerable<NotificationViewModel>>(groupInv);
            Invitations = Invitations.Concat(Mapper.Map<IEnumerable<SupportInvitation>, IEnumerable<NotificationViewModel>>(supportInv));
            Invitations = Invitations.Concat(Mapper.Map<IEnumerable<FollowRequest>, IEnumerable<NotificationViewModel>>(followRequests));
            foreach (var item in Invitations)
            {
                var fromUser = userService.GetUser(item.FromUserId);
                var toUser = userService.GetUser(item.ToUserId);
                item.FromUser = fromUser;
                item.ToUser = toUser;
            }

            //for paging

            var skipNotifications = noOfRecords * page;
            Invitations = Invitations.Skip(skipNotifications).Take(noOfRecords);
            return Invitations;
        }
    }
}
