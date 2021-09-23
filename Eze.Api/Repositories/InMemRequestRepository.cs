using System;
using System.Collections.Generic;
using Eze.Api.Entities;
using System.Linq;

namespace Eze.Api.Repositories
{
    public class InMemRequestRepository : IRequestRepository
    {
        private readonly List<Request> requests = new List<Request>()
        {
            new Request {Id=Guid.NewGuid(), ItemIds= new List<Guid>(){Guid.NewGuid(), Guid.NewGuid()}, CreatedDate=DateTimeOffset.UtcNow, StudentName="Glenn", ProfessorId=Guid.NewGuid(), Status="Pending", Code="sdawadsd", Description="For Testing"},
            new Request {Id=Guid.NewGuid(), ItemIds= new List<Guid>(){Guid.NewGuid()}, CreatedDate=DateTimeOffset.UtcNow, StudentName="John", ProfessorId=Guid.NewGuid(), Status="Pending", Code="sdawwew", Description="For experiment"},
            new Request {Id=Guid.NewGuid(), ItemIds= new List<Guid>(){Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid()}, CreatedDate=DateTimeOffset.UtcNow, StudentName="Eligio", ProfessorId=Guid.NewGuid(), Status="Pending", Code="sdawwewe", Description="Test"}
        };

        public void CreateRequest(Request request)
        {
            requests.Add(request);
        }

        public void DeleteRequest(Guid id)
        {
            var index = requests.FindIndex(item => item.Id == id);
            requests.RemoveAt(index);
        }

        public Request GetRequest(Guid id)
        {
            var request = requests.Where(existingRequest => existingRequest.Id == id).FirstOrDefault();
            return request;
        }

        public ICollection<Request> GetRequests()
        {
            return requests;
        }

        public void UpdateRequest(Request request)
        {
            var index = requests.FindIndex(existingItem => existingItem.Id == request.Id);
            requests[index] = request;
        }
    }
}