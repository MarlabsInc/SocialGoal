using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using SocialGoal.Model.Models;

namespace SocialGoal.Data.Configuration
{
    public class SupportInvitationConfiguration:EntityTypeConfiguration<SupportInvitation>
    {
        public SupportInvitationConfiguration()
        {
            Property(s => s.GoalId).IsRequired();
            Property(s => s.SentDate).IsRequired();
            Property(s => s.Accepted).IsRequired();
            Property(s => s.FromUserId).IsMaxLength();
            Property(s => s.ToUserId).IsMaxLength();
        }
    }
}
