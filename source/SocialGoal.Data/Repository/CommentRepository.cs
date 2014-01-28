using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
namespace SocialGoal.Data.Repository
{
    public class CommentRepository: RepositoryBase<Comment>, ICommentRepository
        {
        public CommentRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }
      
        }
    public interface ICommentRepository : IRepository<Comment>
    {    

    }
}