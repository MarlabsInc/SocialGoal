using System;
using SocialGoal.Model.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocialGoal.Data.Infrastructure;

namespace SocialGoal.Data.Repository
{
    class GroupUpdateSupportRepository : RepositoryBase<GroupUpdateSupport>, IGroupUpdateSupportRepository
    {
        public GroupUpdateSupportRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IGroupUpdateSupportRepository : IRepository<GroupUpdateSupport>
    {
    }
}
