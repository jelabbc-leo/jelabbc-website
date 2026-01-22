// ========================================================================
// MÓDULO: Comunicados
// ========================================================================

var currentComunicadoId = 0;

// ========================================================================
// TOOLBAR DEL GRID
// ========================================================================

function onToolbarComunicadosClick(s, e) {
    switch (e.item.name) {
        case 'btnNuevoComunicado':
            mostrarNuevoComunicado();
            break;
        case 'btnEditarComunicado':
            editarComunicadoSeleccionado();
            break;
        case 'btnPublicarComunicado':
            publicarComunicadoSeleccionado();
            break;
        case 'btnEliminarComunicado':
            eliminarComunicadoSeleccionado();
            break;
    }
}

function onRowDblClick(s, e) {
    editarComunicadoSeleccionado();
}

// ========================================================================
// FUNCIONES CRUD
// ========================================================================

function mostrarNuevoComunicado() {
    limpiarFormularioComunicado();
    document.getElementById('hfId').value = '0';
    currentComunicadoId = 0;
    if (dteFechaPublicacion) dteFechaPublicacion.SetDate(new Date());
    popupComunicado.SetHeaderText('Nuevo Comunicado');
    popupComunicado.Show();
}

function editarComunicadoSeleccionado() {
    var idx = gridComunicados.GetFocusedRowIndex();
    if (idx < 0) {
        showToast('warning', 'Seleccione un comunicado');
        return;
    }
    var id = gridComunicados.GetRowKey(idx);
    cargarComunicado(id);
}

function eliminarComunicadoSeleccionado() {
    var idx = gridComunicados.GetFocusedRowIndex();
    if (idx < 0) {
        showToast('warning', 'Seleccione un comunicado');
        return;
    }
    if (confirm('¿Eliminar este comunicado?')) {
        ajaxCall('EliminarComunicado', { id: gridComunicados.GetRowKey(idx) }, function(r) {
            if (r.success) {
                showToast('success', r.message);
                gridComunicados.PerformCallback('cargar');
            } else {
                showToast('error', r.message);
            }
        });
    }
}

function publicarComunicadoSeleccionado() {
    var idx = gridComunicados.GetFocusedRowIndex();
    if (idx < 0) {
        showToast('warning', 'Seleccione un comunicado');
        return;
    }
    if (confirm('¿Publicar este comunicado?')) {
        ajaxCall('PublicarComunicado', { id: gridComunicados.GetRowKey(idx) }, function(r) {
            if (r.success) {
                showToast('success', r.message);
                gridComunicados.PerformCallback('cargar');
            } else {
                showToast('error', r.message);
            }
        });
    }
}

function cargarComunicado(id) {
    ajaxCall('ObtenerComunicado', { id: id }, function(r) {
        if (r.success && r.data) {
            currentComunicadoId = id;
            document.getElementById('hfId').value = id;
            cargarDatosComunicado(r.data);
            popupComunicado.SetHeaderText('Editar Comunicado');
            popupComunicado.Show();
        } else {
            showToast('error', r.message || 'Error al cargar comunicado');
        }
    });
}

function cargarDatosComunicado(d) {
    if (cboEntidad) cboEntidad.SetValue(d.EntidadId);
    if (cboSubEntidad && d.EntidadId) {
        cargarSubEntidades(d.EntidadId, function() {
            cboSubEntidad.SetValue(d.SubEntidadId);
        });
    }
    if (txtTitulo) txtTitulo.SetValue(d.Titulo || '');
    if (cboTipoComunicado) cboTipoComunicado.SetValue(d.TipoComunicado || 'General');
    if (cboDestinatarios) cboDestinatarios.SetValue(d.Destinatarios || 'Todos');
    if (dteFechaPublicacion && d.FechaPublicacion) {
        dteFechaPublicacion.SetDate(new Date(d.FechaPublicacion));
    }
    if (dteFechaExpiracion && d.FechaExpiracion) {
        dteFechaExpiracion.SetDate(new Date(d.FechaExpiracion));
    }
    if (cboEstado) cboEstado.SetValue(d.Estado || 'Borrador');
    if (chkEnviarEmail) chkEnviarEmail.SetChecked(d.EnviarEmail == 1);
    if (chkEnviarTelegram) chkEnviarTelegram.SetChecked(d.EnviarTelegram == 1);
    if (chkEnviarPush) chkEnviarPush.SetChecked(d.EnviarPush == 1);
    if (txtContenido) txtContenido.SetValue(d.Contenido || '');
    if (document.getElementById('hfArchivoAdjunto')) {
        document.getElementById('hfArchivoAdjunto').value = d.ArchivoAdjunto || '';
    }
}

function limpiarFormularioComunicado() {
    if (cboEntidad) cboEntidad.SetValue(null);
    if (cboSubEntidad) cboSubEntidad.ClearItems();
    if (txtTitulo) txtTitulo.SetValue('');
    if (cboTipoComunicado) cboTipoComunicado.SetValue('General');
    if (cboDestinatarios) cboDestinatarios.SetValue('Todos');
    if (dteFechaPublicacion) dteFechaPublicacion.SetDate(new Date());
    if (dteFechaExpiracion) dteFechaExpiracion.SetDate(null);
    if (cboEstado) cboEstado.SetValue('Borrador');
    if (chkEnviarEmail) chkEnviarEmail.SetChecked(false);
    if (chkEnviarTelegram) chkEnviarTelegram.SetChecked(false);
    if (chkEnviarPush) chkEnviarPush.SetChecked(false);
    if (txtContenido) txtContenido.SetValue('');
    if (document.getElementById('hfArchivoAdjunto')) {
        document.getElementById('hfArchivoAdjunto').value = '';
    }
}

function guardarComunicado() {
    var datos = {
        id: parseInt(document.getElementById('hfId').value) || 0,
        entidadId: cboEntidad.GetValue(),
        subEntidadId: cboSubEntidad.GetValue(),
        titulo: txtTitulo.GetValue(),
        tipoComunicado: cboTipoComunicado.GetValue(),
        destinatarios: cboDestinatarios.GetValue(),
        fechaPublicacion: dteFechaPublicacion.GetDate(),
        fechaExpiracion: dteFechaExpiracion.GetDate(),
        estado: cboEstado.GetValue(),
        enviarEmail: chkEnviarEmail.GetChecked(),
        enviarTelegram: chkEnviarTelegram.GetChecked(),
        enviarPush: chkEnviarPush.GetChecked(),
        contenido: txtContenido.GetValue(),
        archivoAdjunto: document.getElementById('hfArchivoAdjunto') ? document.getElementById('hfArchivoAdjunto').value : ''
    };

    if (!datos.entidadId || !datos.titulo || !datos.contenido || !datos.fechaPublicacion) {
        showToast('warning', 'Complete los campos requeridos');
        return;
    }

    ajaxCall('GuardarComunicado', { datos: datos }, function(r) {
        if (r.success) {
            showToast('success', r.message);
            popupComunicado.Hide();
            gridComunicados.PerformCallback('cargar');
        } else {
            showToast('error', r.message);
        }
    });
}

function onArchivoUploadComplete(s, e) {
    if (e.isValid && e.callbackData) {
        if (document.getElementById('hfArchivoAdjunto')) {
            document.getElementById('hfArchivoAdjunto').value = e.callbackData;
        }
        showToast('success', 'Archivo subido correctamente');
    } else {
        showToast('error', 'Error al subir archivo');
    }
}

// ========================================================================
// EVENTOS DE COMBOS
// ========================================================================

function onEntidadChanged(s, e) {
    var entidadId = s.GetValue();
    if (!entidadId) {
        cboSubEntidad.ClearItems();
        return;
    }
    cargarSubEntidades(entidadId);
}

function cargarSubEntidades(entidadId, callback) {
    ajaxCall('ListarSubEntidades', { entidadId: entidadId }, function(r) {
        if (r.success && r.data) {
            cboSubEntidad.ClearItems();
            cboSubEntidad.AddItem('-- Ninguna (Toda la entidad) --', null);
            r.data.forEach(function(item) {
                cboSubEntidad.AddItem(item.Nombre, item.Id);
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
        url: 'Comunicados.aspx/' + method,
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
