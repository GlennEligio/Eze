using System;
using System.Collections.Generic;
using Eze.Entities;

namespace Eze.Repositories
{
    public interface IItemRepository
    {
        //Item CRUD
        ICollection<Item> GetItems();
        Item GetItem(Guid id);
        void CreateItem(Item item);
        void UpdateItem(Item item);
        void DeleteItem(Guid id);
    }
}