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
using SocialGoal.Web.Core.Extensions;
using SocialGoal.Web.Mailers;
using SocialGoal.Properties;
using Microsoft.AspNet.Identity;

namespace SocialGoal.Web.Controllers
{
    [Authorize]
    public class GoalController : Controller
    {
        private readonly IGoalService goalService;
        private readonly IMetricService metricService;
        private readonly IFocusService focusService;
        private readonly ISupportService supportService;
        private readonly IUpdateService updateService;
        private readonly ICommentService commentService;
        private readonly IUserService userService;
        public readonly ISecurityTokenService securityTokenService;
        public readonly ISupportInvitationService supportInvitationService;
        private readonly IGoalStatusService goalStatusService;
        private readonly ICommentUserService commentUserService;
        private readonly IUpdateSupportService updateSupportService;

        private IUserMailer userMailer = new UserMailer();
        public IUserMailer UserMailer
        {
            get { return userMailer; }
            set { userMailer = value; }
        }

        public GoalController(IGoalService goalService, IMetricService metricService, IFocusService focusService, ISupportService supportService, IUpdateService updateService, ICommentService commentService, IUserService userService, ISecurityTokenService securityTokenService, ISupportInvitationService supportInvitationService, IGoalStatusService goalStatusService, ICommentUserService commentUserService, IUpdateSupportService updateSupportService)
        {
            this.goalService = goalService;
            this.supportInvitationService = supportInvitationService;
            this.metricService = metricService;
            this.focusService = focusService;
            this.supportService = supportService;
            this.updateService = updateService;
            this.commentService = commentService;
            this.userService = userService;
            this.securityTokenService = securityTokenService;
            this.goalStatusService = goalStatusService;
            this.commentUserService = commentUserService;
            this.updateSupportService = updateSupportService;
        }

        public ActionResult Index(int id)
        {
            var goal = goalService.GetGoal(id);
            if (goal == null)
            {
                return HttpNotFound();
            }
            var goalDetails = Mapper.Map<Goal, GoalViewModel>(goal);
            goalDetails.Supported = supportService.IsGoalSupported(id, User.Identity.GetUserId());
            var goalstatus = goalStatusService.GetGoalStatus();
            goalDetails.GoalStatuses = goalstatus.ToSelectListItems(goal.GoalStatusId);
            return View(goalDetails);
        }

        public ViewResult MyGoal()
        {
            string userid = User.Identity.GetUserId();
            var Goals = goalService.GetMyGoals(userid);
            var goalDetails = Mapper.Map<IEnumerable<Goal>, IEnumerable<GoalViewModel>>(Goals);
            return View(goalDetails);
        }

        public ViewResult FollowedGoal()
        {
            var followed = supportService.GetUserSupportedGoals(User.Identity.GetUserId(), goalService);
            return View(followed);
        }

        public PartialViewResult Create()
        {
            var createGoal = new GoalFormModel();
            var metrics = metricService.GetMetrics();
            var goalstatus = goalStatusService.GetGoalStatus();
            createGoal.Metrics = metrics.ToSelectListItems(-1);
            return PartialView(createGoal);
        }

        [HttpPost]
        public ActionResult Create(GoalFormModel createGoal)
        {
            Goal goal = Mapper.Map<GoalFormModel, Goal>(createGoal);
            var errors = goalService.CanAddGoal(goal, updateService).ToList();
            ModelState.AddModelErrors(errors);
            if (ModelState.IsValid)
            {
                goalService.CreateGoal(goal);
                return RedirectToAction("Index", new { id = goal.GoalId });
            }
            var metrics = metricService.GetMetrics();
            var goalstatuss = goalStatusService.GetGoalStatus();
            createGoal.Metrics = metrics.ToSelectListItems(-1);
            return View("Create", createGoal);
        }

        public ActionResult Edit(int id)
        {
            var goal = goalService.GetGoal(id);
            GoalFormModel editGoal = Mapper.Map<Goal, GoalFormModel>(goal);
            if (goal == null)
            {
                return HttpNotFound();
            }
            var metrics = metricService.GetMetrics();
            if (goal.Metric != null)
                editGoal.Metrics = metrics.ToSelectListItems(goal.Metric.MetricId);
            else
                editGoal.Metrics = metrics.ToSelectListItems(-1);
            return View(editGoal);
        }

        [HttpPost]
        public ActionResult Edit(GoalFormModel editGoal)
        {
            Goal goalToEdit = Mapper.Map<GoalFormModel, Goal>(editGoal);
            var errors = goalService.CanAddGoal(goalToEdit, updateService);
            ModelState.AddModelErrors(errors);
            if (ModelState.IsValid)
            {
                goalService.EditGoal(goalToEdit);
                return RedirectToAction("Index", new { id = editGoal.GoalId });
            }
            else
            {
                var metrics = metricService.GetMetrics();
                var goalstatus = goalStatusService.GetGoalStatus();
                var goal = goalService.GetGoal(editGoal.GoalId);
                if (goal.Metric != null)
                    editGoal.Metrics = metrics.ToSelectListItems(goal.Metric.MetricId);
                else
                    editGoal.Metrics = metrics.ToSelectListItems(-1);

                return View(editGoal);
            }
        }

        [HttpPost]
        public string GoalStatus(int id, int goalid)
        {
            var goal = goalService.GetGoal(goalid);
            goal.GoalStatusId = id;
            goalService.SaveGoal();
            string msg = goal.GoalStatus.GoalStatusType;
            return msg;
        }

        public ActionResult Delete(int id)
        {
            var goal = goalService.GetGoal(id);
            if (goal == null)
            {
                return HttpNotFound();
            }
            return View(goal);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var goal = goalService.GetGoal(id);
            if (goal == null)
            {
                return HttpNotFound();
            }

            goalService.DeleteGoal(id);
            return RedirectToAction("Index", "Home");
        }

        public PartialViewResult MyGoals()
        {
            string userid = User.Identity.GetUserId();
            var Goals = goalService.GetMyGoals(userid);
            return PartialView("_MyGoalsView", Goals);
        }

        public PartialViewResult GoalsFollowing()
        {
            var followed = supportService.GetUserSupportedGoals(User.Identity.GetUserId(), goalService);
            return PartialView("_FollowedGoals", followed);
        }

        public PartialViewResult DisplayUpdates(int id)
        {
            var Updates = Mapper.Map<IEnumerable<Update>, IEnumerable<UpdateViewModel>>(updateService.GetUpdatesByGoal(id));
            foreach (var item in Updates)
            {
                item.IsSupported = updateSupportService.IsUpdateSupported(item.UpdateId, User.Identity.GetUserId());
            }
            UpdateListViewModel updates = new UpdateListViewModel()
            {

                Updates = Updates,
                Metric = goalService.GetGoal(id).Metric,
                Target = goalService.GetGoal(id).Target,
                // IsSupported =  updateSupportService.IsUpdateSupported(,((SocialGoalUser)(User.Identity)).UserId)
            };
            return PartialView("_UpdateView", updates);
        }

        public ViewResult Supporters(int id)
        {
            var members = supportService.GetSupportersOfGoal(id, userService);
            GoalSupporterViewModel gsvm = new GoalSupporterViewModel() { GoalId = id, Goal = goalService.GetGoal(id), Users = members };
            return View(gsvm);
        }

        [HttpPost]
        public ActionResult SaveUpdate(UpdateFormModel newupdate)
        {
            // Update update = Mapper.Map<UpdateFormModel, Update>(newupdate);
            if (ModelState.IsValid)
            {
                Update update = Mapper.Map<UpdateFormModel, Update>(newupdate);
                update.Goal = goalService.GetGoal(newupdate.GoalId);
                var updateVal = updateService.GetHighestUpdateValue(newupdate.GoalId);
               
                if(updateVal!=null)
                {  
                    if (updateVal.status <= newupdate.status)
                    {
                        updateService.CreateUpdate(update);
                       
                    }
                    else
                    {
                        update.status = -1;
                        ModelState.AddModelError(update.Updatemsg, "cannot enter");
                    }
                }
                else
                {
                    updateService.CreateUpdate(update);
                }


                     var Updates = Mapper.Map<IEnumerable<Update>, IEnumerable<UpdateViewModel>>(updateService.GetUpdatesByGoal(newupdate.GoalId));
                     foreach (var item in Updates)
                     {
                         item.IsSupported = updateSupportService.IsUpdateSupported(item.UpdateId, User.Identity.GetUserId());
                     }
                     UpdateListViewModel updates = new UpdateListViewModel()
                     {
                         Updates = Updates,
                         Metric = goalService.GetGoal(newupdate.GoalId).Metric,
                         Target = goalService.GetGoal(newupdate.GoalId).Target,
                         //IsSupported = updateSupportService.IsUpdateSupported(newupdate.UpdateId,((SocialGoalUser)(User.Identity)).UserId)
                     };
                     return PartialView("_UpdateView", updates);

            }
            return null;
        }

        public PartialViewResult InviteUser(int id)
        {
            return PartialView("_SupportInvite", goalService.GetGoal(id));
        }

        [HttpPost]
        public PartialViewResult InviteUser(int id, string userId)
        {
            SupportInvitation newInvitation = new SupportInvitation()
            {
                GoalId = id,
                FromUserId = User.Identity.GetUserId(),
                ToUserId = userId,
                SentDate = DateTime.Now
            };
            supportInvitationService.CreateSupportInvitation(newInvitation);
            var user = userService.GetUser(userId);
            return PartialView(user);
        }

        public PartialViewResult DisplayComments(int updateId)
        {
            var comments = commentService.GetCommentsByUpdate(updateId);
            IEnumerable<CommentsViewModel> commentsView = Mapper.Map<IEnumerable<Comment>, IEnumerable<CommentsViewModel>>(comments);
            foreach (var item in commentsView)
            {
                var user = commentUserService.GetUser(item.CommentId);
                item.UserId = user.Id;
                item.User = user;
            }
            return PartialView("_CommentView", commentsView);
        }

        [HttpPost]
        public ActionResult SaveComment(CommentFormModel newcomment)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var comment = Mapper.Map<CommentFormModel, Comment>(newcomment);
                commentService.CreateComment(comment, userId);
            }
            return RedirectToAction("DisplayComments", new { updateId = newcomment.UpdateId });
        }

        [HttpGet]
        public JsonResult DisplayCommentCount(int UpdId)
        {
            int commentcount = commentService.GetCommentsByUpdate(UpdId).Count();
            return Json(commentcount, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchUser(string username, int goalId)
        {
            return Json(from g in supportService.SearchUserToSupport(username, goalId, userService, supportInvitationService,User.Identity.GetUserId())
                        select new { label = g.UserName + ", " + g.Email, value = g.UserName, id = g.Id }, JsonRequestBehavior.AllowGet);
        }

        public int NoOfComments(int id)
        {
            return commentService.GetCommentcount(id);
        }

        public IEnumerable<Goal> SearchGoal(string name)
        {
            var goals = Mapper.Map<IEnumerable<Goal>, IEnumerable<GoalViewModel>>(goalService.SearchGoal(name)).ToList();
            goals.ForEach(g => g.Supported = g.UserId != User.Identity.GetUserId() ? (bool?)supportService.IsGoalSupported(g.GoalId, User.Identity.GetUserId()) : null);
            return (IEnumerable<Goal>)goals;
        }

        public void SupportGoal(int id)
        {
            supportService.CreateSupport(new Support() { UserId = User.Identity.GetUserId(), GoalId = id, SupportedDate = DateTime.Now });
        }

        public ActionResult SupportGoalNow(int id)
        {
            supportService.CreateUserSupport(new Support() { UserId = User.Identity.GetUserId(), GoalId = id, SupportedDate = DateTime.Now }, supportInvitationService);
            return RedirectToAction("Index", new { id = id });
        }

        public void UnSupportGoal(int id)
        {
            supportService.DeleteSupport(id, User.Identity.GetUserId());
        }

        public PartialViewResult SupportInvitation(int goalId)
        {
            var goal = goalService.GetGoal(goalId);
            return PartialView("_SupportInvite", goal);
        }

        [HttpPost]
        public string InviteEmail(InviteEmailFormModel inviteEmail)
        {
            var user = userService.GetUsersByEmail(inviteEmail.Email);
            if (user != null)
            {

                if (!supportService.CanInviteUser(user.Id, inviteEmail.GrouporGoalId))
                {
                    return Resources.PersonSupporting;
                }
            }
            if (ModelState.IsValid)
            {
                Guid goalIdToken = Guid.NewGuid();
                SecurityToken goalIdSecurity = new SecurityToken()
                {
                    Token = goalIdToken,
                    ActualID = inviteEmail.GrouporGoalId
                };
                securityTokenService.CreateSecurityToken(goalIdSecurity);
                UserMailer.Support(inviteEmail.Email, goalIdToken).Send();
                return Resources.InvitationSent;

            }
            else
            {
                return Resources.WrongEmail;
            }
        }

        public PartialViewResult Reportpage(int id)
        {
            return PartialView();
        }

        public JsonResult GetGoalReport(int id)
        {
            var goal = goalService.GetGoal(id);
            return Json(new
            {
                Data = (from g in updateService.GetUpdatesWithStatus(id).OrderBy(u => u.UpdateDate)
                        select new { Date = g.UpdateDate.ToString(), Value = g.status }),
                Target = new { EndDate = goal.EndDate.ToString(), Target = (goal.Target != null) ? goal.Target : 100 }
            }, JsonRequestBehavior.AllowGet);
        }

        public double GoalProgress(int id)
        {
            return updateService.Progress(id);
        }

        public ActionResult ListOfGoals()
        {
            var goals = goalService.GetGoals();
            return View(goals);
        }

        public ActionResult Goalslist(int sortby, int filter)
        {
            if (sortby == 0 && filter == 0)
            {
                var goals = goalService.GetGoals();
                return PartialView("_Goalslist", goals);
            }
            else if (sortby == 0 && filter == 1)
            {

                var goal = goalService.GetGoalsofFollowing(User.Identity.GetUserId());
                return PartialView("_Goalslist", goal);
            }
            else if (sortby == 0 && filter == 2)
            {
                var goal = goalService.GetGoalByDefault(User.Identity.GetUserId());
                return PartialView("_Goalslist", goal);
            }
            else if (sortby == 0 && filter == 3)
            {
                var goal = supportService.GetUserSupportedGoals(User.Identity.GetUserId(), goalService);
                return PartialView("_Goalslist", goal);
            }
            else if (sortby == 1 && filter == 0)
            {
                var goal = goalService.GetPublicGoalsbyPopularity();
                return PartialView("_Goalslist", goal);
            }
            else if (sortby == 1 && filter == 1)
            {
                var goals = goalService.GetGoalsofFollowingByPopularity(User.Identity.GetUserId());
                return PartialView("_Goalslist", goals);

            }
            else if (sortby == 1 && filter == 2)
            {
                var goals = goalService.GetGoalsbyPopularity(User.Identity.GetUserId());

                return PartialView("_Goalslist", goals);
            }
            else if (sortby == 1 && filter == 3)
            {
                var goals = supportService.GetUserSupportedGoalsBYPopularity(User.Identity.GetUserId(), goalService);
                return PartialView("_Goalslist", goals);
            }
            return PartialView();
        }


        /// <summary>
        /// Action to load goal list
        /// </summary>
        /// <param name="sortBy"></param>
        /// <param name="filterBy"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        /// 


        public ActionResult GoalList(string sortBy = "Date", string filterBy = "All", int page = 0)
        {
            var goals = goalService.GetGoalsByPage(User.Identity.GetUserId(), page, 5, sortBy, filterBy).ToList();

            var goalsViewModel = Mapper.Map<IEnumerable<Goal>, IEnumerable<GoalListViewModel>>(goals).ToList();
            var goalsList = new GoalsPageViewModel(filterBy, sortBy);
            goalsList.GoalList = goalsViewModel;

            if (Request.IsAjaxRequest())
            {
                return Json(goalsViewModel, JsonRequestBehavior.AllowGet);
            }
            return View("ListOfGoals", goalsList);
        }

        public ActionResult EditUpdate(int id)
        {
            var update = updateService.GetUpdate(id);
            UpdateFormModel editUpdate = Mapper.Map<Update, UpdateFormModel>(update);
            if (update == null)
            {
                return HttpNotFound();
            }
            return PartialView("_EditUpdate", editUpdate);
        }

        [HttpPost]
        public ActionResult EditUpdate(UpdateFormModel newupdate)
        {
            Update update = Mapper.Map<UpdateFormModel, Update>(newupdate);
            if (ModelState.IsValid)
            {
                update.Goal = goalService.GetGoal(newupdate.GoalId);
                updateService.EditUpdate(update);
                var Updates = Mapper.Map<IEnumerable<Update>, IEnumerable<UpdateViewModel>>(updateService.GetUpdatesByGoal(newupdate.GoalId));
                foreach (var item in Updates)
                {
                    item.IsSupported = updateSupportService.IsUpdateSupported(item.UpdateId, User.Identity.GetUserId());
                }
                UpdateListViewModel updates = new UpdateListViewModel()
                {
                    Updates = Updates,
                    Metric = goalService.GetGoal(newupdate.GoalId).Metric,
                    Target = goalService.GetGoal(newupdate.GoalId).Target
                };
                return PartialView("_UpdateView", updates);
            }
            return null;
        }

        public ActionResult DeleteUpdate(int id)
        {
            var update = updateService.GetUpdate(id);

            if (update == null)
            {
                return HttpNotFound();
            }
            return PartialView("_DeleteUpdate", update);
        }

        [HttpPost, ActionName("DeleteUpdate")]
        public ActionResult DeleteConfirmedUpdate(int id)
        {
            var update = updateService.GetUpdate(id);
            if (update == null)
            {
                return HttpNotFound();
            }

            updateService.DeleteUpdate(id);
            return RedirectToAction("Index", new { id = update.GoalId });
        }

        public void SupportUpdate(int id)
        {
            updateSupportService.CreateSupport(new UpdateSupport() { UserId = User.Identity.GetUserId(), UpdateId = id, UpdateSupportedDate = DateTime.Now });
        }


        public int NoOfSupports(int id)
        {
            return updateSupportService.GetSupportcount(id);
        }


        public void UnSupportUpdate(int id)
        {
            updateSupportService.DeleteSupport(id, User.Identity.GetUserId());
        }

        [HttpGet]
        public JsonResult DisplayUpdateSupportCount(int id)
        {
            int supportcount = updateSupportService.GetSupportcount(id);
            return Json(supportcount, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult SupportersOfUpdate(int id)
        {
            var supporters = updateSupportService.GetSupportersOfUpdate(id, userService);
            UpdateSupportersViewModel usvm = new UpdateSupportersViewModel() { UpdateId = id, Update = updateService.GetUpdate(id), Users = supporters };
            return PartialView(usvm);
        }

        //public PartialViewResult Cancel(int id)
        //{

        //}
    }
}