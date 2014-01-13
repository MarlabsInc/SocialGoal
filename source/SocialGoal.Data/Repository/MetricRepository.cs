using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
namespace SocialGoal.Data.Repository
{
    public class MetricRepository: RepositoryBase<Metric>, IMetricRepository
        {
        public MetricRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }           
        }
    public interface IMetricRepository : IRepository<Metric>
    {
    }
}