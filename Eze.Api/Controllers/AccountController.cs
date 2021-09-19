using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Eze.Api.Dtos;
using Eze.Api.Entities;
using Eze.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Eze.Api.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly IEzeRepository repo;
        private readonly ILogger<AccountController> logger;
        private readonly JWTSettings jwtSettings;

        public AccountController(IEzeRepository repo, ILogger<AccountController> logger, IOptions<JWTSettings> jwtSettings)
        {
            this.repo = repo;
            this.logger = logger;
            this.jwtSettings = jwtSettings.Value;
        }

        [HttpGet]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<IEnumerable<AccountDto>>> GetAccountsAsync()
        {
            var accounts = (await repo.GetAccountsAsync())
                            .Select(account => account.AsAccountDto());

            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved {accounts.Count()} items");

            return Ok(accounts);
        }

        [HttpGet("{id}")]
        [ActionName("GetAccountAsync")]
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
                CreatedDate = DateTimeOffset.UtcNow,
                Role = accountDto.Role
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
            account.Role = accountDto.Role;

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

        [HttpGet("Login")]
        public async Task<ActionResult<AccountWithTokenDto>> LoginAsync([FromBody]LoginAccountDto accountDto)
        {
            var account = (await repo.GetAccountsAsync())
                            .Where(existingAccount => existingAccount.Username == accountDto.Username
                                    && existingAccount.Password == accountDto.Password)
                            .FirstOrDefault();

            if(account == null)
            {
                return NotFound();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, account.Id.ToString()),  
                    new Claim(ClaimTypes.Role, account.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key)
                    , SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accountWithToken = new AccountWithTokenDto(account.Name, 
                                                            account.Username, 
                                                            account.Password, 
                                                            tokenHandler.WriteToken(token));

            return Ok(accountWithToken);
        }

        private RefreshToken GenerateRefreshToken()
        {
            RefreshToken refreshToken = new();
            var randomNumber = new byte[32];

            using(var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                refreshToken.Token = Convert.ToBase64String(randomNumber);
            }

            refreshToken.ExpiryDate = DateTimeOffset.UtcNow.AddMonths(6);

            return refreshToken;
        }

    }
}