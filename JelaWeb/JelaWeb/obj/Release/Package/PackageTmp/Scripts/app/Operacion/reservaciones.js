// ========================================================================
// MÓDULO: Reservaciones
// ========================================================================

var currentReservacionId = 0;

// ========================================================================
// TOOLBAR DEL GRID
// ========================================================================

function onToolbarReservacionesClick(s, e) {
    switch (e.item.name) {
        case 'btnNuevaReservacion':
            mostrarNuevaReservacion();
            break;
        case 'btnEditarReservacion':
            editarReservacionSeleccionada();
            break;
        case 'btnEliminarReservacion':
            eliminarReservacionSeleccionada();
            break;
    }
}

function onRowDblClick(s, e) {
    editarReservacionSeleccionada();
}

// ========================================================================
// FUNCIONES CRUD
// ========================================================================

function mostrarNuevaReservacion() {
    limpiarFormularioReservacion();
    document.getElementById('hfId').value = '0';
    currentReservacionId = 0;
    popupReservacion.SetHeaderText('Nueva Reservación');
    popupReservacion.Show();
}

function editarReservacionSeleccionada() {
    var idx = gridReservaciones.GetFocusedRowIndex();
    if (idx < 0) {
        showToast('warning', 'Seleccione una reservación');
        return;
    }
    var id = gridReservaciones.GetRowKey(idx);
    cargarReservacion(id);
}

function eliminarReservacionSeleccionada() {
    var idx = gridReservaciones.GetFocusedRowIndex();
    if (idx < 0) {
        showToast('warning', 'Seleccione una reservación');
        return;
    }
    if (confirm('¿Eliminar esta reservación?')) {
        ajaxCall('EliminarReservacion', { id: gridReservaciones.GetRowKey(idx) }, function(r) {
            if (r.success) {
                showToast('success', r.message);
                gridReservaciones.PerformCallback('cargar');
            } else {
                showToast('error', r.message);
            }
        });
    }
}

function cargarReservacion(id) {
    ajaxCall('ObtenerReservacion', { id: id }, function(r) {
        if (r.success && r.data) {
            currentReservacionId = id;
            document.getElementById('hfId').value = id;
            cargarDatosReservacion(r.data);
            popupReservacion.SetHeaderText('Editar Reservación');
            popupReservacion.Show();
        } else {
            showToast('error', r.message || 'Error al cargar reservación');
        }
    });
}

function cargarDatosReservacion(d) {
    if (cboEntidad) cboEntidad.SetValue(d.EntidadId);
    if (cboAreaComun) {
        cargarAreasComunes(d.EntidadId, function() {
            cboAreaComun.SetValue(d.AreaComunId);
        });
    }
    if (cboUnidad) {
        cargarUnidades(d.EntidadId, function() {
            cboUnidad.SetValue(d.UnidadId);
        });
    }
    if (cboResidente && d.UnidadId) {
        cargarResidentes(d.UnidadId, function() {
            cboResidente.SetValue(d.ResidenteId);
        });
    }
    if (dteFechaReservacion && d.FechaReservacion) {
        dteFechaReservacion.SetDate(new Date(d.FechaReservacion));
    }
    if (cboEstado) cboEstado.SetValue(d.Estado || 'Pendiente');
    if (teHoraInicio && d.HoraInicio) {
        var horaInicio = typeof d.HoraInicio === 'string' ? d.HoraInicio : d.HoraInicio.toString();
        teHoraInicio.SetValue(horaInicio);
    }
    if (teHoraFin && d.HoraFin) {
        var horaFin = typeof d.HoraFin === 'string' ? d.HoraFin : d.HoraFin.toString();
        teHoraFin.SetValue(horaFin);
    }
    if (spNumeroInvitados) spNumeroInvitados.SetNumber(d.NumeroInvitados || 0);
    if (spCostoTotal) spCostoTotal.SetNumber(d.CostoTotal || 0);
    if (spDepositoPagado) spDepositoPagado.SetNumber(d.DepositoPagado || 0);
    if (spDepositoDevuelto) spDepositoDevuelto.SetNumber(d.DepositoDevuelto || 0);
    if (txtMotivo) txtMotivo.SetValue(d.Motivo || '');
    if (txtObservaciones) txtObservaciones.SetValue(d.Observaciones || '');
}

function limpiarFormularioReservacion() {
    if (cboEntidad) cboEntidad.SetValue(null);
    if (cboAreaComun) cboAreaComun.ClearItems();
    if (cboUnidad) cboUnidad.ClearItems();
    if (cboResidente) cboResidente.ClearItems();
    if (dteFechaReservacion) dteFechaReservacion.SetDate(new Date());
    if (cboEstado) cboEstado.SetValue('Pendiente');
    if (teHoraInicio) teHoraInicio.SetValue('08:00');
    if (teHoraFin) teHoraFin.SetValue('10:00');
    if (spNumeroInvitados) spNumeroInvitados.SetNumber(0);
    if (spCostoTotal) spCostoTotal.SetNumber(0);
    if (spDepositoPagado) spDepositoPagado.SetNumber(0);
    if (spDepositoDevuelto) spDepositoDevuelto.SetNumber(0);
    if (txtMotivo) txtMotivo.SetValue('');
    if (txtObservaciones) txtObservaciones.SetValue('');
}

function guardarReservacion() {
    var datos = {
        id: parseInt(document.getElementById('hfId').value) || 0,
        entidadId: cboEntidad.GetValue(),
        areaComunId: cboAreaComun.GetValue(),
        unidadId: cboUnidad.GetValue(),
        residenteId: cboResidente.GetValue(),
        fechaReservacion: dteFechaReservacion.GetDate(),
        horaInicio: teHoraInicio.GetValue(),
        horaFin: teHoraFin.GetValue(),
        numeroInvitados: spNumeroInvitados.GetNumber(),
        motivo: txtMotivo.GetValue(),
        costoTotal: spCostoTotal.GetNumber(),
        depositoPagado: spDepositoPagado.GetNumber(),
        depositoDevuelto: spDepositoDevuelto.GetNumber(),
        estado: cboEstado.GetValue(),
        observaciones: txtObservaciones.GetValue()
    };

    if (!datos.entidadId || !datos.areaComunId || !datos.unidadId || !datos.fechaReservacion) {
        showToast('warning', 'Complete los campos requeridos');
        return;
    }

    ajaxCall('GuardarReservacion', { datos: datos }, function(r) {
        if (r.success) {
            showToast('success', r.message);
            popupReservacion.Hide();
            gridReservaciones.PerformCallback('cargar');
        } else {
            showToast('error', r.message);
        }
    });
}

// ========================================================================
// EVENTOS DE COMBOS
// ========================================================================

function onEntidadChanged(s, e) {
    var entidadId = s.GetValue();
    if (!entidadId) {
        cboAreaComun.ClearItems();
        cboUnidad.ClearItems();
        return;
    }
    cargarAreasComunes(entidadId);
    cargarUnidades(entidadId);
}

function onUnidadChanged(s, e) {
    var unidadId = s.GetValue();
    if (!unidadId) {
        cboResidente.ClearItems();
        return;
    }
    cargarResidentes(unidadId);
}

function cargarAreasComunes(entidadId, callback) {
    ajaxCall('ListarAreasComunes', { entidadId: entidadId }, function(r) {
        if (r.success && r.data) {
            cboAreaComun.ClearItems();
            cboAreaComun.AddItem('-- Seleccione --', null);
            r.data.forEach(function(item) {
                cboAreaComun.AddItem(item.Nombre, item.Id);
            });
            if (callback) callback();
        }
    });
}

function cargarUnidades(entidadId, callback) {
    ajaxCall('ListarUnidades', { entidadId: entidadId }, function(r) {
        if (r.success && r.data) {
            cboUnidad.ClearItems();
            cboUnidad.AddItem('-- Seleccione --', null);
            r.data.forEach(function(item) {
                cboUnidad.AddItem(item.NombreCompleto, item.Id);
            });
            if (callback) callback();
        }
    });
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
        url: 'Reservaciones.aspx/' + method,
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
