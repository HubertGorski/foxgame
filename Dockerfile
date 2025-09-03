FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["HubWorkspace.sln", "."]
COPY ["FoxTales/FoxTales.Api/FoxTales.Api.csproj", "FoxTales/FoxTales.Api/"]
COPY ["FoxTales/FoxTales.Application/FoxTales.Application.csproj", "FoxTales/FoxTales.Application/"]
COPY ["FoxTales/FoxTales.Domain/FoxTales.Domain.csproj", "FoxTales/FoxTales.Domain/"]
COPY ["FoxTales/FoxTales.Infrastructure/FoxTales.Infrastructure.csproj", "FoxTales/FoxTales.Infrastructure/"]
COPY ["FoxTales/FoxTales.Composition/FoxTales.Composition.csproj", "FoxTales/FoxTales.Composition/"]

RUN dotnet restore "HubWorkspace.sln" -v detailed

COPY . .

RUN dotnet build "HubWorkspace.sln" -c Release

RUN dotnet publish "FoxTales/FoxTales.Api/FoxTales.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FoxTales.Api.dll"]