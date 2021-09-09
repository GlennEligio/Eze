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
        public async Task<ActionResult<ICollection<Request>>> GetRequestsAsync()
        {
            var requests = await repo.GetRequestsAsync();

            return Ok(requests);  
        }

        //GET: request/account-request/{id}
        [HttpGet("/account-request/{id}")]
        public async Task<ActionResult<ICollection<RequestDto>>> GetRequestsByProfIdAsync(Guid id)
        {
            var account = await repo.GetAccountAsync(id);

            if(account == null)
            {
                return NotFound();
            }

            var requests = (await repo.GetRequestsAsync())
                                .Where(request => request.ProfessorId == id);

            var requestDtos = new List<RequestDto>();  

            foreach (var request in requests)
            {
                var items = new List<Item>();

                foreach (var itemId in request.ItemIds)
                {
                    items.Add(await repo.GetItemAsync(itemId));
                }

                var professorName = (await repo.GetAccountAsync(request.ProfessorId)).Name;

                requestDtos.Add(new RequestDto(request.Id, items, request.CreatedDate, request.StudentName, professorName, request.Code, request.Status));
            }
            
            return Ok(requestDtos);
        }

        //GET: Request/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Request>> GetRequestAsync(Guid id)
        {
            var request = await repo.GetRequestAsync(id);
            
            if(request == null)
            {
                return NotFound();
            }

            return Ok(request);
        }

        //POST: Request/
        [HttpPost]
        public async Task<ActionResult> CreateRequestAsync(CreateRequestDto requestDto)
        {
            var account = await repo.GetAccountAsync(requestDto.ProfessorId);
            var items = (await repo.GetItemsAsync()).Where(item => requestDto.ItemIds.Contains(item.Id)).ToList();
            
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

            await repo.CreateRequestAsync(request);

            return CreatedAtAction(nameof(GetRequestAsync), new {id = request.Id}, RequestAsDto(request, account, items));
        }

        //PUT: Request/{id}
        [HttpPut("{id}")]
        public async Task UpdateRequestAsync(Guid id, UpdateRequestDto requestDto)
        {
            var request = await repo.GetRequestAsync(id);

            request.Status = requestDto.Status;

            await repo.UpdateRequestAsync(request);
        }

        [HttpDelete("{id}")]
        public async Task DeleteRequestAsync(Guid id)
        {
            await repo.DeleteRequestAsync(id);
        }

        private static RequestDto RequestAsDto(Request request, Account account, IEnumerable<Item> items)
        {
            return new RequestDto(request.Id, items, request.CreatedDate, request.StudentName, account.Name, request.Code, request.Status);
        }
    }
}
