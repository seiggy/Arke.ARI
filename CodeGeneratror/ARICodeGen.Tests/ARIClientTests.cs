using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Arke.ARI;
using Arke.ARI.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace ARICodeGen.Tests
{
    public class ARIClientTests : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly HttpMessageHandlerMock _mockHttpHandler;
        private readonly HttpClient _httpClient;
        private readonly ARIClient _client;

        public ARIClientTests(ITestOutputHelper output)
        {
            _output = output;
            _mockHttpHandler = new HttpMessageHandlerMock();
            _httpClient = new HttpClient(_mockHttpHandler);
            
            // Create the client with our mocked HTTP handler
            // We're passing false for disposeHttpClient because we want to manage the lifecycle ourselves
            _client = new ARIClient("http://test:8088/ari", "user", "pass", _httpClient);
        }

        public void Dispose()
        {
            _client.Dispose();
            _httpClient.Dispose();
        }

        [Fact]
        public async Task GetAsync_DeserializesJsonCorrectly()
        {
            // Arrange
            var testResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    """
                    {
                        "id": "test-endpoint",
                        "technology": "SIP",
                        "resource": "SIP/test",
                        "state": "online"
                    }
                    """,
                    Encoding.UTF8, 
                    "application/json"
                )
            };

            _mockHttpHandler.SetResponse(testResponse);

            // Act - Call our test extension method
            var response = await _client.GetTestAsync<EndpointTestModel>("/test-endpoint"); 

            // Assert
            Assert.NotNull(response);
            Assert.Equal("test-endpoint", response.Id);
            Assert.Equal("SIP", response.Technology);
            Assert.Equal("SIP/test", response.Resource);
            Assert.Equal("online", response.State);
        }

        [Fact]
        public async Task PostAsync_SerializesJsonCorrectly()
        {
            // Arrange
            var testRequest = new
            {
                Name = "test-object",
                Value = 123
            };

            var testResponse = new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(
                    """
                    {
                        "id": "new-id",
                        "name": "test-object",
                        "value": 123
                    }
                    """,
                    Encoding.UTF8,
                    "application/json"
                )
            };

            _mockHttpHandler.SetResponse(testResponse);

            // Act - Call our test extension method
            var response = await _client.PostTestAsync<ObjectTestModel>("/test-object", testRequest);

            // Assert
            Assert.NotNull(response);
            Assert.Equal("new-id", response.Id);
            Assert.Equal("test-object", response.Name);
            Assert.Equal(123, response.Value);

            // Verify the request content was serialized correctly
            var requestContent = _mockHttpHandler.LastRequestContent;
            Assert.NotNull(requestContent);

            // Parse the sent request to verify the JSON was serialized correctly
            var requestDoc = JsonDocument.Parse(requestContent);
            var root = requestDoc.RootElement;
            
            Assert.Equal("test-object", root.GetProperty("name").GetString());
            Assert.Equal(123, root.GetProperty("value").GetInt32());
        }
        
        [Fact]
        public void Constructor_WithHttpClientFactory_CreatesValidClient()
        {
            // Arrange
            var factory = new MockHttpClientFactory(_mockHttpHandler);
            
            // Act
            using var client = new ARIClient("http://test:8088/ari", "user", "pass", factory);
            
            // Assert
            Assert.NotNull(client);
            Assert.True(client is IAriClient);
            // We can't easily verify the HttpClient was created correctly without exposing internal state
            // but the successful constructor call is a good indication
        }

        [Fact]
        public void Constructor_WithIOptions_CreatesValidClient()
        {
            // Arrange
            var factory = new MockHttpClientFactory(_mockHttpHandler);
            var options = Options.Create(new ARIClientOptions 
            { 
                BaseUrl = "http://test:8088/ari",
                Username = "user",
                Password = "pass",
                HttpClientName = "ARIClient"
            });
            
            // Act
            using var client = new ARIClient(options, factory);
            
            // Assert
            Assert.NotNull(client);
            Assert.True(client is IAriClient);
        }

        [Fact]
        public void AddARIClient_RegistersClientInDI()
        {
            // Arrange
            var services = new ServiceCollection();
            
            // Act
            services.AddARIClient(options => 
            {
                options.BaseUrl = "http://test:8088/ari";
                options.Username = "user";
                options.Password = "pass";
            });
            
            // Add our mock handler to the DI container
            services.AddSingleton<HttpMessageHandler>(_mockHttpHandler);
            services.AddHttpClient("ARIClient")
                .ConfigurePrimaryHttpMessageHandler(sp => sp.GetRequiredService<HttpMessageHandler>());
            
            var serviceProvider = services.BuildServiceProvider();
            
            // Assert
            var client = serviceProvider.GetService<IAriClient>();
            Assert.NotNull(client);
            Assert.IsType<ARIClient>(client);
        }
    }

    /// <summary>
    /// Mock implementation of IHttpClientFactory for testing
    /// </summary>
    public class MockHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpMessageHandler _handler;

        public MockHttpClientFactory(HttpMessageHandler handler)
        {
            _handler = handler;
        }

        public HttpClient CreateClient(string name)
        {
            return new HttpClient(_handler);
        }
    }

    /// <summary>
    /// A test model class used to verify serialization/deserialization
    /// </summary>
    public class EndpointTestModel
    {
        public string Id { get; set; } = string.Empty;
        public string Technology { get; set; } = string.Empty;
        public string Resource { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
    }

    /// <summary>
    /// A test model class for verifying POST operations
    /// </summary>
    public class ObjectTestModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    /// <summary>
    /// Mock HTTP handler for testing HTTP client operations
    /// </summary>
    public class HttpMessageHandlerMock : HttpMessageHandler
    {
        private HttpResponseMessage? _response;
        public string LastRequestContent { get; private set; } = string.Empty;

        public void SetResponse(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Content != null)
            {
                LastRequestContent = await request.Content.ReadAsStringAsync();
            }
            
            return _response ?? new HttpResponseMessage(HttpStatusCode.NotFound);
        }
    }
} 