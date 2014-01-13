using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mvc.Mailer;
using System.Net.Mail;

namespace SocialGoal.Web.Mailers
{ 
	public interface IUserMailer
	{

		MvcMailMessage Welcome();
		MvcMailMessage Invite(string email, Guid groupIdToken);

		MvcMailMessage Support(string email, Guid goalIdToken);
		MvcMailMessage ResetPassword(string email,Guid passwordResetToken);

        MvcMailMessage InviteNewUser(string email, Guid registrationToken);
		
	}
}