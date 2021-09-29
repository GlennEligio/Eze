using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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
    [Route("api/request-pending")]
    public class RequestPendingController : ControllerBase
    {        
        private readonly IEzeRepository repo;
        private readonly ILogger<RequestPendingController> logger;
        private readonly JWTSettings jwtSettings;

        public RequestPendingController(IEzeRepository repo, ILogger<RequestPendingController> logger, IOptions<JWTSettings> jwtSettings)
        {
            this.repo = repo;
            this.logger = logger;
            this.jwtSettings = jwtSettings.Value;
        }
        
        //GET: /request-pending/{id}
        [HttpGet("{profId}")]
        [Authorize(Roles = "Admin,Professor")]
        public async Task<ActionResult<IEnumerable<RequestPendingDto>>> RequestPendingAsync(string accessToken)
        {
            var account = await GetAccountFromAccessTokenAsync(accessToken);

            if(account == null)
            {
                return NotFound();
            }

            var requests = (await repo.GetRequestsAsync())
                                .Where(request => request.ProfessorId == account.Id && request.Status == "Pending");

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

        private async Task<Account> GetAccountFromAccessTokenAsync(string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

            SecurityToken securityToken;

            var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = false
                };

            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if(jwtSecurityToken is not null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                var accountId = principal.FindFirst(ClaimTypes.Name)?.Value;
                var account = await repo.GetAccountAsync(Guid.Parse(accountId));
                return account;
            }

            return null;
        }
    }
}