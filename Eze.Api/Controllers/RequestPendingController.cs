using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eze.Api.Dtos;
using Eze.Api.Entities;
using Eze.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Eze.Api.Controllers
{
    [ApiController]
    [Route("request-pending")]
    public class RequestPendingController : ControllerBase
    {        
        private readonly IEzeRepository repo;
        private readonly ILogger logger;

        public RequestPendingController(IEzeRepository repo, ILogger logger)
        {
            this.repo = repo;
            this.logger = logger;
        }
        
        //GET: /request-pending/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<RequestPendingDto>>> RequestPendingAsync(Guid id)
        {
            var account = await repo.GetAccountAsync(id);

            if(account == null)
            {
                return NotFound();
            }

            var requests = (await repo.GetRequestsAsync())
                                .Where(request => request.ProfessorId == id && request.Status == false);

            var requestDtos = new List<RequestPendingDto>();  

            foreach (var request in requests)
            {
                var items = new List<Item>();

                foreach (var itemId in request.ItemIds)
                {
                    items.Add(await repo.GetItemAsync(itemId));
                }

                var professorName = (await repo.GetAccountAsync(request.ProfessorId)).Name;

                requestDtos.Add(new RequestPendingDto(request.Id, items, request.CreatedDate, request.StudentName, professorName, request.Code, request.Status));
            }

            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved {requestDtos.Count} items");

            return Ok(requestDtos);
        }
    }
}