if [ -z "$1" ]; then
  echo "❗Please provide a migration name. Example: ./add-migration.sh AddUsersTable"
  exit 1
fi

dotnet ef migrations add "$1" \
  --project ./FoxTales.Infrastructure \
  --startup-project ./FoxTales.Api \
  --output-dir Persistence/Migrations
