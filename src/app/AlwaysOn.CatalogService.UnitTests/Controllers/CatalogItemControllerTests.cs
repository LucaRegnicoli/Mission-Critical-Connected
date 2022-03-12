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
            [Theory, AutoData]
            public async Task WhenLimitIsAccepted_ReturnsCatalogItems(IEnumerable<CatalogItem> items)
            {
                _databaseService.ListCatalogItemsAsync(100).Returns(items);
                
                var controller = new CatalogItemController(_logger, _databaseService, null, null);

                var result = await controller.ListCatalogItemsAsync();

                result.Result.Should()
                    .BeOfType<OkObjectResult>()
                    .Which
                    .Value.Should().BeEquivalentTo(items);
            }

            [Fact]
            public async Task WhenDatabaseIsUnavailable_ReturnsInternalServerError()
            {
                _databaseService.ListCatalogItemsAsync(100).Throws(new AlwaysOnDependencyException(HttpStatusCode.ServiceUnavailable));
                
                var controller = new CatalogItemController(_logger, _databaseService, null, null);
                
                var result = await controller.ListCatalogItemsAsync();

                result.Result.Should()
                    .BeOfType<ObjectResult>()
                    .Which
                    .StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
            }

            [Fact]
            public async Task WhenTooManyRequests_ReturnsServiceUnavailable()
            {
                _databaseService.ListCatalogItemsAsync(100).Throws(new AlwaysOnDependencyException(HttpStatusCode.TooManyRequests));

                var controller = new CatalogItemController(_logger, _databaseService, null, null);
                
                var result = await controller.ListCatalogItemsAsync();

                result.Result.Should()
                    .BeOfType<ObjectResult>()
                    .Which
                    .StatusCode.Should().Be((int) HttpStatusCode.ServiceUnavailable);
            }
        }

        public class GetCatalogItemByIdAsync : CatalogItemControllerTests
        {
            [Theory, AutoData]
            public async Task WhenItemExists_ReturnsCatalogItem(Guid itemId, CatalogItem item)
            {
                _databaseService.GetCatalogItemByIdAsync(itemId).Returns(item);

                var controller = new CatalogItemController(_logger, _databaseService, null, null);

                var result = await controller.GetCatalogItemByIdAsync(itemId);
                
                result.Result.Should()
                    .BeOfType<OkObjectResult>()
                    .Which
                    .Value.Should().BeEquivalentTo(item);
            }
            
            [Theory, AutoData]
            public async Task WhenItemDoesNotExist_ReturnsCatalogItem(Guid itemId)
            {
                _databaseService.GetCatalogItemByIdAsync(itemId).Returns(Task.FromResult<CatalogItem>(null!));

                var controller = new CatalogItemController(_logger, _databaseService, null, null);

                var result = await controller.GetCatalogItemByIdAsync(itemId);
                
                result.Result.Should().BeOfType<NotFoundResult>();
            }
        }

        public class DeleteCatalogItemAsync : CatalogItemControllerTests
        {
            [Theory, AutoData]
            public async Task WhenItemDoesNotExist_ReturnsAccepted(Guid itemId)
            {
                _databaseService.GetCatalogItemByIdAsync(itemId).Returns(Task.FromResult<CatalogItem>(null!));

                var controller = new CatalogItemController(_logger, _databaseService, null, null);

                var result = await controller.DeleteCatalogItemAsync(itemId);
                
                result.Should().BeOfType<ObjectResult>();
            }
        }
    }
}
