FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/PostsListener/PostsListener.csproj", "PostsListener/"]
COPY ["src/Scraper.MassTransit.Common/Scraper.MassTransit.Common.csproj", "Scraper.MassTransit.Common/"]
RUN dotnet restore "Scraper.MassTransit/Scraper.MassTransit.csproj"
COPY . .
WORKDIR "/src/PostsListener"
RUN dotnet build "PostsListener.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PostsListener.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PostsListener.dll"]