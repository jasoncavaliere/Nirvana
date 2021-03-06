﻿using System;
using System.Linq;
using MongoDB.Driver;
using Nirvana.Data;
using Nirvana.Domain;

namespace Nirvana.MongoProvider
{
    public class MonogoViewModelRepository : IViewModelRepository
    {
        protected readonly IMongoClient Client;
        protected readonly IMongoDatabase Database;
        private string _connectionString;


        public MonogoViewModelRepository(NirvanaMongoConfiguration config)
        {
            _connectionString = $"mongodb://{config.ServerName}:{config.Port}/{config.Database}";
            Client = new MongoClient(_connectionString);
            Database = Client.GetDatabase("ViewModels",new MongoDatabaseSettings
            {
                
            });
        }

        public void Dispose()
        {
        }

        public T Get<T>(Guid id) where T : ViewModel<Guid>
        {
            return GetAll<T>().FirstOrDefault(x => x.Id == id);
        }

        public void Save<T>(T input) where T : ViewModel<Guid>
        {
            Database.GetCollection<T>(typeof(T).Name).InsertOneAsync(input);
        }

        public IQueryable<T> GetAll<T>() where T : ViewModel<Guid>
        {
            return Database.GetCollection<T>(typeof(T).Name).AsQueryable();
        }


        public void BeginTransaction()
        {
        }

        public void CommitTransaction()
        {
        }

        public void RollbackTransaction()
        {
        }
    }

    public class NirvanaMongoConfiguration
    {
        public string ServerName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public int Port { get; set; }
    }
}