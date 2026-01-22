// =====================================================
// JAVASCRIPT PARA PROVEEDORES - ECOSISTEMA JELABBC
// =====================================================

$(document).ready(function () {
    'use strict';

    // Inicialización específica para proveedores
    initializeProveedores();

    function initializeProveedores() {
        // Configurar eventos específicos si es necesario
        console.log('Proveedores module initialized');

        // Aquí se pueden agregar validaciones específicas o comportamientos personalizados
        // para la gestión de proveedores
    }

    // Función para validar RFC de proveedores (ejemplo)
    window.validarRFCProveedor = function (rfc) {
        // Lógica de validación de RFC mexicano
        if (!rfc || rfc.length < 12 || rfc.length > 13) {
            return false;
        }

        // Validación básica de formato
        var rfcRegex = /^([A-ZÑ&]{3,4}) ?(?:- ?)?(\d{2}(?:0[1-9]|1[0-2])(?:0[1-9]|[12]\d|3[01])) ?(?:- ?)?([A-Z\d]{2})([A\d])$/;
        return rfcRegex.test(rfc.toUpperCase());
    };

    // Función para formatear teléfono
    window.formatearTelefonoProveedor = function (telefono) {
        if (!telefono) return '';

        // Remover todos los caracteres no numéricos
        var numeroLimpio = telefono.replace(/\D/g, '');

        // Formatear como (XXX) XXX-XXXX
        if (numeroLimpio.length === 10) {
            return '(' + numeroLimpio.substr(0, 3) + ') ' +
                   numeroLimpio.substr(3, 3) + '-' +
                   numeroLimpio.substr(6, 4);
        }

        return telefono;
    };
});