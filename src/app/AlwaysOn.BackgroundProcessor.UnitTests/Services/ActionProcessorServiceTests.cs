using System;
using System.Threading.Tasks;
using AlwaysOn.BackgroundProcessor.Services;
using AlwaysOn.BackgroundProcessor.UnitTests.Services.Fixtures;
using AlwaysOn.Shared.Interfaces;
using AlwaysOn.Shared;
using AlwaysOn.Shared.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace AlwaysOn.BackgroundProcessor.UnitTests.Services
{
    public class ActionProcessorServiceTests
    {
        private readonly IDatabaseService _databaseService = Substitute.For<IDatabaseService>();
        private readonly ILogger<ActionProcessorService> _logger = Substitute.For<ILogger<ActionProcessorService>>();
        
        public class Process : ActionProcessorServiceTests
        {
            [Theory, ItemBodyAutoData]
            public async Task WhenActionIsAddCatalog_CatalogItemIsPersisted(ItemBody body)
            {
                var service = new ActionProcessorService(_logger, _databaseService, null);

                await service.Process(Constants.AddCatalogItemActionName, body.Body);

                await _databaseService.ReceivedWithAnyArgs(1).AddNewCatalogItemAsync(Arg.Any<CatalogItem>());
            }
            
            [Theory, ItemBodyAutoData]
            public async Task WhenDatabaseIsUnavailable_ThrowsException(ItemBody body)
            {
                _databaseService.AddNewCatalogItemAsync(Arg.Any<CatalogItem>()).ThrowsForAnyArgs(new Exception());
                
                var service = new ActionProcessorService(_logger, _databaseService, null);

                Func<Task> action = async () => await service.Process(Constants.AddCatalogItemActionName, body.Body);

                await action.Should().ThrowAsync<Exception>();
            }
        }
    }    
}

