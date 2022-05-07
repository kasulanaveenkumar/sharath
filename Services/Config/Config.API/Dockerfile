#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Services/Config/Config.API/Config.API.csproj", "Services/Config/Config.API/"]
COPY ["BuildingBlocks/Common.EventMessages/Common.EventMessages.csproj", "BuildingBlocks/Common.EventMessages/"]
COPY ["BuildingBlocks/Common.Models/Common.Models.csproj", "BuildingBlocks/Common.Models/"]
COPY ["BuildingBlocks/Common.Validations/Common.Validations.csproj", "BuildingBlocks/Common.Validations/"]
COPY ["BuildingBlocks/Common.Authorization/Common.Authorization.csproj", "BuildingBlocks/Common.Authorization/"]
COPY ["BuildingBlocks/Common.AzureBlobUtility/Common.AzureBlobUtility.csproj", "BuildingBlocks/Common.AzureBlobUtility/"]
COPY ["BuildingBlocks/Common.Extensions/Common.Extensions/Common.Extensions.csproj", "BuildingBlocks/Common.Extensions/Common.Extensions/"]
COPY ["BuildingBlocks/Common.Notifications/Common.Notifications.csproj", "BuildingBlocks/Common.Notifications/"]
COPY ["BuildingBlocks/Common.Messages/Common.Messages/Common.Messages.csproj", "BuildingBlocks/Common.Messages/Common.Messages/"]
COPY ["BuildingBlocks/Common.Payments/Common.Payments.csproj", "BuildingBlocks/Common.Payments/"]
RUN dotnet restore "Services/Config/Config.API/Config.API.csproj"
COPY . .
WORKDIR "/src/Services/Config/Config.API"
RUN dotnet build "Config.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Config.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Config.API.dll"]