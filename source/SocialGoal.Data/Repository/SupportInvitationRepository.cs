using SocialGoal.Model.Models;
using SocialGoal.Data.Infrastructure;

namespace SocialGoal.Data.Repository
{
    public class SupportInvitationRepository : RepositoryBase<SupportInvitation>, ISupportInvitationRepository
    {
        public SupportInvitationRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface ISupportInvitationRepository : IRepository<SupportInvitation>
    {
    }
}
