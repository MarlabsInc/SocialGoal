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
        }
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        
    }
}