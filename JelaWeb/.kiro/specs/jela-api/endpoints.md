# 🌐 ENDPOINTS DOCUMENTATION - JELA.API

**Fecha:** Enero 21, 2026  
**Versión:** 1.0  
**Base URL:** `https://jela-api.azurewebsites.net/api`  

---

## 📋 ÍNDICE DE ENDPOINTS

### 🔐 Autenticación
- [POST /auth/login](#post-authlogin)
- [POST /auth/refresh](#post-authrefresh)
- [GET /auth/validate](#get-authvalidate)

### 🗄️ CRUD Dinámico
- [GET /crud](#get-crud)
- [POST /crud/{table}](#post-crudtable)
- [PUT /crud/{table}/{id}](#put-crudtableid)
- [DELETE /crud/{table}](#delete-crudtable)

### 🤖 Inteligencia Artificial
- [POST /openai/ticket-analysis](#post-openaiticket-analysis)
- [POST /openai/chat-completion](#post-openaichat-completion)

### 📄 Procesamiento de Documentos
- [POST /document-intelligence](#post-document-intelligence)

### 📞 Webhooks Multi-Canal
- [POST /webhooks/vapi](#post-webhooksvapi)
- [POST /webhooks/y-cloud](#post-webhooksy-cloud)
- [POST /webhooks/firebase](#post-webhooksfirebase)
- [POST /webhooks/chat-web](#post-webhookschat-web)

### 🏥 Health & Monitoring
- [GET /health](#get-health)
- [GET /version](#get-version)
- [GET /health/live](#get-healthlive)
- [GET /health/ready](#get-healthready)

---

## 🔐 AUTENTICACIÓN ENDPOINTS

### POST /auth/login
Inicia sesión de usuario y retorna JWT token.

**Request:**
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "usuario@ejemplo.com",
  "password": "password123"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "refresh-token-here",
    "expiresIn": 3600,
    "user": {
      "id": 1,
      "email": "usuario@ejemplo.com",
      "nombre": "Juan Pérez",
      "roles": ["Usuario"]
    }
  }
}
```

**Response (401 Unauthorized):**
```json
{
  "success": false,
  "error": "Credenciales inválidas"
}
```

**Códigos de Error:**
- `400`: Datos de entrada inválidos
- `401`: Credenciales incorrectas
- `429`: Demasiados intentos de login
- `500`: Error interno del servidor

---

### POST /auth/refresh
Renueva el token de acceso usando refresh token.

**Request:**
```http
POST /api/auth/refresh
Content-Type: application/json
Authorization: Bearer {access-token}

{
  "refreshToken": "refresh-token-here"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "token": "new-jwt-token-here",
    "refreshToken": "new-refresh-token-here",
    "expiresIn": 3600
  }
}
```

---

### GET /auth/validate
Valida si el token actual es válido.

**Request:**
```http
GET /api/auth/validate
Authorization: Bearer {access-token}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "valid": true,
    "user": {
      "id": 1,
      "email": "usuario@ejemplo.com",
      "roles": ["Usuario"]
    }
  }
}
```

---

## 🗄️ CRUD DINÁMICO ENDPOINTS

### GET /crud
Ejecuta consultas SELECT dinámicas.

**Parámetros de Query:**
- `strQuery`: Consulta SQL SELECT (requerido)

**Ejemplos de uso:**

**Obtener todos los proveedores:**
```http
GET /api/crud?strQuery=SELECT * FROM cat_proveedores WHERE IdEntidad = 1 ORDER BY RazonSocial
Authorization: Bearer {token}
```

**Buscar tickets por estado:**
```http
GET /api/crud?strQuery=SELECT Id, Titulo, Estado, FechaCreacion FROM op_tickets WHERE Estado = 'Abierto' AND IdEntidad = 1
Authorization: Bearer {token}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "Id": 1,
      "RazonSocial": "Proveedor ABC S.A.",
      "RFC": "ABC123456789",
      "Activo": true
    }
  ]
}
```

**Validaciones:**
- ✅ Tabla debe tener prefijo válido (`cat_`, `conf_`, `op_`, `log_`)
- ✅ Usuario debe tener permisos para la tabla
- ✅ SQL injection prevention automático
- ❌ No permite `INSERT`, `UPDATE`, `DELETE` (usar endpoints específicos)

---

### POST /crud/{table}
Inserta un nuevo registro en la tabla especificada.

**Parámetros de URL:**
- `table`: Nombre de la tabla (ej: `cat_proveedores`)

**Request:**
```http
POST /api/crud/cat_proveedores
Content-Type: application/json
Authorization: Bearer {token}

{
  "RazonSocial": "Nueva Empresa S.A.",
  "NombreComercial": "Nueva Empresa",
  "RFC": "NES123456789",
  "Email": "contacto@nuevaempresa.com",
  "Telefono": "555-123-4567",
  "Activo": true,
  "IdEntidad": 1
}
```

**Response (201 Created):**
```json
{
  "success": true,
  "data": {
    "id": 123,
    "message": "Registro creado exitosamente"
  }
}
```

**Validaciones:**
- ✅ Campos requeridos validados automáticamente
- ✅ Tipos de datos verificados
- ✅ Relaciones foráneas validadas
- ✅ Auditoría automática (usuario, fecha, IP)

---

### PUT /crud/{table}/{id}
Actualiza un registro existente.

**Parámetros de URL:**
- `table`: Nombre de la tabla
- `id`: ID del registro a actualizar

**Request:**
```http
PUT /api/crud/cat_proveedores/123
Content-Type: application/json
Authorization: Bearer {token}

{
  "Email": "nuevo-email@nuevaempresa.com",
  "Telefono": "555-987-6543"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "message": "Registro actualizado exitosamente"
  }
}
```

---

### DELETE /crud/{table}
Elimina uno o más registros.

**Parámetros de Query:**
- `idField`: Campo ID (default: `Id`)
- `idValue`: Valor del ID a eliminar

**Ejemplo:**
```http
DELETE /api/crud/cat_proveedores?idField=Id&idValue=123
Authorization: Bearer {token}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "message": "Registro eliminado exitosamente"
  }
}
```

---

## 🤖 INTELIGENCIA ARTIFICIAL ENDPOINTS

### POST /openai/ticket-analysis
Analiza tickets usando IA para generar respuestas automáticas.

**Request:**
```http
POST /api/openai/ticket-analysis
Content-Type: application/json
Authorization: Bearer {token}

{
  "ticketId": 123,
  "canal": "ChatWeb",
  "mensajeUsuario": "Mi elevador no funciona",
  "contextoAdicional": {
    "unidad": "101",
    "tipoProblema": "Mantenimiento"
  }
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "respuestaIA": "Lamento las molestias con el elevador...",
    "categoriaDetectada": "Mantenimiento",
    "urgenciaSugerida": "Alta",
    "accionesRecomendadas": [
      "Asignar técnico",
      "Notificar residente"
    ]
  }
}
```

**Canales soportados:**
- `VAPI` - Llamadas telefónicas
- `YCloud` - WhatsApp Business
- `ChatWeb` - Widget web
- `Firebase` - App móvil

---

### POST /openai/chat-completion
Genera completado de chat general usando GPT-4.

**Request:**
```http
POST /api/openai/chat-completion
Content-Type: application/json
Authorization: Bearer {token}

{
  "messages": [
    {
      "role": "system",
      "content": "Eres un asistente de condominios"
    },
    {
      "role": "user",
      "content": "¿Cómo pago mi mantenimiento?"
    }
  ],
  "maxTokens": 500,
  "temperature": 0.7
}
```

---

## 📄 PROCESAMIENTO DE DOCUMENTOS

### POST /document-intelligence
Procesa documentos usando Azure Document Intelligence (OCR).

**Tipos de documento soportados:**
- INE (Identificación Nacional Electoral)
- Licencias de Conducir
- Tarjetas de Circulación

**Request:**
```http
POST /api/document-intelligence
Content-Type: multipart/form-data
Authorization: Bearer {token}

FormData:
- file: [archivo PDF/JPG/PNG]
- tipoDocumento: "INE"
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "tipoDocumento": "INE",
    "datosExtraidos": {
      "nombre": "Juan Pérez García",
      "curp": "PEGJ900101HDFRRN00",
      "fechaNacimiento": "1990-01-01",
      "domicilio": "Calle Principal 123",
      "numeroEmision": "1234567890"
    },
    "confianza": 0.95
  }
}
```

---

## 📞 WEBHOOKS MULTI-CANAL

### POST /webhooks/vapi
Webhook para integraciones con VAPI (llamadas telefónicas).

**Request de VAPI:**
```json
{
  "type": "call.ended",
  "call": {
    "id": "call-123",
    "status": "ended",
    "transcript": "Hola, tengo un problema...",
    "duration": 300
  },
  "metadata": {
    "telefono": "+52551234567"
  }
}
```

### POST /webhooks/y-cloud
Webhook para WhatsApp Business (YCloud).

**Request de YCloud:**
```json
{
  "event": "message.received",
  "message": {
    "id": "msg-123",
    "from": "+52551234567",
    "body": "Hola, mi elevador no funciona",
    "timestamp": "2026-01-21T10:00:00Z"
  }
}
```

### POST /webhooks/firebase
Webhook para notificaciones push (Firebase).

### POST /webhooks/chat-web
Webhook para chat widget integrado en sitio web.

---

## 🏥 HEALTH & MONITORING

### GET /health
Estado general del API y dependencias.

**Response (200 OK):**
```json
{
  "status": "Healthy",
  "timestamp": "2026-01-21T10:00:00Z",
  "version": "1.0.0",
  "services": {
    "database": "Healthy",
    "azureOpenAI": "Healthy",
    "azureDocumentIntelligence": "Healthy",
    "redis": "Healthy"
  }
}
```

### GET /version
Versión actual del API.

**Response (200 OK):**
```json
{
  "version": "1.0.0",
  "buildDate": "2026-01-21T08:00:00Z",
  "environment": "Production"
}
```

### GET /health/live
Health check de liveness (Kubernetes).

### GET /health/ready
Health check de readiness (Kubernetes).

---

## 🔒 SEGURIDAD Y AUTENTICACIÓN

### Headers Requeridos
```http
Authorization: Bearer {jwt-token}
Content-Type: application/json
X-API-Key: {api-key}  // Para webhooks externos
```

### Rate Limiting
- **Autenticados:** 1000 requests/minuto
- **No autenticados:** 100 requests/minuto
- **Webhooks:** 500 requests/minuto

### Validaciones Globales
- ✅ JWT token válido
- ✅ Usuario activo
- ✅ Permisos por tabla/endpoint
- ✅ Rate limiting aplicado
- ✅ Input sanitization
- ✅ SQL injection prevention

---

## 📊 CÓDIGOS DE ERROR

### 4xx Client Errors
- `400`: Bad Request - Datos inválidos
- `401`: Unauthorized - Token inválido/expirado
- `403`: Forbidden - Sin permisos
- `404`: Not Found - Recurso no existe
- `429`: Too Many Requests - Rate limit excedido

### 5xx Server Errors
- `500`: Internal Server Error - Error interno
- `502`: Bad Gateway - Servicio externo caído
- `503`: Service Unavailable - Mantenimiento
- `504`: Gateway Timeout - Timeout en servicio externo

### Formato de Error
```json
{
  "success": false,
  "error": "Descripción del error",
  "code": "ERROR_CODE",
  "timestamp": "2026-01-21T10:00:00Z",
  "traceId": "correlation-id"
}
```

---

## 🧪 TESTING CON POSTMAN

### Environment Variables
```json
{
  "baseUrl": "https://jela-api.azurewebsites.net",
  "token": "",
  "refreshToken": ""
}
```

### Collection Structure
```
JELA.API Tests
├── Auth
│   ├── Login
│   ├── Refresh Token
│   └── Validate Token
├── CRUD
│   ├── Get Proveedores
│   ├── Create Proveedor
│   ├── Update Proveedor
│   └── Delete Proveedor
├── AI
│   ├── Ticket Analysis
│   └── Chat Completion
├── Documents
│   └── Process INE
└── Health
    ├── Health Check
    └── Version
```

---

## 📈 LIMITACIONES Y CUOTAS

### Rate Limits
- **Por minuto:** 1000 requests (autenticados)
- **Por hora:** 50000 requests
- **Por día:** 100000 requests

### Tamaños Máximos
- **Request body:** 10MB
- **File upload:** 25MB
- **Response size:** 50MB

### Timeouts
- **Database queries:** 30 segundos
- **External APIs:** 60 segundos
- **File processing:** 300 segundos

---

## 🔄 VERSIONADO DE API

### Versionado por URL
```
GET /api/v1/crud/...
GET /api/v2/crud/...  // Futuro
```

### Headers de Versionado
```http
Accept: application/vnd.jela.v1+json
API-Version: 1.0
```

### Backward Compatibility
- ✅ Versiones menores: Compatible hacia atrás
- ⚠️ Versiones mayores: Breaking changes
- 📋 Deprecation notices: 6 meses de antelación

---

## 📞 SOPORTE Y CONTACTO

### Canales de Soporte
- **Email:** soporte@jela.com
- **Slack:** #api-support
- **Issues:** GitHub repository

### SLA (Service Level Agreement)
- **Disponibilidad:** 99.9% uptime mensual
- **Response Time:** P95 < 500ms
- **Support:** 24/7 para críticos

### Escalation Matrix
1. **L1:** Soporte técnico básico
2. **L2:** Ingenieros senior
3. **L3:** Arquitectos/DevOps

---

## 🔗 REFERENCIAS

- [README.md](./README.md) - Documentación general
- [rules.md](./rules.md) - Reglas de programación
- [architecture.md](./architecture.md) - Arquitectura detallada
- [Swagger UI](https://jela-api.azurewebsites.net/swagger) - Documentación interactiva