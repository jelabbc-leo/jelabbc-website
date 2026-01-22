# Script para probar JELA.API desplegado en Azure
# URL: https://jela-api-ctb8a6ggbpdqbxhg.mexicocentral-01.azurewebsites.net

$apiUrl = "https://jela-api-ctb8a6ggbpdqbxhg.mexicocentral-01.azurewebsites.net"

Write-Host "=== Prueba de JELA.API en Azure ===" -ForegroundColor Green
Write-Host "URL: $apiUrl`n" -ForegroundColor Cyan

# 1. Health Check
Write-Host "1. Probando Health Endpoint..." -ForegroundColor Yellow
try {
    $health = Invoke-RestMethod -Uri "$apiUrl/api/health" -Method GET
    Write-Host "   ✓ Health: $($health.Status)" -ForegroundColor Green
    Write-Host "   ✓ Database: $($health.Checks.database.Status)" -ForegroundColor Green
    Write-Host "   ✓ API: $($health.Checks.api.Status)" -ForegroundColor Green
} catch {
    Write-Host "   ✗ Error: $_" -ForegroundColor Red
}

# 2. Version
Write-Host "`n2. Probando Version Endpoint..." -ForegroundColor Yellow
try {
    $version = Invoke-RestMethod -Uri "$apiUrl/api/version" -Method GET
    Write-Host "   ✓ Version: $($version.Version)" -ForegroundColor Green
    Write-Host "   ✓ Environment: $($version.Environment)" -ForegroundColor Green
    Write-Host "   ✓ Framework: $($version.Framework)" -ForegroundColor Green
} catch {
    Write-Host "   ✗ Error: $_" -ForegroundColor Red
}

# 3. Swagger
Write-Host "`n3. Verificando Swagger..." -ForegroundColor Yellow
try {
    $swagger = Invoke-WebRequest -Uri "$apiUrl/swagger/index.html" -Method GET -UseBasicParsing
    if ($swagger.StatusCode -eq 200) {
        Write-Host "   ✓ Swagger disponible: $apiUrl/swagger" -ForegroundColor Green
    }
} catch {
    Write-Host "   ✗ Error: $_" -ForegroundColor Red
}

# 4. Login (requiere credenciales)
Write-Host "`n4. Probando Login Endpoint..." -ForegroundColor Yellow
$username = Read-Host "   Ingresa username (o Enter para omitir)"
if ($username) {
    $password = Read-Host "   Ingresa password" -AsSecureString
    $plainPassword = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
        [Runtime.InteropServices.Marshal]::SecureStringToBSTR($password)
    )
    
    try {
        $loginBody = @{
            username = $username
            password = $plainPassword
        } | ConvertTo-Json
        
        $loginResponse = Invoke-RestMethod -Uri "$apiUrl/api/auth/login" `
            -Method POST `
            -ContentType "application/json" `
            -Body $loginBody
        
        if ($loginResponse.Success) {
            Write-Host "   ✓ Login exitoso!" -ForegroundColor Green
            Write-Host "   ✓ Token obtenido: $($loginResponse.Token.Substring(0, 20))..." -ForegroundColor Green
            $global:jwtToken = $loginResponse.Token
            
            # Guardar token para pruebas posteriores
            Write-Host "`n   Token guardado para pruebas posteriores" -ForegroundColor Cyan
        } else {
            Write-Host "   ✗ Login falló: $($loginResponse.Message)" -ForegroundColor Red
        }
    } catch {
        Write-Host "   ✗ Error: $_" -ForegroundColor Red
        if ($_.ErrorDetails.Message) {
            $errorDetails = $_.ErrorDetails.Message | ConvertFrom-Json
            Write-Host "   Detalle: $($errorDetails.mensaje)" -ForegroundColor Red
        }
    }
} else {
    Write-Host "   ⚠ Omitido (requiere credenciales)" -ForegroundColor Yellow
}

# 5. Probar CRUD (requiere token)
if ($global:jwtToken) {
    Write-Host "`n5. Probando CRUD Endpoint (GET)..." -ForegroundColor Yellow
    try {
        $query = "SELECT COUNT(*) as Total FROM conf_usuarios WHERE Activo = 1"
        $encodedQuery = [System.Web.HttpUtility]::UrlEncode($query)
        
        $headers = @{
            "Authorization" = "Bearer $global:jwtToken"
        }
        
        $crudResponse = Invoke-RestMethod -Uri "$apiUrl/api/crud?strQuery=$encodedQuery" `
            -Method GET `
            -Headers $headers
        
        Write-Host "   ✓ CRUD funcionando!" -ForegroundColor Green
        Write-Host "   ✓ Registros retornados: $($crudResponse.Count)" -ForegroundColor Green
    } catch {
        Write-Host "   ✗ Error: $_" -ForegroundColor Red
    }
} else {
    Write-Host "`n5. CRUD Endpoint..." -ForegroundColor Yellow
    Write-Host "   ⚠ Requiere token (haz login primero)" -ForegroundColor Yellow
}

# 6. Document Intelligence (requiere token y archivo)
Write-Host "`n6. Document Intelligence Endpoint..." -ForegroundColor Yellow
Write-Host "   ⚠ Requiere:" -ForegroundColor Yellow
Write-Host "      - Token JWT (haz login)" -ForegroundColor Yellow
Write-Host "      - Archivo PDF/JPG/PNG" -ForegroundColor Yellow
Write-Host "   Para probar manualmente:" -ForegroundColor Cyan
Write-Host "   1. Abre Swagger: $apiUrl/swagger" -ForegroundColor Cyan
Write-Host "   2. Autentica con el token" -ForegroundColor Cyan
Write-Host "   3. Prueba POST /api/document-intelligence" -ForegroundColor Cyan

# Resumen
Write-Host "`n=== Resumen ===" -ForegroundColor Green
Write-Host "API URL: $apiUrl" -ForegroundColor Cyan
Write-Host "Swagger: $apiUrl/swagger" -ForegroundColor Cyan
Write-Host "Health: $apiUrl/api/health" -ForegroundColor Cyan
Write-Host "`nPara probar Document Intelligence:" -ForegroundColor Yellow
Write-Host "1. Abre $apiUrl/swagger en el navegador" -ForegroundColor White
Write-Host "2. Ve a POST /api/auth/login y obtén un token" -ForegroundColor White
Write-Host "3. Haz clic en 'Authorize' e ingresa: Bearer {tu-token}" -ForegroundColor White
Write-Host "4. Ve a POST /api/document-intelligence" -ForegroundColor White
Write-Host "5. Haz clic en 'Try it out'" -ForegroundColor White
Write-Host "6. Selecciona un archivo (PDF, JPG o PNG)" -ForegroundColor White
Write-Host "7. Ingresa tipoDocumento: 'INE' o 'TARJETA_CIRCULACION'" -ForegroundColor White
Write-Host "8. Haz clic en 'Execute'" -ForegroundColor White
