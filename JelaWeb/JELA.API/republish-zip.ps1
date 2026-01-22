# Script para republicar JELA.API usando ZIP deployment
# Método más simple que no requiere credenciales de Web Deploy

Write-Host "=== Republicando JELA.API a Azure (ZIP Deployment) ===" -ForegroundColor Green

# Variables
$appName = "jela-api-ctb8a6ggbpdqbxhg"
$resourceGroup = "jela-resources"
$projectPath = Join-Path $PSScriptRoot "JELA.API\JELA.API.csproj"
$publishPath = Join-Path $PSScriptRoot "publish"
$zipPath = Join-Path $PSScriptRoot "publish.zip"

# Verificar que Azure CLI esté instalado
if (-not (Get-Command az -ErrorAction SilentlyContinue)) {
    Write-Host "Error: Azure CLI no está instalado." -ForegroundColor Red
    Write-Host "Instálalo desde: https://aka.ms/installazurecliwindows" -ForegroundColor Yellow
    exit 1
}

# Verificar login
Write-Host "Verificando autenticación en Azure..." -ForegroundColor Yellow
$account = az account show 2>$null | ConvertFrom-Json
if (-not $account) {
    Write-Host "No estás autenticado. Iniciando login..." -ForegroundColor Yellow
    az login
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Error al autenticar." -ForegroundColor Red
        exit 1
    }
}

Write-Host "Autenticado como: $($account.user.name)" -ForegroundColor Green

# Limpiar carpetas anteriores
if (Test-Path $publishPath) {
    Write-Host "Limpiando carpeta de publicación anterior..." -ForegroundColor Yellow
    Remove-Item -Path $publishPath -Recurse -Force
}
if (Test-Path $zipPath) {
    Remove-Item -Path $zipPath -Force
}

# Compilar proyecto
Write-Host "Compilando proyecto en modo Release..." -ForegroundColor Yellow
dotnet publish $projectPath -c Release -o $publishPath

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error al compilar el proyecto." -ForegroundColor Red
    exit 1
}

Write-Host "Compilación exitosa." -ForegroundColor Green

# Crear ZIP
Write-Host "Creando archivo ZIP..." -ForegroundColor Yellow
Compress-Archive -Path "$publishPath\*" -DestinationPath $zipPath -Force

if (-not (Test-Path $zipPath)) {
    Write-Host "Error al crear el archivo ZIP." -ForegroundColor Red
    exit 1
}

$zipSize = (Get-Item $zipPath).Length / 1MB
Write-Host "ZIP creado: $([math]::Round($zipSize, 2)) MB" -ForegroundColor Green

# Desplegar a Azure
Write-Host "Desplegando a Azure App Service..." -ForegroundColor Yellow
Write-Host "Esto puede tomar 1-2 minutos..." -ForegroundColor Cyan

az webapp deployment source config-zip `
    --resource-group $resourceGroup `
    --name $appName `
    --src $zipPath

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error al desplegar." -ForegroundColor Red
    exit 1
}

# Limpiar archivos temporales
Write-Host "Limpiando archivos temporales..." -ForegroundColor Yellow
Remove-Item -Path $zipPath -Force -ErrorAction SilentlyContinue
Remove-Item -Path $publishPath -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "`n=== Publicación completada exitosamente ===" -ForegroundColor Green
Write-Host "URL de la API: https://$appName.mexicocentral-01.azurewebsites.net" -ForegroundColor Cyan
Write-Host "Swagger: https://$appName.mexicocentral-01.azurewebsites.net/swagger" -ForegroundColor Cyan
Write-Host "`nCambios aplicados:" -ForegroundColor Yellow
Write-Host "  ✓ TicketValidation:ValidarClientesDuplicados = false (Development)" -ForegroundColor Green
Write-Host "  ✓ Ahora puedes enviar múltiples mensajes sin límite" -ForegroundColor Green
Write-Host "`nPrueba el widget ahora!" -ForegroundColor Cyan
