using System.ComponentModel.DataAnnotations;
using SocialGoal.Model.Models;

namespace SocialGoal.Web.ViewModels
{
    public class GroupUpdateFormModel
    {
        public int GroupUpdateId { get; set; }
        [Required(ErrorMessage = "*")]
        public string Updatemsg { get; set; }

        public double? status { get; set; }

        public int GroupGoalId { get; set; }

        public int GroupId { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public GroupGoal GroupGoal { get; set; }
    }
}