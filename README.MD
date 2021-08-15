# dotnet-http-proxy-generator

Usage example:

1. Define the Interface that will be used in both projects
2. Register proxy controllers in the Consumer app

```csharp
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
```

3. Create a proxy in the Client app

```csharp
            var client = new HttpClient {BaseAddress = new Uri("http://localhost:5000")};

            var generator = new HttpClientProxyGenerator();

            var proxy = generator.CreateProxy<IDataFetcher>(client, new DefaultProxyContractProvider(), new DefaultProxyNamingConventionProvider());
```

4. Call any method on the returned proxy

```csharp
            var result = await proxy.GetBaseData("test", 1);
            Console.WriteLine(result);

            var genericData = await proxy.GetGenericData();
            Console.WriteLine(genericData.Info1);
```