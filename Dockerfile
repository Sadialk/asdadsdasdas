FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /Source

# copy csproj and restore as distinct layers
COPY Source/Testing/*.csproj .
RUN dotnet restore -r linux-musl-x64 /p:PublishReadyToRun=true

# copy everything else and build app
COPY Source/Testing/. .
RUN dotnet publish -c Release -o /app -r linux-musl-x64 --self-contained true /p:PublishReadyToRun=true /p:PublishSingleFile=true

FROM mcr.microsoft.com/dotnet/runtime-deps:7.0-alpine-amd64
WORKDIR /app
COPY --from=build /app .