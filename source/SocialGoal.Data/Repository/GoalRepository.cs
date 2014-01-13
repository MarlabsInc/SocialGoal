using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
using System.Collections.Generic;
using System.Linq;
using SocialGoal.Data.Repository;
using SocialGoal.Data.Models;
using System.Data.Entity;
using System;
namespace SocialGoal.Data.Repository
{
    public class GoalRepository : RepositoryBase<Goal>, IGoalRepository
    {

        public GoalRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }


        public IEnumerable<Goal> GetGoalsByPage(string userId, int currentPage, int noOfRecords, string sortBy, string filterBy)
        {
            var skipGoals = noOfRecords * currentPage;

            var goals = this.GetMany(g => g.GoalType == false);

            //for filter options
            //Following goals
            if (filterBy == "My Followings Goals")
            {
                goals = from g in goals
                        where (from f in this.DataContext.FollowUser.Where(fol => fol.FromUserId == userId) select f.ToUserId).ToList().Contains(g.UserId)
                        select g;
            }
            //User goals
            else if (filterBy == "My Goals")
            {
                goals = goals.Where(g => g.UserId == userId);
            }
            //Followed goals
            else if (filterBy =="My Followed Goals")
            {
                goals = from g in goals
                        join s in this.DataContext.Support on g.GoalId equals s.GoalId
                        where s.UserId == userId
                        select g;
            }

            //for sorting based on date and popularity
            goals = (sortBy == "Date") ? goals.OrderByDescending(g => g.CreatedDate) : goals;
            goals = (sortBy == "Popularity") ? goals.OrderByDescending(g => g.Supports.Count()) : goals;

            goals = goals.Skip(skipGoals).Take(noOfRecords);

            return goals.ToList();
        }
    }

    public interface IGoalRepository : IRepository<Goal>
    {
        /// <summary>
        /// /// Method will return goals as different page with specified number of records ,filter condition and sort criteria
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="currentPage"></param>
        /// <param name="noOfRecords"></param>
        /// <param name="sortBy"></param>
        /// <param name="filterBy"></param>
        /// <returns></returns>
        IEnumerable<Goal> GetGoalsByPage(string userId, int currentPage, int noOfRecords, string sortBy, string filterBy);
    }
}