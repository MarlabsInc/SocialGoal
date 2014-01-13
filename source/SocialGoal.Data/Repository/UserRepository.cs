using SocialGoal.Data.Infrastructure;
using SocialGoal.Data.Models;
using SocialGoal.Model.Models;
using System;
using System.Linq.Expressions;
namespace SocialGoal.Data.Repository
{
    public class UserRepository: RepositoryBase<ApplicationUser>, IUserRepository
        {
        public UserRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }
        public ApplicationUser GetUserById(string userId)
        {
            SocialGoalEntities dbContext=new SocialGoalEntities();
            var user = dbContext.Users.Find(userId);
            return user;
        }
        }
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        ApplicationUser GetUserById(string userId);
        
    }
}