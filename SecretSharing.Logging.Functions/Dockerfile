FROM mcr.microsoft.com/dotnet/sdk:8.0 AS secretsharing-logging-functions

COPY . /SecretSharing.Logging.Functions/
RUN cd /SecretSharing.Logging.Functions/ && \
mkdir -p /home/site/wwwroot && \
dotnet publish *.csproj --output /home/site/wwwroot

# To enable ssh & remote debugging on app service change the base image to the one below
# FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated6.0-appservice
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

COPY --from=secretsharing-logging-functions ["/home/site/wwwroot", "/home/site/wwwroot"]