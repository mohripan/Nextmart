FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY *.slnx .
COPY ProductCatalog.WebApi/*.csproj ProductCatalog.WebApi/
COPY ProductCatalog.Infrastructure/*.csproj ProductCatalog.Infrastructure/
COPY ProductCatalog.Models/*.csproj ProductCatalog.Models/

RUN dotnet restore

COPY . .
RUN dotnet publish ProductCatalog.WebApi -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN groupadd --system appgroup && useradd --system --gid appgroup appuser
USER appuser

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ProductCatalog.WebApi.dll"]