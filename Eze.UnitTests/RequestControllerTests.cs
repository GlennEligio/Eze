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
    public class RequestControllerTests
    {
        private readonly Mock<IEzeRepository> repositoryStub = new ();
        private readonly Mock<ILogger> loggerStub = new ();
        private readonly Random rand = new();

        [Fact]
        public async Task GetRequestAsync_WithUnexistingRequest_ReturnsNotFound()
        {
            //Arrange
            repositoryStub.Setup(repo => repo.GetRequestAsync(It.IsAny<Guid>())).ReturnsAsync((Request)null);

            var controller = new RequestController(repositoryStub.Object, loggerStub.Object);

            //Act
            var result = await controller.GetRequestAsync(Guid.NewGuid());

            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetRequestAsync_WithExistingRequest_ReturnsExpectedItem()
        {
            //Arrange
            var existingRequest = CreateRandomRequest();
            repositoryStub.Setup(repo => repo.GetRequestAsync(It.IsAny<Guid>())).ReturnsAsync(existingRequest);

            var controller = new RequestController(repositoryStub.Object, loggerStub.Object);

            //Act
            var result = await controller.GetRequestAsync(Guid.NewGuid());

            //Assert
            var expectedResult = (result.Result as OkObjectResult).Value as RequestDto;
            expectedResult.Should().NotBeNull();
            expectedResult.Should().BeEquivalentTo(existingRequest, 
                                                    options => options.ComparingByMembers<Request>());
        }

        [Fact]
        public async Task GetRequestsAsync_WithExistingRequests_ReturnsAllRequests()
        {
            //Arrange
            var existingRequests = new []{CreateRandomRequest(), CreateRandomRequest(), CreateRandomRequest()};
            repositoryStub.Setup(repo => repo.GetRequestsAsync()).ReturnsAsync(existingRequests);

            var controller = new RequestController(repositoryStub.Object, loggerStub.Object);

            //Act
            var result = await controller.GetRequestsAsync();

            //Assert
            var expectedRequests = (result.Result as OkObjectResult).Value as IEnumerable<RequestDto>;
            expectedRequests.Should().BeEquivalentTo(existingRequests,
                                                    options => options.ComparingByMembers<Request>());
        }

        [Fact]
        public async Task CreateRequestAsync_WithRequestToCreate_ReturnsRequestCreated()
        {
            //Arrange
            var requestToCreate = new CreateRequestDto
            {
                ItemIds = new List<Guid>{Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()},
                StudentName = Guid.NewGuid().ToString(),
                ProfessorId = Guid.NewGuid(),
                Description = Guid.NewGuid().ToString(),
            };

            var controller = new RequestController(repositoryStub.Object, loggerStub.Object);

            //Act
            var result = await controller.CreateRequestAsync(requestToCreate);

            //Assert
            var createdRequest = (result.Result as CreatedAtActionResult).Value as RequestDto;
            createdRequest.Should().NotBeNull();
            createdRequest.Should().BeEquivalentTo(requestToCreate,
                                                    options => options.ComparingByMembers<Request>().ExcludingMissingMembers());
            createdRequest.Id.Should().NotBeEmpty();
            createdRequest.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task UpdateRequestAsync_WithExistingRequest_ReturnsNoContent()
        {
            //Arrange
            var existingRequest = CreateRandomRequest();
            repositoryStub.Setup(repo => repo.GetRequestAsync(It.IsAny<Guid>())).ReturnsAsync(existingRequest);
            
            var requestId = existingRequest.Id;
            var requestToUpdate = new UpdateRequestDto
            {
                Status = rand.Next(3)%2 == 1 ? false : true              
            };

            var controller = new RequestController(repositoryStub.Object, loggerStub.Object);

            //Act
            var result = await controller.UpdateRequestAsync(requestId, requestToUpdate);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteRequestAsync_WithExistingRequest_ReturnsNoContent()
        {
            //Arrange
            var existingRequest = CreateRandomRequest();
            repositoryStub.Setup(repo => repo.GetRequestAsync(It.IsAny<Guid>())).ReturnsAsync(existingRequest);
            
            var requestId = existingRequest.Id;

            var controller = new RequestController(repositoryStub.Object, loggerStub.Object);

            //Act
            var result = await controller.DeleteRequestAsync(requestId);

            //Assert
            result.Should().BeOfType<NoContentResult>();
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
    }
}