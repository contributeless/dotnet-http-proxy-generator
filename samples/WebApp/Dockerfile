#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["samples/WebApp/WebApp.csproj", "samples/WebApp/"]
COPY ["lib/HttpProxyGenerator.Consumer/HttpProxyGenerator.Consumer.csproj", "lib/HttpProxyGenerator.Consumer/"]
COPY ["lib/HttpProxyGenerator.Common/HttpProxyGenerator.Common.csproj", "lib/HttpProxyGenerator.Common/"]
COPY ["samples/InterfacesLibrary/InterfacesLibrary.csproj", "samples/InterfacesLibrary/"]
RUN dotnet restore "samples/WebApp/WebApp.csproj"
COPY . .
WORKDIR "/src/samples/WebApp"
RUN dotnet build "WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApp.dll"]