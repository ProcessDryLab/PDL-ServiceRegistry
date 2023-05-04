#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS base
WORKDIR /app
EXPOSE 3000
EXPOSE 3001

ENV ASPNETCORE_URLS=https://*:3000

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ServiceRegistry.csproj", "."]
RUN dotnet restore "./ServiceRegistry.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "ServiceRegistry.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ServiceRegistry.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServiceRegistry.dll"]