using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AlwaysOn.CatalogService.Controllers;
using AlwaysOn.Shared.Interfaces;
using AlwaysOn.Shared.Models.DataTransfer;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace AlwaysOn.CatalogService.UnitTests.Controllers
{
    public class RatingControllerTests
    {
        private readonly ILogger<RatingsController> _logger = Substitute.For<ILogger<RatingsController>>();
        private readonly IDatabaseService _databaseService = Substitute.For<IDatabaseService>();
        private readonly IMessageProducerService _messageProducerService = Substitute.For<IMessageProducerService>();
        
        public class AddNewItemRatingAsync : RatingControllerTests
        {
            public static IEnumerable<object[]> ValidRatings()
            {
                yield return new object[] {1};
                yield return new object[] {5};
            }
            
            [Theory, MemberAutoData(nameof(ValidRatings))]
            public async Task WhenRatingIsValid_ReturnsAccepted(int rating, Guid itemId)
            {
                var controller = new RatingsController(_logger, _databaseService, _messageProducerService)
                {
                    ControllerContext = new ControllerContext()
                    {
                        HttpContext = new DefaultHttpContext()
                    }
                };

                var result = await controller.AddNewItemRatingAsync(itemId, new NewRatingDto() {Rating = rating});

                result.Should()
                    .BeOfType<StatusCodeResult>()
                    .Which
                    .StatusCode.Should().Be((int) HttpStatusCode.Accepted);
            } 
            
            [Theory]
            [InlineAutoData(int.MinValue)]
            [InlineAutoData(int.MaxValue)]
            public async Task WhenRatingIsNotValid_ReturnsBadRequest(int rating, Guid itemId)
            {
                var controller = new RatingsController(_logger, _databaseService, _messageProducerService);

                var result = await controller.AddNewItemRatingAsync(itemId, new NewRatingDto() {Rating = rating});

                result.Should().BeOfType<BadRequestObjectResult>();
            } 
        }
    }
}