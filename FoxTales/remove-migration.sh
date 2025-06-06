if [ ! -f "./FoxTales.Api/FoxTales.Api.csproj" ] && [ ! -f "./FoxTales.sln" ]; then
  echo "❗Please run this script from the solution root directory"
  exit 1
fi

echo "Removing the last migration..."
dotnet ef migrations remove \
  --project ./FoxTales.Infrastructure \
  --startup-project ./FoxTales.Api \
  --verbose

if [ $? -eq 0 ]; then
  echo "✅ Last migration removed successfully"
else
  echo "❗Failed to remove migration"
  exit 1
fi