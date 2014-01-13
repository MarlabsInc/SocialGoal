using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using SocialGoal.Data.Models;
using SocialGoal.Model.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SocialGoal.Data
{
    public class GoalsSampleData : DropCreateDatabaseIfModelChanges<SocialGoalEntities>
    {
        protected override void Seed(SocialGoalEntities context)
        {
            //var ApplicationUser = new ApplicationUser
            //{
            //    Email = "admin@socialgoal.com",
            //    FirstName = "Admin",
            //    LastName = "Social Goal",
            //    Password = "123456",
            //    Activated = true,
            //    RoleId = 1
            //};
            //context.Users.Add(user);
            //context.Commit();

            //user = context.Users.Where(u => u.Email == "admin@socialgoal.com").FirstOrDefault();
            //var userProfile = new UserProfile
            //{
            //    Email = "admin@socialgoal.com",
            //    FirstName = "Admin",
            //    LastName = "Social Goal",
            //    UserId = user.UserId
            //};
            //context.UserProfile.Add(userProfile);
            //context.Commit();
            var UserManager = new UserManager<ApplicationUser>(new

                                                UserStore<ApplicationUser>(context));

            var RoleManager = new RoleManager<IdentityRole>(new
                                       RoleStore<IdentityRole>(context));

            string name = "Admin";
            string password = "123456";


            //Create Role Admin if it does not exist
            if (!RoleManager.RoleExists(name))
            {
                var roleresult = RoleManager.Create(new IdentityRole(name));
            }

            //Create User=Admin with password=123456
            var user = new ApplicationUser();
            user.UserName = name;
            var adminresult = UserManager.Create(user, password);

            //Add User Admin to Role Admin
            if (adminresult.Succeeded)
            {
                var result = UserManager.AddToRole(user.Id, name);
            }
            //base.Seed(context);
            context.Commit();

            new List<Metric>
            {
                new Metric { Type ="%"},
                new Metric { Type ="$"},
                new Metric { Type ="$ M"},
                new Metric { Type ="Rs"},
                new Metric { Type ="Hours"},
                new Metric { Type ="Km"},
                new Metric { Type ="Kg"},
                new Metric { Type ="Years"}

            }.ForEach(m => context.Metrics.Add(m));

            new List<GoalStatus>
            {
                new GoalStatus{GoalStatusType="In Progress"},
                new GoalStatus{GoalStatusType="On Hold"},
                new GoalStatus{GoalStatusType="Completed"}
            }.ForEach(m => context.GoalStatus.Add(m));

            context.Commit();

        }

    }
}