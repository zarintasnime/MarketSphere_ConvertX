#!/usr/bin/env sh
set -eu
cd "$(dirname "$0")"

echo "Starting MarketSphere ConvertX without Docker..."
echo "Ensure SQL Server is running and local user-secrets are configured first."

(cd backend && dotnet restore MarketSphereConvertX.sln && dotnet run --project MarketSphere.Api/MarketSphere.Api.csproj --launch-profile http) &
(cd frontend/angular-operations && [ -d node_modules ] || npm install; npm start) &
(cd frontend/react-management && [ -d node_modules ] || npm install; npm run dev) &
wait
