// ============================================================================
// Módulo: Sub Entidades
// Archivo: sub-entidades.js
// ============================================================================

var SubEntidadesModule = (function () {
    'use strict';

    function onToolbarSubEntidadesClick(s, e) {
        var itemName = e.item.name;

        switch (itemName) {
            case 'btnNuevo':
                mostrarNuevaSubEntidad();
                break;
            case 'btnEditar':
                editarSubEntidadSeleccionada();
                break;
            case 'btnEliminar':
                eliminarSubEntidadSeleccionada();
                break;
        }
    }

    function mostrarNuevaSubEntidad() {
        limpiarFormulario();
        popupSubEntidad.SetHeaderText('Nueva Sub Entidad');
        tabsSubEntidad.SetActiveTabIndex(0);
        popupSubEntidad.Show();
        actualizarSeccionVisible();
    }

    function editarSubEntidadSeleccionada() {
        var focusedRowIndex = gridSubEntidades.GetFocusedRowIndex();
        if (focusedRowIndex < 0) {
            showToast('warning', 'Seleccione una sub entidad para editar');
            return;
        }

        var id = gridSubEntidades.GetRowKey(focusedRowIndex);
        cargarSubEntidad(id);
    }

    function eliminarSubEntidadSeleccionada() {
        var focusedRowIndex = gridSubEntidades.GetFocusedRowIndex();
        if (focusedRowIndex < 0) {
            showToast('warning', 'Seleccione una sub entidad para eliminar');
            return;
        }

        var id = gridSubEntidades.GetRowKey(focusedRowIndex);
        var nombre = gridSubEntidades.GetRowValues(focusedRowIndex, 'RazonSocial');

        if (confirm('¿Está seguro de eliminar la sub entidad "' + nombre + '"?\n\nEsta acción no se puede deshacer.')) {
            eliminarSubEntidad(id);
        }
    }

    function cargarSubEntidad(id) {
        $.ajax({
            type: 'POST',
            url: 'SubEntidades.aspx/ObtenerSubEntidad',
            data: JSON.stringify({ id: id }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                var result = response.d;
                if (result.success) {
                    cargarDatosEnFormulario(result.data);
                    popupSubEntidad.SetHeaderText('Editar Sub Entidad');
                    tabsSubEntidad.SetActiveTabIndex(0);
                    popupSubEntidad.Show();
                    actualizarSeccionVisible();
                } else {
                    showToast('error', result.message);
                }
            },
            error: function () {
                showToast('error', 'Error al cargar sub entidad');
            }
        });
    }

    function eliminarSubEntidad(id) {
        $.ajax({
            type: 'POST',
            url: 'SubEntidades.aspx/EliminarSubEntidad',
            data: JSON.stringify({ id: id }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                var result = response.d;
                if (result.success) {
                    showToast('success', result.message);
                    gridSubEntidades.Refresh();
                } else {
                    showToast('error', result.message);
                }
            },
            error: function () {
                showToast('error', 'Error al eliminar sub entidad');
            }
        });
    }

    function cargarDatosEnFormulario(data) {
        setHiddenFieldValue('hfSubEntidadId', data.Id || 0);

        if (txtClave) txtClave.SetValue(data.Clave || '');
        if (txtAlias) txtAlias.SetValue(data.Alias || '');
        if (txtCIF) txtCIF.SetValue(data.CIF || '');
        if (txtRFC) txtRFC.SetValue(data.RFC || '');
        if (txtRazonSocial) txtRazonSocial.SetValue(data.RazonSocial || '');
        if (dateFechaAlta && data.FechaAlta) {
            var fechaAlta = parseNetDate(data.FechaAlta);
            if (fechaAlta) {
                dateFechaAlta.SetDate(fechaAlta);
            }
        }
        if (chkActivo) chkActivo.SetChecked(data.Activo === 1 || data.Activo === true);

        if (txtTelefonos) txtTelefonos.SetValue(data.Telefonos || '');
        if (txtWhatsapp) txtWhatsapp.SetValue(data.Whatsapp || '');
        if (txtMail) txtMail.SetValue(data.Mail || '');
        if (txtAdministrador) txtAdministrador.SetValue(data.Administrador || '');
        if (txtTelefonoVigilancia) txtTelefonoVigilancia.SetValue(data.TelefonoVigilancia || '');

        if (txtCP) txtCP.SetValue(data.CP || '');
        if (txtTipoVialidad) txtTipoVialidad.SetValue(data.TipoVialidad || '');
        if (txtNombreVialidad) txtNombreVialidad.SetValue(data.NombreVialidad || '');
        if (txtNoExterior) txtNoExterior.SetValue(data.NoExterior || '');
        if (txtNoInterior) txtNoInterior.SetValue(data.NoInterior || '');
        if (txtColonia) txtColonia.SetValue(data.Colonia || '');
        if (txtLocalidad) txtLocalidad.SetValue(data.Localidad || '');
        if (txtMunicipio) txtMunicipio.SetValue(data.Municipio || '');
        if (txtEntidadFederativa) txtEntidadFederativa.SetValue(data.EntidadFederativa || '');
        if (txtEntreCalle) txtEntreCalle.SetValue(data.EntreCalle || '');

        if (chkEsSeccionCondominio) chkEsSeccionCondominio.SetChecked(data.EsSeccionCondominio === 1 || data.EsSeccionCondominio === true);
        if (cmbTipoSeccion) cmbTipoSeccion.SetValue(data.TipoSeccion || null);
        if (spnNumeroNiveles) spnNumeroNiveles.SetNumber(data.NumeroNiveles || 0);
        if (spnNumeroUnidades) spnNumeroUnidades.SetNumber(data.NumeroUnidades || 0);
        if (chkTieneElevador) chkTieneElevador.SetChecked(data.TieneElevador === 1 || data.TieneElevador === true);
        if (spnNumeroElevadores) spnNumeroElevadores.SetNumber(data.NumeroElevadores || 0);
        if (chkTieneEstacionamiento) chkTieneEstacionamiento.SetChecked(data.TieneEstacionamiento === 1 || data.TieneEstacionamiento === true);
        if (spnCajonesEstacionamiento) spnCajonesEstacionamiento.SetNumber(data.CajonesEstacionamiento || 0);

        if (spnLatitud) spnLatitud.SetNumber(data.Latitud || 0);
        if (spnLongitud) spnLongitud.SetNumber(data.Longitud || 0);
    }

    function limpiarFormulario() {
        setHiddenFieldValue('hfSubEntidadId', '0');

        if (txtClave) txtClave.SetValue('');
        if (txtAlias) txtAlias.SetValue('');
        if (txtCIF) txtCIF.SetValue('');
        if (txtRFC) txtRFC.SetValue('');
        if (txtRazonSocial) txtRazonSocial.SetValue('');
        if (dateFechaAlta) dateFechaAlta.SetDate(new Date());
        if (chkActivo) chkActivo.SetChecked(true);

        if (txtTelefonos) txtTelefonos.SetValue('');
        if (txtWhatsapp) txtWhatsapp.SetValue('');
        if (txtMail) txtMail.SetValue('');
        if (txtAdministrador) txtAdministrador.SetValue('');
        if (txtTelefonoVigilancia) txtTelefonoVigilancia.SetValue('');

        if (txtCP) txtCP.SetValue('');
        if (txtTipoVialidad) txtTipoVialidad.SetValue('');
        if (txtNombreVialidad) txtNombreVialidad.SetValue('');
        if (txtNoExterior) txtNoExterior.SetValue('');
        if (txtNoInterior) txtNoInterior.SetValue('');
        if (txtColonia) txtColonia.SetValue('');
        if (txtLocalidad) txtLocalidad.SetValue('');
        if (txtMunicipio) txtMunicipio.SetValue('');
        if (txtEntidadFederativa) txtEntidadFederativa.SetValue('');
        if (txtEntreCalle) txtEntreCalle.SetValue('');

        if (chkEsSeccionCondominio) chkEsSeccionCondominio.SetChecked(false);
        if (cmbTipoSeccion) cmbTipoSeccion.SetValue(null);
        if (spnNumeroNiveles) spnNumeroNiveles.SetNumber(0);
        if (spnNumeroUnidades) spnNumeroUnidades.SetNumber(0);
        if (chkTieneElevador) chkTieneElevador.SetChecked(false);
        if (spnNumeroElevadores) spnNumeroElevadores.SetNumber(0);
        if (chkTieneEstacionamiento) chkTieneEstacionamiento.SetChecked(false);
        if (spnCajonesEstacionamiento) spnCajonesEstacionamiento.SetNumber(0);

        if (spnLatitud) spnLatitud.SetNumber(0);
        if (spnLongitud) spnLongitud.SetNumber(0);
    }

    function onRowDblClickSubEntidades(s, e) {
        var id = s.GetRowKey(e.visibleIndex);
        cargarSubEntidad(id);
    }

    function onEsSeccionChanged() {
        actualizarSeccionVisible();
    }

    function onCerrarSubEntidadClick() {
        popupSubEntidad.Hide();
    }

    function actualizarSeccionVisible() {
        var esSeccion = chkEsSeccionCondominio && chkEsSeccionCondominio.GetChecked();
        var tabIndex = tabsSubEntidad.GetTabIndexByName('tabSeccion');
        if (tabIndex >= 0) {
            tabsSubEntidad.SetTabVisible(tabIndex, true);
        }
        if (esSeccion === false) {
            if (cmbTipoSeccion) cmbTipoSeccion.SetValue(null);
            if (spnNumeroNiveles) spnNumeroNiveles.SetNumber(0);
            if (spnNumeroUnidades) spnNumeroUnidades.SetNumber(0);
            if (chkTieneElevador) chkTieneElevador.SetChecked(false);
            if (spnNumeroElevadores) spnNumeroElevadores.SetNumber(0);
            if (chkTieneEstacionamiento) chkTieneEstacionamiento.SetChecked(false);
            if (spnCajonesEstacionamiento) spnCajonesEstacionamiento.SetNumber(0);
        }
    }

    function setHiddenFieldValue(fieldId, value) {
        var field = document.querySelector('[id$="' + fieldId + '"]');
        if (field) {
            field.value = value;
        }
    }

    function parseNetDate(value) {
        if (!value) return null;

        if (value instanceof Date) return value;

        var match = /Date\((\d+)\)/.exec(value);
        if (match && match.length > 1) {
            return new Date(parseInt(match[1], 10));
        }

        var parsed = new Date(value);
        if (!isNaN(parsed.getTime())) {
            return parsed;
        }

        return null;
    }

    return {
        onToolbarSubEntidadesClick: onToolbarSubEntidadesClick,
        onRowDblClickSubEntidades: onRowDblClickSubEntidades,
        onEsSeccionChanged: onEsSeccionChanged,
        onCerrarSubEntidadClick: onCerrarSubEntidadClick
    };

})();

function onToolbarSubEntidadesClick(s, e) {
    SubEntidadesModule.onToolbarSubEntidadesClick(s, e);
}

function onRowDblClickSubEntidades(s, e) {
    SubEntidadesModule.onRowDblClickSubEntidades(s, e);
}

function onEsSeccionChanged(s, e) {
    SubEntidadesModule.onEsSeccionChanged(s, e);
}

function onCerrarSubEntidadClick(s, e) {
    SubEntidadesModule.onCerrarSubEntidadClick(s, e);
}
