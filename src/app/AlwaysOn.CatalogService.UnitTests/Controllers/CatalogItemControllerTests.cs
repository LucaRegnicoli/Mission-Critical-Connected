using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AlwaysOn.CatalogService.Controllers;
using AlwaysOn.Shared.Exceptions;
using AlwaysOn.Shared.Interfaces;
using AlwaysOn.Shared.Models;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using NSubstitute;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;

namespace AlwaysOn.CatalogService.UnitTests.Controllers
{
    public class CatalogItemControllerTests
    {
        private readonly IDatabaseService _databaseService = Substitute.For<IDatabaseService>();
        private readonly ILogger<CatalogItemController> _logger = Substitute.For<ILogger<CatalogItemController>>();
        
        public class ListCatalogItemsAsync : CatalogItemControllerTests
        {
            [Fact]
            public async Task ReturnsCatalogItems()
            {
                var controller = new CatalogItemController(_logger, _databaseService, null, null);

                var result = await controller.ListCatalogItemsAsync();

                result.Result.Should().BeOfType<OkObjectResult>();
                result.Should().BeAssignableTo<ActionResult<IEnumerable<CatalogItem>>>();
            }

            [Fact]
            public async Task WhenDatabaseIsUnavailable_ReturnsInternalServerError()
            {
                _databaseService.ListCatalogItemsAsync(100).Throws(new AlwaysOnDependencyException(HttpStatusCode.ServiceUnavailable));
                
                var controller = new CatalogItemController(_logger, _databaseService, null, null);
                
                var result = await controller.ListCatalogItemsAsync();

                result.Result.Should().BeOfType<ObjectResult>();
                ((ObjectResult) result.Result!).StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
            }

            [Fact]
            public async Task WhenTooManyRequests_ReturnsServiceUnavailable()
            {
                _databaseService.ListCatalogItemsAsync(100).Throws(new AlwaysOnDependencyException(HttpStatusCode.TooManyRequests));

                var controller = new CatalogItemController(_logger, _databaseService, null, null);
                
                var result = await controller.ListCatalogItemsAsync();

                result.Result.Should().BeOfType<ObjectResult>();
                ((ObjectResult) result.Result!).StatusCode.Should().Be((int) HttpStatusCode.ServiceUnavailable);
            }
        }

        public class GetCatalogItemByIdAsync
        {
            
        }

        public class CreateNewCatalogItemAsync
        {
            
        }

        public class UpdateCatalogItemAsync
        {
            
        }

        public class DeleteCatalogItemAsync : CatalogItemControllerTests
        {

            [Theory, AutoData]
            public async Task WhenNotExisting_ReturnsAccepted(Guid itemId)
            {
                _databaseService.GetCatalogItemByIdAsync(itemId).Returns(Task.FromResult<CatalogItem>(null!));

                var controller = new CatalogItemController(_logger, _databaseService, null, null);

                var result = await controller.DeleteCatalogItemAsync(itemId);
                
                result.Should().BeOfType<ObjectResult>();
            }
        }
    }
}

//         
//
//         private List<CatalogItem> GetTestCatalogItems()
//         {
//             return new List<CatalogItem>()
//             {
//                 new CatalogItem()
//                 {
//                     LastUpdated = DateTime.UtcNow,
//                     Id = Guid.NewGuid(),
//                     Description= "First test item",
//                     Name = "First Item",
//                     Price = 11111.11m
//                 },
//                 new CatalogItem() {
//                     LastUpdated = DateTime.UtcNow.AddDays(-1),
//                     Id = Guid.NewGuid(),
//                     Description= "Second test item",
//                     Name = "Second Item",
//                     Price = 99.99m
//
//                 }
//             };
//
//         }