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
    [Route("api/item")]
    [Authorize(Roles = "Admin")]
    public class ItemController : ControllerBase
    {
        private readonly IEzeRepository repo;
        private readonly ILogger<ItemController> logger;

        public ItemController(IEzeRepository repo, ILogger<ItemController> logger)
        {
            this.repo = repo;
            this.logger = logger;
        }

        [HttpGet]
        [Authorize(Roles ="Admin,Professor,Student")]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetItemsAsync()
        {
            var items = (await repo.GetItemsAsync()).Select(item => item.AsItemDto());

            if(items == null)
            {
                return NotFound();
            }

            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved {items.Count()} items");

            return Ok(items);
        }

        [HttpGet("{id}")]
        [ActionName("GetItemAsync")]
        public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id)
        {
            var item = await repo.GetItemAsync(id);

            if(item == null)
            {
                return NotFound();
            }

            return Ok(item.AsItemDto());
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateItemDto itemDto)
        {
            var item = new Item()
            {
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Description = itemDto.Description,
                Condition = itemDto.Condition,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await repo.CreateItemAsync(item);

            return CreatedAtAction(nameof(GetItemAsync), new{id = item.Id}, item.AsItemDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateItemAsync(Guid id, UpdateItemDto itemDto)
        {
            var item = await repo.GetItemAsync(id);

            if (item == null)
            {
                return NotFound();   
            }

            item.Name = itemDto.Name;
            item.Description = itemDto.Description;
            item.Condition = itemDto.Condition;

            await repo.UpdateItemAsync(item);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItemAsync(Guid id)
        {
            var item = await repo.GetItemAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            await repo.DeleteItemAsync(id);

            return NoContent();
        }
    }
}