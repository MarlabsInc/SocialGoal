using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SocialGoal.Model.Models;

namespace SocialGoal.Web.ViewModels
{
    public class FollowRequestViewModel
    {
        public string FromUserId { get;set; }

        public string ToUserId { get; set; }

        public virtual ApplicationUser FromUser { get; set; }

        public virtual ApplicationUser ToUser { get; set; }
    }
}