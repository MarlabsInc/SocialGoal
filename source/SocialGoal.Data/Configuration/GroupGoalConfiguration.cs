using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using SocialGoal.Model.Models;

namespace SocialGoal.Data.Configuration
{
    public class GroupGoalConfiguration: EntityTypeConfiguration<GroupGoal>
    {
        public GroupGoalConfiguration()
        {
            Property(g => g.GroupId).IsRequired();
            Property(g => g.GoalName).HasMaxLength(50);
            Property(g => g.Description).HasMaxLength(100);
            Property(g => g.StartDate).IsRequired();
            Property(g => g.EndDate).IsRequired();
            Property(g => g.GoalStatusId).IsRequired();
            Property(g => g.GroupUserId).IsRequired();
        }
    }
}
