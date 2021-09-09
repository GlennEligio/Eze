using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eze.Dtos;
using Eze.Entities;
using Eze.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Eze.Controllers
{
    [ApiController]
    [Route("request")]
    public class RequestController : ControllerBase
    {
        private readonly IEzeRepository repo;

        public RequestController(IEzeRepository repo)
        {
            this.repo = repo;
        }

        //GET: Request/
        [HttpGet]
        public ActionResult<ICollection<Request>> GetRequests()
        {
            var requests = repo.GetRequests();

            return Ok(requests);  
        }

        //GET: request/account-request/{id}
        [HttpGet("/account-request/{id}")]
        public ActionResult<ICollection<RequestDto>> GetRequestsByProfId(Guid id)
        {
            var account = repo.GetAccount(id);

            if(account == null)
            {
                return NotFound();
            }

            var requests = repo.GetRequests()
                                .Where(request => request.ProfessorId == id);

            var requestDtos = new List<RequestDto>();  

            foreach (var request in requests)
            {
                var items = new List<Item>();

                foreach (var itemId in request.ItemIds)
                {
                    items.Add(repo.GetItem(itemId));
                }

                var professorName = repo.GetAccount(request.ProfessorId).Name;

                requestDtos.Add(new RequestDto(request.Id, items, request.CreatedDate, request.StudentName, professorName, request.Code, request.Status));
            }
            
            return Ok(requestDtos);
        }

        //GET: Request/{id}
        [HttpGet("{id}")]
        public ActionResult<Request> GetRequest(Guid id)
        {
            var request = repo.GetRequest(id);
            
            if(request == null)
            {
                return NotFound();
            }

            return Ok(request);
        }

        //POST: Request/
        [HttpPost]
        public ActionResult CreateRequest(CreateRequestDto requestDto)
        {
            var account = repo.GetAccount(requestDto.ProfessorId);
            var items = repo.GetItems().Where(item => requestDto.ItemIds.Contains(item.Id)).ToList();
            
            var request = new Request
            {
                Id = Guid.NewGuid(),
                ItemIds = requestDto.ItemIds,
                StudentName = requestDto.StudentName,
                ProfessorId = requestDto.ProfessorId,
                Code = Extensions.GenerateRandomAlphanumericString(),
                Status = false,
                CreatedDate = DateTimeOffset.UtcNow,
                Description = requestDto.Description
            };

            repo.CreateRequest(request);

            return CreatedAtAction(nameof(GetRequest), new {id = request.Id}, RequestAsDto(request, account, items));
        }

        //PUT: Request/{id}
        [HttpPut("{id}")]
        public void UpdateRequest(Guid id, UpdateRequestDto requestDto)
        {
            var request = repo.GetRequest(id);

            request.Status = requestDto.Status;

            repo.UpdateRequest(request);
        }

        [HttpDelete("{id}")]
        public void DeleteRequest(Guid id)
        {
            repo.DeleteRequest(id);
        }

        private static RequestDto RequestAsDto(Request request, Account account, IEnumerable<Item> items)
        {
            return new RequestDto(request.Id, items, request.CreatedDate, request.StudentName, account.Name, request.Code, request.Status);
        }
    }
}
