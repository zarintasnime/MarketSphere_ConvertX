@echo off
setlocal
cd /d %~dp0

where dotnet >nul 2>nul || (
  echo [ERROR] .NET SDK was not found. Install .NET 9 SDK first.
  pause
  exit /b 1
)
where npm >nul 2>nul || (
  echo [ERROR] npm was not found. Install Node.js 22 LTS first.
  pause
  exit /b 1
)

if not exist "backend\MarketSphereConvertX.sln" (
  echo [ERROR] backend\MarketSphereConvertX.sln was not found.
  pause
  exit /b 1
)

echo Starting MarketSphere ConvertX locally...
echo Make sure Microsoft SQL Server is running.
echo Run setup-local.cmd once before the first backend start.
echo.

start "MarketSphere Backend" cmd /k "cd /d %~dp0backend && dotnet restore MarketSphereConvertX.sln && dotnet run --project MarketSphere.Api\MarketSphere.Api.csproj --launch-profile http"

if exist "%~dp0frontend\angular-operations\node_modules" (
  start "MarketSphere Angular" cmd /k "cd /d %~dp0frontend\angular-operations && npm start"
) else (
  start "MarketSphere Angular" cmd /k "cd /d %~dp0frontend\angular-operations && npm install && npm start"
)

if exist "%~dp0frontend\react-management\node_modules" (
  start "MarketSphere React" cmd /k "cd /d %~dp0frontend\react-management && npm run dev"
) else (
  start "MarketSphere React" cmd /k "cd /d %~dp0frontend\react-management && npm install && npm run dev"
)

echo Backend Swagger : http://localhost:5080/swagger
echo Angular Portal   : http://localhost:4200
echo React Dashboard  : http://localhost:5173
echo.
echo This project does not require Docker.
endlocal
