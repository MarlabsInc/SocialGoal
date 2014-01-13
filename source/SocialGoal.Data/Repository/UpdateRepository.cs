using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
namespace SocialGoal.Data.Repository
{
    public class UpdateRepository: RepositoryBase<Update>, IUpdateRepository
        {
        public UpdateRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }           
        }
    public interface IUpdateRepository : IRepository<Update>
    {
    }
}