using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using SocialGoal.Model.Models;

namespace SocialGoal.Data.Configuration
{
    public class GoalConfiguration: EntityTypeConfiguration<Goal>
    {
        public GoalConfiguration()
        {
            Property(g => g.GoalName).IsRequired().HasMaxLength(55);
            Property(g => g.GoalType).IsRequired();
            Property(g => g.GoalStatusId).IsRequired();
            Property(g => g.StartDate).IsRequired();
            Property(g => g.EndDate).IsRequired();
            Property(g => g.CreatedDate).IsRequired();
            Property(g => g.Desc).HasMaxLength(100);
        }
    }
}
