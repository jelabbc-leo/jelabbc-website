// ============================================================================
// Módulo: Áreas Comunes
// Archivo: areas-comunes.js
// ============================================================================

var AreasComunesModule = (function () {
    'use strict';

    function onToolbarAreasClick(s, e) {
        var itemName = e.item.name;

        switch (itemName) {
            case 'btnNuevo':
                mostrarNuevaArea();
                break;
            case 'btnEditar':
                editarAreaSeleccionada();
                break;
            case 'btnEliminar':
                eliminarAreaSeleccionada();
                break;
        }
    }

    function mostrarNuevaArea() {
        limpiarFormulario();
        popupArea.SetHeaderText('Nueva Área Común');
        tabsArea.SetActiveTabIndex(0);
        popupArea.Show();
    }

    function editarAreaSeleccionada() {
        var focusedRowIndex = gridAreas.GetFocusedRowIndex();
        if (focusedRowIndex < 0) {
            showToast('warning', 'Seleccione un área para editar');
            return;
        }

        var id = gridAreas.GetRowKey(focusedRowIndex);
        cargarArea(id);
    }

    function eliminarAreaSeleccionada() {
        var focusedRowIndex = gridAreas.GetFocusedRowIndex();
        if (focusedRowIndex < 0) {
            showToast('warning', 'Seleccione un área para eliminar');
            return;
        }

        var id = gridAreas.GetRowKey(focusedRowIndex);
        var nombre = gridAreas.GetRowValues(focusedRowIndex, 'Nombre');

        if (confirm('¿Está seguro de eliminar el área "' + nombre + '"?\n\nEsta acción no se puede deshacer.')) {
            eliminarArea(id);
        }
    }

    function cargarArea(id) {
        $.ajax({
            type: 'POST',
            url: 'AreasComunes.aspx/ObtenerArea',
            data: JSON.stringify({ id: id }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                var result = response.d;
                if (result.success) {
                    cargarDatosEnFormulario(result.data);
                    popupArea.SetHeaderText('Editar Área Común');
                    tabsArea.SetActiveTabIndex(0);
                    popupArea.Show();
                } else {
                    showToast('error', result.message);
                }
            },
            error: function () {
                showToast('error', 'Error al cargar área');
            }
        });
    }

    function eliminarArea(id) {
        $.ajax({
            type: 'POST',
            url: 'AreasComunes.aspx/EliminarArea',
            data: JSON.stringify({ id: id }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                var result = response.d;
                if (result.success) {
                    showToast('success', result.message);
                    gridAreas.Refresh();
                } else {
                    showToast('error', result.message);
                }
            },
            error: function () {
                showToast('error', 'Error al eliminar área');
            }
        });
    }

    function cargarDatosEnFormulario(data) {
        setHiddenFieldValue('hfAreaId', data.Id || 0);

        // Datos generales
        if (cmbEntidad) cmbEntidad.SetValue(data.EntidadId);
        if (cmbSubEntidad) cmbSubEntidad.SetValue(data.SubEntidadId);
        if (txtClave) txtClave.SetValue(data.Clave || '');
        if (txtNombre) txtNombre.SetValue(data.Nombre || '');
        if (cmbTipoArea) cmbTipoArea.SetValue(data.TipoArea || 'Salon');
        if (txtDescripcion) txtDescripcion.SetValue(data.Descripcion || '');
        if (txtUbicacion) txtUbicacion.SetValue(data.Ubicacion || '');
        if (spnCapacidad) spnCapacidad.SetNumber(data.Capacidad || 0);
        if (chkActivo) chkActivo.SetChecked(data.Activo === 1 || data.Activo === true);

        // Reservación y costos
        if (chkRequiereReservacion) chkRequiereReservacion.SetChecked(data.RequiereReservacion === 1 || data.RequiereReservacion === true);
        if (spnCostoReservacion) spnCostoReservacion.SetNumber(data.CostoReservacion || 0);
        if (spnDeposito) spnDeposito.SetNumber(data.DepositoRequerido || 0);

        // Horarios
        if (tmeHoraApertura && data.HoraApertura) {
            var horaApertura = parseTimeString(data.HoraApertura);
            if (horaApertura) tmeHoraApertura.SetDate(horaApertura);
        }
        if (tmeHoraCierre && data.HoraCierre) {
            var horaCierre = parseTimeString(data.HoraCierre);
            if (horaCierre) tmeHoraCierre.SetDate(horaCierre);
        }

        // Duración
        if (spnDuracionMinima) spnDuracionMinima.SetNumber(data.DuracionMinimaHoras || 2);
        if (spnDuracionMaxima) spnDuracionMaxima.SetNumber(data.DuracionMaximaHoras || 8);

        // Anticipación
        if (spnAnticipacionMinima) spnAnticipacionMinima.SetNumber(data.AnticipacionMinimaDias || 1);
        if (spnAnticipacionMaxima) spnAnticipacionMaxima.SetNumber(data.AnticipacionMaximaDias || 30);

        // Días disponibles
        if (txtDiasDisponibles) txtDiasDisponibles.SetValue(data.DiasDisponibles || 'L,M,X,J,V,S,D');
    }

    function limpiarFormulario() {
        setHiddenFieldValue('hfAreaId', '0');

        if (cmbEntidad) cmbEntidad.SetValue(null);
        if (cmbSubEntidad) cmbSubEntidad.SetValue(null);
        if (txtClave) txtClave.SetValue('');
        if (txtNombre) txtNombre.SetValue('');
        if (cmbTipoArea) cmbTipoArea.SetValue('Salon');
        if (txtDescripcion) txtDescripcion.SetValue('');
        if (txtUbicacion) txtUbicacion.SetValue('');
        if (spnCapacidad) spnCapacidad.SetNumber(0);
        if (chkActivo) chkActivo.SetChecked(true);

        if (chkRequiereReservacion) chkRequiereReservacion.SetChecked(true);
        if (spnCostoReservacion) spnCostoReservacion.SetNumber(0);
        if (spnDeposito) spnDeposito.SetNumber(0);

        if (tmeHoraApertura) tmeHoraApertura.SetDate(new Date(2000, 0, 1, 8, 0, 0));
        if (tmeHoraCierre) tmeHoraCierre.SetDate(new Date(2000, 0, 1, 22, 0, 0));

        if (spnDuracionMinima) spnDuracionMinima.SetNumber(2);
        if (spnDuracionMaxima) spnDuracionMaxima.SetNumber(8);

        if (spnAnticipacionMinima) spnAnticipacionMinima.SetNumber(1);
        if (spnAnticipacionMaxima) spnAnticipacionMaxima.SetNumber(30);

        if (txtDiasDisponibles) txtDiasDisponibles.SetValue('L,M,X,J,V,S,D');
    }

    function onEntidadChanged(s, e) {
        var entidadId = s.GetValue();
        if (!entidadId) {
            if (cmbSubEntidad) {
                cmbSubEntidad.ClearItems();
                cmbSubEntidad.AddItem('(Compartida - Toda la entidad)', null);
            }
            return;
        }

        $.ajax({
            type: 'POST',
            url: 'AreasComunes.aspx/ObtenerSubEntidadesPorEntidad',
            data: JSON.stringify({ entidadId: entidadId }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                var result = response.d;
                if (result.success && cmbSubEntidad) {
                    cmbSubEntidad.ClearItems();
                    cmbSubEntidad.AddItem('(Compartida - Toda la entidad)', null);
                    for (var i = 0; i < result.data.length; i++) {
                        cmbSubEntidad.AddItem(result.data[i].RazonSocial, result.data[i].Id);
                    }
                }
            }
        });
    }

    function onRowDblClick(s, e) {
        var id = s.GetRowKey(e.visibleIndex);
        cargarArea(id);
    }

    function parseTimeString(timeStr) {
        if (!timeStr) return null;
        var parts = timeStr.split(':');
        if (parts.length >= 2) {
            var date = new Date();
            date.setHours(parseInt(parts[0], 10));
            date.setMinutes(parseInt(parts[1], 10));
            date.setSeconds(parts.length > 2 ? parseInt(parts[2], 10) : 0);
            return date;
        }
        return null;
    }

    function setHiddenFieldValue(fieldId, value) {
        var field = document.querySelector('[id$="' + fieldId + '"]');
        if (field) {
            field.value = value;
        }
    }

    return {
        onToolbarAreasClick: onToolbarAreasClick,
        onRowDblClick: onRowDblClick,
        onEntidadChanged: onEntidadChanged,
        mostrarNuevaArea: mostrarNuevaArea,
        editarAreaSeleccionada: editarAreaSeleccionada,
        eliminarAreaSeleccionada: eliminarAreaSeleccionada
    };

})();

// Funciones globales para eventos del grid
function onToolbarAreasClick(s, e) {
    AreasComunesModule.onToolbarAreasClick(s, e);
}

function onRowDblClick(s, e) {
    AreasComunesModule.onRowDblClick(s, e);
}

function onEntidadChanged(s, e) {
    AreasComunesModule.onEntidadChanged(s, e);
}
