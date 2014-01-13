using System;
using SocialGoal.Model.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocialGoal.Data.Infrastructure;

namespace SocialGoal.Data.Repository
{
    class UpdateSupportRepository : RepositoryBase<UpdateSupport>, IUpdateSupportRepository
    {
        public UpdateSupportRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IUpdateSupportRepository : IRepository<UpdateSupport>
    {
    }
}
