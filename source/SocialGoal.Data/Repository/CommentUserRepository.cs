using SocialGoal.Model.Models;
using SocialGoal.Data.Infrastructure;

namespace SocialGoal.Data.Repository
{
    public class CommentUserRepository : RepositoryBase<CommentUser>, ICommentUserRepository
    {
        public CommentUserRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        
    }
    public interface ICommentUserRepository : IRepository<CommentUser>
    {
        
    }
}
