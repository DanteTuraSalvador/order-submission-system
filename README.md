# Order Submission System

ASP.NET Web API application for ingesting customer orders, validating the payload, and routing successfully processed orders to the configured downstream processor (SQL database or FTP file delivery). The solution includes infrastructure services such as metrics, logging, and background dependencies that can be launched via Docker.

## Technology Stack
- **Runtime:** .NET Framework 4.7.2 (ASP.NET Web API)
- **Dependency Injection:** Unity
- **Logging:** Serilog (Seq sink optional)
- **API Documentation:** Swashbuckle (Swagger UI)
- **Metrics:** prometheus-net
- **Domain & Infrastructure:** Custom order validation, SQL/FTP processors, CSV/JSON/XML/Excel formatters
- **Testing:** MSTest projects (see `tests/`)
- **Optional Services (Docker Compose):** Redis, Seq, Prometheus, Grafana, FTP server, Azure SQL Edge (disabled by default in current workflow)

## Prerequisites
- Windows 10/11 with **Visual Studio 2022** (or VS Build Tools) that supports .NET Framework 4.7.2
- **IIS Express** (installed with Visual Studio) for local hosting
- **SQL Server LocalDB** (installed automatically with Visual Studio)
- **Docker Desktop** (optional) for running supporting services such as Seq, Prometheus, Grafana, FTP
- PowerShell 5+ or VS Code integrated terminal

## Getting Started

### 1. Clone & Restore
```powershell
git clone https://github.com/DanteTuraSalvador/order-submission-system.git
cd order-submission-system
```
- Open `OrderSubmissionSystem.sln` in Visual Studio (NuGet packages restore automatically), **or**
- Restore manually: `nuget restore OrderSubmissionSystem.sln`

### 2. Database Setup (LocalDB)
The API currently persists orders to LocalDB. Run the script below **once** to create the database and tables.

```powershell
$schema = @'
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'Orders')
BEGIN
    EXEC('CREATE DATABASE Orders');
END;
GO
USE Orders;
GO
IF OBJECT_ID('dbo.Orders', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Orders (
        OrderId     NVARCHAR(64) NOT NULL PRIMARY KEY,
        CustomerId  NVARCHAR(64) NOT NULL,
        TotalAmount DECIMAL(18,2) NOT NULL,
        OrderDate   DATETIME2     NOT NULL
    );
END;
IF OBJECT_ID('dbo.OrderItems', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.OrderItems (
        Id         INT IDENTITY(1,1) PRIMARY KEY,
        OrderId    NVARCHAR(64) NOT NULL FOREIGN KEY REFERENCES dbo.Orders(OrderId),
        ProductId  NVARCHAR(64) NOT NULL,
        Quantity   INT NOT NULL,
        UnitPrice  DECIMAL(18,2) NOT NULL
    );
END;
GO
'@
$temp = New-TemporaryFile
Set-Content -Path $temp -Value $schema -Encoding UTF8
sqlcmd -S "(localdb)\MSSQLLocalDB" -i $temp
Remove-Item $temp
```

The API configuration (`src/OrderSubmissionSystem/OrderSubmissionSystem.Api/secrets.connectionStrings.config`) points to `Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=Orders`.

### 3. Running the API

**Visual Studio**
1. Set `OrderSubmissionSystem.Api` as the startup project.
2. Choose the IIS Express profile.
3. Press `F5` (or `Ctrl+F5`).

**VS Code / PowerShell**
```powershell
# Build
& "C:\Program Files\Microsoft Visual Studio\2022\Preview\MSBuild\Current\Bin\MSBuild.exe" `
  src\OrderSubmissionSystem\OrderSubmissionSystem.Api\OrderSubmissionSystem.Api.csproj `
  /p:Configuration=Debug

# Run via IIS Express
& "C:\Program Files\IIS Express\iisexpress.exe" `
  /path:"C:\path\to\order-submission-system\src\OrderSubmissionSystem\OrderSubmissionSystem.Api" `
  /port:5000
```

### 4. Exercising the API
- Swagger UI: `http://localhost:5000/swagger/ui/index`
- Health check: `GET http://localhost:5000/api/orders/health`
- Submit order example:
  ```bash
  curl -X POST http://localhost:5000/api/orders \
    -H "Content-Type: application/json" \
    -H "X-API-Key: 6f5c9a41-d7b2-4c3e-9f1a-6d8f7b3e2c19" \
    -d '{
      "OrderId": "ORD-3001",
      "CustomerId": "CUST-9001",
      "TotalAmount": 159.97,
      "OrderDate": "2025-11-03T13:49:00Z",
      "Items": [
        { "ProductId": "SKU-101", "Quantity": 1, "UnitPrice": 59.99 },
        { "ProductId": "SKU-202", "Quantity": 2, "UnitPrice": 49.99 }
      ]
    }'
  ```

> The API requires a valid key in header `X-API-KEY`. A development key is stored at `App_Data/api-keys.json`.

### 5. Optional Docker Services
To start supporting services (logging, metrics, FTP):

```powershell
docker compose up -d redis seq prometheus grafana ftp
```

- Seq UI: `http://localhost:5341`
- Prometheus: `http://localhost:9090`
- Grafana: `http://localhost:3000` (default creds `admin` / `admin`)
- FTP server (for the FTP processor flow): credentials defined in `Web.config`/`secrets.config`

> The SQL container in `docker-compose.yml` is disabled in the current workflow because the API uses LocalDB. If you prefer a containerized SQL instance, swap the connection string and follow the documented schema script against that endpoint.

## Project Structure
- `src/OrderSubmissionSystem/OrderSubmissionSystem.Api` – Web API host, controllers, Swagger, DI bootstrap
- `src/OrderSubmissionSystem.Application` – Application services, validation, formatter/uploader abstractions
- `src/OrderSubmissionSystem.Infrastructure` – Concrete processors (SQL/FTP), uploaders, formatters
- `src/OrderSubmissionSystem.Domain` – Domain entities
- `tests/` – Unit tests
- `docker-compose.yml` – Optional supporting services

## Testing
Run tests from Visual Studio Test Explorer or via command line (requires vstest.console.exe):
```powershell
& "C:\Program Files (x86)\Microsoft Visual Studio\2022\Preview\Common7\IDE\Extensions\TestPlatform\vstest.console.exe" `
  tests\OrderSubmissionSystem.UnitTests\bin\Debug\OrderSubmissionSystem.UnitTests.dll
```

## Troubleshooting
- **API returns 500 (Parser Error)** – Ensure the project was built before launching IIS Express.
- **SQL duplicate key error** – Orders are keyed by `OrderId`. Use unique IDs or delete existing rows from `dbo.Orders` / `dbo.OrderItems`.
- **FTP upload failures** – Confirm the FTP container/service is running and credentials match `secrets.config`.


## License
Internal project – no public license specified.
