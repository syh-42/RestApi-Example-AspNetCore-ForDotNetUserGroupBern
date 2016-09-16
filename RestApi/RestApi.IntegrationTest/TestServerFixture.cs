using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace RestApi.IntegrationTest
{
    public class TestServerFixture<TStartup, TDbContext> : IDisposable
        where TStartup : class
        where TDbContext : DbContext
    {
        private readonly TestServer _server;
        public HttpClient Client { get; }
        public TDbContext DbContext { get; }

        public TestServerFixture()
        {
            var builder = GetWebHostBuilder();
            _server = new TestServer(builder);
            Client = _server.CreateClient();
            DbContext = _server.Host.Services.GetService<TDbContext>();
        }

        private IWebHostBuilder GetWebHostBuilder()
        {
            return new WebHostBuilder()
                .UseStartup(typeof(TStartup))
                .ConfigureServices(AddInMemoryDatabase);
        }

        private static void AddInMemoryDatabase(IServiceCollection services)
        {
            services.AddDbContext<TDbContext>(options =>
            {
                options.UseInMemoryDatabase();
            });
        }

        public void Dispose()
        {
            Client.Dispose();
            _server.Dispose();
        }
    }
}