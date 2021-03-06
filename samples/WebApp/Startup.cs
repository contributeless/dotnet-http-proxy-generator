using HttpProxyGenerator.Consumer.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using InterfacesLibrary;
using InterfacesLibrary.AnotherNamespace;
using WebApp.Controllers;

namespace WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson()
                .RegisterHttpProxyEndpoints(options =>
                {
                    options.RegisterInterfaceToExpose<IDataFetcher>();
                    options.RegisterInterfaceToExpose<IGenericDataFetcher<SampleData>>();
                    options.RegisterInterfaceToExpose<IGenericDataFetcher<SampleData2>>();
                });

            services.AddSingleton<IDataFetcher, DataFetcher>();
            services.AddSingleton<IGenericDataFetcher<SampleData>, GenericDataFetcher>();
            services.AddSingleton<IGenericDataFetcher<SampleData2>, GenericDataFetcher2>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApp", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApp v1"));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
