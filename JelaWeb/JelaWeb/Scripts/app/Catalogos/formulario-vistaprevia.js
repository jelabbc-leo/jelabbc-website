/**
 * Vista Previa de Formularios - JavaScript
 * Renderiza formularios dinámicos en vista web y móvil
 */

var camposFormulario = [];
var plataformaActual = 'web';
var plataformasDisponibles = ['web', 'movil'];

// Mapeo de tipos a iconos
var iconosPorTipo = {
    'foto': 'fa-camera',
    'archivo': 'fa-file-upload',
    'firma': 'fa-signature'
};

/**
 * Inicializa la vista previa con los campos del formulario
 */
function inicializarVistaPrevia(camposJSON, plataformas) {
    try {
        // Parsear campos
        var data = JSON.parse(camposJSON);
        
        // Manejar nueva estructura con filas o estructura plana
        if (data && data.campos) {
            camposFormulario = data.campos;
        } else if (Array.isArray(data)) {
            camposFormulario = data;
        } else {
            camposFormulario = [];
        }
        
        // Configurar plataformas disponibles
        if (plataformas) {
            plataformasDisponibles = plataformas.split(',').map(function(p) { return p.trim().toLowerCase(); });
        }
        
        // Configurar botones de plataforma
        configurarBotonesPlataforma();
        
        // Renderizar formulario
        renderizarFormulario();
        
    } catch (ex) {
        console.error('Error al inicializar vista previa:', ex);
    }
}

/**
 * Configura los botones de plataforma según las disponibles
 */
function configurarBotonesPlataforma() {
    var btnWeb = document.getElementById('btnWeb');
    var btnMovil = document.getElementById('btnMovil');
    
    if (btnWeb) {
        btnWeb.style.display = plataformasDisponibles.includes('web') ? 'flex' : 'none';
    }
    if (btnMovil) {
        btnMovil.style.display = plataformasDisponibles.includes('movil') ? 'flex' : 'none';
    }
    
    // Si solo hay una plataforma, seleccionarla automáticamente
    if (plataformasDisponibles.length === 1) {
        cambiarPlataforma(plataformasDisponibles[0]);
    } else if (!plataformasDisponibles.includes(plataformaActual)) {
        cambiarPlataforma(plataformasDisponibles[0] || 'web');
    }
}

/**
 * Cambia entre vista web y móvil
 */
function cambiarPlataforma(plataforma) {
    plataformaActual = plataforma;
    
    // Actualizar botones
    document.querySelectorAll('.platform-btn').forEach(function(btn) {
        btn.classList.remove('active');
    });
    
    var btnActivo = document.getElementById('btn' + plataforma.charAt(0).toUpperCase() + plataforma.slice(1));
    if (btnActivo) {
        btnActivo.classList.add('active');
    }
    
    // Mostrar/ocultar vistas
    var webPreview = document.getElementById('webPreview');
    var movilPreview = document.getElementById('movilPreview');
    
    if (webPreview) webPreview.style.display = plataforma === 'web' ? 'block' : 'none';
    if (movilPreview) movilPreview.style.display = plataforma === 'movil' ? 'flex' : 'none';
    
    // Re-renderizar
    renderizarFormulario();
}

/**
 * Renderiza el formulario en el contenedor correspondiente
 */
function renderizarFormulario() {
    var containerId = plataformaActual === 'web' ? 'webFormContent' : 'movilFormContent';
    var container = document.getElementById(containerId);
    
    if (!container) return;
    
    if (camposFormulario.length === 0) {
        container.innerHTML = '<div class="form-preview"><p style="text-align:center;color:#999;padding:50px;">No hay campos en este formulario</p></div>';
        return;
    }
    
    // Agrupar campos por sección
    var secciones = agruparPorSeccion(camposFormulario);
    
    // Verificar si hay botones de acción definidos
    var tieneBotones = camposFormulario.some(function(c) {
        return c.tipo === 'boton_guardar' || c.tipo === 'boton_cancelar';
    });
    
    var html = '<div class="form-preview">';
    
    // Renderizar cada sección
    Object.keys(secciones).forEach(function(seccion) {
        html += '<div class="form-section">';
        
        if (seccion !== 'General' || Object.keys(secciones).length > 1) {
            html += '<div class="section-title">' + escapeHtml(seccion) + '</div>';
        }
        
        html += '<div class="form-row">';
        
        secciones[seccion].forEach(function(campo) {
            html += renderizarCampo(campo);
        });
        
        html += '</div></div>';
    });
    
    // Solo agregar botón de envío por defecto si no hay botones definidos
    if (!tieneBotones) {
        html += '<button type="button" class="btn-submit" onclick="simularEnvio()"><i class="fas fa-paper-plane"></i> Enviar Formulario</button>';
    }
    
    html += '</div>';
    
    container.innerHTML = html;
}

/**
 * Agrupa los campos por sección
 */
function agruparPorSeccion(campos) {
    var secciones = {};
    
    campos.forEach(function(campo) {
        var seccion = campo.seccion || 'General';
        if (!secciones[seccion]) {
            secciones[seccion] = [];
        }
        secciones[seccion].push(campo);
    });
    
    return secciones;
}

/**
 * Renderiza un campo individual
 */
function renderizarCampo(campo) {
    var ancho = plataformaActual === 'movil' ? 12 : (campo.ancho || 12);
    var requerido = campo.requerido ? '<span class="required">*</span>' : '';
    var placeholder = campo.placeholder || '';
    
    var html = '<div class="form-group col-' + ancho + '">';
    html += '<label>' + escapeHtml(campo.etiqueta) + requerido + '</label>';
    
    switch (campo.tipo) {
        case 'texto':
            html += '<input type="text" class="form-control" placeholder="' + escapeHtml(placeholder) + '"' + (campo.requerido ? ' required' : '') + ' />';
            break;
            
        case 'numero':
            html += '<input type="number" class="form-control" placeholder="' + escapeHtml(placeholder) + '"' + (campo.requerido ? ' required' : '') + ' />';
            break;
            
        case 'decimal':
            html += '<input type="number" step="0.01" class="form-control" placeholder="' + escapeHtml(placeholder) + '"' + (campo.requerido ? ' required' : '') + ' />';
            break;
            
        case 'fecha':
            html += '<input type="date" class="form-control"' + (campo.requerido ? ' required' : '') + ' />';
            break;
            
        case 'fecha_hora':
            html += '<input type="datetime-local" class="form-control"' + (campo.requerido ? ' required' : '') + ' />';
            break;
            
        case 'hora':
            html += '<input type="time" class="form-control"' + (campo.requerido ? ' required' : '') + ' />';
            break;
            
        case 'dropdown':
            html += '<select class="form-control"' + (campo.requerido ? ' required' : '') + '>';
            html += '<option value="">Seleccione una opción...</option>';
            html += '<option value="1">Opción 1</option>';
            html += '<option value="2">Opción 2</option>';
            html += '<option value="3">Opción 3</option>';
            html += '</select>';
            break;
            
        case 'radio':
            html += '<div class="radio-group">';
            html += '<label class="radio-item"><input type="radio" name="' + campo.nombre + '" value="1" /> Opción 1</label>';
            html += '<label class="radio-item"><input type="radio" name="' + campo.nombre + '" value="2" /> Opción 2</label>';
            html += '<label class="radio-item"><input type="radio" name="' + campo.nombre + '" value="3" /> Opción 3</label>';
            html += '</div>';
            break;
            
        case 'checkbox':
            html += '<div class="checkbox-group">';
            html += '<label class="checkbox-item"><input type="checkbox" /> Sí</label>';
            html += '</div>';
            break;
            
        case 'textarea':
            var textareaHeight = campo.altura || 150;
            html += '<textarea class="form-control" style="min-height:' + textareaHeight + 'px;height:' + textareaHeight + 'px;" placeholder="' + escapeHtml(placeholder) + '"' + (campo.requerido ? ' required' : '') + '></textarea>';
            break;
            
        case 'foto':
            var fotoHeight = campo.altura || 150;
            html += '<div class="file-upload" style="min-height:' + fotoHeight + 'px;" onclick="simularSubida(\'foto\')">';
            html += '<i class="fas fa-camera"></i>';
            html += '<p>Toca para tomar o seleccionar una foto</p>';
            html += '</div>';
            break;
            
        case 'archivo':
            html += '<div class="file-upload" onclick="simularSubida(\'archivo\')">';
            html += '<i class="fas fa-file-upload"></i>';
            html += '<p>Arrastra un archivo o haz clic para seleccionar</p>';
            html += '</div>';
            break;
            
        case 'firma':
            var firmaHeight = campo.altura || 150;
            html += '<div class="signature-pad" style="height:' + firmaHeight + 'px;">';
            html += '<i class="fas fa-signature"></i> Toca para firmar';
            html += '</div>';
            break;
            
        case 'boton_guardar':
            html += '<button type="button" class="btn-action btn-guardar" onclick="simularGuardar()">';
            html += '<i class="fas fa-save"></i> ' + escapeHtml(campo.etiqueta);
            html += '</button>';
            break;
            
        case 'boton_cancelar':
            html += '<button type="button" class="btn-action btn-cancelar" onclick="simularCancelar()">';
            html += '<i class="fas fa-times"></i> ' + escapeHtml(campo.etiqueta);
            html += '</button>';
            break;
            
        default:
            html += '<input type="text" class="form-control" placeholder="' + escapeHtml(placeholder) + '" />';
    }
    
    html += '</div>';
    return html;
}

/**
 * Simula el envío del formulario
 */
function simularEnvio() {
    alert('Vista previa: El formulario se enviaría aquí.\n\nEsta es solo una vista previa para visualizar cómo se verá el formulario.');
}

/**
 * Simula guardar el formulario
 */
function simularGuardar() {
    alert('Vista previa: Botón GUARDAR presionado.\n\nEn el formulario real, aquí se guardarían los datos capturados.');
}

/**
 * Simula cancelar el formulario
 */
function simularCancelar() {
    if (confirm('Vista previa: Botón CANCELAR presionado.\n\n¿Desea simular el cierre del formulario?')) {
        alert('En el formulario real, aquí se cerraría el formulario sin guardar.');
    }
}

/**
 * Simula la subida de archivos
 */
function simularSubida(tipo) {
    var mensaje = tipo === 'foto' 
        ? 'Vista previa: Aquí se abriría la cámara o el selector de fotos.'
        : 'Vista previa: Aquí se abriría el selector de archivos.';
    alert(mensaje);
}

/**
 * Escapa HTML para prevenir XSS
 */
function escapeHtml(text) {
    if (!text) return '';
    var div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

// Exponer funciones globalmente
window.inicializarVistaPrevia = inicializarVistaPrevia;
window.cambiarPlataforma = cambiarPlataforma;
window.simularEnvio = simularEnvio;
window.simularSubida = simularSubida;
