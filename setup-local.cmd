@echo off
setlocal
cd /d %~dp0

where dotnet >nul 2>nul || (
  echo [ERROR] .NET SDK was not found. Install .NET 9 SDK first.
  pause
  exit /b 1
)

set "API_PROJECT=backend\MarketSphere.Api\MarketSphere.Api.csproj"
if not exist "%API_PROJECT%" (
  echo [ERROR] Could not find %API_PROJECT%.
  echo Run this script from the extracted MarketSphere_ConvertX root folder.
  pause
  exit /b 1
)

echo MarketSphere ConvertX - Local Development Setup
echo ------------------------------------------------
echo Secrets are stored using .NET User Secrets, NOT in appsettings.json.
echo.

set /p ADMIN_PASSWORD=Enter a LOCAL bootstrap admin password: 
if "%ADMIN_PASSWORD%"=="" (
  echo [ERROR] Password cannot be empty.
  pause
  exit /b 1
)

for /f %%i in ('powershell -NoProfile -Command "[guid]::NewGuid().ToString('N') + [guid]::NewGuid().ToString('N')"') do set JWT_KEY=%%i

if "%JWT_KEY%"=="" (
  echo [ERROR] Could not generate a JWT signing key.
  pause
  exit /b 1
)

dotnet user-secrets set "Jwt:SigningKey" "%JWT_KEY%" --project "%API_PROJECT%"
if errorlevel 1 (
  echo [ERROR] Failed to save Jwt:SigningKey.
  pause
  exit /b 1
)

dotnet user-secrets set "BootstrapAdmin:Password" "%ADMIN_PASSWORD%" --project "%API_PROJECT%"
if errorlevel 1 (
  echo [ERROR] Failed to save BootstrapAdmin:Password.
  pause
  exit /b 1
)

echo.
echo [OK] Local secrets configured successfully.
echo Login email: admin@marketsphere.local
echo Password: the password you just entered.
echo.
echo Next step: run start-local.cmd or start the MarketSphere.Api http profile in Visual Studio.
echo.
pause
endlocal
