// ============================================================================
// Módulo: Unidades Privativas
// Archivo: unidades.js
// Versión: 2.0 - Sin dependencia de jQuery (usando fetch API nativo)
// ============================================================================

// CRÍTICO: Definir inicializarMapa como stub FUERA de cualquier IIFE para que esté
// disponible globalmente cuando Google Maps la busque como callback.
// Google Maps busca la función en el scope global, no en window.inicializarMapa
if (typeof inicializarMapa === 'undefined') {
    inicializarMapa = function() {
        console.warn('inicializarMapa (stub) llamada antes de la inicialización completa, esperando...');
        // Reintentar después de un breve delay
        setTimeout(function() {
            if (typeof window.inicializarMapa === 'function') {
                var realFunction = window.inicializarMapa;
                // Verificar que no sea el stub (comprobando si tiene más de 3 líneas de código)
                if (realFunction.toString().length > 100) {
                    realFunction();
                }
            } else if (typeof inicializarMapa === 'function' && inicializarMapa.toString().length > 100) {
                inicializarMapa();
            }
        }, 1000);
    };
    // También en window para compatibilidad
    window.inicializarMapa = inicializarMapa;
}

// IMPORTANTE: Definir funciones de eventos ANTES de las variables para asegurar
// que estén disponibles cuando DevExpress inicialice los controles
(function() {
    'use strict';
    
    // Definir funciones stub primero para evitar errores de referencia
    if (typeof window.onGridCustomButtonClick === 'undefined') {
        window.onGridCustomButtonClick = function(s, e) {
            console.warn('onGridCustomButtonClick llamada antes de la inicialización completa');
        };
    }
    
    if (typeof window.onToolbarUnidadesClick === 'undefined') {
        window.onToolbarUnidadesClick = function(s, e) {
            console.warn('onToolbarUnidadesClick llamada antes de la inicialización completa');
        };
    }
    
    if (typeof window.onRowDblClick === 'undefined') {
        window.onRowDblClick = function(s, e) {
            console.warn('onRowDblClick llamada antes de la inicialización completa');
        };
    }
    
    if (typeof window.onUnidadFocusedRowChanged === 'undefined') {
        window.onUnidadFocusedRowChanged = function(s, e) {
            console.warn('onUnidadFocusedRowChanged llamada antes de la inicialización completa');
        };
    }
    
    if (typeof window.onEntidadChanged === 'undefined') {
        window.onEntidadChanged = function(s, e) {
            console.warn('onEntidadChanged llamada antes de la inicialización completa');
        };
    }
    
    if (typeof window.onToolbarResidentesClick === 'undefined') {
        window.onToolbarResidentesClick = function(s, e) {
            console.warn('onToolbarResidentesClick llamada antes de la inicialización completa');
        };
    }
    
    if (typeof window.onToolbarVehiculosClick === 'undefined') {
        window.onToolbarVehiculosClick = function(s, e) {
            console.warn('onToolbarVehiculosClick llamada antes de la inicialización completa');
        };
    }
    
    if (typeof window.onToolbarTagsClick === 'undefined') {
        window.onToolbarTagsClick = function(s, e) {
            console.warn('onToolbarTagsClick llamada antes de la inicialización completa');
        };
    }
    
    if (typeof window.onToolbarDocumentosClick === 'undefined') {
        window.onToolbarDocumentosClick = function(s, e) {
            console.warn('onToolbarDocumentosClick llamada antes de la inicialización completa');
        };
    }
    
    if (typeof window.onToolbarArchivosResidenteClick === 'undefined') {
        window.onToolbarArchivosResidenteClick = function(s, e) {
            console.warn('onToolbarArchivosResidenteClick llamada antes de la inicialización completa');
        };
    }
    
    if (typeof window.onToolbarArchivosVehiculoClick === 'undefined') {
        window.onToolbarArchivosVehiculoClick = function(s, e) {
            console.warn('onToolbarArchivosVehiculoClick llamada antes de la inicialización completa');
        };
    }
    
    if (typeof window.onToolbarArchivosDocumentoClick === 'undefined') {
        window.onToolbarArchivosDocumentoClick = function(s, e) {
            console.warn('onToolbarArchivosDocumentoClick llamada antes de la inicialización completa');
        };
    }
})();

var currentUnidadId = 0;
var mapaUnidades = null; // Instancia del componente <gmp-map>
var mapaInicializado = false; // Flag para evitar inicializaciones duplicadas
var marcadorUnidad = null; // Marcador <gmp-advanced-marker> en el mapa
var lastProcessedUnidadId = null; // Para evitar procesar la misma unidad múltiples veces
var editandoUnidad = false; // Flag para indicar que se está editando una unidad

// Flags para rastrear inicialización de popups
var popupsInicializados = {
    popupResidente: false,
    popupVehiculo: false,
    popupTag: false,
    popupDocumento: false
};

// Función helper para obtener popup de manera segura
function getPopup(nombrePopup) {
    // Intentar obtener desde el objeto global de DevExpress
    if (typeof ASPxClientPopupControl !== 'undefined') {
        var popup = ASPxClientPopupControl.Cast(nombrePopup);
        if (popup) return popup;
    }
    // Intentar obtener desde window
    if (window[nombrePopup]) return window[nombrePopup];
    // Intentar obtener desde el objeto global
    if (typeof eval !== 'undefined') {
        try {
            var popup = eval(nombrePopup);
            if (popup) return popup;
        } catch (e) {
            // Ignorar errores de eval
        }
    }
    return null;
}

// Nota: showToast está definida globalmente en el Master Page
// Si no está disponible, usar fallback
if (typeof showToast === 'undefined') {
    window.showToast = function(type, message) {
        if (typeof toastr !== 'undefined' && toastr && typeof toastr[type] === 'function') {
            toastr[type](message);
        } else {
            console.log('[' + type.toUpperCase() + '] ' + message);
            if (type === 'error') {
                alert('Error: ' + message);
            }
        }
    };
}

// ========================================================================
// TOOLBAR PRINCIPAL - UNIDADES
// ========================================================================
// Registrar funciones globalmente para asegurar disponibilidad
window.onToolbarUnidadesClick = function onToolbarUnidadesClick(s, e) {
    try {
        switch (e.item.name) {
            case 'btnNuevo': mostrarNuevaUnidad(); break;
            case 'btnEditar': editarUnidadSeleccionada(); break;
            case 'btnEliminar': eliminarUnidadSeleccionada(); break;
        }
    } catch (error) {
        console.error('Error en onToolbarUnidadesClick:', error);
        if (typeof showToast !== 'undefined') {
            showToast('error', 'Error al procesar la acción: ' + error.message);
        }
    }
};
var onToolbarUnidadesClick = window.onToolbarUnidadesClick;

window.onRowDblClick = function onRowDblClick(s, e) {
    try {
        console.log('onRowDblClick: visibleIndex =', e.visibleIndex);
        var id = s.GetRowKey(e.visibleIndex);
        console.log('onRowDblClick: ID obtenido =', id);
        
        if (!id || id === null || id === undefined) {
            console.warn('onRowDblClick: No se pudo obtener el ID de la fila');
            // Intentar obtener el ID del focused row como alternativa
            var focusedIdx = s.GetFocusedRowIndex();
            if (focusedIdx >= 0) {
                id = s.GetRowKey(focusedIdx);
                console.log('onRowDblClick: Intentando con focused row, ID =', id);
            }
            
            if (!id || id === null || id === undefined) {
                showToast('warning', 'No se pudo obtener el ID de la unidad');
                return;
            }
        }
        cargarUnidad(id);
    } catch (error) {
        console.error('Error en onRowDblClick:', error);
        if (typeof showToast !== 'undefined') {
            showToast('error', 'Error al cargar la unidad: ' + error.message);
        }
    }
};
var onRowDblClick = window.onRowDblClick;

// Función para manejar clicks en botones personalizados del grid
// Registrada en window para asegurar disponibilidad global
window.onGridCustomButtonClick = function onGridCustomButtonClick(s, e) {
    try {
        var unidadId = s.GetRowKey(e.visibleIndex);
        
        // Validar que tengamos un ID válido
        if (!unidadId || unidadId === null || unidadId === undefined) {
            console.warn('onGridCustomButtonClick: No se pudo obtener el ID de la unidad');
            showToast('warning', 'No se pudo obtener el ID de la unidad');
            return;
        }
        
        console.log('onGridCustomButtonClick: Botón clickeado:', e.buttonID, 'para unidad:', unidadId);
        
        // Abrir el popup de unidad en el tab correspondiente
        switch (e.buttonID) {
            case 'btnResidentes':
                abrirPopupUnidadEnTab(unidadId, 1); // Tab de Residentes
                break;
            case 'btnVehiculos':
                abrirPopupUnidadEnTab(unidadId, 2); // Tab de Vehículos
                break;
            case 'btnTags':
                abrirPopupUnidadEnTab(unidadId, 3); // Tab de Tags
                break;
            case 'btnDocumentos':
                abrirPopupUnidadEnTab(unidadId, 4); // Tab de Documentos
                break;
            default:
                console.warn('onGridCustomButtonClick: Botón no reconocido:', e.buttonID);
        }
    } catch (error) {
        console.error('Error en onGridCustomButtonClick:', error);
        if (typeof showToast !== 'undefined') {
            showToast('error', 'Error al procesar la acción: ' + error.message);
        }
    }
};

// Alias global para compatibilidad y sobrescribir stub
var onGridCustomButtonClick = window.onGridCustomButtonClick;

// Función auxiliar para abrir el popup de unidad en un tab específico
function abrirPopupUnidadEnTab(unidadId, tabIndex) {
    console.log('abrirPopupUnidadEnTab: unidadId =', unidadId, ', tabIndex =', tabIndex);
    
    // Cargar la unidad completa
    ajaxCall('ObtenerUnidad', { id: unidadId }, function(r) {
        if (r.success && r.data) {
            currentUnidadId = unidadId;
            document.getElementById('hfId').value = unidadId;
            document.getElementById('hfUnidadIdActual').value = unidadId;
            
            // Cargar datos de la unidad
            cargarCombosUnidad(function() {
                cargarDatosUnidad(r.data);
                
                // Cargar subcatálogos
                cargarSubcatalogos(unidadId);
                
                // Obtener código con fallback a lowercase
                var codigo = r.data.Codigo || r.data.codigo || 'Unidad ' + unidadId;
                popupUnidad.SetHeaderText('Administrar: ' + codigo);
                
                // Activar el tab solicitado
                tabsUnidad.SetActiveTabIndex(tabIndex);
                
                // Mostrar el popup
                popupUnidad.Show();
            });
        } else {
            console.error('Error al obtener unidad:', r.message);
            showToast('error', r.message || 'Error al cargar la unidad');
        }
    });
}

// Verificar que la función esté disponible (para debug)
if (typeof window.onGridCustomButtonClick === 'function') {
    console.log('✅ onGridCustomButtonClick registrada correctamente');
} else {
    console.error('❌ Error: onGridCustomButtonClick no está definida');
}

// Función para cuando cambia la fila seleccionada en el grid
window.onUnidadFocusedRowChanged = function onUnidadFocusedRowChanged(s, e) {
    try {
        var idx = s.GetFocusedRowIndex();
        if (idx < 0) return;
        
        // Si estamos editando una unidad, no actualizar el mapa
        if (editandoUnidad) {
            console.log('⏸️ Editando unidad, no actualizar mapa');
            return;
        }
        
        // Verificar que el mapa esté inicializado y Google Maps esté disponible
        if (!mapaInicializado || !mapaUnidades || typeof google === 'undefined' || !google.maps) {
            console.log('Mapa no inicializado aún, intentando más tarde...');
            // Reintentar después de un breve delay
            setTimeout(function() {
                if (typeof window.onUnidadFocusedRowChanged === 'function') {
                    window.onUnidadFocusedRowChanged(s, e);
                }
            }, 500);
            return;
        }
        
        // Función auxiliar para mostrar el mapa en ubicación por defecto
        function mostrarMapaPorDefecto() {
            if (mapaUnidades && typeof google !== 'undefined' && google.maps) {
                var defaultLocation = new google.maps.LatLng(20.7131, -103.3889);
                mapaUnidades.setCenter(defaultLocation);
                mapaUnidades.setZoom(13);
                // Limpiar marcador si existe
                if (marcadorUnidad) {
                    marcadorUnidad.setMap(null);
                    marcadorUnidad = null;
                }
            }
        }
        
        // Obtener el ID de la unidad seleccionada
        var unidadId = s.GetRowKey(idx);
        
        // Validar que tengamos un ID válido
        if (!unidadId || unidadId === null || unidadId === undefined) {
            console.log('No hay unidad seleccionada (ID nulo), mostrando mapa por defecto');
            lastProcessedUnidadId = null;
            mostrarMapaPorDefecto();
            return;
        }
        
        // CAMBIO: En lugar de verificar lastProcessedUnidadId, verificar si ya hay un marcador
        // y si ese marcador corresponde a la misma unidad
        if (lastProcessedUnidadId === unidadId && marcadorUnidad !== null) {
            console.log('⏭️ Unidad', unidadId, 'ya tiene marcador visible, ignorando llamada duplicada');
            return;
        }
        
        lastProcessedUnidadId = unidadId;
        console.log('📍 Unidad seleccionada con ID:', unidadId);
        
        // Hacer una llamada al servidor para obtener los datos completos de la unidad
        // Esto es más confiable que intentar leer desde el grid con columnas dinámicas
        ajaxCall('ObtenerUnidad', { id: unidadId }, function(r) {
            if (r.success && r.data) {
                var latitud = r.data.Latitud;
                var longitud = r.data.Longitud;
                var numeroUnidad = r.data.Numero || r.data.numero || '';
                var edificio = r.data.Edificio || r.data.edificio || '';
                var codigo = r.data.Codigo || r.data.codigo || '';
                var nombre = r.data.Nombre || r.data.nombre || '';
                var propietarioPrincipal = r.data.PropietarioPrincipal || r.data.propietarioPrincipal || '';
                
                console.log('✅ Datos de unidad obtenidos del servidor:', { 
                    id: unidadId,
                    codigo: codigo,
                    nombre: nombre,
                    latitud: latitud, 
                    longitud: longitud, 
                    numero: numeroUnidad, 
                    edificio: edificio,
                    propietarioPrincipal: propietarioPrincipal
                });
                
                // Validar que tengamos valores válidos antes de actualizar el mapa
                if (latitud !== null && latitud !== undefined && 
                    longitud !== null && longitud !== undefined &&
                    parseFloat(latitud) !== 0 && parseFloat(longitud) !== 0) {
                    // Actualizar el mapa con las coordenadas y toda la información
                    actualizarMapaUnidad(unidadId, latitud, longitud, codigo, nombre, numeroUnidad, edificio, propietarioPrincipal);
                } else {
                    console.log('📍 No hay coordenadas válidas para esta unidad (lat:', latitud, ', lng:', longitud, '), mostrando mapa por defecto');
                    mostrarMapaPorDefecto();
                }
            } else {
                console.warn('No se pudieron obtener los datos de la unidad:', r.message || 'Error desconocido');
                mostrarMapaPorDefecto();
            }
        });
    } catch (error) {
        console.error('Error en onUnidadFocusedRowChanged:', error);
        // Si hay error, intentar mostrar el mapa en ubicación por defecto
        if (mapaUnidades && typeof google !== 'undefined' && google.maps) {
            try {
                var defaultLocation = new google.maps.LatLng(20.7131, -103.3889);
                mapaUnidades.setCenter(defaultLocation);
                mapaUnidades.setZoom(13);
            } catch (mapError) {
                console.error('Error al centrar mapa por defecto:', mapError);
            }
        }
    }
};
var onUnidadFocusedRowChanged = window.onUnidadFocusedRowChanged;

function mostrarNuevaUnidad() {
    limpiarFormularioUnidad();
    currentUnidadId = 0;
    document.getElementById('hfId').value = '0';
    document.getElementById('hfUnidadIdActual').value = '0';
    console.log('mostrarNuevaUnidad: Inicializado currentUnidadId a 0');
    cargarCombosUnidad();
    popupUnidad.SetHeaderText('Nueva Unidad Privativa');
    tabsUnidad.SetActiveTabIndex(0);
        popupUnidad.Show();
    }

    function editarUnidadSeleccionada() {
    var idx = gridUnidades.GetFocusedRowIndex();
    if (idx < 0) { showToast('warning', 'Seleccione una unidad'); return; }
    cargarUnidad(gridUnidades.GetRowKey(idx));
    }

    function eliminarUnidadSeleccionada() {
    var idx = gridUnidades.GetFocusedRowIndex();
    if (idx < 0) { showToast('warning', 'Seleccione una unidad'); return; }
    var id = gridUnidades.GetRowKey(idx);
    if (confirm('¿Eliminar esta unidad y todos sus datos asociados?')) {
        ajaxCall('EliminarUnidad', { id: id }, function(r) {
            if (r.success) { showToast('success', r.message); gridUnidades.Refresh(); }
            else showToast('error', r.message);
        });
        }
    }

    function cargarUnidad(id) {
    console.log('🔧 cargarUnidad llamada con ID:', id);
    console.log('🔧 Estado actual editandoUnidad:', editandoUnidad);
    
    editandoUnidad = true; // Activar flag para evitar que se limpie el mapa
    console.log('🔧 Flag editandoUnidad activado:', editandoUnidad);
    
    ajaxCall('ObtenerUnidad', { id: id }, function(r) {
        console.log('🔧 Respuesta de ObtenerUnidad:', r);
        
        if (r.success) {
            currentUnidadId = id;
            document.getElementById('hfId').value = id;
            document.getElementById('hfUnidadIdActual').value = id;
            console.log('cargarUnidad: Establecido currentUnidadId y hfUnidadIdActual a', id);
            
            cargarCombosUnidad(function() {
                console.log('🔧 Combos cargados, cargando datos...');
                cargarDatosUnidad(r.data);
                cargarSubcatalogos(id);
                
                // Obtener código con fallback a lowercase
                var codigo = r.data.Codigo || r.data.codigo || 'Unidad ' + id;
                popupUnidad.SetHeaderText('Administrar: ' + codigo);
                tabsUnidad.SetActiveTabIndex(0);
                
                console.log('🔧 Intentando mostrar popup...');
                console.log('🔧 popupUnidad existe:', typeof popupUnidad !== 'undefined');
                
                popupUnidad.Show();
                
                console.log('🔧 Popup.Show() ejecutado');
                
                // Verificar si el popup está visible después de un breve delay
                setTimeout(function() {
                    var isVisible = popupUnidad.IsVisible();
                    console.log('🔧 ¿Popup visible después de Show()?', isVisible);
                }, 100);
            });
        } else {
            console.error('🔧 Error al obtener unidad:', r.message);
            editandoUnidad = false; // Desactivar flag si hay error
            showToast('error', r.message);
        }
    });
}

// Función para cuando se cierra el popup de unidad
function onPopupUnidadClosed() {
    console.log('🔧 onPopupUnidadClosed llamada');
    console.log('🔧 Estado editandoUnidad antes:', editandoUnidad);
    
    editandoUnidad = false; // Desactivar flag cuando se cierra el popup
    
    console.log('🔧 Estado editandoUnidad después:', editandoUnidad);
    console.log('✅ Popup cerrado, flag editandoUnidad desactivado');
    
    // Recargar el grid mediante CallbackPanel (como en Entidades.aspx)
    if (typeof gridCallback !== 'undefined' && gridCallback) {
        console.log('🔄 Recargando grid después de cerrar popup...');
        gridCallback.PerformCallback();
    }
}

// Función específica para editar unidad desde el mapa (desde el InfoWindow)
window.editarUnidadDesdeMapa = function(unidadId) {
    console.log('🗺️ editarUnidadDesdeMapa llamada con ID:', unidadId);
    console.log('🗺️ Cerrando InfoWindow antes de abrir popup...');
    
    // Cerrar el InfoWindow si está abierto
    if (marcadorUnidad && marcadorUnidad.infoWindow) {
        marcadorUnidad.infoWindow.close();
    }
    
    // Pequeño delay para asegurar que el InfoWindow se cierre antes de abrir el popup
    setTimeout(function() {
        console.log('🗺️ Llamando a cargarUnidad...');
        cargarUnidad(unidadId);
    }, 100);
};

function cargarDatosUnidad(d) {
    console.log('🔧 cargarDatosUnidad - datos recibidos:', d);
    
    // Helper para obtener valor con fallback a lowercase
    function getValue(obj, key) {
        return obj[key] || obj[key.toLowerCase()] || '';
    }
    
    function getNumberValue(obj, key) {
        return obj[key] || obj[key.toLowerCase()] || null;
    }
    
    if (cboEntidad) cboEntidad.SetValue(d.entidad_id || d.EntidadId);
    if (cboSubEntidad) cboSubEntidad.SetValue(d.SubEntidadId || d.subentidadid);
    if (txtCodigo) txtCodigo.SetValue(getValue(d, 'Codigo'));
    if (txtNombre) txtNombre.SetValue(getValue(d, 'Nombre'));
    if (txtTorre) txtTorre.SetValue(getValue(d, 'Torre'));
    if (txtEdificio) txtEdificio.SetValue(getValue(d, 'Edificio'));
    if (txtPiso) txtPiso.SetValue(getValue(d, 'Piso'));
    if (txtNumero) txtNumero.SetValue(getValue(d, 'Numero'));
    if (txtCalle) txtCalle.SetValue(getValue(d, 'Calle'));
    if (txtNumeroExterior) txtNumeroExterior.SetValue(getValue(d, 'NumeroExterior'));
    if (txtNumeroInterior) txtNumeroInterior.SetValue(getValue(d, 'NumeroInterior'));
    if (txtReferencia) txtReferencia.SetValue(getValue(d, 'Referencia'));
    if (txtLatitud) txtLatitud.SetNumber(getNumberValue(d, 'Latitud'));
    if (txtLongitud) txtLongitud.SetNumber(getNumberValue(d, 'Longitud'));
    if (txtSuperficie) txtSuperficie.SetNumber(getNumberValue(d, 'Superficie') || 0);
    if (txtDescripcion) txtDescripcion.SetValue(getValue(d, 'Descripcion'));
    if (chkActivo) chkActivo.SetChecked((d.Activo || d.activo) == 1);
    
    console.log('✅ cargarDatosUnidad - datos cargados en formulario');
}

function limpiarFormularioUnidad() {
        if (cboEntidad) cboEntidad.SetValue(null);
        if (cboSubEntidad) cboSubEntidad.SetValue(null);
        if (txtCodigo) txtCodigo.SetValue('');
        if (txtNombre) txtNombre.SetValue('');
        if (txtTorre) txtTorre.SetValue('');
        if (txtEdificio) txtEdificio.SetValue('');
        if (txtPiso) txtPiso.SetValue('');
        if (txtNumero) txtNumero.SetValue('');
    if (txtCalle) txtCalle.SetValue('');
    if (txtNumeroExterior) txtNumeroExterior.SetValue('');
    if (txtNumeroInterior) txtNumeroInterior.SetValue('');
    if (txtReferencia) txtReferencia.SetValue('');
    if (txtLatitud) txtLatitud.SetValue(null);
    if (txtLongitud) txtLongitud.SetValue(null);
        if (txtSuperficie) txtSuperficie.SetNumber(0);
        if (txtDescripcion) txtDescripcion.SetValue('');
        if (chkActivo) chkActivo.SetChecked(true);
}

function guardarUnidad(callback) {
    var id = parseInt(document.getElementById('hfId').value) || 0;
    var datos = {
        id: id,
        entidadId: cboEntidad.GetValue(),
        subEntidadId: cboSubEntidad.GetValue(),
        codigo: txtCodigo.GetValue(),
        nombre: txtNombre.GetValue(),
        torre: txtTorre.GetValue(),
        edificio: txtEdificio.GetValue(),
        piso: txtPiso.GetValue(),
        numero: txtNumero.GetValue(),
        calle: txtCalle ? txtCalle.GetValue() : '',
        numeroExterior: txtNumeroExterior ? txtNumeroExterior.GetValue() : '',
        numeroInterior: txtNumeroInterior ? txtNumeroInterior.GetValue() : '',
        referencia: txtReferencia ? txtReferencia.GetValue() : '',
        latitud: txtLatitud ? txtLatitud.GetNumber() : null,
        longitud: txtLongitud ? txtLongitud.GetNumber() : null,
        superficie: txtSuperficie.GetNumber(),
        descripcion: txtDescripcion.GetValue(),
        activo: chkActivo.GetChecked()
    };
    ajaxCall('GuardarUnidad', { datos: datos }, function(r) {
        if (r.success) {
            var unidadIdGuardada = r.id || id;
            
            if (id == 0 && r.id) {
                currentUnidadId = r.id;
                document.getElementById('hfId').value = r.id;
                document.getElementById('hfUnidadIdActual').value = r.id;
                console.log('guardarUnidad: Nueva unidad guardada con ID:', r.id);
            }
            
            // Guardar todos los registros unbound (residentes, vehículos, tags, documentos)
            guardarRegistrosUnbound(unidadIdGuardada, function() {
                showToast('success', r.message + (r.message.indexOf('guardado') === -1 ? '. Todos los datos fueron guardados correctamente.' : ''));
                gridUnidades.Refresh();
                
                // Cargar los subcatálogos para la unidad
                cargarSubcatalogos(unidadIdGuardada);
                
                // Llamar callback si existe
                if (callback && typeof callback === 'function') {
                    callback(unidadIdGuardada);
                }
            });
        } else {
            showToast('error', r.message);
            if (callback && typeof callback === 'function') {
                callback(0);
            }
        }
    });
}

function guardarUnidadYContinuar(callback) {
    guardarUnidad(callback);
}

// Función para guardar todos los registros unbound después de guardar la unidad
function guardarRegistrosUnbound(unidadId, callback) {
    var totalRegistros = 0;
    var registrosGuardados = 0;
    var errorOcurrido = false;
    
    // Contar total de registros a guardar
    if (window.datosResidentesUnbound && window.datosResidentesUnbound.length > 0) {
        totalRegistros += window.datosResidentesUnbound.length;
    }
    if (window.datosVehiculosUnbound && window.datosVehiculosUnbound.length > 0) {
        totalRegistros += window.datosVehiculosUnbound.length;
    }
    if (window.datosTagsUnbound && window.datosTagsUnbound.length > 0) {
        totalRegistros += window.datosTagsUnbound.length;
    }
    if (window.datosDocumentosUnbound && window.datosDocumentosUnbound.length > 0) {
        totalRegistros += window.datosDocumentosUnbound.length;
    }
    
    // Si no hay registros, ejecutar callback inmediatamente
    if (totalRegistros === 0) {
        if (callback) callback();
        return;
    }
    
    // Función para verificar si se completaron todos
    function verificarCompletado() {
        registrosGuardados++;
        if (registrosGuardados >= totalRegistros) {
            if (!errorOcurrido && callback) {
                callback();
            }
        }
    }
    
    // Guardar residentes
    if (window.datosResidentesUnbound && window.datosResidentesUnbound.length > 0) {
        window.datosResidentesUnbound.forEach(function(item) {
            item.datos.unidadId = unidadId; // Actualizar unidadId
            ajaxCall('GuardarResidente', { datos: item.datos }, function(r) {
                if (r.success && r.data && r.data.id) {
                    var residenteId = r.data.id;
                    
                    // Guardar archivos del residente
                    if (item.archivos && item.archivos.length > 0) {
                        guardarArchivosResidente(residenteId, item.archivos);
                    }
                    if (item.archivosUnbound && item.archivosUnbound.length > 0) {
                        var archivosUnboundArray = [];
                        item.archivosUnbound.forEach(function(archivoItem) {
                            archivosUnboundArray.push({
                                nombre: archivoItem.datos.NombreArchivo,
                                base64: archivoItem.archivoBase64,
                                tipoMime: archivoItem.datos.TipoMime,
                                tamanio: archivoItem.datos.TamanioBytes
                            });
                        });
                        if (archivosUnboundArray.length > 0) {
                            guardarArchivosResidente(residenteId, archivosUnboundArray);
                        }
                    }
                } else {
                    errorOcurrido = true;
                    console.error('Error al guardar residente:', r.message);
                }
                verificarCompletado();
            });
        });
    }
    
    // Guardar vehículos
    if (window.datosVehiculosUnbound && window.datosVehiculosUnbound.length > 0) {
        window.datosVehiculosUnbound.forEach(function(item) {
            item.datos.unidadId = unidadId; // Actualizar unidadId
            ajaxCall('GuardarVehiculo', { datos: item.datos }, function(r) {
                if (r.success && r.data && r.data.id) {
                    var vehiculoId = r.data.id;
                    
                    // Guardar archivos del vehículo
                    if (item.archivos && item.archivos.length > 0) {
                        guardarArchivosVehiculo(vehiculoId, item.archivos);
                    }
                    if (item.archivosUnbound && item.archivosUnbound.length > 0) {
                        var archivosUnboundArray = [];
                        item.archivosUnbound.forEach(function(archivoItem) {
                            archivosUnboundArray.push({
                                nombre: archivoItem.datos.NombreArchivo,
                                base64: archivoItem.archivoBase64,
                                tipoMime: archivoItem.datos.TipoMime,
                                tamanio: archivoItem.datos.TamanioBytes
                            });
                        });
                        if (archivosUnboundArray.length > 0) {
                            guardarArchivosVehiculo(vehiculoId, archivosUnboundArray);
                        }
                    }
                } else {
                    errorOcurrido = true;
                    console.error('Error al guardar vehículo:', r.message);
                }
                verificarCompletado();
            });
        });
    }
    
    // Guardar tags
    if (window.datosTagsUnbound && window.datosTagsUnbound.length > 0) {
        window.datosTagsUnbound.forEach(function(item) {
            item.datos.unidadId = unidadId; // Actualizar unidadId
            ajaxCall('GuardarTag', { datos: item.datos }, function(r) {
                if (!r.success) {
                    errorOcurrido = true;
                    console.error('Error al guardar tag:', r.message);
                }
                verificarCompletado();
            });
        });
    }
    
    // Guardar documentos
    if (window.datosDocumentosUnbound && window.datosDocumentosUnbound.length > 0) {
        window.datosDocumentosUnbound.forEach(function(item) {
            item.datos.unidadId = unidadId; // Actualizar unidadId
            ajaxCall('GuardarDocumento', { datos: item.datos }, function(r) {
                if (r.success && r.data && r.data.id) {
                    var documentoId = r.data.id;
                    
                    // Guardar archivos del documento
                    if (item.archivos && item.archivos.length > 0) {
                        guardarArchivosDocumento(documentoId, item.archivos);
                    }
                } else {
                    errorOcurrido = true;
                    console.error('Error al guardar documento:', r.message);
                }
                verificarCompletado();
            });
        });
    }
    
    // Limpiar arrays después de guardar
    window.datosResidentesUnbound = [];
    window.datosVehiculosUnbound = [];
    window.datosTagsUnbound = [];
    window.datosDocumentosUnbound = [];
}

function cargarSubcatalogos(unidadId) {
    cargarResidentesGrid(unidadId);
    cargarVehiculosGrid(unidadId);
    cargarTagsGrid(unidadId);
    cargarDocumentosGrid(unidadId);
}

function cargarCombosUnidad(callback) {
    console.log('cargarCombosUnidad: Iniciando carga de combos...');
    
    // Verificar que los controles existan
    if (typeof cboEntidad === 'undefined' || !cboEntidad) {
        console.error('cargarCombosUnidad: cboEntidad no está definido');
        if (callback) callback();
        return;
    }
    
    // Cargar entidades
    console.log('cargarCombosUnidad: Llamando a ObtenerEntidades...');
    ajaxCall('ObtenerEntidades', {}, function(r) {
        console.log('cargarCombosUnidad: Respuesta recibida:', r);
        
        if (r && r.success && r.data && Array.isArray(r.data)) {
            console.log('cargarCombosUnidad: Cargando ' + r.data.length + ' entidades en el combo');
            cboEntidad.ClearItems();
            r.data.forEach(function(item) {
                if (item && item.Id && item.RazonSocial) {
                    cboEntidad.AddItem(item.RazonSocial, item.Id);
                }
            });
            console.log('cargarCombosUnidad: Entidades cargadas correctamente');
        } else {
            console.warn('cargarCombosUnidad: No se pudieron cargar las entidades. Respuesta:', r);
            if (r && r.message) {
                showToast('warning', 'No se pudieron cargar las entidades: ' + r.message);
            }
        }
        
        // Limpiar subentidades inicialmente
        if (typeof cboSubEntidad !== 'undefined' && cboSubEntidad) {
            cboSubEntidad.ClearItems();
            cboSubEntidad.AddItem('-- Ninguna --', null);
        }
        
        if (callback) callback();
    });
    }

// Función para cuando cambia la entidad seleccionada
window.onEntidadChanged = function onEntidadChanged(s, e) {
    try {
        var entidadId = s.GetValue();
        console.log('onEntidadChanged: Entidad seleccionada:', entidadId);
        
        if (!entidadId) {
            console.log('onEntidadChanged: No hay entidad seleccionada, limpiando subentidades');
            if (typeof cboSubEntidad !== 'undefined' && cboSubEntidad) {
                cboSubEntidad.ClearItems();
                cboSubEntidad.AddItem('-- Ninguna --', null);
                cboSubEntidad.SetValue(null);
                cboSubEntidad.SetSelectedIndex(-1);
            }
            return;
        }

        console.log('onEntidadChanged: Llamando a ObtenerSubEntidadesPorEntidad con entidadId:', entidadId);
        
        // Limpiar el combo de subentidades mientras carga
        if (typeof cboSubEntidad !== 'undefined' && cboSubEntidad) {
            cboSubEntidad.ClearItems();
            cboSubEntidad.SetValue(null);
        }
        
        ajaxCall('ObtenerSubEntidadesPorEntidad', { entidadId: entidadId }, function(r) {
            try {
                console.log('onEntidadChanged: Respuesta recibida:', r);
                
                if (r && r.success && r.data && Array.isArray(r.data) && typeof cboSubEntidad !== 'undefined' && cboSubEntidad) {
                    console.log('onEntidadChanged: Cargando ' + r.data.length + ' subentidades');
                    
                    // Limpiar y agregar items
                    cboSubEntidad.ClearItems();
                    cboSubEntidad.AddItem('-- Ninguna --', null);
                    
                    r.data.forEach(function(item) { 
                        if (item && item.Id && item.RazonSocial) {
                            console.log('onEntidadChanged: Agregando subentidad:', item.RazonSocial, item.Id);
                            cboSubEntidad.AddItem(item.RazonSocial, item.Id); 
                        }
                    });
                    
                    // Forzar actualización visual del combo
                    setTimeout(function() {
                        // Seleccionar "-- Ninguna --" por defecto
                        cboSubEntidad.SetSelectedIndex(0);
                        // Forzar actualización del control
                        if (typeof cboSubEntidad.GetItemCount === 'function') {
                            console.log('onEntidadChanged: Subentidades cargadas correctamente. Total items:', cboSubEntidad.GetItemCount());
                        } else {
                            console.log('onEntidadChanged: Subentidades cargadas correctamente');
                        }
                    }, 100);
                } else {
                    console.warn('onEntidadChanged: No se pudieron cargar las subentidades. Respuesta:', r);

                    if (typeof cboSubEntidad !== 'undefined' && cboSubEntidad) {
                        cboSubEntidad.ClearItems();
                        cboSubEntidad.AddItem('-- Ninguna --', null);
                    }

                    if (r && r.message) {
                        showToast('warning', 'No se pudieron cargar las subentidades: ' + r.message);
                    }
                }
            } catch (error) {
                console.error('Error en callback de onEntidadChanged:', error);
                if (typeof showToast !== 'undefined') {
                    showToast('error', 'Error al cargar subentidades: ' + error.message);
                }
            }
        });
    } catch (error) {
        console.error('Error en onEntidadChanged:', error);
        if (typeof showToast !== 'undefined') {
            showToast('error', 'Error al procesar cambio de entidad: ' + error.message);
        }
    }
};
var onEntidadChanged = window.onEntidadChanged;

// ========================================================================
// RESIDENTES
// ========================================================================
function abrirPopupResidentes(unidadId) {
    currentUnidadId = unidadId;
    document.getElementById('hfUnidadIdActual').value = unidadId;
    cargarResidentesGrid(unidadId);
    popupUnidad.SetHeaderText('Administrar: Unidad ' + unidadId);
    tabsUnidad.SetActiveTabIndex(1);
    popupUnidad.Show();
}

// Función para manejar clicks en el toolbar de residentes
window.onToolbarResidentesClick = function onToolbarResidentesClick(s, e) {
    try {
        console.log('onToolbarResidentesClick: Item clickeado:', e.item.name);
        switch (e.item.name) {
            case 'btnNuevoResidente': 
                console.log('onToolbarResidentesClick: Llamando a mostrarNuevoResidente');
                mostrarNuevoResidente(); 
                break;
            case 'btnEditarResidente': 
                console.log('onToolbarResidentesClick: Llamando a editarResidenteSeleccionado');
                editarResidenteSeleccionado(); 
                break;
            case 'btnEliminarResidente': 
                console.log('onToolbarResidentesClick: Llamando a eliminarResidenteSeleccionado');
                eliminarResidenteSeleccionado(); 
                break;
            default:
                console.log('onToolbarResidentesClick: Item no reconocido:', e.item.name);
        }
    } catch (error) {
        console.error('Error en onToolbarResidentesClick:', error);
        if (typeof showToast !== 'undefined') {
            showToast('error', 'Error al procesar la acción: ' + error.message);
        }
    }
};
var onToolbarResidentesClick = window.onToolbarResidentesClick;

function cargarResidentesGrid(unidadId) {
    if (gridResidentes) {
        gridResidentes.PerformCallback('cargar|' + unidadId);
    }
}

function mostrarNuevoResidente() {
    console.log('mostrarNuevoResidente: Iniciando...');
    console.log('mostrarNuevoResidente: currentUnidadId =', currentUnidadId);
    
    // Intentar obtener el popup directamente
    var popup = null;
    if (typeof popupResidente !== 'undefined' && popupResidente) {
        popup = popupResidente;
    } else {
        // Intentar usar getPopup como respaldo
        popup = getPopup('popupResidente');
    }
    
    if (!popup) {
        console.error('popupResidente no está disponible. Reintentando en 500ms...');
        setTimeout(function() {
            if (typeof popupResidente !== 'undefined' && popupResidente) {
                mostrarNuevoResidente();
            } else {
                console.error('popupResidente aún no está disponible después del reintento');
                showToast('error', 'Error al abrir el popup de residente. Por favor, recargue la página.');
            }
        }, 500);
        return;
    }
    
    try {
        // Permitir abrir el popup siempre - la validación se hará al guardar
        limpiarFormularioResidente();
        var hfResidenteId = document.getElementById('hfResidenteId');
        if (hfResidenteId) {
            hfResidenteId.value = '0';
        }
        
        // Asegurar que el mensaje verde se limpie al abrir
        if (typeof actualizarEstadoINE === 'function') {
            actualizarEstadoINE('', '');
        } else {
            var statusDiv = document.getElementById('ineStatus');
            if (statusDiv) {
                statusDiv.style.display = 'none';
                statusDiv.textContent = '';
            }
        }
        
        popup.SetHeaderText('Nuevo Residente');
        popup.Show();
        console.log('mostrarNuevoResidente: Popup abierto correctamente');
    } catch (error) {
        console.error('Error al abrir el popup de residente:', error);
        showToast('error', 'Error al abrir el popup de residente: ' + error.message);
    }
}

function editarResidenteSeleccionado() {
    var idx = gridResidentes.GetFocusedRowIndex();
    if (idx < 0) { showToast('warning', 'Seleccione un residente'); return; }
    var id = gridResidentes.GetRowKey(idx);
    
    // Validar que el ID no sea null o undefined
    if (!id || id === null || id === undefined || id === '') {
        console.error('❌ Error: No se pudo obtener el ID del residente. Verifique que la consulta SQL incluya r.Id');
        showToast('error', 'Error: No se pudo obtener el ID del residente. Por favor, recargue la página.');
        return;
    }
    
    // Asegurar que el ID sea un número
    id = parseInt(id, 10);
    if (isNaN(id) || id <= 0) {
        console.error('❌ Error: ID del residente inválido:', id);
        showToast('error', 'Error: ID del residente inválido. Por favor, recargue la página.');
        return;
    }
    
    ajaxCall('ObtenerResidente', { id: id }, function(r) {
        if (r.success) {
            // Intentar obtener el popup directamente
            var popup = null;
            if (typeof popupResidente !== 'undefined' && popupResidente) {
                popup = popupResidente;
            } else {
                popup = getPopup('popupResidente');
            }
            
            if (!popup) {
                console.error('popupResidente no está disponible');
                showToast('error', 'Error al abrir el popup de residente. Por favor, recargue la página.');
                return;
            }
            
            try {
                cargarDatosResidente(r.data);
                var hfResidenteId = document.getElementById('hfResidenteId');
                if (hfResidenteId) {
                    hfResidenteId.value = id;
                }
                popup.SetHeaderText('Editar Residente');
                popup.Show();
            } catch (error) {
                console.error('Error al editar residente:', error);
                showToast('error', 'Error al abrir el popup de residente: ' + error.message);
            }
        } else showToast('error', r.message);
    });
}

function eliminarResidenteSeleccionado() {
    var idx = gridResidentes.GetFocusedRowIndex();
    if (idx < 0) { showToast('warning', 'Seleccione un residente'); return; }

    if (confirm('¿Eliminar este residente?')) {
        ajaxCall('EliminarResidente', { id: gridResidentes.GetRowKey(idx) }, function(r) {
            if (r.success) { 
                showToast('success', r.message); 
                if (currentUnidadId) {
                    cargarResidentesGrid(currentUnidadId);
                }
            } else {
                showToast('error', r.message);
            }
        });
    }
}

function cargarDatosResidente(d) {
    if (cboTipoResidente) cboTipoResidente.SetValue(d.TipoResidente || 'Propietario');
    if (chkResPrincipal) chkResPrincipal.SetChecked(d.EsPrincipal == 1);
    if (txtResNombre) txtResNombre.SetValue(d.Nombre || '');
    if (txtResApPaterno) txtResApPaterno.SetValue(d.ApellidoPaterno || '');
    if (txtResApMaterno) txtResApMaterno.SetValue(d.ApellidoMaterno || '');
    if (txtResEmail) txtResEmail.SetValue(d.Email || '');
    if (txtResTelefono) txtResTelefono.SetValue(d.Telefono || '');
    if (txtResCelular) txtResCelular.SetValue(d.TelefonoCelular || '');
    if (txtResCURP) txtResCURP.SetValue(d.CURP || '');
    if (chkResActivo) chkResActivo.SetChecked(d.Activo == 1);
    
    // Cargar archivos del residente
    if (d.Id && d.Id > 0) {
        cargarArchivosResidente(d.Id);
    }
}

function limpiarFormularioResidente() {
    // Limpiar todos los controles del formulario
    if (cboTipoResidente) cboTipoResidente.SetValue('Propietario');
    if (chkResPrincipal) chkResPrincipal.SetChecked(false);
    if (txtResNombre) txtResNombre.SetValue('');
    if (txtResApPaterno) txtResApPaterno.SetValue('');
    if (txtResApMaterno) txtResApMaterno.SetValue('');
    if (txtResEmail) txtResEmail.SetValue('');
    if (txtResTelefono) txtResTelefono.SetValue('');
    if (txtResCelular) txtResCelular.SetValue('');
    if (txtResCURP) txtResCURP.SetValue('');
    if (chkResActivo) chkResActivo.SetChecked(true);
    
    // Limpiar archivos y arrays
    ineArchivosBase64 = [];
    window.archivosResidenteUnbound = [];
    
    // Limpiar input de archivos HTML nativo
    var fileInput = document.getElementById('ineFileInput');
    if (fileInput) {
        fileInput.value = '';
    }
    
    // Limpiar preview de archivos
    var filesPreview = document.getElementById('ineFilesPreview');
    if (filesPreview) {
        filesPreview.innerHTML = '';
    }
    
    // Ocultar lista de archivos
    var filesList = document.getElementById('ineFilesList');
    if (filesList) {
        filesList.style.display = 'none';
    }
    
    // Limpiar mensaje de estado (verde de éxito, errores, etc.)
    if (typeof actualizarEstadoINE === 'function') {
        actualizarEstadoINE('', '');
    } else {
        // Fallback: ocultar directamente el div de estado
        var statusDiv = document.getElementById('ineStatus');
        if (statusDiv) {
            statusDiv.style.display = 'none';
            statusDiv.textContent = '';
        }
    }
    
    // Actualizar preview
    actualizarPreviewINE();
    
    // SIEMPRE limpiar el grid de archivos cuando se limpia el formulario
    // (ya sea que se esté guardando o abriendo un nuevo popup)
    if (typeof gridArchivosResidente !== 'undefined' && gridArchivosResidente) {
        gridArchivosResidente.PerformCallback('cargar|0');
    }
    
    // Resetear flag de procesamiento
    ineProcesamientoDisparado = false;
}

function guardarResidente() {
    // NO hacer POST. Solo validar, guardar en memoria y cerrar popup.
    // El POST se hará cuando se guarde la unidad desde el popup principal.
    
    // Obtener unidadId (puede ser 0 si aún no está guardada, eso está bien)
    var unidadId = currentUnidadId;
    if (!unidadId || unidadId === 0) {
        var hfId = document.getElementById('hfId');
        if (hfId && hfId.value && hfId.value !== '0') {
            unidadId = parseInt(hfId.value);
        } else {
            // Si no hay unidad guardada, usar 0 (se guardará después junto con la unidad)
            unidadId = 0;
        }
    }
    
    // Llamar directamente a guardarResidenteContinuar sin hacer POST
    guardarResidenteContinuar(unidadId);
}

function guardarResidenteContinuar(unidadId) {
    currentUnidadId = unidadId;
    
    // Validar campos requeridos
    var nombre = txtResNombre.GetValue() || '';
    var apellidoPaterno = txtResApPaterno.GetValue() || '';
    var apellidoMaterno = txtResApMaterno.GetValue() || '';
    var email = txtResEmail.GetValue() || '';
    var celular = txtResCelular.GetValue() || '';
    
    if (!nombre || nombre.trim() === '') {
        showToast('warning', 'El nombre es requerido');
        txtResNombre.Focus();
        return;
    }
    
    if (!apellidoPaterno || apellidoPaterno.trim() === '') {
        showToast('warning', 'El apellido paterno es requerido');
        txtResApPaterno.Focus();
        return;
    }
    
    if (!apellidoMaterno || apellidoMaterno.trim() === '') {
        showToast('warning', 'El apellido materno es requerido');
        txtResApMaterno.Focus();
        return;
    }
    
    if (!email || email.trim() === '') {
        showToast('warning', 'El email es requerido');
        txtResEmail.Focus();
        return;
    }
    
    // Validar formato de email
    var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
        showToast('warning', 'El formato del email no es válido');
        txtResEmail.Focus();
        return;
    }
    
    if (!celular || celular.trim() === '') {
        showToast('warning', 'El celular es requerido');
        txtResCelular.Focus();
        return;
    }
    
    var datos = {
        id: parseInt(document.getElementById('hfResidenteId').value) || 0,
        unidadId: unidadId,
        tipoResidente: cboTipoResidente.GetValue(),
        esPrincipal: chkResPrincipal.GetChecked(),
        nombre: nombre.trim(),
        apellidoPaterno: apellidoPaterno.trim(),
        apellidoMaterno: apellidoMaterno.trim(),
        email: email.trim(),
        telefono: txtResTelefono.GetValue() || '',
        celular: celular.trim(),
        curp: txtResCURP.GetValue() || '',
        activo: chkResActivo.GetChecked()
    };
    
    // Si es edición (ID > 0), guardar inmediatamente en BD y recargar grid
    if (datos.id > 0) {
        // Guardar inmediatamente en BD
        ajaxCall('GuardarResidente', { datos: datos }, function(r) {
            if (r.success) {
                var residenteId = r.data && r.data.id ? r.data.id : datos.id;
                
                // Guardar archivos del residente si hay
                var archivosParaGuardar = ineArchivosBase64.slice();
                if (window.archivosResidenteUnbound && window.archivosResidenteUnbound.length > 0) {
                    var archivosUnboundArray = [];
                    window.archivosResidenteUnbound.forEach(function(archivoItem) {
                        archivosUnboundArray.push({
                            nombre: archivoItem.datos.NombreArchivo,
                            base64: archivoItem.archivoBase64,
                            tipoMime: archivoItem.datos.TipoMime,
                            tamanio: archivoItem.datos.TamanioBytes
                        });
                    });
                    archivosParaGuardar = archivosParaGuardar.concat(archivosUnboundArray);
                }
                
                if (archivosParaGuardar.length > 0) {
                    guardarArchivosResidente(residenteId, archivosParaGuardar);
                }
                
                showToast('success', 'Residente actualizado correctamente.');
                
                // Limpiar formulario
                document.getElementById('hfResidenteId').value = '0';
                limpiarFormularioResidente();
                
                // Cerrar popup
                popupResidente.Hide();
                
                // Recargar grid desde BD para mostrar los cambios
                if (gridResidentes && unidadId) {
                    cargarResidentesGrid(unidadId);
                }
                
                // Mostrar popup de unidad si no está visible
                if (popupUnidad && !popupUnidad.IsVisible()) {
                    popupUnidad.Show();
                }
            } else {
                showToast('error', r.message || 'Error al guardar el residente.');
            }
        });
        return; // Salir de la función después de guardar
    }
    
    // Si es nuevo (ID = 0), agregar al grid unbound para guardar después con la unidad
    // Preparar datos para agregar al grid
    var datosResidente = {
        Id: datos.id, // 0 para nuevos
        Nombre: datos.nombre,
        ApellidoPaterno: datos.apellidoPaterno,
        ApellidoMaterno: datos.apellidoMaterno,
        TipoResidente: datos.tipoResidente,
        EsPrincipal: datos.esPrincipal,
        Email: datos.email,
        Telefono: datos.telefono,
        Celular: datos.celular,
        CURP: datos.curp,
        Activo: datos.activo
    };
    
    // Guardar datos y archivos en memoria para guardarlos después junto con la unidad
    if (!window.datosResidentesUnbound) {
        window.datosResidentesUnbound = [];
    }
    window.datosResidentesUnbound.push({
        datos: datos,
        archivos: ineArchivosBase64.slice(), // Copia del array de archivos
        archivosUnbound: window.archivosResidenteUnbound ? window.archivosResidenteUnbound.slice() : []
    });
    
    // Agregar al grid usando callback
    if (gridResidentes) {
        gridResidentes.PerformCallback('agregarUnbound|' + unidadId + '|' + JSON.stringify(datosResidente));
    }
    
    showToast('success', 'Residente agregado. Los datos se guardarán al guardar la unidad.');
    
    // Limpiar formulario pero mantener archivos en memoria para guardar con la unidad
    document.getElementById('hfResidenteId').value = '0';
    limpiarFormularioResidente();
    
    // Cerrar popup y regresar al popup de unidad
    popupResidente.Hide();
    
    // Mostrar popup de unidad si no está visible
    if (popupUnidad && !popupUnidad.IsVisible()) {
        popupUnidad.Show();
    }
}

// ========================================================================
// VEHÍCULOS
// ========================================================================
function abrirPopupVehiculos(unidadId) {
    currentUnidadId = unidadId;
    document.getElementById('hfUnidadIdActual').value = unidadId;
    cargarVehiculosGrid(unidadId);
    popupUnidad.SetHeaderText('Administrar: Unidad ' + unidadId);
    tabsUnidad.SetActiveTabIndex(2);
    popupUnidad.Show();
}

// Función para manejar clicks en el toolbar de vehículos
window.onToolbarVehiculosClick = function onToolbarVehiculosClick(s, e) {
    try {
        console.log('onToolbarVehiculosClick: Item clickeado:', e.item.name);
        switch (e.item.name) {
            case 'btnNuevoVehiculo': 
                console.log('onToolbarVehiculosClick: Llamando a mostrarNuevoVehiculo');
                mostrarNuevoVehiculo(); 
                break;
            case 'btnEditarVehiculo': 
                console.log('onToolbarVehiculosClick: Llamando a editarVehiculoSeleccionado');
                editarVehiculoSeleccionado(); 
                break;
            case 'btnEliminarVehiculo': 
                console.log('onToolbarVehiculosClick: Llamando a eliminarVehiculoSeleccionado');
                eliminarVehiculoSeleccionado(); 
                break;
            default:
                console.log('onToolbarVehiculosClick: Item no reconocido:', e.item.name);
        }
    } catch (error) {
        console.error('Error en onToolbarVehiculosClick:', error);
        if (typeof showToast !== 'undefined') {
            showToast('error', 'Error al procesar la acción: ' + error.message);
        }
    }
};
var onToolbarVehiculosClick = window.onToolbarVehiculosClick;

function cargarVehiculosGrid(unidadId) {
    if (gridVehiculos) {
        gridVehiculos.PerformCallback('cargar|' + unidadId);
    }
}

function mostrarNuevoVehiculo() {
    console.log('=== mostrarNuevoVehiculo: Iniciando ===');
    
    // Intentar obtener el popup directamente primero
    var popup = null;
    
    // Método 1: Desde window
    if (typeof window.popupVehiculo !== 'undefined' && window.popupVehiculo) {
        popup = window.popupVehiculo;
        console.log('✓ Popup obtenido desde window.popupVehiculo');
    }
    
    // Método 2: Variable global directa
    if (!popup && typeof popupVehiculo !== 'undefined' && popupVehiculo) {
        popup = popupVehiculo;
        console.log('✓ Popup obtenido desde variable global popupVehiculo');
    }
    
    // Método 3: Cast de DevExpress
    if (!popup && typeof ASPxClientPopupControl !== 'undefined') {
        try {
            popup = ASPxClientPopupControl.Cast('popupVehiculo');
            if (popup) {
                console.log('✓ Popup obtenido usando ASPxClientPopupControl.Cast');
                // Registrarlo globalmente para futuros usos
                window.popupVehiculo = popup;
            }
        } catch (e) {
            console.warn('Error al usar ASPxClientPopupControl.Cast:', e.message);
        }
    }
    
    if (!popup) {
        console.error('❌ popupVehiculo no está disponible');
        console.error('Estado de variables:');
        console.error('  - window.popupVehiculo:', typeof window.popupVehiculo);
        console.error('  - popupVehiculo:', typeof popupVehiculo);
        console.error('  - ASPxClientPopupControl:', typeof ASPxClientPopupControl);
        
        // Listar todos los popups disponibles
        if (typeof ASPxClientPopupControl !== 'undefined' && ASPxClientPopupControl.GetControlCollection) {
            var popups = ASPxClientPopupControl.GetControlCollection();
            console.error('Popups disponibles:', popups);
            
            // Intentar encontrar el popup por ID en la colección
            if (popups && popups.GetByName) {
                popup = popups.GetByName('popupVehiculo');
                if (popup) {
                    console.log('✓ Popup encontrado en la colección por nombre');
                    window.popupVehiculo = popup;
                }
            }
        }
    }
    
    if (!popup) {
        showToast('error', 'Error al abrir el popup de vehículo. Por favor, recargue la página.');
        return;
    }
    
    try {
        // Permitir abrir el popup siempre - la validación se hará al guardar
        limpiarFormularioVehiculo();
        var hfVehiculoId = document.getElementById('hfVehiculoId');
        if (hfVehiculoId) {
            hfVehiculoId.value = '0';
        }
        
        // Asegurar que el mensaje verde se limpie al abrir (igual que en mostrarNuevoResidente)
        if (typeof actualizarEstadoTarjeta === 'function') {
            actualizarEstadoTarjeta('', '');
        } else {
            // Fallback: ocultar directamente el div de estado
            var statusDiv = document.getElementById('tarjetaStatus');
            if (statusDiv) {
                statusDiv.style.display = 'none';
                statusDiv.textContent = '';
            }
        }
        
        // Cargar combo de residentes si está disponible
        if (typeof cboVehResidente !== 'undefined' && cboVehResidente) {
            cargarComboResidentes(cboVehResidente);
        }
        
        popup.SetHeaderText('Nuevo Vehículo');
        popup.Show();
        console.log('mostrarNuevoVehiculo: Popup abierto correctamente');
    } catch (error) {
        console.error('Error al abrir el popup de vehículo:', error);
        showToast('error', 'Error al abrir el popup de vehículo: ' + error.message);
    }
}

function editarVehiculoSeleccionado() {
    var idx = gridVehiculos.GetFocusedRowIndex();
    if (idx < 0) { showToast('warning', 'Seleccione un vehículo'); return; }
    var id = gridVehiculos.GetRowKey(idx);
    
    // Validar que el ID no sea null o undefined
    if (!id || id === null || id === undefined || id === '') {
        console.error('❌ Error: No se pudo obtener el ID del vehículo. Verifique que la consulta SQL incluya v.Id');
        showToast('error', 'Error: No se pudo obtener el ID del vehículo. Por favor, recargue la página.');
        return;
    }
    
    // Asegurar que el ID sea un número
    id = parseInt(id, 10);
    if (isNaN(id) || id <= 0) {
        console.error('❌ Error: ID del vehículo inválido:', id);
        showToast('error', 'Error: ID del vehículo inválido. Por favor, recargue la página.');
        return;
    }
    
    ajaxCall('ObtenerVehiculo', { id: id }, function(r) {
        if (r.success) {
            // Intentar obtener el popup usando el mismo patrón que mostrarNuevoVehiculo
            var popup = null;
            
            // Método 1: Desde window
            if (typeof window.popupVehiculo !== 'undefined' && window.popupVehiculo) {
                popup = window.popupVehiculo;
            }
            
            // Método 2: Variable global directa
            if (!popup && typeof popupVehiculo !== 'undefined' && popupVehiculo) {
                popup = popupVehiculo;
            }
            
            // Método 3: Cast de DevExpress
            if (!popup && typeof ASPxClientPopupControl !== 'undefined') {
                try {
                    popup = ASPxClientPopupControl.Cast('popupVehiculo');
                } catch (e) {
                    console.warn('Error al usar ASPxClientPopupControl.Cast:', e.message);
                }
            }
            
            if (!popup) {
                console.error('popupVehiculo no está disponible');
                showToast('error', 'Error al abrir el popup de vehículo. Por favor, recargue la página.');
                return;
            }
            
            try {
                cargarComboResidentes(cboVehResidente, function() {
                    cargarDatosVehiculo(r.data);
                    var hfVehiculoId = document.getElementById('hfVehiculoId');
                    if (hfVehiculoId) {
                        hfVehiculoId.value = id;
                    }
                    popup.SetHeaderText('Editar Vehículo');
                    popup.Show();
                });
            } catch (error) {
                console.error('Error al editar vehículo:', error);
                showToast('error', 'Error al abrir el popup de vehículo: ' + error.message);
            }
        } else showToast('error', r.message);
    });
}

function eliminarVehiculoSeleccionado() {
    var idx = gridVehiculos.GetFocusedRowIndex();
    if (idx < 0) { showToast('warning', 'Seleccione un vehículo'); return; }

    if (confirm('¿Eliminar este vehículo?')) {
        ajaxCall('EliminarVehiculo', { id: gridVehiculos.GetRowKey(idx) }, function(r) {
            if (r.success) { 
                showToast('success', r.message); 
                if (currentUnidadId) {
                    cargarVehiculosGrid(currentUnidadId);
                }
            } else {
                showToast('error', r.message);
            }
        });
    }
}

function cargarDatosVehiculo(d) {
    if (txtVehPlacas) txtVehPlacas.SetValue(d.Placas || '');
    if (cboVehTipo) cboVehTipo.SetValue(d.TipoVehiculo || 'Automóvil');
    if (txtVehMarca) txtVehMarca.SetValue(d.Marca || '');
    if (txtVehModelo) txtVehModelo.SetValue(d.Modelo || '');
    if (txtVehAnio) txtVehAnio.SetNumber(d.Anio || new Date().getFullYear());
    if (txtVehColor) txtVehColor.SetValue(d.Color || '');
    if (txtVehNumeroMotor) txtVehNumeroMotor.SetValue(d.NumeroMotor || '');
    if (txtVehNumeroSerie) txtVehNumeroSerie.SetValue(d.NumeroSerie || '');
    if (cboVehTipoCombustible) cboVehTipoCombustible.SetValue(d.TipoCombustible || '');
    if (txtVehCapacidadPasajeros) txtVehCapacidadPasajeros.SetNumber(d.CapacidadPasajeros || null);
    if (cboVehUso) cboVehUso.SetValue(d.UsoVehiculo || 'Particular');
    if (txtVehTarjeton) txtVehTarjeton.SetValue(d.NumeroTarjeton || '');
    if (dteVehFechaExpedicion && d.FechaExpedicionTarjeta) dteVehFechaExpedicion.SetDate(parseDate(d.FechaExpedicionTarjeta));
    if (dteVehFechaVigencia && d.FechaVigenciaTarjeta) dteVehFechaVigencia.SetDate(parseDate(d.FechaVigenciaTarjeta));
    if (txtVehPropietarioRegistrado) txtVehPropietarioRegistrado.SetValue(d.PropietarioRegistrado || '');
    if (cboVehResidente) cboVehResidente.SetValue(d.ResidenteId);
    if (txtVehObservaciones) txtVehObservaciones.SetValue(d.Observaciones || '');
    if (chkVehActivo) chkVehActivo.SetChecked(d.Activo == 1);
    
    // Cargar archivos del vehículo
    if (d.Id && d.Id > 0) {
        cargarArchivosVehiculo(d.Id);
    }
}

function parseDate(dateString) {
    if (!dateString) return null;
    try {
        var date = new Date(dateString);
        if (isNaN(date.getTime())) return null;
        return date;
    } catch (e) {
        return null;
    }
}

function limpiarFormularioVehiculo() {
    // Limpiar todos los controles del formulario
    if (txtVehPlacas) txtVehPlacas.SetValue('');
    if (cboVehTipo) cboVehTipo.SetValue('Automóvil');
    if (txtVehMarca) txtVehMarca.SetValue('');
    if (txtVehModelo) txtVehModelo.SetValue('');
    if (txtVehAnio) txtVehAnio.SetNumber(new Date().getFullYear());
    if (txtVehColor) txtVehColor.SetValue('');
    if (txtVehNumeroMotor) txtVehNumeroMotor.SetValue('');
    if (txtVehNumeroSerie) txtVehNumeroSerie.SetValue('');
    if (cboVehTipoCombustible) cboVehTipoCombustible.SetValue(null);
    if (txtVehCapacidadPasajeros) txtVehCapacidadPasajeros.SetValue(null);
    if (cboVehUso) cboVehUso.SetValue('Particular');
    if (txtVehTarjeton) txtVehTarjeton.SetValue('');
    if (dteVehFechaExpedicion) dteVehFechaExpedicion.SetDate(null);
    if (dteVehFechaVigencia) dteVehFechaVigencia.SetDate(null);
    if (txtVehPropietarioRegistrado) txtVehPropietarioRegistrado.SetValue('');
    if (cboVehResidente) cboVehResidente.SetValue(null);
    if (txtVehObservaciones) txtVehObservaciones.SetValue('');
    if (chkVehActivo) chkVehActivo.SetChecked(true);
    
    // Limpiar archivos y arrays
    tarjetaArchivosBase64 = [];
    window.archivosVehiculoUnbound = [];
    
    // Limpiar input de archivos HTML nativo
    var fileInput = document.getElementById('tarjetaFileInput');
    if (fileInput) {
        fileInput.value = '';
    }
    
    // Limpiar preview de archivos
    var filesPreview = document.getElementById('tarjetaFilesPreview');
    if (filesPreview) {
        filesPreview.innerHTML = '';
    }
    
    // Ocultar lista de archivos
    var filesList = document.getElementById('tarjetaFilesList');
    if (filesList) {
        filesList.style.display = 'none';
    }
    
    // Limpiar mensaje de estado (verde de éxito, errores, etc.)
    if (typeof actualizarEstadoTarjeta === 'function') {
        actualizarEstadoTarjeta('', '');
    } else {
        // Fallback: ocultar directamente el div de estado
        var statusDiv = document.getElementById('tarjetaStatus');
        if (statusDiv) {
            statusDiv.style.display = 'none';
            statusDiv.textContent = '';
        }
    }
    
    // Actualizar preview
    actualizarPreviewTarjeta();
    
    // SIEMPRE limpiar el grid de archivos cuando se limpia el formulario
    // (ya sea que se esté guardando o abriendo un nuevo popup)
    if (typeof gridArchivosVehiculo !== 'undefined' && gridArchivosVehiculo) {
        gridArchivosVehiculo.PerformCallback('cargar|0');
    }
    
    // Resetear flag de procesamiento
    tarjetaProcesamientoDisparado = false;
}

function guardarVehiculo() {
    // NO hacer POST. Solo validar, guardar en memoria y cerrar popup.
    // El POST se hará cuando se guarde la unidad desde el popup principal.
    
    // Obtener unidadId (puede ser 0 si aún no está guardada, eso está bien)
    var unidadId = currentUnidadId;
    if (!unidadId || unidadId === 0) {
        var hfId = document.getElementById('hfId');
        if (hfId && hfId.value && hfId.value !== '0') {
            unidadId = parseInt(hfId.value);
        } else {
            // Si no hay unidad guardada, usar 0 (se guardará después junto con la unidad)
            unidadId = 0;
        }
    }
    
    // Llamar directamente a guardarVehiculoContinuar sin hacer POST
    guardarVehiculoContinuar(unidadId);
}

function guardarVehiculoContinuar(unidadId) {
    currentUnidadId = unidadId;
    
    // Validar campos requeridos
    var placas = txtVehPlacas.GetValue() || '';
    if (!placas || placas.trim() === '') {
        showToast('warning', 'Las placas son requeridas');
        txtVehPlacas.Focus();
        return;
    }
    
    var datos = {
        id: parseInt(document.getElementById('hfVehiculoId').value) || 0,
        unidadId: unidadId,
        residenteId: cboVehResidente.GetValue(),
        placas: placas.trim(),
        tipoVehiculo: cboVehTipo.GetValue(),
        marca: txtVehMarca.GetValue(),
        modelo: txtVehModelo.GetValue(),
        anio: txtVehAnio.GetNumber(),
        color: txtVehColor.GetValue(),
        numeroMotor: txtVehNumeroMotor ? txtVehNumeroMotor.GetValue() : '',
        numeroSerie: txtVehNumeroSerie ? txtVehNumeroSerie.GetValue() : '',
        tipoCombustible: cboVehTipoCombustible ? cboVehTipoCombustible.GetValue() : '',
        capacidadPasajeros: txtVehCapacidadPasajeros ? txtVehCapacidadPasajeros.GetNumber() : null,
        usoVehiculo: cboVehUso ? cboVehUso.GetValue() : 'Particular',
        numeroTarjeton: txtVehTarjeton.GetValue(),
        fechaExpedicionTarjeta: dteVehFechaExpedicion ? dteVehFechaExpedicion.GetDate() : null,
        fechaVigenciaTarjeta: dteVehFechaVigencia ? dteVehFechaVigencia.GetDate() : null,
        propietarioRegistrado: txtVehPropietarioRegistrado ? txtVehPropietarioRegistrado.GetValue() : '',
        observaciones: txtVehObservaciones.GetValue(),
        activo: chkVehActivo.GetChecked()
    };
    
    // Si es edición (ID > 0), guardar inmediatamente en BD y recargar grid
    if (datos.id > 0) {
        // Guardar inmediatamente en BD
        ajaxCall('GuardarVehiculo', { datos: datos }, function(r) {
            if (r.success) {
                var vehiculoId = r.data && r.data.id ? r.data.id : datos.id;
                
                // Guardar archivos del vehículo si hay
                var archivosParaGuardar = tarjetaArchivosBase64.slice();
                if (window.archivosVehiculoUnbound && window.archivosVehiculoUnbound.length > 0) {
                    var archivosUnboundArray = [];
                    window.archivosVehiculoUnbound.forEach(function(archivoItem) {
                        archivosUnboundArray.push({
                            nombre: archivoItem.datos.NombreArchivo,
                            base64: archivoItem.archivoBase64,
                            tipoMime: archivoItem.datos.TipoMime,
                            tamanio: archivoItem.datos.TamanioBytes
                        });
                    });
                    archivosParaGuardar = archivosParaGuardar.concat(archivosUnboundArray);
                }
                
                if (archivosParaGuardar.length > 0) {
                    guardarArchivosVehiculo(vehiculoId, archivosParaGuardar);
                }
                
                showToast('success', 'Vehículo actualizado correctamente.');
                
                // Limpiar formulario
                document.getElementById('hfVehiculoId').value = '0';
                limpiarFormularioVehiculo();
                
                // Cerrar popup
                popupVehiculo.Hide();
                
                // Recargar grid desde BD para mostrar los cambios
                if (gridVehiculos && unidadId) {
                    cargarVehiculosGrid(unidadId);
                }
                
                // Mostrar popup de unidad si no está visible
                if (popupUnidad && !popupUnidad.IsVisible()) {
                    popupUnidad.Show();
                }
            } else {
                showToast('error', r.message || 'Error al guardar el vehículo.');
            }
        });
        return; // Salir de la función después de guardar
    }
    
    // Si es nuevo (ID = 0), agregar al grid unbound para guardar después con la unidad
    // Guardar datos y archivos en memoria para guardarlos después junto con la unidad
    if (!window.datosVehiculosUnbound) {
        window.datosVehiculosUnbound = [];
    }
    window.datosVehiculosUnbound.push({
        datos: datos,
        archivos: tarjetaArchivosBase64.slice(), // Copia del array de archivos
        archivosUnbound: window.archivosVehiculoUnbound ? window.archivosVehiculoUnbound.slice() : []
    });
    
    // Agregar al grid usando callback
    if (gridVehiculos) {
        var datosVehiculo = {
            Id: datos.id,
            Placas: datos.placas,
            TipoVehiculo: datos.tipoVehiculo,
            Marca: datos.marca || '',
            Modelo: datos.modelo || '',
            Anio: datos.anio || null,
            Color: datos.color || '',
            ResidenteNombre: '' // Se puede llenar después
        };
        gridVehiculos.PerformCallback('agregarUnbound|' + unidadId + '|' + JSON.stringify(datosVehiculo));
    }
    
    showToast('success', 'Vehículo agregado. Los datos se guardarán al guardar la unidad.');
    
    // Limpiar formulario pero mantener archivos en memoria para guardar con la unidad
    document.getElementById('hfVehiculoId').value = '0';
    limpiarFormularioVehiculo();
    
    // Cerrar popup y regresar al popup de unidad
    popupVehiculo.Hide();
    
    // Mostrar popup de unidad si no está visible
    if (popupUnidad && !popupUnidad.IsVisible()) {
        popupUnidad.Show();
    }
}

// ========================================================================
// TAGS
// ========================================================================
function abrirPopupTags(unidadId) {
    currentUnidadId = unidadId;
    document.getElementById('hfUnidadIdActual').value = unidadId;
    cargarTagsGrid(unidadId);
    popupUnidad.SetHeaderText('Administrar: Unidad ' + unidadId);
    tabsUnidad.SetActiveTabIndex(3);
    popupUnidad.Show();
}

function abrirPopupDocumentos(unidadId) {
    currentUnidadId = unidadId;
    document.getElementById('hfUnidadIdActual').value = unidadId;
    cargarDocumentosGrid(unidadId);
    popupUnidad.SetHeaderText('Administrar: Unidad ' + unidadId);
    tabsUnidad.SetActiveTabIndex(4);
    popupUnidad.Show();
}

// Función para manejar clicks en el toolbar de tags
window.onToolbarTagsClick = function onToolbarTagsClick(s, e) {
    try {
        console.log('onToolbarTagsClick: Item clickeado:', e.item.name);
        switch (e.item.name) {
            case 'btnNuevoTag': 
                console.log('onToolbarTagsClick: Llamando a mostrarNuevoTag');
                mostrarNuevoTag(); 
                break;
            case 'btnEditarTag': 
                console.log('onToolbarTagsClick: Llamando a editarTagSeleccionado');
                editarTagSeleccionado(); 
                break;
            case 'btnEliminarTag': 
                console.log('onToolbarTagsClick: Llamando a eliminarTagSeleccionado');
                eliminarTagSeleccionado(); 
                break;
            default:
                console.log('onToolbarTagsClick: Item no reconocido:', e.item.name);
        }
    } catch (error) {
        console.error('Error en onToolbarTagsClick:', error);
        if (typeof showToast !== 'undefined') {
            showToast('error', 'Error al procesar la acción: ' + error.message);
        }
    }
};
var onToolbarTagsClick = window.onToolbarTagsClick;

function cargarTagsGrid(unidadId) {
    if (gridTags) {
        gridTags.PerformCallback('cargar|' + unidadId);
    }
}

function mostrarNuevoTag() {
    console.log('mostrarNuevoTag: Iniciando...');
    
    // Verificar que el popup esté disponible
    if (typeof popupTag === 'undefined' || !popupTag) {
        console.error('popupTag no está disponible. Reintentando en 500ms...');
        setTimeout(function() {
            if (typeof popupTag !== 'undefined' && popupTag) {
                mostrarNuevoTag();
            } else {
                console.error('popupTag aún no está disponible después del reintento');
                showToast('error', 'Error al abrir el popup de tag. Por favor, recargue la página.');
            }
        }, 500);
        return;
    }
    
    try {
        // Permitir abrir el popup siempre - la validación se hará al guardar
        limpiarFormularioTag();
        var hfTagId = document.getElementById('hfTagId');
        if (hfTagId) {
            hfTagId.value = '0';
        }
        
        // Cargar combo de residentes si está disponible
        if (typeof cboTagResidente !== 'undefined' && cboTagResidente) {
            cargarComboResidentes(cboTagResidente);
        }
        
        popupTag.SetHeaderText('Nuevo Tag de Acceso');
        popupTag.Show();
        console.log('mostrarNuevoTag: Popup abierto correctamente');
    } catch (error) {
        console.error('Error al abrir el popup de tag:', error);
        showToast('error', 'Error al abrir el popup de tag: ' + error.message);
    }
}

function editarTagSeleccionado() {
    var idx = gridTags.GetFocusedRowIndex();
    if (idx < 0) { showToast('warning', 'Seleccione un tag'); return; }
    var id = gridTags.GetRowKey(idx);
    ajaxCall('ObtenerTag', { id: id }, function(r) {
        if (r.success) {
            cargarComboResidentes(cboTagResidente, function() {
                cargarDatosTag(r.data);
                document.getElementById('hfTagId').value = id;
                popupTag.SetHeaderText('Editar Tag');
                popupTag.Show();
            });
        } else showToast('error', r.message);
    });
}

function eliminarTagSeleccionado() {
    var idx = gridTags.GetFocusedRowIndex();
    if (idx < 0) { showToast('warning', 'Seleccione un tag'); return; }

    if (confirm('¿Eliminar este tag?')) {
        ajaxCall('EliminarTag', { id: gridTags.GetRowKey(idx) }, function(r) {
            if (r.success) { 
                showToast('success', r.message); 
                if (currentUnidadId) {
                    cargarTagsGrid(currentUnidadId);
                }
            } else {
                showToast('error', r.message);
            }
        });
    }
}

function cargarDatosTag(d) {
    if (txtTagCodigo) txtTagCodigo.SetValue(d.CodigoTag || '');
    if (cboTagTipo) cboTagTipo.SetValue(d.TipoTag || 'RFID');
    if (txtTagDescripcion) txtTagDescripcion.SetValue(d.Descripcion || '');
    if (cboTagResidente) cboTagResidente.SetValue(d.ResidenteId);
    if (dteTagAsignacion && d.FechaAsignacion) dteTagAsignacion.SetDate(new Date(d.FechaAsignacion));
    if (dteTagVencimiento && d.FechaVencimiento) dteTagVencimiento.SetDate(new Date(d.FechaVencimiento));
    if (chkTagPeatonal) chkTagPeatonal.SetChecked(d.PermiteAccesoPeatonal == 1);
    if (chkTagVehicular) chkTagVehicular.SetChecked(d.PermiteAccesoVehicular == 1);
    if (chkTagAreas) chkTagAreas.SetChecked(d.PermiteAccesoAreas == 1);
    if (txtTagObservaciones) txtTagObservaciones.SetValue(d.Observaciones || '');
    if (chkTagActivo) chkTagActivo.SetChecked(d.Activo == 1);
}

function limpiarFormularioTag() {
    // Limpiar todos los controles del formulario
    if (txtTagCodigo) txtTagCodigo.SetValue('');
    if (cboTagTipo) cboTagTipo.SetValue('RFID');
    if (txtTagDescripcion) txtTagDescripcion.SetValue('');
    if (cboTagResidente) cboTagResidente.SetValue(null);
    if (dteTagAsignacion) dteTagAsignacion.SetDate(new Date());
    if (dteTagVencimiento) dteTagVencimiento.SetDate(null);
    if (chkTagPeatonal) chkTagPeatonal.SetChecked(true);
    if (chkTagVehicular) chkTagVehicular.SetChecked(false);
    if (chkTagAreas) chkTagAreas.SetChecked(false);
    if (txtTagObservaciones) txtTagObservaciones.SetValue('');
    if (chkTagActivo) chkTagActivo.SetChecked(true);
    
    // Nota: Los tags no tienen archivos asociados, solo datos
}

function guardarTag() {
    // NO hacer POST. Solo validar, guardar en memoria y cerrar popup.
    // El POST se hará cuando se guarde la unidad desde el popup principal.
    
    // Obtener unidadId (puede ser 0 si aún no está guardada, eso está bien)
    var unidadId = currentUnidadId;
    if (!unidadId || unidadId === 0) {
        var hfId = document.getElementById('hfId');
        if (hfId && hfId.value && hfId.value !== '0') {
            unidadId = parseInt(hfId.value);
        } else {
            // Si no hay unidad guardada, usar 0 (se guardará después junto con la unidad)
            unidadId = 0;
        }
    }
    
    // Llamar directamente a guardarTagContinuar sin hacer POST
    guardarTagContinuar(unidadId);
}

function guardarTagContinuar(unidadId) {
    // Validar campos requeridos
    var codigoTag = txtTagCodigo.GetValue() || '';
    if (!codigoTag || codigoTag.trim() === '') {
        showToast('warning', 'El código del tag es requerido');
        txtTagCodigo.Focus();
        return;
    }
    
    var datos = {
        id: parseInt(document.getElementById('hfTagId').value) || 0,
        unidadId: unidadId,
        residenteId: cboTagResidente.GetValue(),
        codigoTag: codigoTag.trim(),
        tipoTag: cboTagTipo.GetValue(),
        descripcion: txtTagDescripcion.GetValue(),
        fechaAsignacion: dteTagAsignacion.GetDate(),
        fechaVencimiento: dteTagVencimiento.GetDate(),
        permiteAccesoPeatonal: chkTagPeatonal.GetChecked(),
        permiteAccesoVehicular: chkTagVehicular.GetChecked(),
        permiteAccesoAreas: chkTagAreas.GetChecked(),
        observaciones: txtTagObservaciones.GetValue(),
        activo: chkTagActivo.GetChecked()
    };
    
    // NO hacer POST, solo agregar al grid unbound y cerrar popup
    // El POST se hará cuando se guarde la unidad desde el popup principal
    
    // Guardar datos en memoria para guardarlos después junto con la unidad
    if (!window.datosTagsUnbound) {
        window.datosTagsUnbound = [];
    }
    window.datosTagsUnbound.push({ datos: datos });
    
    // Agregar al grid usando callback
    if (gridTags) {
        var datosTag = {
            Id: datos.id,
            CodigoTag: datos.codigoTag,
            TipoTag: datos.tipoTag || '',
            Descripcion: datos.descripcion || '',
            ResidenteNombre: '' // Se puede llenar después
        };
        gridTags.PerformCallback('agregarUnbound|' + unidadId + '|' + JSON.stringify(datosTag));
    }
    
    showToast('success', 'Tag agregado. Los datos se guardarán al guardar la unidad.');
    
    // Limpiar formulario
    document.getElementById('hfTagId').value = '0';
    limpiarFormularioTag();
    
    // Cerrar popup y regresar al popup de unidad
    popupTag.Hide();
    
    // Mostrar popup de unidad si no está visible
    if (popupUnidad && !popupUnidad.IsVisible()) {
        popupUnidad.Show();
    }
}

// ========================================================================
// DOCUMENTOS
// ========================================================================
// Función para manejar clicks en el toolbar de documentos
window.onToolbarDocumentosClick = function onToolbarDocumentosClick(s, e) {
    try {
        console.log('onToolbarDocumentosClick: Item clickeado:', e.item.name);
        switch (e.item.name) {
            case 'btnNuevoDocumento': 
                console.log('onToolbarDocumentosClick: Llamando a mostrarNuevoDocumento');
                mostrarNuevoDocumento(); 
                break;
            case 'btnEditarDocumento':
                console.log('onToolbarDocumentosClick: Llamando a editarDocumentoSeleccionado');
                editarDocumentoSeleccionado();
                break;
            case 'btnVerDocumento': 
                console.log('onToolbarDocumentosClick: Llamando a verDocumentoSeleccionado');
                verDocumentoSeleccionado(); 
                break;
            case 'btnEliminarDocumento': 
                console.log('onToolbarDocumentosClick: Llamando a eliminarDocumentoSeleccionado');
                eliminarDocumentoSeleccionado(); 
                break;
            default:
                console.log('onToolbarDocumentosClick: Item no reconocido:', e.item.name);
        }
    } catch (error) {
        console.error('Error en onToolbarDocumentosClick:', error);
        if (typeof showToast !== 'undefined') {
            showToast('error', 'Error al procesar la acción: ' + error.message);
        }
    }
};
var onToolbarDocumentosClick = window.onToolbarDocumentosClick;

function cargarDocumentosGrid(unidadId) {
    if (gridDocumentos) {
        gridDocumentos.PerformCallback('cargar|' + unidadId);
    }
}

function mostrarNuevoDocumento() {
    console.log('mostrarNuevoDocumento: Iniciando...');
    console.log('mostrarNuevoDocumento: currentUnidadId =', currentUnidadId);
    
    // Intentar obtener el popup directamente
    var popup = null;
    if (typeof popupDocumento !== 'undefined' && popupDocumento) {
        popup = popupDocumento;
    } else {
        // Intentar usar getPopup como respaldo
        popup = getPopup('popupDocumento');
    }
    
    if (!popup) {
        console.error('popupDocumento no está disponible. Reintentando en 500ms...');
        setTimeout(function() {
            if (typeof popupDocumento !== 'undefined' && popupDocumento) {
                mostrarNuevoDocumento();
            } else {
                console.error('popupDocumento aún no está disponible después del reintento');
                showToast('error', 'Error al abrir el popup de documento. Por favor, recargue la página.');
            }
        }, 500);
        return;
    }
    
    try {
        // Permitir abrir el popup siempre - la validación se hará al guardar
        limpiarFormularioDocumento();
        var hfDocumentoId = document.getElementById('hfDocumentoId');
        if (hfDocumentoId) {
            hfDocumentoId.value = '0';
        }
        popup.SetHeaderText('Subir Documento');
        
        // Inicializar input de archivos múltiples
        setTimeout(function() {
            if (typeof initDocumentoFileInput === 'function') {
                initDocumentoFileInput();
            }
        }, 100);
        
        popup.Show();
        console.log('mostrarNuevoDocumento: Popup abierto correctamente');
    } catch (error) {
        console.error('Error al abrir el popup de documento:', error);
        showToast('error', 'Error al abrir el popup de documento: ' + error.message);
    }
}

function editarDocumentoSeleccionado() {
    var idx = gridDocumentos.GetFocusedRowIndex();
    if (idx < 0) { showToast('warning', 'Seleccione un documento'); return; }
    var id = gridDocumentos.GetRowKey(idx);
    ajaxCall('ObtenerDocumento', { id: id }, function(r) {
        if (r.success) {
            // Intentar obtener el popup directamente
            var popup = null;
            if (typeof popupDocumento !== 'undefined' && popupDocumento) {
                popup = popupDocumento;
            } else {
                popup = getPopup('popupDocumento');
            }
            
            if (!popup) {
                console.error('popupDocumento no está disponible');
                showToast('error', 'Error al abrir el popup de documento. Por favor, recargue la página.');
                return;
            }
            
            try {
                cargarDatosDocumento(r.data);
                var hfDocumentoId = document.getElementById('hfDocumentoId');
                if (hfDocumentoId) {
                    hfDocumentoId.value = id;
                }
            popup.SetHeaderText('Editar Documento');
            
            // Inicializar input de archivos múltiples
            setTimeout(function() {
                if (typeof initDocumentoFileInput === 'function') {
                    initDocumentoFileInput();
                }
                // Cargar archivos del documento
                cargarArchivosDocumento(id);
            }, 100);
            
                popup.Show();
            } catch (error) {
                console.error('Error al editar documento:', error);
                showToast('error', 'Error al abrir el popup de documento: ' + error.message);
            }
        } else {
            showToast('error', r.message);
        }
    });
}

function cargarDatosDocumento(d) {
    if (cboDocTipo) cboDocTipo.SetValue(d.TipoDocumento || 'Escritura');
    if (txtDocNumero) txtDocNumero.SetValue(d.NumeroDocumento || '');
    if (txtDocNombre) txtDocNombre.SetValue(d.Nombre || '');
    if (dteDocFecha && d.FechaDocumento) dteDocFecha.SetDate(parseDate(d.FechaDocumento));
    if (txtDocNotaria) txtDocNotaria.SetValue(d.Notaria || '');
    if (dteDocVigenciaInicio && d.FechaVigenciaInicio) dteDocVigenciaInicio.SetDate(parseDate(d.FechaVigenciaInicio));
    if (dteDocVigenciaFin && d.FechaVigenciaFin) dteDocVigenciaFin.SetDate(parseDate(d.FechaVigenciaFin));
    if (txtDocDescripcion) txtDocDescripcion.SetValue(d.Descripcion || '');
    if (chkDocActivo) chkDocActivo.SetChecked(d.Activo == 1);
    
    // Cargar archivos del documento
    if (d.Id && d.Id > 0) {
        cargarArchivosDocumento(d.Id);
    }
}

function verDocumentoSeleccionado() {
    var idx = gridDocumentos.GetFocusedRowIndex();
    if (idx < 0) { showToast('warning', 'Seleccione un documento'); return; }
    var id = gridDocumentos.GetRowKey(idx);
    // Cargar archivos del documento y abrir el primero si existe
    ajaxCall('ObtenerArchivosDocumento', { documentoId: id }, function(r) {
        if (r.success && r.data && r.data.length > 0) {
            var primerArchivo = r.data[0];
            window.open('/Views/Consultas/VisorArchivo.aspx?tipo=documento&id=' + id + '&archivoId=' + primerArchivo.Id, '_blank', 'width=1024,height=768');
        } else {
            showToast('warning', 'El documento no tiene archivos adjuntos');
        }
    });
}

function eliminarDocumentoSeleccionado() {
    var idx = gridDocumentos.GetFocusedRowIndex();
    if (idx < 0) { showToast('warning', 'Seleccione un documento'); return; }

    if (confirm('¿Eliminar este documento?')) {
        ajaxCall('EliminarDocumento', { id: gridDocumentos.GetRowKey(idx) }, function(r) {
            if (r.success) { 
                showToast('success', r.message); 
                if (currentUnidadId) {
                    cargarDocumentosGrid(currentUnidadId);
                }
            } else {
                showToast('error', r.message);
            }
        });
    }
}

function limpiarFormularioDocumento() {
    // Limpiar todos los controles del formulario
    if (cboDocTipo) cboDocTipo.SetValue('Escritura');
    if (txtDocNumero) txtDocNumero.SetValue('');
    if (txtDocNombre) txtDocNombre.SetValue('');
    if (dteDocFecha) dteDocFecha.SetDate(null);
    if (txtDocNotaria) txtDocNotaria.SetValue('');
    if (dteDocVigenciaInicio) dteDocVigenciaInicio.SetDate(null);
    if (dteDocVigenciaFin) dteDocVigenciaFin.SetDate(null);
    if (txtDocDescripcion) txtDocDescripcion.SetValue('');
    if (chkDocActivo) chkDocActivo.SetChecked(true);
    
    // Limpiar hidden fields
    var hfDocRutaArchivo = document.getElementById('hfDocRutaArchivo');
    if (hfDocRutaArchivo) {
        hfDocRutaArchivo.value = '';
    }
    var hfDocNombreArchivo = document.getElementById('hfDocNombreArchivo');
    if (hfDocNombreArchivo) {
        hfDocNombreArchivo.value = '';
    }
    
    // Limpiar archivos y arrays
    documentoArchivosBase64 = [];
    
    // Limpiar input de archivos HTML nativo
    var fileInput = document.getElementById('documentoFileInput');
    if (fileInput) {
        fileInput.value = '';
    }
    
    // Limpiar preview de archivos
    var filesPreview = document.getElementById('documentoFilesPreview');
    if (filesPreview) {
        filesPreview.innerHTML = '';
    }
    
    // Ocultar lista de archivos
    var filesList = document.getElementById('documentoFilesList');
    if (filesList) {
        filesList.style.display = 'none';
    }
    
    // Actualizar preview
    actualizarPreviewDocumento();
    
    // SIEMPRE limpiar el grid de archivos cuando se limpia el formulario
    // (ya sea que se esté guardando o abriendo un nuevo popup)
    if (typeof gridArchivosDocumento !== 'undefined' && gridArchivosDocumento) {
        gridArchivosDocumento.PerformCallback('cargar|0');
    }
}

// Variable global para almacenar el archivo en Base64 antes de subirlo
// Arrays para almacenar múltiples archivos
var ineArchivosBase64 = []; // Array de objetos: { nombre, base64, base64Completo, tipoMime, tamanio }
var tarjetaArchivosBase64 = [];
var documentoArchivosBase64 = [];

// Variables globales para rastrear si ya se disparó el procesamiento automático
var ineProcesamientoDisparado = false;
var tarjetaProcesamientoDisparado = false;

// Función verDocumento actualizada para ver el primer archivo del documento (si existe)
function verDocumento() {
    var documentoId = parseInt(document.getElementById('hfDocumentoId').value) || 0;
    if (documentoId > 0) {
        // Cargar archivos del documento y abrir el primero
        ajaxCall('ObtenerArchivosDocumento', { documentoId: documentoId }, function(r) {
            if (r.success && r.data && r.data.length > 0) {
                var primerArchivo = r.data[0];
                window.open('/Views/Consultas/VisorArchivo.aspx?tipo=documento&id=' + documentoId + '&archivoId=' + primerArchivo.Id, '_blank', 'width=1024,height=768');
            } else {
                showToast('warning', 'No hay archivos para visualizar');
            }
        });
    } else if (documentoArchivosBase64 && documentoArchivosBase64.length > 0) {
        // Si hay archivos cargados pero no guardados, mostrar el primero en una nueva ventana
        var archivo = documentoArchivosBase64[0];
        var ventana = window.open('', '_blank', 'width=1024,height=768');
        var esPDF = archivo.base64Completo.indexOf('data:application/pdf') === 0;
        if (esPDF) {
            ventana.document.write('<iframe src="' + archivo.base64Completo + '" style="width:100%;height:100%;border:none;"></iframe>');
        } else {
            ventana.document.write('<img src="' + archivo.base64Completo + '" style="max-width:100%;max-height:100%;" />');
        }
    } else {
        showToast('warning', 'No hay documento para visualizar');
    }
}

function guardarDocumento() {
    // NO hacer POST. Solo validar, guardar en memoria y cerrar popup.
    // El POST se hará cuando se guarde la unidad desde el popup principal.
    
    // Obtener unidadId (puede ser 0 si aún no está guardada, eso está bien)
    var unidadId = currentUnidadId;
    if (!unidadId || unidadId === 0) {
        var hfId = document.getElementById('hfId');
        if (hfId && hfId.value && hfId.value !== '0') {
            unidadId = parseInt(hfId.value);
        } else {
            // Si no hay unidad guardada, usar 0 (se guardará después junto con la unidad)
            unidadId = 0;
        }
    }
    
    // Llamar directamente a guardarDocumentoContinuar sin hacer POST
    guardarDocumentoContinuar(unidadId);
}

function guardarDocumentoContinuar(unidadId) {
    // Validar campos requeridos
    var nombre = txtDocNombre.GetValue() || '';
    if (!nombre || nombre.trim() === '') {
        showToast('warning', 'El nombre del documento es requerido');
        txtDocNombre.Focus();
        return;
    }
    
    var datos = {
        id: parseInt(document.getElementById('hfDocumentoId').value) || 0,
        unidadId: unidadId,
        tipoDocumento: cboDocTipo.GetValue(),
        numeroDocumento: txtDocNumero.GetValue(),
        nombre: nombre.trim(),
        fechaDocumento: dteDocFecha.GetDate(),
        notaria: txtDocNotaria.GetValue(),
        fechaVigenciaInicio: dteDocVigenciaInicio.GetDate(),
        fechaVigenciaFin: dteDocVigenciaFin.GetDate(),
        descripcion: txtDocDescripcion.GetValue(),
        rutaArchivo: document.getElementById('hfDocRutaArchivo').value,
        nombreArchivo: document.getElementById('hfDocNombreArchivo').value,
        activo: chkDocActivo.GetChecked()
    };
    
    // NO hacer POST, solo agregar al grid unbound y cerrar popup
    // El POST se hará cuando se guarde la unidad desde el popup principal
    
    // Guardar datos y archivos en memoria para guardarlos después junto con la unidad
    if (!window.datosDocumentosUnbound) {
        window.datosDocumentosUnbound = [];
    }
    window.datosDocumentosUnbound.push({
        datos: datos,
        archivos: documentoArchivosBase64.slice() // Copia del array de archivos
    });
    
    // Agregar al grid usando callback
    if (gridDocumentos) {
        var datosDocumento = {
            Id: datos.id,
            TipoDocumento: datos.tipoDocumento || '',
            NumeroDocumento: datos.numeroDocumento || '',
            Nombre: datos.nombre,
            FechaDocumento: datos.fechaDocumento || null
        };
        gridDocumentos.PerformCallback('agregarUnbound|' + unidadId + '|' + JSON.stringify(datosDocumento));
    }
    
    showToast('success', 'Documento agregado. Los datos se guardarán al guardar la unidad.');
    
    // Limpiar formulario
    document.getElementById('hfDocumentoId').value = '0';
    limpiarFormularioDocumento();
    
    // Cerrar popup y regresar al popup de unidad
    popupDocumento.Hide();
    
    // Mostrar popup de unidad si no está visible
    if (popupUnidad && !popupUnidad.IsVisible()) {
        popupUnidad.Show();
    }
}

// ========================================================================
// INICIALIZACIÓN DE GRIDS EN POPUP
// ========================================================================
function inicializarGridsPopup() {
    console.log('inicializarGridsPopup: Iniciando inicialización de grids...');
    
    // Esperar un poco para que los grids se rendericen completamente
    setTimeout(function() {
        try {
            // Reinicializar eventos del toolbar para cada grid si existen
            if (typeof gridResidentes !== 'undefined' && gridResidentes) {
                console.log('inicializarGridsPopup: Grid residentes encontrado');
                // Los eventos ya están registrados vía ClientSideEvents en el ASPX
                // Solo verificamos que el grid esté disponible
            }
            
            if (typeof gridVehiculos !== 'undefined' && gridVehiculos) {
                console.log('inicializarGridsPopup: Grid vehículos encontrado');
            }

            if (typeof gridTags !== 'undefined' && gridTags) {
                console.log('inicializarGridsPopup: Grid tags encontrado');
            }

            if (typeof gridDocumentos !== 'undefined' && gridDocumentos) {
                console.log('inicializarGridsPopup: Grid documentos encontrado');
            }
            
            console.log('inicializarGridsPopup: Inicialización completada');
        } catch (ex) {
            console.error('inicializarGridsPopup: Error durante inicialización:', ex);
        }
    }, 200);
}

// ========================================================================
// HELPERS
// ========================================================================
function ajaxCall(method, data, callback) {
    // Usar fetch API en lugar de jQuery
    // Para WebMethods en ASP.NET, la URL debe ser: PageName.aspx/MethodName
    var pagePath = window.location.pathname; // Obtener la ruta completa de la página actual
    var url = pagePath + '/' + method;
    console.log('ajaxCall: Llamando a', url, 'con datos:', data);
    
    fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json; charset=utf-8'
        },
        body: JSON.stringify(data)
    })
    .then(function(response) {
        console.log('ajaxCall: Respuesta HTTP recibida. Status:', response.status, 'OK:', response.ok);
        if (!response.ok) {
            // Intentar leer el cuerpo de la respuesta para más detalles
            return response.text().then(function(text) {
                console.error('ajaxCall: Error en respuesta. Texto:', text);
                throw new Error('Error HTTP: ' + response.status + ' - ' + text.substring(0, 200));
            });
        }
        return response.json();
    })
    .then(function(response) {
        console.log('ajaxCall: JSON parseado:', response);
        // ASP.NET WebMethods devuelven el resultado en response.d
        if (response && response.d !== undefined) {
            callback(response.d);
        } else {
            callback(response);
        }
    })
    .catch(function(error) {
        console.error('ajaxCall: Error completo:', error);
        showToast('error', 'Error de comunicación: ' + (error.message || 'Error desconocido'));
        // Llamar callback con error para que no se quede bloqueado
        if (callback) {
            callback({ success: false, message: 'Error de comunicación: ' + (error.message || 'Error desconocido') });
        }
    });
}

function cargarComboResidentes(combo, callback) {
    if (!combo) { if (callback) callback(); return; }
    ajaxCall('ObtenerResidentesUnidad', { unidadId: currentUnidadId }, function(r) {
        if (r.success) {
            combo.ClearItems();
            combo.AddItem('-- Sin asignar --', null);
            r.data.forEach(function(item) {
                combo.AddItem(item.NombreCompleto, item.Id);
            });
        }
        if (callback) callback();
    });
}

// ========================================================================
// ESCANEO DE INE CON AZURE DOCUMENT INTELLIGENCE
// ========================================================================

// Función de inicialización para múltiples archivos INE
function initINEScanner() {
    var dropZone = document.getElementById('ineDropZone');
    var fileInput = document.getElementById('ineFileInput');

    if (!dropZone || !fileInput) return;

    // Click para seleccionar archivos (múltiples)
    dropZone.addEventListener('click', function () {
        fileInput.click();
    });

    // Cambio de archivos (múltiples)
    fileInput.addEventListener('change', function (e) {
        if (e.target.files && e.target.files.length > 0) {
            onINEFileInputChange(e);
        }
    });

    // Drag & Drop múltiple
    dropZone.addEventListener('dragover', function (e) {
        e.preventDefault();
        e.stopPropagation();
        dropZone.style.backgroundColor = '#e0e0e0';
    });

    dropZone.addEventListener('dragleave', function (e) {
        e.preventDefault();
        e.stopPropagation();
        dropZone.style.backgroundColor = '#f9f9f9';
    });

    dropZone.addEventListener('drop', function (e) {
        e.preventDefault();
        e.stopPropagation();
        dropZone.style.backgroundColor = '#f9f9f9';

        if (e.dataTransfer.files && e.dataTransfer.files.length > 0) {
            onINEFileInputChange({ target: { files: e.dataTransfer.files } });
        }
    });
}

function procesarArchivoINE(file) {
    // Validar tipo de archivo
    var validTypes = ['image/jpeg', 'image/png', 'image/jpg', 'application/pdf'];
    if (validTypes.indexOf(file.type) === -1) {
        showToast('error', 'Formato de archivo no válido. Use JPG, PNG o PDF.');
        return;
    }

    // Validar tamaño (máx 10MB)
    if (file.size > 10 * 1024 * 1024) {
        showToast('error', 'El archivo es demasiado grande. Máximo 10MB.');
        return;
    }

    var reader = new FileReader();
    reader.onload = function (e) {
        ineImageBase64 = e.target.result;
        mostrarPreviewINE(e.target.result);
    };
    reader.readAsDataURL(file);
}

function mostrarPreviewINE(dataUrl) {
    var dropZone = document.getElementById('ineDropZone');
    var previewContainer = document.getElementById('inePreviewContainer');
    var previewImage = document.getElementById('inePreviewImage');

    if (dropZone) dropZone.style.display = 'none';
    if (previewContainer) previewContainer.style.display = 'block';
    if (previewImage) previewImage.src = dataUrl;
    
    // Mostrar botón Ver INE cuando hay imagen
    if (btnVerINE) {
        btnVerINE.SetVisible(true);
    }

    actualizarEstadoINE('Imagen cargada. Haga clic en "Escanear INE" para extraer los datos.', 'info');
}

function removeINEImage() {
    ineImageBase64 = null;

    var dropZone = document.getElementById('ineDropZone');
    var previewContainer = document.getElementById('inePreviewContainer');
    var previewImage = document.getElementById('inePreviewImage');
    var fileInput = document.getElementById('ineFileInput');

    if (dropZone) dropZone.style.display = 'block';
    if (previewContainer) previewContainer.style.display = 'none';
    if (previewImage) previewImage.src = '';
    if (fileInput) fileInput.value = '';
    
    // Ocultar botón Ver INE
    if (btnVerINE) {
        btnVerINE.SetVisible(false);
    }

    actualizarEstadoINE('', '');
}

function verINE() {
    var residenteId = parseInt(document.getElementById('hfResidenteId').value) || 0;
    if (residenteId > 0) {
        // Cargar archivos del residente y abrir el primero si existe
        ajaxCall('ObtenerArchivosResidente', { residenteId: residenteId }, function(r) {
            if (r.success && r.data && r.data.length > 0) {
                var primerArchivo = r.data[0];
                window.open('/Views/Consultas/VisorArchivo.aspx?tipo=residente&id=' + residenteId + '&archivoId=' + primerArchivo.Id, '_blank', 'width=1024,height=768');
            } else if (ineArchivosBase64 && ineArchivosBase64.length > 0) {
                // Si hay archivos cargados pero no guardados, mostrar el primero
                var archivo = ineArchivosBase64[0];
                var ventana = window.open('', '_blank', 'width=1024,height=768');
                var esPDF = archivo.base64Completo.indexOf('data:application/pdf') === 0;
                if (esPDF) {
                    ventana.document.write('<iframe src="' + archivo.base64Completo + '" style="width:100%;height:100%;border:none;"></iframe>');
                } else {
                    ventana.document.write('<img src="' + archivo.base64Completo + '" style="max-width:100%;max-height:100%;" />');
                }
            } else {
                showToast('warning', 'No hay archivos de INE para visualizar');
            }
        });
    } else if (ineArchivosBase64 && ineArchivosBase64.length > 0) {
        // Si hay archivos cargados pero no guardados, mostrar el primero
        var archivo = ineArchivosBase64[0];
        var ventana = window.open('', '_blank', 'width=1024,height=768');
        var esPDF = archivo.base64Completo.indexOf('data:application/pdf') === 0;
        if (esPDF) {
            ventana.document.write('<iframe src="' + archivo.base64Completo + '" style="width:100%;height:100%;border:none;"></iframe>');
        } else {
            ventana.document.write('<img src="' + archivo.base64Completo + '" style="max-width:100%;max-height:100%;" />');
        }
    } else {
        showToast('warning', 'No hay imagen de INE para visualizar');
    }
}

function escanearINE() {
    // Obtener el primer archivo del array si existe
    var archivoObj = null;
    if (ineArchivosBase64 && ineArchivosBase64.length > 0) {
        archivoObj = ineArchivosBase64[0];
    }
    
    if (!archivoObj || !archivoObj.archivoOriginal) {
        mostrarIndicadorProcesamientoINE(false);
        showToast('warning','Primero cargue una imagen de INE');
        return;
    }

    // Mostrar estado de procesamiento
    mostrarIndicadorProcesamientoINE(true);
    actualizarEstadoINE('Procesando imagen con Azure Document Intelligence...', 'processing');

    // Crear FormData con el archivo
    var formData = new FormData();
    formData.append('archivo', archivoObj.archivoOriginal);
    formData.append('tipoDocumento', 'INE');

    // Enviar archivo al proxy (el proxy maneja la autenticación JWT)
    fetch('/Services/DocumentIntelligenceProxy.ashx', {
        method: 'POST',
        body: formData,
        signal: new AbortController().signal
    })
    .then(function(response) {
        if (!response.ok) {
            return response.json().then(function(err) {
                throw new Error(err.Mensaje || 'Error HTTP: ' + response.status);
            });
        }
        return response.json();
    })
    .then(function(response) {
        mostrarIndicadorProcesamientoINE(false);
        // La respuesta viene en formato List<CrudDto>
        if (response && response.length > 0 && response[0].Campos) {
            var datos = response[0].Campos;
            llenarCamposDesdeINE(datos);
            actualizarEstadoINE('✓ Datos extraídos correctamente de la INE', 'success');
            showToast('success', 'Datos de INE extraídos. Complete los datos faltantes y guarde cuando termine.');
            
            var residenteId = parseInt(document.getElementById('hfResidenteId').value) || 0;
            if (ineArchivosBase64.length > 0) {
                var archivo = ineArchivosBase64[0];
                
                if (residenteId > 0) {
                    // Si el residente ya existe (edición), guardar el archivo inmediatamente
                    guardarArchivoResidenteYAgregarGrid(residenteId, archivo);
                } else {
                    // Si es un nuevo residente, agregar al grid de manera unbound (en memoria)
                    agregarArchivoResidenteUnbound(archivo);
                }
                
                // Remover el archivo del array después de procesarlo
                ineArchivosBase64.shift();
                actualizarPreviewINE();
            }
        } else {
            actualizarEstadoINE('✗ No se pudieron extraer datos', 'error');
            showToast('error', 'No se pudieron extraer datos del documento');
        }
    })
    .catch(function(error) {
        mostrarIndicadorProcesamientoINE(false);
        var mensaje = 'Error al comunicarse con el servidor';
        if (error.name === 'AbortError') {
            mensaje = 'Tiempo de espera agotado. Intente con una imagen más pequeña.';
        } else if (error.message) {
            mensaje = error.message;
        }
        actualizarEstadoINE('✗ ' + mensaje, 'error');
        showToast('error', mensaje);
    });
}

function llenarCamposDesdeINE(datos) {
    if (!datos) return;

    // Si datos viene en formato CrudDto (con Campos.Valor), convertir a formato simple
    var datosSimples = {};
    if (datos && typeof datos === 'object' && !Array.isArray(datos)) {
        for (var key in datos) {
            if (datos.hasOwnProperty(key)) {
                var campo = datos[key];
                if (campo && typeof campo === 'object' && campo.Valor !== undefined) {
                    datosSimples[key] = campo.Valor;
                } else {
                    datosSimples[key] = campo;
                }
            }
        }
    } else {
        datosSimples = datos;
    }

    // Mapear datos a los campos del formulario de residente
    if (datosSimples.nombre && typeof txtResNombre !== 'undefined') {
        txtResNombre.SetValue(datosSimples.nombre);
    }
    if (datosSimples.apellidoPaterno && typeof txtResApPaterno !== 'undefined') {
        txtResApPaterno.SetValue(datosSimples.apellidoPaterno);
    }
    if (datosSimples.apellidoMaterno && typeof txtResApMaterno !== 'undefined') {
        txtResApMaterno.SetValue(datosSimples.apellidoMaterno);
    }
    if (datosSimples.curp && typeof txtResCURP !== 'undefined') {
        txtResCURP.SetValue(datosSimples.curp);
    }
    // Nota: Los demás campos (email, teléfono, etc.) no están en la INE
    // pero podrían extraerse de otros documentos o llenarse manualmente
}

/**
 * Agrega un residente al grid de manera unbound (sin guardar en BD)
 * Los datos se mantienen en memoria hasta que el usuario haga clic en Guardar
 */
function agregarResidenteAlGridUnbound(datos) {
    if (!datos || !gridResidentes) return;
    
    try {
        // Obtener datos del formulario actual y combinarlos con los datos escaneados
        var datosResidente = {
            Id: 0, // ID temporal 0 para indicar que no está guardado
            TipoResidente: (typeof cboTipoResidente !== 'undefined' && cboTipoResidente) ? cboTipoResidente.GetValue() : 'Propietario',
            EsPrincipal: (typeof chkResPrincipal !== 'undefined' && chkResPrincipal) ? chkResPrincipal.GetChecked() : false,
            Nombre: datos.nombre || '',
            ApellidoPaterno: datos.apellidoPaterno || '',
            ApellidoMaterno: datos.apellidoMaterno || '',
            Email: (typeof txtResEmail !== 'undefined' && txtResEmail) ? txtResEmail.GetValue() : '',
            Telefono: (typeof txtResTelefono !== 'undefined' && txtResTelefono) ? txtResTelefono.GetValue() : '',
            Celular: (typeof txtResCelular !== 'undefined' && txtResCelular) ? txtResCelular.GetValue() : '',
            CURP: datos.curp || '',
            Activo: (typeof chkResActivo !== 'undefined' && chkResActivo) ? chkResActivo.GetChecked() : true
        };
        
        // Agregar la fila usando el callback del servidor
        if (gridResidentes) {
            // Obtener unidadId actual para pasarlo si es necesario
            var unidadId = currentUnidadId || 0;
            var hfId = document.getElementById('hfId');
            if (!unidadId && hfId && hfId.value && hfId.value !== '0') {
                unidadId = parseInt(hfId.value);
            }
            
            // Incluir unidadId en el callback
            gridResidentes.PerformCallback('agregarUnbound|' + unidadId + '|' + JSON.stringify(datosResidente));
            console.log('Residente agregado al grid (unbound):', datosResidente);
        }
        
    } catch (ex) {
        console.error('Error al agregar residente al grid:', ex);
        showToast('error', 'Error al agregar el registro al grid: ' + ex.message);
    }
}

/**
 * Muestra u oculta el indicador de procesamiento para INE
 */
function mostrarIndicadorProcesamientoINE(mostrar) {
    var indicator = document.getElementById('ineProcessingIndicator');
    if (indicator) {
        indicator.style.display = mostrar ? 'block' : 'none';
    }
}

function actualizarEstadoINE(mensaje, tipo) {
    var statusDiv = document.getElementById('ineStatus');
    if (!statusDiv) return;

    if (!mensaje || mensaje === '') {
        statusDiv.style.display = 'none';
        return;
    }

    statusDiv.style.display = 'block';
    statusDiv.textContent = mensaje;

    // Aplicar estilos según el tipo
    statusDiv.className = '';
    switch (tipo) {
        case 'success':
            statusDiv.style.backgroundColor = '#d4edda';
            statusDiv.style.color = '#155724';
            statusDiv.style.border = '1px solid #c3e6cb';
            break;
        case 'error':
            statusDiv.style.backgroundColor = '#f8d7da';
            statusDiv.style.color = '#721c24';
            statusDiv.style.border = '1px solid #f5c6cb';
            break;
        case 'processing':
            statusDiv.style.backgroundColor = '#d1ecf1';
            statusDiv.style.color = '#0c5460';
            statusDiv.style.border = '1px solid #bee5eb';
            break;
        case 'info':
        default:
            statusDiv.style.backgroundColor = '#d1ecf1';
            statusDiv.style.color = '#0c5460';
            statusDiv.style.border = '1px solid #bee5eb';
            break;
    }
}

// Nota: La inicialización del escáner INE se maneja mediante ClientSideEvents Shown
// en el ASPX del popupResidente. No se necesita inicializar aquí.

// ========================================================================
// ESCANEO DE TARJETA DE CIRCULACIÓN CON AZURE DOCUMENT INTELLIGENCE
// ========================================================================

var tarjetaImageBase64 = null;

function initTarjetaScanner() {
    var dropZone = document.getElementById('tarjetaDropZone');
    var fileInput = document.getElementById('tarjetaFileInput');

    if (!dropZone || !fileInput) return;

    // Click para seleccionar archivos (múltiples)
    dropZone.addEventListener('click', function () {
        fileInput.click();
    });

    // Cambio de archivos (múltiples)
    fileInput.addEventListener('change', function (e) {
        if (e.target.files && e.target.files.length > 0) {
            onTarjetaFileInputChange(e);
        }
    });

    // Drag & Drop múltiple
    dropZone.addEventListener('dragover', function (e) {
        e.preventDefault();
        e.stopPropagation();
        dropZone.style.backgroundColor = '#e0e0e0';
    });

    dropZone.addEventListener('dragleave', function (e) {
        e.preventDefault();
        e.stopPropagation();
        dropZone.style.backgroundColor = '#f9f9f9';
    });

    dropZone.addEventListener('drop', function (e) {
        e.preventDefault();
        e.stopPropagation();
        dropZone.style.backgroundColor = '#f9f9f9';

        if (e.dataTransfer.files && e.dataTransfer.files.length > 0) {
            onTarjetaFileInputChange({ target: { files: e.dataTransfer.files } });
        }
    });
}

// Inicialización para documentos múltiples
function initDocumentoFileInput() {
    var dropZone = document.getElementById('documentoDropZone');
    var fileInput = document.getElementById('documentoFileInput');

    if (!dropZone || !fileInput) return;

    dropZone.addEventListener('click', function () {
        fileInput.click();
    });

    fileInput.addEventListener('change', function (e) {
        if (e.target.files && e.target.files.length > 0) {
            onDocumentoFileInputChange(e);
        }
    });

    dropZone.addEventListener('dragover', function (e) {
        e.preventDefault();
        dropZone.style.backgroundColor = '#e0e0e0';
    });

    dropZone.addEventListener('dragleave', function (e) {
        e.preventDefault();
        dropZone.style.backgroundColor = '#f9f9f9';
    });

    dropZone.addEventListener('drop', function (e) {
        e.preventDefault();
        dropZone.style.backgroundColor = '#f9f9f9';
        if (e.dataTransfer.files && e.dataTransfer.files.length > 0) {
            onDocumentoFileInputChange({ target: { files: e.dataTransfer.files } });
        }
    });
}

// Inicializar cuando se carga la página o se muestra el popup
if (typeof document !== 'undefined' && document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', function() {
        initDocumentoFileInput();
    });
} else {
    // Si ya está cargado
    setTimeout(function() {
        initDocumentoFileInput();
    }, 500);
}

function procesarArchivoTarjeta(file) {
    // Validar tipo de archivo
    var validTypes = ['image/jpeg', 'image/png', 'image/jpg', 'application/pdf'];
    if (validTypes.indexOf(file.type) === -1) {
        showToast('error', 'Formato de archivo no válido. Use JPG, PNG o PDF.');
        return;
    }

    // Validar tamaño (máx 10MB)
    if (file.size > 10 * 1024 * 1024) {
        showToast('error', 'El archivo es demasiado grande. Máximo 10MB.');
        return;
    }

    var reader = new FileReader();
    reader.onload = function (e) {
        tarjetaImageBase64 = e.target.result;
        mostrarPreviewTarjeta(e.target.result);
    };
    reader.readAsDataURL(file);
}

function mostrarPreviewTarjeta(dataUrl) {
    var dropZone = document.getElementById('tarjetaDropZone');
    var previewContainer = document.getElementById('tarjetaPreviewContainer');
    var previewImage = document.getElementById('tarjetaPreviewImage');

    if (dropZone) dropZone.style.display = 'none';
    if (previewContainer) previewContainer.style.display = 'block';
    if (previewImage) previewImage.src = dataUrl;
    
    // Mostrar botón Ver Tarjeta cuando hay imagen
    if (btnVerTarjeta) {
        btnVerTarjeta.SetVisible(true);
    }

    actualizarEstadoTarjeta('Imagen cargada. Haga clic en "Escanear Tarjeta" para extraer los datos.', 'info');
}

function removeTarjetaImage() {
    tarjetaImageBase64 = null;

    var dropZone = document.getElementById('tarjetaDropZone');
    var previewContainer = document.getElementById('tarjetaPreviewContainer');
    var previewImage = document.getElementById('tarjetaPreviewImage');
    var fileInput = document.getElementById('tarjetaFileInput');

    if (dropZone) dropZone.style.display = 'block';
    if (previewContainer) previewContainer.style.display = 'none';
    if (previewImage) previewImage.src = '';
    if (fileInput) fileInput.value = '';
    
    // Ocultar botón Ver Tarjeta
    if (btnVerTarjeta) {
        btnVerTarjeta.SetVisible(false);
    }

    actualizarEstadoTarjeta('', '');
}

function verTarjeta() {
    var vehiculoId = parseInt(document.getElementById('hfVehiculoId').value) || 0;
    if (vehiculoId > 0) {
        // Cargar archivos del vehículo y abrir el primero si existe
        ajaxCall('ObtenerArchivosVehiculo', { vehiculoId: vehiculoId }, function(r) {
            if (r.success && r.data && r.data.length > 0) {
                var primerArchivo = r.data[0];
                window.open('/Views/Consultas/VisorArchivo.aspx?tipo=vehiculo&id=' + vehiculoId + '&archivoId=' + primerArchivo.Id, '_blank', 'width=1024,height=768');
            } else if (tarjetaArchivosBase64 && tarjetaArchivosBase64.length > 0) {
                // Si hay archivos cargados pero no guardados, mostrar el primero
                var archivo = tarjetaArchivosBase64[0];
                var ventana = window.open('', '_blank', 'width=1024,height=768');
                var esPDF = archivo.base64Completo.indexOf('data:application/pdf') === 0;
                if (esPDF) {
                    ventana.document.write('<iframe src="' + archivo.base64Completo + '" style="width:100%;height:100%;border:none;"></iframe>');
                } else {
                    ventana.document.write('<img src="' + archivo.base64Completo + '" style="max-width:100%;max-height:100%;" />');
                }
            } else {
                showToast('warning', 'No hay archivos de tarjeta de circulación para visualizar');
            }
        });
    } else if (tarjetaArchivosBase64 && tarjetaArchivosBase64.length > 0) {
        // Si hay archivos cargados pero no guardados, mostrar el primero
        var archivo = tarjetaArchivosBase64[0];
        var ventana = window.open('', '_blank', 'width=1024,height=768');
        var esPDF = archivo.base64Completo.indexOf('data:application/pdf') === 0;
        if (esPDF) {
            ventana.document.write('<iframe src="' + archivo.base64Completo + '" style="width:100%;height:100%;border:none;"></iframe>');
        } else {
            ventana.document.write('<img src="' + archivo.base64Completo + '" style="max-width:100%;max-height:100%;" />');
        }
    } else {
        showToast('warning', 'No hay imagen de tarjeta para visualizar');
    }
}

function escanearTarjetaCirculacion() {
    // Obtener el primer archivo del array si existe
    var archivoObj = null;
    if (tarjetaArchivosBase64 && tarjetaArchivosBase64.length > 0) {
        archivoObj = tarjetaArchivosBase64[0];
    }
    
    if (!archivoObj || !archivoObj.archivoOriginal) {
        mostrarIndicadorProcesamientoTarjeta(false);
        showToast('warning', 'Primero cargue una imagen de tarjeta de circulación');
        return;
    }

    // Mostrar estado de procesamiento
    mostrarIndicadorProcesamientoTarjeta(true);
    actualizarEstadoTarjeta('Procesando imagen con Azure Document Intelligence...', 'processing');

    // Crear FormData con el archivo
    var formData = new FormData();
    formData.append('archivo', archivoObj.archivoOriginal);
    formData.append('tipoDocumento', 'TARJETA_CIRCULACION');

    // Enviar archivo al proxy (el proxy maneja la autenticación JWT)
    fetch('/Services/DocumentIntelligenceProxy.ashx', {
        method: 'POST',
        body: formData,
        signal: new AbortController().signal
    })
    .then(function(response) {
        if (!response.ok) {
            return response.json().then(function(err) {
                throw new Error(err.Mensaje || 'Error HTTP: ' + response.status);
            });
        }
        return response.json();
    })
    .then(function(response) {
        mostrarIndicadorProcesamientoTarjeta(false);
        // La respuesta viene en formato List<CrudDto>
        if (response && response.length > 0 && response[0].Campos) {
            var datos = response[0].Campos;
            llenarCamposDesdeTarjeta(datos);
            actualizarEstadoTarjeta('✓ Datos extraídos correctamente de la tarjeta de circulación', 'success');
            showToast('success', 'Datos de tarjeta extraídos. Complete los datos faltantes y guarde cuando termine.');
            
            var vehiculoId = parseInt(document.getElementById('hfVehiculoId').value) || 0;
            if (tarjetaArchivosBase64.length > 0) {
                var archivo = tarjetaArchivosBase64[0];
                
                if (vehiculoId > 0) {
                    // Si el vehículo ya existe (edición), guardar el archivo inmediatamente
                    guardarArchivoVehiculoYAgregarGrid(vehiculoId, archivo);
                } else {
                    // Si es un nuevo vehículo, agregar al grid de manera unbound (en memoria)
                    agregarArchivoVehiculoUnbound(archivo);
                }
                
                // Remover el archivo del array después de procesarlo
                tarjetaArchivosBase64.shift();
                actualizarPreviewTarjeta();
            }
        } else {
            actualizarEstadoTarjeta('✗ No se pudieron extraer datos', 'error');
            showToast('error', 'No se pudieron extraer datos del documento');
        }
    })
    .catch(function(error) {
        clearTimeout(timeoutId);
        mostrarIndicadorProcesamientoTarjeta(false);
        var mensaje = 'Error al comunicarse con el servidor';
        if (error.name === 'AbortError') {
            mensaje = 'Tiempo de espera agotado. Intente con una imagen más pequeña.';
        } else if (error.message) {
            mensaje = error.message;
        }
        actualizarEstadoTarjeta('✗ ' + mensaje, 'error');
        showToast('error', mensaje);
    });
}

/**
 * Agrega un vehículo al grid de manera unbound (sin guardar en BD)
 * Los datos se mantienen en memoria hasta que el usuario haga clic en Guardar
 */
function agregarVehiculoAlGridUnbound(datos) {
    if (!datos || !gridVehiculos) return;
    
    try {
        // Obtener datos del formulario actual y combinarlos con los datos escaneados
        var datosVehiculo = {
            Id: 0, // ID temporal 0 para indicar que no está guardado
            ResidenteId: (typeof cboVehResidente !== 'undefined' && cboVehResidente) ? cboVehResidente.GetValue() : null,
            Placas: datos.placas || '',
            TipoVehiculo: (typeof cboVehTipo !== 'undefined' && cboVehTipo) ? cboVehTipo.GetValue() : 'Automóvil',
            Marca: datos.marca || '',
            Modelo: datos.modelo || '',
            Anio: datos.anio ? parseInt(datos.anio) : null,
            Color: datos.color || '',
            NumeroMotor: datos.numeroMotor || '',
            NumeroSerie: datos.numeroSerie || '',
            TipoCombustible: datos.tipoCombustible || '',
            CapacidadPasajeros: datos.capacidadPasajeros ? parseInt(datos.capacidadPasajeros) : null,
            UsoVehiculo: datos.usoVehiculo || 'Particular',
            NumeroTarjeton: datos.numeroTarjeta || '',
            FechaExpedicionTarjeta: datos.fechaExpedicion || null,
            FechaVigenciaTarjeta: datos.fechaVigencia || null,
            PropietarioRegistrado: datos.propietarioRegistrado || '',
            Observaciones: (typeof txtVehObservaciones !== 'undefined' && txtVehObservaciones) ? txtVehObservaciones.GetValue() : '',
            Activo: (typeof chkVehActivo !== 'undefined' && chkVehActivo) ? chkVehActivo.GetChecked() : true
        };
        
        // Agregar la fila usando el callback del servidor
        if (gridVehiculos) {
            // Obtener unidadId actual para pasarlo si es necesario
            var unidadId = currentUnidadId || 0;
            var hfId = document.getElementById('hfId');
            if (!unidadId && hfId && hfId.value && hfId.value !== '0') {
                unidadId = parseInt(hfId.value);
            }
            
            // Incluir unidadId en el callback
            gridVehiculos.PerformCallback('agregarUnbound|' + unidadId + '|' + JSON.stringify(datosVehiculo));
            console.log('Vehículo agregado al grid (unbound):', datosVehiculo);
        }
        
    } catch (ex) {
        console.error('Error al agregar vehículo al grid:', ex);
        showToast('error', 'Error al agregar el registro al grid: ' + ex.message);
    }
}

function llenarCamposDesdeTarjeta(datos) {
    if (!datos) return;

    // Si datos viene en formato CrudDto (con Campos.Valor), convertir a formato simple
    var datosSimples = {};
    if (datos && typeof datos === 'object' && !Array.isArray(datos)) {
        for (var key in datos) {
            if (datos.hasOwnProperty(key)) {
                var campo = datos[key];
                if (campo && typeof campo === 'object' && campo.Valor !== undefined) {
                    datosSimples[key] = campo.Valor;
                } else {
                    datosSimples[key] = campo;
                }
            }
        }
    } else {
        datosSimples = datos;
    }
    
    datos = datosSimples;
    if (!datos) return;

    // Mapear datos a los campos del formulario de vehículo
    if (datos.placas && typeof txtVehPlacas !== 'undefined') {
        txtVehPlacas.SetValue(datos.placas);
    }
    if (datos.numeroTarjeta && typeof txtVehTarjeton !== 'undefined') {
        txtVehTarjeton.SetValue(datos.numeroTarjeta);
    }
    if (datos.marca && typeof txtVehMarca !== 'undefined') {
        txtVehMarca.SetValue(datos.marca);
    }
    if (datos.modelo && typeof txtVehModelo !== 'undefined') {
        txtVehModelo.SetValue(datos.modelo);
    }
    if (datos.anio && typeof txtVehAnio !== 'undefined') {
        var anio = parseInt(datos.anio);
        if (!isNaN(anio)) {
            txtVehAnio.SetNumber(anio);
        }
    }
    if (datos.color && typeof txtVehColor !== 'undefined') {
        txtVehColor.SetValue(datos.color);
    }
    if (datos.numeroMotor && typeof txtVehNumeroMotor !== 'undefined') {
        txtVehNumeroMotor.SetValue(datos.numeroMotor);
    }
    if (datos.numeroSerie && typeof txtVehNumeroSerie !== 'undefined') {
        txtVehNumeroSerie.SetValue(datos.numeroSerie);
    }
    if (datos.tipoCombustible && typeof cboVehTipoCombustible !== 'undefined') {
        cboVehTipoCombustible.SetValue(datos.tipoCombustible);
    }
    if (datos.capacidadPasajeros && typeof txtVehCapacidadPasajeros !== 'undefined') {
        var capacidad = parseInt(datos.capacidadPasajeros);
        if (!isNaN(capacidad)) {
            txtVehCapacidadPasajeros.SetNumber(capacidad);
        }
    }
    if (datos.usoVehiculo && typeof cboVehUso !== 'undefined') {
        cboVehUso.SetValue(datos.usoVehiculo);
    }
    if (datos.propietarioRegistrado && typeof txtVehPropietarioRegistrado !== 'undefined') {
        txtVehPropietarioRegistrado.SetValue(datos.propietarioRegistrado);
    }
    // Fechas
    if (datos.fechaExpedicion && typeof dteVehFechaExpedicion !== 'undefined') {
        var fechaExp = parseFecha(datos.fechaExpedicion);
        if (fechaExp) {
            dteVehFechaExpedicion.SetDate(fechaExp);
        }
    }
    if (datos.fechaVigencia && typeof dteVehFechaVigencia !== 'undefined') {
        var fechaVig = parseFecha(datos.fechaVigencia);
        if (fechaVig) {
            dteVehFechaVigencia.SetDate(fechaVig);
        }
    }
}

function parseFecha(fechaStr) {
    if (!fechaStr) return null;
    try {
        // Intentar parsear diferentes formatos de fecha (dd/MM/yyyy, dd-MM-yyyy, yyyy-MM-dd)
        var partes = fechaStr.split(/[\/\-]/);
        if (partes.length === 3) {
            var dia, mes, anio;
            // Si el año tiene 4 dígitos y está al final, formato dd/MM/yyyy
            if (partes[2].length === 4) {
                dia = parseInt(partes[0]);
                mes = parseInt(partes[1]) - 1; // Los meses en JS son 0-indexed
                anio = parseInt(partes[2]);
            } else if (partes[0].length === 4) {
                // Formato yyyy-MM-dd
                anio = parseInt(partes[0]);
                mes = parseInt(partes[1]) - 1;
                dia = parseInt(partes[2]);
            } else {
                return null;
            }
            var fecha = new Date(anio, mes, dia);
            if (!isNaN(fecha.getTime())) {
                return fecha;
            }
        }
        return null;
    } catch (e) {
        return null;
    }
}

/**
 * Muestra u oculta el indicador de procesamiento para Tarjeta
 */
function mostrarIndicadorProcesamientoTarjeta(mostrar) {
    var indicator = document.getElementById('tarjetaProcessingIndicator');
    if (indicator) {
        indicator.style.display = mostrar ? 'block' : 'none';
    }
}

function actualizarEstadoTarjeta(mensaje, tipo) {
    var statusDiv = document.getElementById('tarjetaStatus');
    if (!statusDiv) return;

    if (!mensaje || mensaje === '') {
        statusDiv.style.display = 'none';
        return;
    }

    statusDiv.style.display = 'block';
    statusDiv.textContent = mensaje;

    // Aplicar estilos según el tipo
    statusDiv.className = '';
    switch (tipo) {
        case 'success':
            statusDiv.style.backgroundColor = '#d4edda';
            statusDiv.style.color = '#155724';
            statusDiv.style.border = '1px solid #c3e6cb';
            break;
        case 'error':
            statusDiv.style.backgroundColor = '#f8d7da';
            statusDiv.style.color = '#721c24';
            statusDiv.style.border = '1px solid #f5c6cb';
            break;
        case 'processing':
            statusDiv.style.backgroundColor = '#d1ecf1';
            statusDiv.style.color = '#0c5460';
            statusDiv.style.border = '1px solid #bee5eb';
            break;
        case 'info':
        default:
            statusDiv.style.backgroundColor = '#d1ecf1';
            statusDiv.style.color = '#0c5460';
            statusDiv.style.border = '1px solid #bee5eb';
            break;
    }
}
// ============================================================================
// FUNCIONES PARA MÚLTIPLES ARCHIVOS - RESIDENTES
// ============================================================================

// Función para manejar selección múltiple de archivos INE
function onINEFileInputChange(e) {
    var files = e.target.files;
    if (!files || files.length === 0) return;
    
    var archivosValidos = [];
    
    // Filtrar archivos válidos
    for (var i = 0; i < files.length; i++) {
        var file = files[i];
        if (file.size > 10 * 1024 * 1024) { // 10MB
            showToast('warning', 'El archivo ' + file.name + ' excede 10MB');
            continue;
        }
        archivosValidos.push(file);
    }
    
    if (archivosValidos.length === 0) return;
    
    // Resetear la bandera de procesamiento
    ineProcesamientoDisparado = false;
    
    // Procesar cada archivo válido
    for (var j = 0; j < archivosValidos.length; j++) {
        var file = archivosValidos[j];
        var esPrimerArchivo = (j === 0);
        
        var reader = new FileReader();
        reader.onload = function(file, esPrimero) {
            return function(e) {
                var base64Completo = e.target.result;
                var base64SinPrefijo = base64Completo;
                if (base64SinPrefijo.indexOf(',') > -1) {
                    base64SinPrefijo = base64SinPrefijo.split(',')[1];
                }
                
                ineArchivosBase64.push({
                    nombre: file.name,
                    base64: base64SinPrefijo,
                    base64Completo: base64Completo,
                    tipoMime: file.type || 'application/octet-stream',
                    tamanio: file.size,
                    archivoOriginal: file // Guardar el archivo original para enviarlo directamente
                });
                
                actualizarPreviewINE();
                
                // Cuando se completa la carga del primer archivo nuevo, procesar automáticamente
                if (esPrimero && !ineProcesamientoDisparado) {
                    ineProcesamientoDisparado = true;
                    console.log('Primer archivo INE cargado, iniciando procesamiento automático...');
                    // Mostrar indicador de procesamiento
                    mostrarIndicadorProcesamientoINE(true);
                    // Esperar un momento para que se actualice el preview, luego escanear
                    setTimeout(function() {
                        escanearINE();
                    }, 500);
                }
            };
        }(file, esPrimerArchivo);
        reader.readAsDataURL(file);
    }
}

// Función para actualizar preview de archivos INE
function actualizarPreviewINE() {
    var container = document.getElementById('ineFilesPreview');
    if (!container) return;
    
    container.innerHTML = '';
    
    ineArchivosBase64.forEach(function(archivo, index) {
        var div = document.createElement('div');
        div.style.cssText = 'position: relative; display: inline-block; margin: 5px;';
        
        if (archivo.base64Completo.indexOf('data:image/') === 0) {
            var img = document.createElement('img');
            img.src = archivo.base64Completo;
            img.style.cssText = 'max-width: 150px; max-height: 150px; border: 1px solid #ccc; border-radius: 4px;';
            div.appendChild(img);
        } else {
            var span = document.createElement('span');
            span.textContent = archivo.nombre;
            span.style.cssText = 'padding: 10px; border: 1px solid #ccc; border-radius: 4px; display: block;';
            div.appendChild(span);
        }
        
        var btn = document.createElement('button');
        btn.textContent = '×';
        btn.style.cssText = 'position: absolute; top: 5px; right: 5px; background: red; color: white; border: none; border-radius: 50%; width: 25px; height: 25px; cursor: pointer;';
        btn.onclick = function(idx) {
            return function() {
                ineArchivosBase64.splice(idx, 1);
                actualizarPreviewINE();
            };
        }(index);
        div.appendChild(btn);
        
        container.appendChild(div);
    });
    
    var filesList = document.getElementById('ineFilesList');
    if (ineArchivosBase64.length > 0) {
        filesList.style.display = 'block';
    } else {
        filesList.style.display = 'none';
    }
}

// Función para cargar archivos existentes de residente
function cargarArchivosResidente(residenteId) {
    // Permitir cargar incluso si residenteId es 0 para mostrar archivos unbound
    if (gridArchivosResidente) {
        gridArchivosResidente.PerformCallback('cargar|' + (residenteId || 0));
    }
}

// Handler para toolbar de archivos de residente
// Función para manejar clicks en el toolbar de archivos de residente
window.onToolbarArchivosResidenteClick = function onToolbarArchivosResidenteClick(s, e) {
    try {
        var idx = gridArchivosResidente.GetFocusedRowIndex();
        if (idx < 0) {
            showToast('warning', 'Seleccione un archivo');
            return;
        }
        var archivoId = gridArchivosResidente.GetRowKey(idx);
        
        switch (e.item.name) {
            case 'btnVerArchivoResidente':
                verArchivoResidente(null, archivoId);
                break;
            case 'btnEliminarArchivoResidente':
                eliminarArchivoResidente(null, archivoId);
                break;
        }
    } catch (error) {
        console.error('Error en onToolbarArchivosResidenteClick:', error);
        if (typeof showToast !== 'undefined') {
            showToast('error', 'Error al procesar la acción: ' + error.message);
        }
    }
};
var onToolbarArchivosResidenteClick = window.onToolbarArchivosResidenteClick;

// Función para ver archivo de residente
function verArchivoResidente(container, archivoId) {
    var residenteId = parseInt(document.getElementById('hfResidenteId').value) || 0;
    window.open('/Views/Consultas/VisorArchivo.aspx?tipo=residente&id=' + residenteId + '&archivoId=' + archivoId, '_blank', 'width=1024,height=768');
}

// Función para eliminar archivo de residente
function eliminarArchivoResidente(container, archivoId) {
    if (!confirm('¿Eliminar este archivo?')) return;
    
    ajaxCall('EliminarArchivoResidente', { id: archivoId }, function(r) {
        if (r.success) {
            showToast('success', r.message);
            var residenteId = parseInt(document.getElementById('hfResidenteId').value) || 0;
            cargarArchivosResidente(residenteId);
        } else {
            showToast('error', r.message);
        }
    });
}

// Función para agregar un archivo al grid de residente de manera unbound (en memoria, sin guardar en BD)
function agregarArchivoResidenteUnbound(archivo) {
    if (!archivo || !gridArchivosResidente) return;
    
    var datosArchivo = {
        Id: 0, // ID temporal 0 para indicar que no está guardado
        ResidenteId: 0, // Aún no hay residenteId
        TipoArchivo: 'INE',
        NombreArchivo: archivo.nombre,
        TamanioBytes: archivo.tamanio,
        TipoMime: archivo.tipoMime,
        FechaCreacion: new Date().toISOString(),
        Activo: 1
    };
    
    // Guardar el archivo en memoria para guardarlo después
    if (!window.archivosResidenteUnbound) {
        window.archivosResidenteUnbound = [];
    }
    window.archivosResidenteUnbound.push({
        datos: datosArchivo,
        archivoBase64: archivo.base64,
        archivoCompleto: archivo.base64Completo
    });
    
    // Agregar al grid usando callback
    gridArchivosResidente.PerformCallback('agregarUnbound|' + JSON.stringify(datosArchivo));
}

// Función para guardar un archivo de residente y agregarlo inmediatamente al grid
function guardarArchivoResidenteYAgregarGrid(residenteId, archivo) {
    if (!archivo || !residenteId || residenteId === 0) return;
    
    var datosArchivo = {
        residenteId: residenteId,
        tipoArchivo: 'INE',
        nombreArchivo: archivo.nombre,
        archivoBase64: archivo.base64,
        tipoMime: archivo.tipoMime,
        tamanioBytes: archivo.tamanio
    };
    
    ajaxCall('GuardarArchivoResidente', { datos: datosArchivo }, function(r) {
        if (r.success) {
            // Recargar el grid de archivos para mostrar el nuevo archivo
            cargarArchivosResidente(residenteId);
            showToast('success', 'Archivo guardado y agregado correctamente');
        } else {
            showToast('error', 'Error al guardar archivo: ' + (r.message || 'Error desconocido'));
        }
    });
}

// Función para guardar archivos después de guardar el residente
function guardarArchivosResidente(residenteId, archivos) {
    if (!archivos || archivos.length === 0) return;
    
    var archivosGuardados = 0;
    var archivosTotal = archivos.length;
    
    archivos.forEach(function(archivo) {
        var datosArchivo = {
            residenteId: residenteId,
            tipoArchivo: 'INE',
            nombreArchivo: archivo.nombre,
            archivoBase64: archivo.base64,
            tipoMime: archivo.tipoMime,
            tamanioBytes: archivo.tamanio
        };
        
        ajaxCall('GuardarArchivoResidente', { datos: datosArchivo }, function(r) {
            archivosGuardados++;
            if (!r.success) {
                console.error('Error al guardar archivo:', r.message);
            }
            
            if (archivosGuardados === archivosTotal) {
                cargarArchivosResidente(residenteId);
            }
        });
    });
}

// Función modificada para guardarResidenteContinuar
function guardarResidenteContinuarActualizado(unidadId) {
    currentUnidadId = unidadId;
    
    // Validar campos requeridos
    var nombre = txtResNombre.GetValue() || '';
    var apellidoPaterno = txtResApPaterno.GetValue() || '';
    var apellidoMaterno = txtResApMaterno.GetValue() || '';
    var email = txtResEmail.GetValue() || '';
    var celular = txtResCelular.GetValue() || '';
    
    if (!nombre || nombre.trim() === '') {
        showToast('warning', 'El nombre es requerido');
        txtResNombre.Focus();
        return;
    }
    
    if (!apellidoPaterno || apellidoPaterno.trim() === '') {
        showToast('warning', 'El apellido paterno es requerido');
        txtResApPaterno.Focus();
        return;
    }
    
    if (!apellidoMaterno || apellidoMaterno.trim() === '') {
        showToast('warning', 'El apellido materno es requerido');
        txtResApMaterno.Focus();
        return;
    }
    
    if (!email || email.trim() === '') {
        showToast('warning', 'El email es requerido');
        txtResEmail.Focus();
        return;
    }
    
    // Validar formato de email
    var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
        showToast('warning', 'El formato del email no es válido');
        txtResEmail.Focus();
        return;
    }
    
    if (!celular || celular.trim() === '') {
        showToast('warning', 'El celular es requerido');
        txtResCelular.Focus();
        return;
    }
    
    var datos = {
        id: parseInt(document.getElementById('hfResidenteId').value) || 0,
        unidadId: unidadId,
        tipoResidente: cboTipoResidente.GetValue(),
        esPrincipal: chkResPrincipal.GetChecked(),
        nombre: nombre.trim(),
        apellidoPaterno: apellidoPaterno.trim(),
        apellidoMaterno: apellidoMaterno.trim(),
        email: email.trim(),
        telefono: txtResTelefono.GetValue() || '',
        celular: celular.trim(),
        curp: txtResCURP.GetValue() || '',
        activo: chkResActivo.GetChecked()
    };
    
    ajaxCall('GuardarResidente', { datos: datos }, function(r) {
        if (r.success) {
            showToast('success', r.message);
            var residenteId = r.data && r.data.id ? r.data.id : datos.id;
            
            // Guardar archivos pendientes (del array y unbound)
            if (residenteId > 0) {
                // Guardar archivos del array ineArchivosBase64
                if (ineArchivosBase64.length > 0) {
                    guardarArchivosResidente(residenteId, ineArchivosBase64);
                    ineArchivosBase64 = [];
                    actualizarPreviewINE();
                }
                
                // Guardar archivos unbound (en memoria)
                if (window.archivosResidenteUnbound && window.archivosResidenteUnbound.length > 0) {
                    var archivosUnboundArray = [];
                    window.archivosResidenteUnbound.forEach(function(item) {
                        archivosUnboundArray.push({
                            nombre: item.datos.NombreArchivo,
                            base64: item.archivoBase64,
                            tipoMime: item.datos.TipoMime,
                            tamanio: item.datos.TamanioBytes
                        });
                    });
                    if (archivosUnboundArray.length > 0) {
                        guardarArchivosResidente(residenteId, archivosUnboundArray);
                    }
                    window.archivosResidenteUnbound = [];
                }
                
                // Recargar grid de archivos
                cargarArchivosResidente(residenteId);
            }
            
            popupResidente.Hide();
            if (currentUnidadId) {
                cargarResidentesGrid(currentUnidadId);
            }
        } else {
            showToast('error', r.message);
        }
    });
}

// ============================================================================
// FUNCIONES PARA MÚLTIPLES ARCHIVOS - VEHÍCULOS
// ============================================================================

// Funciones similares para vehículos
function onTarjetaFileInputChange(e) {
    var files = e.target.files;
    if (!files || files.length === 0) return;
    
    var archivosValidos = [];
    
    // Filtrar archivos válidos
    for (var i = 0; i < files.length; i++) {
        var file = files[i];
        if (file.size > 10 * 1024 * 1024) {
            showToast('warning', 'El archivo ' + file.name + ' excede 10MB');
            continue;
        }
        archivosValidos.push(file);
    }
    
    if (archivosValidos.length === 0) return;
    
    // Resetear la bandera de procesamiento
    tarjetaProcesamientoDisparado = false;
    
    // Procesar cada archivo válido
    for (var j = 0; j < archivosValidos.length; j++) {
        var file = archivosValidos[j];
        var esPrimerArchivo = (j === 0);
        
        var reader = new FileReader();
        reader.onload = function(file, esPrimero) {
            return function(e) {
                var base64Completo = e.target.result;
                var base64SinPrefijo = base64Completo;
                if (base64SinPrefijo.indexOf(',') > -1) {
                    base64SinPrefijo = base64SinPrefijo.split(',')[1];
                }
                
                tarjetaArchivosBase64.push({
                    nombre: file.name,
                    base64: base64SinPrefijo,
                    base64Completo: base64Completo,
                    tipoMime: file.type || 'application/octet-stream',
                    tamanio: file.size,
                    archivoOriginal: file // Guardar el archivo original para enviarlo directamente
                });
                
                actualizarPreviewTarjeta();
                
                // Cuando se completa la carga del primer archivo nuevo, procesar automáticamente
                if (esPrimero && !tarjetaProcesamientoDisparado) {
                    tarjetaProcesamientoDisparado = true;
                    console.log('Primer archivo de tarjeta cargado, iniciando procesamiento automático...');
                    // Mostrar indicador de procesamiento
                    mostrarIndicadorProcesamientoTarjeta(true);
                    // Esperar un momento para que se actualice el preview, luego escanear
                    setTimeout(function() {
                        escanearTarjetaCirculacion();
                    }, 500);
                }
            };
        }(file, esPrimerArchivo);
        reader.readAsDataURL(file);
    }
}

function actualizarPreviewTarjeta() {
    var container = document.getElementById('tarjetaFilesPreview');
    if (!container) return;
    
    container.innerHTML = '';
    
    tarjetaArchivosBase64.forEach(function(archivo, index) {
        var div = document.createElement('div');
        div.style.cssText = 'position: relative; display: inline-block; margin: 5px;';
        
        if (archivo.base64Completo.indexOf('data:image/') === 0) {
            var img = document.createElement('img');
            img.src = archivo.base64Completo;
            img.style.cssText = 'max-width: 150px; max-height: 150px; border: 1px solid #ccc; border-radius: 4px;';
            div.appendChild(img);
        } else {
            var span = document.createElement('span');
            span.textContent = archivo.nombre;
            span.style.cssText = 'padding: 10px; border: 1px solid #ccc; border-radius: 4px; display: block;';
            div.appendChild(span);
        }
        
        var btn = document.createElement('button');
        btn.textContent = '×';
        btn.style.cssText = 'position: absolute; top: 5px; right: 5px; background: red; color: white; border: none; border-radius: 50%; width: 25px; height: 25px; cursor: pointer;';
        btn.onclick = function(idx) {
            return function() {
                tarjetaArchivosBase64.splice(idx, 1);
                actualizarPreviewTarjeta();
            };
        }(index);
        div.appendChild(btn);
        
        container.appendChild(div);
    });
    
    var filesList = document.getElementById('tarjetaFilesList');
    if (tarjetaArchivosBase64.length > 0) {
        filesList.style.display = 'block';
    } else {
        filesList.style.display = 'none';
    }
}

function cargarArchivosVehiculo(vehiculoId) {
    // Permitir cargar incluso si vehiculoId es 0 para mostrar archivos unbound
    if (gridArchivosVehiculo) {
        gridArchivosVehiculo.PerformCallback('cargar|' + (vehiculoId || 0));
    }
}

// Handler para toolbar de archivos de vehículo
// Función para manejar clicks en el toolbar de archivos de vehículo
window.onToolbarArchivosVehiculoClick = function onToolbarArchivosVehiculoClick(s, e) {
    try {
        var idx = gridArchivosVehiculo.GetFocusedRowIndex();
        if (idx < 0) {
            showToast('warning', 'Seleccione un archivo');
            return;
        }
        var archivoId = gridArchivosVehiculo.GetRowKey(idx);
        
        switch (e.item.name) {
            case 'btnVerArchivoVehiculo':
                verArchivoVehiculo(null, archivoId);
                break;
            case 'btnEliminarArchivoVehiculo':
                eliminarArchivoVehiculo(null, archivoId);
                break;
        }
    } catch (error) {
        console.error('Error en onToolbarArchivosVehiculoClick:', error);
        if (typeof showToast !== 'undefined') {
            showToast('error', 'Error al procesar la acción: ' + error.message);
        }
    }
};
var onToolbarArchivosVehiculoClick = window.onToolbarArchivosVehiculoClick;

function verArchivoVehiculo(container, archivoId) {
    var vehiculoId = parseInt(document.getElementById('hfVehiculoId').value) || 0;
    window.open('/Views/Consultas/VisorArchivo.aspx?tipo=vehiculo&id=' + vehiculoId + '&archivoId=' + archivoId, '_blank', 'width=1024,height=768');
}

function eliminarArchivoVehiculo(container, archivoId) {
    if (!confirm('¿Eliminar este archivo?')) return;
    ajaxCall('EliminarArchivoVehiculo', { id: archivoId }, function(r) {
        if (r.success) {
            showToast('success', r.message);
            var vehiculoId = parseInt(document.getElementById('hfVehiculoId').value) || 0;
            cargarArchivosVehiculo(vehiculoId);
        } else {
            showToast('error', r.message);
        }
    });
}

// Función para agregar un archivo al grid de vehículo de manera unbound (en memoria, sin guardar en BD)
function agregarArchivoVehiculoUnbound(archivo) {
    if (!archivo || !gridArchivosVehiculo) return;
    
    var datosArchivo = {
        Id: 0, // ID temporal 0 para indicar que no está guardado
        VehiculoId: 0, // Aún no hay vehiculoId
        TipoArchivo: 'TarjetaCirculacion',
        NombreArchivo: archivo.nombre,
        TamanioBytes: archivo.tamanio,
        TipoMime: archivo.tipoMime,
        FechaCreacion: new Date().toISOString(),
        Activo: 1
    };
    
    // Guardar el archivo en memoria para guardarlo después
    if (!window.archivosVehiculoUnbound) {
        window.archivosVehiculoUnbound = [];
    }
    window.archivosVehiculoUnbound.push({
        datos: datosArchivo,
        archivoBase64: archivo.base64,
        archivoCompleto: archivo.base64Completo
    });
    
    // Agregar al grid usando callback
    gridArchivosVehiculo.PerformCallback('agregarUnbound|' + JSON.stringify(datosArchivo));
}

// Función para guardar un archivo de vehículo y agregarlo inmediatamente al grid
function guardarArchivoVehiculoYAgregarGrid(vehiculoId, archivo) {
    if (!archivo || !vehiculoId || vehiculoId === 0) return;
    
    var datosArchivo = {
        vehiculoId: vehiculoId,
        tipoArchivo: 'TarjetaCirculacion',
        nombreArchivo: archivo.nombre,
        archivoBase64: archivo.base64,
        tipoMime: archivo.tipoMime,
        tamanioBytes: archivo.tamanio
    };
    
    ajaxCall('GuardarArchivoVehiculo', { datos: datosArchivo }, function(r) {
        if (r.success) {
            // Recargar el grid de archivos para mostrar el nuevo archivo
            cargarArchivosVehiculo(vehiculoId);
            showToast('success', 'Archivo guardado y agregado correctamente');
        } else {
            showToast('error', 'Error al guardar archivo: ' + (r.message || 'Error desconocido'));
        }
    });
}

function guardarArchivosVehiculo(vehiculoId, archivos) {
    if (!archivos || archivos.length === 0) return;
    var archivosGuardados = 0;
    var archivosTotal = archivos.length;
    
    archivos.forEach(function(archivo) {
        var datosArchivo = {
            vehiculoId: vehiculoId,
            tipoArchivo: 'TarjetaCirculacion',
            nombreArchivo: archivo.nombre,
            archivoBase64: archivo.base64,
            tipoMime: archivo.tipoMime,
            tamanioBytes: archivo.tamanio
        };
        
        ajaxCall('GuardarArchivoVehiculo', { datos: datosArchivo }, function(r) {
            archivosGuardados++;
            if (!r.success) console.error('Error al guardar archivo:', r.message);
            if (archivosGuardados === archivosTotal) {
                cargarArchivosVehiculo(vehiculoId);
            }
        });
    });
}

// ============================================================================
// FUNCIONES PARA MÚLTIPLES ARCHIVOS - DOCUMENTOS
// ============================================================================

function onDocumentoFileInputChange(e) {
    var files = e.target.files;
    if (!files || files.length === 0) return;
    
    for (var i = 0; i < files.length; i++) {
        var file = files[i];
        if (file.size > 10 * 1024 * 1024) {
            showToast('warning', 'El archivo ' + file.name + ' excede 10MB');
            continue;
        }
        
        var reader = new FileReader();
        reader.onload = function(file) {
            return function(e) {
                var base64Completo = e.target.result;
                var base64SinPrefijo = base64Completo;
                if (base64SinPrefijo.indexOf(',') > -1) {
                    base64SinPrefijo = base64SinPrefijo.split(',')[1];
                }
                
                documentoArchivosBase64.push({
                    nombre: file.name,
                    base64: base64SinPrefijo,
                    base64Completo: base64Completo,
                    tipoMime: file.type || 'application/octet-stream',
                    tamanio: file.size
                });
                
                actualizarPreviewDocumento();
            };
        }(file);
        reader.readAsDataURL(file);
    }
}

function actualizarPreviewDocumento() {
    var container = document.getElementById('documentoFilesPreview');
    if (!container) return;
    
    container.innerHTML = '';
    
    documentoArchivosBase64.forEach(function(archivo, index) {
        var div = document.createElement('div');
        div.style.cssText = 'position: relative; display: inline-block; margin: 5px;';
        
        if (archivo.base64Completo.indexOf('data:image/') === 0) {
            var img = document.createElement('img');
            img.src = archivo.base64Completo;
            img.style.cssText = 'max-width: 150px; max-height: 150px; border: 1px solid #ccc; border-radius: 4px;';
            div.appendChild(img);
        } else {
            var span = document.createElement('span');
            span.textContent = archivo.nombre;
            span.style.cssText = 'padding: 10px; border: 1px solid #ccc; border-radius: 4px; display: block;';
            div.appendChild(span);
        }
        
        var btn = document.createElement('button');
        btn.textContent = '×';
        btn.style.cssText = 'position: absolute; top: 5px; right: 5px; background: red; color: white; border: none; border-radius: 50%; width: 25px; height: 25px; cursor: pointer;';
        btn.onclick = function(idx) {
            return function() {
                documentoArchivosBase64.splice(idx, 1);
                actualizarPreviewDocumento();
            };
        }(index);
        div.appendChild(btn);
        
        container.appendChild(div);
    });
    
    var filesList = document.getElementById('documentoFilesList');
    if (documentoArchivosBase64.length > 0) {
        filesList.style.display = 'block';
    } else {
        filesList.style.display = 'none';
    }
}

function cargarArchivosDocumento(documentoId) {
    if (!documentoId || documentoId === 0) return;
    if (gridArchivosDocumento) {
        gridArchivosDocumento.PerformCallback('cargar|' + documentoId);
    }
}

// Handler para toolbar de archivos de documento
// Función para manejar clicks en el toolbar de archivos de documento
window.onToolbarArchivosDocumentoClick = function onToolbarArchivosDocumentoClick(s, e) {
    try {
        var idx = gridArchivosDocumento.GetFocusedRowIndex();
        if (idx < 0) {
            showToast('warning', 'Seleccione un archivo');
            return;
        }
        var archivoId = gridArchivosDocumento.GetRowKey(idx);
        
        switch (e.item.name) {
            case 'btnVerArchivoDocumento':
                verArchivoDocumento(null, archivoId);
                break;
            case 'btnEliminarArchivoDocumento':
                eliminarArchivoDocumento(null, archivoId);
                break;
        }
    } catch (error) {
        console.error('Error en onToolbarArchivosDocumentoClick:', error);
        if (typeof showToast !== 'undefined') {
            showToast('error', 'Error al procesar la acción: ' + error.message);
        }
    }
};
var onToolbarArchivosDocumentoClick = window.onToolbarArchivosDocumentoClick;

function verArchivoDocumento(container, archivoId) {
    var documentoId = parseInt(document.getElementById('hfDocumentoId').value) || 0;
    window.open('/Views/Consultas/VisorArchivo.aspx?tipo=documento&id=' + documentoId + '&archivoId=' + archivoId, '_blank', 'width=1024,height=768');
}

function eliminarArchivoDocumento(container, archivoId) {
    if (!confirm('¿Eliminar este archivo?')) return;
    ajaxCall('EliminarArchivoDocumento', { id: archivoId }, function(r) {
        if (r.success) {
            showToast('success', r.message);
            var documentoId = parseInt(document.getElementById('hfDocumentoId').value) || 0;
            cargarArchivosDocumento(documentoId);
        } else {
            showToast('error', r.message);
        }
    });
}

function guardarArchivosDocumento(documentoId, archivos) {
    if (!archivos || archivos.length === 0) return;
    var archivosGuardados = 0;
    var archivosTotal = archivos.length;
    
    archivos.forEach(function(archivo) {
        var datosArchivo = {
            documentoId: documentoId,
            nombreArchivo: archivo.nombre,
            archivoBase64: archivo.base64,
            tipoMime: archivo.tipoMime,
            tamanioBytes: archivo.tamanio
        };
        
        ajaxCall('GuardarArchivoDocumento', { datos: datosArchivo }, function(r) {
            archivosGuardados++;
            if (!r.success) console.error('Error al guardar archivo:', r.message);
            if (archivosGuardados === archivosTotal) {
                cargarArchivosDocumento(documentoId);
            }
        });
    });
}

// ============================================================================
// MAPA DE GOOGLE MAPS
// ============================================================================

// Inicializar el mapa (API tradicional de Google Maps, exactamente como frmOperacion.aspx)
// Esta función DEBE estar disponible globalmente para el callback de Google Maps
// Definir tanto en window.inicializarMapa como en el scope global (inicializarMapa)
window.inicializarMapa = function inicializarMapa() {
    console.log('=== INICIO inicializarMapa ===');
    
    var container = document.getElementById('mapaUnidades');
    if (!container) {
        console.error('❌ Contenedor del mapa no encontrado');
        return;
    }
    
    console.log('✅ Contenedor encontrado:', container);
    console.log('📏 Dimensiones del contenedor:', {
        width: container.offsetWidth,
        height: container.offsetHeight,
        clientWidth: container.clientWidth,
        clientHeight: container.clientHeight
    });
    
    // Coordenadas por defecto (Zapopan, Jalisco)
    var defaultLat = 20.7131;
    var defaultLng = -103.3889;
    var defaultZoom = 13;
    
    try {
        // Usar colores por defecto de Google Maps (sin estilo personalizado)
        
        // Crear mapa usando la API tradicional con colores por defecto
        var mapOptions = {
            center: new google.maps.LatLng(defaultLat, defaultLng),
            zoom: defaultZoom,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            streetViewControl: true,
            mapTypeControl: true,
            zoomControl: true,
            fullscreenControl: true
            // Sin styles para usar colores por defecto de Google Maps
        };
        
        console.log('🗺️ Creando mapa con opciones:', mapOptions);
        mapaUnidades = new google.maps.Map(container, mapOptions);
        
        mapaInicializado = true;
        
        // CRÍTICO: Resetear lastProcessedUnidadId para permitir que se procese la primera unidad
        lastProcessedUnidadId = null;
        
        console.log('✅ Mapa de Google Maps inicializado correctamente');
        
        // Ocultar indicador de carga
        var loadingDiv = document.getElementById('mapaLoading');
        if (loadingDiv) {
            loadingDiv.style.display = 'none';
        }
        
        // Cambiar #E9EAEF por #E4EFFA en elementos del mapa después de la inicialización
        setTimeout(function() {
            try {
                var mapContainer = document.getElementById('mapaUnidades');
                if (mapContainer) {
                    // Buscar y reemplazar estilos inline con #E9EAEF
                    var allElements = mapContainer.querySelectorAll('*');
                    allElements.forEach(function(el) {
                        if (el.style && el.style.backgroundColor) {
                            var bgColor = el.style.backgroundColor;
                            if (bgColor && (bgColor.toLowerCase().includes('#e9eaef') || bgColor.toLowerCase().includes('rgb(233, 234, 239)'))) {
                                el.style.backgroundColor = '#E4EFFA';
                            }
                        }
                    });
                    
                    // Observar cambios en el DOM para aplicar el cambio dinámicamente
                    var observer = new MutationObserver(function(mutations) {
                        mutations.forEach(function(mutation) {
                            mutation.addedNodes.forEach(function(node) {
                                if (node.nodeType === 1 && node.style) { // Element node
                                    if (node.style.backgroundColor && 
                                        (node.style.backgroundColor.toLowerCase().includes('#e9eaef') || 
                                         node.style.backgroundColor.toLowerCase().includes('rgb(233, 234, 239)'))) {
                                        node.style.backgroundColor = '#E4EFFA';
                                    }
                                }
                            });
                        });
                    });
                    
                    observer.observe(mapContainer, {
                        childList: true,
                        subtree: true,
                        attributes: true,
                        attributeFilter: ['style']
                    });
                }
            } catch (ex) {
                console.warn('⚠️ Error al cambiar color #E9EAEF:', ex);
            }
        }, 500);
        
        return; // Salir temprano - el código de estilo antiguo está más abajo pero no se ejecutará
        
        // Código de estilo antiguo (no se ejecuta debido al return anterior):
        /*
            // Estilizar elementos de agua - azul pastel suave
            {
                featureType: "water",
                elementType: "geometry",
                stylers: [
                    { color: "#b8d4e8" }, // Azul pastel suave para agua
                    { lightness: 0.7 },
                    { saturation: 0.3 }
                ]
            },
            {
                featureType: "water",
                elementType: "labels.text.fill",
                stylers: [
                    { color: "#2c3e50" }, // Texto oscuro
                    { visibility: "on" }
                ]
            },
            {
                featureType: "water",
                elementType: "labels.text.stroke",
                stylers: [
                    { color: "#ffffff" },
                    { weight: 1.5 },
                    { visibility: "on" }
                ]
            },
            // Estilizar elementos de paisaje/terreno - azul-gris pastel claro
            // Solo aplicar a áreas urbanas, no a áreas naturales (verdes)
            {
                featureType: "landscape",
                elementType: "geometry",
                stylers: [
                    { color: "#e8f0f5" }, // Light pastel blue-grey para tierra urbana
                    { lightness: 0.8 },
                    { saturation: -0.2 }
                ]
            },
            {
                featureType: "landscape",
                elementType: "geometry.fill",
                stylers: [
                    { color: "#e8f0f5" },
                    { lightness: 0.9 }
                ]
            },
            {
                featureType: "landscape",
                elementType: "labels.text.fill",
                stylers: [
                    { color: "#2c3e50" } // Texto oscuro
                ]
            },
            // Mantener verde por defecto en áreas naturales y campos
            {
                featureType: "landscape.natural",
                elementType: "geometry",
                stylers: [
                    { visibility: "on" }
                    // No aplicar color para mantener el verde por defecto
                ]
            },
            {
                featureType: "landscape.natural",
                elementType: "geometry.fill",
                stylers: [
                    { visibility: "on" }
                    // No aplicar color para mantener el verde por defecto
                ]
            },
            // Estilizar carreteras menores - gris claro/casi blanco
            {
                featureType: "road",
                elementType: "geometry",
                stylers: [
                    { color: "#f5f5f0" }, // Light grey / almost white
                    { lightness: 1.0 }
                ]
            },
            {
                featureType: "road",
                elementType: "geometry.stroke",
                stylers: [
                    { color: "#d0d0d0" }, // Gris claro para bordes
                    { weight: 0.5 }
                ]
            },
            // Estilizar carreteras principales - amarillo mostaza/ocre
            {
                featureType: "road.highway",
                elementType: "geometry",
                stylers: [
                    { color: "#d4a574" }, // Mustard yellow / yellow-ochre para carreteras principales
                    { saturation: 0.4 },
                    { lightness: 0.7 }
                ]
            },
            {
                featureType: "road.highway",
                elementType: "geometry.stroke",
                stylers: [
                    { color: "#b8945f" }, // Amarillo ocre más oscuro para bordes
                    { weight: 1.5 }
                ]
            },
            {
                featureType: "road.arterial",
                elementType: "geometry",
                stylers: [
                    { color: "#e0b888" }, // Amarillo ocre más claro para arteriales
                    { saturation: 0.3 },
                    { lightness: 0.75 }
                ]
            },
            {
                featureType: "road.arterial",
                elementType: "geometry.stroke",
                stylers: [
                    { color: "#c9a574" },
                    { weight: 1 }
                ]
            },
            // Etiquetas de carreteras
            {
                featureType: "road",
                elementType: "labels.text.fill",
                stylers: [
                    { color: "#2c3e50" } // Texto oscuro
                ]
            },
            {
                featureType: "road",
                elementType: "labels.text.stroke",
                stylers: [
                    { color: "#ffffff" },
                    { weight: 2 }
                ]
            },
            // Estilizar áreas administrativas - gris claro
            {
                featureType: "administrative",
                elementType: "geometry",
                stylers: [
                    { color: "#f5f5f0" }, // Light grey / almost white
                    { lightness: 0.95 }
                ]
            },
            {
                featureType: "administrative",
                elementType: "geometry.stroke",
                stylers: [
                    { color: "#d0d0d0" }, // Gris claro para bordes
                    { weight: 0.5 }
                ]
            },
            {
                featureType: "administrative",
                elementType: "labels.text.fill",
                stylers: [
                    { color: "#2c3e50" } // Texto oscuro
                ]
            },
            {
                featureType: "administrative",
                elementType: "labels.text.stroke",
                stylers: [
                    { color: "#ffffff" },
                    { weight: 1.5 }
                ]
            },
            // Estilizar puntos de interés (POI) - simplificado
            {
                featureType: "poi",
                elementType: "geometry",
                stylers: [
                    { color: "#f5f5f0" }, // Light grey
                    { visibility: "simplified" }
                ]
            },
            {
                featureType: "poi",
                elementType: "labels.text.fill",
                stylers: [
                    { color: "#2c3e50" } // Texto oscuro
                ]
            },
            {
                featureType: "poi",
                elementType: "labels.icon",
                stylers: [
                    { visibility: "off" } // Ocultar iconos de POI para un mapa más limpio
                ]
            },
            // Mantener verde por defecto en parques y áreas verdes
            {
                featureType: "poi.park",
                elementType: "geometry",
                stylers: [
                    { visibility: "on" }
                    // No aplicar color para mantener el verde por defecto
                ]
            },
            {
                featureType: "poi.park",
                elementType: "geometry.fill",
                stylers: [
                    { visibility: "on" }
                    // No aplicar color para mantener el verde por defecto
                ]
            },
            {
                featureType: "poi.park",
                elementType: "labels.text.fill",
                stylers: [
                    { color: "#2c3e50" } // Texto oscuro
                ]
            },
            // Estilizar transit (transporte público)
            {
                featureType: "transit",
                elementType: "geometry",
                stylers: [
                    { color: "#e8f0f5" } // Azul-gris pastel
                ]
            },
            {
                featureType: "transit",
                elementType: "labels.text.fill",
                stylers: [
                    { color: "#2c3e50" } // Texto oscuro
                ]
            },
            {
                featureType: "transit.station",
                elementType: "geometry",
                stylers: [
                    { color: "#d0d0d0" },
                    { visibility: "simplified" }
                ]
            }
        ];
*/
        
    } catch (error) {
        console.error('❌ Error al inicializar el mapa de Google Maps:', error);
        mapaInicializado = false;
        
        var loadingDiv = document.getElementById('mapaLoading');
        if (loadingDiv) {
            loadingDiv.innerHTML = '<span style="color: red;">Error al cargar el mapa: ' + error.message + '</span>';
        }
    }
};

// Asegurar que inicializarMapa esté disponible globalmente (sin window.)
// para que Google Maps pueda encontrarla como callback
// Esto sobrescribe el stub inicial con la función real
// CRÍTICO: Debe estar en el scope global, no dentro de ninguna función
if (typeof window.inicializarMapa !== 'undefined') {
    inicializarMapa = window.inicializarMapa;
    console.log('✅ inicializarMapa asignada globalmente desde window.inicializarMapa');
}

// Actualizar el mapa con las coordenadas de una unidad (API tradicional)
function actualizarMapaUnidad(unidadId, latitud, longitud, codigo, nombre, numeroUnidad, edificio, propietarioPrincipal) {
    try {
        // Verificar que el mapa esté inicializado y Google Maps esté disponible
        if (!mapaInicializado || !mapaUnidades || typeof google === 'undefined' || !google.maps) {
            console.warn('Mapa no inicializado o Google Maps no disponible');
            return;
        }
    
        // Validar coordenadas
        if (!latitud || !longitud || 
            isNaN(parseFloat(latitud)) || isNaN(parseFloat(longitud)) ||
            parseFloat(latitud) === 0 || parseFloat(longitud) === 0) {
            // Si no hay coordenadas válidas, centrar en ubicación por defecto
            console.log('No hay coordenadas válidas para esta unidad, mostrando mapa por defecto');
            var defaultLocation = new google.maps.LatLng(20.7131, -103.3889);
            mapaUnidades.setCenter(defaultLocation);
            mapaUnidades.setZoom(13);
            return;
        }
        
        var lat = parseFloat(latitud);
        var lng = parseFloat(longitud);
        
        // Validar rango de coordenadas
        if (lat < -90 || lat > 90 || lng < -180 || lng > 180) {
            console.warn('Coordenadas fuera de rango válido, mostrando mapa por defecto');
            var defaultLocation = new google.maps.LatLng(20.7131, -103.3889);
            mapaUnidades.setCenter(defaultLocation);
            mapaUnidades.setZoom(13);
            return;
        }
        
        var ubicacion = new google.maps.LatLng(lat, lng);
        
        // Eliminar marcador anterior si existe
        if (marcadorUnidad) {
            marcadorUnidad.setMap(null);
        }
        
        // Crear texto del marcador
        var titulo = 'Unidad ' + (numeroUnidad || '');
        if (edificio) {
            titulo += ' - ' + edificio;
        }
        
        // Crear nuevo marcador con color azul vibrante como en la imagen de referencia
        // Usar azul vibrante (#4285f4 o #2196f3) para el marcador, similar a la imagen
        marcadorUnidad = new google.maps.Marker({
            position: ubicacion,
            map: mapaUnidades,
            title: titulo,
            animation: google.maps.Animation.DROP,
            icon: {
                path: google.maps.SymbolPath.CIRCLE,
                scale: 12,
                fillColor: '#4285f4', // Vibrant blue como en la imagen
                fillOpacity: 1,
                strokeColor: '#ffffff',
                strokeWeight: 3
            }
        });
        
        // Crear contenido mejorado para la ventana de información
        var infoContent = '<div style="padding: 15px; min-width: 320px; font-family: Arial, sans-serif;">';
        infoContent += '<h3 style="margin: 0 0 12px 0; color: #2c3e50; font-size: 17px; border-bottom: 2px solid #3498db; padding-bottom: 8px;">';
        infoContent += (codigo || 'Sin código') + '</h3>';
        
        if (nombre) {
            infoContent += '<p style="margin: 8px 0; color: #34495e; font-size: 14px; line-height: 1.4;"><strong>Nombre:</strong> ' + nombre + '</p>';
        }
        
        infoContent += '<p style="margin: 8px 0; color: #34495e; font-size: 14px; line-height: 1.4;"><strong>Unidad:</strong> ' + (numeroUnidad || 'N/A');
        if (edificio) {
            infoContent += ' - ' + edificio;
        }
        infoContent += '</p>';
        
        if (propietarioPrincipal && propietarioPrincipal.trim() !== '') {
            infoContent += '<p style="margin: 8px 0; color: #34495e; font-size: 14px; line-height: 1.4;"><strong>Propietario:</strong> ' + propietarioPrincipal + '</p>';
        }
        
        // Agregar botón para abrir el popup de detalle
        if (unidadId) {
            infoContent += '<div style="margin-top: 15px; text-align: center;">';
            infoContent += '<button onclick="event.stopPropagation(); window.editarUnidadDesdeMapa(' + unidadId + '); return false;" ';
            infoContent += 'style="background-color: #3498db; color: white; border: none; padding: 10px 20px; ';
            infoContent += 'border-radius: 5px; cursor: pointer; font-size: 14px; font-weight: bold; transition: background-color 0.3s; box-shadow: 0 2px 4px rgba(0,0,0,0.2);"';
            infoContent += 'onmouseover="this.style.backgroundColor=\'#2980b9\'" onmouseout="this.style.backgroundColor=\'#3498db\'">';
            infoContent += 'ℹ️ Ver detalle</button>';
            infoContent += '</div>';
        }
        
        infoContent += '</div>';
        
        // Crear ventana de información con configuración para mostrar todo el contenido
        var infoWindow = new google.maps.InfoWindow({
            content: infoContent,
            maxWidth: 400,
            pixelOffset: new google.maps.Size(0, 0)
        });
        
        // Guardar referencia al InfoWindow en el marcador para poder cerrarlo después
        marcadorUnidad.infoWindow = infoWindow;
        
        // Mostrar ventana de información al hacer clic en el marcador
        marcadorUnidad.addListener('click', function() {
            infoWindow.open(mapaUnidades, marcadorUnidad);
        });
        
        // Centrar y ajustar zoom del mapa
        mapaUnidades.setCenter(ubicacion);
        mapaUnidades.setZoom(17);
        
        // Mostrar ventana de información automáticamente
        infoWindow.open(mapaUnidades, marcadorUnidad);
        
        console.log('✅ Marcador actualizado en el mapa:', titulo);
    } catch (error) {
        console.error('Error en actualizarMapaUnidad:', error);
        // Si hay error al actualizar el mapa, centrarlo en ubicación por defecto
        if (mapaUnidades && typeof google !== 'undefined' && google.maps) {
            try {
                var defaultLocation = new google.maps.LatLng(20.7131, -103.3889);
                mapaUnidades.setCenter(defaultLocation);
                mapaUnidades.setZoom(13);
            } catch (mapError) {
                console.error('Error al centrar mapa por defecto:', mapError);
            }
        }
    }
}

// Manejar redimensionamiento del splitter
if (typeof splitterMain !== 'undefined') {
    splitterMain.AddPaneResizedHandler(function() {
        console.log('🔄 Splitter redimensionado');
        if (mapaUnidades && typeof google !== 'undefined' && google.maps) {
            setTimeout(function() {
                google.maps.event.trigger(mapaUnidades, 'resize');
                console.log('🗺️ Mapa redimensionado');
            }, 100);
        }
    });
}

// Función para forzar redimensionamiento del mapa cuando sea necesario
function forzarRedimensionamientoMapa() {
    if (mapaUnidades && typeof google !== 'undefined' && google.maps) {
        setTimeout(function() {
            google.maps.event.trigger(mapaUnidades, 'resize');
            console.log('🗺️ Mapa redimensionado forzadamente');
        }, 500);
    }
}

// Función para verificar y mostrar el mapa por defecto si no hay registros
function verificarYMostrarMapaPorDefecto() {
    console.log('🔍 verificarYMostrarMapaPorDefecto ejecutándose...');
    console.log('🔍 Estado actual - marcadorUnidad:', marcadorUnidad ? 'existe' : 'null');
    console.log('� Estado actual - lastProcessedUnidadId:', lastProcessedUnidadId);
    
    // Verificar si el grid tiene registros
    if (typeof gridUnidades !== 'undefined' && gridUnidades) {
        var rowCount = gridUnidades.GetVisibleRowsOnPage();
        console.log('� Registros visibles en el grid:', rowCount);
        
        // Si ya hay un marcador visible, NO hacer nada
        if (marcadorUnidad) {
            console.log('✅ Ya hay un marcador visible, no hacer nada');
            return;
        }
        
        // Si no hay registros, mostrar mapa por defecto
        if (rowCount === 0) {
            console.log('📍 No hay registros, mostrando mapa en ubicación por defecto');
            if (mapaUnidades) {
                var defaultLocation = new google.maps.LatLng(20.7131, -103.3889);
                mapaUnidades.setCenter(defaultLocation);
                mapaUnidades.setZoom(13);
                
                // Forzar redimensionamiento
                forzarRedimensionamientoMapa();
            }
        } else if (rowCount > 0) {
            // Si hay registros pero no hay marcador, intentar seleccionar la primera fila
            console.log('📍 Hay registros pero no hay marcador visible');
            var focusedIndex = gridUnidades.GetFocusedRowIndex();
            console.log('📍 Índice de fila enfocada:', focusedIndex);
            
            if (focusedIndex < 0) {
                // Si no hay fila seleccionada, seleccionar la primera
                console.log('📍 No hay fila seleccionada, seleccionando la primera...');
                gridUnidades.SetFocusedRowIndex(0);
            } else {
                // Si ya hay una fila seleccionada pero no hay marcador, forzar actualización
                console.log('📍 Hay fila seleccionada pero no hay marcador, forzando actualización...');
                // Resetear lastProcessedUnidadId para forzar actualización
                var tempId = lastProcessedUnidadId;
                lastProcessedUnidadId = null;
                // Disparar el evento manualmente
                if (typeof window.onUnidadFocusedRowChanged === 'function') {
                    window.onUnidadFocusedRowChanged(gridUnidades, {});
                }
            }
        }
    }
}

// Asegurar que la función inicializarMapa esté disponible globalmente
console.log('🔍 Verificando disponibilidad de inicializarMapa...');
if (typeof window.inicializarMapa === 'function') {
    console.log('✅ inicializarMapa está disponible globalmente');
} else {
    console.error('❌ inicializarMapa NO está disponible globalmente');
}

// Ejecutar verificación cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', function() {
    console.log('📄 DOM cargado, inicializando arrays de datos...');
    
    // Inicializar arrays de datos unbound para evitar datos residuales de sesiones anteriores
    window.datosResidentesUnbound = [];
    window.datosVehiculosUnbound = [];
    window.datosTagsUnbound = [];
    window.datosDocumentosUnbound = [];
    window.archivosResidenteUnbound = [];
    window.archivosVehiculoUnbound = [];
    
    // Resetear currentUnidadId
    currentUnidadId = 0;
    
    console.log('✅ Arrays de datos inicializados');
});

