using System.Collections.Generic;
using SocialGoal.Model.Models;

namespace SocialGoal.Web.ViewModels
{
    public class GroupUpdateSupportersViewModel
    {
        public int GroupUpdateId { get; set; }

        public GroupUpdate GroupUpdate { get; set; }

        public IEnumerable<ApplicationUser> Users { get; set; }
    }
}