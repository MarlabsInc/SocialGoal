using SocialGoal.Data.Models;

namespace SocialGoal.Data.Infrastructure
{
public class DatabaseFactory : Disposable, IDatabaseFactory
{
    private SocialGoalEntities _dataContext;
    public SocialGoalEntities Get()
    {
        return _dataContext ?? (_dataContext = new SocialGoalEntities());
    }
    protected override void DisposeCore()
    {
        if (_dataContext != null)
            _dataContext.Dispose();
    }
}
}
