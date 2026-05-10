FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY *.slnx .
COPY OrderService.WebApi/*.csproj OrderService.WebApi/
COPY OrderService.Domain/*.csproj OrderService.Domain/
COPY OrderService.Infrastructure/*.csproj OrderService.Infrastructure/
RUN dotnet restore
COPY . .
RUN dotnet publish OrderService.WebApi -c Release -o /app/publish --no-restore
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
RUN groupadd --system appgroup && useradd --system --gid appgroup appuser
USER appuser
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "OrderService.WebApi.dll"]