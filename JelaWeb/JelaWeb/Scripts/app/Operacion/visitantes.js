// ========================================================================
// MÓDULO: Visitantes
// ========================================================================
// NOTA: Sistema multi-entidad - La entidad se obtiene automáticamente desde la sesión
// No se requiere selector de entidad en el formulario
// ========================================================================

var currentVisitanteId = 0;

// ========================================================================
// TOOLBAR DEL GRID
// ========================================================================

function onToolbarVisitantesClick(s, e) {
    switch (e.item.name) {
        case 'btnNuevoVisitante':
            mostrarNuevoVisitante();
            break;
        case 'btnEditarVisitante':
            editarVisitanteSeleccionado();
            break;
        case 'btnRegistrarSalida':
            registrarSalidaVisitante();
            break;
        case 'btnEliminarVisitante':
            eliminarVisitanteSeleccionado();
            break;
    }
}

function onRowDblClick(s, e) {
    editarVisitanteSeleccionado();
}

// ========================================================================
// FUNCIONES CRUD
// ========================================================================

function mostrarNuevoVisitante() {
    limpiarFormularioVisitante();
    document.getElementById('hfId').value = '0';
    currentVisitanteId = 0;
    if (dteFechaEntrada) dteFechaEntrada.SetDate(new Date());
    popupVisitante.SetHeaderText('Registrar Visitante');
    popupVisitante.Show();
}

function editarVisitanteSeleccionado() {
    var idx = gridVisitantes.GetFocusedRowIndex();
    if (idx < 0) {
        showToast('warning', 'Seleccione un visitante');
        return;
    }
    var id = gridVisitantes.GetRowKey(idx);
    cargarVisitante(id);
}

function eliminarVisitanteSeleccionado() {
    var idx = gridVisitantes.GetFocusedRowIndex();
    if (idx < 0) {
        showToast('warning', 'Seleccione un visitante');
        return;
    }
    if (confirm('¿Eliminar este registro de visitante?')) {
        ajaxCall('EliminarVisitante', { id: gridVisitantes.GetRowKey(idx) }, function(r) {
            if (r.success) {
                showToast('success', r.message);
                gridVisitantes.PerformCallback('cargar');
            } else {
                showToast('error', r.message);
            }
        });
    }
}

function registrarSalidaVisitante() {
    var idx = gridVisitantes.GetFocusedRowIndex();
    if (idx < 0) {
        showToast('warning', 'Seleccione un visitante');
        return;
    }
    if (confirm('¿Registrar salida de este visitante?')) {
        ajaxCall('RegistrarSalida', { id: gridVisitantes.GetRowKey(idx) }, function(r) {
            if (r.success) {
                showToast('success', r.message);
                gridVisitantes.PerformCallback('cargar');
            } else {
                showToast('error', r.message);
            }
        });
    }
}

function cargarVisitante(id) {
    ajaxCall('ObtenerVisitante', { id: id }, function(r) {
        if (r.success && r.data) {
            currentVisitanteId = id;
            document.getElementById('hfId').value = id;
            cargarDatosVisitante(r.data);
            popupVisitante.SetHeaderText('Editar Visitante');
            popupVisitante.Show();
        } else {
            showToast('error', r.message || 'Error al cargar visitante');
        }
    });
}

function cargarDatosVisitante(d) {
    // NOTA: EntidadId se maneja automáticamente desde la sesión
    if (cboUnidad) cboUnidad.SetValue(d.UnidadId);
    if (txtNombreVisitante) txtNombreVisitante.SetValue(d.NombreVisitante || '');
    if (cboTipoIdentificacion) cboTipoIdentificacion.SetValue(d.TipoIdentificacion || 'INE');
    if (txtNumeroIdentificacion) txtNumeroIdentificacion.SetValue(d.NumeroIdentificacion || '');
    if (cboTipoVisita) cboTipoVisita.SetValue(d.TipoVisita || 'Personal');
    if (txtMotivoVisita) txtMotivoVisita.SetValue(d.MotivoVisita || '');
    if (txtVehiculo) txtVehiculo.SetValue(d.Vehiculo || '');
    if (txtColorVehiculo) txtColorVehiculo.SetValue(d.ColorVehiculo || '');
    if (txtMarcaVehiculo) txtMarcaVehiculo.SetValue(d.MarcaVehiculo || '');
    if (dteFechaEntrada && d.FechaEntrada) {
        dteFechaEntrada.SetDate(new Date(d.FechaEntrada));
    }
    if (cboEstado) cboEstado.SetValue(d.Estado || 'EnCondominio');
    if (cboResidente && d.UnidadId) {
        cargarResidentes(d.UnidadId, function() {
            cboResidente.SetValue(d.ResidenteId);
        });
    }
    if (txtObservaciones) txtObservaciones.SetValue(d.Observaciones || '');
}

function limpiarFormularioVisitante() {
    // NOTA: No hay combo de entidad - se maneja automáticamente
    if (cboUnidad) cboUnidad.SetValue(null);
    if (txtNombreVisitante) txtNombreVisitante.SetValue('');
    if (cboTipoIdentificacion) cboTipoIdentificacion.SetValue('INE');
    if (txtNumeroIdentificacion) txtNumeroIdentificacion.SetValue('');
    if (cboTipoVisita) cboTipoVisita.SetValue('Personal');
    if (txtMotivoVisita) txtMotivoVisita.SetValue('');
    if (txtVehiculo) txtVehiculo.SetValue('');
    if (txtColorVehiculo) txtColorVehiculo.SetValue('');
    if (txtMarcaVehiculo) txtMarcaVehiculo.SetValue('');
    if (dteFechaEntrada) dteFechaEntrada.SetDate(new Date());
    if (cboEstado) cboEstado.SetValue('EnCondominio');
    if (cboResidente) cboResidente.ClearItems();
    if (txtObservaciones) txtObservaciones.SetValue('');
}

function guardarVisitante() {
    var datos = {
        id: parseInt(document.getElementById('hfId').value) || 0,
        // NOTA: entidadId se obtiene automáticamente desde la sesión en el backend
        unidadId: cboUnidad.GetValue(),
        residenteId: cboResidente.GetValue(),
        nombreVisitante: txtNombreVisitante.GetValue(),
        tipoIdentificacion: cboTipoIdentificacion.GetValue(),
        numeroIdentificacion: txtNumeroIdentificacion.GetValue(),
        vehiculo: txtVehiculo.GetValue(),
        colorVehiculo: txtColorVehiculo.GetValue(),
        marcaVehiculo: txtMarcaVehiculo.GetValue(),
        motivoVisita: txtMotivoVisita.GetValue(),
        tipoVisita: cboTipoVisita.GetValue(),
        fechaEntrada: dteFechaEntrada.GetDate(),
        fechaSalida: null,
        estado: cboEstado.GetValue(),
        observaciones: txtObservaciones.GetValue()
    };

    if (!datos.unidadId || !datos.nombreVisitante || !datos.fechaEntrada) {
        showToast('warning', 'Complete los campos requeridos');
        return;
    }

    ajaxCall('GuardarVisitante', { datos: datos }, function(r) {
        if (r.success) {
            showToast('success', r.message);
            popupVisitante.Hide();
            gridVisitantes.PerformCallback('cargar');
        } else {
            showToast('error', r.message);
        }
    });
}

// ========================================================================
// EVENTOS DE COMBOS
// ========================================================================

function onUnidadChanged(s, e) {
    var unidadId = s.GetValue();
    if (!unidadId) {
        cboResidente.ClearItems();
        return;
    }
    cargarResidentes(unidadId);
}

function cargarResidentes(unidadId, callback) {
    ajaxCall('ListarResidentes', { unidadId: unidadId }, function(r) {
        if (r.success && r.data) {
            cboResidente.ClearItems();
            cboResidente.AddItem('-- Sin asignar --', null);
            r.data.forEach(function(item) {
                cboResidente.AddItem(item.NombreCompleto, item.Id);
            });
            if (callback) callback();
        }
    });
}

// ========================================================================
// HELPERS AJAX
// ========================================================================

function ajaxCall(method, data, callback) {
    $.ajax({
        type: 'POST',
        url: 'Visitantes.aspx/' + method,
        data: JSON.stringify(data),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function(response) {
            callback(response.d);
        },
        error: function(xhr, status, error) {
            showToast('error', 'Error de comunicación: ' + error);
        }
    });
}
