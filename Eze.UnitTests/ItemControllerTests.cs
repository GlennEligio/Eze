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
    public class ItemControllerTests
    {
        private readonly Mock<IEzeRepository> repositoryStub = new();
        private readonly Mock<ILogger<ItemController>> loggerStub = new();

        [Fact]
        public async Task GetItemAsync_WithUnexistingItem_ReturnsNotFound()
        {
            //Arrange
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                            .ReturnsAsync((Item)null);

            var controller = new ItemController(repositoryStub.Object, loggerStub.Object);

            //Act
            var result = await controller.GetItemAsync(Guid.NewGuid());
            
            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetItemAsync_WithExistingItem_ReturnsExpectedItem()
        {
            //Arrange
            var expectedItem = CreateRandomItem();

            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                            .ReturnsAsync(expectedItem);

            var controller = new ItemController(repo: repositoryStub.Object, logger: loggerStub.Object);

            //Act
            var result = await controller.GetItemAsync(Guid.NewGuid());
            
            //Assert
            var itemDtoResult = (result.Result as OkObjectResult).Value as ItemDto;
            itemDtoResult.Should().NotBeNull();
            itemDtoResult.Should().BeEquivalentTo(expectedItem, options => options.ComparingByMembers<Item>());

            result.Result.Should().BeOfType<OkObjectResult>();
        }
        
        [Fact]
        public async Task GetItemsAsync_WithExistingItems_ReturnAllItems()
        {
            //Arrange
            var expectedItems = new []{CreateRandomItem(), CreateRandomItem(), CreateRandomItem()};
            repositoryStub.Setup(repo => repo.GetItemsAsync()).ReturnsAsync(expectedItems);

            var controller = new ItemController(repositoryStub.Object, loggerStub.Object);

            //Act
            var result = await controller.GetItemsAsync();

            //Assert
            var actualItems = (result.Result as OkObjectResult).Value as IEnumerable<ItemDto>;
            actualItems.Should().BeEquivalentTo(expectedItems);
        }

        [Fact]
        public async Task CreateItemAsync_WithItemToCreated_ReturnsCreatedItem()
        {
            //Arrange
            var itemToCreate = new CreateItemDto(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var controller = new ItemController(repositoryStub.Object, loggerStub.Object);

            //Act
            var result = await controller.CreateItemAsync(itemToCreate);

            //Assert
            var createdItem = (result.Result as CreatedAtActionResult).Value as ItemDto;
            createdItem.Should().BeEquivalentTo(itemToCreate, 
                        options => options.ComparingByMembers<Item>().ExcludingMissingMembers());
            createdItem.Id.Should().NotBeEmpty();
            createdItem.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task UpdateItemAsync_WithExistingItem_ReturnsNoContent()
        {
            //Arrange
            var exisingItem = CreateRandomItem();
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>())).ReturnsAsync(exisingItem);

            var itemId = exisingItem.Id;
            var itemToUpdate = new UpdateItemDto(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var controller = new ItemController(repositoryStub.Object, loggerStub.Object);

            //Act
            var result = await controller.UpdateItemAsync(itemId, itemToUpdate);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteItemAsync_WithExistingItem_ReturnsNoContent()
        {
            //Arrange
            var exisingItem = CreateRandomItem();
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>())).ReturnsAsync(exisingItem);

            var itemId = exisingItem.Id;

            var controller = new ItemController(repositoryStub.Object, loggerStub.Object);

            //Act
            var result = await controller.DeleteItemAsync(itemId);

            //Assert
            result.Should().BeOfType<NoContentResult>();
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
    }

    
}
