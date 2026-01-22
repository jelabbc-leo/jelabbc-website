// ========================================================================
// MÓDULO: Reservaciones
// ========================================================================
// NOTA: Sistema multi-entidad - La entidad se obtiene automáticamente desde la sesión
// No se requiere selector de entidad en el formulario
// ========================================================================

var currentReservacionId = 0;

// ========================================================================
// EVENTOS DEL CALENDARIO
// ========================================================================

function onSchedulerAppointmentDblClick(s, e) {
    e.handled = true;

    var id = e.appointmentId || (e.appointment ? e.appointment.id : null);

    if (id) {
        cargarReservacion(id);
    }
}

function onSchedulerCellDblClick(s, e) {
    e.handled = true;
    mostrarNuevaReservacion();

    if (dteFechaReservacion && e.interval && e.interval.start) {
        dteFechaReservacion.SetDate(e.interval.start);
    }
}

function onSchedulerInit(s, e) {
    var el = s.GetMainElement ? s.GetMainElement() : (s.GetElement ? s.GetElement() : null);
    if (!el || el.__jelaCtxBound) return;

    el.__jelaCtxBound = true;
    el.addEventListener('contextmenu', function (ev) {
        ev.preventDefault();
        if (menuReservaciones) {
            menuReservaciones.ShowAtPos(ev.clientX, ev.clientY);
        }
    });

    bindSchedulerFloatingButton(el);
}

function bindSchedulerFloatingButton(root) {
    if (!root || root.__jelaFabBound) return;

    root.__jelaFabBound = true;

    var attachHandlers = function () {
        var nodes = root.querySelectorAll('[title], [aria-label]');

        nodes.forEach(function (node) {
            var label = (node.getAttribute('title') || node.getAttribute('aria-label') || '').toLowerCase();

            if (!label) return;

            var isNewAppointment = label.indexOf('appointment') >= 0 ||
                label.indexOf('cita') >= 0 ||
                label.indexOf('reservacion') >= 0 ||
                label.indexOf('reservación') >= 0 ||
                label.indexOf('nuevo') >= 0;

            if (!isNewAppointment) return;

            if (node.__jelaFabClick) return;

            node.__jelaFabClick = true;
            node.addEventListener('click', function (ev) {
                ev.preventDefault();
                ev.stopPropagation();
                mostrarNuevaReservacion();
            });
        });
    };

    attachHandlers();

    var observer = new MutationObserver(function () {
        attachHandlers();
    });

    observer.observe(root, { childList: true, subtree: true });
}

function onReservacionesMenuItemClick(s, e) {
    switch (e.item.name) {
        case 'nueva':
            mostrarNuevaReservacion();
            break;
        case 'eliminar':
            eliminarReservacionActual();
            break;
    }
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

function eliminarReservacionActual() {
    var id = parseInt(document.getElementById('hfId').value) || 0;

    if (!id) {
        showToast('warning', 'Seleccione una reservación');
        return;
    }

    if (confirm('¿Eliminar esta reservación?')) {
        ajaxCall('EliminarReservacion', { id: id }, function(r) {
            if (r.success) {
                showToast('success', r.message);
                window.location.reload();
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
    // NOTA: EntidadId se maneja automáticamente desde la sesión
    if (cboAreaComun) cboAreaComun.SetValue(d.AreaComunId);
    if (cboUnidad) cboUnidad.SetValue(d.UnidadId);
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
        teHoraInicio.SetText(horaInicio);
    }
    if (teHoraFin && d.HoraFin) {
        var horaFin = typeof d.HoraFin === 'string' ? d.HoraFin : d.HoraFin.toString();
        teHoraFin.SetText(horaFin);
    }
    if (spNumeroInvitados) spNumeroInvitados.SetNumber(d.NumeroInvitados || 0);
    if (spCostoTotal) spCostoTotal.SetNumber(d.CostoTotal || 0);
    if (spDepositoPagado) spDepositoPagado.SetNumber(d.DepositoPagado || 0);
    if (spDepositoDevuelto) spDepositoDevuelto.SetNumber(d.DepositoDevuelto || 0);
    if (txtMotivo) txtMotivo.SetValue(d.Motivo || '');
    if (txtObservaciones) txtObservaciones.SetValue(d.Observaciones || '');
}

function limpiarFormularioReservacion() {
    // NOTA: No hay combo de entidad - se maneja automáticamente
    if (cboAreaComun) cboAreaComun.SetValue(null);
    if (cboUnidad) cboUnidad.SetValue(null);
    if (cboResidente) cboResidente.ClearItems();
    if (dteFechaReservacion) dteFechaReservacion.SetDate(new Date());
    if (cboEstado) cboEstado.SetValue('Pendiente');
    if (teHoraInicio) teHoraInicio.SetText('08:00');
    if (teHoraFin) teHoraFin.SetText('10:00');
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
        // NOTA: entidadId se obtiene automáticamente desde la sesión en el backend
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

    if (!datos.areaComunId || !datos.unidadId || !datos.fechaReservacion) {
        showToast('warning', 'Complete los campos requeridos');
        return;
    }

    ajaxCall('GuardarReservacion', { datos: datos }, function(r) {
        if (r.success) {
            showToast('success', r.message);
            popupReservacion.Hide();
            window.location.reload();
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
