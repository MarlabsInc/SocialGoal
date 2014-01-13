using System.Collections.Generic;
using SocialGoal.Model.Models;

namespace SocialGoal.Web.ViewModels
{
    public class UpdateListViewModel
    {
        public IEnumerable<UpdateViewModel> Updates { get; set; }

        public double? Target { get; set; }

        public Metric Metric { get; set; }

    }
}