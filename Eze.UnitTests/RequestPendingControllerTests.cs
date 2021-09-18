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
using Moq;
using Xunit;

namespace Eze.UnitTests
{
    public class RequestPendingControllerTests
    {
        private readonly Mock<IEzeRepository> repositoryStub = new ();
        private readonly Mock<ILogger<RequestPendingController>> loggerStub = new();
        private readonly Random rand = new();

        [Fact]
        public async Task RequestPendingAsync_WithExistingRequests_ReturnsPendingRequest()
        {
            //Arrange
            //GetItemAsync
            var existingItem = CreateRandomItem();
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>())).ReturnsAsync(existingItem);

            //GetAccountAsync
            var existingAccount = CreateRandomAccount();
            repositoryStub.Setup(repo => repo.GetAccountAsync(It.IsAny<Guid>())).ReturnsAsync(existingAccount);

            //GetRequestsAsync
            var existingRequests = new[]{CreateRandomRequest(), CreateRandomRequest(), CreateRandomRequest(), CreatePendingRequest()};
            repositoryStub.Setup(repo => repo.GetRequestsAsync()).ReturnsAsync(existingRequests);

            var controller = new RequestPendingController(repositoryStub.Object, loggerStub.Object);

            //Act
            var result = await controller.RequestPendingAsync(existingAccount.Id);

            //Assert
            var pendingRequest = (result.Result as OkObjectResult).Value as List<RequestPendingDto>;
            pendingRequest.Should().NotBeNull();
            if(pendingRequest.Count < 0)
            {
                pendingRequest.Should().OnlyContain(request => request.Status == false);
            }
            pendingRequest.Should().OnlyHaveUniqueItems(request => request.Id);
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

        private Item CreateRandomItem()
        {
            return new ()
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Condition = Guid.NewGuid().ToString(),
                CreatedDate = DateTimeOffset.UtcNow
            };
        }

        private Request CreateRandomRequest()
        {
            return new Request
            {
                Id = Guid.NewGuid(),
                ItemIds = new List<Guid>{Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()},
                CreatedDate = DateTimeOffset.UtcNow,
                StudentName = Guid.NewGuid().ToString(),
                ProfessorId = Guid.NewGuid(),
                Code = Guid.NewGuid().ToString(),
                Status = rand.Next(3)%2 == 1 ? false : true,
                Description = Guid.NewGuid().ToString()
            };
        }

        private Request CreatePendingRequest()
        {
            return new Request
            {
                Id = Guid.NewGuid(),
                ItemIds = new List<Guid>{Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()},
                CreatedDate = DateTimeOffset.UtcNow,
                StudentName = Guid.NewGuid().ToString(),
                ProfessorId = Guid.NewGuid(),
                Code = Guid.NewGuid().ToString(),
                Status = false,
                Description = Guid.NewGuid().ToString()
            };
        }
    }
}