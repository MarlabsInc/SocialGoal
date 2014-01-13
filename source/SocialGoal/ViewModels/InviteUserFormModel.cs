using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SocialGoal.Web.ViewModels
{
    public class InviteUserFormModel
    {
        [RegularExpression(@"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string Role { get; set; }

        public IEnumerable<SelectListItem> UserRoles { get; set; }

        public IEnumerable<SelectListItem> InvitationModes { get; set; }

        public InviteUserFormModel()
        {
            UserRoles = new SelectList(new[]{
                       new SelectListItem{ Text="User", Value="User"},
                       new SelectListItem{ Text="Admin", Value="Admin"}
                       }, "Text", "Value", "User");
            InvitationModes=new SelectList(new[]{
                        new SelectListItem{Text="Single User", Value="Single User"},
                        new SelectListItem{Text="Multiple Users", Value="Multiple Users"}
                        },"Text","Value","Single Users");
        }

    }
}