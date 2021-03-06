FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0.102-ca-patch-buster-slim AS build
WORKDIR /src

COPY "TheRoom.sln" "TheRoom.sln"

COPY "BuildingBlocks/Devspaces.Support/Devspaces.Support.csproj" "BuildingBlocks/Devspaces.Support/Devspaces.Support.csproj"
COPY "BuildingBlocks/EventBus/EventBus/EventBus.csproj" "BuildingBlocks/EventBus/EventBus/EventBus.csproj"
COPY "BuildingBlocks/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj" "BuildingBlocks/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj"
COPY "BuildingBlocks/EventBus/EventBusServiceBus/EventBusServiceBus.csproj" "BuildingBlocks/EventBus/EventBusServiceBus/EventBusServiceBus.csproj"
COPY "BuildingBlocks/EventBus/IntegrationEventLogEF/IntegrationEventLogEF.csproj" "BuildingBlocks/EventBus/IntegrationEventLogEF/IntegrationEventLogEF.csproj"
COPY "BuildingBlocks/WebHostCustomization/WebHost.Customization/WebHost.Customization.csproj" "BuildingBlocks/WebHostCustomization/WebHost.Customization/WebHost.Customization.csproj"
COPY "Services/Bonus/BonusAPI/BonusAPI.csproj" "Services/Bonus/BonusAPI/BonusAPI.csproj"
COPY "Services/ServiceList/ServiceListAPI/ServiceListAPI.csproj" "Services/ServiceList/ServiceListAPI/ServiceListAPI.csproj"
COPY "Services/ServiceList/tests/ServiceList.FunctionalTests/ServiceList.FunctionalTests.csproj" "Services/ServiceList/tests/ServiceList.FunctionalTests/ServiceList.FunctionalTests.csproj"
COPY "Services/ServiceList/tests/ServiceList.UnitTests/ServiceList.UnitTests.csproj" "Services/ServiceList/tests/ServiceList.UnitTests/ServiceList.UnitTests.csproj"
COPY "Services/Identity/IdentityAPI/IdentityAPI.csproj" "Services/Identity/IdentityAPI/IdentityAPI.csproj"
COPY "Web/WebStatus/WebStatus.csproj" "Web/WebStatus/WebStatus.csproj"
COPY "Simple/TheRoomSimpleAPI/TheRoomSimpleAPI.csproj" "Simple/TheRoomSimpleAPI/TheRoomSimpleAPI.csproj"

COPY "docker-compose.dcproj" "docker-compose.dcproj"
COPY "NuGet.config" "NuGet.config"

RUN dotnet restore "TheRoom.sln"
COPY . .
WORKDIR /src/Services/ServiceList/ServiceListAPI
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY --from=build /src/Services/ServiceList/ServiceListAPI/Proto /app/Proto
COPY --from=build /src/Services/ServiceList/ServiceListAPI/theroom.pfx .
ENTRYPOINT ["dotnet", "ServiceListAPI.dll"]
