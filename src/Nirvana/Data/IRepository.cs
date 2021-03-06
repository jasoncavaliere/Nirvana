﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Nirvana.CQRS;
using Nirvana.Data.EntityTypes;
using Nirvana.Domain;

namespace Nirvana.Data
{
    public interface ISqlRepository
    {
        IEnumerable<T> Sql<T>(string sql, params object[] parameters);
    }


    public interface IRepository<TRoot> where TRoot : ServiceRootType
    {
        T Get<T>(Guid id) where T : Entity<Guid>;
        T Get<T>(long id) where T : Entity<long>;

        IQueryable<T> GetAll<T>() where T : Entity;
        IQueryable<T> GetAllAndInclude<T>(IList<Expression<Func<T, object>>> includes) where T : Entity;
        IQueryable<T> Include<T>(IQueryable<T> queryable, IList<Expression<Func<T, object>>> includes) where T : Entity;
        IQueryable<T> GetAllAndInclude<T, TProperty>(Expression<Func<T, TProperty>> path) where T : Entity;

        PagedResult<T> GetPaged<T>(PaginationQuery pageInfo,
            IList<Expression<Func<T, bool>>> conditions,
            IList<Expression<Func<T, object>>> orders = null,
            IList<Expression<Func<T, object>>> includes = null
        ) where T : Entity;

        void EnableSoftDeleteLoading();
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();

        void SaveOrUpdate<T>(T entity) where T : Entity;
        void SaveOrUpdateCollection<T>(IEnumerable<T> entities) where T : Entity;
        void Delete<T>(T entity) where T : Entity;
        void DeleteCollection<T>(IEnumerable<T> entities) where T : Entity;

        T GetAndInclude<T, TProperty>(Guid id, Expression<Func<T, TProperty>> path)
            where T : Entity<Guid>
            where TProperty : Entity;

        int SqlCommand(string sql, params object[] parameters);

        void ClearContext();
        T Refresh<T>(T entity) where T : Entity;
        IEnumerable<T> Refresh<T>(IEnumerable<T> entities) where T : Entity;

        IQueryable<T> Include<T>(IQueryable<T> query, params Expression<Func<T, object>>[] includes)
            where T : class;
    }

    public static class RepositoryExtensions
    {
        public static IQueryable<T> Include<TRoot,T>(this IQueryable<T> input, IRepository<TRoot> repository, params Expression<Func<T, object>>[] includes) 
            where T : class 
            where TRoot : ServiceRootType
        {
            return repository.Include(input, includes);
        }
    }

}