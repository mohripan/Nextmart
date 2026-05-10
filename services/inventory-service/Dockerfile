FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY *.slnx .
COPY InventoryService.WebApi/*.csproj InventoryService.WebApi/
COPY InventoryService.Domain/*.csproj InventoryService.Domain/
COPY InventoryService.Infrastructure/*.csproj InventoryService.Infrastructure/
RUN dotnet restore
COPY . .
RUN dotnet publish InventoryService.WebApi -c Release -o /app/publish --no-restore
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
RUN groupadd --system appgroup && useradd --system --gid appgroup appuser
USER appuser
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "InventoryService.WebApi.dll"]