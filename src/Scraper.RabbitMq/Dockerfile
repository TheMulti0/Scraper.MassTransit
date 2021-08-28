#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Scraper.RabbitMq/Scraper.RabbitMq.csproj", "Scraper.RabbitMq/"]
COPY ["Scraper.RabbitMq.Common/Scraper.RabbitMq.Common.csproj", "Scraper.RabbitMq.Common/"]
RUN dotnet restore "Scraper.RabbitMq/Scraper.RabbitMq.csproj"
COPY . .
WORKDIR "/src/Scraper.RabbitMq"
RUN dotnet build "Scraper.RabbitMq.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Scraper.RabbitMq.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=build /src/Scraper.RabbitMq/requirements.txt .
RUN apt-get update -y && apt-get install python3-pip tor -y 
RUN pip3 install --upgrade pip
RUN pip3 install -r "requirements.txt"

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Scraper.RabbitMq.dll"]