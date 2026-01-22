// ========================================================================
// MÓDULO: Estado de Cuenta
// ========================================================================
// Consulta de estado de cuenta por unidad
// ========================================================================

// Nota: showToast está definida globalmente en el Master Page
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
// INICIALIZACIÓN
// ========================================================================

$(document).ready(function() {
    if (typeof gridEstadoCuenta !== 'undefined') {
        console.log('EstadoCuenta: Grid inicializado');
    }
});

// ========================================================================
// EVENTOS DE COMBOBOX
// ========================================================================

function onFiltroEntidadChanged(s, e) {
    var entidadId = cboFiltroEntidad.GetValue();
    if (!entidadId || entidadId === 0) {
        cboFiltroUnidad.SetValue(null);
        cboFiltroUnidad.SetItems([]);
        return;
    }
    
    ajaxCall('ListarUnidades', { entidadId: entidadId }, function(r) {
        if (r.success && r.data) {
            cboFiltroUnidad.ClearItems();
            for (var i = 0; i < r.data.length; i++) {
                cboFiltroUnidad.AddItem(r.data[i].NombreCompleto, r.data[i].Id);
            }
        }
    });
}

function onFiltroUnidadChanged(s, e) {
    // Opcional: Auto-consultar cuando se seleccione una unidad
    // CargarEstadoCuenta();
}

// ========================================================================
// TOOLBAR DEL GRID
// ========================================================================

function onToolbarEstadoCuentaClick(s, e) {
    // Solo acciones de exportación y refresh
    console.log('onToolbarEstadoCuentaClick: Item clickeado:', e.item.name);
}
