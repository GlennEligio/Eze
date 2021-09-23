using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eze.Api.Dtos;
using Eze.Api.Entities;
using Eze.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Eze.Api.Controllers
{
    [ApiController]
    [Route("api/request")]
    public class RequestController : ControllerBase
    {
        private readonly IEzeRepository repo;
        private readonly ILogger<RequestController> logger;

        public RequestController(IEzeRepository repo, ILogger<RequestController> logger)
        {
            this.repo = repo;
            this.logger = logger;
        }

        //GET: Request/
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ICollection<RequestDto>>> GetRequestsAsync()
        {
            var requests = (await repo.GetRequestsAsync()).Select(request => request.AsRequestDto());

            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved {requests.Count()} items");

            return Ok(requests);  
        }

        //GET: Request/{id}
        [HttpGet("{id}")]
        [ActionName("GetRequestAsync")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RequestDto>> GetRequestAsync(Guid id)
        {
            var request = await repo.GetRequestAsync(id);
            
            if(request == null)
            {
                return NotFound();
            }

            return Ok(request.AsRequestDto());
        }

        //POST: Request/
        [HttpPost]
        [Authorize(Roles = "Admin,Student")]
        public async Task<ActionResult<RequestDto>> CreateRequestAsync(CreateRequestDto requestDto)
        {
            var request = new Request
            {
                Id = Guid.NewGuid(),
                ItemIds = requestDto.ItemIds as List<Guid>,
                StudentName = requestDto.StudentName,
                ProfessorId = requestDto.ProfessorId,
                Code = Extensions.GenerateRandomAlphanumericString(),
                Status = "Pending",
                CreatedDate = DateTimeOffset.UtcNow,
                Description = requestDto.Description
            };

            await repo.CreateRequestAsync(request);

            return CreatedAtAction(nameof(GetRequestAsync), new {id = request.Id}, request.AsRequestDto());
        }

        //PUT: Request/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Professor,Student")]
        public async Task<ActionResult> UpdateRequestAsync(Guid id, UpdateRequestDto requestDto)
        {
            var request = await repo.GetRequestAsync(id);

            if(request == null)
            {
                return NotFound();
            }

            request.Status = requestDto.Status;

            await repo.UpdateRequestAsync(request);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteRequestAsync(Guid id)
        {
            var request = await repo.GetRequestAsync(id);

            if(request == null)
            {
                return NotFound();
            }

            await repo.DeleteRequestAsync(request.Id);

            return NoContent();
        }
    }
}
