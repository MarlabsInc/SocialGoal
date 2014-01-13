using System;
using SocialGoal.Data.Models;

namespace SocialGoal.Data.Infrastructure
{
    public interface IDatabaseFactory : IDisposable
    {
        SocialGoalEntities Get();
    }
}
