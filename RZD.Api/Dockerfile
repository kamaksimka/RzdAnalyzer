#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RZD.Api/RZD.Api.csproj", "RZD.Api/"]
COPY ["RZD.Integration/RZD.Integration.csproj", "RZD.Integration/"]
COPY ["RZD.Common/RZD.Common.csproj", "RZD.Common/"]
RUN dotnet restore "./RZD.Api/./RZD.Api.csproj"
COPY . .
WORKDIR "/src/RZD.Api"
RUN dotnet build "./RZD.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RZD.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RZD.Api.dll"]