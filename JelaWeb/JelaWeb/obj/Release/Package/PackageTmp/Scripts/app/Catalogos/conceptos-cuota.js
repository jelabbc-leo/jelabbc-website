// ============================================================================
// Módulo: Conceptos de Cuota
// Archivo: conceptos-cuota.js
// ============================================================================

var ConceptosCuotaModule = (function () {
    'use strict';

    function onToolbarConceptosClick(s, e) {
        var itemName = e.item.name;

        switch (itemName) {
            case 'btnNuevo':
                mostrarNuevoConcepto();
                break;
            case 'btnEditar':
                editarConceptoSeleccionado();
                break;
            case 'btnEliminar':
                eliminarConceptoSeleccionado();
                break;
        }
    }

    function mostrarNuevoConcepto() {
        limpiarFormulario();
        popupConcepto.SetHeaderText('Nuevo Concepto de Cuota');
        tabsConcepto.SetActiveTabIndex(0);
        popupConcepto.Show();
    }

    function editarConceptoSeleccionado() {
        var focusedRowIndex = gridConceptos.GetFocusedRowIndex();
        if (focusedRowIndex < 0) {
            toastr.warning('Seleccione un concepto para editar');
            return;
        }

        var id = gridConceptos.GetRowKey(focusedRowIndex);
        cargarConcepto(id);
    }

    function eliminarConceptoSeleccionado() {
        var focusedRowIndex = gridConceptos.GetFocusedRowIndex();
        if (focusedRowIndex < 0) {
            toastr.warning('Seleccione un concepto para eliminar');
            return;
        }

        var id = gridConceptos.GetRowKey(focusedRowIndex);
        var nombre = gridConceptos.GetRowValues(focusedRowIndex, 'Nombre');

        if (confirm('¿Está seguro de eliminar el concepto "' + nombre + '"?\n\nEsta acción no se puede deshacer.')) {
            eliminarConcepto(id);
        }
    }

    function cargarConcepto(id) {
        $.ajax({
            type: 'POST',
            url: 'ConceptosCuota.aspx/ObtenerConcepto',
            data: JSON.stringify({ id: id }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                var result = response.d;
                if (result.success) {
                    cargarDatosEnFormulario(result.data);
                    popupConcepto.SetHeaderText('Editar Concepto de Cuota');
                    tabsConcepto.SetActiveTabIndex(0);
                    popupConcepto.Show();
                } else {
                    toastr.error(result.message);
                }
            },
            error: function () {
                toastr.error('Error al cargar concepto');
            }
        });
    }

    function eliminarConcepto(id) {
        $.ajax({
            type: 'POST',
            url: 'ConceptosCuota.aspx/EliminarConcepto',
            data: JSON.stringify({ id: id }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                var result = response.d;
                if (result.success) {
                    toastr.success(result.message);
                    gridConceptos.Refresh();
                } else {
                    toastr.error(result.message);
                }
            },
            error: function () {
                toastr.error('Error al eliminar concepto');
            }
        });
    }

    function cargarDatosEnFormulario(data) {
        setHiddenFieldValue('hfConceptoId', data.Id || 0);

        // Datos generales
        if (cmbEntidad) cmbEntidad.SetValue(data.EntidadId);
        if (txtClave) txtClave.SetValue(data.Clave || '');
        if (txtNombre) txtNombre.SetValue(data.Nombre || '');
        if (txtDescripcion) txtDescripcion.SetValue(data.Descripcion || '');
        if (cmbTipoCuota) cmbTipoCuota.SetValue(data.TipoCuota || 'Ordinaria');
        if (spnMontoBase) spnMontoBase.SetNumber(data.MontoBase || 0);
        if (txtCuentaContable) txtCuentaContable.SetValue(data.CuentaContable || '');
        if (chkActivo) chkActivo.SetChecked(data.Activo === 1 || data.Activo === true);

        // Configuración de cobro
        if (chkEsRecurrente) chkEsRecurrente.SetChecked(data.EsRecurrente === 1 || data.EsRecurrente === true);
        if (cmbPeriodicidad) cmbPeriodicidad.SetValue(data.Periodicidad || 'Mensual');
        if (spnDiaVencimiento) spnDiaVencimiento.SetNumber(data.DiaVencimiento || 10);
        if (spnDiasGracia) spnDiasGracia.SetNumber(data.DiasGracia || 5);

        // Recargos
        if (chkAplicaRecargo) chkAplicaRecargo.SetChecked(data.AplicaRecargo === 1 || data.AplicaRecargo === true);
        if (spnPorcentajeRecargo) spnPorcentajeRecargo.SetNumber(data.PorcentajeRecargo || 5);

        // Descuentos
        if (chkAplicaDescuento) chkAplicaDescuento.SetChecked(data.AplicaDescuentoProntoPago === 1 || data.AplicaDescuentoProntoPago === true);
        if (spnPorcentajeDescuento) spnPorcentajeDescuento.SetNumber(data.PorcentajeDescuento || 0);
        if (spnDiaLimiteDescuento) spnDiaLimiteDescuento.SetNumber(data.DiaLimiteDescuento || 5);
    }

    function limpiarFormulario() {
        setHiddenFieldValue('hfConceptoId', '0');

        if (cmbEntidad) cmbEntidad.SetValue(null);
        if (txtClave) txtClave.SetValue('');
        if (txtNombre) txtNombre.SetValue('');
        if (txtDescripcion) txtDescripcion.SetValue('');
        if (cmbTipoCuota) cmbTipoCuota.SetValue('Ordinaria');
        if (spnMontoBase) spnMontoBase.SetNumber(0);
        if (txtCuentaContable) txtCuentaContable.SetValue('');
        if (chkActivo) chkActivo.SetChecked(true);

        if (chkEsRecurrente) chkEsRecurrente.SetChecked(true);
        if (cmbPeriodicidad) cmbPeriodicidad.SetValue('Mensual');
        if (spnDiaVencimiento) spnDiaVencimiento.SetNumber(10);
        if (spnDiasGracia) spnDiasGracia.SetNumber(5);

        if (chkAplicaRecargo) chkAplicaRecargo.SetChecked(true);
        if (spnPorcentajeRecargo) spnPorcentajeRecargo.SetNumber(5);

        if (chkAplicaDescuento) chkAplicaDescuento.SetChecked(false);
        if (spnPorcentajeDescuento) spnPorcentajeDescuento.SetNumber(0);
        if (spnDiaLimiteDescuento) spnDiaLimiteDescuento.SetNumber(5);
    }

    function onRowDblClick(s, e) {
        var id = s.GetRowKey(e.visibleIndex);
        cargarConcepto(id);
    }

    function setHiddenFieldValue(fieldId, value) {
        var field = document.querySelector('[id$="' + fieldId + '"]');
        if (field) {
            field.value = value;
        }
    }

    return {
        onToolbarConceptosClick: onToolbarConceptosClick,
        onRowDblClick: onRowDblClick,
        mostrarNuevoConcepto: mostrarNuevoConcepto,
        editarConceptoSeleccionado: editarConceptoSeleccionado,
        eliminarConceptoSeleccionado: eliminarConceptoSeleccionado
    };

})();

// Funciones globales para eventos del grid
function onToolbarConceptosClick(s, e) {
    ConceptosCuotaModule.onToolbarConceptosClick(s, e);
}

function onRowDblClick(s, e) {
    ConceptosCuotaModule.onRowDblClick(s, e);
}
