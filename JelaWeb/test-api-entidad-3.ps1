# Test del API con IdEntidad 3

$apiUrl = "https://jela-api-ctb8a6ggbpdqbxhg.mexicocentral-01.azurewebsites.net/api/webhooks/chatweb"

$payload = @{
    Nombre = "Administrador de Condominios"
    Email = "usuario5@jelaweb.com"
    Mensaje = "Prueba con entidad 3"
    IPOrigen = "127.0.0.1"
    IdEntidad = 3
    SessionId = "test_session_123"
} | ConvertTo-Json

Write-Host "Enviando payload con IdEntidad: 3" -ForegroundColor Cyan
Write-Host $payload -ForegroundColor Yellow

try {
    $response = Invoke-RestMethod -Uri $apiUrl -Method Post -Body $payload -ContentType "application/json"
    Write-Host "`nRespuesta exitosa:" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 10
} catch {
    Write-Host "`nError:" -ForegroundColor Red
    Write-Host "StatusCode:" $_.Exception.Response.StatusCode.value__
    Write-Host "StatusDescription:" $_.Exception.Response.StatusDescription
    
    # Intentar leer el cuerpo de la respuesta de error
    $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
    $reader.BaseStream.Position = 0
    $reader.DiscardBufferedData()
    $responseBody = $reader.ReadToEnd()
    Write-Host "`nDetalle del error:" -ForegroundColor Yellow
    Write-Host $responseBody
}

# Ahora probar con IdEntidad 1 para comparar
Write-Host "`n`n========================================" -ForegroundColor Cyan
Write-Host "Probando con IdEntidad: 1 para comparar" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

$payload2 = @{
    Nombre = "Administrador de Condominios"
    Email = "usuario5@jelaweb.com"
    Mensaje = "Prueba con entidad 1"
    IPOrigen = "127.0.0.1"
    IdEntidad = 1
    SessionId = "test_session_456"
} | ConvertTo-Json

Write-Host "Enviando payload con IdEntidad: 1" -ForegroundColor Cyan
Write-Host $payload2 -ForegroundColor Yellow

try {
    $response2 = Invoke-RestMethod -Uri $apiUrl -Method Post -Body $payload2 -ContentType "application/json"
    Write-Host "`nRespuesta exitosa:" -ForegroundColor Green
    $response2 | ConvertTo-Json -Depth 10
} catch {
    Write-Host "`nError:" -ForegroundColor Red
    Write-Host "StatusCode:" $_.Exception.Response.StatusCode.value__
    Write-Host "StatusDescription:" $_.Exception.Response.StatusDescription
    
    $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
    $reader.BaseStream.Position = 0
    $reader.DiscardBufferedData()
    $responseBody = $reader.ReadToEnd()
    Write-Host "`nDetalle del error:" -ForegroundColor Yellow
    Write-Host $responseBody
}
