// ========================================================================
// MÓDULO: Calendario de Reservaciones
// ========================================================================

function cargarCalendario() {
    if (gridCalendario) {
        gridCalendario.PerformCallback('cargar');
    }
}

$(document).ready(function() {
    if (typeof cboMes !== 'undefined' && typeof cboAnio !== 'undefined') {
        cargarCalendario();
    }
});
