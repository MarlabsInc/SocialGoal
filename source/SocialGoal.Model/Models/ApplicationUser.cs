﻿using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;

namespace SocialGoal.Model.Models
{
    public class ApplicationUser: IdentityUser
    {
        public ApplicationUser()
        {
            DateCreated = DateTime.Now;
        }

        //[Required]
        public string Email { get; set; }

        //[Required]
        public string FirstName { get; set; }

        //[Required]
        public string LastName { get; set; }       

        public string ProfilePicUrl { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public bool Activated { get; set; }

        public int RoleId { get; set; }

        public virtual ICollection<Goal> Goals { get; set; }

        public virtual ICollection<FollowUser> FollowFromUser { get; set; }

        public virtual ICollection<FollowUser> FollowToUser { get; set; }

        public virtual ICollection<GroupRequest> GroupRequests { get; set; }        

        public string DisplayName
        {
            get { return FirstName + " " + LastName; }
        }


    }
}
