# MarketSphere ConvertX — Local Run (No Docker)

## Prerequisites
- .NET 9 SDK
- Node.js 22 LTS / npm
- Microsoft SQL Server (Developer/Express/LocalDB-capable SQL Server instance)

The default backend connection string is:

`Server=.;Database=MarketSphereConvertXDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True`

That expects a local default SQL Server instance using Windows Authentication. If your instance is `SQLEXPRESS`, change `Server=.` to `Server=.\\SQLEXPRESS` in `backend/MarketSphere.Api/appsettings.json`.

## 1. Configure development secrets once
From the repository root on Windows:

```bat
setup-local.cmd
```

Enter a local admin password when prompted.

Login email: `admin@marketsphere.local`

## 2. Start everything

```bat
start-local.cmd
```

This opens three terminals:
- Backend API: http://localhost:5080
- Swagger: http://localhost:5080/swagger
- Angular Operations: http://localhost:4200
- React Management: http://localhost:5173

On first backend startup, EF Core applies the existing SQL Server migrations and runs the seeders automatically in Development.

## Manual commands

### Backend
```bat
cd backend
dotnet restore MarketSphereConvertX.sln
dotnet run --project MarketSphere.Api\MarketSphere.Api.csproj --launch-profile http
```

### Angular frontend
```bat
cd frontend\angular-operations
npm install
npm start
```

### React frontend
```bat
cd frontend\react-management
npm install
npm run dev
```
