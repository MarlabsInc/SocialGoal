using System.Collections.Generic;
using SocialGoal.Model.Models;

namespace SocialGoal.Web.ViewModels
{
    public class GoalSupporterViewModel
    {
        public int GoalId { get; set; }

        public Goal Goal { get; set; }

        public IEnumerable<ApplicationUser> Users { get; set; }
    }
}