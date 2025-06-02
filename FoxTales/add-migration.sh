if [ ! -f "./FoxTales.Api/FoxTales.Api.csproj" ] && [ ! -f "./FoxTales.sln" ]; then
  echo "❗Please run this script from the solution root directory"
  exit 1
fi

if [ -z "$1" ]; then
  echo "❗Please provide a migration name. Example: ./add-migration.sh AddUsersTable"
  exit 1
fi

dotnet ef migrations add "$1" \
  --project ./FoxTales.Infrastructure \
  --startup-project ./FoxTales.Api \
  --output-dir Migrations