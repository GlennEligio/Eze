using System;
using System.Collections.Generic;
using MongoDB.Driver;
using System.Data;
using System.Linq;
using MongoDB.Bson;
using System.Threading.Tasks;
using Eze.Api.Entities;

namespace Eze.Api.Repositories
{
    public class MongoDbRepository : IEzeRepository
    {
        private const string databaseName = "eze";
        private const string itemCollectionName = "items";
        private const string requestCollectionName = "request";
        private const string accountCollectionName = "accounts";
        private const string refreshTokenCollectionName = "refreshToken";

        private readonly IMongoCollection<Item> itemsCollection;
        private readonly IMongoCollection<Request> requestsCollection;
        private readonly IMongoCollection<Account> accountsCollection;
        private readonly IMongoCollection<RefreshToken> refreshTokensCollection;

        private readonly FilterDefinitionBuilder<Item> itemFilterBuilder = Builders<Item>.Filter;
        private readonly FilterDefinitionBuilder<Request> requestFilterBuilder = Builders<Request>.Filter;
        private readonly FilterDefinitionBuilder<Account> accountFilterBuilder = Builders<Account>.Filter;
        private readonly FilterDefinitionBuilder<RefreshToken> refreshTokenFilterBuilder = Builders<RefreshToken>.Filter;


        public MongoDbRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            itemsCollection = database.GetCollection<Item>(itemCollectionName);
            accountsCollection = database.GetCollection<Account>(accountCollectionName);
            requestsCollection = database.GetCollection<Request>(requestCollectionName);
            refreshTokensCollection = database.GetCollection<RefreshToken>(refreshTokenCollectionName);
        }

        //Account CRUD
        public async Task CreateAccountAsync(Account account)
        {
            await accountsCollection.InsertOneAsync(account);
        }

         public async Task DeleteAccountAsync(Guid id)
        {
            var filter = accountFilterBuilder.Eq(account => account.Id, id);
            await accountsCollection.DeleteOneAsync(filter);
        }

        public async Task<Account> GetAccountAsync(Guid id)
        {
            var filter = accountFilterBuilder.Eq(account => account.Id, id);
            return await accountsCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<ICollection<Account>> GetAccountsAsync()
        {
            return await accountsCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task UpdateAccountAsync(Account account)
        {
            var filter = accountFilterBuilder.Eq(existingAccount => existingAccount.Id, account.Id);
            await accountsCollection.ReplaceOneAsync(filter, account);
        }
  
        //Item CRUD
        public async Task CreateItemAsync(Item item)
        {
            await itemsCollection.InsertOneAsync(item);
        }

        public async Task DeleteItemAsync(Guid id)
        {
            var filter = itemFilterBuilder.Eq(item => item.Id, id);
            await itemsCollection.DeleteOneAsync(filter);
        }

        public async Task<Item> GetItemAsync(Guid id)
        {
            var filter = itemFilterBuilder.Eq(item => item.Id, id);
            return await itemsCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<ICollection<Item>> GetItemsAsync()
        {
            return await itemsCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task UpdateItemAsync(Item item)
        {
            var filter = itemFilterBuilder.Eq(existingItem => existingItem.Id, item.Id);
            await itemsCollection.ReplaceOneAsync(filter, item);
        }

        //RefreshToken CRUD
        public async Task CreateRefreshTokenAsync(RefreshToken refreshToken)
        {
            await refreshTokensCollection.InsertOneAsync(refreshToken);
        }

        public async Task DeleteRefreshTokenAsync(Guid id)
        {
            var filter = refreshTokenFilterBuilder.Eq(refreshToken => refreshToken.Id, id);
            await refreshTokensCollection.DeleteOneAsync(filter);
        }

        public async Task<ICollection<RefreshToken>> GetRefreshTokensAsync()
        {
            return await refreshTokensCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(Guid id)
        {
            var filter = refreshTokenFilterBuilder.Eq(refreshToken => refreshToken.Id, id);
            return (await refreshTokensCollection.FindAsync(filter)).SingleOrDefault();
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            var filter = refreshTokenFilterBuilder.Eq(existingRefreshToken => existingRefreshToken.Id, refreshToken.Id);
            await refreshTokensCollection.ReplaceOneAsync(filter, refreshToken);
        }

        //Request CRUD
        public async Task CreateRequestAsync(Request request)
        {
            await requestsCollection.InsertOneAsync(request);
        }

        public async Task DeleteRequestAsync(Guid id)
        {
            var filter = requestFilterBuilder.Eq(request => request.Id, id);
            await requestsCollection.DeleteOneAsync(filter);
        }

        public async Task<Request> GetRequestAsync(Guid id)
        {
            var filter = requestFilterBuilder.Eq(request => request.Id, id);
            return await requestsCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<ICollection<Request>> GetRequestsAsync()
        {
            return await requestsCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task UpdateRequestAsync(Request request)
        {
            var filter = requestFilterBuilder.Eq(exisitingRequest => exisitingRequest.Id, request.Id);
            await requestsCollection.ReplaceOneAsync(filter, request);
        }
    }
}