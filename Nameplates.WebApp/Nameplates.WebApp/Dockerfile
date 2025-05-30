FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Nameplates.WebApp/Nameplates.WebApp/Nameplates.WebApp.csproj", "Nameplates.WebApp/Nameplates.WebApp/"]
COPY ["Nameplates.WebApp/Nameplates.WebApp.Client/Nameplates.WebApp.Client.csproj", "Nameplates.WebApp/Nameplates.WebApp.Client/"]
RUN dotnet restore "Nameplates.WebApp/Nameplates.WebApp/Nameplates.WebApp.csproj"
COPY . .
WORKDIR "/src/Nameplates.WebApp/Nameplates.WebApp"
RUN dotnet build "./Nameplates.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Nameplates.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nameplates.WebApp.dll"]
