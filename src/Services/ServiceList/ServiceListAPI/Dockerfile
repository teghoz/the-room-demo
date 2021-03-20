FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0.102-ca-patch-buster-slim AS build
WORKDIR /src

# It's important to keep lines from here down to "COPY . ." identical in all Dockerfiles
# to take advantage of Docker's build cache, to speed up local container builds
COPY "eShopOnContainers-ServicesAndWebApps.sln" "eShopOnContainers-ServicesAndWebApps.sln"

COPY "ApiGateways/Mobile.Bff.Shopping/aggregator/Mobile.Shopping.HttpAggregator.csproj" "ApiGateways/Mobile.Bff.Shopping/aggregator/Mobile.Shopping.HttpAggregator.csproj"
COPY "ApiGateways/Web.Bff.Shopping/aggregator/Web.Shopping.HttpAggregator.csproj" "ApiGateways/Web.Bff.Shopping/aggregator/Web.Shopping.HttpAggregator.csproj"
COPY "BuildingBlocks/Devspaces.Support/Devspaces.Support.csproj" "BuildingBlocks/Devspaces.Support/Devspaces.Support.csproj"
COPY "BuildingBlocks/EventBus/EventBus/EventBus.csproj" "BuildingBlocks/EventBus/EventBus/EventBus.csproj"
COPY "BuildingBlocks/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj" "BuildingBlocks/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj"
COPY "BuildingBlocks/EventBus/EventBusServiceBus/EventBusServiceBus.csproj" "BuildingBlocks/EventBus/EventBusServiceBus/EventBusServiceBus.csproj"
COPY "BuildingBlocks/EventBus/IntegrationEventLogEF/IntegrationEventLogEF.csproj" "BuildingBlocks/EventBus/IntegrationEventLogEF/IntegrationEventLogEF.csproj"
COPY "BuildingBlocks/WebHostCustomization/WebHost.Customization/WebHost.Customization.csproj" "BuildingBlocks/WebHostCustomization/WebHost.Customization/WebHost.Customization.csproj"
COPY "Services/ServiceList/ServiceList.API.csproj" "Services/ServiceList/ServiceList.API.csproj"
#COPY "Services/Catalog/Catalog.FunctionalTests/Catalog.FunctionalTests.csproj" "Services/Catalog/Catalog.FunctionalTests/Catalog.FunctionalTests.csproj"
#COPY "Services/Catalog/Catalog.UnitTests/Catalog.UnitTests.csproj" "Services/Catalog/Catalog.UnitTests/Catalog.UnitTests.csproj"
COPY "Services/Identity/IdentityAPI/IdentityAPI.csproj" "Services/Identity/IdentityAPI/IdentityAPI.csproj"

COPY "docker-compose.dcproj" "docker-compose.dcproj"

COPY "NuGet.config" "NuGet.config"

RUN dotnet restore "TheRoom.sln"

COPY . .
WORKDIR /src/Services/ServiceList/ServiceList.API
RUN dotnet publish --no-restore -c Release -o /app

#FROM build as unittest
#WORKDIR /src/Services/Catalog/Catalog.UnitTests
#
#FROM build as functionaltest
#WORKDIR /src/Services/Catalog/Catalog.FunctionalTests

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY --from=build /src/Services/ServiceList/ServiceList.API/Proto /app/Proto
COPY --from=build /src/Services/ServiceList/ServiceList.API/eshop.pfx .
ENTRYPOINT ["dotnet", "Catalog.API.dll"]
