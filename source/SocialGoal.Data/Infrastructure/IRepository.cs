using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PagedList;

namespace SocialGoal.Data.Infrastructure
{
public interface IRepository<T> where T : class
{
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    void Delete(Expression<Func<T, bool>> where);
    T GetById(long Id);
    T GetById(string Id);
    T Get(Expression<Func<T, bool>> where);
    IEnumerable<T> GetAll();
    IEnumerable<T> GetMany(Expression<Func<T, bool>> where);
    IPagedList<T> GetPage<TOrder>(Page page, Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> order);
}
}
