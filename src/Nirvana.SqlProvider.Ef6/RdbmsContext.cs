﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using Nirvana.Data;
using Nirvana.Data.EntityTypes;
using Nirvana.Domain;

namespace Nirvana.SqlProvider
{


    public abstract class RdbmsContext : DbContext, IDataContext
    {
        protected readonly ISaveChangesDecorator[] _saveChangesDecorators;

        protected RdbmsContext(DataConfiguration config,string connectionName) : this(ActiveDecoratorType.Live,config, connectionName)
        {
        }

        protected RdbmsContext(ActiveDecoratorType type, DataConfiguration config,string connectionName) : base(GetconnectionString(config,connectionName))
        {
            _saveChangesDecorators = new SaveChangesDecoratorFactory().Build(type);
        }

        private static string GetconnectionString(DataConfiguration config, string connectionName)
        {
            return config.GetConnectionString(connectionName);
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


        public ObjectContext ObjectContext => ((IObjectContextAdapter) this).ObjectContext;

        protected RdbmsContext(ActiveDecoratorType type,DataConfiguration config) : base(type, config,GetConnectionStringName())
        {
        }

        protected RdbmsContext(ActiveDecoratorType type,DataConfiguration config, string connectionStringName)
            : base(type,config, connectionStringName)
        {
        }

        public static string GetConnectionStringName()
        {
            return nameof(T) + "ConnectionString";
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            ConfigureModel(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void ConfigureModel(DbModelBuilder modelBuilder)
        {
            var entityMethod = typeof(DbModelBuilder).GetMethod("Entity");

            foreach (var type in GetAllEntityTypes())
            {
                entityMethod.MakeGenericMethod(type).Invoke(modelBuilder, new object[] {});
            }
            

        }

        public abstract IEnumerable<Type> GetAllEntityTypes();

    }


   
}
