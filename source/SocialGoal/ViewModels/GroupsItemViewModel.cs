using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialGoal.Web.ViewModels
{
    public class GroupsItemViewModel
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public string Description { get; set; }

        public string CreatedDate { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}