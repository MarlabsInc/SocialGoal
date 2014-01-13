using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SocialGoal.Model.Models;
using SocialGoal.Web.Core.Models;

namespace SocialGoal.Web.ViewModels
{
    public class FocusViewModel
    {
        public int FocusId { get; set; }

        public string FocusName { get; set; }

        public string Description { get; set; }

        public int GroupId { get; set; }

        public virtual Group Group { get; set; }

        public bool IsAMember { get; set; }

        public virtual IEnumerable<GroupGoalViewModel> GroupGoal { get; set; }

        public virtual IEnumerable<ApplicationUser> Users { get; set; }

    }
}