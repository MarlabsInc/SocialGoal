using System;
using System.Security.Principal;
using System.Web.Security;


namespace SocialGoal.Web.Core.Models
{ 
    [Serializable]
    public class SocialGoalUser : IIdentity
    {
        public SocialGoalUser(){}
        public SocialGoalUser(string name, string displayName, string userId)
        {
            this.Name = name;
            this.DisplayName = displayName;
            this.UserId = userId;
        }
        public SocialGoalUser(string name, string displayName, string userId,string roleName)
        {
            this.Name = name;
            this.DisplayName = displayName;
            this.UserId = userId;
            this.RoleName = roleName;
        }
        public SocialGoalUser(string name, UserInfo userInfo)
            : this(name, userInfo.DisplayName, userInfo.UserId,userInfo.RoleName)
        {
            if (userInfo == null) throw new ArgumentNullException("userInfo");
            this.UserId = userInfo.UserId;
        }

        public SocialGoalUser(FormsAuthenticationTicket ticket)
            : this(ticket.Name, UserInfo.FromString(ticket.UserData))
        {
            if (ticket == null) throw new ArgumentNullException("ticket");
        }

        public string Name { get; private set; }

        public string AuthenticationType
        {
            get { return "GoalSetterForms"; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }

        public string DisplayName { get; private set; }
        public string RoleName { get; private set; }
        public string UserId { get; private set; }
    }
}
