FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
COPY ["node/src/Node.Service/Node.Service.csproj", "node/src/Node.Service/"]
COPY ["node/src/Node.Core/Node.Core.csproj", "node/src/Node.Core/"]
COPY ["shared/RemoteBrowser.Contracts/RemoteBrowser.Contracts.csproj", "shared/RemoteBrowser.Contracts/"]
RUN dotnet restore "node/src/Node.Service/Node.Service.csproj"
COPY . .
WORKDIR "/app/node/src/Node.Service"
RUN dotnet build "Node.Service.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/app/node/src/Node.Service"
RUN dotnet publish "Node.Service.csproj" -c Release -o /app/publish/

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Node.Service.dll"]