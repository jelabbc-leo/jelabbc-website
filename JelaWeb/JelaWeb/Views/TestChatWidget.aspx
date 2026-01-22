<%@ Page Title="Prueba Chat Widget" Language="vb" MasterPageFile="~/MasterPages/Jela.Master" AutoEventWireup="false" CodeBehind="TestChatWidget.aspx.vb" Inherits="JelaWeb.TestChatWidget" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .test-container {
            max-width: 1200px;
            margin: 40px auto;
            padding: 20px;
        }
        
        .test-header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 40px;
            border-radius: 12px;
            margin-bottom: 30px;
            text-align: center;
        }
        
        .test-header h1 {
            margin: 0 0 10px 0;
            font-size: 32px;
        }
        
        .test-header p {
            margin: 0;
            font-size: 16px;
            opacity: 0.9;
        }
        
        .test-section {
            background: white;
            padding: 30px;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
            margin-bottom: 20px;
        }
        
        .test-section h2 {
            margin: 0 0 20px 0;
            font-size: 24px;
            color: #333;
            border-bottom: 2px solid #667eea;
            padding-bottom: 10px;
        }
        
        .test-section h3 {
            margin: 20px 0 10px 0;
            font-size: 18px;
            color: #555;
        }
        
        .test-section ul {
            margin: 10px 0;
            padding-left: 20px;
        }
        
        .test-section li {
            margin: 8px 0;
            line-height: 1.6;
        }
        
        .test-code {
            background: #f5f5f5;
            padding: 15px;
            border-radius: 8px;
            border-left: 4px solid #667eea;
            font-family: 'Courier New', monospace;
            font-size: 13px;
            overflow-x: auto;
            margin: 15px 0;
        }
        
        .test-alert {
            background: #e3f2fd;
            border-left: 4px solid #2196f3;
            padding: 15px;
            border-radius: 8px;
            margin: 15px 0;
        }
        
        .test-alert-success {
            background: #e8f5e9;
            border-left-color: #4caf50;
        }
        
        .test-alert-warning {
            background: #fff3e0;
            border-left-color: #ff9800;
        }
        
        .test-button {
            background: #667eea;
            color: white;
            border: none;
            padding: 12px 24px;
            border-radius: 8px;
            font-size: 14px;
            cursor: pointer;
            transition: all 0.3s ease;
            margin: 5px;
        }
        
        .test-button:hover {
            background: #5568d3;
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
        }
        
        .test-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 20px;
            margin: 20px 0;
        }
        
        .test-card {
            background: #f8f9fa;
            padding: 20px;
            border-radius: 8px;
            border: 1px solid #dee2e6;
        }
        
        .test-card h4 {
            margin: 0 0 10px 0;
            color: #667eea;
        }
        
        .test-card p {
            margin: 5px 0;
            font-size: 14px;
            color: #666;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="test-container">
        <!-- Header -->
        <div class="test-header">
            <h1>🤖 Prueba del Chat Widget con IA</h1>
            <p>Sistema JELABBC - Módulo de Tickets Colaborativos</p>
        </div>
        
        <!-- Instrucciones -->
        <div class="test-section">
            <h2>📋 Instrucciones de Prueba</h2>
            
            <div class="test-alert test-alert-success">
                <strong>✅ Widget Activo:</strong> El widget de chat está visible en la esquina inferior derecha de esta página.
                Haz clic en el botón flotante azul para abrir el chat.
            </div>
            
            <h3>Pasos para probar:</h3>
            <ol>
                <li><strong>Abrir el widget:</strong> Haz clic en el botón flotante azul en la esquina inferior derecha</li>
                <li><strong>Completar el formulario:</strong> Ingresa tu nombre y email (solo la primera vez)</li>
                <li><strong>Enviar un mensaje:</strong> Escribe una pregunta o solicitud y presiona Enter o el botón de enviar</li>
                <li><strong>Esperar respuesta:</strong> El asistente de IA procesará tu mensaje y responderá automáticamente</li>
                <li><strong>Continuar la conversación:</strong> Puedes seguir enviando mensajes en la misma sesión</li>
            </ol>
            
            <div class="test-alert test-alert-warning">
                <strong>⚠️ Rate Limiting:</strong> El widget tiene un límite de 5 mensajes por hora para prevenir abuso.
                Si alcanzas el límite, deberás esperar una hora para enviar más mensajes.
            </div>
        </div>
        
        <!-- Características -->
        <div class="test-section">
            <h2>✨ Características Implementadas</h2>
            
            <div class="test-grid">
                <div class="test-card">
                    <h4>🎨 Diseño Moderno</h4>
                    <p>Interfaz limpia y profesional con animaciones suaves</p>
                </div>
                
                <div class="test-card">
                    <h4>🤖 IA Integrada</h4>
                    <p>Respuestas automáticas generadas por Azure OpenAI</p>
                </div>
                
                <div class="test-card">
                    <h4>💾 Persistencia de Sesión</h4>
                    <p>El historial de chat se mantiene durante la sesión del navegador</p>
                </div>
                
                <div class="test-card">
                    <h4>📱 Responsive</h4>
                    <p>Funciona perfectamente en desktop, tablet y móvil</p>
                </div>
                
                <div class="test-card">
                    <h4>🔒 Validación de Duplicados</h4>
                    <p>Detecta si ya tienes un ticket abierto</p>
                </div>
                
                <div class="test-card">
                    <h4>🎯 Rate Limiting</h4>
                    <p>Protección contra abuso con límite de mensajes</p>
                </div>
            </div>
        </div>
        
        <!-- Ejemplos de Mensajes -->
        <div class="test-section">
            <h2>💬 Ejemplos de Mensajes para Probar</h2>
            
            <h3>Preguntas Generales:</h3>
            <ul>
                <li>"¿Cómo puedo pagar mi cuota de mantenimiento?"</li>
                <li>"¿Cuál es el horario de atención?"</li>
                <li>"Necesito información sobre las áreas comunes"</li>
            </ul>
            
            <h3>Reportes de Problemas:</h3>
            <ul>
                <li>"Hay una fuga de agua en mi unidad"</li>
                <li>"El elevador no está funcionando"</li>
                <li>"Necesito reportar un problema con la iluminación"</li>
            </ul>
            
            <h3>Solicitudes:</h3>
            <ul>
                <li>"Quiero reservar el salón de eventos"</li>
                <li>"Necesito un pase de visitante"</li>
                <li>"¿Cómo puedo actualizar mis datos de contacto?"</li>
            </ul>
        </div>
        
        <!-- Información Técnica -->
        <div class="test-section">
            <h2>🔧 Información Técnica</h2>
            
            <h3>Endpoint de API:</h3>
            <div class="test-code">
                POST <%= ConfigurationManager.AppSettings("ApiBaseUrl") %>/api/webhooks/chatweb
            </div>
            
            <h3>Estructura del Request:</h3>
            <div class="test-code">
{
    "Nombre": "Juan Pérez",
    "Email": "juan@example.com",
    "Mensaje": "¿Cómo puedo pagar mi cuota?",
    "IPOrigen": "192.168.1.1",
    "IdEntidad": 1,
    "SessionId": "session_1234567890_abc123"
}
            </div>
            
            <h3>Estructura del Response:</h3>
            <div class="test-code">
{
    "Success": true,
    "TicketId": 123,
    "Mensaje": "Ticket #123 creado exitosamente",
    "RespuestaIA": "Hola Juan, para pagar tu cuota puedes...",
    "SessionId": "session_1234567890_abc123"
}
            </div>
        </div>
        
        <!-- Controles de Prueba -->
        <div class="test-section">
            <h2>🎮 Controles de Prueba</h2>
            
            <p>Usa estos botones para probar diferentes funcionalidades del widget:</p>
            
            <button class="test-button" onclick="abrirWidget()">
                <i class="fas fa-comments"></i> Abrir Widget
            </button>
            
            <button class="test-button" onclick="cerrarWidget()">
                <i class="fas fa-times"></i> Cerrar Widget
            </button>
            
            <button class="test-button" onclick="limpiarHistorial()">
                <i class="fas fa-trash"></i> Limpiar Historial
            </button>
            
            <button class="test-button" onclick="verEstado()">
                <i class="fas fa-info-circle"></i> Ver Estado
            </button>
        </div>
        
        <!-- Estado del Sistema -->
        <div class="test-section">
            <h2>📊 Estado del Sistema</h2>
            
            <div class="test-grid">
                <div class="test-card">
                    <h4>API Backend</h4>
                    <p><strong>Estado:</strong> <span id="api-status">Verificando...</span></p>
                    <p><strong>URL:</strong> <%= ConfigurationManager.AppSettings("ApiBaseUrl") %></p>
                </div>
                
                <div class="test-card">
                    <h4>Base de Datos</h4>
                    <p><strong>Tablas:</strong> 13 tablas creadas</p>
                    <p><strong>Stored Procedures:</strong> 3 implementados</p>
                </div>
                
                <div class="test-card">
                    <h4>Servicios</h4>
                    <p><strong>Validación:</strong> ✅ Activo</p>
                    <p><strong>OpenAI:</strong> ✅ Activo</p>
                    <p><strong>YCloud:</strong> ✅ Activo</p>
                </div>
                
                <div class="test-card">
                    <h4>Widget</h4>
                    <p><strong>Versión:</strong> 1.0.0</p>
                    <p><strong>Estado:</strong> <span id="widget-status">Inicializado</span></p>
                </div>
            </div>
        </div>
    </div>
    
    <script type="text/javascript">
        // Funciones de control del widget
        function abrirWidget() {
            if (typeof JelaChatWidget !== 'undefined') {
                JelaChatWidget.openWidget();
            } else {
                alert('El widget no está inicializado');
            }
        }
        
        function cerrarWidget() {
            if (typeof JelaChatWidget !== 'undefined') {
                JelaChatWidget.closeWidget();
            } else {
                alert('El widget no está inicializado');
            }
        }
        
        function limpiarHistorial() {
            if (confirm('¿Estás seguro de que quieres limpiar el historial del chat?')) {
                sessionStorage.removeItem('jela_chat_session');
                localStorage.removeItem('jela_chat_rate_limit');
                alert('Historial limpiado. Recarga la página para ver los cambios.');
            }
        }
        
        function verEstado() {
            if (typeof JelaChatWidget !== 'undefined') {
                var estado = {
                    'Session ID': JelaChatWidget.state.sessionId,
                    'Ticket ID': JelaChatWidget.state.ticketId || 'N/A',
                    'IP Cliente': JelaChatWidget.state.clientIP || 'Obteniendo...',
                    'Mensajes': JelaChatWidget.state.messages.length,
                    'Widget Abierto': JelaChatWidget.state.isOpen ? 'Sí' : 'No',
                    'Esperando Respuesta': JelaChatWidget.state.isWaitingResponse ? 'Sí' : 'No'
                };
                
                var mensaje = 'Estado del Widget:\n\n';
                for (var key in estado) {
                    mensaje += key + ': ' + estado[key] + '\n';
                }
                
                alert(mensaje);
            } else {
                alert('El widget no está inicializado');
            }
        }
        
        // Verificar estado de la API
        document.addEventListener('DOMContentLoaded', function() {
            var apiUrl = '<%= ConfigurationManager.AppSettings("ApiBaseUrl") %>';
            
            fetch(apiUrl + '/health/live')
                .then(response => {
                    if (response.ok) {
                        document.getElementById('api-status').textContent = '✅ En línea';
                        document.getElementById('api-status').style.color = '#4caf50';
                    } else {
                        document.getElementById('api-status').textContent = '⚠️ Problemas';
                        document.getElementById('api-status').style.color = '#ff9800';
                    }
                })
                .catch(error => {
                    document.getElementById('api-status').textContent = '❌ Fuera de línea';
                    document.getElementById('api-status').style.color = '#f44336';
                });
            
            // Actualizar estado del widget
            setTimeout(function() {
                if (typeof JelaChatWidget !== 'undefined') {
                    document.getElementById('widget-status').textContent = '✅ Activo';
                    document.getElementById('widget-status').style.color = '#4caf50';
                } else {
                    document.getElementById('widget-status').textContent = '❌ Error';
                    document.getElementById('widget-status').style.color = '#f44336';
                }
            }, 1000);
        });
    </script>
</asp:Content>
