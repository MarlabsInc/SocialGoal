using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
namespace SocialGoal.Data.Repository
{
    public class GroupRequestRepository: RepositoryBase<GroupRequest>, IGroupRequestRepository
        {
        public GroupRequestRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }           
        }
    public interface IGroupRequestRepository : IRepository<GroupRequest>
    {
    }
}