using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eze.Api.Controllers;
using Eze.Api.Dtos;
using Eze.Api.Entities;
using Eze.Api.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Eze.UnitTests
{
    public class AccountControllerTests
    {
        private readonly Mock<IEzeRepository> repositoryStub = new();
        private readonly Mock<ILogger<AccountController>> loggerStub = new();
        private readonly Mock<IOptions<JWTSettings>> jwtSettingsStub = new();

        [Fact]
        public async Task GetAccountAsync_WithUnexistingAccount_ReturnsNotFound()
        {
            //Arrange
            repositoryStub.Setup(repo => repo.GetAccountAsync(It.IsAny<Guid>()))
                            .ReturnsAsync((Account) null);

            var controller = new AccountController(repositoryStub.Object, loggerStub.Object, jwtSettingsStub.Object);

            //Act
            var result = await controller.GetAccountAsync(Guid.NewGuid());

            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetAccountAsync_WithExistingAccount_ReturnsExpectedAccount()
        {
            //Arrange
            var expectedAccount = CreateRandomAccount();
            repositoryStub.Setup(repo => repo.GetAccountAsync(It.IsAny<Guid>()))
                            .ReturnsAsync(expectedAccount);

            var controller = new AccountController(repositoryStub.Object, loggerStub.Object, jwtSettingsStub.Object);

            //Act
            var result = await controller.GetAccountAsync(Guid.NewGuid());

            //Assert
            var accountDtoResult = (result.Result as OkObjectResult).Value as AccountDto;
            accountDtoResult.Should().NotBeNull();
            accountDtoResult.Should().BeEquivalentTo(expectedAccount,
                                                    options => options.ComparingByMembers<Account>().ExcludingMissingMembers());
        }

        [Fact]
        public async Task GetAccountsAsync_WithExistingAccounts_ReturnsAllAccounts()
        {
            //Arrange
            var expectedAccounts = new[]{CreateRandomAccount(), CreateRandomAccount(), CreateRandomAccount()};

            repositoryStub.Setup(repo => repo.GetAccountsAsync())
                            .ReturnsAsync(expectedAccounts);

            var controller = new AccountController(repositoryStub.Object, loggerStub.Object, jwtSettingsStub.Object);

            //Act
            var result = await controller.GetAccountsAsync();

            //Assert
            var accountDtosResult = (result.Result as OkObjectResult).Value as IEnumerable<AccountDto>;
            accountDtosResult.Should().NotBeNull();
            accountDtosResult.Should().BeEquivalentTo(expectedAccounts
                                                , options => options.ComparingByMembers<Account>().ExcludingMissingMembers());
        }

        [Fact]
        public async Task CreateAccountAsync_WithAccountToCreate_ReturnsCreatedItem()
        {
            //Arrange
            var accountToCreate = new CreateAccountDto(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var controller = new AccountController(repositoryStub.Object, loggerStub.Object, jwtSettingsStub.Object);

            //Act
            var result = await controller.CreateAccountAsync(accountToCreate);

            //Assert
            var accountDtoResult = (result.Result as CreatedAtActionResult).Value as AccountDto;
            accountDtoResult.Should().BeEquivalentTo(accountToCreate,
                                                    options => options.ComparingByMembers<Account>().ExcludingMissingMembers());
            accountDtoResult.Id.Should().NotBeEmpty();
            accountDtoResult.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task UpdateAccountAsync_WithExistingAccount_ReturnsNoContent()
        {
            //Arrange
            var existingAccount = CreateRandomAccount();
            repositoryStub.Setup(repo => repo.GetAccountAsync(It.IsAny<Guid>())).ReturnsAsync(existingAccount);

            var accountId = existingAccount.Id;
            var accountToUpdate = new UpdateAccountDto(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var controller = new AccountController(repositoryStub.Object, loggerStub.Object, jwtSettingsStub.Object);

            //Act
            var result = await controller.UpdateAccountAsync(accountId, accountToUpdate);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }


        [Fact]
        public async Task DeleteAccountAsync_WithExistingAccount_ReturnsNoContent()
        {
            //Arrange
            var existingAccount = CreateRandomAccount();
            repositoryStub.Setup(repo => repo.GetAccountAsync(It.IsAny<Guid>())).ReturnsAsync(existingAccount);

            var controller = new AccountController(repositoryStub.Object, loggerStub.Object, jwtSettingsStub.Object);

            //Act
            var result = await controller.DeleteAccountAsync(existingAccount.Id);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        private Account CreateRandomAccount()
        {
            return new Account
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Username = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString(),
                CreatedDate = DateTimeOffset.UtcNow
            };
        }
    }
}