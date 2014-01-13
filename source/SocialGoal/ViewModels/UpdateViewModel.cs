using System;
using System.Collections.Generic;
using SocialGoal.Model.Models;
using System.Linq;
using System.Web;

namespace SocialGoal.Web.ViewModels
{
    public class UpdateViewModel
    {
        public int UpdateId { get; set; }

        public string Updatemsg { get; set; }

        public double? status { get; set; }

        public int GoalId { get; set; }

        public DateTime UpdateDate { get; set; }

        public virtual Goal Goal { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public bool? IsSupported { get; set; }

        public virtual ICollection<UpdateSupport> UpdateSupports { get; set; }
    }
}