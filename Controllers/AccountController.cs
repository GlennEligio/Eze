using System;
using System.Collections.Generic;
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
        public ActionResult<ICollection<Account>> GetAccounts()
        {
            var accounts = repo.GetAccounts();
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public ActionResult<Account> GetAccount(Guid id)
        {
            var account = repo.GetAccount(id);

            if(account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        [HttpPost]
        public ActionResult CreateAccount(CreateAccountDto accountDto)
        {
            var account = new Account()
            {
                Id = Guid.NewGuid(),
                Name = accountDto.Name,
                Username = accountDto.Username,
                Password = accountDto.Password
            };

            repo.CreateAccount(account);

            return CreatedAtAction(nameof(GetAccount), new {id = account.Id}, account.AsAccountDto());
        }

        [HttpPut("{id}")]
        public ActionResult UpdateAccount(Guid id, UpdateAccountDto accountDto)
        {
            var account = repo.GetAccount(id);

            if(account == null)
            {
                return NotFound();
            }

            account.Name = accountDto.Name;
            account.Password = accountDto.Password;
            account.Username = accountDto.Username;

            repo.UpdateAccount(account);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteAccount(Guid id)
        {
            var account = repo.GetAccount(id);

            if(account == null)
            {
                return NotFound();
            }

            repo.DeleteAccount(id);

            return NoContent();
        }

    }
}