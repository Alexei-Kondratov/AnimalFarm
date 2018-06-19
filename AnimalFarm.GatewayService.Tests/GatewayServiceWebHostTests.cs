using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using AnimalFarm.Service.Utils.Communication;
using Moq;
using AnimalFarm.Service.Utils;
using System.Threading;
using AnimalFarm.Service.Utils.Tracing;
using AnimalFarm.Service;
using Microsoft.AspNetCore.Cors.Infrastructure;
using AnimalFarm.Utils.DependencyInjection;
using System.Net;

namespace AnimalFarm.GatewayService.Tests
{
    public class GatewayServiceWebHostTests
    {
        private Mock<IServiceHttpClient> _serviceHttpClientMock = new Mock<IServiceHttpClient>();
        private Mock<IServiceHttpClientFactory> _serviceHttpClientFactoryMock = new Mock<IServiceHttpClientFactory>();

        public GatewayServiceWebHostTests()
        {
            _serviceHttpClientMock.Setup(_ => _.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage());
        }

        private void ConfigureTestServices(IServiceCollection services)
        {
            services
                .AddAnimalFarmCommonServices()
                .AddCors()
                .AddRouting()
                .ReplaceSingleton<ILogger>(new Mock<ILogger>().Object)
                .AddSingleton<IServiceHttpClientFactory>(_serviceHttpClientFactoryMock.Object)
                .AddSingleton<RequestForwarder>()
                .AddSingleton<CorsPolicy>(new CorsPolicy());
        }

        [Fact]
        public void ForwardsLogin()
        {
            _serviceHttpClientFactoryMock.Setup(_ => _.CreateAsync(ServiceType.Authentication, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_serviceHttpClientMock.Object);

            var server = new TestServer(new WebHostBuilder()
               .ConfigureServices(ConfigureTestServices)
               .UseStartup<Startup>());
            var loginData = new { Login = "Login", Password = "Password" };

            var stringContent = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");
            server.CreateRequest("login").And(m => m.Content = stringContent).PostAsync().GetAwaiter().GetResult();

            _serviceHttpClientMock.Verify(_ => _.SendAsync(It.Is<HttpRequestMessage>((message) =>
                message.Content.ReadAsStringAsync().GetAwaiter().GetResult().Contains("Password")
                    && message.RequestUri.ToString() == "login"),
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public void ForwardsSpecificRulesetRequest()
        {
            _serviceHttpClientFactoryMock.Setup(_ => _.CreateAsync(ServiceType.Ruleset, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_serviceHttpClientMock.Object);

            var server = new TestServer(new WebHostBuilder()
               .ConfigureServices(ConfigureTestServices)
               .UseStartup<Startup>());

            server.CreateRequest("ruleset/db5e60ce-f7d9-47d8-9c74-0231190ba3df").GetAsync().GetAwaiter().GetResult();

            _serviceHttpClientMock.Verify(_ => _.SendAsync(It.Is<HttpRequestMessage>((message) =>
                message.RequestUri.ToString() == "db5e60ce-f7d9-47d8-9c74-0231190ba3df"), It.IsAny<CancellationToken>()));
        }

        [Fact]
        public void ForwardsLatestRulesetRequest()
        {
            _serviceHttpClientFactoryMock.Setup(_ => _.CreateAsync(ServiceType.Ruleset, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_serviceHttpClientMock.Object);

            var server = new TestServer(new WebHostBuilder()
               .ConfigureServices(ConfigureTestServices)
               .UseStartup<Startup>());

            server.CreateRequest("ruleset").GetAsync().GetAwaiter().GetResult();

            _serviceHttpClientMock.Verify(_ => _.SendAsync(It.Is<HttpRequestMessage>((message) =>
                message.RequestUri.ToString() == ""), It.IsAny<CancellationToken>()));
        }

        [Fact]
        public void GetAnimalRequestWithoutAuthenticationTokenFails()
        {
            var server = new TestServer(new WebHostBuilder()
               .ConfigureServices(ConfigureTestServices)
               .UseStartup<Startup>());

            var result = server.CreateRequest("animal/641107c5a9f74520b4391787985679c2").GetAsync().GetAwaiter().GetResult();

            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }
    }
}
