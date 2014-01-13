using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
namespace SocialGoal.Data.Repository
{
    public class SecurityTokenRepository: RepositoryBase<SecurityToken>, ISecurityTokenRepository
        {
        public SecurityTokenRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }           
        }
    public interface ISecurityTokenRepository : IRepository<SecurityToken>
    {
    }
}