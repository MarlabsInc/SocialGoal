using System;
using System.ComponentModel.DataAnnotations;

namespace SocialGoal.Web.ViewModels
{
    public class UserProfileFormModel
    {
        public UserProfileFormModel()
        {
            DateEdited = DateTime.Now;
            //DateOfBirth = DateTime.Now;
        }

        public int UserProfileId { get; set; }

        public DateTime DateEdited { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy}")]
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

        public string UserId { get; set; }



    }
}