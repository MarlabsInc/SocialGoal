using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;

namespace SocialGoal.Data.Repository
{
    public class GroupRepository:RepositoryBase<Group>, IGroupRepository
        {
        public GroupRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }           
        }
    public interface IGroupRepository : IRepository<Group>
    {
    }
   
}
