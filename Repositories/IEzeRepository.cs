using System;
using System.Collections.Generic;
using Eze.Entities;

namespace Eze.Repositories
{
    public interface IEzeRepository
    {
        //Account CRUD
        ICollection<Account> GetAccounts();
        Account GetAccount(Guid id);
        void CreateAccount(Account account);
        void UpdateAccount(Account account);
        void DeleteAccount(Guid id);

        //Item CRUD
        ICollection<Item> GetItems();
        Item GetItem(Guid id);
        void CreateItem(Item item);
        void UpdateItem(Item item);
        void DeleteItem(Guid id);

        //Request CRUD
        ICollection<Request> GetRequests();
        Request GetRequest(Guid id);
        void CreateRequest(Request request);
        void UpdateRequest(Request request);
        void DeleteRequest(Guid id);
    }
}