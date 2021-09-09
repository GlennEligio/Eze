using System;
using System.Collections.Generic;
using Eze.Entities;

namespace Eze.Repositories
{
    public interface IRequestRepository
    {
        //Request CRUD
        ICollection<Request> GetRequests();
        Request GetRequest(Guid id);
        void CreateRequest(Request request);
        void UpdateRequest(Request request);
        void DeleteRequest(Guid id);
    }
}