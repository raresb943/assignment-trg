FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
COPY ["dispatcher/src/Dispatcher.Api/Dispatcher.Api.csproj", "dispatcher/src/Dispatcher.Api/"]
COPY ["dispatcher/src/Dispatcher.Core/Dispatcher.Core.csproj", "dispatcher/src/Dispatcher.Core/"]
COPY ["shared/RemoteBrowser.Contracts/RemoteBrowser.Contracts.csproj", "shared/RemoteBrowser.Contracts/"]
RUN dotnet restore "dispatcher/src/Dispatcher.Api/Dispatcher.Api.csproj"
COPY . .
WORKDIR "/app/dispatcher/src/Dispatcher.Api"
RUN dotnet build "Dispatcher.Api.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/app/dispatcher/src/Dispatcher.Api"
RUN dotnet publish "Dispatcher.Api.csproj" -c Release -o /app/publish/

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dispatcher.Api.dll"]