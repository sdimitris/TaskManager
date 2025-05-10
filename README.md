# TaskManager Interview Project
## Design

![Design](./img.png)

# Running the Project (Frontend + Backend + DB)

```docker-compose build```

```docker-compose up```

# Running the Backend Local

##  Prerequisites
Install dotnet sdk 8, dotnet runtime  in your local machine and dotnet-ef tool
Be sure that you have postgres installed on your machine

```sudo apt-get update && sudo apt-get install -y dotnet-sdk-8.0```

```sudo apt-get install -y dotnet-runtime-8.0```

```dotnet tool install --global dotnet-ef```

## Run Migrations

```dotnet ef migrations add YourMigrationName --project ./TaskManager.Infrastructure --startup-project ./TaskManager.WebApi```

### Running the Application Locally
Do not forget to set the connection string in the appsettings.json file

```json
  "ConnectionStrings": {
    "DefaultConnection": "Host=db;Port=5432;Database=taskmanager;Username=postgres;Password=postgres"
  },
```

```dotnet run --project .\StealAllTheCats.WebApi\```

### Building the Application Locally

```dotnet build StealAllTheCats.sln```



# NOTES

While the docker compose is working well I had some issues while fetching data from the UI (CORS ERROR) which I could not solve. I am not sure if it is a problem with the docker compose or the code itself. 
I would suggest running the backend and frontend separately to avoid this issue.