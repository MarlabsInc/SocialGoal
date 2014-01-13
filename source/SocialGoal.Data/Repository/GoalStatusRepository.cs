using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
namespace SocialGoal.Data.Repository
{
    public class GoalStatusRepository : RepositoryBase<GoalStatus>, IGoalStatusRepository
    {
        public GoalStatusRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IGoalStatusRepository : IRepository<GoalStatus>
    {
    }
}