using SocialGoal.Model.Models;
using SocialGoal.Data.Infrastructure;

namespace SocialGoal.Data.Repository
{
    public class GroupInvitationRepository : RepositoryBase<GroupInvitation>, IGroupInvitationRepository
    {
        public GroupInvitationRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IGroupInvitationRepository : IRepository<GroupInvitation>
    {
    }
}
