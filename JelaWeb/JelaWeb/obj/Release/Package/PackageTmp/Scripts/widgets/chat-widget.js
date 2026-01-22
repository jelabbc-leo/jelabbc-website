/**
 * JELA Chat Widget - Widget de Chat Web con IA
 * Sistema JELABBC - Módulo de Tickets Colaborativos
 * 
 * Características:
 * - Widget flotante en esquina inferior derecha
 * - Formulario de contacto: Nombre, Email, Mensaje
 * - Envío de mensaje vía POST a /api/webhooks/chatweb
 * - Mostrar respuesta de IA en tiempo real
 * - Historial de conversación en sesión
 * - Diseño responsivo y personalizable
 * - Captura automática de IP del cliente
 * - Rate limiting del lado del cliente
 * 
 * Uso:
 * <script src="/Scripts/widgets/chat-widget.js"></script>
 * <link rel="stylesheet" href="/Content/CSS/chat-widget.css" />
 * <script>
 *   JelaChatWidget.init({
 *     apiUrl: 'https://jela-api-xxx.azurewebsites.net',
 *     idEntidad: 1
 *   });
 * </script>
 */

(function(window) {
    'use strict';

    // ========================================
    // CONFIGURACIÓN Y ESTADO
    // ========================================
    
    var JelaChatWidget = {
        config: {
            apiUrl: 'https://jela-api-ctb8a6ggbpdqbxhg.mexicocentral-01.azurewebsites.net',
            idEntidad: 1,
            position: 'bottom-right', // bottom-right, bottom-left
            theme: 'blue', // blue, green, purple
            maxMessages: 50,
            rateLimitMessages: 5, // Máximo 5 mensajes
            rateLimitWindow: 3600000, // Por hora (1 hora en ms)
            autoOpen: false,
            showBranding: true
        },
        
        state: {
            isOpen: false,
            isMinimized: false,
            sessionId: null,
            clientIP: null,
            ticketId: null,
            messages: [],
            messageCount: 0,
            lastMessageTime: null,
            isWaitingResponse: false,
            userInfo: null, // Información del usuario autenticado
            isAuthenticated: false
        },
        
        elements: {},
        
        // ========================================
        // INICIALIZACIÓN
        // ========================================
        
        init: function(options) {
            var self = this;
            
            // Combinar configuración
            if (options) {
                Object.assign(this.config, options);
            }
            
            // Generar session ID único
            this.state.sessionId = this.generateSessionId();
            
            // Cargar historial de sesión
            this.loadSessionHistory();
            
            // Obtener IP del cliente
            this.getClientIP();
            
            // Crear elementos del DOM primero
            this.createWidget();
            
            // Registrar eventos
            this.registerEvents();
            
            // Obtener información del usuario autenticado (después de crear el DOM)
            this.getUserInfo().then(function() {
                // Auto-abrir si está configurado
                if (self.config.autoOpen) {
                    setTimeout(() => self.openWidget(), 1000);
                }
            });
            
            console.log('[JELA Chat Widget] Inicializado correctamente');
        },
        
        // ========================================
        // GENERACIÓN DE IDs Y UTILIDADES
        // ========================================
        
        generateSessionId: function() {
            return 'session_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9);
        },
        
        generateMessageId: function() {
            return 'msg_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9);
        },
        
        getClientIP: function() {
            var self = this;
            
            // Intentar obtener IP desde servicio externo
            fetch('https://api.ipify.org?format=json')
                .then(response => response.json())
                .then(data => {
                    self.state.clientIP = data.ip;
                    console.log('[JELA Chat Widget] IP del cliente:', self.state.clientIP);
                })
                .catch(error => {
                    console.warn('[JELA Chat Widget] No se pudo obtener IP del cliente:', error);
                    self.state.clientIP = 'unknown';
                });
        },
        
        getUserInfo: function() {
            var self = this;
            
            console.log('[JELA Chat Widget] ===== INICIANDO getUserInfo =====');
            console.log('[JELA Chat Widget] formFields element:', self.elements.formFields);
            
            // Retornar Promise para poder esperar el resultado
            return fetch('/Services/UserInfoHandler.ashx')
                .then(response => {
                    console.log('[JELA Chat Widget] Response status:', response.status);
                    if (response.ok) {
                        return response.json();
                    }
                    throw new Error('Usuario no autenticado');
                })
                .then(data => {
                    console.log('[JELA Chat Widget] Data recibida:', data);
                    if (data.Success) {
                        self.state.userInfo = data;
                        self.state.isAuthenticated = true;
                        console.log('[JELA Chat Widget] ✓ Usuario autenticado:', data.Nombre);
                        console.log('[JELA Chat Widget] ✓ IdEntidad recibido:', data.IdEntidad);  // ← NUEVO LOG
                        
                        // Pre-llenar y ocultar campos
                        if (self.elements.nombre && self.elements.email) {
                            self.elements.nombre.value = data.Nombre || '';
                            self.elements.email.value = data.Email || '';
                            
                            console.log('[JELA Chat Widget] Nombre:', data.Nombre);
                            console.log('[JELA Chat Widget] Email:', data.Email);
                            
                            // Ocultar campos si están llenos
                            if (data.Nombre && data.Email) {
                                console.log('[JELA Chat Widget] Display ANTES:', self.elements.formFields.style.display);
                                self.elements.formFields.style.display = 'none';
                                console.log('[JELA Chat Widget] Display DESPUES:', self.elements.formFields.style.display);
                                console.log('[JELA Chat Widget] ✓ Campos de formulario OCULTADOS');
                            } else {
                                console.log('[JELA Chat Widget] ✗ Faltan datos, mostrando campos');
                            }
                        } else {
                            console.log('[JELA Chat Widget] ✗ Elementos no encontrados');
                        }
                    }
                })
                .catch(error => {
                    console.log('[JELA Chat Widget] ✗ Error o usuario no autenticado:', error.message);
                    self.state.isAuthenticated = false;
                    // Mostrar campos para usuarios no autenticados
                    if (self.elements.formFields) {
                        self.elements.formFields.style.display = 'block';
                        console.log('[JELA Chat Widget] Mostrando campos para usuario no autenticado');
                    }
                });
        },
        
        // ========================================
        // CREACIÓN DEL WIDGET
        // ========================================
        
        createWidget: function() {
            var self = this;
            
            // Contenedor principal
            var container = document.createElement('div');
            container.id = 'jela-chat-widget';
            container.className = 'jela-chat-widget jela-chat-' + this.config.position + ' jela-chat-theme-' + this.config.theme;
            
            // Botón flotante
            var button = document.createElement('div');
            button.className = 'jela-chat-button';
            button.innerHTML = `
                <i class="fas fa-comments"></i>
                <span class="jela-chat-badge" style="display: none;">0</span>
            `;
            
            // Ventana de chat
            var window = document.createElement('div');
            window.className = 'jela-chat-window';
            window.style.display = 'none';
            window.innerHTML = `
                <div class="jela-chat-header">
                    <div class="jela-chat-header-content">
                        <i class="fas fa-robot"></i>
                        <div class="jela-chat-header-text">
                            <h4>Asistente JELA</h4>
                            <span class="jela-chat-status">En línea</span>
                        </div>
                    </div>
                    <div class="jela-chat-header-actions">
                        <button class="jela-chat-minimize" title="Minimizar">
                            <i class="fas fa-minus"></i>
                        </button>
                        <button class="jela-chat-close" title="Cerrar">
                            <i class="fas fa-times"></i>
                        </button>
                    </div>
                </div>
                
                <div class="jela-chat-body">
                    <div class="jela-chat-messages" id="jela-chat-messages">
                        <div class="jela-chat-welcome">
                            <i class="fas fa-robot"></i>
                            <h3>¡Hola! 👋</h3>
                            <p>Soy el asistente virtual de JELA. ¿En qué puedo ayudarte hoy?</p>
                        </div>
                    </div>
                </div>
                
                <div class="jela-chat-footer">
                    <form class="jela-chat-form" id="jela-chat-form">
                        <div class="jela-chat-form-group" id="jela-chat-form-fields" style="display: none;">
                            <input type="text" 
                                   id="jela-chat-nombre" 
                                   class="jela-chat-input" 
                                   placeholder="Tu nombre" 
                                   autocomplete="name" />
                            <input type="email" 
                                   id="jela-chat-email" 
                                   class="jela-chat-input" 
                                   placeholder="Tu email" 
                                   autocomplete="email" />
                        </div>
                        <div class="jela-chat-input-group">
                            <textarea id="jela-chat-mensaje" 
                                      class="jela-chat-textarea" 
                                      placeholder="Escribe tu mensaje..." 
                                      rows="1"
                                      required></textarea>
                            <button type="submit" class="jela-chat-send" title="Enviar">
                                <i class="fas fa-paper-plane"></i>
                            </button>
                        </div>
                    </form>
                    ${this.config.showBranding ? '<div class="jela-chat-branding">Powered by <strong>JELA BBC</strong></div>' : ''}
                </div>
            `;
            
            // Ensamblar widget
            container.appendChild(button);
            container.appendChild(window);
            document.body.appendChild(container);
            
            // Guardar referencias
            this.elements = {
                container: container,
                button: button,
                window: window,
                header: window.querySelector('.jela-chat-header'),
                messages: window.querySelector('.jela-chat-messages'),
                form: window.querySelector('.jela-chat-form'),
                formFields: window.querySelector('#jela-chat-form-fields'),
                nombre: window.querySelector('#jela-chat-nombre'),
                email: window.querySelector('#jela-chat-email'),
                mensaje: window.querySelector('#jela-chat-mensaje'),
                sendButton: window.querySelector('.jela-chat-send'),
                closeButton: window.querySelector('.jela-chat-close'),
                minimizeButton: window.querySelector('.jela-chat-minimize'),
                badge: button.querySelector('.jela-chat-badge')
            };
        },
        
        // ========================================
        // EVENTOS
        // ========================================
        
        registerEvents: function() {
            var self = this;
            
            // Abrir/cerrar widget
            this.elements.button.addEventListener('click', function() {
                self.toggleWidget();
            });
            
            // Cerrar widget
            this.elements.closeButton.addEventListener('click', function() {
                self.closeWidget();
            });
            
            // Minimizar widget
            this.elements.minimizeButton.addEventListener('click', function() {
                self.minimizeWidget();
            });
            
            // Enviar mensaje
            this.elements.form.addEventListener('submit', function(e) {
                e.preventDefault();
                self.sendMessage();
            });
            
            // Auto-resize del textarea
            this.elements.mensaje.addEventListener('input', function() {
                this.style.height = 'auto';
                this.style.height = Math.min(this.scrollHeight, 120) + 'px';
            });
            
            // Enter para enviar (Shift+Enter para nueva línea)
            this.elements.mensaje.addEventListener('keydown', function(e) {
                if (e.key === 'Enter' && !e.shiftKey) {
                    e.preventDefault();
                    self.elements.form.dispatchEvent(new Event('submit'));
                }
            });
        },
        
        // ========================================
        // CONTROL DEL WIDGET
        // ========================================
        
        toggleWidget: function() {
            if (this.state.isOpen) {
                this.closeWidget();
            } else {
                this.openWidget();
            }
        },
        
        openWidget: function() {
            this.elements.window.style.display = 'flex';
            this.elements.container.classList.add('jela-chat-open');
            this.state.isOpen = true;
            this.state.isMinimized = false;
            
            // Ocultar badge
            this.elements.badge.style.display = 'none';
            this.elements.badge.textContent = '0';
            
            // Manejar visibilidad de campos y focus
            setTimeout(() => {
                if (this.state.isAuthenticated && this.state.userInfo) {
                    // Usuario autenticado: ocultar campos y focus en mensaje
                    this.elements.formFields.style.display = 'none';
                    this.elements.mensaje.focus();
                    console.log('[JELA Chat Widget] openWidget: Usuario autenticado, campos ocultos');
                } else {
                    // Usuario no autenticado: mostrar campos si están vacíos
                    var nombreVacio = !this.elements.nombre.value || this.elements.nombre.value.trim() === '';
                    var emailVacio = !this.elements.email.value || this.elements.email.value.trim() === '';
                    
                    if (nombreVacio || emailVacio) {
                        this.elements.formFields.style.display = 'block';
                        if (nombreVacio) {
                            this.elements.nombre.focus();
                        } else if (emailVacio) {
                            this.elements.email.focus();
                        } else {
                            this.elements.mensaje.focus();
                        }
                        console.log('[JELA Chat Widget] openWidget: Usuario no autenticado, mostrando campos');
                    } else {
                        // Campos ya llenos, ocultar y focus en mensaje
                        this.elements.formFields.style.display = 'none';
                        this.elements.mensaje.focus();
                        console.log('[JELA Chat Widget] openWidget: Campos ya llenos, ocultando');
                    }
                }
            }, 300);
            
            // Scroll al último mensaje
            this.scrollToBottom();
        },
        
        closeWidget: function() {
            this.elements.window.style.display = 'none';
            this.elements.container.classList.remove('jela-chat-open');
            this.state.isOpen = false;
            this.state.isMinimized = false;
        },
        
        minimizeWidget: function() {
            this.elements.container.classList.add('jela-chat-minimized');
            this.state.isMinimized = true;
            
            setTimeout(() => {
                this.closeWidget();
                this.elements.container.classList.remove('jela-chat-minimized');
            }, 300);
        },
        
        // ========================================
        // MENSAJES
        // ========================================
        
        sendMessage: function() {
            var self = this;
            
            // Validar rate limiting
            if (!this.checkRateLimit()) {
                this.showError('Has alcanzado el límite de mensajes por hora. Por favor, intenta más tarde.');
                return;
            }
            
            // Obtener datos del formulario o del usuario autenticado
            var nombre, email;
            
            if (this.state.isAuthenticated && this.state.userInfo) {
                // Usuario autenticado: usar datos de la sesión
                nombre = this.state.userInfo.Nombre;
                email = this.state.userInfo.Email;
                console.log('[JELA Chat Widget] Usando datos de usuario autenticado:', nombre, email);
            } else {
                // Usuario no autenticado: usar datos del formulario
                nombre = this.elements.nombre.value.trim();
                email = this.elements.email.value.trim();
                
                // Validar que los campos estén llenos
                if (!nombre) {
                    this.showError('Por favor, ingresa tu nombre.');
                    this.elements.formFields.style.display = 'block';
                    this.elements.nombre.focus();
                    return;
                }
                
                if (!email) {
                    this.showError('Por favor, ingresa tu email.');
                    this.elements.formFields.style.display = 'block';
                    this.elements.email.focus();
                    return;
                }
                
                // Validar email
                if (!this.validateEmail(email)) {
                    this.showError('Por favor, ingresa un email válido.');
                    this.elements.formFields.style.display = 'block';
                    this.elements.email.focus();
                    return;
                }
                
                console.log('[JELA Chat Widget] Usando datos del formulario:', nombre, email);
            }
            
            var mensaje = this.elements.mensaje.value.trim();
            
            if (!mensaje) {
                this.elements.mensaje.focus();
                return;
            }
            
            // Deshabilitar formulario
            this.setFormEnabled(false);
            this.state.isWaitingResponse = true;
            
            // Agregar mensaje del usuario
            this.addMessage('user', mensaje, nombre);
            
            // Limpiar textarea
            this.elements.mensaje.value = '';
            this.elements.mensaje.style.height = 'auto';
            
            // Mostrar indicador de escritura
            this.showTypingIndicator();
            
            // Enviar a la API
            var payload = {
                Nombre: nombre,
                Email: email,
                Mensaje: mensaje,
                IPOrigen: this.state.clientIP || 'unknown',
                IdEntidad: this.state.isAuthenticated ? this.state.userInfo.IdEntidad : this.config.idEntidad,
                SessionId: this.state.sessionId
            };
            
            console.log('[JELA Chat Widget] Enviando payload:', payload);
            
            fetch(this.config.apiUrl + '/api/webhooks/chatweb', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(payload)
            })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Error en la respuesta del servidor: ' + response.status);
                }
                return response.json();
            })
            .then(data => {
                self.hideTypingIndicator();
                
                if (data.Success) {
                    // Guardar ticket ID
                    if (data.TicketId) {
                        self.state.ticketId = data.TicketId;
                    }
                    
                    // Mostrar respuesta de IA
                    if (data.RespuestaIA) {
                        self.addMessage('bot', data.RespuestaIA);
                    } else if (data.Mensaje) {
                        self.addMessage('bot', data.Mensaje);
                    }
                    
                    // Incrementar contador de rate limiting
                    self.incrementRateLimit();
                    
                } else {
                    self.showError(data.Mensaje || 'Error al procesar tu mensaje. Por favor, intenta de nuevo.');
                }
            })
            .catch(error => {
                console.error('[JELA Chat Widget] Error:', error);
                self.hideTypingIndicator();
                self.showError('No se pudo conectar con el servidor. Por favor, verifica tu conexión e intenta de nuevo.');
            })
            .finally(() => {
                self.setFormEnabled(true);
                self.state.isWaitingResponse = false;
                self.elements.mensaje.focus();
            });
        },
        
        addMessage: function(type, text, senderName) {
            var messageId = this.generateMessageId();
            var timestamp = new Date();
            
            // Crear elemento de mensaje
            var messageDiv = document.createElement('div');
            messageDiv.className = 'jela-chat-message jela-chat-message-' + type;
            messageDiv.setAttribute('data-message-id', messageId);
            
            var avatar = type === 'user' 
                ? '<i class="fas fa-user"></i>' 
                : '<i class="fas fa-robot"></i>';
            
            var name = type === 'user' 
                ? (senderName || 'Tú') 
                : 'Asistente JELA';
            
            messageDiv.innerHTML = `
                <div class="jela-chat-message-avatar">${avatar}</div>
                <div class="jela-chat-message-content">
                    <div class="jela-chat-message-header">
                        <span class="jela-chat-message-name">${this.escapeHtml(name)}</span>
                        <span class="jela-chat-message-time">${this.formatTime(timestamp)}</span>
                    </div>
                    <div class="jela-chat-message-text">${this.formatMessageText(text)}</div>
                </div>
            `;
            
            // Remover mensaje de bienvenida si existe
            var welcome = this.elements.messages.querySelector('.jela-chat-welcome');
            if (welcome) {
                welcome.remove();
            }
            
            // Agregar mensaje
            this.elements.messages.appendChild(messageDiv);
            
            // Guardar en estado
            this.state.messages.push({
                id: messageId,
                type: type,
                text: text,
                senderName: senderName,
                timestamp: timestamp.toISOString()
            });
            
            // Guardar en sessionStorage
            this.saveSessionHistory();
            
            // Scroll al final
            this.scrollToBottom();
            
            // Mostrar badge si está cerrado
            if (!this.state.isOpen && type === 'bot') {
                var count = parseInt(this.elements.badge.textContent) || 0;
                this.elements.badge.textContent = count + 1;
                this.elements.badge.style.display = 'flex';
            }
        },
        
        showTypingIndicator: function() {
            var indicator = document.createElement('div');
            indicator.className = 'jela-chat-typing-indicator';
            indicator.id = 'jela-chat-typing';
            indicator.innerHTML = `
                <div class="jela-chat-message jela-chat-message-bot">
                    <div class="jela-chat-message-avatar"><i class="fas fa-robot"></i></div>
                    <div class="jela-chat-message-content">
                        <div class="jela-chat-typing-dots">
                            <span></span><span></span><span></span>
                        </div>
                    </div>
                </div>
            `;
            this.elements.messages.appendChild(indicator);
            this.scrollToBottom();
        },
        
        hideTypingIndicator: function() {
            var indicator = document.getElementById('jela-chat-typing');
            if (indicator) {
                indicator.remove();
            }
        },
        
        showError: function(message) {
            this.addMessage('system', '⚠️ ' + message);
        },
        
        // ========================================
        // UTILIDADES
        // ========================================
        
        setFormEnabled: function(enabled) {
            this.elements.nombre.disabled = !enabled;
            this.elements.email.disabled = !enabled;
            this.elements.mensaje.disabled = !enabled;
            this.elements.sendButton.disabled = !enabled;
            
            if (enabled) {
                this.elements.sendButton.classList.remove('jela-chat-send-disabled');
            } else {
                this.elements.sendButton.classList.add('jela-chat-send-disabled');
            }
        },
        
        scrollToBottom: function() {
            setTimeout(() => {
                this.elements.messages.scrollTop = this.elements.messages.scrollHeight;
            }, 100);
        },
        
        validateEmail: function(email) {
            var re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            return re.test(email);
        },
        
        escapeHtml: function(text) {
            var div = document.createElement('div');
            div.textContent = text;
            return div.innerHTML;
        },
        
        formatMessageText: function(text) {
            // Convertir URLs a links
            text = this.escapeHtml(text);
            text = text.replace(/(https?:\/\/[^\s]+)/g, '<a href="$1" target="_blank" rel="noopener">$1</a>');
            
            // Convertir saltos de línea
            text = text.replace(/\n/g, '<br>');
            
            return text;
        },
        
        formatTime: function(date) {
            var hours = date.getHours();
            var minutes = date.getMinutes();
            var ampm = hours >= 12 ? 'PM' : 'AM';
            hours = hours % 12;
            hours = hours ? hours : 12;
            minutes = minutes < 10 ? '0' + minutes : minutes;
            return hours + ':' + minutes + ' ' + ampm;
        },
        
        // ========================================
        // RATE LIMITING
        // ========================================
        
        checkRateLimit: function() {
            var now = Date.now();
            var key = 'jela_chat_rate_limit';
            var data = JSON.parse(localStorage.getItem(key) || '{"count": 0, "timestamp": 0}');
            
            // Resetear si pasó la ventana de tiempo
            if (now - data.timestamp > this.config.rateLimitWindow) {
                data = { count: 0, timestamp: now };
                localStorage.setItem(key, JSON.stringify(data));
            }
            
            // Verificar límite
            return data.count < this.config.rateLimitMessages;
        },
        
        incrementRateLimit: function() {
            var now = Date.now();
            var key = 'jela_chat_rate_limit';
            var data = JSON.parse(localStorage.getItem(key) || '{"count": 0, "timestamp": 0}');
            
            // Resetear si pasó la ventana de tiempo
            if (now - data.timestamp > this.config.rateLimitWindow) {
                data = { count: 1, timestamp: now };
            } else {
                data.count++;
            }
            
            localStorage.setItem(key, JSON.stringify(data));
        },
        
        // ========================================
        // PERSISTENCIA DE SESIÓN
        // ========================================
        
        saveSessionHistory: function() {
            try {
                var data = {
                    sessionId: this.state.sessionId,
                    ticketId: this.state.ticketId,
                    messages: this.state.messages,
                    nombre: this.elements.nombre.value,
                    email: this.elements.email.value
                };
                sessionStorage.setItem('jela_chat_session', JSON.stringify(data));
            } catch (e) {
                console.warn('[JELA Chat Widget] No se pudo guardar el historial:', e);
            }
        },
        
        loadSessionHistory: function() {
            try {
                var data = JSON.parse(sessionStorage.getItem('jela_chat_session'));
                if (data && data.sessionId) {
                    this.state.sessionId = data.sessionId;
                    this.state.ticketId = data.ticketId;
                    this.state.messages = data.messages || [];
                    
                    // Restaurar campos del formulario
                    if (data.nombre) {
                        setTimeout(() => {
                            this.elements.nombre.value = data.nombre;
                        }, 100);
                    }
                    if (data.email) {
                        setTimeout(() => {
                            this.elements.email.value = data.email;
                        }, 100);
                    }
                    
                    // Restaurar mensajes
                    if (this.state.messages.length > 0) {
                        setTimeout(() => {
                            this.restoreMessages();
                        }, 200);
                    }
                }
            } catch (e) {
                console.warn('[JELA Chat Widget] No se pudo cargar el historial:', e);
            }
        },
        
        restoreMessages: function() {
            // Remover mensaje de bienvenida
            var welcome = this.elements.messages.querySelector('.jela-chat-welcome');
            if (welcome) {
                welcome.remove();
            }
            
            // Ocultar campos si ya hay mensajes
            if (this.state.messages.length > 0) {
                this.elements.formFields.style.display = 'none';
            }
            
            // Restaurar cada mensaje
            this.state.messages.forEach(msg => {
                var messageDiv = document.createElement('div');
                messageDiv.className = 'jela-chat-message jela-chat-message-' + msg.type;
                messageDiv.setAttribute('data-message-id', msg.id);
                
                var avatar = msg.type === 'user' 
                    ? '<i class="fas fa-user"></i>' 
                    : '<i class="fas fa-robot"></i>';
                
                var name = msg.type === 'user' 
                    ? (msg.senderName || 'Tú') 
                    : 'Asistente JELA';
                
                var timestamp = new Date(msg.timestamp);
                
                messageDiv.innerHTML = `
                    <div class="jela-chat-message-avatar">${avatar}</div>
                    <div class="jela-chat-message-content">
                        <div class="jela-chat-message-header">
                            <span class="jela-chat-message-name">${this.escapeHtml(name)}</span>
                            <span class="jela-chat-message-time">${this.formatTime(timestamp)}</span>
                        </div>
                        <div class="jela-chat-message-text">${this.formatMessageText(msg.text)}</div>
                    </div>
                `;
                
                this.elements.messages.appendChild(messageDiv);
            });
            
            this.scrollToBottom();
        }
    };
    
    // Exponer globalmente
    window.JelaChatWidget = JelaChatWidget;
    
})(window);
