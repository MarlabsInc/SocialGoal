using SocialGoal.Model.Models;
using SocialGoal.Data.Infrastructure;

namespace SocialGoal.Data.Repository
{
    public class FollowUserRepository : RepositoryBase<FollowUser>, IFollowUserRepository
    {
        public FollowUserRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        //public IEnumerable<User> SearchUserForGroup(string searchString, int groupId)
        //{

        //}
    }
    public interface IFollowUserRepository : IRepository<FollowUser>
    {
        //IEnumerable<User> SearchUserForGroup(string searchString, int groupId);
    }
}
