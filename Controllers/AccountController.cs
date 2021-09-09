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
    [Route("account")]
    public class AccountController : ControllerBase
    {
        private readonly IEzeRepository repo;

        public AccountController(IEzeRepository repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<Account>>> GetAccountsAsync()
        {
            var accounts = await repo.GetAccountsAsync();
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public ActionResult<Account> GetAccountAsync(Guid id)
        {
            var account = repo.GetAccountAsync(id);

            if(account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAccountAsync(CreateAccountDto accountDto)
        {
            var account = new Account()
            {
                Id = Guid.NewGuid(),
                Name = accountDto.Name,
                Username = accountDto.Username,
                Password = accountDto.Password
            };

            await repo.CreateAccountAsync(account);

            return CreatedAtAction(nameof(GetAccountAsync), new {id = account.Id}, account.AsAccountDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAccountAsync(Guid id, UpdateAccountDto accountDto)
        {
            var account = await repo.GetAccountAsync(id);

            if(account == null)
            {
                return NotFound();
            }

            account.Name = accountDto.Name;
            account.Password = accountDto.Password;
            account.Username = accountDto.Username;

            await repo.UpdateAccountAsync(account);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAccountAsync(Guid id)
        {
            var account = await repo.GetAccountAsync(id);

            if(account == null)
            {
                return NotFound();
            }

            await repo.DeleteAccountAsync(id);

            return NoContent();
        }

    }
}