using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using SocialGoal.Model.Models;

namespace SocialGoal.Data.Configuration
{
    public class GroupInvitationConfiguration: EntityTypeConfiguration<GroupInvitation>
    {
        public GroupInvitationConfiguration()
        {
            Property(g => g.FromUserId).IsMaxLength();
            Property(g => g.ToUserId).IsMaxLength();
            Property(g => g.GroupId).IsRequired();
            Property(g => g.SentDate).IsRequired();
            Property(g => g.Accepted).IsRequired();
        }
    }
}
