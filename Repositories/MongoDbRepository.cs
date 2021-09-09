using System;
using System.Collections.Generic;
using Eze.Entities;
using MongoDB.Driver;
using System.Data;
using System.Linq;
using MongoDB.Bson;

namespace Eze.Repositories
{
    public class MongoDbRepository : IEzeRepository
    {
        private const string databaseName = "eze";
        private const string itemCollectionName = "items";
        private const string requestCollectionName = "request";
        private const string accountCollectionName = "accounts";

        private readonly IMongoCollection<Item> itemsCollection;
        private readonly IMongoCollection<Request> requestsCollection;
        private readonly IMongoCollection<Account> accountsCollection;

        private readonly FilterDefinitionBuilder<Item> itemFilterBuilder = Builders<Item>.Filter;
        private readonly FilterDefinitionBuilder<Request> requestFilterBuilder = Builders<Request>.Filter;
        private readonly FilterDefinitionBuilder<Account> accountFilterBuilder = Builders<Account>.Filter;


        public MongoDbRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            itemsCollection = database.GetCollection<Item>(itemCollectionName);
            accountsCollection = database.GetCollection<Account>(accountCollectionName);
            requestsCollection = database.GetCollection<Request>(requestCollectionName);
        }

        public void CreateAccount(Account account)
        {
            accountsCollection.InsertOne(account);
        }
  
        public void CreateItem(Item item)
        {
            itemsCollection.InsertOne(item);
        }

        public void CreateRequest(Request request)
        {
            requestsCollection.InsertOne(request);
        }

        public void DeleteAccount(Guid id)
        {
            var filter = accountFilterBuilder.Eq(account => account.Id, id);
            accountsCollection.DeleteOne(filter);
        }

        public void DeleteItem(Guid id)
        {
            var filter = itemFilterBuilder.Eq(item => item.Id, id);
            itemsCollection.DeleteOne(filter);
        }

        public void DeleteRequest(Guid id)
        {
            var filter = requestFilterBuilder.Eq(request => request.Id, id);
            requestsCollection.DeleteOne(filter);
        }

        public Account GetAccount(Guid id)
        {
            var filter = accountFilterBuilder.Eq(account => account.Id, id);
            return accountsCollection.Find(filter).SingleOrDefault();
        }

        public ICollection<Account> GetAccounts()
        {
            return accountsCollection.Find(new BsonDocument()).ToList();
        }

        public Item GetItem(Guid id)
        {
            var filter = itemFilterBuilder.Eq(item => item.Id, id);
            return itemsCollection.Find(filter).SingleOrDefault();
        }

        public ICollection<Item> GetItems()
        {
            return itemsCollection.Find(new BsonDocument()).ToList();
        }

        public Request GetRequest(Guid id)
        {
            var filter = requestFilterBuilder.Eq(request => request.Id, id);
            return requestsCollection.Find(filter).SingleOrDefault();
        }

        public ICollection<Request> GetRequests()
        {
            return requestsCollection.Find(new BsonDocument()).ToList();
        }

        public void UpdateAccount(Account account)
        {
            var filter = accountFilterBuilder.Eq(existingAccount => existingAccount.Id, account.Id);
            accountsCollection.ReplaceOne(filter, account);
        }

        public void UpdateItem(Item item)
        {
            var filter = itemFilterBuilder.Eq(existingItem => existingItem.Id, item.Id);
            itemsCollection.ReplaceOne(filter, item);
        }

        public void UpdateRequest(Request request)
        {
            var filter = requestFilterBuilder.Eq(exisitingRequest => exisitingRequest.Id, request.Id);
            requestsCollection.ReplaceOne(filter, request);
        }
    }
}