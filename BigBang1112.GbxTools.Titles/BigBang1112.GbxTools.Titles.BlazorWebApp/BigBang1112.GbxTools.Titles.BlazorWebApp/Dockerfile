FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build

RUN apt-get update && apt-get install -y --no-install-recommends git python3 && rm -rf /var/lib/apt/lists/*

RUN dotnet workload install wasm-tools

WORKDIR /src

COPY . .
WORKDIR /src/BigBang1112.GbxTools.Titles.BlazorWebApp/

ARG TARGETARCH

RUN --mount=type=cache,id=nuget-${TARGETARCH},target=/root/.nuget/packages \
    dotnet publish -a ${TARGETARCH} --self-contained false -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

COPY --from=build /app .

RUN mkdir -p /home/app/.aspnet/DataProtection-Keys \
    && chown -R $APP_UID /home/app/.aspnet

USER $APP_UID
ENTRYPOINT ["dotnet", "BigBang1112.GbxTools.Titles.BlazorWebApp.dll"]