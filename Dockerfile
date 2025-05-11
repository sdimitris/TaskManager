FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

COPY *.sln .
COPY TaskManager.Domain/*.csproj ./TaskManager.Domain/
COPY TaskManager.Application/*.csproj ./TaskManager.Application/
COPY TaskManager.Infrastructure/*.csproj ./TaskManager.Infrastructure/
COPY TaskManager.WebApi/*.csproj ./TaskManager.WebApi/

RUN dotnet restore
COPY TaskManager.Domain/. ./TaskManager.Domain/
COPY TaskManager.Application/. ./TaskManager.Application/
COPY TaskManager.Infrastructure/. ./TaskManager.Infrastructure/
COPY TaskManager.WebApi/. ./TaskManager.WebApi/

# Run migrations to ensure they are generated
RUN dotnet ef migrations add MyMigrations --project ./TaskManager.Infrastructure --startup-project ./TaskManager.WebApi

# Publish the app, including the migrations and all relevant files
RUN dotnet publish TaskManager.WebApi/TaskManager.WebApi.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080

# Copy the published output and migrations
COPY --from=build /app/out .

# Ensure migrations folder is included
COPY --from=build /app/TaskManager.Infrastructure/Migrations /app/TaskManager.Infrastructure/Migrations

EXPOSE 8080
ENTRYPOINT ["dotnet", "TaskManager.WebApi.dll"]
