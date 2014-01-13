using SocialGoal.Model.Models;
using SocialGoal.Data.Infrastructure;


namespace SocialGoal.Data.Repository
{
    public class GroupGoalRepository : RepositoryBase<GroupGoal>, IGroupGoalRepository
    {
        public GroupGoalRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IGroupGoalRepository : IRepository<GroupGoal>
    {
    }
}
