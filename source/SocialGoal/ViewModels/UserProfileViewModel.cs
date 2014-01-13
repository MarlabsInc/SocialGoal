using System;
using System.ComponentModel.DataAnnotations;

namespace SocialGoal.Web.ViewModels
{
    public class UserProfileViewModel
    {
        public string UserId { get; set; }
        
        public string Email { get; set; }
  
        public string FirstName { get; set; }
        
        public string LastName { get; set; }

        public string ProfilePicUrl { get; set; }

         [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime DateCreated { get; set; }

        public DateTime? LastLoginTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? DateOfBirth { get; set; }

        public bool? Gender { get; set; }

        public string Address { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string City { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string State { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string Country { get; set; }

        public double? ZipCode { get; set; }

        [DataType(DataType.PhoneNumber)]
        public double? ContactNo { get; set; }

        public bool RequestSent { get; set; }

        public bool Following { get; set; }

        public string UserName{get;set;}

    }
}