using System.Collections.Generic;
using SocialGoal.Data.Repository;
using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;

namespace SocialGoal.Service
{
    public interface IMetricService
    {
        IEnumerable<Metric> GetMetrics();
        Metric GetMetric(int id);
        void CreateMetric(Metric metric);
        void DeleteMetric(int id);
        void SaveMetric();
    }
  
    public class MetricService : IMetricService
    {
        private readonly IMetricRepository metricRepository;
        private readonly IUnitOfWork unitOfWork;
      
        public MetricService(IMetricRepository metricRepository, IUnitOfWork unitOfWork)
        {
            this.metricRepository = metricRepository;
            this.unitOfWork = unitOfWork;
        }
     
        #region IMetricService Members

        public IEnumerable<Metric> GetMetrics()
        {
            var metric = metricRepository.GetAll();
            return metric;
        }

        public Metric GetMetric(int id)
        {
            var metric = metricRepository.GetById(id);
            return metric;
        }

        public void CreateMetric(Metric metric)
        {
            metricRepository.Add(metric);
            SaveMetric();
        }

        public void DeleteMetric(int id)
        {
            var metric = metricRepository.GetById(id);
            metricRepository.Delete(metric);
            SaveMetric();
        }

        public void SaveMetric()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
