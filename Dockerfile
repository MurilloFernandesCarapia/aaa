
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY OrbitalGuard.API/OrbitalGuard.API.csproj OrbitalGuard.API/
RUN dotnet restore OrbitalGuard.API/OrbitalGuard.API.csproj

COPY OrbitalGuard.API/ OrbitalGuard.API/
WORKDIR /src/OrbitalGuard.API
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true \
    TZ=America/Sao_Paulo


RUN groupadd --system --gid 1001 orbitalgroup \
 && useradd  --system --uid 1001 --gid orbitalgroup --shell /bin/bash --home /home/orbitalapp --create-home orbitalapp


WORKDIR /app

COPY --from=build /app/publish .
RUN chown -R orbitalapp:orbitalgroup /app

USER orbitalapp

EXPOSE 8080

ENTRYPOINT ["dotnet", "OrbitalGuard.API.dll"]