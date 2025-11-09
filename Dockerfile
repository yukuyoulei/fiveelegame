# Five Elements Game Server Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/FiveElements.Shared/FiveElements.Shared.csproj", "src/FiveElements.Shared/"]
COPY ["src/FiveElements.Server/FiveElements.Server.csproj", "src/FiveElements.Server/"]
RUN dotnet restore "src/FiveElements.Server/FiveElements.Server.csproj"
COPY . .
WORKDIR "/src/src/FiveElements.Server"
RUN dotnet build "FiveElements.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FiveElements.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FiveElements.Server.dll"]