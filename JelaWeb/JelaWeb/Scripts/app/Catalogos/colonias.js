// Módulo de JavaScript para Colonias
// Patrón de módulo para evitar conflictos globales
var ColoniasModule = (function() {
    'use strict';

    // Variables privadas del módulo
    var currentColoniaId = 0;

    // Función privada para inicializar eventos
    function initEvents() {
        console.log('🔧 Inicializando eventos de Colonias');

        // Evento para nuevo registro
        if (typeof gridColonias !== 'undefined') {
            gridColonias.RowDblClick = function(s, e) {
                editarColonia(s.GetRowKey(e.visibleIndex));
            };
        }
    }

    // Función privada para mostrar popup de nueva colonia
    function mostrarNuevaColonia() {
        console.log('🔧 Mostrando popup para nueva colonia');
        currentColoniaId = 0;

        // Limpiar formulario
        if (typeof txtNombreColonia !== 'undefined') {
            txtNombreColonia.SetText('');
        }
        if (typeof txtCodigoPostal !== 'undefined') {
            txtCodigoPostal.SetText('');
        }
        if (typeof chkActivo !== 'undefined') {
            chkActivo.SetChecked(true);
        }

        // Mostrar popup
        if (typeof popupColonia !== 'undefined') {
            popupColonia.Show();
        }
    }

    // Función privada para editar colonia
    function editarColonia(id) {
        console.log('🔧 Editando colonia con ID:', id);
        currentColoniaId = id;

        // Aquí iría la lógica para cargar datos de la colonia
        // Por ahora, solo mostrar el popup
        if (typeof popupColonia !== 'undefined') {
            popupColonia.Show();
        }
    }

    // Función privada para guardar colonia
    function guardarColonia() {
        console.log('🔧 Guardando colonia...');

        // Validar campos requeridos
        if (typeof txtNombreColonia !== 'undefined' && !txtNombreColonia.GetText()) {
            toastr.warning('El nombre de la colonia es obligatorio');
            return;
        }

        // Aquí iría la lógica para guardar
        // Por ahora, solo cerrar popup y mostrar mensaje
        if (typeof popupColonia !== 'undefined') {
            popupColonia.Hide();
        }

        toastr.success('Colonia guardada exitosamente');

        // Refrescar grid
        if (typeof gridColonias !== 'undefined') {
            gridColonias.Refresh();
        }
    }

    // Función privada para eliminar colonia
    function eliminarColonia(id) {
        console.log('🔧 Eliminando colonia con ID:', id);

        if (confirm('¿Está seguro de eliminar esta colonia?')) {
            // Aquí iría la lógica para eliminar
            toastr.success('Colonia eliminada exitosamente');

            // Refrescar grid
            if (typeof gridColonias !== 'undefined') {
                gridColonias.Refresh();
            }
        }
    }

    // API pública del módulo
    return {
        init: function() {
            console.log('🚀 Inicializando módulo Colonias');
            initEvents();
        },

        mostrarNueva: mostrarNuevaColonia,
        editar: editarColonia,
        guardar: guardarColonia,
        eliminar: eliminarColonia,

        // Getters para uso externo
        getCurrentId: function() {
            return currentColoniaId;
        }
    };
})();

// Inicializar cuando el DOM esté listo
$(document).ready(function() {
    ColoniasModule.init();
});