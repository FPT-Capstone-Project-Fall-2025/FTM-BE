FROM mcr.microsoft.com/dotnet/aspnet:7.0 as base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln ./
COPY FTM.API/*.csproj ./FTM.API/
COPY FTM.Application/*.csproj ./FTM.Application/
COPY FTM.Infrastructure/*.csproj ./FTM.Infrastructure/
COPY FTM.Domain/*.csproj ./FTM.Domain/
COPY FTM.Tests/*.csproj ./FTM.Tests/
COPY Directory.Packages.props .

RUN dotnet restore

# Copy everything else and build
COPY FTM.API/. ./FTM.API/
COPY FTM.Application/. ./FTM.Application/
COPY FTM.Infrastructure/. ./FTM.Infrastructure/
COPY FTM.Domain/. ./FTM.Domain/
COPY FTM.Tests/. ./FTM.Tests/

WORKDIR /app/FTM.API

RUN dotnet publish -c Release -o out

# Build runtime image
FROM base AS runtime
WORKDIR /app
COPY --from=build-env /app/FTM.API/out .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "FTM.API.dll", "--launch-profile API-dev"]
