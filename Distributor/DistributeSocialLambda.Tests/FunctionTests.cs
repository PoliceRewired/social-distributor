using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using DistributeSocialLambda;
using DistributeSocialLambda.DTO;

namespace DistributeSocialLambda.Tests
{
    public class FunctionTest
    {
        [Fact]
        public async Task TestDryRunNetworks()
        {
            var function = new Function();
            var context = new TestLambdaContext();

            var input = new DistributeSocialCommand()
            {
                command = "dry-run",
                text = "test message",
                tags = "#test",
                linkUrl = "https://www.google.com",

                networks = new string[] { "twitter", "facebook", "discord" }
            };

            var result = await function.FunctionHandler(input, context);

            // this is not a real test - we should seek assurances that things can really post, somehow
            Assert.Equal(result.input.command, input.command);

            // for now this test assures itself that dry-run doesn't succeed
            foreach (var network in result.results)
            {
                Assert.Contains(network.Key, input.networks);
                Assert.False(network.Value.Success);
            }
        }
    }
}
