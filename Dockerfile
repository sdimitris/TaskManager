# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the .csproj file and restore dependencies
COPY *.sln .
COPY TaskManager.Domain/*.csproj ./TaskManager.Domain/
COPY TaskManager.Application/*.csproj ./TaskManager.Application/
COPY TaskManager.Infrastructure/*.csproj ./TaskManager.Infrastructure/
COPY TaskManager.WebApi/*.csproj ./TaskManager.WebApi/
#COPY TaskManager.Tests/*.csproj ./TaskManager.Tests/
RUN dotnet restore

# Copy the rest of the application code
COPY TaskManager.Domain/. ./TaskManager.Domain/
COPY TaskManager.Application/. ./TaskManager.Application/
COPY TaskManager.Infrastructure/. ./TaskManager.Infrastructure/
COPY TaskManager.WebApi/. ./TaskManager.WebApi/
#COPY TaskManager.Tests/. ./TaskManager.Tests/


# Build the application
RUN dotnet publish TaskManager.WebApi/TaskManager.WebApi.csproj -c Release -o out

# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Set environment variable to ensure ASP.NET Core listens on port 8080
ENV ASPNETCORE_URLS=http://+:8080

# Copy the built application from the build stage
COPY --from=build /app/out .

# Expose the port the application runs on
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "TaskManager.WebApi.dll"]
