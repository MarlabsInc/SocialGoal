using System.Collections.Generic;
using SocialGoal.Model.Models;

namespace SocialGoal.Web.ViewModels
{
    public class UpdateSupportersViewModel
    {
        public int UpdateId { get; set; }

        public Update Update { get; set; }

        public IEnumerable<ApplicationUser> Users { get; set; }
    }
}