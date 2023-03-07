FROM docker.io/alpine:3.17 AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
RUN apk --no-cache add postgresql15-client aspnetcore7-runtime
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /src
COPY ["pg-backup.csproj", "./"]
RUN dotnet restore "pg-backup.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "pg-backup.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "pg-backup.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "pg-backup.dll"]
