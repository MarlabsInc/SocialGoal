using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using SocialGoal.Model.Models;

namespace SocialGoal.Data.Configuration
{
   public class CommentConfiguration:EntityTypeConfiguration<Comment>
    {
       public CommentConfiguration()
       {
           Property(c => c.CommentText).HasMaxLength(250);
       }
    }
}
