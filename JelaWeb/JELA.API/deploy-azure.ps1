# Script de despliegue de JELA.API a Azure App Service
# Requiere: Azure CLI instalado y autenticado

param(
    [Parameter(Mandatory=$true)]
    [string]$ResourceGroupName = "jela-resources",
    
    [Parameter(Mandatory=$true)]
    [string]$AppServiceName = "jela-api",
    
    [Parameter(Mandatory=$false)]
    [string]$Location = "eastus",
    
    [Parameter(Mandatory=$false)]
    [string]$PlanName = "jela-api-plan",
    
    [Parameter(Mandatory=$false)]
    [string]$Sku = "B1",
    
    [Parameter(Mandatory=$false)]
    [string]$ConfigurationPath = ".\azure-config.json"
)

Write-Host "=== Despliegue de JELA.API a Azure ===" -ForegroundColor Green

# Verificar que Azure CLI esté instalado
if (-not (Get-Command az -ErrorAction SilentlyContinue)) {
    Write-Host "Error: Azure CLI no está instalado. Instálalo desde https://aka.ms/installazurecliwindows" -ForegroundColor Red
    exit 1
}

# Verificar login
Write-Host "Verificando autenticación en Azure..." -ForegroundColor Yellow
$account = az account show 2>$null | ConvertFrom-Json
if (-not $account) {
    Write-Host "No estás autenticado. Iniciando login..." -ForegroundColor Yellow
    az login
}

# Crear grupo de recursos si no existe
Write-Host "Verificando grupo de recursos: $ResourceGroupName..." -ForegroundColor Yellow
$rg = az group show --name $ResourceGroupName 2>$null
if (-not $rg) {
    Write-Host "Creando grupo de recursos: $ResourceGroupName..." -ForegroundColor Yellow
    az group create --name $ResourceGroupName --location $Location
} else {
    Write-Host "Grupo de recursos ya existe." -ForegroundColor Green
}

# Crear App Service Plan si no existe
Write-Host "Verificando App Service Plan: $PlanName..." -ForegroundColor Yellow
$plan = az appservice plan show --name $PlanName --resource-group $ResourceGroupName 2>$null
if (-not $plan) {
    Write-Host "Creando App Service Plan: $PlanName..." -ForegroundColor Yellow
    az appservice plan create `
        --name $PlanName `
        --resource-group $ResourceGroupName `
        --sku $Sku `
        --is-linux
} else {
    Write-Host "App Service Plan ya existe." -ForegroundColor Green
}

# Crear Web App si no existe
Write-Host "Verificando Web App: $AppServiceName..." -ForegroundColor Yellow
$webapp = az webapp show --name $AppServiceName --resource-group $ResourceGroupName 2>$null
if (-not $webapp) {
    Write-Host "Creando Web App: $AppServiceName..." -ForegroundColor Yellow
    az webapp create `
        --name $AppServiceName `
        --resource-group $ResourceGroupName `
        --plan $PlanName `
        --runtime "DOTNET|8.0"
} else {
    Write-Host "Web App ya existe." -ForegroundColor Green
}

# Compilar proyecto
Write-Host "Compilando proyecto en modo Release..." -ForegroundColor Yellow
$projectPath = Join-Path $PSScriptRoot "JELA.API\JELA.API.csproj"
if (-not (Test-Path $projectPath)) {
    Write-Host "Error: No se encontró el proyecto en $projectPath" -ForegroundColor Red
    exit 1
}

$publishPath = Join-Path $PSScriptRoot "publish"
if (Test-Path $publishPath) {
    Remove-Item -Path $publishPath -Recurse -Force
}

dotnet publish $projectPath -c Release -o $publishPath

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error al compilar el proyecto." -ForegroundColor Red
    exit 1
}

# Crear ZIP
Write-Host "Creando archivo ZIP para despliegue..." -ForegroundColor Yellow
$zipPath = Join-Path $PSScriptRoot "publish.zip"
if (Test-Path $zipPath) {
    Remove-Item -Path $zipPath -Force
}

Compress-Archive -Path "$publishPath\*" -DestinationPath $zipPath -Force

# Desplegar
Write-Host "Desplegando a Azure..." -ForegroundColor Yellow
az webapp deployment source config-zip `
    --resource-group $ResourceGroupName `
    --name $AppServiceName `
    --src $zipPath

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error al desplegar." -ForegroundColor Red
    exit 1
}

# Configurar settings si existe archivo de configuración
if (Test-Path $ConfigurationPath) {
    Write-Host "Aplicando configuración desde $ConfigurationPath..." -ForegroundColor Yellow
    $config = Get-Content $ConfigurationPath | ConvertFrom-Json
    
    $settings = @()
    foreach ($key in $config.PSObject.Properties.Name) {
        $value = $config.$key
        if ($value -is [PSCustomObject] -or $value -is [System.Collections.Hashtable]) {
            # Nested objects (como Jwt, AzureDocumentIntelligence)
            foreach ($nestedKey in $value.PSObject.Properties.Name) {
                $nestedValue = $value.$nestedKey
                $settings += "$key`:$nestedKey=$nestedValue"
            }
        } elseif ($value -is [Array]) {
            # Arrays (como AllowedPrefixes)
            for ($i = 0; $i -lt $value.Length; $i++) {
                $settings += "$key`__$i=$($value[$i])"
            }
        } else {
            $settings += "$key=$value"
        }
    }
    
    az webapp config appsettings set `
        --name $AppServiceName `
        --resource-group $ResourceGroupName `
        --settings $settings
}

Write-Host "`n=== Despliegue completado ===" -ForegroundColor Green
Write-Host "URL de la API: https://$AppServiceName.azurewebsites.net" -ForegroundColor Cyan
Write-Host "Swagger: https://$AppServiceName.azurewebsites.net/swagger" -ForegroundColor Cyan
Write-Host "`nNo olvides configurar:" -ForegroundColor Yellow
Write-Host "  1. Connection Strings en Azure Portal" -ForegroundColor Yellow
Write-Host "  2. Variables de entorno (JWT, Azure Document Intelligence)" -ForegroundColor Yellow
Write-Host "  3. Actualizar Web.config de JelaWeb con la nueva URL" -ForegroundColor Yellow

# Limpiar
Remove-Item -Path $zipPath -Force -ErrorAction SilentlyContinue
