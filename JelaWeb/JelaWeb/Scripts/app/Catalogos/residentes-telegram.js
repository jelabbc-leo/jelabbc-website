// ============================================================================
// Módulo: Residentes Telegram
// Archivo: residentes-telegram.js
// ============================================================================

var ResidentesTelegramModule = (function () {
    'use strict';

    // ========================================================================
    // EVENTOS DEL TOOLBAR - GRID RESIDENTES
    // ========================================================================
    
    function onToolbarResidentesClick(s, e) {
        var itemName = e.item.name;

        switch (itemName) {
            case 'btnNuevoResidente':
                mostrarNuevoResidente();
                break;
            case 'btnEditarResidente':
                editarResidenteSeleccionado();
                break;
            case 'btnEnviarBlacklist':
                enviarABlacklistSeleccionado();
                break;
            case 'btnVerLogs':
                verLogsResidente();
                break;
        }
    }

    // ========================================================================
    // EVENTOS DEL TOOLBAR - GRID BLACKLIST
    // ========================================================================
    
    function onToolbarBlacklistClick(s, e) {
        var itemName = e.item.name;

        switch (itemName) {
            case 'btnRestaurar':
                restaurarResidenteSeleccionado();
                break;
            case 'btnVerRazon':
                verRazonBloqueo();
                break;
        }
    }

    // ========================================================================
    // FUNCIONES RESIDENTES
    // ========================================================================

    function mostrarNuevoResidente() {
        limpiarFormularioResidente();
        popupResidente.SetHeaderText('Nuevo Residente');
        popupResidente.Show();
    }


    function editarResidenteSeleccionado() {
        var focusedRowIndex = gridResidentes.GetFocusedRowIndex();
        if (focusedRowIndex < 0) {
            toastr.warning('Seleccione un residente para editar');
            return;
        }

        var id = gridResidentes.GetRowKey(focusedRowIndex);

        // Obtener datos de la fila
        var chatId = gridResidentes.GetRowValues(focusedRowIndex, 'ChatId');
        var username = gridResidentes.GetRowValues(focusedRowIndex, 'Username');
        var firstName = gridResidentes.GetRowValues(focusedRowIndex, 'FirstName');
        var lastName = gridResidentes.GetRowValues(focusedRowIndex, 'LastName');
        var estado = gridResidentes.GetRowValues(focusedRowIndex, 'EstadoResidente');
        var tipo = gridResidentes.GetRowValues(focusedRowIndex, 'TipoResidente');
        var creditos = gridResidentes.GetRowValues(focusedRowIndex, 'CreditosDisponibles');
        var limite = gridResidentes.GetRowValues(focusedRowIndex, 'LimiteTicketsMes');
        var activo = gridResidentes.GetRowValues(focusedRowIndex, 'Activo');

        // Cargar datos en el formulario
        txtChatId.SetNumber(chatId || 0);
        txtUsername.SetValue(username || '');
        txtFirstName.SetValue(firstName || '');
        txtLastName.SetValue(lastName || '');
        cmbEstadoResidente.SetValue(estado || 'activo');
        cmbTipoResidente.SetValue(tipo || 'standard');
        txtCreditos.SetNumber(creditos || 0);
        txtLimiteTickets.SetNumber(limite || 50);
        chkActivo.SetChecked(activo === true || activo === 1);

        // Actualizar hidden field
        setHiddenFieldValue('hfResidenteId', id);

        popupResidente.SetHeaderText('Editar Residente');
        popupResidente.Show();
    }

    function limpiarFormularioResidente() {
        txtChatId.SetNumber(0);
        txtUsername.SetValue('');
        txtFirstName.SetValue('');
        txtLastName.SetValue('');
        cmbEstadoResidente.SetValue('activo');
        cmbTipoResidente.SetValue('standard');
        txtCreditos.SetNumber(10);
        txtLimiteTickets.SetNumber(50);
        chkActivo.SetChecked(true);
        setHiddenFieldValue('hfResidenteId', '0');
    }


    // ========================================================================
    // FUNCIONES BLACKLIST
    // ========================================================================

    function enviarABlacklistSeleccionado() {
        var focusedRowIndex = gridResidentes.GetFocusedRowIndex();
        if (focusedRowIndex < 0) {
            toastr.warning('Seleccione un residente para enviar a blacklist');
            return;
        }

        var chatId = gridResidentes.GetRowValues(focusedRowIndex, 'ChatId');
        var username = gridResidentes.GetRowValues(focusedRowIndex, 'Username');
        var firstName = gridResidentes.GetRowValues(focusedRowIndex, 'FirstName');

        setHiddenFieldValue('hfChatIdBlacklist', chatId);

        // Limpiar formulario
        txtRazonBloqueo.SetValue('');
        chkPermanente.SetChecked(false);
        dteFechaLevantamiento.SetValue(null);
        txtNotasBlacklist.SetValue('');

        var nombre = (firstName || '') + ' (' + (username || chatId) + ')';
        popupBlacklist.SetHeaderText('Enviar a Blacklist: ' + nombre);
        popupBlacklist.Show();
    }

    function restaurarResidenteSeleccionado() {
        var focusedRowIndex = gridBlacklist.GetFocusedRowIndex();
        if (focusedRowIndex < 0) {
            toastr.warning('Seleccione un registro para restaurar');
            return;
        }

        var id = gridBlacklist.GetRowKey(focusedRowIndex);
        var username = gridBlacklist.GetRowValues(focusedRowIndex, 'Username');
        var chatId = gridBlacklist.GetRowValues(focusedRowIndex, 'ChatId');

        var nombre = username || chatId;

        if (confirm('¿Está seguro de restaurar al residente ' + nombre + '?\n\nEsto lo quitará de la blacklist y reactivará su cuenta.')) {
            $.ajax({
                type: 'POST',
                url: 'ResidentesTelegram.aspx/RestaurarResidente',
                data: JSON.stringify({ blacklistId: id }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    var result = response.d;
                    if (result.success) {
                        toastr.success(result.message);
                        gridResidentes.Refresh();
                        gridBlacklist.Refresh();
                    } else {
                        toastr.error(result.message);
                    }
                },
                error: function () {
                    toastr.error('Error al restaurar residente');
                }
            });
        }
    }

    function verRazonBloqueo() {
        var focusedRowIndex = gridBlacklist.GetFocusedRowIndex();
        if (focusedRowIndex < 0) {
            toastr.warning('Seleccione un registro para ver la razón');
            return;
        }

        var razon = gridBlacklist.GetRowValues(focusedRowIndex, 'RazonBloqueo');
        var username = gridBlacklist.GetRowValues(focusedRowIndex, 'Username');
        var bloqueadoPor = gridBlacklist.GetRowValues(focusedRowIndex, 'BloqueadoPor');
        var fechaBloqueo = gridBlacklist.GetRowValues(focusedRowIndex, 'FechaBloqueo');

        var mensaje = 'Usuario: ' + (username || 'N/A') + '\n';
        mensaje += 'Bloqueado por: ' + (bloqueadoPor || 'N/A') + '\n';
        mensaje += 'Fecha: ' + (fechaBloqueo || 'N/A') + '\n\n';
        mensaje += 'Razón:\n' + (razon || 'Sin razón especificada');

        alert(mensaje);
    }


    // ========================================================================
    // FUNCIONES LOGS
    // ========================================================================

    function verLogsResidente() {
        var focusedRowIndex = gridResidentes.GetFocusedRowIndex();
        if (focusedRowIndex < 0) {
            toastr.warning('Seleccione un residente para ver sus logs');
            return;
        }

        var chatId = gridResidentes.GetRowValues(focusedRowIndex, 'ChatId');
        window.open('/Views/Consultas/LogsValidacion.aspx?ChatId=' + chatId, '_blank');
    }

    // ========================================================================
    // EVENTO CAMBIO DE TAB
    // ========================================================================

    function onTabChanged(s) {
        var activeTabIndex = s.GetActiveTabIndex();
        console.log('Tab activo: ' + activeTabIndex);
    }

    // ========================================================================
    // HELPERS
    // ========================================================================

    function setHiddenFieldValue(fieldId, value) {
        var field = document.querySelector('[id$="' + fieldId + '"]');
        if (field) {
            field.value = value;
        }
    }

    // API pública
    return {
        onToolbarResidentesClick: onToolbarResidentesClick,
        onToolbarBlacklistClick: onToolbarBlacklistClick,
        onTabChanged: onTabChanged,
        mostrarNuevoResidente: mostrarNuevoResidente,
        editarResidenteSeleccionado: editarResidenteSeleccionado,
        enviarABlacklistSeleccionado: enviarABlacklistSeleccionado,
        restaurarResidenteSeleccionado: restaurarResidenteSeleccionado
    };

})();

// Funciones globales para eventos del grid
function onToolbarResidentesClick(s, e) {
    ResidentesTelegramModule.onToolbarResidentesClick(s, e);
}

function onToolbarBlacklistClick(s, e) {
    ResidentesTelegramModule.onToolbarBlacklistClick(s, e);
}

function onTabChanged(s, e) {
    ResidentesTelegramModule.onTabChanged(s, e);
}
