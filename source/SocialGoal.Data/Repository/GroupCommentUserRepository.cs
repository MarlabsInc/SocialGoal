using SocialGoal.Model.Models;
using SocialGoal.Data.Infrastructure;

namespace SocialGoal.Data.Repository
{
    public class GroupCommentUserRepository : RepositoryBase<GroupCommentUser>, IGroupCommentUserRepository
    {
        public GroupCommentUserRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        
    }
    public interface IGroupCommentUserRepository : IRepository<GroupCommentUser>
    {
        
    }
}
