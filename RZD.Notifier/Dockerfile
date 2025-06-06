#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RZD.Notifier/RZD.Notifier.csproj", "RZD.Notifier/"]
COPY ["RZD.Application/RZD.Application.csproj", "RZD.Application/"]
COPY ["RZD.Database/RZD.Database.csproj", "RZD.Database/"]
COPY ["RZD.Common/RZD.Common.csproj", "RZD.Common/"]
COPY ["RZD.Integration/RZD.Integration.csproj", "RZD.Integration/"]
COPY ["RZD.MailSender/RZD.MailSender.csproj", "RZD.MailSender/"]
RUN dotnet restore "./RZD.Notifier/./RZD.Notifier.csproj"
COPY . .
WORKDIR "/src/RZD.Notifier"
RUN dotnet build "./RZD.Notifier.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RZD.Notifier.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RZD.Notifier.dll"]