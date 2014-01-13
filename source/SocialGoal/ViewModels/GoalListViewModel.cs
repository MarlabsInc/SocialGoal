using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SocialGoal.Model.Models;

namespace SocialGoal.Web.ViewModels
{
    public class GoalListViewModel
    {
        public int GoalId { get; set; }

        public string GoalName { set; get; }

        public string Desc { get; set; }


        public string StartDate { get; set; }


        public string EndDate { get; set; }

      
        public string UserId { get; set; }

        
        public string UserName { get; set; }

        public string CreatedDate { get; set; }

      

        public int SupportsCount { get; set; }

        
       
    }
}