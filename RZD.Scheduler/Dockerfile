#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RZD.Scheduler/RZD.Scheduler.csproj", "RZD.Scheduler/"]
COPY ["RZD.Application/RZD.Application.csproj", "RZD.Application/"]
COPY ["RZD.Integration/RZD.Integration.csproj", "RZD.Integration/"]
COPY ["RZD.Common/RZD.Common.csproj", "RZD.Common/"]
COPY ["RZD.Database/RZD.Database.csproj", "RZD.Database/"]
RUN dotnet restore "./RZD.Scheduler/./RZD.Scheduler.csproj"
COPY . .
WORKDIR "/src/RZD.Scheduler"
RUN dotnet build "./RZD.Scheduler.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RZD.Scheduler.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RZD.Scheduler.dll"]