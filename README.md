# TaskManager Interview Project

##  Prerequisites
Microsoft SQL Server need to be installed on your machine.
* Create a server with name **taskmanagerdb**

* Run EF Migrations

  ```dotnet ef migrations add YourMigrationName --project .\TaskManager.Infrastructure\ --startup-project .\TaskManager.WebApi\```
* Apply Migrations

  ```dotnet ef database update --project TaskManager.Infrastructure --startup-project TaskManager.WebApi```

# Running the Application

## Local

### Running the Application Locally

```dotnet run --project .\TaskManager.WebApi\```

### Building the Application Locally

```dotnet build TaskManager.sln```

### Frontend Application

``` npm install```

```npm run dev```

## Using Docker

### Building The Application Image

```docker build -t taskmanager:latest .```

### Creating an Application Container
```docker run -d --name mssql  -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=123321' -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest ```

### Creating an Application Container
```docker run -d -p 8080:8080 taskmanager:latest```

Now you will be able to navigate at http://localhost:8080/swagger/index.html


## Running Tests

```dotnet test .\TaskManager
.Tests\TaskManager.Tests.csproj```


