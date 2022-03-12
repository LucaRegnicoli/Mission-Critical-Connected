using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AlwaysOn.CatalogService.Controllers;
using AlwaysOn.Shared.Interfaces;
using AlwaysOn.Shared.Models.DataTransfer;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace AlwaysOn.CatalogService.UnitTests.Controllers
{
    public class CommentsControllerTests
    {
        private readonly IDatabaseService _databaseService = Substitute.For<IDatabaseService>();
        private readonly IMessageProducerService _messageProducerService = Substitute.For<IMessageProducerService>();
        private readonly ILogger<CommentsController> _logger = Substitute.For<ILogger<CommentsController>>();
        
        public class AddNewItemCommentAsync : CommentsControllerTests
        {
            [Theory, AutoData]
            public async Task WhenCommentIsValid_ReturnsAccepted(Guid itemId, NewCommentDto comment)
            {
                var controller = new CommentsController(_logger, _databaseService, _messageProducerService)
                {
                    ControllerContext = new ControllerContext()
                    {
                        HttpContext = new DefaultHttpContext()
                    }
                };

                var result = await controller.AddNewItemCommentAsync(itemId, comment);

                result.Result.Should().BeOfType<AcceptedAtRouteResult>();
            }
        }
        
    }
}