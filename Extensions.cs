using System;
using System.Linq;
using Eze.Dtos;
using Eze.Entities;

namespace Eze
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
            return new AccountDto(account.Id, account.Name, account.Username, account.Password);
        }

        public static ItemDto AsItemDto(this Item item)
        {
            return new ItemDto(item.Id, item.Name, item.Description, item.Condition);
        }
    }
}