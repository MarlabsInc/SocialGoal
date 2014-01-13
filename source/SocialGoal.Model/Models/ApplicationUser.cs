using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        //private string userName;

        //public string UserName
        //{
        //    get { return userName; }
        //    set { userName = FirstName + " " + LastName; }
        //}

        //public string PasswordHash { get; set; }

        public string ProfilePicUrl { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public bool Activated { get; set; }

        public int RoleId { get; set; }

        public virtual ICollection<Goal> Goals { get; set; }

        public virtual ICollection<FollowUser> FollowFromUser { get; set; }

        public virtual ICollection<FollowUser> FollowToUser { get; set; }

        public virtual ICollection<GroupRequest> GroupRequests { get; set; }

        

        //public string Password
        //{
        //    set { PasswordHash = Md5Encrypt.Md5EncryptPassword(value); }
        //}

        //internal static string GenerateRandomString()
        //{
        //    var builder = new StringBuilder();
        //    var random = new Random();
        //    for (int i = 0; i < 6; i++)
        //    {
        //        char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(25 * random.NextDouble() + 75)));
        //        builder.Append(ch);
        //    }
        //    return builder.ToString();
        //}

        //public string ResetPassword()
        //{
        //    var newPass = GenerateRandomString();
        //    Password = newPass;
        //    return newPass;
        //}

        public string DisplayName
        {
            get { return FirstName + " " + LastName; }
        }


    }
}
