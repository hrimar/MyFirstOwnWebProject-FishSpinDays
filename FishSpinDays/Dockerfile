# Use official .NET 8 SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore ./FishSpinDays.Web/FishSpinDays.Web.csproj
RUN dotnet publish ./FishSpinDays.Web/FishSpinDays.Web.csproj -c Release -o /app

# Use official .NET 8 ASP.NET runtime image for run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
RUN apt-get update && apt-get install -y netcat-openbsd dos2unix && rm -rf /var/lib/apt/lists/*
COPY --from=build /app .
COPY FishSpinDays.Web/wwwroot ./wwwroot
COPY FishSpinDays.Web/wait-for-sqlserver.sh /wait-for-sqlserver.sh
RUN dos2unix /wait-for-sqlserver.sh
RUN chmod +x /wait-for-sqlserver.sh
RUN ls -la /wait-for-sqlserver.sh
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["/wait-for-sqlserver.sh", "sqlserver", "1433", "dotnet", "FishSpinDays.Web.dll"]