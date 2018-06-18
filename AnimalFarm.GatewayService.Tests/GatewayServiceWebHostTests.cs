using AnimalFarm.Utils.Security;
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

namespace AnimalFarm.GatewayService.Tests
{
    public class GatewayServiceWebHostTests
    {
        private void ConfigureTestServices(IServiceCollection services)
        {
            services
                .AddAnimalFarmCommonServices()
                .AddCors()
                .AddRouting()
                .ReplaceSingleton<ILogger>(new Mock<ILogger>().Object)
                .AddSingleton<RequestForwarder>()
                .AddSingleton<CorsPolicy>(new CorsPolicy());
        }

        [Fact]
        public void ForwardsLogin()
        {
            var serviceHttpClientMock = new Mock<IServiceHttpClient>();
            serviceHttpClientMock.Setup(_ => _.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage());
            var serviceHttpClientFactoryMock = new Mock<IServiceHttpClientFactory>();
            serviceHttpClientFactoryMock.Setup(_ => _.CreateAsync(ServiceType.Authentication, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceHttpClientMock.Object);

            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    ConfigureTestServices(services);
                    services.AddSingleton<IServiceHttpClientFactory>(serviceHttpClientFactoryMock.Object);
                })
               .UseStartup<Startup>());
            var loginData = new { Login = "Login", Password = "Password" };

            var stringContent = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");
            server.CreateRequest("login").And(m => m.Content = stringContent).PostAsync().GetAwaiter().GetResult();

            serviceHttpClientMock.Verify(_ => _.SendAsync(It.Is<HttpRequestMessage>((message) =>
                message.Content.ReadAsStringAsync().GetAwaiter().GetResult().Contains("Password")
                    && message.RequestUri.ToString() == "login"),
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public void ForwardsSpecificRulesetRequest()
        {
            var serviceHttpClientMock = new Mock<IServiceHttpClient>();
            serviceHttpClientMock.Setup(_ => _.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage());
            var serviceHttpClientFactoryMock = new Mock<IServiceHttpClientFactory>();
            serviceHttpClientFactoryMock.Setup(_ => _.CreateAsync(ServiceType.Ruleset, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceHttpClientMock.Object);


            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    ConfigureTestServices(services);
                    services.AddSingleton<IServiceHttpClientFactory>(serviceHttpClientFactoryMock.Object);
                })
               .UseStartup<Startup>());

            server.CreateRequest("ruleset/db5e60ce-f7d9-47d8-9c74-0231190ba3df").GetAsync().GetAwaiter().GetResult();

            serviceHttpClientMock.Verify(_ => _.SendAsync(It.Is<HttpRequestMessage>((message) =>
                message.RequestUri.ToString() == "db5e60ce-f7d9-47d8-9c74-0231190ba3df"), It.IsAny<CancellationToken>()));
        }

        [Fact]
        public void ForwardsLatestRulesetRequest()
        {
            var serviceHttpClientMock = new Mock<IServiceHttpClient>();
            serviceHttpClientMock.Setup(_ => _.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage());
            var serviceHttpClientFactoryMock = new Mock<IServiceHttpClientFactory>();
            serviceHttpClientFactoryMock.Setup(_ => _.CreateAsync(ServiceType.Ruleset, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceHttpClientMock.Object);

            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(services => 
                {
                    ConfigureTestServices(services);
                    services.AddSingleton<IServiceHttpClientFactory>(serviceHttpClientFactoryMock.Object);
                })
               .UseStartup<Startup>());

            server.CreateRequest("ruleset").GetAsync().GetAwaiter().GetResult();

            serviceHttpClientMock.Verify(_ => _.SendAsync(It.Is<HttpRequestMessage>((message) =>
                message.RequestUri.ToString() == ""), It.IsAny<CancellationToken>()));
        }
    }
}
