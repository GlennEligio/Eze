using System;
using System.Collections.Generic;
using Eze.Api.Entities;

namespace Eze.Api.Repositories
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