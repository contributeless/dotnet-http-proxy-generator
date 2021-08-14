using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Linq;
using HttpProxyGenerator;
using InterfacesLibrary;
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

            var sw = new Stopwatch();
            sw.Start();

            var options = new ControllerGeneratorOptions();
            options.RegisterInterfaceToExpose<IDataFetcher>();
            // options.RegisterInterfaceToExpose<IData2Fetcher>();
            // options.RegisterInterfaceToExpose<IData3Fetcher>();
            // options.RegisterInterfaceToExpose<IData7Fetcher>();
            // options.RegisterInterfaceToExpose<IData5Fetcher>();

            services.AddSingleton<IDataFetcher, DataFetcher>();
            // services.AddSingleton<IData2Fetcher, Data2Fetcher>();
            // services.AddSingleton<IData3Fetcher, Data3Fetcher>();
            // services.AddSingleton<IData5Fetcher, Data5Fetcher>();
            // services.AddSingleton<IData7Fetcher, Data7Fetcher>();

            var generator = new ControllerGenerator(options);

            var (result, assemblies) = generator.Generate();

            Console.WriteLine($"Classes generated in {sw.ElapsedMilliseconds}");

            var compiler = new InMemoryCompiler();

            var assembly = compiler.CompileCSharpCode(result, assemblies);
            var types = assembly.GetExportedTypes().Where(x => typeof(ControllerBase).IsAssignableFrom(x));

            Console.WriteLine($"Classes compiled in {sw.ElapsedMilliseconds}");
            foreach (var type in types)
            {
                services.AddTransient(type);
            }

            Console.WriteLine($"Classes registered in {sw.ElapsedMilliseconds}");

            services.AddControllers().AddNewtonsoftJson().AddControllersAsServices()
                .AddApplicationPart(types.First().Assembly)
                ;

            
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
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApp v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
