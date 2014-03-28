using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using SocialGoal.Model.Models;

namespace SocialGoal.Data.Configuration
{
    public  class FollowRequestConfiguration: EntityTypeConfiguration<FollowRequest>
    {
        public FollowRequestConfiguration()
        {
            Property(f => f.FromUserId).IsRequired();
            Property(f => f.ToUserId).IsRequired();
            Property(f => f.Accepted).IsRequired();
        }
    }
}
