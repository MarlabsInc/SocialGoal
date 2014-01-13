using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SocialGoal.Model.Models;
using System.Web.Mvc;

namespace SocialGoal.Web.ViewModels
{
    public class GroupGoalFormModel
    {
        public int GroupGoalId { get; set; }

       [Required(ErrorMessage = "*")]
       [StringLength(50)]
        public string GoalName { set; get; }

        [Required(ErrorMessage = "*")]
        [StringLength(100)]
        public string Description { get; set; }

        [Required(ErrorMessage = "*")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "*")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? EndDate { get; set; }

        public double? Target { get; set; }
        
        public int? MetricId { get; set; }

        public int? FocusId { get; set; }

        public int GroupUserId { get; set; }

        public int? AssignedGroupUserId { get; set; }

        public string AssignedTo { get; set; }

        public DateTime CreatedDate { get; set; }

        public int GroupId { get; set; }

        public IEnumerable<SelectListItem> Metrics { get; set; }

        public IEnumerable<SelectListItem> Foci { get; set; }

        //public virtual GroupUser GroupUser { get; set; }

        //public virtual User User { get; set; }

        public virtual Metric Metric { get; set; }

        public virtual Focus Focus { get; set; }

        public virtual Group Group { get; set; }

        public virtual IEnumerable<GroupUpdate> Updates { get; set; }

        public GroupGoalFormModel()
        {
            CreatedDate = DateTime.Now;
           // StartDate = DateTime.Now;
           // EndDate = DateTime.Now;
        }
    }
}