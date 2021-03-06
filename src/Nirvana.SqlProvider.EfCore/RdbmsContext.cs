﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Nirvana.Data;
using Nirvana.Data.EntityTypes;
using Nirvana.Domain;

namespace Nirvana.SqlProvider
{


    public abstract class RdbmsContext : DbContext, IDataContext
    {
        protected readonly ISaveChangesDecorator[] _saveChangesDecorators;

        protected RdbmsContext(string connectionName) : this(ActiveDecoratorType.Live, connectionName)
        {
        }

        protected RdbmsContext(ActiveDecoratorType type, string connectionType) : base(new DbContextOptionsBuilder<RdbmsContext>().UseSqlServer(connectionType).Options)
        {
            _saveChangesDecorators = new SaveChangesDecoratorFactory().Build(type);
        }

        public override int SaveChanges()
        {
            Func<int> saveChanges = () => base.SaveChanges();

            foreach (var decorator in _saveChangesDecorators)
            {
                var newContext = new SaveChangesContext(this, saveChanges);

                var localDecorator = decorator;
                saveChanges = () => localDecorator.Decorate(newContext);
            }

            return saveChanges();
        }


        public IEnumerable<Entity> GetEntities(EntityChangeState state)
        {

            var efState = GetState(state);
            return this.ChangeTracker.Entries<Entity>()
                .Where(x => x.State == efState)
                .Select(x => x.Entity);
        }

        private EntityState GetState(EntityChangeState state)
        {
            switch (state)
            {
                case EntityChangeState.Unchanged:
                    return EntityState.Unchanged;
                case EntityChangeState.Added:
                    return EntityState.Added;
                case EntityChangeState.Deleted:
                    return EntityState.Deleted;
                case EntityChangeState.Modified:
                    return EntityState.Modified;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        protected static IEnumerable<Type> EntityTypesByNamespace(string baseNamespace, Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(
                    x =>
                        !x.IsAbstract && typeof(Entity).IsAssignableFrom(x) &&
                        x.Namespace.Contains(baseNamespace));
        }

    }



    public abstract class RdbmsContext<T> : RdbmsContext 
        where T: ServiceRootType
    {


      
        protected RdbmsContext(ActiveDecoratorType type) : base(type, GetConnectionStringName())
        {
        }

        protected RdbmsContext(ActiveDecoratorType type, string connectionStringName)
            : base(type, connectionStringName)
        {
        }

        private static string GetConnectionStringName()
        {
            return nameof(T) + "ConnectionString";
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureModel(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void ConfigureModel(ModelBuilder modelBuilder)
        {
            var entityMethod = typeof(ModelBuilder).GetMethod("Entity");

            foreach (var type in GetAllEntityTypes())
            {
                entityMethod.MakeGenericMethod(type).Invoke(modelBuilder, new object[] {});
            }
            

        }

        public abstract IEnumerable<Type> GetAllEntityTypes();

    }


   
}
