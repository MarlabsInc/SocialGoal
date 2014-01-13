using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
namespace SocialGoal.Data.Repository
{
    public class GroupCommentRepository : RepositoryBase<GroupComment>, IGroupCommentRepository
    {
        public GroupCommentRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IGroupCommentRepository : IRepository<GroupComment>
    {
    }
}