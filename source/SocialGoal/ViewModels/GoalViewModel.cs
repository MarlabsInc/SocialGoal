using System;
using System.Collections.Generic;
using SocialGoal.Model.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SocialGoal.Web.ViewModels
{
    public class GoalViewModel
    {
        public int GoalId { get; set; }

        public string GoalName { set; get; }

        public string Desc { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime EndDate { get; set; }

        public double? Target { get; set; }

        public bool GoalType { get; set; }

        public int MetricId { get; set; }

        public int GoalStatusId { get; set; }

        public string UserId { get; set; }

        public Metric Metric { get; set; }

        public IEnumerable<SelectListItem> GoalStatuses { get; set; }

        public GoalStatus GoalStatus { get; set; }

        public ApplicationUser User { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool? Supported { get; set; }

      //  public int? GroupGoalId { get; set; }

        public int? GroupId { get; set; }

        public virtual IEnumerable<Support> Supports { get; set; }

        public virtual IEnumerable<Update> Updates { get; set; }

    }
}