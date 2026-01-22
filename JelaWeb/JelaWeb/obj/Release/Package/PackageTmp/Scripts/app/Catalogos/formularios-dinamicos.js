/**
 * Formularios Dinámicos - JavaScript Module
 * Diseñador estilo Visual Studio con Drag & Drop
 */

// ========================================
// VARIABLES GLOBALES
// ========================================
var camposFormulario = [];
var campoSeleccionado = null;
var contadorCampos = 0;

// IDs de hidden fields (se establecen desde el servidor)
var hfFormularioIdClientID = '';
var hfCampoIdClientID = '';
var hfCamposJSONClientID = '';

// Iconos por tipo de campo
var iconosPorTipo = {
    'texto': 'fa-font',
    'numero': 'fa-hashtag',
    'decimal': 'fa-percentage',
    'fecha': 'fa-calendar',
    'fecha_hora': 'fa-calendar-alt',
    'hora': 'fa-clock',
    'dropdown': 'fa-caret-square-down',
    'radio': 'fa-dot-circle',
    'checkbox': 'fa-check-square',
    'textarea': 'fa-align-left',
    'foto': 'fa-camera',
    'archivo': 'fa-file-upload',
    'firma': 'fa-signature'
};

var nombresTipo = {
    'texto': 'Texto',
    'numero': 'Número',
    'decimal': 'Decimal',
    'fecha': 'Fecha',
    'fecha_hora': 'Fecha/Hora',
    'hora': 'Hora',
    'dropdown': 'Lista',
    'radio': 'Opciones',
    'checkbox': 'Casilla',
    'textarea': 'Área Texto',
    'foto': 'Foto',
    'archivo': 'Archivo',
    'firma': 'Firma'
};

// ========================================
// INICIALIZACIÓN
// ========================================
document.addEventListener('DOMContentLoaded', function() {
    inicializarDragDrop();
});

function inicializarDragDrop() {
    // Configurar items del toolbox como draggables
    var toolboxItems = document.querySelectorAll('.toolbox-item');
    toolboxItems.forEach(function(item) {
        item.addEventListener('dragstart', onDragStart);
        item.addEventListener('dragend', onDragEnd);
    });

    // Configurar canvas como drop zone
    var canvas = document.getElementById('designerCanvas');
    if (canvas) {
        canvas.addEventListener('dragover', onDragOver);
        canvas.addEventListener('dragleave', onDragLeave);
        canvas.addEventListener('drop', onDrop);
    }
}

// ========================================
// DRAG & DROP HANDLERS
// ========================================
function onDragStart(e) {
    e.target.classList.add('dragging');
    e.dataTransfer.setData('text/plain', JSON.stringify({
        tipo: e.target.dataset.tipo,
        etiqueta: e.target.dataset.etiqueta
    }));
    e.dataTransfer.effectAllowed = 'copy';
}

function onDragEnd(e) {
    e.target.classList.remove('dragging');
}

function onDragOver(e) {
    e.preventDefault();
    e.dataTransfer.dropEffect = 'copy';
    e.currentTarget.classList.add('drag-over');
}

function onDragLeave(e) {
    e.currentTarget.classList.remove('drag-over');
}

function onDrop(e) {
    e.preventDefault();
    e.currentTarget.classList.remove('drag-over');
    
    try {
        var data = JSON.parse(e.dataTransfer.getData('text/plain'));
        agregarCampoAlCanvas(data.tipo, data.etiqueta);
    } catch (ex) {
        console.error('Error al procesar drop:', ex);
    }
}

// ========================================
// GESTIÓN DE CAMPOS EN EL CANVAS
// ========================================
function agregarCampoAlCanvas(tipo, etiquetaBase) {
    contadorCampos++;
    
    var campo = {
        id: 'campo_' + contadorCampos,
        campoId: 0, // 0 = nuevo campo
        etiqueta: etiquetaBase + ' ' + contadorCampos,
        nombre: tipo + '_' + contadorCampos,
        tipo: tipo,
        seccion: 'General',
        requerido: false,
        ancho: 12,
        placeholder: '',
        orden: camposFormulario.length + 1
    };
    
    camposFormulario.push(campo);
    renderizarCampoEnCanvas(campo);
    seleccionarCampo(campo.id);
    ocultarPlaceholder();
    actualizarHiddenField();
}

function renderizarCampoEnCanvas(campo) {
    var canvas = document.getElementById('designerCanvas');
    var icono = iconosPorTipo[campo.tipo] || 'fa-question';
    var nombreTipo = nombresTipo[campo.tipo] || campo.tipo;
    
    var fieldHtml = 
        '<div class="designer-field" id="' + campo.id + '" onclick="seleccionarCampo(\'' + campo.id + '\')">' +
            '<div class="field-icon"><i class="fas ' + icono + '"></i></div>' +
            '<div class="field-info">' +
                '<div class="field-label">' + campo.etiqueta + 
                    (campo.requerido ? '<span class="field-required">*</span>' : '') +
                '</div>' +
                '<div class="field-type">' + nombreTipo + ' | Ancho: ' + campo.ancho + '/12</div>' +
            '</div>' +
            '<div class="field-actions">' +
                '<button class="field-delete" onclick="event.stopPropagation(); eliminarCampoPorId(\'' + campo.id + '\')" title="Eliminar">' +
                    '<i class="fas fa-times"></i>' +
                '</button>' +
            '</div>' +
        '</div>';
    
    canvas.insertAdjacentHTML('beforeend', fieldHtml);
    
    // Animación de entrada
    var element = document.getElementById(campo.id);
    if (element) {
        element.classList.add('just-dropped');
        setTimeout(function() {
            element.classList.remove('just-dropped');
        }, 500);
    }
}

function actualizarCampoEnCanvas(campo) {
    var element = document.getElementById(campo.id);
    if (!element) return;
    
    var icono = iconosPorTipo[campo.tipo] || 'fa-question';
    var nombreTipo = nombresTipo[campo.tipo] || campo.tipo;
    
    element.querySelector('.field-icon i').className = 'fas ' + icono;
    element.querySelector('.field-label').innerHTML = campo.etiqueta + 
        (campo.requerido ? '<span class="field-required">*</span>' : '');
    element.querySelector('.field-type').textContent = nombreTipo + ' | Ancho: ' + campo.ancho + '/12';
}

function seleccionarCampo(campoId) {
    // Quitar selección anterior
    document.querySelectorAll('.designer-field.selected').forEach(function(el) {
        el.classList.remove('selected');
    });
    
    // Seleccionar nuevo
    var element = document.getElementById(campoId);
    if (element) {
        element.classList.add('selected');
    }
    
    // Encontrar campo en el array
    campoSeleccionado = camposFormulario.find(function(c) { return c.id === campoId; });
    
    // Cargar propiedades en el VerticalGrid
    if (campoSeleccionado && typeof vgridPropiedades !== 'undefined') {
        cargarPropiedadesEnGrid(campoSeleccionado);
    }
}

function cargarPropiedadesEnGrid(campo) {
    // Establecer valores en el VerticalGrid
    try {
        vgridPropiedades.SetRowValue('rowEtiqueta', campo.etiqueta);
        vgridPropiedades.SetRowValue('rowNombre', campo.nombre);
        vgridPropiedades.SetRowValue('rowTipo', campo.tipo);
        vgridPropiedades.SetRowValue('rowSeccion', campo.seccion);
        vgridPropiedades.SetRowValue('rowRequerido', campo.requerido);
        vgridPropiedades.SetRowValue('rowAncho', campo.ancho);
        vgridPropiedades.SetRowValue('rowPlaceholder', campo.placeholder);
        vgridPropiedades.SetRowValue('rowOrden', campo.orden);
    } catch (ex) {
        console.log('Error al cargar propiedades:', ex);
    }
}

function eliminarCampoPorId(campoId) {
    if (!confirm('¿Eliminar este campo?')) return;
    
    // Remover del array
    camposFormulario = camposFormulario.filter(function(c) { return c.id !== campoId; });
    
    // Remover del DOM
    var element = document.getElementById(campoId);
    if (element) {
        element.remove();
    }
    
    // Limpiar selección si era el seleccionado
    if (campoSeleccionado && campoSeleccionado.id === campoId) {
        campoSeleccionado = null;
        limpiarPropiedades();
    }
    
    // Mostrar placeholder si no hay campos
    if (camposFormulario.length === 0) {
        mostrarPlaceholder();
    }
    
    // Reordenar
    reordenarCampos();
    actualizarHiddenField();
}

function eliminarCampoSeleccionadoDesigner() {
    if (!campoSeleccionado) {
        alert('Seleccione un campo para eliminar');
        return;
    }
    eliminarCampoPorId(campoSeleccionado.id);
}

function moverCampoArriba() {
    if (!campoSeleccionado) return;
    
    var index = camposFormulario.findIndex(function(c) { return c.id === campoSeleccionado.id; });
    if (index > 0) {
        // Intercambiar en array
        var temp = camposFormulario[index];
        camposFormulario[index] = camposFormulario[index - 1];
        camposFormulario[index - 1] = temp;
        
        // Mover en DOM
        var element = document.getElementById(campoSeleccionado.id);
        var prevElement = element.previousElementSibling;
        if (prevElement && prevElement.classList.contains('designer-field')) {
            element.parentNode.insertBefore(element, prevElement);
        }
        
        reordenarCampos();
        actualizarHiddenField();
    }
}

function moverCampoAbajo() {
    if (!campoSeleccionado) return;
    
    var index = camposFormulario.findIndex(function(c) { return c.id === campoSeleccionado.id; });
    if (index < camposFormulario.length - 1) {
        // Intercambiar en array
        var temp = camposFormulario[index];
        camposFormulario[index] = camposFormulario[index + 1];
        camposFormulario[index + 1] = temp;
        
        // Mover en DOM
        var element = document.getElementById(campoSeleccionado.id);
        var nextElement = element.nextElementSibling;
        if (nextElement && nextElement.classList.contains('designer-field')) {
            nextElement.parentNode.insertBefore(nextElement, element);
        }
        
        reordenarCampos();
        actualizarHiddenField();
    }
}

function reordenarCampos() {
    camposFormulario.forEach(function(campo, index) {
        campo.orden = index + 1;
    });
}

// ========================================
// PROPIEDADES - EVENTOS
// ========================================
function onPropiedadChanged(s, e) {
    if (!campoSeleccionado) return;
    
    // Obtener valores del grid
    try {
        campoSeleccionado.etiqueta = vgridPropiedades.GetRowValue('rowEtiqueta') || '';
        campoSeleccionado.nombre = vgridPropiedades.GetRowValue('rowNombre') || '';
        campoSeleccionado.tipo = vgridPropiedades.GetRowValue('rowTipo') || 'texto';
        campoSeleccionado.seccion = vgridPropiedades.GetRowValue('rowSeccion') || 'General';
        campoSeleccionado.requerido = vgridPropiedades.GetRowValue('rowRequerido') || false;
        campoSeleccionado.ancho = vgridPropiedades.GetRowValue('rowAncho') || 12;
        campoSeleccionado.placeholder = vgridPropiedades.GetRowValue('rowPlaceholder') || '';
        campoSeleccionado.orden = vgridPropiedades.GetRowValue('rowOrden') || 1;
        
        // Actualizar visualización
        actualizarCampoEnCanvas(campoSeleccionado);
        actualizarHiddenField();
    } catch (ex) {
        console.log('Error al actualizar propiedad:', ex);
    }
}

function limpiarPropiedades() {
    try {
        if (typeof vgridPropiedades !== 'undefined') {
            vgridPropiedades.SetRowValue('rowEtiqueta', '');
            vgridPropiedades.SetRowValue('rowNombre', '');
            vgridPropiedades.SetRowValue('rowTipo', 'texto');
            vgridPropiedades.SetRowValue('rowSeccion', 'General');
            vgridPropiedades.SetRowValue('rowRequerido', false);
            vgridPropiedades.SetRowValue('rowAncho', 12);
            vgridPropiedades.SetRowValue('rowPlaceholder', '');
            vgridPropiedades.SetRowValue('rowOrden', 1);
        }
    } catch (ex) { }
}

// ========================================
// UTILIDADES
// ========================================
function ocultarPlaceholder() {
    var placeholder = document.querySelector('.designer-placeholder');
    if (placeholder) {
        placeholder.style.display = 'none';
    }
}

function mostrarPlaceholder() {
    var placeholder = document.querySelector('.designer-placeholder');
    if (placeholder) {
        placeholder.style.display = 'flex';
    }
}

function actualizarHiddenField() {
    var hf = document.getElementById(hfCamposJSONClientID);
    if (hf) {
        hf.value = JSON.stringify(camposFormulario);
    }
}

function cargarCamposDesdeJSON(jsonString) {
    try {
        camposFormulario = JSON.parse(jsonString) || [];
        var canvas = document.getElementById('designerCanvas');
        
        // Limpiar canvas (excepto placeholder)
        canvas.querySelectorAll('.designer-field').forEach(function(el) {
            el.remove();
        });
        
        // Renderizar campos
        if (camposFormulario.length > 0) {
            ocultarPlaceholder();
            camposFormulario.forEach(function(campo) {
                renderizarCampoEnCanvas(campo);
            });
            contadorCampos = camposFormulario.length;
        } else {
            mostrarPlaceholder();
        }
    } catch (ex) {
        console.error('Error al cargar campos:', ex);
    }
}

// ========================================
// HANDLERS DEL TOOLBAR PRINCIPAL
// ========================================
function onToolbarFormulariosClick(s, e) {
    var itemName = e.item.name;
    
    switch (itemName) {
        case 'btnNuevoFormulario':
            mostrarNuevoFormulario();
            break;
        case 'btnCrearDesdePDF':
            mostrarCrearDesdePDF();
            break;
        case 'btnEditarFormulario':
            editarFormularioSeleccionado();
            break;
        case 'btnVistaPrevia':
            vistaPreviaFormulario();
            break;
        case 'btnEliminarFormulario':
            eliminarFormularioSeleccionado();
            break;
    }
}

// Handler para el toolbar del grid de campos (legacy - ya no se usa)
function onToolbarCamposClick(s, e) {
    // Ya no se usa - el diseñador visual reemplaza el grid
}

function OnGridRowClick(s, e) {
    // Seleccionar fila al hacer click
}

function OnGridRowDblClick(s, e) {
    editarFormularioSeleccionado();
}

// ========================================
// FUNCIONES DE FORMULARIO
// ========================================
function mostrarNuevoFormulario() {
    limpiarFormulario();
    popupFormulario.SetHeaderText('Nuevo Formulario');
    popupFormulario.Show();
    
    // Reinicializar drag & drop después de mostrar popup
    setTimeout(inicializarDragDrop, 100);
}

function mostrarCrearDesdePDF() {
    if (typeof txtNombrePDF !== 'undefined') {
        txtNombrePDF.SetText('');
    }
    var divCampos = document.getElementById('divCamposExtraidos');
    if (divCampos) {
        divCampos.style.display = 'none';
    }
    popupCrearPDF.Show();
}

function editarFormularioSeleccionado() {
    var keys = gridFormularios.GetSelectedKeysOnPage();
    if (keys.length === 0) {
        alert('Seleccione un formulario para editar');
        return;
    }
    
    var formularioId = keys[0];
    cargarFormulario(formularioId);
}

function vistaPreviaFormulario() {
    var keys = gridFormularios.GetSelectedKeysOnPage();
    if (keys.length === 0) {
        alert('Seleccione un formulario para ver la vista previa');
        return;
    }
    
    var formularioId = keys[0];
    alert('Vista previa del formulario ID: ' + formularioId);
}

function eliminarFormularioSeleccionado() {
    var keys = gridFormularios.GetSelectedKeysOnPage();
    if (keys.length === 0) {
        alert('Seleccione un formulario para eliminar');
        return;
    }
    
    if (confirm('¿Está seguro de eliminar este formulario?')) {
        var formularioId = keys[0];
        gridFormularios.PerformCallback('delete|' + formularioId);
    }
}

function limpiarFormulario() {
    // Limpiar campos del formulario
    if (typeof txtNombreFormulario !== 'undefined') txtNombreFormulario.SetText('');
    if (typeof txtDescripcion !== 'undefined') txtDescripcion.SetText('');
    if (typeof cmbEstado !== 'undefined') cmbEstado.SetValue('borrador');
    if (typeof chkRequiereFirma !== 'undefined') chkRequiereFirma.SetChecked(false);
    if (typeof chkRequiereFoto !== 'undefined') chkRequiereFoto.SetChecked(false);
    
    // Limpiar diseñador
    camposFormulario = [];
    campoSeleccionado = null;
    contadorCampos = 0;
    
    var canvas = document.getElementById('designerCanvas');
    if (canvas) {
        canvas.querySelectorAll('.designer-field').forEach(function(el) {
            el.remove();
        });
        mostrarPlaceholder();
    }
    
    limpiarPropiedades();
    actualizarHiddenField();
}

function cargarFormulario(formularioId) {
    gridFormularios.PerformCallback('load|' + formularioId);
}

function onPDFUploaded(e) {
    var callbackData = e.callbackData;
    if (callbackData) {
        var parts = callbackData.split('|');
        if (parts[0] === 'success') {
            var divCampos = document.getElementById('divCamposExtraidos');
            if (divCampos) {
                divCampos.style.display = 'block';
            }
            if (typeof gridCamposExtraidos !== 'undefined') {
                gridCamposExtraidos.Refresh();
            }
            alert('Se detectaron ' + parts[1] + ' campos en el PDF');
        } else {
            alert('Error al procesar PDF: ' + parts[1]);
        }
    }
}