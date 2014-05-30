using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SocialGoal.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SocialGoal.Tests.Helpers
{
    public class TestUserStore : IUserStore<ApplicationUser>, IUserLoginStore<ApplicationUser>, IUserRoleStore<ApplicationUser>, IUserClaimStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserSecurityStampStore<ApplicationUser>
    {
        private Dictionary<string, ApplicationUser> _users;
        private Dictionary<IdentityUserLogin, ApplicationUser> _logins = new Dictionary<IdentityUserLogin, ApplicationUser>();


        public TestUserStore()
        {
            _users = new Dictionary<string, ApplicationUser>()
            {
                { "TestUser1", new ApplicationUser {FirstName="Sharon", UserName="Sharon", Id="15e8cda1-3ea3-473d-902f-b62aa55816db"}},
                { "TestUser2", new ApplicationUser {FirstName="adarsh", UserName="adarsh", Id="67e6yda1-3ea3-473d-902f-b62er55816db"}},
                { "TestUser3", new ApplicationUser {FirstName="Shiju",  UserName="Shiju",  Id="78e5fda1-3ea3-473d-902f-b62aa86716db"}}
            };

        }


        public Task CreateAsync(ApplicationUser user,string password)
        {
          _users[user.Id] = user;
            return Task.FromResult(0);
        }
       
        public Task CreateAsync(ApplicationUser user)
        { 
            _users[user.Id]=user;
            return Task.FromResult(0);
        }
        public Task UpdateAsync(ApplicationUser user)
        {
            _users[user.Id] = user;
            return Task.FromResult(0);
        }

        public Task<ApplicationUser> FindByIdAsync(string userId)
        {
            if (_users.ContainsKey(userId))
            {
                return Task.FromResult(_users[userId]);
            }
            return Task.FromResult<ApplicationUser>(null);
        }

        public void Dispose()
        {
        }

        public IQueryable<ApplicationUser> Users
        {
            get
            {
                return _users.Values.AsQueryable();
            }
        }
        public Task<ApplicationUser> FindAsync(string userName, string password)
        {
            var user = new ApplicationUser() { UserName = "adarsh", Id = "402bd590-fdc7-49ad-9728-40efbfe512ec", PasswordHash = "abcd" };
            return Task.FromResult(user);
        }
        public Task<ApplicationUser> FindByNameAsync(string userName)
        {

            foreach (ApplicationUser user in _users.Values)
            {
                if (user.UserName == userName)
                    return Task.FromResult(user);
            }
            return Task.FromResult<ApplicationUser>(null);
        }

        public Task AddLoginAsync(ApplicationUser user, IdentityUserLogin login)
        {
            user.Logins.Add(login);
            _logins[login] = user;
            return Task.FromResult(0);
        }

        public Task AddLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }
        public Task RemoveLoginAsync(ApplicationUser user, IdentityUserLogin login)
        {
            var logs = user.Logins.Where(l => l.ProviderKey == login.ProviderKey && l.LoginProvider == login.LoginProvider).ToList();
            foreach (var l in logs)
            {
                user.Logins.Remove(l);
                _logins[l] = null;
            }
            return Task.FromResult(0);
        }

        public Task RemoveLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> FindAsync(IdentityUserLogin login)
        {
            if (_logins.ContainsKey(login))
            {
                return Task.FromResult(_logins[login]);
            }
            return Task.FromResult<ApplicationUser>(null);
        }

        
        public Task<ApplicationUser> FindAsync(UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task AddToRoleAsync(ApplicationUser user, IdentityUserRole role)
        {
            user.Roles.Add(role);
            return Task.FromResult(0);
        }

        public Task AddToRoleAsync(ApplicationUser user, string role)
        {
            throw new NotImplementedException();
        }
        public Task RemoveFromRoleAsync(ApplicationUser user, IdentityUserRole role)
        {
            user.Roles.Remove(role);
            return Task.FromResult(0);
        }

        public Task RemoveFromRoleAsync(ApplicationUser user, string role)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            return Task.FromResult<IList<string>>(new List<string>());
        }

        public Task<bool> IsInRoleAsync(ApplicationUser user, string role)
        {
            return Task.FromResult<bool>(true);
        }

        public Task<IList<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            return Task.FromResult<IList<Claim>>(new List<Claim>());
        }

        public Task AddClaimAsync(ApplicationUser user, IdentityUserClaim claim)
        {
            user.Claims.Add(claim);
            return Task.FromResult(0);
        }

        public Task AddClaimAsync(ApplicationUser user, Claim claim)
        {
            throw new NotImplementedException();
        }

        public Task RemoveClaimAsync(ApplicationUser user, IdentityUserClaim claim)
        {
            user.Claims.Remove(claim);
            return Task.FromResult(0);
        }

        public Task RemoveClaimAsync(ApplicationUser user, Claim claim)
        {

            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task SetSecurityStampAsync(ApplicationUser user, string stamp)
        {
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(ApplicationUser user)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            return Task.FromResult(user.PasswordHash != null);
        }
    }
}
