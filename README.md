# TaskManager Interview Project

## Design

![Design](./img.png)

## Running the Project (Frontend + Backend + Database)

```bash
docker-compose build
docker-compose up
```

### IMPORTANT

The first time you run `docker-compose`, it may fail because the database might not be ready.  
I didn’t have time to add proper health checks in the `docker-compose.yml` file.  
Simply re-run the command and it should work.

To start the frontend:

```bash
npm install && npm run dev
```

You will be able to access the application at: [http://localhost:3000](http://localhost:3000)

---

## Running the Backend Locally

### Prerequisites

Make sure the following are installed on your local machine:

- [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download)
- .NET Runtime 8.0
- PostgreSQL
- `dotnet-ef` tool

Install commands for Ubuntu:

```bash
sudo apt-get update && sudo apt-get install -y dotnet-sdk-8.0
sudo apt-get install -y dotnet-runtime-8.0
dotnet tool install --global dotnet-ef
```

---

### Run Migrations

```bash
dotnet ef migrations add YourMigrationName --project ./TaskManager.Infrastructure --startup-project ./TaskManager.WebApi
```

---

### Running the Application Locally

Make sure to set your connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=taskmanager;Username=postgres;Password=postgres"
}
```

To run the backend:

```bash
dotnet run --project ./TaskManager.WebApi/
```

---

### Building the Application Locally

```bash
dotnet build TaskManager.sln
```

---

## Notes

- API documentation is available at: [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html)
- You can create a user via the Swagger endpoint `POST /api/Users`. The frontend for this feature is not implemented due to time constraints.
- While `docker-compose` works, you may encounter CORS errors when fetching data from the UI. This might be due to configuration issues in Docker or the code itself.  
  **Recommendation:** Run the frontend and backend separately during development.
- Test cases were not included due to a strict 4-day deadline. I suggest using **xUnit** and **Moq** for testing.
- Consider using **FluentValidation** to validate DTOs and entities for better code clarity and maintainability.
- Adding a cache layer for user retrieval might improve performance and reduce database hits.