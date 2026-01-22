// ========================================================================
// MÓDULO: Cuotas
// ========================================================================
// Gestiona la administración de cuotas de condominios
// ========================================================================

var currentCuotaId = 0;

// ========================================================================
// INICIALIZACIÓN
// ========================================================================

$(document).ready(function() {
    if (typeof gridCuotas !== 'undefined') {
        console.log('Cuotas: Grid inicializado');
    }
});

// ========================================================================
// TOOLBAR DEL GRID
// ========================================================================

function onToolbarCuotasClick(s, e) {
    console.log('onToolbarCuotasClick: Item clickeado:', e.item.name);
    switch (e.item.name) {
        case 'btnNuevaCuota':
            mostrarNuevaCuota();
            break;
        case 'btnEditarCuota':
            editarCuotaSeleccionada();
            break;
        case 'btnGenerarMasiva':
            mostrarGenerarMasiva();
            break;
        case 'btnAplicarRecargos':
            aplicarRecargosMora();
            break;
        case 'btnEliminarCuota':
            eliminarCuotaSeleccionada();
            break;
        default:
            console.log('onToolbarCuotasClick: Item no reconocido:', e.item.name);
    }
}

function onRowDblClick(s, e) {
    editarCuotaSeleccionada();
}

// ========================================================================
// FUNCIONES DE CUOTAS
// ========================================================================

function mostrarNuevaCuota() {
    limpiarFormularioCuota();
    document.getElementById('hfCuotaId').value = '0';
    currentCuotaId = 0;
    popupCuota.SetHeaderText('Nueva Cuota');
    popupCuota.Show();
}

function editarCuotaSeleccionada() {
    var idx = gridCuotas.GetFocusedRowIndex();
    if (idx < 0) {
        showToast('warning', 'Seleccione una cuota para editar');
        return;
    }
    var id = gridCuotas.GetRowKey(idx);
    cargarCuotaParaEditar(id);
}

function eliminarCuotaSeleccionada() {
    var idx = gridCuotas.GetFocusedRowIndex();
    if (idx < 0) {
        showToast('warning', 'Seleccione una cuota para eliminar');
        return;
    }
    var id = gridCuotas.GetRowKey(idx);
    
    if (confirm('¿Está seguro de eliminar esta cuota?')) {
        ajaxCall('EliminarCuota', { id: id }, function(r) {
            if (r.success) {
                showToast('success', r.message);
                gridCuotas.PerformCallback('cargar');
            } else {
                showToast('error', r.message);
            }
        });
    }
}

function cargarCuotaParaEditar(id) {
    ajaxCall('ObtenerCuota', { id: id }, function(r) {
        if (r.success && r.data) {
            currentCuotaId = id;
            document.getElementById('hfCuotaId').value = id;
            cargarDatosCuota(r.data);
            popupCuota.SetHeaderText('Editar Cuota');
            popupCuota.Show();
        } else {
            showToast('error', r.message || 'Error al cargar la cuota');
        }
    });
}

function cargarDatosCuota(d) {
    // NOTA: cboCuotaEntidad fue eliminado en el sistema multi-entidad
    // La entidad se maneja automáticamente desde la sesión
    
    if (cboCuotaUnidad) {
        cboCuotaUnidad.SetValue(d.UnidadId);
        onCuotaUnidadChanged(null, null); // Cargar residentes
    }
    if (cboConceptoCuota) {
        setTimeout(function() {
            cboConceptoCuota.SetValue(d.ConceptoCuotaId);
        }, 300);
    }
    if (cboCuotaResidente && d.ResidenteId) {
        setTimeout(function() {
            cboCuotaResidente.SetValue(d.ResidenteId);
        }, 600);
    }
    if (txtPeriodo) txtPeriodo.SetValue(d.Periodo || '');
    if (cboEstado) cboEstado.SetValue(d.Estado || 'Pendiente');
    if (txtMonto) txtMonto.SetNumber(d.Monto || 0);
    if (txtDescuento) txtDescuento.SetNumber(d.Descuento || 0);
    if (txtRecargo) txtRecargo.SetNumber(d.Recargo || 0);
    if (dtFechaEmision && d.FechaEmision) {
        var fechaEmision = parseDate(d.FechaEmision);
        if (fechaEmision) dtFechaEmision.SetDate(fechaEmision);
    }
    if (dtFechaVencimiento && d.FechaVencimiento) {
        var fechaVencimiento = parseDate(d.FechaVencimiento);
        if (fechaVencimiento) dtFechaVencimiento.SetDate(fechaVencimiento);
    }
    if (txtObservaciones) txtObservaciones.SetValue(d.Observaciones || '');
}

function limpiarFormularioCuota() {
    // NOTA: cboCuotaEntidad fue eliminado en el sistema multi-entidad
    if (cboCuotaUnidad) cboCuotaUnidad.SetValue(null);
    if (cboConceptoCuota) cboConceptoCuota.SetValue(null);
    if (cboCuotaResidente) cboCuotaResidente.SetValue(null);
    if (txtPeriodo) {
        var now = new Date();
        var periodo = now.getFullYear() + '-' + String(now.getMonth() + 1).padStart(2, '0');
        txtPeriodo.SetValue(periodo);
    }
    if (cboEstado) cboEstado.SetValue('Pendiente');
    if (txtMonto) txtMonto.SetNumber(0);
    if (txtDescuento) txtDescuento.SetNumber(0);
    if (txtRecargo) txtRecargo.SetNumber(0);
    if (dtFechaEmision) dtFechaEmision.SetDate(new Date());
    if (dtFechaVencimiento) {
        var vencimiento = new Date();
        vencimiento.setDate(vencimiento.getDate() + 30); // 30 días por defecto
        dtFechaVencimiento.SetDate(vencimiento);
    }
    if (txtObservaciones) txtObservaciones.SetValue('');
}

function guardarCuota() {
    var id = parseInt(document.getElementById('hfCuotaId').value) || 0;
    var datos = {
        id: id,
        // NOTA: entidadId se obtiene automáticamente desde la sesión en el backend
        unidadId: cboCuotaUnidad.GetValue(),
        conceptoCuotaId: cboConceptoCuota.GetValue(),
        residenteId: cboCuotaResidente.GetValue(),
        periodo: txtPeriodo.GetValue(),
        estado: cboEstado.GetValue(),
        monto: txtMonto.GetNumber(),
        descuento: txtDescuento.GetNumber(),
        recargo: txtRecargo.GetNumber(),
        fechaEmision: dtFechaEmision.GetDate(),
        fechaVencimiento: dtFechaVencimiento.GetDate(),
        observaciones: txtObservaciones.GetValue()
    };
    
    ajaxCall('GuardarCuota', { datos: datos }, function(r) {
        if (r.success) {
            showToast('success', r.message);
            popupCuota.Hide();
            gridCuotas.PerformCallback('cargar');
        } else {
            showToast('error', r.message);
        }
    });
}

// ========================================================================
// EVENTOS DE COMBOBOX
// ========================================================================

// NOTA: Esta función ya no se usa porque el sistema multi-entidad
// carga automáticamente las unidades y conceptos de la entidad actual
// Se mantiene por compatibilidad pero no hace nada
function onCuotaEntidadChanged(s, e) {
    console.log('onCuotaEntidadChanged: Función obsoleta - el sistema multi-entidad maneja esto automáticamente');
}

function onCuotaUnidadChanged(s, e) {
    var unidadId = cboCuotaUnidad.GetValue();
    if (!unidadId || unidadId === 0) {
        cboCuotaResidente.SetValue(null);
        cboCuotaResidente.SetItems([]);
        return;
    }
    
    ajaxCall('ListarResidentes', { unidadId: unidadId }, function(r) {
        if (r.success && r.data) {
            cboCuotaResidente.ClearItems();
            cboCuotaResidente.AddItem('-- Ninguno --', null);
            for (var i = 0; i < r.data.length; i++) {
                cboCuotaResidente.AddItem(r.data[i].NombreCompleto, r.data[i].Id);
            }
        }
    });
}

// NOTA: Esta función ya no se usa porque el sistema multi-entidad
// carga automáticamente los conceptos de la entidad actual
// Se mantiene por compatibilidad pero no hace nada
function onGenEntidadChanged(s, e) {
    console.log('onGenEntidadChanged: Función obsoleta - el sistema multi-entidad maneja esto automáticamente');
}

// ========================================================================
// GENERACIÓN MASIVA
// ========================================================================

function mostrarGenerarMasiva() {
    limpiarFormularioGenerarMasiva();
    popupGenerarMasiva.Show();
}

function limpiarFormularioGenerarMasiva() {
    // NOTA: cboGenEntidad fue eliminado en el sistema multi-entidad
    if (cboGenConcepto) cboGenConcepto.SetValue(null);
    if (txtGenPeriodo) {
        var now = new Date();
        var periodo = now.getFullYear() + '-' + String(now.getMonth() + 1).padStart(2, '0');
        txtGenPeriodo.SetValue(periodo);
    }
    if (dtGenVencimiento) {
        var vencimiento = new Date();
        vencimiento.setDate(vencimiento.getDate() + 30);
        dtGenVencimiento.SetDate(vencimiento);
    }
    if (txtGenMonto) txtGenMonto.SetNumber(0);
}

function generarCuotasMasivas() {
    var datos = {
        // NOTA: entidadId se obtiene automáticamente desde la sesión en el backend
        conceptoCuotaId: cboGenConcepto.GetValue(),
        periodo: txtGenPeriodo.GetValue(),
        fechaVencimiento: dtGenVencimiento.GetDate(),
        montoBase: txtGenMonto.GetNumber()
    };
    
    if (!datos.conceptoCuotaId || !datos.periodo || !datos.fechaVencimiento || !datos.montoBase) {
        showToast('warning', 'Complete todos los campos requeridos');
        return;
    }
    
    if (confirm('¿Generar cuotas masivas para todas las unidades activas de la entidad actual?')) {
        ASPxClientUtils.ShowLoadingPanel();
        ajaxCall('GenerarCuotasMasivas', { datos: datos }, function(r) {
            ASPxClientUtils.HideLoadingPanel();
            if (r.success) {
                showToast('success', r.message);
                popupGenerarMasiva.Hide();
                gridCuotas.PerformCallback('cargar');
            } else {
                showToast('error', r.message);
            }
        });
    }
}

// ========================================================================
// APLICAR RECARGOS POR MORA
// ========================================================================

function aplicarRecargosMora() {
    if (confirm('¿Aplicar recargos por mora a todas las cuotas vencidas?')) {
        ASPxClientUtils.ShowLoadingPanel();
        ajaxCall('AplicarRecargosMora', {}, function(r) {
            ASPxClientUtils.HideLoadingPanel();
            if (r.success) {
                showToast('success', r.message);
                gridCuotas.PerformCallback('cargar');
            } else {
                showToast('error', r.message);
            }
        });
    }
}

// ========================================================================
// UTILIDADES
// ========================================================================

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
