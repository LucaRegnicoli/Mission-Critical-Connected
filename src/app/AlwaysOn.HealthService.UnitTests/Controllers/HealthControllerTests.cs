using System;
using System.Collections.Generic;
using System.Configuration;
using AlwaysOn.HealthService.Controllers;
using AlwaysOn.Shared;
using Castle.Core.Configuration;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSubstitute;
using Xunit;

namespace AlwaysOn.HealthService.UnitTests.Controllers
{
    public class HealthControllerTests
    {
        public class Constructor : HealthControllerTests
        {
            public static IEnumerable<object[]> NullServices()
            {
                yield return new object[]
                {
                    Substitute.For<SysConfiguration>((IConfiguration)null!), null! , "healthCheckService",
                };
                yield return new object[]
                {
                    null!, Substitute.For<HealthCheckService>(), "sysConfig",
                };
            }
            
            [Theory, MemberData(nameof(NullServices))]
            public void WhenDependentServicesAreNull_ThrowsArgumentNullException_(SysConfiguration sysConfig, HealthCheckService healthCheckService, string expectedErrorParameter)
            {
                Action action = () => { new HealthController(sysConfig, healthCheckService); };

                action.Should().Throw<ArgumentNullException>().Where(x => x.ParamName == expectedErrorParameter);
            }
        }
    }    
}

