using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
namespace SocialGoal.Data.Repository
{
    public class CommentRepository: RepositoryBase<Comment>, ICommentRepository
        {
        public CommentRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
            {
            }

        //public int Add(Comment comment)
        //{
        //    this.DataContext.Comments.Add(comment);
        //    this.DataContext.SaveChanges();
        //    return comment.CommentId;
        //}
        }
    public interface ICommentRepository : IRepository<Comment>
    {
        //int Add(Comment comment);

    }
}