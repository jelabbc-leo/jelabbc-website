# Script rápido para republicar JELA.API a Azure
# Usa el perfil de publicación existente de Visual Studio

Write-Host "=== Republicando JELA.API a Azure ===" -ForegroundColor Green

# Cambiar al directorio del proyecto
$projectPath = Join-Path $PSScriptRoot "JELA.API"
Set-Location $projectPath

# Compilar y publicar usando el perfil de Visual Studio
Write-Host "Compilando y publicando..." -ForegroundColor Yellow
dotnet publish -c Release /p:PublishProfile="JELA-API - Web Deploy"

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n=== Publicación completada exitosamente ===" -ForegroundColor Green
    Write-Host "URL de la API: https://jela-api-ctb8a6ggbpdqbxhg.mexicocentral-01.azurewebsites.net" -ForegroundColor Cyan
    Write-Host "Swagger: https://jela-api-ctb8a6ggbpdqbxhg.mexicocentral-01.azurewebsites.net/swagger" -ForegroundColor Cyan
    Write-Host "`nLa configuración TicketValidation:ValidarClientesDuplicados ya está en appsettings.json" -ForegroundColor Yellow
    Write-Host "Ahora puedes probar el widget sin límite de mensajes." -ForegroundColor Green
} else {
    Write-Host "`nError al publicar. Verifica los logs arriba." -ForegroundColor Red
}
