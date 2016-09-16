using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using RestApi.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.PlatformAbstractions;
using RestApi.Business;
using RestApi.Configuration;
using RestApi.Swagger;

namespace RestApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            // Add framework services.
            services.AddMvc()
                .AddJsonOptions(option => option.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver())
                .AddJsonOptions(option => option.SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true }));

            services.AddTransient<ICalculator, Calculator>();
            services.AddTransient<IFoo, Foo>();

            services.Configure<MySettings>(Configuration.GetSection("MySettings"));

            var connection = @"Server=.;Database=Sales;User ID=SalesUser;Password=1234;MultipleActiveResultSets=true";
            services.AddDbContext<SalesDbContext>(options => options.UseSqlServer(connection));

            var app = PlatformServices.Default.Application;
            var xmlDocPath = Path.Combine(app.ApplicationBasePath, Path.ChangeExtension(app.ApplicationName, "xml"));

            services.AddSwaggerGen(options =>
            {
                if (File.Exists(xmlDocPath))
                {
                    options.IncludeXmlComments(xmlDocPath);
                }
                options.DescribeAllEnumsAsStrings();
                options.OperationFilter<ExamplesOperationFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
            app.UseCorsWithStandardMethods();
            app.UseLoggerMiddleware();

            app.UseSwagger();
            app.UseSwaggerUi();
        }
    }
}
