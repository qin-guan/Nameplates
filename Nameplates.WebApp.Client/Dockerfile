FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Nameplates.WebApp.Client/Nameplates.WebApp.Client.csproj", "Nameplates.WebApp.Client/"]
RUN dotnet restore "Nameplates.WebApp.Client/Nameplates.WebApp.Client.csproj"
COPY . .
WORKDIR "/src/Nameplates.WebApp.Client"
RUN dotnet build "./Nameplates.WebApp.Client.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Nameplates.WebApp.Client.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nameplates.WebApp.Client.dll"]
