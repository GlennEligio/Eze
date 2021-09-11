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
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly IEzeRepository repo;
        private readonly ILogger logger;

        public AccountController(IEzeRepository repo, ILogger logger)
        {
            this.repo = repo;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountDto>>> GetAccountsAsync()
        {
            var accounts = (await repo.GetAccountsAsync())
                            .Select(account => account.AsAccountDto());

            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved {accounts.Count()} items");

            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AccountDto>> GetAccountAsync(Guid id)
        {
            var account = await repo.GetAccountAsync(id);

            if(account == null)
            {
                return NotFound();
            }


            return Ok(account.AsAccountDto());
        }

        [HttpPost]
        public async Task<ActionResult<AccountDto>> CreateAccountAsync(CreateAccountDto accountDto)
        {
            var account = new Account()
            {
                Id = Guid.NewGuid(),
                Name = accountDto.Name,
                Username = accountDto.Username,
                Password = accountDto.Password,
                CreatedDate = DateTimeOffset.UtcNow
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