using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
namespace SocialGoal.Data.Repository
{
    public class FocusRepository: RepositoryBase<Focus>, IFocusRepository
        {
        public FocusRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }           
        }
    public interface IFocusRepository : IRepository<Focus>
    {
    }
}