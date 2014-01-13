using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SocialGoal.Web.ViewModels
{
    public class UserFormModel
    {
        public UserFormModel()
        {
            Login = new LoginFormModel();
            Register = new RegisterFormModel();
        }
        public LoginFormModel Login { get; set; }

        public RegisterFormModel Register { get; set; }
    }
    public class RegisterFormModel
    {
            [Required(ErrorMessage = "*")]
            [Display(Name = "First name")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "*")]
            [Display(Name = "Last name")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "*")]
            [RegularExpression(@"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
            [DataType(DataType.EmailAddress)]
            [Display(Name = "Email address")]
            public string Email { get; set; }

            [Required(ErrorMessage = "*")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password Mismatch")]
            public string ConfirmPassword { get; set; }

            public bool RememberMe { get; set; }
    }

    public class LoginFormModel
    {
        [Required(ErrorMessage = "*")]
        [RegularExpression(@"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}