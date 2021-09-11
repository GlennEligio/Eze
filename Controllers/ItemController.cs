using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eze.Dtos;
using Eze.Entities;
using Eze.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Eze.Controllers
{
    [ApiController]
    [Route("api/item")]
    public class ItemController : ControllerBase
    {
        private readonly IEzeRepository repo;

        public ItemController(IEzeRepository repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<Item>>> GetItemsAsync()
        {
            var items = await repo.GetItemsAsync();

            if(items == null)
            {
                return NotFound();
            }

            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItemAsync(Guid id)
        {
            var item = await repo.GetItemAsync(id);

            if(item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult> CreateItemAsync(CreateItemDto itemDto)
        {
            var item = new Item()
            {
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Description = itemDto.Description,
                Condition = itemDto.Condition
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