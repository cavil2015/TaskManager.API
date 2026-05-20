# Стадия 1: базовый образ для запуска (легковесный runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Стадия 2: сборка проекта
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TaskManager.API.csproj", "./"]
RUN dotnet restore "TaskManager.API.csproj"
COPY . .
RUN dotnet build "TaskManager.API.csproj" -c Release -o /app/build

# Стадия 3: публикация
FROM build AS publish
RUN dotnet publish "TaskManager.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Стадия 4: финальный образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskManager.API.dll"]
