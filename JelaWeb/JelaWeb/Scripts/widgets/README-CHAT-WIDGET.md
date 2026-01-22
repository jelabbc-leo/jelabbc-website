# 🤖 JELA Chat Widget - Documentación

Widget de chat web con inteligencia artificial para el Sistema JELABBC.

## 📋 Tabla de Contenidos

- [Características](#características)
- [Instalación](#instalación)
- [Configuración](#configuración)
- [Uso](#uso)
- [API](#api)
- [Personalización](#personalización)
- [Troubleshooting](#troubleshooting)

---

## ✨ Características

### Funcionalidades Principales

- ✅ **Widget flotante** en esquina inferior derecha
- ✅ **Formulario de contacto** con validación (Nombre, Email, Mensaje)
- ✅ **Respuestas de IA** en tiempo real vía Azure OpenAI
- ✅ **Historial de conversación** persistente durante la sesión
- ✅ **Diseño responsivo** (desktop, tablet, móvil)
- ✅ **Temas personalizables** (blue, green, purple)
- ✅ **Captura automática de IP** del cliente
- ✅ **Rate limiting** (5 mensajes por hora)
- ✅ **Validación de duplicados** (detecta tickets abiertos)
- ✅ **Animaciones suaves** y transiciones
- ✅ **Accesibilidad** (WCAG 2.1 AA)

### Características Técnicas

- 📦 **Auto-contenido** - No requiere dependencias externas
- 🔒 **Seguro** - Validación de entrada y sanitización de HTML
- 💾 **Persistente** - Usa sessionStorage para mantener historial
- 🚀 **Ligero** - ~15KB minificado
- 📱 **Mobile-first** - Optimizado para dispositivos móviles
- ♿ **Accesible** - Soporte para lectores de pantalla

---

## 🚀 Instalación

### Opción 1: Integración en Master Page (Recomendado)

El widget ya está integrado en el Master Page (`Jela.Master`) y estará disponible en todas las páginas del sistema.

**Archivos incluidos:**
```
JelaWeb/
├── Scripts/
│   └── widgets/
│       └── chat-widget.js
├── Content/
│   └── CSS/
│       └── chat-widget.css
└── MasterPages/
    └── Jela.Master (ya modificado)
```

### Opción 2: Integración Manual en Página Específica

Si deseas agregar el widget solo en páginas específicas:

```html
<!-- En el <head> de tu página -->
<link rel="stylesheet" href="/Content/CSS/chat-widget.css" />

<!-- Antes del cierre de </body> -->
<script src="/Scripts/widgets/chat-widget.js"></script>
<script>
  JelaChatWidget.init({
    apiUrl: 'https://jela-api-xxx.azurewebsites.net',
    idEntidad: 1
  });
</script>
```

---

## ⚙️ Configuración

### Opciones de Configuración

```javascript
JelaChatWidget.init({
  // URL base de la API (requerido)
  apiUrl: 'https://jela-api-xxx.azurewebsites.net',
  
  // ID de la entidad (requerido)
  idEntidad: 1,
  
  // Posición del widget (opcional)
  position: 'bottom-right', // 'bottom-right' | 'bottom-left'
  
  // Tema de colores (opcional)
  theme: 'blue', // 'blue' | 'green' | 'purple'
  
  // Máximo de mensajes en historial (opcional)
  maxMessages: 50,
  
  // Rate limiting - mensajes permitidos (opcional)
  rateLimitMessages: 5,
  
  // Rate limiting - ventana de tiempo en ms (opcional)
  rateLimitWindow: 3600000, // 1 hora
  
  // Abrir automáticamente al cargar (opcional)
  autoOpen: false,
  
  // Mostrar branding "Powered by JELA BBC" (opcional)
  showBranding: true
});
```

### Ejemplo de Configuración Completa

```javascript
// Configuración personalizada
JelaChatWidget.init({
  apiUrl: JELA_API_BASE_URL, // Variable global del Master Page
  idEntidad: <%= Session("IdEntidad") %>, // Desde sesión ASP.NET
  position: 'bottom-right',
  theme: 'green',
  maxMessages: 100,
  rateLimitMessages: 10,
  rateLimitWindow: 3600000,
  autoOpen: false,
  showBranding: true
});
```

---

## 📖 Uso

### Para Usuarios Finales

1. **Abrir el widget:**
   - Haz clic en el botón flotante azul en la esquina inferior derecha

2. **Primera vez:**
   - Ingresa tu nombre y email
   - Escribe tu mensaje
   - Presiona Enter o haz clic en el botón de enviar

3. **Mensajes siguientes:**
   - Los campos de nombre/email se ocultan automáticamente
   - Solo escribe tu mensaje y envía

4. **Cerrar el widget:**
   - Haz clic en el botón X en la esquina superior derecha
   - O haz clic en el botón de minimizar (-)

### Para Desarrolladores

#### Controlar el Widget Programáticamente

```javascript
// Abrir el widget
JelaChatWidget.openWidget();

// Cerrar el widget
JelaChatWidget.closeWidget();

// Minimizar el widget
JelaChatWidget.minimizeWidget();

// Alternar (abrir/cerrar)
JelaChatWidget.toggleWidget();

// Obtener estado actual
console.log(JelaChatWidget.state);
// {
//   isOpen: false,
//   sessionId: "session_1234567890_abc123",
//   ticketId: 123,
//   messages: [...],
//   clientIP: "192.168.1.1"
// }
```

#### Eventos Personalizados

```javascript
// Escuchar cuando se envía un mensaje
document.addEventListener('jela-chat-message-sent', function(e) {
  console.log('Mensaje enviado:', e.detail);
});

// Escuchar cuando se recibe respuesta
document.addEventListener('jela-chat-response-received', function(e) {
  console.log('Respuesta recibida:', e.detail);
});
```

---

## 🔌 API

### Endpoint del Widget

```
POST /api/webhooks/chatweb
```

### Request

```json
{
  "Nombre": "Juan Pérez",
  "Email": "juan@example.com",
  "Mensaje": "¿Cómo puedo pagar mi cuota?",
  "IPOrigen": "192.168.1.1",
  "IdEntidad": 1,
  "SessionId": "session_1234567890_abc123"
}
```

### Response (Éxito)

```json
{
  "Success": true,
  "TicketId": 123,
  "Mensaje": "Ticket #123 creado exitosamente",
  "RespuestaIA": "Hola Juan, para pagar tu cuota puedes...",
  "SessionId": "session_1234567890_abc123"
}
```

### Response (Cliente con Ticket Abierto)

```json
{
  "Success": true,
  "TicketId": 120,
  "Mensaje": "Ya tienes un ticket abierto (#120). Un agente te atenderá pronto.",
  "RespuestaIA": null,
  "SessionId": "session_1234567890_abc123"
}
```

### Response (Error)

```json
{
  "Success": false,
  "ErrorCode": "CHATWEB_ERROR",
  "ErrorMessage": "Error procesando mensaje de chat"
}
```

---

## 🎨 Personalización

### Temas de Colores

El widget incluye 3 temas predefinidos:

#### Tema Azul (por defecto)
```javascript
theme: 'blue'
```
- Color primario: `#0066cc`
- Ideal para: Corporativo, profesional

#### Tema Verde
```javascript
theme: 'green'
```
- Color primario: `#28a745`
- Ideal para: Eco-friendly, salud

#### Tema Morado
```javascript
theme: 'purple'
```
- Color primario: `#6f42c1`
- Ideal para: Creativo, moderno

### Personalizar Colores (CSS)

Puedes sobrescribir las variables CSS para crear tu propio tema:

```css
:root {
  --jela-chat-primary-blue: #0066cc;
  --jela-chat-primary-dark: #003d7a;
  --jela-chat-bg-light: #f8f9fa;
  --jela-chat-bg-white: #ffffff;
  --jela-chat-text-dark: #212529;
  --jela-chat-text-muted: #6c757d;
  --jela-chat-border: #dee2e6;
  --jela-chat-user-bg: #e3f2fd;
  --jela-chat-bot-bg: #f5f5f5;
}
```

### Personalizar Posición

```css
/* Cambiar posición del widget */
.jela-chat-widget {
  bottom: 20px;
  right: 20px;
}

/* Cambiar tamaño del botón */
.jela-chat-button {
  width: 70px;
  height: 70px;
}

/* Cambiar tamaño de la ventana */
.jela-chat-window {
  width: 400px;
  height: 650px;
}
```

---

## 🐛 Troubleshooting

### El widget no aparece

**Problema:** El widget no se muestra en la página.

**Soluciones:**
1. Verificar que los archivos CSS y JS estén cargados:
   ```javascript
   // En la consola del navegador
   console.log(typeof JelaChatWidget); // Debe mostrar "object"
   ```

2. Verificar que no haya errores en la consola:
   ```
   F12 → Console → Buscar errores en rojo
   ```

3. Verificar que el widget esté inicializado:
   ```javascript
   // En la consola del navegador
   JelaChatWidget.state.sessionId; // Debe mostrar un ID
   ```

### El widget no envía mensajes

**Problema:** Al enviar un mensaje, no pasa nada.

**Soluciones:**
1. Verificar la URL de la API:
   ```javascript
   console.log(JelaChatWidget.config.apiUrl);
   ```

2. Verificar que la API esté en línea:
   ```javascript
   fetch(JelaChatWidget.config.apiUrl + '/health/live')
     .then(r => console.log('API Status:', r.status));
   ```

3. Verificar CORS en la API:
   - La API debe permitir requests desde el dominio del frontend

4. Verificar rate limiting:
   ```javascript
   // Limpiar rate limit
   localStorage.removeItem('jela_chat_rate_limit');
   ```

### No recibo respuestas de IA

**Problema:** El mensaje se envía pero no hay respuesta.

**Soluciones:**
1. Verificar que Azure OpenAI esté configurado en la API

2. Verificar logs del servidor:
   ```
   JELA.API/logs/jela-api-YYYYMMDD.log
   ```

3. Verificar que el servicio OpenAI esté activo:
   ```csharp
   // En Program.cs
   builder.Services.AddScoped<IOpenAIService, AzureOpenAIService>();
   ```

### El historial no se guarda

**Problema:** Al recargar la página, se pierde el historial.

**Soluciones:**
1. Verificar que sessionStorage esté habilitado:
   ```javascript
   console.log(typeof sessionStorage); // Debe mostrar "object"
   ```

2. Verificar que no estés en modo incógnito:
   - El modo incógnito puede bloquear sessionStorage

3. Limpiar y reiniciar:
   ```javascript
   sessionStorage.removeItem('jela_chat_session');
   location.reload();
   ```

### Rate limiting muy restrictivo

**Problema:** El límite de 5 mensajes por hora es muy bajo.

**Soluciones:**
1. Aumentar el límite en la configuración:
   ```javascript
   JelaChatWidget.init({
     rateLimitMessages: 10, // Aumentar a 10
     rateLimitWindow: 3600000 // 1 hora
   });
   ```

2. Limpiar el contador manualmente:
   ```javascript
   localStorage.removeItem('jela_chat_rate_limit');
   ```

---

## 📊 Métricas y Monitoreo

### Datos Almacenados

El widget almacena los siguientes datos:

**sessionStorage:**
- `jela_chat_session` - Historial de la sesión actual

**localStorage:**
- `jela_chat_rate_limit` - Contador de rate limiting

### Logs del Cliente

El widget registra eventos en la consola del navegador:

```
[JELA Chat Widget] Inicializado correctamente
[JELA Chat Widget] IP del cliente: 192.168.1.1
[JELA Chat Widget] Respuesta de IA generada para email: juan@example.com
```

### Logs del Servidor

El endpoint registra eventos en los logs de la API:

```
[Information] Mensaje Chat Web recibido - Email: juan@example.com, IP: 192.168.1.1
[Information] Respuesta de IA generada para email: juan@example.com
[Information] Ticket #123 creado para chat web de juan@example.com
```

---

## 🧪 Página de Prueba

Accede a la página de prueba para validar el funcionamiento:

```
http://localhost/Views/TestChatWidget.aspx
```

La página incluye:
- ✅ Instrucciones detalladas
- ✅ Ejemplos de mensajes
- ✅ Controles de prueba
- ✅ Estado del sistema
- ✅ Información técnica

---

## 📝 Notas Importantes

### Seguridad

- ✅ El widget valida y sanitiza todas las entradas
- ✅ Los mensajes se escapan para prevenir XSS
- ✅ La IP se obtiene de forma segura
- ✅ Rate limiting previene abuso

### Performance

- ✅ El widget es ligero (~15KB)
- ✅ Carga asíncrona de recursos
- ✅ Lazy loading de imágenes
- ✅ Optimizado para móviles

### Accesibilidad

- ✅ Compatible con lectores de pantalla
- ✅ Navegación por teclado
- ✅ Contraste de colores WCAG AA
- ✅ Soporte para modo de alto contraste

---

## 🤝 Soporte

Para reportar problemas o solicitar nuevas características:

1. **Documentación:** Consulta este README
2. **Logs:** Revisa los logs del navegador (F12 → Console)
3. **API:** Verifica los logs del servidor
4. **Contacto:** Equipo de desarrollo JELA BBC

---

## 📄 Licencia

© 2026 JELA BBC. Todos los derechos reservados.

---

**Última actualización:** 18 de Enero de 2026  
**Versión:** 1.0.0
