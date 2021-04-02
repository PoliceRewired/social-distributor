using System;
using System.Threading.Tasks;
using PoliceRewiredSocialDistributorLib.Helpers;
using Xunit;

namespace DistributeSocialLambda.Tests
{
    public class UriHelperTests
    {
        [Fact]
        public async Task UriHelperCanResolveRedirectionAsync()
        {
            for (var i = 0; i < 10; i++)
            {
                var input = new Uri("http://bit.ly/PoliceRewired-join");
                var expected = new Uri("https://policerewired.us20.list-manage.com/subscribe?u=af564765bdade494172e3e071&id=364eaf7368");
                var found = await UrlHelper.ResolveUrlAsync(input);
                Assert.Equal(found, expected);
            }
        }

    }
}
