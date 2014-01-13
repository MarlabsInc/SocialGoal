using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SocialGoal.Model.Models;
using System.Web.Mvc;

namespace SocialGoal.Web.ViewModels
{
    public class GroupGoalViewModel
    {
         public int GroupGoalId { get; set; }
       [Required(ErrorMessage = "*")]
        public string GoalName { set; get; }
        [Required(ErrorMessage = "*")]
        public string Description { get; set; }

        [Required(ErrorMessage = "*")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "*")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime EndDate { get; set; }

        public double? Target { get; set; }
        
        public int? MetricId { get; set; }

        public int? FocusId { get; set; }

        public string UserId { get; set; }

        public int GoalStatusId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int GroupUserId { get; set; }

        public int AssignedGroupUserId { get; set; }

        public string AssignedUserId { get; set; }

        public string AssignedTo { get; set; }

        public int GroupId { get; set; }

        public virtual GoalStatus GoalStatus { get; set; }

        public IEnumerable<SelectListItem> GoalStatuses { get; set; }

        public ApplicationUser User { get; set; }

        public virtual GroupUser GroupUser { get; set; }

        public virtual Metric Metric { get; set; }

        public virtual Focus Focus { get; set; }

        public virtual Group Group { get; set; }

        public virtual IEnumerable<GroupUpdate> Updates { get; set; }

        public IEnumerable<ApplicationUser> Users { get; set; }

        public bool IsAMember { get; set; }

        //public GroupGoalViewModel()
        //{
        //    CreatedDate = DateTime.Now;
        //    StartDate = DateTime.Now;
        //    EndDate = DateTime.Now;
        //}
    }
}