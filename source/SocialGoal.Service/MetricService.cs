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
        private readonly IMetricRepository _metricRepository;
        private readonly IUnitOfWork _unitOfWork;
      
        public MetricService(IMetricRepository metricRepository, IUnitOfWork unitOfWork)
        {
            _metricRepository = metricRepository;
            _unitOfWork = unitOfWork;
        }
     
        #region IMetricService Members

        public IEnumerable<Metric> GetMetrics()
        {
            var metric = _metricRepository.GetAll();
            return metric;
        }

        public Metric GetMetric(int id)
        {
            var metric = _metricRepository.GetById(id);
            return metric;
        }

        public void CreateMetric(Metric metric)
        {
            _metricRepository.Add(metric);
            SaveMetric();
        }

        public void DeleteMetric(int id)
        {
            var metric = _metricRepository.GetById(id);
            _metricRepository.Delete(metric);
            SaveMetric();
        }

        public void SaveMetric()
        {
            _unitOfWork.Commit();
        }

        #endregion
    }
}
