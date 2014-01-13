using SocialGoal.Model.Models;
using SocialGoal.Data.Infrastructure;

namespace SocialGoal.Data.Repository
{
    public class GroupUpdateUserRepository : RepositoryBase<GroupUpdateUser>, IGroupUpdateUserRepository
    {
        public GroupUpdateUserRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        
    }
    public interface IGroupUpdateUserRepository : IRepository<GroupUpdateUser>
    {
       
    }
}
