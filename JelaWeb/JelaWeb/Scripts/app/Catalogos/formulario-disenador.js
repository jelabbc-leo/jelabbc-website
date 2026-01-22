/**
 * Diseñador de Formularios - JavaScript
 * Drag & Drop con soporte para Grid de 12 columnas y posicionamiento horizontal
 */

// Variables globales
var camposFormulario = [];
var filasFormulario = []; // Array de filas, cada fila contiene array de campos
var campoSeleccionado = null;
var contadorCampos = 0;
var contadorFilas = 0;
var draggedField = null; // Campo que se está arrastrando (reordenamiento)

// Iconos por tipo
var iconosPorTipo = {
    'texto': 'fa-font', 'numero': 'fa-hashtag', 'decimal': 'fa-percentage',
    'fecha': 'fa-calendar', 'fecha_hora': 'fa-calendar-alt', 'hora': 'fa-clock',
    'dropdown': 'fa-caret-square-down', 'radio': 'fa-dot-circle', 'checkbox': 'fa-check-square',
    'textarea': 'fa-align-left', 'foto': 'fa-camera', 'archivo': 'fa-file-upload', 'firma': 'fa-signature',
    'boton_guardar': 'fa-save', 'boton_cancelar': 'fa-times-circle'
};

var nombresTipo = {
    'texto': 'Texto', 'numero': 'Número', 'decimal': 'Decimal',
    'fecha': 'Fecha', 'fecha_hora': 'Fecha/Hora', 'hora': 'Hora',
    'dropdown': 'Lista', 'radio': 'Opciones', 'checkbox': 'Casilla',
    'textarea': 'Área Texto', 'foto': 'Foto', 'archivo': 'Archivo', 'firma': 'Firma',
    'boton_guardar': 'Botón Guardar', 'boton_cancelar': 'Botón Cancelar'
};

// Tipos que son botones de acción (no capturan datos)
var tiposBotones = ['boton_guardar', 'boton_cancelar'];

// Inicialización
document.addEventListener('DOMContentLoaded', function() {
    inicializarDragDrop();
    renderizarCanvas();
});

function inicializarDragDrop() {
    // Toolbox items
    document.querySelectorAll('.toolbox-item').forEach(function(item) {
        item.addEventListener('dragstart', onToolboxDragStart);
        item.addEventListener('dragend', onDragEnd);
    });

    // Canvas general
    var canvas = document.getElementById('designerCanvas');
    if (canvas) {
        canvas.addEventListener('dragover', onCanvasDragOver);
        canvas.addEventListener('dragleave', onCanvasDragLeave);
        canvas.addEventListener('drop', onCanvasDrop);
    }
}

// ========================================
// DRAG HANDLERS - TOOLBOX
// ========================================
function onToolboxDragStart(e) {
    e.target.classList.add('dragging');
    e.dataTransfer.setData('application/json', JSON.stringify({
        source: 'toolbox',
        tipo: e.target.dataset.tipo,
        etiqueta: e.target.dataset.etiqueta
    }));
    e.dataTransfer.effectAllowed = 'copy';
}

function onDragEnd(e) {
    e.target.classList.remove('dragging');
    limpiarIndicadoresDrop();
}

// ========================================
// DRAG HANDLERS - CAMPOS EXISTENTES
// ========================================
function onFieldDragStart(e, campoId) {
    e.stopPropagation();
    draggedField = campoId;
    e.target.classList.add('dragging');
    e.dataTransfer.setData('application/json', JSON.stringify({
        source: 'canvas',
        campoId: campoId
    }));
    e.dataTransfer.effectAllowed = 'move';
}

function onFieldDragEnd(e) {
    e.target.classList.remove('dragging');
    draggedField = null;
    limpiarIndicadoresDrop();
}

// ========================================
// DRAG HANDLERS - CANVAS
// ========================================
function onCanvasDragOver(e) {
    e.preventDefault();
    e.dataTransfer.dropEffect = 'copy';
}

function onCanvasDragLeave(e) {
    // Solo limpiar si realmente salimos del canvas
    if (!e.currentTarget.contains(e.relatedTarget)) {
        limpiarIndicadoresDrop();
    }
}

function onCanvasDrop(e) {
    e.preventDefault();
    e.stopPropagation();
    limpiarIndicadoresDrop();
    
    // Verificar si el drop fue en un elemento hijo (fila, campo, zona nueva fila)
    // Si es así, no procesar aquí - el hijo lo manejará
    var target = e.target;
    if (target.closest('.designer-row') || target.closest('.designer-field') || target.closest('.new-row-drop-zone')) {
        return; // El evento será manejado por el elemento específico
    }
    
    try {
        var data = JSON.parse(e.dataTransfer.getData('application/json'));
        
        if (data.source === 'toolbox') {
            // Nuevo campo desde toolbox - crear en nueva fila al final
            agregarCampoNuevaFila(data.tipo, data.etiqueta);
        }
    } catch (ex) {
        console.error('Error en canvas drop:', ex);
    }
}

// ========================================
// DRAG HANDLERS - FILAS
// ========================================
function onRowDragOver(e, filaIndex) {
    e.preventDefault();
    e.stopPropagation();
    
    var row = e.currentTarget;
    var rect = row.getBoundingClientRect();
    var x = e.clientX - rect.left;
    var anchoUsado = calcularAnchoUsadoEnFila(filaIndex);
    
    // Si hay espacio en la fila, permitir drop
    if (anchoUsado < 12) {
        row.classList.add('drag-over-row');
        e.dataTransfer.dropEffect = 'copy';
    }
}

function onRowDragLeave(e) {
    e.currentTarget.classList.remove('drag-over-row');
}

function onRowDrop(e, filaIndex) {
    e.preventDefault();
    e.stopPropagation();
    e.currentTarget.classList.remove('drag-over-row');
    
    // Si el drop fue directamente sobre un campo, no procesar aquí
    var target = e.target;
    if (target.closest('.designer-field')) {
        return; // El campo lo manejará
    }
    
    try {
        var data = JSON.parse(e.dataTransfer.getData('application/json'));
        
        if (data.source === 'toolbox') {
            agregarCampoEnFila(data.tipo, data.etiqueta, filaIndex);
        } else if (data.source === 'canvas' && data.campoId) {
            moverCampoAFila(data.campoId, filaIndex);
        }
    } catch (ex) {
        console.error('Error en row drop:', ex);
    }
    
    limpiarIndicadoresDrop();
}

// ========================================
// DRAG HANDLERS - CAMPOS (para insertar al lado)
// ========================================
function onFieldDragOver(e, campoId) {
    e.preventDefault();
    e.stopPropagation();
    
    var field = e.currentTarget;
    var rect = field.getBoundingClientRect();
    var x = e.clientX - rect.left;
    var mitad = rect.width / 2;
    
    // Limpiar clases anteriores
    field.classList.remove('drop-left', 'drop-right');
    
    // Determinar si soltar a la izquierda o derecha
    if (x < mitad) {
        field.classList.add('drop-left');
    } else {
        field.classList.add('drop-right');
    }
    
    e.dataTransfer.dropEffect = 'copy';
}

function onFieldDragLeave(e) {
    e.currentTarget.classList.remove('drop-left', 'drop-right');
}

function onFieldDrop(e, campoId) {
    e.preventDefault();
    e.stopPropagation();
    
    var field = e.currentTarget;
    var insertarAntes = field.classList.contains('drop-left');
    
    field.classList.remove('drop-left', 'drop-right');
    
    try {
        var data = JSON.parse(e.dataTransfer.getData('application/json'));
        
        if (data.source === 'toolbox') {
            agregarCampoJuntoA(data.tipo, data.etiqueta, campoId, insertarAntes);
        } else if (data.source === 'canvas' && data.campoId && data.campoId !== campoId) {
            moverCampoJuntoA(data.campoId, campoId, insertarAntes);
        }
    } catch (ex) {
        console.error('Error en field drop:', ex);
    }
    
    limpiarIndicadoresDrop();
}

// ========================================
// DRAG HANDLERS - ZONA NUEVA FILA
// ========================================
function onNewRowDragOver(e) {
    e.preventDefault();
    e.stopPropagation();
    e.currentTarget.classList.add('drag-over-zone');
    e.dataTransfer.dropEffect = 'copy';
}

function onNewRowDragLeave(e) {
    e.currentTarget.classList.remove('drag-over-zone');
}

function onNewRowDrop(e) {
    e.preventDefault();
    e.stopPropagation();
    e.currentTarget.classList.remove('drag-over-zone');
    
    try {
        var data = JSON.parse(e.dataTransfer.getData('application/json'));
        
        if (data.source === 'toolbox') {
            agregarCampoNuevaFila(data.tipo, data.etiqueta);
        } else if (data.source === 'canvas' && data.campoId) {
            moverCampoANuevaFila(data.campoId);
        }
    } catch (ex) {
        console.error('Error en new row drop:', ex);
    }
    
    limpiarIndicadoresDrop();
}

// ========================================
// GESTIÓN DE CAMPOS Y FILAS
// ========================================
function agregarCampoNuevaFila(tipo, etiquetaBase) {
    contadorCampos++;
    contadorFilas++;
    
    var campo = crearCampo(tipo, etiquetaBase);
    var nuevaFila = {
        id: 'fila_' + contadorFilas,
        campos: [campo]
    };
    
    filasFormulario.push(nuevaFila);
    camposFormulario.push(campo);
    
    renderizarCanvas();
    seleccionarCampo(campo.id);
    guardarEnHiddenField();
}

function agregarCampoEnFila(tipo, etiquetaBase, filaIndex) {
    var fila = filasFormulario[filaIndex];
    if (!fila) return;
    
    var anchoUsado = calcularAnchoUsadoEnFila(filaIndex);
    var anchoDisponible = 12 - anchoUsado;
    
    if (anchoDisponible <= 0) {
        // No hay espacio, crear nueva fila
        agregarCampoNuevaFila(tipo, etiquetaBase);
        return;
    }
    
    contadorCampos++;
    var campo = crearCampo(tipo, etiquetaBase);
    campo.ancho = Math.min(campo.ancho, anchoDisponible);
    
    fila.campos.push(campo);
    camposFormulario.push(campo);
    
    renderizarCanvas();
    seleccionarCampo(campo.id);
    guardarEnHiddenField();
}

function agregarCampoJuntoA(tipo, etiquetaBase, campoRefId, insertarAntes) {
    var ubicacion = encontrarCampo(campoRefId);
    if (!ubicacion) return;
    
    var fila = filasFormulario[ubicacion.filaIndex];
    var anchoUsado = calcularAnchoUsadoEnFila(ubicacion.filaIndex);
    var anchoDisponible = 12 - anchoUsado;
    
    contadorCampos++;
    var campo = crearCampo(tipo, etiquetaBase);
    
    if (anchoDisponible <= 0) {
        // No hay espacio, reducir anchos existentes
        campo.ancho = 3;
        ajustarAnchosEnFila(ubicacion.filaIndex, campo.ancho);
    } else {
        campo.ancho = Math.min(campo.ancho, anchoDisponible);
    }
    
    // Insertar en la posición correcta
    var posicion = insertarAntes ? ubicacion.campoIndex : ubicacion.campoIndex + 1;
    fila.campos.splice(posicion, 0, campo);
    camposFormulario.push(campo);
    
    renderizarCanvas();
    seleccionarCampo(campo.id);
    guardarEnHiddenField();
}

function moverCampoAFila(campoId, filaDestinoIndex) {
    var ubicacionOrigen = encontrarCampo(campoId);
    if (!ubicacionOrigen) return;
    
    var campo = ubicacionOrigen.campo;
    var filaOrigen = filasFormulario[ubicacionOrigen.filaIndex];
    var filaDestino = filasFormulario[filaDestinoIndex];
    
    // Verificar espacio en fila destino
    var anchoUsado = calcularAnchoUsadoEnFila(filaDestinoIndex);
    if (ubicacionOrigen.filaIndex === filaDestinoIndex) {
        anchoUsado -= campo.ancho; // No contar el campo que se mueve
    }
    
    if (anchoUsado + campo.ancho > 12) {
        campo.ancho = 12 - anchoUsado;
        if (campo.ancho <= 0) return;
    }
    
    // Remover de fila origen
    filaOrigen.campos.splice(ubicacionOrigen.campoIndex, 1);
    
    // Si la fila origen queda vacía, eliminarla
    if (filaOrigen.campos.length === 0) {
        filasFormulario.splice(ubicacionOrigen.filaIndex, 1);
        // Ajustar índice destino si es necesario
        if (filaDestinoIndex > ubicacionOrigen.filaIndex) {
            filaDestinoIndex--;
        }
        filaDestino = filasFormulario[filaDestinoIndex];
    }
    
    // Agregar a fila destino
    if (filaDestino) {
        filaDestino.campos.push(campo);
    }
    
    renderizarCanvas();
    seleccionarCampo(campoId);
    guardarEnHiddenField();
}

function moverCampoJuntoA(campoMoverId, campoRefId, insertarAntes) {
    if (campoMoverId === campoRefId) return;
    
    var ubicacionMover = encontrarCampo(campoMoverId);
    var ubicacionRef = encontrarCampo(campoRefId);
    
    if (!ubicacionMover || !ubicacionRef) return;
    
    var campo = ubicacionMover.campo;
    var filaOrigen = filasFormulario[ubicacionMover.filaIndex];
    var filaDestino = filasFormulario[ubicacionRef.filaIndex];
    
    // Calcular espacio disponible
    var anchoUsado = calcularAnchoUsadoEnFila(ubicacionRef.filaIndex);
    if (ubicacionMover.filaIndex === ubicacionRef.filaIndex) {
        anchoUsado -= campo.ancho;
    }
    
    if (anchoUsado + campo.ancho > 12) {
        campo.ancho = Math.max(1, 12 - anchoUsado);
    }
    
    // Remover de posición original
    filaOrigen.campos.splice(ubicacionMover.campoIndex, 1);
    
    // Si la fila origen queda vacía y es diferente a la destino
    var filaOrigenEliminada = false;
    if (filaOrigen.campos.length === 0 && ubicacionMover.filaIndex !== ubicacionRef.filaIndex) {
        filasFormulario.splice(ubicacionMover.filaIndex, 1);
        filaOrigenEliminada = true;
        
        // Ajustar índice de referencia si es necesario
        if (ubicacionRef.filaIndex > ubicacionMover.filaIndex) {
            ubicacionRef.filaIndex--;
            filaDestino = filasFormulario[ubicacionRef.filaIndex];
            // Recalcular índice del campo de referencia
            ubicacionRef.campoIndex = filaDestino.campos.findIndex(function(c) { return c.id === campoRefId; });
        }
    }
    
    // Insertar en nueva posición
    var posicion = insertarAntes ? ubicacionRef.campoIndex : ubicacionRef.campoIndex + 1;
    
    // Ajustar posición si movimos dentro de la misma fila
    if (!filaOrigenEliminada && ubicacionMover.filaIndex === ubicacionRef.filaIndex && ubicacionMover.campoIndex < ubicacionRef.campoIndex) {
        posicion--;
    }
    
    filaDestino.campos.splice(Math.max(0, posicion), 0, campo);
    
    renderizarCanvas();
    seleccionarCampo(campoMoverId);
    guardarEnHiddenField();
}

function moverCampoANuevaFila(campoId) {
    var ubicacion = encontrarCampo(campoId);
    if (!ubicacion) return;
    
    var campo = ubicacion.campo;
    var filaOrigen = filasFormulario[ubicacion.filaIndex];
    
    // Remover de fila origen
    filaOrigen.campos.splice(ubicacion.campoIndex, 1);
    
    // Si la fila origen queda vacía, eliminarla
    if (filaOrigen.campos.length === 0) {
        filasFormulario.splice(ubicacion.filaIndex, 1);
    }
    
    // Crear nueva fila
    contadorFilas++;
    var nuevaFila = {
        id: 'fila_' + contadorFilas,
        campos: [campo]
    };
    
    campo.ancho = 12; // Restaurar ancho completo en nueva fila
    filasFormulario.push(nuevaFila);
    
    renderizarCanvas();
    seleccionarCampo(campoId);
    guardarEnHiddenField();
}

// Tipos que soportan propiedad de altura
var tiposConAltura = ['textarea', 'foto', 'firma'];

function crearCampo(tipo, etiquetaBase) {
    var campo = {
        id: 'campo_' + contadorCampos,
        campoId: 0,
        etiqueta: etiquetaBase + ' ' + contadorCampos,
        nombre: tipo + '_' + contadorCampos,
        tipo: tipo,
        seccion: 'General',
        requerido: false,
        ancho: 12,
        placeholder: '',
        orden: camposFormulario.length + 1
    };
    
    // Agregar altura por defecto para tipos que lo soportan
    if (tiposConAltura.includes(tipo)) {
        campo.altura = 150; // Altura por defecto en px
    }
    
    return campo;
}

function encontrarCampo(campoId) {
    for (var i = 0; i < filasFormulario.length; i++) {
        var fila = filasFormulario[i];
        for (var j = 0; j < fila.campos.length; j++) {
            if (fila.campos[j].id === campoId) {
                return {
                    filaIndex: i,
                    campoIndex: j,
                    campo: fila.campos[j],
                    fila: fila
                };
            }
        }
    }
    return null;
}

function calcularAnchoUsadoEnFila(filaIndex) {
    var fila = filasFormulario[filaIndex];
    if (!fila) return 0;
    
    return fila.campos.reduce(function(total, campo) {
        return total + (campo.ancho || 12);
    }, 0);
}

function ajustarAnchosEnFila(filaIndex, anchoNuevoCampo) {
    var fila = filasFormulario[filaIndex];
    if (!fila || fila.campos.length === 0) return;
    
    var totalActual = calcularAnchoUsadoEnFila(filaIndex);
    var totalDeseado = 12 - anchoNuevoCampo;
    var factor = totalDeseado / totalActual;
    
    fila.campos.forEach(function(campo) {
        campo.ancho = Math.max(1, Math.floor(campo.ancho * factor));
    });
}

// ========================================
// RENDERIZADO
// ========================================
function renderizarCanvas() {
    var canvas = document.getElementById('designerCanvas');
    if (!canvas) return;
    
    // Limpiar canvas
    canvas.innerHTML = '';
    
    if (filasFormulario.length === 0) {
        // Mostrar placeholder
        canvas.innerHTML = 
            '<div class="designer-placeholder" id="designerPlaceholder">' +
                '<i class="fas fa-hand-point-left"></i>' +
                '<p>Arrastra controles desde el Toolbox</p>' +
            '</div>';
        return;
    }
    
    // Renderizar cada fila
    filasFormulario.forEach(function(fila, filaIndex) {
        var rowHtml = '<div class="designer-row" id="' + fila.id + '" ' +
            'ondragover="onRowDragOver(event, ' + filaIndex + ')" ' +
            'ondragleave="onRowDragLeave(event)" ' +
            'ondrop="onRowDrop(event, ' + filaIndex + ')">';
        
        fila.campos.forEach(function(campo) {
            rowHtml += renderizarCampoHTML(campo);
        });
        
        rowHtml += '</div>';
        canvas.insertAdjacentHTML('beforeend', rowHtml);
    });
    
    // Zona para agregar nueva fila
    canvas.insertAdjacentHTML('beforeend', 
        '<div class="new-row-drop-zone" ' +
            'ondragover="onNewRowDragOver(event)" ' +
            'ondragleave="onNewRowDragLeave(event)" ' +
            'ondrop="onNewRowDrop(event)">' +
            '<i class="fas fa-plus"></i> Soltar aquí para nueva fila' +
        '</div>'
    );
    
    // Re-seleccionar campo si estaba seleccionado
    if (campoSeleccionado) {
        var element = document.getElementById(campoSeleccionado.id);
        if (element) {
            element.classList.add('selected');
        }
    }
}

function renderizarCampoHTML(campo) {
    var icono = iconosPorTipo[campo.tipo] || 'fa-question';
    var nombreTipo = nombresTipo[campo.tipo] || campo.tipo;
    
    return '<div class="designer-field" id="' + campo.id + '" data-ancho="' + campo.ancho + '" ' +
        'draggable="true" ' +
        'onclick="seleccionarCampo(\'' + campo.id + '\')" ' +
        'ondragstart="onFieldDragStart(event, \'' + campo.id + '\')" ' +
        'ondragend="onFieldDragEnd(event)" ' +
        'ondragover="onFieldDragOver(event, \'' + campo.id + '\')" ' +
        'ondragleave="onFieldDragLeave(event)" ' +
        'ondrop="onFieldDrop(event, \'' + campo.id + '\')">' +
        '<div class="field-drag-handle"></div>' +
        '<div class="field-icon"><i class="fas ' + icono + '"></i></div>' +
        '<div class="field-info">' +
            '<div class="field-label">' + escapeHtml(campo.etiqueta) + 
                (campo.requerido ? '<span class="field-required">*</span>' : '') +
            '</div>' +
            '<div class="field-meta">' + nombreTipo + ' | Ancho: ' + campo.ancho + '/12' + 
                (campo.altura ? ' | Altura: ' + campo.altura + 'px' : '') + 
                ' | ' + campo.seccion + '</div>' +
        '</div>' +
        '<button type="button" class="field-delete" onclick="event.stopPropagation(); eliminarCampo(\'' + campo.id + '\')" title="Eliminar">' +
            '<i class="fas fa-times"></i>' +
        '</button>' +
    '</div>';
}

function limpiarIndicadoresDrop() {
    document.querySelectorAll('.drag-over-row, .drag-over-zone, .drop-left, .drop-right').forEach(function(el) {
        el.classList.remove('drag-over-row', 'drag-over-zone', 'drop-left', 'drop-right');
    });
}

// ========================================
// SELECCIÓN Y PROPIEDADES
// ========================================
function seleccionarCampo(campoId) {
    // Quitar selección anterior
    document.querySelectorAll('.designer-field.selected').forEach(function(el) {
        el.classList.remove('selected');
    });
    
    // Seleccionar nuevo
    var element = document.getElementById(campoId);
    if (element) element.classList.add('selected');
    
    // Buscar campo en estructura
    var ubicacion = encontrarCampo(campoId);
    campoSeleccionado = ubicacion ? ubicacion.campo : null;
    
    // Mostrar propiedades
    if (campoSeleccionado) {
        mostrarPropiedades(campoSeleccionado);
    }
}

function mostrarPropiedades(campo) {
    document.querySelector('.property-placeholder').style.display = 'none';
    document.getElementById('propertyForm').style.display = 'block';
    
    var esBoton = tiposBotones.includes(campo.tipo);
    
    document.getElementById('propEtiqueta').value = campo.etiqueta || '';
    document.getElementById('propNombre').value = campo.nombre || '';
    document.getElementById('propTipo').value = campo.tipo || 'texto';
    document.getElementById('propSeccion').value = campo.seccion || 'General';
    document.getElementById('propAncho').value = campo.ancho || 12;
    document.getElementById('propPlaceholder').value = campo.placeholder || '';
    document.getElementById('propRequerido').checked = campo.requerido || false;
    
    // Actualizar display del slider de ancho
    var anchoValue = document.getElementById('propAnchoValue');
    if (anchoValue) {
        anchoValue.textContent = (campo.ancho || 12) + '/12';
    }
    
    // Mostrar/ocultar propiedades según el tipo de campo
    var propSeccionRow = document.getElementById('propSeccion').parentElement;
    var propPlaceholderRow = document.getElementById('propPlaceholder').parentElement;
    var propRequeridoRow = document.getElementById('propRequerido').parentElement.parentElement;
    
    // Ocultar propiedades que no aplican a botones
    if (propSeccionRow) propSeccionRow.style.display = esBoton ? 'none' : 'block';
    if (propPlaceholderRow) propPlaceholderRow.style.display = esBoton ? 'none' : 'block';
    if (propRequeridoRow) propRequeridoRow.style.display = esBoton ? 'none' : 'block';
    
    // Mostrar/ocultar propiedad de altura según el tipo
    var alturaRow = document.getElementById('propAlturaRow');
    var alturaInput = document.getElementById('propAltura');
    var alturaValue = document.getElementById('propAlturaValue');
    
    if (alturaRow && alturaInput) {
        if (tiposConAltura.includes(campo.tipo)) {
            alturaRow.style.display = 'block';
            var altura = campo.altura || 150;
            alturaInput.value = altura;
            if (alturaValue) {
                alturaValue.textContent = altura + 'px';
            }
        } else {
            alturaRow.style.display = 'none';
        }
    }
}

function ocultarPropiedades() {
    document.querySelector('.property-placeholder').style.display = 'flex';
    document.getElementById('propertyForm').style.display = 'none';
}

function actualizarPropiedad(propiedad, valor) {
    if (!campoSeleccionado) return;
    
    campoSeleccionado[propiedad] = valor;
    
    // Si cambió el ancho, verificar que no exceda el espacio disponible
    if (propiedad === 'ancho') {
        var ubicacion = encontrarCampo(campoSeleccionado.id);
        if (ubicacion) {
            var anchoOtros = calcularAnchoUsadoEnFila(ubicacion.filaIndex) - campoSeleccionado.ancho + valor;
            if (anchoOtros > 12) {
                // Ajustar para que quepa
                campoSeleccionado.ancho = 12 - (anchoOtros - valor);
                document.getElementById('propAncho').value = campoSeleccionado.ancho;
            }
        }
        
        // Actualizar display
        var anchoValue = document.getElementById('propAnchoValue');
        if (anchoValue) {
            anchoValue.textContent = campoSeleccionado.ancho + '/12';
        }
    }
    
    // Si cambió el tipo, actualizar visibilidad de propiedad altura
    if (propiedad === 'tipo') {
        var alturaRow = document.getElementById('propAlturaRow');
        var alturaInput = document.getElementById('propAltura');
        var alturaValue = document.getElementById('propAlturaValue');
        
        if (alturaRow) {
            if (tiposConAltura.includes(valor)) {
                alturaRow.style.display = 'block';
                // Asignar altura por defecto si no tiene
                if (!campoSeleccionado.altura) {
                    campoSeleccionado.altura = 150;
                }
                if (alturaInput) alturaInput.value = campoSeleccionado.altura;
                if (alturaValue) alturaValue.textContent = campoSeleccionado.altura + 'px';
            } else {
                alturaRow.style.display = 'none';
                // Remover propiedad altura si el tipo no la soporta
                delete campoSeleccionado.altura;
            }
        }
    }
    
    // Actualizar display de altura
    if (propiedad === 'altura') {
        var alturaValueEl = document.getElementById('propAlturaValue');
        if (alturaValueEl) {
            alturaValueEl.textContent = valor + 'px';
        }
    }
    
    renderizarCanvas();
    guardarEnHiddenField();
}

// ========================================
// ACCIONES DE CAMPOS
// ========================================
function eliminarCampo(campoId) {
    if (!confirm('¿Eliminar este campo?')) return;
    
    var ubicacion = encontrarCampo(campoId);
    if (!ubicacion) return;
    
    // Remover de la fila
    ubicacion.fila.campos.splice(ubicacion.campoIndex, 1);
    
    // Si la fila queda vacía, eliminarla
    if (ubicacion.fila.campos.length === 0) {
        filasFormulario.splice(ubicacion.filaIndex, 1);
    }
    
    // Remover del array plano
    camposFormulario = camposFormulario.filter(function(c) { return c.id !== campoId; });
    
    if (campoSeleccionado && campoSeleccionado.id === campoId) {
        campoSeleccionado = null;
        ocultarPropiedades();
    }
    
    renderizarCanvas();
    reordenarCampos();
    guardarEnHiddenField();
}

function eliminarCampoSeleccionado() {
    if (!campoSeleccionado) {
        alert('Seleccione un campo para eliminar');
        return;
    }
    eliminarCampo(campoSeleccionado.id);
}

function moverCampoArriba() {
    if (!campoSeleccionado) return;
    
    var ubicacion = encontrarCampo(campoSeleccionado.id);
    if (!ubicacion) return;
    
    if (ubicacion.campoIndex > 0) {
        // Mover dentro de la misma fila (hacia la izquierda)
        var fila = ubicacion.fila;
        var temp = fila.campos[ubicacion.campoIndex];
        fila.campos[ubicacion.campoIndex] = fila.campos[ubicacion.campoIndex - 1];
        fila.campos[ubicacion.campoIndex - 1] = temp;
    } else if (ubicacion.filaIndex > 0) {
        // Mover a la fila anterior
        moverCampoAFila(campoSeleccionado.id, ubicacion.filaIndex - 1);
        return;
    }
    
    renderizarCanvas();
    reordenarCampos();
    guardarEnHiddenField();
}

function moverCampoAbajo() {
    if (!campoSeleccionado) return;
    
    var ubicacion = encontrarCampo(campoSeleccionado.id);
    if (!ubicacion) return;
    
    if (ubicacion.campoIndex < ubicacion.fila.campos.length - 1) {
        // Mover dentro de la misma fila (hacia la derecha)
        var fila = ubicacion.fila;
        var temp = fila.campos[ubicacion.campoIndex];
        fila.campos[ubicacion.campoIndex] = fila.campos[ubicacion.campoIndex + 1];
        fila.campos[ubicacion.campoIndex + 1] = temp;
    } else if (ubicacion.filaIndex < filasFormulario.length - 1) {
        // Mover a la fila siguiente
        moverCampoAFila(campoSeleccionado.id, ubicacion.filaIndex + 1);
        return;
    }
    
    renderizarCanvas();
    reordenarCampos();
    guardarEnHiddenField();
}

function reordenarCampos() {
    var orden = 1;
    camposFormulario = [];
    
    filasFormulario.forEach(function(fila) {
        fila.campos.forEach(function(campo) {
            campo.orden = orden++;
            camposFormulario.push(campo);
        });
    });
}

// ========================================
// PERSISTENCIA
// ========================================
function guardarEnHiddenField() {
    var hf = document.getElementById(hfCamposJSONClientID);
    if (hf) {
        // Guardar estructura de filas para preservar layout
        var dataToSave = {
            filas: filasFormulario,
            campos: camposFormulario
        };
        hf.value = JSON.stringify(dataToSave);
    }
}

function cargarCamposDesdeServidor(jsonString) {
    try {
        var data = JSON.parse(jsonString);
        
        if (data && data.filas) {
            // Nueva estructura con filas
            filasFormulario = data.filas || [];
            camposFormulario = data.campos || [];
        } else if (Array.isArray(data)) {
            // Estructura antigua (solo array de campos) - migrar a filas
            camposFormulario = data;
            filasFormulario = [];
            
            // Crear una fila por cada campo (comportamiento anterior)
            camposFormulario.forEach(function(campo, index) {
                contadorFilas++;
                filasFormulario.push({
                    id: 'fila_' + contadorFilas,
                    campos: [campo]
                });
            });
        } else {
            filasFormulario = [];
            camposFormulario = [];
        }
        
        contadorCampos = camposFormulario.length;
        contadorFilas = filasFormulario.length;
        
        renderizarCanvas();
        
    } catch (ex) {
        console.error('Error al cargar campos:', ex);
        filasFormulario = [];
        camposFormulario = [];
        renderizarCanvas();
    }
}

// ========================================
// UTILIDADES
// ========================================
function escapeHtml(text) {
    var div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

/**
 * Abre la vista previa del formulario en una nueva ventana
 * Usa un formulario oculto para enviar datos por POST y evitar límites de URL
 */
function abrirVistaPrevia() {
    try {
        // Obtener datos del formulario
        var nombre = '';
        if (typeof txtNombre !== 'undefined' && txtNombre.GetText) {
            nombre = txtNombre.GetText();
        } else {
            var txtNombreEl = document.querySelector('[id$="txtNombre_I"]');
            if (txtNombreEl) nombre = txtNombreEl.value;
        }
        
        // Obtener plataformas seleccionadas
        var plataformas = [];
        if (typeof chkPlataformas !== 'undefined' && chkPlataformas.GetSelectedValues) {
            plataformas = chkPlataformas.GetSelectedValues();
        } else {
            document.querySelectorAll('[id*="chkPlataformas"] input[type="checkbox"]:checked').forEach(function(chk) {
                var label = chk.parentElement.textContent.trim().toLowerCase();
                if (label.includes('web')) plataformas.push('web');
                if (label.includes('móvil') || label.includes('movil')) plataformas.push('movil');
            });
        }
        
        // Preparar datos de campos
        var dataToSave = {
            filas: filasFormulario,
            campos: camposFormulario
        };
        var camposJSON = JSON.stringify(dataToSave);
        
        // Verificar si el formulario ya está guardado
        var formularioId = document.getElementById(hfFormularioIdClientID);
        var id = formularioId ? formularioId.value : '0';
        
        if (id && id !== '0') {
            // Formulario guardado - usar ID directamente
            window.open('FormularioVistaPrevia.aspx?id=' + id, '_blank', 'width=1200,height=800,scrollbars=yes,resizable=yes');
        } else {
            // Formulario no guardado - usar POST mediante formulario oculto
            var form = document.getElementById('formVistaPrevia');
            if (!form) {
                form = document.createElement('form');
                form.id = 'formVistaPrevia';
                form.method = 'POST';
                form.target = '_blank';
                form.style.display = 'none';
                
                var inputCampos = document.createElement('input');
                inputCampos.type = 'hidden';
                inputCampos.name = 'camposJSON';
                form.appendChild(inputCampos);
                
                var inputNombre = document.createElement('input');
                inputNombre.type = 'hidden';
                inputNombre.name = 'nombreFormulario';
                form.appendChild(inputNombre);
                
                var inputPlataformas = document.createElement('input');
                inputPlataformas.type = 'hidden';
                inputPlataformas.name = 'plataformas';
                form.appendChild(inputPlataformas);
                
                document.body.appendChild(form);
            }
            
            form.action = 'FormularioVistaPrevia.aspx';
            form.querySelector('[name="camposJSON"]').value = camposJSON;
            form.querySelector('[name="nombreFormulario"]').value = nombre || 'Vista Previa';
            form.querySelector('[name="plataformas"]').value = plataformas.join(',') || 'web,movil';
            
            form.submit();
        }
        
    } catch (ex) {
        console.error('Error al abrir vista previa:', ex);
        alert('Error al abrir la vista previa: ' + ex.message);
    }
}

// Exponer función globalmente
window.abrirVistaPrevia = abrirVistaPrevia;
