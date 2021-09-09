using System;
using System.Collections.Generic;
using Eze.Dtos;
using Eze.Entities;
using Eze.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Eze.Controllers
{
    [ApiController]
    [Route("item")]
    public class ItemController : ControllerBase
    {
        private readonly IEzeRepository repo;

        public ItemController(IEzeRepository repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        public ActionResult<ICollection<Item>> GetItems()
        {
            var items = repo.GetItems();

            if(items == null)
            {
                return NotFound();
            }

            return Ok(items);
        }

        [HttpGet("{id}")]
        public ActionResult<Item> GetItem(Guid id)
        {
            var item = repo.GetItem(id);

            if(item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpPost]
        public ActionResult CreateItem(CreateItemDto itemDto)
        {
            var item = new Item()
            {
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Description = itemDto.Description,
                Condition = itemDto.Condition
            };

            repo.CreateItem(item);

            return CreatedAtAction(nameof(GetItem), new{id = item.Id}, item.AsItemDto());
        }

        [HttpPut("{id}")]
        public ActionResult UpdateItem(Guid id, UpdateItemDto itemDto)
        {
            var item = repo.GetItem(id);

            if (item == null)
            {
                return NotFound();   
            }

            item.Name = itemDto.Name;
            item.Description = itemDto.Description;
            item.Condition = itemDto.Condition;

            repo.UpdateItem(item);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteItem(Guid id)
        {
            var item = repo.GetItem(id);

            if (item == null)
            {
                return NotFound();
            }

            repo.DeleteItem(id);

            return NoContent();
        }
    }
}