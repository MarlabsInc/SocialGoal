using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
namespace SocialGoal.Data.Repository
{
    public class FollowRequestRepository : RepositoryBase<FollowRequest>, IFollowRequestRepository
    {
        public FollowRequestRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IFollowRequestRepository : IRepository<FollowRequest>
    {
    }
}