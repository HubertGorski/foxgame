IMAGE_NAME="backend-foxtales"
TAR_FILE="$PWD/$IMAGE_NAME.tar"
PROJECT_DIR="$PWD"

VPS_USER=$(grep -w VPS_USER ../../.env | cut -d '=' -f2)
VPS_IP=$(grep -w VPS_IP ../../.env | cut -d '=' -f2)
VPS_FOLDER=$(grep -w VPS_FOLDER ../../.env | cut -d '=' -f2)

echo "Buduję nowy obraz $IMAGE_NAME ..."
docker build -t $IMAGE_NAME ../../.

echo "Zapisuję obraz $IMAGE_NAME do $TAR_FILE ..."
docker save -o "$TAR_FILE" "$IMAGE_NAME"

echo "Tworzę folder $VPS_FOLDER na VPS ..."
ssh "$VPS_USER@$VPS_IP" "mkdir -p $VPS_FOLDER"

echo "Wysyłam plik obrazu na VPS ..."
scp "$TAR_FILE" "$VPS_USER@$VPS_IP:$VPS_FOLDER/"

if [ -f "$PROJECT_DIR/../../docker-compose.yml" ]; then
    echo "Wysyłam docker-compose.yml ..."
    scp "$PROJECT_DIR/../../docker-compose.yml" "$VPS_USER@$VPS_IP:$VPS_FOLDER/"
else
    echo "Nie znaleziono docker-compose.yml w $PROJECT_DIR!"
    exit 1
fi

if [ -f "$PROJECT_DIR/../../.env" ]; then
    echo "Wysyłam plik .env ..."
    scp "$PROJECT_DIR/../../.env" "$VPS_USER@$VPS_IP:$VPS_FOLDER/"
else
    echo "Nie znaleziono pliku .env w $PROJECT_DIR!"
    exit 1
fi

echo "Ładuję obraz na VPS ..."
ssh "$VPS_USER@$VPS_IP" "docker load -i $VPS_FOLDER/$IMAGE_NAME.tar"

echo "Usuwam plik tar na VPS ..."
ssh "$VPS_USER@$VPS_IP" "rm -f $VPS_FOLDER/$IMAGE_NAME.tar"

echo "Usuwam plik tar lokalnie ..."
rm -f "$TAR_FILE"

echo "Uruchamiam tylko SQL Server na VPS ..."
ssh "$VPS_USER@$VPS_IP" "cd $VPS_FOLDER && docker-compose up -d sqlserver-foxtales"

echo "Czekam aż SQL Server będzie gotowy..."
sleep 20

echo "Wykonuję migracje bazy danych..."
ssh "$VPS_USER@$VPS_IP" "cd $VPS_FOLDER && docker-compose run --rm --entrypoint dotnet backend-foxtales FoxTales.Api.dll ef database update"

echo "Uruchamiam resztę serwisów..."
ssh "$VPS_USER@$VPS_IP" "cd $VPS_FOLDER && docker-compose up -d"

echo "✅ Deploy zakończony!"