// Funciones para manejar el Loading Overlay Moderno
function mostrarLoading() {
    var overlay = document.getElementById('loadingOverlay');
    if (overlay) {
        // Asegurar que el contenedor del popup tenga posiciÃ³n relativa
        if (typeof popupDocumento !== 'undefined') {
            var popupContent = popupDocumento.GetContentElement();
            if (popupContent) {
                var contentDiv = popupContent.querySelector('.dxpc-content') || popupContent;
                if (contentDiv) {
                    contentDiv.style.position = 'relative';
                }
            }
        }
        
        // Mostrar overlay con animaciÃ³n
        overlay.style.display = 'block';
        overlay.classList.add('fade-in');
        overlay.classList.remove('fade-out');
        
        // Forzar repaint para la animaciÃ³n
        void overlay.offsetHeight;
    }
}

function ocultarLoading() {
    var overlay = document.getElementById('loadingOverlay');
    if (overlay) {
        overlay.classList.add('fade-out');
        overlay.classList.remove('fade-in');
        setTimeout(function () {
            overlay.style.display = 'none';
            overlay.classList.remove('fade-out');
        }, 300); // Esperar a que termine la animaciÃ³n
    }
}

function FiltrarDocumentos() {
    var tipo = glTipoDocumento.GetValue();   // obtiene el Id del tipo de documento
    var desde = dtDesde.GetDate();           // obtiene fecha desde
    var hasta = dtHasta.GetDate();           // obtiene fecha hasta

    // Construimos un parÃ¡metro para enviar al servidor
    var args = tipo + "|" + (desde ? desde.toISOString() : "") + "|" + (hasta ? hasta.toISOString() : "");

    gridDocumentos.PerformCallback(args);
}

// FunciÃ³n para manejar clics en el toolbar
function onToolbarDocumentosClick(s, e) {
    switch (e.item.name) {
        case 'btnNuevoDoc':
            // Limpiar tablas de sesiÃ³n al abrir popup
            LimpiarTablas();
            // Cambiar tÃ­tulo del popup
            if (typeof popupDocumento !== 'undefined') {
                popupDocumento.SetHeaderText('Nuevo Documento');
            }
            // Limpiar ID del documento
            if (typeof hfIdDocumento !== 'undefined') {
                hfIdDocumento.Set('Value', '');
            }
            popupDocumento.Show();
            break;

        case 'btnEditarDoc':
            // Obtener fila seleccionada
            var focusedRowIndex = s.GetFocusedRowIndex();
            if (focusedRowIndex < 0) {
                alert('Seleccione un documento para editar');
                return;
            }
            // Obtener ID del documento
            var idDocumento = s.GetRowKey(focusedRowIndex);
            if (!idDocumento) {
                alert('No se pudo obtener el ID del documento');
                return;
            }
            // Cargar datos del documento
            CargarDocumentoParaEdicion(idDocumento);
            break;

        case 'btnCancelarDoc':
            CancelarDocumento();
            break;

        case 'btnExportar':
            // Dispara postback al servidor
            __doPostBack('ExportarGrid', '');
            break;

        case 'btnActualizar':
            FiltrarDocumentos();
            break;

        //case 'btnLiberarDoc':
        //    LiberarTareas();
        //    break;
    }
}

function LimpiarTablas() {
    // Limpiar las tablas de sesiÃ³n mediante callback
    gridColonias.PerformCallback("LIMPIAR|");
    gridConceptos.PerformCallback("LIMPIAR|");
    
    // Limpiar controles del formulario
    if (typeof txtNoDocumento !== 'undefined') {
        txtNoDocumento.SetText('');
    }
    if (typeof txtReferencia !== 'undefined') {
        txtReferencia.SetText('');
    }
    if (typeof txtDocumentoRelacionado !== 'undefined') {
        txtDocumentoRelacionado.SetText('');
    }
    if (typeof txtComentarios !== 'undefined') {
        txtComentarios.SetText('');
    }
    if (typeof glSubEntidad !== 'undefined') {
        glSubEntidad.SetValue(null);
    }
    if (typeof dtFechaAsignacion !== 'undefined') {
        dtFechaAsignacion.SetValue(null);
    }
    if (typeof hfIdDocumento !== 'undefined') {
        hfIdDocumento.Set('Value', '');
    }
}

function CargarDocumentoParaEdicion(idDocumento) {
    // Guardar ID del documento en hidden field
    if (typeof hfIdDocumento !== 'undefined') {
        hfIdDocumento.Set('Value', idDocumento);
    }
    // Cambiar tÃ­tulo del popup
    if (typeof popupDocumento !== 'undefined') {
        popupDocumento.SetHeaderText('Editar Documento');
    }
    // Cargar datos mediante callback
    gridDocumentos.PerformCallback('CARGAR_EDICION|' + idDocumento);
}

//==============Funciones auxiliares globales =================

// FunciÃ³n auxiliar para obtener el nÃºmero de filas de forma segura
function getGridRowCount(grid) {
    try {
        if (!grid) return 0;
        
        if (typeof grid.GetVisibleRowCount === 'function') {
            return grid.GetVisibleRowCount();
        } else if (typeof grid.GetRowCount === 'function') {
            return grid.GetRowCount();
        } else if (grid.batchEditApi && typeof grid.batchEditApi.GetRowCount === 'function') {
            return grid.batchEditApi.GetRowCount();
        } else {
            // Intentar obtener desde el DOM como Ãºltimo recurso
            var mainElement = grid.GetMainElement ? grid.GetMainElement() : null;
            if (mainElement) {
                var rows = mainElement.querySelectorAll('tr[data-visible-index], tr[dx-visible-index], tr.dx-row');
                return rows ? rows.length : 0;
            }
        }
    } catch (err) {
        console.log("Error al obtener row count:", err);
    }
    return 0;
}

//==============Colonias =================

// FunciÃ³n para manejar clics en el toolbar
function onToolbarColoniasClick(s, e) {
    if (e.item.name === "btnColAdd") {
        // Agrega una nueva fila en blanco
        try {
            // s es el grid, necesitamos acceder a travÃ©s de gridColonias
            var grid = gridColonias;
            
            // Verificar que el grid estÃ© inicializado
            if (!grid || typeof grid.batchEditApi === 'undefined') {
                alert("Error: El grid no estÃ¡ inicializado correctamente");
                return;
            }
            
            // Obtener el API de Batch Edit
            var batchEditApi = grid.batchEditApi;
            
            // Guardar cualquier ediciÃ³n actual antes de agregar nueva fila
            var activeInput = document.activeElement;
            if (activeInput && activeInput.tagName === "INPUT") {
                try {
                    var cell = activeInput.closest('td');
                    if (cell) {
                        var row = cell.closest('tr');
                        if (row) {
                            var rowIndexAttr = row.getAttribute('data-visible-index') || row.getAttribute('dx-visible-index');
                            if (rowIndexAttr !== null && rowIndexAttr !== "") {
                                var currentRowIndex = parseInt(rowIndexAttr);
                                
                                // Obtener el fieldName de la celda actual
                                var fieldName = cell.getAttribute('data-field');
                                if (!fieldName) {
                                    var inputId = activeInput.id || "";
                                    if (inputId.indexOf("Clave") >= 0) fieldName = "Clave";
                                    else if (inputId.indexOf("MontoMin") >= 0) fieldName = "MontoMin";
                                    else if (inputId.indexOf("MontoMax") >= 0) fieldName = "MontoMax";
                                    else if (inputId.indexOf("Colonia") >= 0) fieldName = "Colonia";
                                }
                                
                                // Guardar el valor actual si existe
                                if (fieldName && activeInput.value !== null && activeInput.value !== undefined && activeInput.value !== "") {
                                    batchEditApi.SetCellValue(currentRowIndex, fieldName, activeInput.value);
                                }
                            }
                        }
                    }
                } catch (err) {
                    console.log("No se pudo guardar valor actual:", err);
                }
            }
            
            // Terminar cualquier ediciÃ³n actual y desenfocar cualquier fila
            try {
                batchEditApi.EndEdit();
                // Desenfocar cualquier fila que estÃ© enfocada
                var focusedRowIndex = grid.GetFocusedRowIndex ? grid.GetFocusedRowIndex() : -1;
                if (focusedRowIndex >= 0) {
                    try {
                        grid.SetFocusedRowIndex(-1);
                    } catch (err) {
                        console.log("No se pudo desenfocar fila:", err);
                    }
                }
            } catch (err) {
                console.log("No habÃ­a ediciÃ³n activa o error al terminar:", err);
            }
            
            // Obtener la columna Clave
            var colClave = grid.GetColumnByField("Clave");
            if (!colClave) {
                alert("Error: No se encontrÃ³ la columna 'Clave'");
                return;
            }
            
            // Obtener el Ã­ndice de la columna Clave
            var colIndex = colClave.index;
            if (colIndex === undefined || colIndex === null) {
                colIndex = grid.GetColumnIndex ? grid.GetColumnIndex("Clave") : 1;
                if (colIndex < 0) {
                    for (var i = 0; i < grid.GetColumnsCount(); i++) {
                        var col = grid.GetColumn(i);
                        if (col && col.fieldName === "Clave") {
                            colIndex = i;
                            break;
                        }
                    }
                }
            }
            var finalColIndex = (colIndex >= 0 && colIndex !== undefined && colIndex !== null) ? colIndex : 1;
            
            // Obtener el nÃºmero de filas visibles antes de agregar
            var visibleRowCount = getGridRowCount(grid);
            
            // Agregar nueva fila usando el mÃ©todo correcto
            batchEditApi.AddNewRow();
            
            // Esperar un momento para que se agregue la fila y el DOM se actualice
            setTimeout(function () {
                try {
                    // Obtener el nuevo nÃºmero de filas despuÃ©s de agregar
                    var newRowCount = getGridRowCount(grid);
                    // El Ã­ndice de la nueva fila serÃ¡ newRowCount - 1 (Ãºltima fila)
                    var rowIndex = newRowCount > 0 ? newRowCount - 1 : 0;
                    
                    console.log("Nueva fila agregada - rowIndex:", rowIndex, "newRowCount:", newRowCount);
                    
                    // Inicializar la fila nueva con valores temporales para evitar que se elimine
                    try {
                        var tempId = newRowCount; // Usar el nÃºmero de fila como ID temporal
                        batchEditApi.SetCellValue(rowIndex, "Id", tempId);
                        // Establecer un valor temporal en Clave (vacÃ­o pero no null) para que la fila no se considere vacÃ­a
                        batchEditApi.SetCellValue(rowIndex, "Clave", "");
                        console.log("Fila nueva inicializada con Id:", tempId, "en Ã­ndice:", rowIndex);
                    } catch (err) {
                        console.error("Error al establecer valores temporales:", err);
                    }
                    
                    // Esperar un momento mÃ¡s antes de establecer el foco y editar
                    setTimeout(function() {
                        try {
                            // Asegurar que el foco estÃ© en la nueva fila ANTES de iniciar ediciÃ³n
                            grid.SetFocusedRowIndex(rowIndex);
                            
                            // PequeÃ±o delay adicional para asegurar que el foco se estableciÃ³
                            setTimeout(function() {
                                console.log("Iniciando ediciÃ³n en nueva fila - rowIndex:", rowIndex, "colIndex:", finalColIndex);
                                batchEditApi.StartEdit(rowIndex, finalColIndex);
                            }, 50);
                        } catch (err) {
                            console.error("Error al establecer foco o iniciar ediciÃ³n:", err);
                            // Intentar iniciar ediciÃ³n de todas formas
                            try {
                                batchEditApi.StartEdit(rowIndex, finalColIndex);
                            } catch (err2) {
                                console.error("Error al iniciar ediciÃ³n (fallback):", err2);
                            }
                        }
                    }, 100);
                } catch (err) {
                    console.error("Error al iniciar ediciÃ³n:", err);
                    // Intentar de nuevo con un delay mayor
                    setTimeout(function() {
                        try {
                            var newRowCount = getGridRowCount(grid);
                            var rowIndex = newRowCount > 0 ? newRowCount - 1 : 0;
                            
                            // Inicializar la fila nueva con valores temporales
                            try {
                                var tempId = newRowCount;
                                batchEditApi.SetCellValue(rowIndex, "Id", tempId);
                                batchEditApi.SetCellValue(rowIndex, "Clave", "");
                            } catch (err) {
                                console.log("No se pudo establecer valores temporales (segundo intento):", err);
                            }
                            
                            // Asegurar que el foco estÃ© en la nueva fila
                            try {
                                grid.SetFocusedRowIndex(rowIndex);
                                setTimeout(function() {
                                    console.log("Segundo intento - rowIndex:", rowIndex, "colIndex:", finalColIndex);
                                    batchEditApi.StartEdit(rowIndex, finalColIndex);
                                }, 100);
                            } catch (err) {
                                console.log("No se pudo establecer foco (segundo intento):", err);
                                // Intentar iniciar ediciÃ³n de todas formas
                                batchEditApi.StartEdit(rowIndex, finalColIndex);
                            }
                        } catch (err2) {
                            console.error("Error al iniciar ediciÃ³n (segundo intento):", err2);
                            alert("Error al iniciar ediciÃ³n: " + err2.message);
                        }
                    }, 400);
                }
            }, 350);
        } catch (err) {
            console.error("Error en onToolbarColoniasClick:", err);
            alert("Error al agregar fila: " + err.message);
        }
    }
    else if (e.item.name === "btnColDelete") {
        // Eliminar fila seleccionada
        var grid = gridColonias;
        var focusedRowIndex = grid.GetFocusedRowIndex ? grid.GetFocusedRowIndex() : -1;
        if (focusedRowIndex >= 0) {
            try {
                grid.batchEditApi.DeleteRow(focusedRowIndex);
            } catch (err) {
                console.error("Error al eliminar fila:", err);
                alert("Error al eliminar fila: " + err.message);
            }
        } else {
            alert("Seleccione una fila para eliminar");
        }
    }
}

// FunciÃ³n de inicializaciÃ³n del grid
function initGridColonias(s, e) {
    // Configurar eventos de teclado para manejar Enter y flecha abajo
    s.GetMainElement().addEventListener("keydown", function (evt) {
        // Solo procesar si es Enter, Tab o ArrowDown
        if (evt.key !== "Enter" && evt.key !== "Tab" && evt.key !== "ArrowDown" && evt.keyCode !== 13 && evt.keyCode !== 9 && evt.keyCode !== 40) {
            return;
        }
        
        var activeInput = document.activeElement;
        var colClave = s.GetColumnByField("Clave");
        var colMontoMin = s.GetColumnByField("MontoMin");
        var colClaveIndex = colClave ? colClave.index : 1;
        
        // Verificar si el input activo estÃ¡ en la columna Clave
        if (activeInput && activeInput.tagName === "INPUT") {
            try {
                var cell = activeInput.closest('td');
                if (cell) {
                    var row = cell.closest('tr');
                    var rowIndex = -1;
                    var fieldName = null;
                    
                    // Obtener el Ã­ndice de la fila
                    if (row) {
                        var rowIndexAttr = row.getAttribute('data-visible-index');
                        if (rowIndexAttr !== null && rowIndexAttr !== "") {
                            rowIndex = parseInt(rowIndexAttr);
                        } else {
                            // Intentar obtener desde el atributo dx-visible-index
                            rowIndexAttr = row.getAttribute('dx-visible-index');
                            if (rowIndexAttr !== null && rowIndexAttr !== "") {
                                rowIndex = parseInt(rowIndexAttr);
                            } else {
                                // Como Ãºltimo recurso, intentar obtener desde el grid
                                try {
                                    rowIndex = s.GetFocusedRowIndex();
                                } catch (err) {
                                    console.log("No se pudo obtener rowIndex");
                                }
                            }
                        }
                    }
                    
                    // Obtener el nombre del campo desde el DOM
                    // Buscar en el atributo data-field o en el id del input
                    fieldName = cell.getAttribute('data-field');
                    if (!fieldName) {
                        // Intentar desde el input
                        var inputId = activeInput.id || "";
                        if (inputId.indexOf("Clave") >= 0) {
                            fieldName = "Clave";
                        } else if (inputId.indexOf("MontoMin") >= 0) {
                            fieldName = "MontoMin";
                        } else if (inputId.indexOf("Colonia") >= 0) {
                            fieldName = "Colonia";
                        } else {
                            // Intentar obtener desde el Ã­ndice de celda
                            var colIndex = cell.cellIndex;
                            if (colIndex >= 0) {
                                var column = s.GetColumn(colIndex);
                                if (column) {
                                    fieldName = column.fieldName;
                                }
                            }
                        }
                    }
                    
                    // Si estamos editando la columna Clave
                    if (fieldName === "Clave" && rowIndex >= 0) {
                        evt.preventDefault();
                        evt.stopPropagation();
                        
                        // Obtener el valor directamente del input
                        var claveValue = activeInput.value;
                        
                        console.log("Clave detectada - rowIndex:", rowIndex, "claveValue:", claveValue);
                        
                        if (claveValue && claveValue.toString().trim() !== "") {
                            var tempClaveValue = claveValue.toString().trim();
                            
                            // Guardar el valor en la celda
                            try {
                                s.batchEditApi.SetCellValue(rowIndex, "Clave", tempClaveValue);
                                console.log("Valor guardado en celda:", tempClaveValue);
                            } catch (err) {
                                console.error("Error al establecer valor:", err);
                            }
                            
                            // Ejecutar bÃºsqueda inmediatamente
                            var args = "BUSCAR_CLAVE|" + tempClaveValue + "|" + rowIndex;
                            console.log("Ejecutando bÃºsqueda desde keydown con args:", args);
                            
                            try {
                                s.PerformCallback(args);
                                console.log("Callback ejecutado correctamente");
                            } catch (err) {
                                console.error("Error al ejecutar PerformCallback:", err);
                                alert("Error al ejecutar bÃºsqueda: " + err.message);
                            }
                            
                            // Terminar ediciÃ³n despuÃ©s de un pequeÃ±o delay
                            setTimeout(function() {
                                try {
                                    s.batchEditApi.EndEdit();
                                    
                                    // Si es Enter o ArrowDown (no Tab), agregar nueva fila
                                    if ((evt.key === "Enter" || evt.key === "ArrowDown" || evt.keyCode === 13 || evt.keyCode === 40) && evt.key !== "Tab" && evt.keyCode !== 9) {
                                        setTimeout(function () {
                                            try {
                                                // Agregar nueva fila primero
                                                s.batchEditApi.AddNewRow();
                                                
                                                setTimeout(function() {
                                                    try {
                                                        var newRowCount = getGridRowCount(s);
                                                        var newRowIndex = newRowCount > 0 ? newRowCount - 1 : 0;
                                                        
                                                        // Inicializar la fila nueva con valores temporales para evitar que se elimine
                                                        try {
                                                            var tempId = newRowCount;
                                                            s.batchEditApi.SetCellValue(newRowIndex, "Id", tempId);
                                                            s.batchEditApi.SetCellValue(newRowIndex, "Clave", "");
                                                        } catch (err) {
                                                            console.log("No se pudo establecer valores temporales:", err);
                                                        }
                                                        
                                                        // Asegurar que el foco estÃ© en la nueva fila
                                                        try {
                                                            s.SetFocusedRowIndex(newRowIndex);
                                                        } catch (err) {
                                                            console.log("No se pudo establecer foco:", err);
                                                        }
                                                        
                                                        s.batchEditApi.StartEdit(newRowIndex, colClaveIndex);
                                                        console.log("Nueva fila iniciada en Ã­ndice:", newRowIndex);
                                                    } catch (err) {
                                                        console.error("Error al iniciar nueva fila:", err);
                                                    }
                                                }, 200);
                                            } catch (err) {
                                                console.error("Error al agregar nueva fila:", err);
                                            }
                                        }, 100);
                                    }
                                } catch (err) {
                                    console.error("Error al terminar ediciÃ³n:", err);
                                }
                            }, 100);
                        } else {
                            console.log("Valor de clave vacÃ­o o invÃ¡lido");
                            try {
                                s.batchEditApi.EndEdit();
                            } catch (err) {
                                console.error("Error al terminar ediciÃ³n:", err);
                            }
                        }
                        
                        return;
                    }
                    
                    // Si estamos editando MontoMin
                    if (fieldName === "MontoMin" && rowIndex >= 0) {
                        if (evt.key === "Enter" || evt.key === "ArrowDown" || evt.keyCode === 13 || evt.keyCode === 40) {
                            evt.preventDefault();
                            evt.stopPropagation();
                            
                            // Guardar el valor actual antes de terminar ediciÃ³n
                            var montoValue = activeInput.value;
                            if (montoValue !== null && montoValue !== undefined && montoValue !== "") {
                                try {
                                    s.batchEditApi.SetCellValue(rowIndex, "MontoMin", montoValue);
                                } catch (err) {
                                    console.error("Error al guardar MontoMin:", err);
                                }
                            }
                            
                            // Terminar ediciÃ³n actual
                            s.batchEditApi.EndEdit();
                            
                            // Esperar a que termine la ediciÃ³n antes de agregar nueva fila
                            setTimeout(function () {
                                try {
                                    // Agregar nueva fila
                                    s.batchEditApi.AddNewRow();
                                    
                                    // Esperar a que se agregue la fila
                                    setTimeout(function () {
                                        try {
                                            var newRowCount = getGridRowCount(s);
                                            var newRowIndex = newRowCount > 0 ? newRowCount - 1 : 0;
                                            
                                            // Inicializar la fila nueva con valores temporales para evitar que se elimine
                                            try {
                                                var tempId = newRowCount;
                                                s.batchEditApi.SetCellValue(newRowIndex, "Id", tempId);
                                                s.batchEditApi.SetCellValue(newRowIndex, "Clave", "");
                                            } catch (err) {
                                                console.log("No se pudo establecer valores temporales:", err);
                                            }
                                            
                                            // Asegurar que el foco estÃ© en la nueva fila
                                            try {
                                                s.SetFocusedRowIndex(newRowIndex);
                                            } catch (err) {
                                                console.log("No se pudo establecer foco:", err);
                                            }
                                            
                                            console.log("Agregando nueva fila en Ã­ndice:", newRowIndex);
                                            s.batchEditApi.StartEdit(newRowIndex, colClaveIndex);
                                        } catch (err) {
                                            console.error("Error al iniciar ediciÃ³n en nueva fila:", err);
                                        }
                                    }, 250);
                                } catch (err) {
                                    console.error("Error al agregar nueva fila:", err);
                                }
                            }, 200);
                            return;
                        }
                    }
                }
            } catch (err) {
                console.error("Error en keydown handler:", err);
            }
        }
    });
}

function gridColonias_EndCallback(s, e) {
    // Manejar respuesta del callback
    if (s.cpEditIndex !== undefined && s.cpEditIndex >= 0) {
        var rowIndex = s.cpEditIndex;
        var colMontoMin = s.GetColumnByField("MontoMin");

        // Si existe la colonia, actualizar valores y abrir MontoMin
        if (s.cpExiste === true) {
            // Actualizar valores en la fila
            s.batchEditApi.SetCellValue(rowIndex, "Clave", s.cpClave);
            s.batchEditApi.SetCellValue(rowIndex, "Colonia", s.cpColonia);

            // Abrir celda MontoMin para ediciÃ³n
            setTimeout(function () {
                s.batchEditApi.StartEdit(rowIndex, colMontoMin.index);
            }, 100);
        } else if (s.cpExiste === false) {
            // Si no existe, mostrar mensaje y mantener en Clave
            alert("La clave ingresada no existe");
            var colClave = s.GetColumnByField("Clave");
            setTimeout(function () {
                s.batchEditApi.StartEdit(rowIndex, colClave.index);
            }, 100);
        }

        // Limpiar propiedades
        delete s.cpEditIndex;
        delete s.cpClave;
        delete s.cpColonia;
        delete s.cpExiste;
    }

    if (s.cpError) {
        alert("Error: " + s.cpError);
        delete s.cpError;
    }
}

function onEndEditingColonias(s, e) {
    try {
        var column = e.focusedColumn;
        var rowIndex = e.visibleIndex;
        
        if (!column) {
            console.log("onEndEditingColonias: column es null");
            return;
        }
        
        console.log("onEndEditingColonias disparado - Column:", column.fieldName, "RowIndex:", rowIndex);
        
        // Obtener el valor - intentar mÃºltiples formas
        var value = null;
        
        // Primero intentar desde el evento
        if (e.value !== undefined && e.value !== null && e.value !== "") {
            value = e.value;
            console.log("Valor obtenido de e.value:", value);
        } else if (e.newValue !== undefined && e.newValue !== null && e.newValue !== "") {
            value = e.newValue;
            console.log("Valor obtenido de e.newValue:", value);
        } else if (e.oldValue !== undefined && e.oldValue !== null && e.oldValue !== "") {
            // A veces el valor nuevo estÃ¡ en oldValue (confuso pero puede pasar)
            value = e.oldValue;
            console.log("Valor obtenido de e.oldValue:", value);
        }
        
        // Si aÃºn no tenemos valor, obtenerlo de la celda despuÃ©s de un delay
        if (!value || value === null || value === "") {
            setTimeout(function() {
                try {
                    var cellValue = s.batchEditApi.GetCellValue(rowIndex, column.fieldName);
                    console.log("Valor obtenido de GetCellValue despuÃ©s de delay:", cellValue);
                    if (cellValue && cellValue.toString().trim() !== "") {
                        procesarCambioColumna(s, column, rowIndex, cellValue);
                    }
                } catch (err) {
                    console.error("Error al obtener valor de celda:", err);
                }
            }, 150);
            
            // TambiÃ©n intentar procesar con el valor que tenemos (aunque sea vacÃ­o)
            if (value !== null) {
                procesarCambioColumna(s, column, rowIndex, value);
            }
            return;
        }

        // Procesar el cambio
        procesarCambioColumna(s, column, rowIndex, value);
    } catch (err) {
        console.error("Error en onEndEditingColonias:", err);
    }
}

// FunciÃ³n auxiliar para procesar el cambio de columna
function procesarCambioColumna(s, column, rowIndex, value) {
    if (!column) {
        console.log("procesarCambioColumna: column es null");
        return;
    }
    
    if (!value || value === null || value === undefined) {
        console.log("procesarCambioColumna: value es null o undefined para columna", column.fieldName);
        return;
    }
    
    var valueStr = value.toString().trim();
    if (valueStr === "") {
        console.log("procesarCambioColumna: value estÃ¡ vacÃ­o para columna", column.fieldName);
        return;
    }
    
    if (column.fieldName === "Clave") {
        // Al terminar de editar Clave, buscar la colonia
        var args = "BUSCAR_CLAVE|" + valueStr + "|" + rowIndex;
        console.log("Ejecutando bÃºsqueda de clave con args:", args);
        try {
            s.PerformCallback(args);
        } catch (err) {
            console.error("Error al ejecutar PerformCallback:", err);
            alert("Error al ejecutar bÃºsqueda: " + err.message);
        }
    }
    else if (column.fieldName === "Colonia") {
        // Al cambiar el dropdown de Colonia, buscar por descripciÃ³n
        var args = "BUSCAR_DESCRIPCION|" + valueStr + "|" + rowIndex;
        console.log("Ejecutando bÃºsqueda de descripciÃ³n con args:", args);
        try {
            s.PerformCallback(args);
        } catch (err) {
            console.error("Error al ejecutar PerformCallback:", err);
            alert("Error al ejecutar bÃºsqueda: " + err.message);
        }
    }
}

// Evento cuando se inicia la ediciÃ³n de una celda
function onStartEditingColonias(s, e) {
    var column = e.focusedColumn;
    var rowIndex = e.visibleIndex;

    // Si estamos editando MontoMin y la Colonia estÃ¡ vacÃ­a, no permitir ediciÃ³n
    if (column && column.fieldName === "MontoMin") {
        var colonia = s.batchEditApi.GetCellValue(rowIndex, "Colonia");
        if (!colonia || colonia.toString().trim() === "") {
            e.cancel = true;
            alert("Debe seleccionar una colonia primero");
            var colClave = s.GetColumnByField("Clave");
            s.batchEditApi.StartEdit(rowIndex, colClave.index);
        }
    }
    
    // Si estamos editando Clave, agregar listener al editor para capturar cambios
    if (column && column.fieldName === "Clave") {
        setTimeout(function() {
            try {
                var editor = e.editor;
                if (!editor) {
                    console.warn("Editor no disponible en onStartEditingColonias");
                    return;
                }
                
                // Obtener el elemento de entrada
                var inputElement = null;
                try {
                    if (editor.GetInputElement) {
                        inputElement = editor.GetInputElement();
                    }
                } catch (err) {
                    console.log("GetInputElement no disponible, intentando otra forma");
                }
                
                if (!inputElement) {
                    try {
                        var editorElement = editor.GetMainElement ? editor.GetMainElement() : null;
                        if (editorElement) {
                            inputElement = editorElement.querySelector('input') || editorElement;
                        }
                    } catch (err) {
                        console.log("No se pudo obtener elemento del editor");
                    }
                }
                
                if (inputElement) {
                    // FunciÃ³n para ejecutar bÃºsqueda
                    var ejecutarBusqueda = function() {
                        try {
                            var value = null;
                            
                            // Intentar obtener el valor directamente del input
                            if (inputElement && inputElement.value !== undefined && inputElement.value !== null && inputElement.value !== "") {
                                value = inputElement.value;
                            } else if (editor && typeof editor.GetValue === 'function') {
                                value = editor.GetValue();
                            } else if (editor && typeof editor.GetText === 'function') {
                                value = editor.GetText();
                            }
                            
                            if (value && value.toString().trim() !== "") {
                                var valueStr = value.toString().trim();
                                
                                // Guardar el valor en la celda primero
                                try {
                                    s.batchEditApi.SetCellValue(rowIndex, "Clave", valueStr);
                                } catch (err) {
                                    console.error("Error al establecer valor:", err);
                                }
                                
                                // Ejecutar bÃºsqueda
                                var args = "BUSCAR_CLAVE|" + valueStr + "|" + rowIndex;
                                console.log("Ejecutando bÃºsqueda desde editor listener con args:", args);
                                s.PerformCallback(args);
                                return true;
                            }
                        } catch (err) {
                            console.error("Error al ejecutar bÃºsqueda desde listener:", err);
                        }
                        return false;
                    };
                    
                    // Listener para Enter - con mayor prioridad y captura
                    var keydownHandler = function(evt) {
                        if (evt.key === "Enter" || evt.keyCode === 13) {
                            evt.preventDefault();
                            evt.stopPropagation();
                            ejecutarBusqueda();
                        }
                    };
                    inputElement.addEventListener("keydown", keydownHandler, true); // Usar capture phase
                    
                    // Listener para cuando se pierde el foco (blur) - mÃ¡s confiable
                    var blurHandler = function(evt) {
                        setTimeout(function() {
                            ejecutarBusqueda();
                        }, 50);
                    };
                    inputElement.addEventListener("blur", blurHandler, true);
                    
                    // TambiÃ©n escuchar el evento change
                    var changeHandler = function(evt) {
                        // No ejecutar bÃºsqueda en cada cambio, solo cuando se termine de editar
                    };
                    inputElement.addEventListener("change", changeHandler);
                } else {
                    console.warn("No se pudo obtener el elemento de entrada del editor para Clave");
                }
            } catch (err) {
                console.error("Error al agregar listener al editor:", err);
            }
        }, 150);
    }
}

//==============Conceptos =================

function onToolbarConceptosClick(s, e) {
    if (e.item.name === "btnConceptoAdd") {
        // Agrega una nueva fila en blanco
        try {
            // s es el grid, necesitamos acceder a travÃ©s de gridConceptos
            var grid = gridConceptos;
            
            // Verificar que el grid estÃ© inicializado
            if (!grid || typeof grid.batchEditApi === 'undefined') {
                alert("Error: El grid no estÃ¡ inicializado correctamente");
                return;
            }
            
            // Obtener el API de Batch Edit
            var batchEditApi = grid.batchEditApi;
            
            // Terminar cualquier ediciÃ³n actual
            if (batchEditApi) {
                batchEditApi.EndEdit();
            }
            
            // Obtener la columna Clave
            var colClave = grid.GetColumnByField("Clave");
            if (!colClave) {
                alert("Error: No se encontrÃ³ la columna 'Clave'");
                return;
            }
            
            // Obtener el nÃºmero de filas visibles usando el mÃ©todo correcto
            var visibleRowCount = getGridRowCount(grid);
            
            // Agregar nueva fila usando el mÃ©todo correcto
            batchEditApi.AddNewRow();
            
            // Esperar un momento para que se agregue la fila
            setTimeout(function () {
                try {
                    // Obtener el nuevo nÃºmero de filas
                    var newRowCount = getGridRowCount(grid);
                    // Iniciar ediciÃ³n en la Ãºltima fila (la nueva)
                    var rowIndex = newRowCount > 0 ? newRowCount - 1 : 0;
                    batchEditApi.StartEdit(rowIndex, colClave.index);
                } catch (err) {
                    console.error("Error al iniciar ediciÃ³n:", err);
                    alert("Error al iniciar ediciÃ³n: " + err.message);
                }
            }, 150);
        } catch (err) {
            console.error("Error en onToolbarConceptosClick:", err);
            alert("Error al agregar fila: " + err.message);
        }
    }
    else if (e.item.name === "btnConceptoDel") {
        // Eliminar fila seleccionada
        var grid = gridConceptos;
        var focusedRowIndex = grid.GetFocusedRowIndex ? grid.GetFocusedRowIndex() : -1;
        if (focusedRowIndex >= 0) {
            try {
                grid.batchEditApi.DeleteRow(focusedRowIndex);
            } catch (err) {
                console.error("Error al eliminar fila:", err);
                alert("Error al eliminar fila: " + err.message);
            }
        } else {
            alert("Seleccione una fila para eliminar");
        }
    }
}

// FunciÃ³n para manejar flecha abajo y agregar fila nueva
function initGridConceptos(s, e) {
    // Configurar eventos de teclado para manejar Enter y flecha abajo
    s.GetMainElement().addEventListener("keydown", function (evt) {
        var focusedRowIndex = s.GetFocusedRowIndex();
        var focusedColumn = s.GetFocusedColumn();
        var colClave = s.GetColumnByField("Clave");
        var colCosto = s.GetColumnByField("CostoUnitario");

        // Si estamos editando CostoUnitario y presionamos Enter o flecha abajo
        if (focusedColumn && focusedColumn.fieldName === "CostoUnitario") {
            if (evt.key === "Enter" || evt.key === "ArrowDown") {
                evt.preventDefault();
                // Terminar ediciÃ³n actual
                s.batchEditApi.EndEdit();
                // Agregar nueva fila y enfocar en Clave
                setTimeout(function () {
                    var newRowCount = getGridRowCount(s);
                    var newRowIndex = newRowCount > 0 ? newRowCount - 1 : 0;
                    s.batchEditApi.StartEdit(newRowIndex, colClave.index);
                }, 100);
                return;
            }
        }

        // Si estamos en la Ãºltima fila y presionamos Enter o flecha abajo
        var rowCount = getGridRowCount(s);
        if ((evt.key === "Enter" || evt.key === "ArrowDown") && focusedRowIndex >= 0 && focusedRowIndex === rowCount - 1) {
            // Solo si no estamos en CostoUnitario (ya se maneja arriba)
            if (!focusedColumn || focusedColumn.fieldName !== "CostoUnitario") {
                evt.preventDefault();
                // Terminar ediciÃ³n actual
                s.batchEditApi.EndEdit();
                // Agregar nueva fila y enfocar en Clave
                setTimeout(function () {
                    var newRowCount = getGridRowCount(s);
                    var newRowIndex = newRowCount > 0 ? newRowCount - 1 : 0;
                    s.batchEditApi.StartEdit(newRowIndex, colClave.index);
                }, 100);
            }
        }
    });
}

function gridConceptos_EndCallback(s, e) {
    // Manejar respuesta del callback
    if (s.cpEditIndex !== undefined && s.cpEditIndex >= 0) {
        var rowIndex = s.cpEditIndex;
        var colCosto = s.GetColumnByField("CostoUnitario");

        // Si existe el concepto, actualizar valores y abrir CostoUnitario
        if (s.cpExiste === true) {
            // Actualizar valores en la fila
            s.batchEditApi.SetCellValue(rowIndex, "Clave", s.cpClave);
            s.batchEditApi.SetCellValue(rowIndex, "Descripcion", s.cpDescripcion);
            if (s.cpCosto !== undefined) {
                s.batchEditApi.SetCellValue(rowIndex, "CostoUnitario", s.cpCosto);
            }

            // Abrir celda CostoUnitario para ediciÃ³n
            setTimeout(function () {
                s.batchEditApi.StartEdit(rowIndex, colCosto.index);
            }, 100);
        } else if (s.cpExiste === false) {
            // Si no existe, mostrar mensaje y mantener en Clave
            alert("La clave ingresada no existe");
            var colClave = s.GetColumnByField("Clave");
            setTimeout(function () {
                s.batchEditApi.StartEdit(rowIndex, colClave.index);
            }, 100);
        }

        // Limpiar propiedades
        delete s.cpEditIndex;
        delete s.cpClave;
        delete s.cpDescripcion;
        delete s.cpCosto;
        delete s.cpExiste;
    }

    if (s.cpError) {
        alert("Error: " + s.cpError);
        delete s.cpError;
    }
}

function onEndEditingConceptos(s, e) {
    var column = e.focusedColumn;
    var rowIndex = e.visibleIndex;
    var value = e.value;

    if (column && column.fieldName === "Clave") {
        // Al terminar de editar Clave, buscar el concepto
        if (value && value.toString().trim() !== "") {
            var args = "BUSCAR_CLAVE|" + value.toString().trim() + "|" + rowIndex;
            s.PerformCallback(args);
        }
    }
    else if (column && column.fieldName === "Descripcion") {
        // Al cambiar el dropdown de Descripcion, buscar por descripciÃ³n
        if (value && value.toString().trim() !== "") {
            var args = "BUSCAR_DESCRIPCION|" + value.toString().trim() + "|" + rowIndex;
            s.PerformCallback(args);
        }
    }
    else if (column && column.fieldName === "CostoUnitario") {
        // Al terminar de editar CostoUnitario, el evento de teclado se maneja en initGridConceptos
        // AquÃ­ solo guardamos el valor
    }
}

// Evento cuando se inicia la ediciÃ³n de una celda
function onStartEditingConceptos(s, e) {
    var column = e.focusedColumn;
    var rowIndex = e.visibleIndex;

    // Si estamos editando CostoUnitario y la Descripcion estÃ¡ vacÃ­a, no permitir ediciÃ³n
    if (column && column.fieldName === "CostoUnitario") {
        var descripcion = s.batchEditApi.GetCellValue(rowIndex, "Descripcion");
        if (!descripcion || descripcion.toString().trim() === "") {
            e.cancel = true;
            alert("Debe seleccionar un concepto primero");
            var colClave = s.GetColumnByField("Clave");
            s.batchEditApi.StartEdit(rowIndex, colClave.index);
        }
    }
}

