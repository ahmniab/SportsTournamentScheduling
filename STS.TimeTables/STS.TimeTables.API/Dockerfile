FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY STS.TimeTables/STS.TimeTables.API/STS.TimeTables.API.csproj STS.TimeTables/STS.TimeTables.API/
COPY STS.TimeTables/STS.TimeTables.Application/STS.TimeTables.Application.csproj STS.TimeTables/STS.TimeTables.Application/
COPY STS.TimeTables/STS.TimeTables.Infrastructure/STS.TimeTables.Infrastructure.csproj STS.TimeTables/STS.TimeTables.Infrastructure/
COPY STS.TimeTables/STS.TimeTables.Domain/STS.TimeTables.Domain.csproj STS.TimeTables/STS.TimeTables.Domain/
RUN dotnet restore STS.TimeTables/STS.TimeTables.API/STS.TimeTables.API.csproj

COPY STS.TimeTables/ STS.TimeTables/
RUN dotnet publish STS.TimeTables/STS.TimeTables.API/STS.TimeTables.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
RUN apt-get update \
    && apt-get install -y --no-install-recommends netcat-openbsd \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .

RUN useradd --create-home --shell /usr/sbin/nologin appuser
USER appuser

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 \
  CMD nc -z 127.0.0.1 8080 || exit 1

ENTRYPOINT ["dotnet", "STS.TimeTables.API.dll"]
