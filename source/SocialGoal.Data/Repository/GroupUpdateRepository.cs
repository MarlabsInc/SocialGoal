using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
namespace SocialGoal.Data.Repository
{
    public class GroupUpdateRepository : RepositoryBase<GroupUpdate>, IGroupUpdateRepository
    {
        public GroupUpdateRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IGroupUpdateRepository : IRepository<GroupUpdate>
    {
    }
}