using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using SocialGoal.Model.Models;

namespace SocialGoal.Data.Configuration
{
   public  class UserProfileConfiguration:EntityTypeConfiguration<UserProfile>
    {
       public UserProfileConfiguration()
       {
           Property(u => u.FirstName).HasMaxLength(100);
           Property(u => u.LastName).IsMaxLength();
           Property(u => u.DateEdited).IsRequired();
           Property(u => u.Email).IsMaxLength();
           Property(u => u.Address).IsMaxLength();
           Property(u => u.City).HasMaxLength(100);
           Property(u => u.Country).HasMaxLength(50);
           Property(u => u.State).HasMaxLength(50);
           Property(u => u.UserId).HasMaxLength(50);
       }
    }
}
