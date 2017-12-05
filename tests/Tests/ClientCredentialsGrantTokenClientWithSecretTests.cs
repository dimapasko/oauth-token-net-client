using System.Threading;
using System.Threading.Tasks;
using Scalepoint.OAuth.TokenClient;
using Xunit;

namespace Tests
{
    public class ClientCredentialsGrantTokenClientWithSecretTests : MockServerTestBase
    {
        private readonly ClientCredentialsGrantTokenClient _tokenClient;

        public ClientCredentialsGrantTokenClientWithSecretTests()
        {
            _tokenClient = new ClientCredentialsGrantTokenClient(
                TokenEndpointUri,
                new ClientSecretCredentials(
                    "test_client",
                    "test_secret"
                ));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _tokenClient.Dispose();
            }
        }

        [Fact]
        public async Task should_cancel()
        {
            await Assert.ThrowsAsync<TaskCanceledException>(
                () => _tokenClient.GetTokenAsync(new[] {"test_scope"}, new CancellationToken(true)));
        }

        [Fact]
        public async Task should_not_cancel()
        {
            await _tokenClient.GetTokenAsync(new[] { "test_scope" }, new CancellationToken(false));
        }

        [Fact]
        public async Task should_get_token()
        {
            var token = await _tokenClient.GetTokenAsync(new[] { "test_scope" });
            Assert.NotNull(token);
        }

        [Fact]
        public async Task should_get_token_from_cache()
        {
            var token1 = await _tokenClient.GetTokenAsync(new[] { "test_scope" });
            var token2 = await _tokenClient.GetTokenAsync(new[] { "test_scope" });
            Assert.Equal(token1, token2);
        }

        [Fact]
        public async Task should_handle_error()
        {
            await Assert.ThrowsAsync<TokenEndpointException>(async () =>
            {
                await _tokenClient.GetTokenAsync(new[] { "test_scope", "invalid_scope" });
            });
        }
    }
}
