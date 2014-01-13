using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SocialGoal.Model.Models;

namespace SocialGoal.Web.ViewModels
{
    public class GroupViewModel
    {

        public int GroupId { get; set; }

        [Required]
        public string GroupName { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }

        //public int UserId { get; set; }

        //public int GroupUserId { get; set; }

        public virtual IEnumerable<GroupGoalViewModel> Goals { get; set; }

        public bool IsAMember { get; set; }

        public bool RequestSent { get; set; }

        public bool InvitationSent { get; set; }

        public int NoOfMembers { get; set; }

        public bool Admin { get; set; }

        public virtual IEnumerable<Focus> Focus { get; set; }

        public virtual IEnumerable<GroupGoalViewModel> GoalsAssignedToOthers { get; set; }

        public virtual IEnumerable<GroupGoalViewModel> GoalsAssignedToMe { get; set; }

        //public virtual GroupUser GroupUser { get; set; }

        public IEnumerable<ApplicationUser> Users { get; set; }

    }
}