using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace SocialGoal.Web.ViewModels
{
    public class GoalFormModel
    {
        //public GoalFormModel()
        //{
        //    StartDate = DateTime.Now;
        //    EndDate = DateTime.Now;

        //}
        public int GoalId { get; set; }

        [Required(ErrorMessage = "*")]
        [StringLength(50)]
        public string GoalName { set; get; }

        [Required(ErrorMessage = "*")]
        [StringLength(100)]
        public string Desc { get; set; }

        [Required(ErrorMessage = "*")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "*")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? EndDate { get; set; }

        public double? Target { get; set; }

        public bool GoalType { get; set; }

        public string UserId { get; set; }

        public int? MetricId { get; set; }

        //public IEnumerable<SelectListItem> MetricType { get; set; }

        //public IEnumerable<Metric> Metrics { get; set; }
        public IEnumerable<SelectListItem> Metrics{ get; set; }

        
    }
    
}