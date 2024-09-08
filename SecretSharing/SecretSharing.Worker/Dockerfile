FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SecretSharing/SecretSharing.Worker/SecretSharing.Worker.csproj", "SecretSharing/SecretSharing.Worker/"]
RUN dotnet restore "SecretSharing/SecretSharing.Worker/SecretSharing.Worker.csproj"
COPY . .
WORKDIR "/src/SecretSharing/SecretSharing.Worker"
RUN dotnet build "SecretSharing.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SecretSharing.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SecretSharing.Worker.dll"]
