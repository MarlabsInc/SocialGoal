using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using SocialGoal.Model.Models;

namespace SocialGoal.Data.Configuration
{
    public class GroupConfiguration: EntityTypeConfiguration<Group>
    {
        public GroupConfiguration()
        {
            Property(g => g.GroupName).HasMaxLength(50);
            Property(g => g.CreatedDate).IsRequired();
            Property(g => g.Description).IsMaxLength();
        }
    }
}
