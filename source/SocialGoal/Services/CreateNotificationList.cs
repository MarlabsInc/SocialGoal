using System.Collections.Generic;
using System.Linq;
using SocialGoal.Model.Models;
using SocialGoal.Web.ViewModels;
using SocialGoal.Service;
using SocialGoal.Web.Core.Models;
using System;

namespace SocialGoal.Web.Services
{
    public class CreateNotificationList
    {

        internal IEnumerable<NotificationsViewModel> GetNotifications(string userId, IGoalService goalService, ICommentService commentService, IUpdateService updateService, ISupportService supportService, IUserService userService, IGroupService groupService, IGroupUserService groupUserSevice, IGroupGoalService groupGoalService, IGroupCommentService groupcommentService, IGroupUpdateService groupupdateService, IFollowUserService followUserService, IGroupCommentUserService groupCommentUserService,ICommentUserService commentUserService,IGroupUpdateUserService groupUpdateUserService)
        {
            var goals = goalService.GetTop20GoalsofFollowing(userId);
            var comments = commentService.GetTop20CommentsOfPublicGoalFollwing(userId);
            var updates = updateService.GetTop20UpdatesOfFollowing(userId);
            var groupusers = groupUserSevice.GetTop20GroupsUsers(userId);
            var supports = supportService.GetTop20SupportsOfFollowings(userId);
            var groupgoals = groupGoalService.GetTop20GroupsGoals(userId, groupUserSevice);
            var groupupdates = groupupdateService.GetTop20Updates(userId, groupUserSevice);
            var groupComments = groupcommentService.GetTop20CommentsOfPublicGoals(userId, groupUserSevice);
            var followers = followUserService.GetTop20Followers(userId);
           // var groups = groupService.GetTop20Groups(groupids);

            return (from g in goals
                    select new NotificationsViewModel()
                    {
                        GoalId = g.GoalId,
                        UserId = g.UserId,
                        CreatedDate = g.CreatedDate,
                        UserName = g.User.UserName,
                        GoalName = g.GoalName,
                        Desc = g.Desc,
                        ProfilePicUrl = g.User.ProfilePicUrl,
                        Goal = g,
                        NotificationDate = DateCalculation(g.CreatedDate),
                        NotificationType = (int)NotificationType.createdGoal
                    })
                    .Concat(from c in comments
                            select new NotificationsViewModel()
                            {
                                CommentId = c.CommentId,
                                CreatedDate = c.CommentDate,                                
                                CommentText = c.CommentText,
                                UpdateId = c.UpdateId,
                                GoalName = c.Update.Goal.GoalName,
                                GoalId = c.Update.GoalId,
                                NumberOfComments = commentService.GetCommentsByUpdate(c.UpdateId).Count(),
                                Updatemsg = updateService.GetUpdate(c.UpdateId).Updatemsg,
                                NotificationDate = DateCalculation(c.CommentDate),
                                UserName =commentUserService.GetUser(c.CommentId).UserName,
                                UserId = commentUserService.GetUser(c.CommentId).Id,
                                ProfilePicUrl = commentUserService.GetUser(c.CommentId).ProfilePicUrl,

                                NotificationType = (int)NotificationType.commentedOnUpdate
                            })
                              .Concat(from u in updates
                                      select new NotificationsViewModel()
                                      {
                                          Update = u,
                                          UpdateId = u.UpdateId,
                                          GoalId = u.GoalId,
                                          ProfilePicUrl = u.Goal.User.ProfilePicUrl,
                                          GoalName = u.Goal.GoalName,
                                          Updatemsg = u.Updatemsg,
                                          UserId = u.Goal.UserId,
                                          UserName = u.Goal.User.UserName,
                                          NumberOfComments = commentService.GetCommentsByUpdate(u.UpdateId).Count(),
                                          CreatedDate = u.UpdateDate,
                                          NotificationDate = DateCalculation(u.UpdateDate),
                                          NotificationType = (int)NotificationType.updatedGoal
                                      })
                                         .Concat(from gr in groupgoals
                                                 select new NotificationsViewModel()
                                                 {
                                                     
                                                     GroupGoalId = gr.GroupGoalId,
                                                     CreatedDate = gr.CreatedDate,
                                                     
                                                     UserId = gr.GroupUser.UserId,
                                                     UserName = userService.GetUser(gr.GroupUser.UserId).UserName,
                                                     ProfilePicUrl = userService.GetUser(gr.GroupUser.UserId).ProfilePicUrl,
                                                     
                                                     GroupName = gr.Group.GroupName,                                                     
                                                     GroupGoalName = gr.GoalName,
                                                     GroupId = gr.GroupId,
                                                     NotificationDate = DateCalculation(gr.CreatedDate),
                                                     NotificationType = (int)NotificationType.createGroup

                                                 })
                                                .Concat(from gu in groupusers
                                                        select new NotificationsViewModel()
                                                        {
                                                            GroupUser = gu,                                                            
                                                            GroupUserId = gu.GroupUserId,
                                                            
                                                            UserId = gu.UserId,
                                                            UserName =userService.GetUser(gu.UserId).UserName,
                                                            ProfilePicUrl = userService.GetUser(gu.UserId).ProfilePicUrl,
                                                            
                                                            GroupName = groupService.GetGroup(gu.GroupId).GroupName,
                                                            GroupId = gu.GroupId,
                                                            CreatedDate = gu.AddedDate,
                                                            NotificationDate = DateCalculation(gu.AddedDate),
                                                            NotificationType = (int)NotificationType.joinGroup
                                                        })
                                                            .Concat(from s in supports
                                                                    select new NotificationsViewModel()
                                                                    {
                                                                        Support = s,                                                                        
                                                                        SupportId = s.SupportId,
                                                                        GoalName = s.Goal.GoalName,

                                                                        ProfilePicUrl = userService.GetUser(s.UserId).ProfilePicUrl,                                                                        
                                                                        UserName =userService.GetUser(s.UserId).UserName,
                                                                        UserId = s.UserId,                                                                        
                                                                        CreatedDate = s.SupportedDate,
                                                                        GoalId = s.GoalId,
                                                                        NotificationDate = DateCalculation(s.SupportedDate),
                                                                        NotificationType = (int)NotificationType.supportGoal
                                                                    })
                                                                      .Concat(from gu in groupupdates
                                                                              select new NotificationsViewModel()
                                                                              {
                                                                                  GroupUpdate = gu,
                                                                                  GroupUpdateId = gu.GroupUpdateId,
                                                                                  GroupUpdatemsg = gu.Updatemsg,

                                                                                  UserId = groupUpdateUserService.GetGroupUpdateUser(gu.GroupUpdateId).Id,
                                                                                  ProfilePicUrl = groupUpdateUserService.GetGroupUpdateUser(gu.GroupUpdateId).ProfilePicUrl,
                                                                                  UserName = groupUpdateUserService.GetGroupUpdateUser(gu.GroupUpdateId).UserName,
                                                                                  NotificationDate = DateCalculation(gu.UpdateDate),
                                                                                  CreatedDate = gu.UpdateDate,
                                                                                  GroupGoalId = gu.GroupGoalId,
                                                                                  GroupId = gu.GroupGoal.GroupId,
                                                                                  GroupGoalName = gu.GroupGoal.GoalName,
                                                                                  GroupName = gu.GroupGoal.Group.GroupName,
                                                                                  NotificationType = (int)NotificationType.updatedGroupgoal

                                                                              })
                                                                              .Concat(from gc in groupComments
                                                                                      select new NotificationsViewModel()
                                                                                      {

                                                                                          GroupCommentId = gc.GroupCommentId,
                                                                                          GroupCommentText = gc.CommentText,
                                                                                          GroupUpdateId = gc.GroupUpdateId,
                                                                                          
                                                                                          GroupGoalId = gc.GroupUpdate.GroupGoalId,
                                                                                          GroupGoalName = gc.GroupUpdate.GroupGoal.GoalName,

                                                                                          UserId = groupCommentUserService.GetGroupCommentUser(gc.GroupCommentId).Id,
                                                                                          UserName = groupCommentUserService.GetGroupCommentUser(gc.GroupCommentId).UserName,
                                                                                          ProfilePicUrl = groupCommentUserService.GetGroupCommentUser(gc.GroupCommentId).ProfilePicUrl,

                                                                                          GroupUpdatemsg = gc.GroupUpdate.Updatemsg,
                                                                                          GroupId = gc.GroupUpdate.GroupGoal.GroupUser.GroupId,
                                                                                          GroupName = gc.GroupUpdate.GroupGoal.Group.GroupName,
                                                                                          NotificationDate = DateCalculation(gc.CommentDate),
                                                                                          CreatedDate = gc.CommentDate,
                                                                                          NotificationType = (int)NotificationType.commentedonGroupUdate
                                                                                      })
                                                                                      .Concat(from f in followers
                                                                                              select new NotificationsViewModel()
                                                                                              {
                                                                                                  FollowUserId = f.FollowUserId,
                                                                                                  FromUser = f.FromUser,
                                                                                                  ToUser = f.ToUser,
                                                                                                  ProfilePicUrl = f.FromUser.ProfilePicUrl,
                                                                                                  FromUserId = f.FromUserId,
                                                                                                  ToUserId = f.ToUserId,
                                                                                                  CreatedDate = f.AddedDate,
                                                                                                  NotificationDate = DateCalculation(f.AddedDate),
                                                                                                  NotificationType = (int)NotificationType.followUser
                                                                                              }).OrderByDescending(n => n.CreatedDate);
        }


        internal IEnumerable<NotificationsViewModel> GetProfileNotifications(string userid, IGoalService goalService, ICommentService commentService, IUpdateService updateService, ISupportService supportService, IUserService userService, IGroupService groupService, IGroupUserService groupUserSevice, IGroupGoalService groupGoalService, IGroupCommentService groupcommentService, IGroupUpdateService groupupdateService, ICommentUserService commentUserService)
        {
            var goals = goalService.GetTop20Goals(userid);
            var comments = commentService.GetTop20CommentsOfPublicGoals(userid);
            var updates = updateService.GetTop20Updates(userid);
            var groupusers = groupUserSevice.GetTop20GroupsUsersForProfile(userid);
            var supports = supportService.GetTop20Support(userid);


            return (from g in goals
                    select new NotificationsViewModel()
                    {
                        GoalId = g.GoalId,
                        UserId = g.UserId,
                        CreatedDate = g.CreatedDate,
                        UserName = g.User.UserName,
                        GoalName = g.GoalName,
                        Desc = g.Desc,
                        ProfilePicUrl = g.User.ProfilePicUrl,
                        Goal = g,
                        NotificationDate = DateCalculation(g.CreatedDate),
                        NotificationType = (int)NotificationType.createdGoal
                    })
                    .Concat(from c in comments
                            select new NotificationsViewModel()
                            {
                                CommentId = c.CommentId,
                                CreatedDate = c.CommentDate,

                                UserName = commentUserService.GetUser(c.CommentId).UserName,
                                UserId = commentUserService.GetUser(c.CommentId).Id,
                                ProfilePicUrl = commentUserService.GetUser(c.CommentId).ProfilePicUrl,

                                CommentText = c.CommentText,
                                UpdateId = c.UpdateId,
                                GoalName = c.Update.Goal.GoalName,
                                GoalId = c.Update.GoalId,
                                NumberOfComments = commentService.GetCommentsByUpdate(c.UpdateId).Count(),
                                Updatemsg = updateService.GetUpdate(c.UpdateId).Updatemsg,
                                NotificationDate = DateCalculation(c.CommentDate),
                                NotificationType = (int)NotificationType.commentedOnUpdate
                            })
                              .Concat(from u in updates
                                      select new NotificationsViewModel()
                                      {
                                          Update = u,
                                          UpdateId = u.UpdateId,
                                          GoalId = u.GoalId,
                                          ProfilePicUrl = u.Goal.User.ProfilePicUrl,
                                          GoalName = u.Goal.GoalName,
                                          Updatemsg = u.Updatemsg,
                                          UserId = u.Goal.UserId,
                                          UserName = u.Goal.User.UserName,
                                          NumberOfComments = commentService.GetCommentsByUpdate(u.UpdateId).Count(),
                                          CreatedDate = u.UpdateDate,
                                          NotificationDate = DateCalculation(u.UpdateDate),
                                          NotificationType = (int)NotificationType.updatedGoal
                                      })
                                        .Concat(from s in supports
                                                select new NotificationsViewModel()
                                                {
                                                    Support = s,                                                    
                                                    SupportId = s.SupportId,
                                                    GoalName = s.Goal.GoalName,

                                                    ProfilePicUrl = userService.GetUser(s.UserId).ProfilePicUrl,
                                                    UserName = userService.GetUser(s.UserId).UserName,
                                                    UserId = s.UserId,                                             

                                                    CreatedDate = s.SupportedDate,
                                                    GoalId = s.GoalId,
                                                    NotificationType = (int)NotificationType.supportGoal
                                                })
                                                .Concat(from gu in groupusers
                                                        select new NotificationsViewModel()
                                                        {
                                                            GroupUser = gu,
                                                                                 
                                                            GroupUserId = gu.GroupUserId,
                                                            
                                                            UserName = userService.GetUser(gu.UserId).UserName,
                                                            ProfilePicUrl = userService.GetUser(gu.UserId).ProfilePicUrl,
                                                            UserId = gu.UserId,

                                                            GroupName = groupService.GetGroup(gu.GroupId).GroupName,
                                                            GroupId = gu.GroupId,
                                                            CreatedDate = gu.AddedDate,
                                                            NotificationDate = DateCalculation(gu.AddedDate),
                                                            NotificationType = (int)NotificationType.joinGroup
                                                        })
                                                        .OrderByDescending(n => n.CreatedDate);
        }

        private string DateCalculation(DateTime  CreatedDate)
        {
            string notificationdate;
            string creDate = CreatedDate.ToShortDateString();
            string dateNow = DateTime.Now.ToShortDateString();
             TimeSpan duration = DateTime.Now - CreatedDate;
            if (creDate == dateNow)
            {
               
                if (duration.Hours >= 1)
                {
                    if (duration.Hours == 1)
                    {
                        return notificationdate = duration.Hours.ToString() + " " + "Hour Ago";

                    }
                    else
                    {
                        return notificationdate = duration.Hours.ToString() + " " + "Hours Ago";
                    }
                }
                else if (duration.Minutes >= 1)
                {
                    if (duration.Minutes == 1)
                    {
                        return notificationdate = duration.Minutes.ToString() + " " + "Minute Ago";
                    }
                    else
                    {
                        return notificationdate = duration.Minutes.ToString() + " " + "Minutes Ago";
                    }
                }
                else
                {
                    return notificationdate = "Few Seconds Ago ";
                }
            }
            else
            {
                if (duration.Days == 0 && duration.Hours < 24)
                {
                    return notificationdate = duration.Hours.ToString() + " " + "Hours Ago";
                }
                else
                {
                    if (duration.Days.ToString() == "1")
                    {
                        return notificationdate = duration.Days.ToString() + " " + "Day Ago";
                    }
                    else
                    {
                        return notificationdate = duration.Days.ToString() + " " + "Days Ago";
                    }
                }
            }
            //else if (CreatedDate.Date ==  DateTime.Today.AddDays(-1).Date)
            //{
            //    return notificationdate = "Yesterday";
            //}
            //else
            //{
            //    return notificationdate = CreatedDate.ToShortDateString();
            //}
        }
    }
}