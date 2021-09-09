using System;
using System.Collections.Generic;
using Eze.Entities;

namespace Eze.Repositories
{
    public interface IAccountRepository
    {
        //Account CRUD
        ICollection<Account> GetAccounts();
        Account GetAccount(Guid id);
        void CreateAccount(Account account);
        void UpdateAccount(Account account);
        void DeleteAccount(Guid id);
    }
}