FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["EbayComplianceEndpoint/EbayComplianceEndpoint.csproj", "EbayComplianceEndpoint/"]
RUN dotnet restore "EbayComplianceEndpoint/EbayComplianceEndpoint.csproj"
COPY ["EbayComplianceEndpoint/", "EbayComplianceEndpoint/"]
RUN dotnet publish "EbayComplianceEndpoint/EbayComplianceEndpoint.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["sh", "-c", "export ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080} && dotnet EbayComplianceEndpoint.dll"]
