FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore src/Services/Payments/SmartShop.Payments.Api/SmartShop.Payments.Api.csproj
RUN dotnet publish src/Services/Payments/SmartShop.Payments.Api/SmartShop.Payments.Api.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "SmartShop.Payments.Api.dll"]
