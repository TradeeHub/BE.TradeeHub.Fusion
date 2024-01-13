FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BE.TradeeHub.Fusion/BE.TradeeHub.Fusion.csproj", "BE.TradeeHub.Fusion/"]
RUN dotnet restore "BE.TradeeHub.Fusion/BE.TradeeHub.Fusion.csproj"
COPY . .
WORKDIR "/src/BE.TradeeHub.Fusion"
RUN dotnet build "BE.TradeeHub.Fusion.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BE.TradeeHub.Fusion.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BE.TradeeHub.Fusion.dll"]
