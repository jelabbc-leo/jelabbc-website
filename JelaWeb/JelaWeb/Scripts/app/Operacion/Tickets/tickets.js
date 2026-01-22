// ====================================================================================
// MÓDULO 07 - TICKETS TIPO KLARNA + IA
// JavaScript para el módulo de tickets
// ====================================================================================

// Variables globales
var gridTickets;
var popupTicket;
var tabsTicket;
var hfIdTicket;

// Inicialización cuando el documento está listo
if (typeof ASPx !== 'undefined' && typeof ASPxClientControl !== 'undefined') {
    ASPxClientControl.GetControlCollection().ControlsInitialized.AddHandler(function() {
        if (typeof gridTickets !== 'undefined') {
            gridTickets = gridTickets || ASPxClientControl.GetControlCollection().GetByName('gridTickets');
        }
        if (typeof popupTicket !== 'undefined') {
            popupTicket = popupTicket || ASPxClientControl.GetControlCollection().GetByName('popupTicket');
        }
        if (typeof tabsTicket !== 'undefined') {
            tabsTicket = tabsTicket || ASPxClientControl.GetControlCollection().GetByName('tabsTicket');
        }
        if (typeof hfIdTicket !== 'undefined') {
            hfIdTicket = hfIdTicket || ASPxClientControl.GetControlCollection().GetByName('hfIdTicket');
        }
    });
}

// Limpiar formulario del popup
function LimpiarFormularioPopup() {
    try {
        // Limpiar campos del tab Resumen IA
        if (typeof txtAsuntoCorto !== 'undefined') txtAsuntoCorto.SetValue('');
        if (typeof txtResumenIA !== 'undefined') txtResumenIA.SetValue('');
        if (typeof txtCategoriaAsignada !== 'undefined') txtCategoriaAsignada.SetValue('');
        if (typeof txtSubcategoriaAsignada !== 'undefined') txtSubcategoriaAsignada.SetValue('');
        if (typeof txtSentimientoDetectado !== 'undefined') txtSentimientoDetectado.SetValue('');
        if (typeof txtPrioridadAsignada !== 'undefined') txtPrioridadAsignada.SetValue('');
        if (typeof txtUrgenciaAsignada !== 'undefined') txtUrgenciaAsignada.SetValue('');
        if (typeof chkPuedeResolverIA !== 'undefined') chkPuedeResolverIA.SetChecked(false);
        if (typeof txtRespuestaIA !== 'undefined') txtRespuestaIA.SetValue('');

        // Limpiar campos del tab Cliente
        if (typeof cmbCanal !== 'undefined') cmbCanal.SetValue(null);
        if (typeof cmbEstado !== 'undefined') cmbEstado.SetValue('Abierto');
        if (typeof txtNombreCompleto !== 'undefined') txtNombreCompleto.SetValue('');
        if (typeof txtEmailCliente !== 'undefined') txtEmailCliente.SetValue('');
        if (typeof txtTelefonoCliente !== 'undefined') txtTelefonoCliente.SetValue('');
        if (typeof cmbAgenteAsignado !== 'undefined') cmbAgenteAsignado.SetValue(null);

        // Limpiar campos del tab Mensaje Original
        if (typeof txtMensajeOriginal !== 'undefined') txtMensajeOriginal.SetValue('');

        // Limpiar campos del tab Resolución
        if (typeof dtFechaResolucion !== 'undefined') dtFechaResolucion.SetValue(null);
        if (typeof txtTiempoResolucion !== 'undefined') txtTiempoResolucion.SetValue(null);
        if (typeof txtSatisfaccionCliente !== 'undefined') txtSatisfaccionCliente.SetValue(null);
        if (typeof txtComentarioSatisfaccion !== 'undefined') txtComentarioSatisfaccion.SetValue('');

        // Limpiar campo de nuevo mensaje
        if (typeof txtNuevoMensaje !== 'undefined') txtNuevoMensaje.SetValue('');

        // Limpiar ID del ticket
        if (typeof hfIdTicket !== 'undefined') hfIdTicket.Set('IdTicket', '0');

        // Resetear tabs al primero
        if (typeof tabsTicket !== 'undefined') tabsTicket.SetActiveTabIndex(0);
    } catch (e) {
        console.error('Error al limpiar formulario:', e);
    }
}

// Validar formulario antes de guardar
function ValidarFormulario() {
    var errores = [];

    // Validar mensaje original
    if (typeof txtMensajeOriginal !== 'undefined') {
        var mensaje = txtMensajeOriginal.GetValue();
        if (!mensaje || mensaje.trim() === '') {
            errores.push('El mensaje original es requerido');
        }
    }

    // Validar canal
    if (typeof cmbCanal !== 'undefined') {
        var canal = cmbCanal.GetValue();
        if (!canal) {
            errores.push('Debe seleccionar un canal');
        }
    }

    // Validar nombre completo
    if (typeof txtNombreCompleto !== 'undefined') {
        var nombre = txtNombreCompleto.GetValue();
        if (!nombre || nombre.trim() === '') {
            errores.push('El nombre completo es requerido');
        }
    }

    if (errores.length > 0) {
        alert('Por favor, corrija los siguientes errores:\n\n' + errores.join('\n'));
        return false;
    }

    return true;
}

// Mostrar mensaje de éxito
function MostrarMensajeExito(mensaje) {
    if (typeof lblMensaje !== 'undefined') {
        lblMensaje.SetText(mensaje);
        lblMensaje.GetMainElement().style.color = '#388e3c';
        lblMensaje.SetVisible(true);
    } else {
        alert(mensaje);
    }
}

// Mostrar mensaje de error
function MostrarMensajeError(mensaje) {
    if (typeof lblMensaje !== 'undefined') {
        lblMensaje.SetText(mensaje);
        lblMensaje.GetMainElement().style.color = '#d32f2f';
        lblMensaje.SetVisible(true);
    } else {
        alert(mensaje);
    }
}

// Callback del popup cuando se completa
function OnPopupCallbackComplete(s, e) {
    try {
        // El servidor maneja la actualización del popup
        // Aquí solo podemos hacer ajustes visuales si es necesario
    } catch (ex) {
        console.error('Error en callback del popup:', ex);
    }
}

// Manejar cierre del popup
function OnPopupClose(s, e) {
    try {
        // Limpiar formulario al cerrar
        LimpiarFormularioPopup();
        
        // Recargar grid si es necesario
        if (typeof gridTickets !== 'undefined') {
            gridTickets.Refresh();
        }
    } catch (ex) {
        console.error('Error al cerrar popup:', ex);
    }
}

// Formatear fecha para mostrar
function FormatearFecha(fecha) {
    if (!fecha) return '';
    
    try {
        var date = new Date(fecha);
        var dia = ('0' + date.getDate()).slice(-2);
        var mes = ('0' + (date.getMonth() + 1)).slice(-2);
        var anio = date.getFullYear();
        var horas = ('0' + date.getHours()).slice(-2);
        var minutos = ('0' + date.getMinutes()).slice(-2);
        
        return dia + '/' + mes + '/' + anio + ' ' + horas + ':' + minutos;
    } catch (e) {
        return fecha;
    }
}

// Confirmar antes de resolver con IA
function ConfirmarResolucionIA() {
    return confirm('¿Está seguro de que desea que la IA intente resolver este ticket automáticamente?\n\n' +
                   'Esto generará una respuesta automática y cambiará el estado del ticket a "Resuelto".');
}

// Exportar tickets a Excel
function ExportarTickets() {
    try {
        if (typeof gridTickets !== 'undefined') {
            gridTickets.ExportToXlsx();
        }
    } catch (e) {
        console.error('Error al exportar tickets:', e);
        alert('Error al exportar los tickets');
    }
}

// Manejar clics en el toolbar del grid
// Asegurar que la función esté disponible globalmente
function onToolbarTicketsClick(s, e) {
    try {
        e.processOnServer = false;
        e.cancel = true;

        var itemName = e.item.name;

        switch (itemName) {
            case 'btnNuevoTicket':
                // Limpiar formulario y mostrar popup para nuevo ticket
                if (typeof LimpiarFormularioPopup === 'function') {
                    LimpiarFormularioPopup();
                }
                var popup = ASPxClientControl.GetControlCollection().GetByName('popupTicket');
                if (popup) {
                    popup.SetHeaderText('Nuevo Ticket');
                    popup.Show();
                } else {
                    // Si el control no está disponible, usar __doPostBack
                    __doPostBack('NUEVO_TICKET', '');
                }
                break;

            case 'btnVerTicket':
                // Ver ticket seleccionado
                var grid = ASPxClientControl.GetControlCollection().GetByName('gridTickets');
                if (!grid) {
                    alert('Error: El grid de tickets no está disponible');
                    return;
                }
                
                // Intentar obtener la fila enfocada
                var focusedIndex = grid.GetFocusedRowIndex();
                
                // Si no hay fila enfocada, intentar obtener la primera fila seleccionada
                if (focusedIndex < 0) {
                    var selectedIndices = grid.GetSelectedRowIndices();
                    if (selectedIndices && selectedIndices.length > 0) {
                        focusedIndex = selectedIndices[0];
                    }
                }
                
                if (focusedIndex < 0) {
                    alert('Por favor, seleccione un ticket haciendo clic en una fila del grid');
                    return;
                }
                
                // Obtener el ID del ticket usando GetRowKey
                try {
                    var ticketId = grid.GetRowKey(focusedIndex);
                    if (ticketId) {
                        __doPostBack('VER_TICKET', ticketId.toString());
                    } else {
                        // Fallback: usar GetRowValues si GetRowKey no funciona
                        grid.GetRowValues(focusedIndex, 'Id', function(values) {
                            var id = null;
                            if (values) {
                                // GetRowValues puede devolver el valor directamente o en un array
                                if (Array.isArray(values)) {
                                    id = values[0];
                                } else {
                                    id = values;
                                }
                            }
                            
                            if (id) {
                                __doPostBack('VER_TICKET', id.toString());
                            } else {
                                alert('Error: No se pudo obtener el ID del ticket seleccionado');
                            }
                        });
                    }
                } catch (ex) {
                    console.error('Error al obtener ID del ticket:', ex);
                    // Fallback: usar GetRowValues
                    grid.GetRowValues(focusedIndex, 'Id', function(values) {
                        var id = null;
                        if (values) {
                            if (Array.isArray(values)) {
                                id = values[0];
                            } else {
                                id = values;
                            }
                        }
                        
                        if (id) {
                            __doPostBack('VER_TICKET', id.toString());
                        } else {
                            alert('Error: No se pudo obtener el ID del ticket seleccionado. Por favor, asegúrese de que la fila esté seleccionada.');
                        }
                    });
                }
                break;

            case 'btnResolverConIA':
                // Resolver ticket con IA
                var grid = ASPxClientControl.GetControlCollection().GetByName('gridTickets');
                if (grid) {
                    var focusedIndex = grid.GetFocusedRowIndex();
                    if (focusedIndex < 0) {
                        alert('Por favor, seleccione un ticket para resolver');
                        return;
                    }
                    if (confirm('¿Está seguro de que desea que la IA intente resolver este ticket automáticamente?')) {
                        try {
                            var ticketId = grid.GetRowKey(focusedIndex);
                            if (ticketId) {
                                __doPostBack('RESOLVER_IA', ticketId.toString());
                            } else {
                                grid.GetRowValues(focusedIndex, 'Id', function(values) {
                                    var id = null;
                                    if (values) {
                                        if (Array.isArray(values)) {
                                            id = values[0];
                                        } else {
                                            id = values;
                                        }
                                    }
                                    if (id) {
                                        __doPostBack('RESOLVER_IA', id.toString());
                                    }
                                });
                            }
                        } catch (ex) {
                            console.error('Error al obtener ID del ticket:', ex);
                            alert('Error al obtener el ID del ticket');
                        }
                    }
                }
                break;

            case 'Refresh':
                // Refrescar grid
                var grid = ASPxClientControl.GetControlCollection().GetByName('gridTickets');
                if (grid) {
                    grid.Refresh();
                }
                break;

            case 'ExportToXlsx':
                // Exportar a Excel
                var grid = ASPxClientControl.GetControlCollection().GetByName('gridTickets');
                if (grid) {
                    grid.ExportToXlsx();
                }
                break;

            default:
                // Para otros comandos, permitir el comportamiento por defecto
                e.processOnServer = true;
                e.cancel = false;
                break;
        }
    } catch (ex) {
        console.error('Error en onToolbarTicketsClick:', ex);
        alert('Error al procesar la acción: ' + ex.message);
    }
}

// Asegurar disponibilidad global de la función
if (typeof window !== 'undefined') {
    window.onToolbarTicketsClick = onToolbarTicketsClick;
}

// Eventos del grid
function OnGridRowClick(s, e) {
    // Enfocar la fila al hacer clic
    try {
        s.SetFocusedRowIndex(e.visibleIndex);
    } catch (ex) {
        console.error('Error al enfocar fila:', ex);
    }
}

// Abrir ticket al hacer doble clic en la fila
function OnGridRowDblClick(s, e) {
    try {
        var grid = s;
        var visibleIndex = e.visibleIndex;
        
        // Obtener el ID del ticket usando GetRowKey
        try {
            var ticketId = grid.GetRowKey(visibleIndex);
            if (ticketId) {
                __doPostBack('VER_TICKET', ticketId.toString());
            } else {
                // Fallback: usar GetRowValues
                grid.GetRowValues(visibleIndex, 'Id', function(values) {
                    var id = null;
                    if (values) {
                        if (Array.isArray(values)) {
                            id = values[0];
                        } else {
                            id = values;
                        }
                    }
                    if (id) {
                        __doPostBack('VER_TICKET', id.toString());
                    }
                });
            }
        } catch (ex) {
            console.error('Error al obtener ID del ticket:', ex);
            alert('Error al abrir el ticket. Por favor, use el botón "Ver Ticket" del toolbar.');
        }
    } catch (ex) {
        console.error('Error al abrir ticket con doble clic:', ex);
        alert('Error al abrir el ticket. Por favor, use el botón "Ver Ticket" del toolbar.');
    }
}

// Inicialización cuando la página carga
document.addEventListener('DOMContentLoaded', function() {
    console.log('Módulo de Tickets tipo Klarna + IA cargado');
    
    // Configurar eventos adicionales si es necesario
    try {
        // Aquí se pueden agregar más inicializaciones
    } catch (e) {
        console.error('Error en inicialización:', e);
    }
});

