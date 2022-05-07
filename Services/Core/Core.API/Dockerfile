#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
RUN apt-get update && apt-get install -y libgdiplus
#RUN apt-get update && apt-get -y install xvfb && apt-get -y install fontconfig && apt-get -y install libssl1.0-dev && apt-get -y install libx11-dev libx11-xcb-dev libxcb-icccm4-dev libxcb-image0-dev libxcb-keysyms1-dev libxcb-randr0-dev libxcb-render-util0-dev libxcb-render0-dev libxcb-shm0-dev libxcb-util0-dev libxcb-xfixes0-dev libxcb-xkb-dev libxcb1-dev libxfixes-dev libxrandr-dev libxrender-dev
WORKDIR /app
EXPOSE 80 2222
EXPOSE 443

#ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Services/Core/Core.API/Core.API.csproj", "Services/Core/Core.API/"]
COPY ["BuildingBlocks/Common.Models/Common.Models.csproj", "BuildingBlocks/Common.Models/"]
COPY ["BuildingBlocks/Common.Validations/Common.Validations.csproj", "BuildingBlocks/Common.Validations/"]
COPY ["BuildingBlocks/Common.Identity/Common.Identity.csproj", "BuildingBlocks/Common.Identity/"]
COPY ["BuildingBlocks/Common.Extensions/Common.Extensions/Common.Extensions.csproj", "BuildingBlocks/Common.Extensions/Common.Extensions/"]
COPY ["BuildingBlocks/Common.Authorization/Common.Authorization.csproj", "BuildingBlocks/Common.Authorization/"]
COPY ["BuildingBlocks/Common.EventMessages/Common.EventMessages.csproj", "BuildingBlocks/Common.EventMessages/"]
COPY ["BuildingBlocks/Common.AzureBlobUtility/Common.AzureBlobUtility.csproj", "BuildingBlocks/Common.AzureBlobUtility/"]
COPY ["BuildingBlocks/Common.Notifications/Common.Notifications.csproj", "BuildingBlocks/Common.Notifications/"]
COPY ["BuildingBlocks/Common.Messages/Common.Messages/Common.Messages.csproj", "BuildingBlocks/Common.Messages/Common.Messages/"]
COPY ["BuildingBlocks/Common.Payments/Common.Payments.csproj", "BuildingBlocks/Common.Payments/"]
RUN dotnet restore "Services/Core/Core.API/Core.API.csproj"
COPY . .
WORKDIR "/src/Services/Core/Core.API"
RUN dotnet build "Core.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Core.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Core.API.dll"]