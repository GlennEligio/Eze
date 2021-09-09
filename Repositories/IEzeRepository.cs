using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eze.Entities;

namespace Eze.Repositories
{
    public interface IEzeRepository
    {
        //Account CRUD
        Task<ICollection<Account>> GetAccountsAsync();
        Task<Account> GetAccountAsync(Guid id);
        Task CreateAccountAsync(Account account);
        Task UpdateAccountAsync(Account account);
        Task DeleteAccountAsync(Guid id);

        //Item CRUD
        Task<ICollection<Item>> GetItemsAsync();
        Task<Item> GetItemAsync(Guid id);
        Task CreateItemAsync(Item item);
        Task UpdateItemAsync(Item item);
        Task DeleteItemAsync(Guid id);

        //Request CRUD
        Task<ICollection<Request>> GetRequestsAsync();
        Task<Request> GetRequestAsync(Guid id);
        Task CreateRequestAsync(Request request);
        Task UpdateRequestAsync(Request request);
        Task DeleteRequestAsync(Guid id);
    }
}