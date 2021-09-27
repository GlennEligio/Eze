using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Eze.Api.Dtos;
using Eze.Api.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Eze.Api
{
    public static class Extensions
    {
        public static string GenerateRandomAlphanumericString(int length = 6)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
 
            var random       = new Random();
            var randomString = new string(Enumerable.Repeat(chars, length)
                                            .Select(s => s[random.Next(s.Length)]).ToArray());
            return randomString;
        }

        public static AccountDto AsAccountDto(this Account account)
        {
            return new AccountDto(account.Id, account.Name, account.Username, account.Password, account.CreatedDate, account.Role);
        }

        public static ItemDto AsItemDto(this Item item)
        {
            return new ItemDto(item.Id, item.Name, item.Description, item.Condition, item.CreatedDate);
        }

        public static RequestDto AsRequestDto(this Request request)
        {
            return new RequestDto(request.Id, request.ItemIds, request.CreatedDate, request.StudentName, request.ProfessorId, request.Code, request.Status, request.Description);
        }

        public static AccountDisplayDto AsAccountDisplayDto(this Account account)
        {
            return new AccountDisplayDto(account.Id, account.Name, account.Role);
        }

        public static RefreshToken GenerateRefreshToken()
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

        public static string GenerateAccessToken(Guid accountId, string role, string secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, accountId.ToString()),  
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key)
                    , SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}