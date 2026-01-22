// ============================================================================
// Módulo: Residentes
// Archivo: residentes.js
// Descripción: CRUD de residentes del condominio
// ============================================================================

var ResidentesModule = (function () {
    'use strict';

    // ========================================================================
    // EVENTOS DEL TOOLBAR
    // ========================================================================
    
    function onToolbarResidentesClick(s, e) {
        var itemName = e.item.name;

        switch (itemName) {
            case 'btnNuevo':
                mostrarNuevoResidente();
                break;
            case 'btnEditar':
                editarResidenteSeleccionado();
                break;
            case 'btnEliminar':
                eliminarResidenteSeleccionado();
                break;
        }
    }

    // ========================================================================
    // FUNCIONES CRUD
    // ========================================================================

    function mostrarNuevoResidente() {
        limpiarFormulario();
        popupResidente.SetHeaderText('Nuevo Residente');
        tabsResidente.SetActiveTabIndex(0);
        popupResidente.Show();
    }

    function editarResidenteSeleccionado() {
        var focusedRowIndex = gridResidentes.GetFocusedRowIndex();
        if (focusedRowIndex < 0) {
            showToast('warning', 'Seleccione un residente para editar');
            return;
        }

        var id = gridResidentes.GetRowKey(focusedRowIndex);
        cargarResidente(id);
    }

    function eliminarResidenteSeleccionado() {
        var focusedRowIndex = gridResidentes.GetFocusedRowIndex();
        if (focusedRowIndex < 0) {
            showToast('warning', 'Seleccione un residente para eliminar');
            return;
        }

        var id = gridResidentes.GetRowKey(focusedRowIndex);
        var nombre = gridResidentes.GetRowValues(focusedRowIndex, 'NombreCompleto');

        if (confirm('¿Está seguro de eliminar al residente "' + nombre + '"?\n\nEsta acción no se puede deshacer.')) {
            eliminarResidente(id);
        }
    }

    // ========================================================================
    // OPERACIONES AJAX
    // ========================================================================

    function cargarResidente(id) {
        $.ajax({
            type: 'POST',
            url: 'Residentes.aspx/ObtenerResidente',
            data: JSON.stringify({ id: id }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                var result = response.d;
                if (result.success) {
                    cargarDatosEnFormulario(result.data);
                    popupResidente.SetHeaderText('Editar Residente');
                    tabsResidente.SetActiveTabIndex(0);
                    popupResidente.Show();
                } else {
                    showToast('error', result.message);
                }
            },
            error: function () {
                showToast('error', 'Error al cargar residente');
            }
        });
    }

    function eliminarResidente(id) {
        $.ajax({
            type: 'POST',
            url: 'Residentes.aspx/EliminarResidente',
            data: JSON.stringify({ id: id }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                var result = response.d;
                if (result.success) {
                    showToast('success', result.message);
                    gridResidentes.Refresh();
                } else {
                    showToast('error', result.message);
                }
            },
            error: function () {
                showToast('error', 'Error al eliminar residente');
            }
        });
    }

    // ========================================================================
    // CARGA DE DATOS EN FORMULARIO
    // ========================================================================

    function cargarDatosEnFormulario(data) {
        // ID
        setHiddenFieldValue('hfResidenteId', data.Id || 0);

        // Datos personales
        if (cmbEntidad) cmbEntidad.SetValue(data.EntidadId);
        if (cmbSubEntidad) cmbSubEntidad.SetValue(data.SubEntidadId);
        if (cmbUnidad) cmbUnidad.SetValue(data.UnidadId);
        if (cmbTipoResidente) cmbTipoResidente.SetValue(data.TipoResidente || 'Propietario');
        if (txtNombre) txtNombre.SetValue(data.Nombre || '');
        if (txtApellidoPaterno) txtApellidoPaterno.SetValue(data.ApellidoPaterno || '');
        if (txtApellidoMaterno) txtApellidoMaterno.SetValue(data.ApellidoMaterno || '');
        if (chkEsPrincipal) chkEsPrincipal.SetChecked(data.EsPrincipal === 1 || data.EsPrincipal === true);
        if (chkActivo) chkActivo.SetChecked(data.Activo === 1 || data.Activo === true);
        if (dteFechaIngreso && data.FechaIngreso) dteFechaIngreso.SetDate(new Date(data.FechaIngreso));

        // Contacto
        if (txtEmail) txtEmail.SetValue(data.Email || '');
        if (txtTelefono) txtTelefono.SetValue(data.Telefono || '');
        if (txtCelular) txtCelular.SetValue(data.TelefonoCelular || '');
        if (txtTelefonoEmergencia) txtTelefonoEmergencia.SetValue(data.TelefonoEmergencia || '');

        // Telegram
        if (txtTelegramChatId) txtTelegramChatId.SetValue(data.TelegramChatId || '');
        if (txtTelegramUsername) txtTelegramUsername.SetValue(data.TelegramUsername || '');
        if (chkTelegramActivo) chkTelegramActivo.SetChecked(data.TelegramActivo === 1 || data.TelegramActivo === true);

        // Notificaciones
        if (chkNotifEmail) chkNotifEmail.SetChecked(data.RecibirNotificacionesEmail !== 0);
        if (chkNotifTelegram) chkNotifTelegram.SetChecked(data.RecibirNotificacionesTelegram !== 0);
        if (chkNotifPush) chkNotifPush.SetChecked(data.RecibirNotificacionesPush !== 0);

        // Identificación
        if (cmbTipoIdentificacion) cmbTipoIdentificacion.SetValue(data.TipoIdentificacion || 'INE');
        if (txtNumeroIdentificacion) txtNumeroIdentificacion.SetValue(data.NumeroIdentificacion || '');
        if (txtRFC) txtRFC.SetValue(data.RFC || '');
        if (txtCURP) txtCURP.SetValue(data.CURP || '');
        if (typeof txtClaveElector !== 'undefined' && txtClaveElector) txtClaveElector.SetValue(data.ClaveElector || '');
        if (typeof txtVigenciaINE !== 'undefined' && txtVigenciaINE) txtVigenciaINE.SetValue(data.VigenciaINE || '');

        // Vehículo
        if (chkTieneVehiculo) chkTieneVehiculo.SetChecked(data.TieneVehiculo === 1 || data.TieneVehiculo === true);
        if (txtPlacas) txtPlacas.SetValue(data.PlacasVehiculo || '');
    }

    function limpiarFormulario() {
        setHiddenFieldValue('hfResidenteId', '0');

        // Datos personales
        if (cmbEntidad) cmbEntidad.SetValue(null);
        if (cmbSubEntidad) cmbSubEntidad.SetValue(null);
        if (cmbUnidad) cmbUnidad.SetValue(null);
        if (cmbTipoResidente) cmbTipoResidente.SetValue('Propietario');
        if (txtNombre) txtNombre.SetValue('');
        if (txtApellidoPaterno) txtApellidoPaterno.SetValue('');
        if (txtApellidoMaterno) txtApellidoMaterno.SetValue('');
        if (chkEsPrincipal) chkEsPrincipal.SetChecked(false);
        if (chkActivo) chkActivo.SetChecked(true);
        if (dteFechaIngreso) dteFechaIngreso.SetDate(new Date());

        // Contacto
        if (txtEmail) txtEmail.SetValue('');
        if (txtTelefono) txtTelefono.SetValue('');
        if (txtCelular) txtCelular.SetValue('');
        if (txtTelefonoEmergencia) txtTelefonoEmergencia.SetValue('');

        // Telegram
        if (txtTelegramChatId) txtTelegramChatId.SetValue('');
        if (txtTelegramUsername) txtTelegramUsername.SetValue('');
        if (chkTelegramActivo) chkTelegramActivo.SetChecked(false);

        // Notificaciones
        if (chkNotifEmail) chkNotifEmail.SetChecked(true);
        if (chkNotifTelegram) chkNotifTelegram.SetChecked(true);
        if (chkNotifPush) chkNotifPush.SetChecked(true);

        // Identificación
        if (cmbTipoIdentificacion) cmbTipoIdentificacion.SetValue('INE');
        if (txtNumeroIdentificacion) txtNumeroIdentificacion.SetValue('');
        if (txtRFC) txtRFC.SetValue('');
        if (txtCURP) txtCURP.SetValue('');
        if (typeof txtClaveElector !== 'undefined' && txtClaveElector) txtClaveElector.SetValue('');
        if (typeof txtVigenciaINE !== 'undefined' && txtVigenciaINE) txtVigenciaINE.SetValue('');

        // Vehículo
        if (chkTieneVehiculo) chkTieneVehiculo.SetChecked(false);
        if (txtPlacas) txtPlacas.SetValue('');
    }

    // ========================================================================
    // EVENTOS DE COMBOS CASCADA
    // ========================================================================

    function onEntidadChanged(s, e) {
        var entidadId = s.GetValue();
        if (!entidadId) {
            if (cmbSubEntidad) {
                cmbSubEntidad.ClearItems();
                cmbSubEntidad.AddItem('-- Ninguna --', null);
            }
            return;
        }

        $.ajax({
            type: 'POST',
            url: 'Residentes.aspx/ObtenerSubEntidadesPorEntidad',
            data: JSON.stringify({ entidadId: entidadId }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                var result = response.d;
                if (result.success && cmbSubEntidad) {
                    cmbSubEntidad.ClearItems();
                    cmbSubEntidad.AddItem('-- Ninguna --', null);
                    for (var i = 0; i < result.data.length; i++) {
                        cmbSubEntidad.AddItem(result.data[i].RazonSocial, result.data[i].Id);
                    }
                }
            }
        });
    }

    function onSubEntidadChanged(s, e) {
        var subEntidadId = s.GetValue() || 0;

        $.ajax({
            type: 'POST',
            url: 'Residentes.aspx/ObtenerUnidadesPorSubEntidad',
            data: JSON.stringify({ subEntidadId: subEntidadId }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                var result = response.d;
                if (result.success && cmbUnidad) {
                    cmbUnidad.ClearItems();
                    for (var i = 0; i < result.data.length; i++) {
                        cmbUnidad.AddItem(result.data[i].Nombre, result.data[i].Id);
                    }
                }
            }
        });
    }

    // ========================================================================
    // EVENTO DOBLE CLICK EN GRID
    // ========================================================================

    function onRowDblClick(s, e) {
        var id = s.GetRowKey(e.visibleIndex);
        cargarResidente(id);
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

    // ========================================================================
    // ESCANEO DE INE CON AZURE DOCUMENT INTELLIGENCE
    // ========================================================================

    var ineImageBase64 = null;
    var ineArchivoOriginal = null; // Guardar el archivo original para enviarlo directamente

    function initINEScanner() {
        var dropZone = document.getElementById('ineDropZone');
        var fileInput = document.getElementById('ineFileInput');

        if (!dropZone || !fileInput) return;

        // Click para seleccionar archivo
        dropZone.addEventListener('click', function () {
            fileInput.click();
        });

        // Cambio de archivo
        fileInput.addEventListener('change', function (e) {
            if (e.target.files && e.target.files[0]) {
                procesarArchivoINE(e.target.files[0]);
            }
        });

        // Drag & Drop
        dropZone.addEventListener('dragover', function (e) {
            e.preventDefault();
            e.stopPropagation();
            dropZone.classList.add('drag-over');
        });

        dropZone.addEventListener('dragleave', function (e) {
            e.preventDefault();
            e.stopPropagation();
            dropZone.classList.remove('drag-over');
        });

        dropZone.addEventListener('drop', function (e) {
            e.preventDefault();
            e.stopPropagation();
            dropZone.classList.remove('drag-over');

            if (e.dataTransfer.files && e.dataTransfer.files[0]) {
                procesarArchivoINE(e.dataTransfer.files[0]);
            }
        });
    }

    function procesarArchivoINE(file) {
        // Validar tipo de archivo
        var validTypes = ['image/jpeg', 'image/png', 'image/jpg', 'application/pdf'];
        if (validTypes.indexOf(file.type) === -1) {
            showToast('error', 'Formato de archivo no válido. Use JPG, PNG o PDF.');
            return;
        }

        // Validar tamaño (máx 10MB)
        if (file.size > 10 * 1024 * 1024) {
            showToast('error', 'El archivo es demasiado grande. Máximo 10MB.');
            return;
        }

        // Guardar archivo original para enviarlo directamente a la API
        ineArchivoOriginal = file;
        
        var reader = new FileReader();
        reader.onload = function (e) {
            ineImageBase64 = e.target.result;
            mostrarPreviewINE(e.target.result);
        };
        reader.readAsDataURL(file);
    }

    function mostrarPreviewINE(dataUrl) {
        var dropZone = document.getElementById('ineDropZone');
        var previewContainer = document.getElementById('inePreviewContainer');
        var previewImage = document.getElementById('inePreviewImage');

        if (dropZone) dropZone.style.display = 'none';
        if (previewContainer) previewContainer.style.display = 'block';
        if (previewImage) previewImage.src = dataUrl;

        actualizarEstadoINE('Imagen cargada. Haga clic en "Escanear INE" para extraer los datos.', 'info');
    }

    function removeINEImage() {
        ineImageBase64 = null;
        ineArchivoOriginal = null;

        var dropZone = document.getElementById('ineDropZone');
        var previewContainer = document.getElementById('inePreviewContainer');
        var previewImage = document.getElementById('inePreviewImage');
        var fileInput = document.getElementById('ineFileInput');

        if (dropZone) dropZone.style.display = 'block';
        if (previewContainer) previewContainer.style.display = 'none';
        if (previewImage) previewImage.src = '';
        if (fileInput) fileInput.value = '';

        actualizarEstadoINE('', '');
    }

    function escanearINE() {
        if (!ineArchivoOriginal) {
            showToast('warning', 'Primero cargue una imagen de INE');
            return;
        }

        // Mostrar loading
        if (typeof loadingINE !== 'undefined') {
            loadingINE.Show();
        }
        actualizarEstadoINE('Procesando imagen con Azure Document Intelligence...', 'processing');

        // Crear FormData con el archivo
        var formData = new FormData();
        formData.append('archivo', ineArchivoOriginal);
        formData.append('tipoDocumento', 'INE');

        // Enviar archivo al proxy (el proxy maneja la autenticación JWT)
        $.ajax({
            type: 'POST',
            url: '/Services/DocumentIntelligenceProxy.ashx',
            data: formData,
            processData: false,
            contentType: false,
            timeout: 60000, // 60 segundos timeout
            success: function (response) {
                if (typeof loadingINE !== 'undefined') {
                    loadingINE.Hide();
                }

                // La respuesta viene en formato List<CrudDto>
                if (response && response.length > 0 && response[0].Campos) {
                    var datos = response[0].Campos;
                    llenarCamposDesdeINE(datos);
                    actualizarEstadoINE('✓ Datos extraídos correctamente de la INE', 'success');
                    showToast('success', 'Datos de INE extraídos correctamente');
                } else {
                    actualizarEstadoINE('✗ No se pudieron extraer datos', 'error');
                    showToast('error', 'No se pudieron extraer datos del documento');
                }
            },
            error: function (xhr, status, error) {
                if (typeof loadingINE !== 'undefined') {
                    loadingINE.Hide();
                }
                var mensaje = 'Error al comunicarse con el servidor';
                if (status === 'timeout') {
                    mensaje = 'Tiempo de espera agotado. Intente con una imagen más pequeña.';
                } else if (xhr.responseJSON && xhr.responseJSON.Mensaje) {
                    mensaje = xhr.responseJSON.Mensaje;
                }
                actualizarEstadoINE('✗ ' + mensaje, 'error');
                showToast('error', mensaje);
            }
        });
    }

    function llenarCamposDesdeINE(datos) {
        // Si datos viene en formato CrudDto (con Campos), extraer los valores
        if (datos && typeof datos === 'object' && !Array.isArray(datos)) {
            // Si tiene estructura Campos.Valor, convertir a formato simple
            var datosSimples = {};
            for (var key in datos) {
                if (datos.hasOwnProperty(key)) {
                    var campo = datos[key];
                    if (campo && typeof campo === 'object' && campo.Valor !== undefined) {
                        datosSimples[key] = campo.Valor;
                    } else {
                        datosSimples[key] = campo;
                    }
                }
            }
            datos = datosSimples;
        }
        if (!datos) return;

        // Datos personales (Tab 1)
        if (datos.nombre && typeof txtNombre !== 'undefined') {
            txtNombre.SetValue(datos.nombre);
        }
        if (datos.apellidoPaterno && typeof txtApellidoPaterno !== 'undefined') {
            txtApellidoPaterno.SetValue(datos.apellidoPaterno);
        }
        if (datos.apellidoMaterno && typeof txtApellidoMaterno !== 'undefined') {
            txtApellidoMaterno.SetValue(datos.apellidoMaterno);
        }

        // Identificación (Tab 3)
        if (typeof cmbTipoIdentificacion !== 'undefined') {
            cmbTipoIdentificacion.SetValue('INE');
        }
        if (datos.numeroIdentificacion && typeof txtNumeroIdentificacion !== 'undefined') {
            txtNumeroIdentificacion.SetValue(datos.numeroIdentificacion);
        }
        if (datos.curp && typeof txtCURP !== 'undefined') {
            txtCURP.SetValue(datos.curp);
        }
        if (datos.claveElector && typeof txtClaveElector !== 'undefined') {
            txtClaveElector.SetValue(datos.claveElector);
        }
        if (datos.vigencia && typeof txtVigenciaINE !== 'undefined') {
            txtVigenciaINE.SetValue(datos.vigencia);
        }

        // Cambiar al tab de datos personales para que el usuario vea los datos
        if (typeof tabsResidente !== 'undefined') {
            tabsResidente.SetActiveTabIndex(0);
        }
    }

    function actualizarEstadoINE(mensaje, tipo) {
        var statusDiv = document.getElementById('ineStatus');
        if (!statusDiv) return;

        statusDiv.textContent = mensaje;
        statusDiv.className = 'ine-status';

        if (tipo) {
            statusDiv.classList.add('ine-status-' + tipo);
        }
    }

    // Inicializar scanner cuando el DOM esté listo
    $(document).ready(function () {
        // Inicializar después de un pequeño delay para asegurar que el popup esté cargado
        setTimeout(initINEScanner, 500);
    });

    // ========================================================================
    // GESTIÓN DE VEHÍCULOS
    // ========================================================================

    function onToolbarVehiculosClick(s, e) {
        var itemName = e.item.name;
        switch (itemName) {
            case 'btnNuevoVehiculo':
                mostrarNuevoVehiculo();
                break;
            case 'btnEditarVehiculo':
                editarVehiculoSeleccionado();
                break;
            case 'btnEliminarVehiculo':
                eliminarVehiculoSeleccionado();
                break;
        }
    }

    function mostrarNuevoVehiculo() {
        limpiarFormularioVehiculo();
        popupVehiculo.SetHeaderText('Nuevo Vehículo');
        popupVehiculo.Show();
    }

    function editarVehiculoSeleccionado() {
        var focusedRowIndex = gridVehiculos.GetFocusedRowIndex();
        if (focusedRowIndex < 0) {
            showToast('warning', 'Seleccione un vehículo para editar');
            return;
        }
        var id = gridVehiculos.GetRowKey(focusedRowIndex);
        cargarVehiculo(id);
    }

    function eliminarVehiculoSeleccionado() {
        var focusedRowIndex = gridVehiculos.GetFocusedRowIndex();
        if (focusedRowIndex < 0) {
            showToast('warning', 'Seleccione un vehículo para eliminar');
            return;
        }
        var id = gridVehiculos.GetRowKey(focusedRowIndex);
        var placas = gridVehiculos.GetRowValues(focusedRowIndex, 'Placas');
        if (confirm('¿Está seguro de eliminar el vehículo con placas "' + placas + '"?')) {
            eliminarVehiculo(id);
        }
    }

    function limpiarFormularioVehiculo() {
        setHiddenFieldValue('hfVehiculoId', '0');
        if (typeof txtVehPlacas !== 'undefined') txtVehPlacas.SetValue('');
        if (typeof cmbVehTipo !== 'undefined') cmbVehTipo.SetValue('Automóvil');
        if (typeof txtVehMarca !== 'undefined') txtVehMarca.SetValue('');
        if (typeof txtVehModelo !== 'undefined') txtVehModelo.SetValue('');
        if (typeof txtVehAnio !== 'undefined') txtVehAnio.SetValue(new Date().getFullYear());
        if (typeof txtVehColor !== 'undefined') txtVehColor.SetValue('');
        if (typeof txtVehTarjeton !== 'undefined') txtVehTarjeton.SetValue('');
        if (typeof chkVehPrincipal !== 'undefined') chkVehPrincipal.SetChecked(false);
        if (typeof txtVehObservaciones !== 'undefined') txtVehObservaciones.SetValue('');
        if (typeof chkVehActivo !== 'undefined') chkVehActivo.SetChecked(true);
    }

    function cargarVehiculo(id) {
        $.ajax({
            type: 'POST',
            url: 'Residentes.aspx/ObtenerVehiculo',
            data: JSON.stringify({ id: id }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                var result = response.d;
                if (result.success) {
                    var data = result.data;
                    setHiddenFieldValue('hfVehiculoId', data.Id);
                    if (typeof txtVehPlacas !== 'undefined') txtVehPlacas.SetValue(data.Placas || '');
                    if (typeof cmbVehTipo !== 'undefined') cmbVehTipo.SetValue(data.TipoVehiculo || 'Automóvil');
                    if (typeof txtVehMarca !== 'undefined') txtVehMarca.SetValue(data.Marca || '');
                    if (typeof txtVehModelo !== 'undefined') txtVehModelo.SetValue(data.Modelo || '');
                    if (typeof txtVehAnio !== 'undefined') txtVehAnio.SetValue(data.Anio || new Date().getFullYear());
                    if (typeof txtVehColor !== 'undefined') txtVehColor.SetValue(data.Color || '');
                    if (typeof txtVehTarjeton !== 'undefined') txtVehTarjeton.SetValue(data.NumeroTarjeton || '');
                    if (typeof chkVehPrincipal !== 'undefined') chkVehPrincipal.SetChecked(data.EsPrincipal == 1);
                    if (typeof txtVehObservaciones !== 'undefined') txtVehObservaciones.SetValue(data.Observaciones || '');
                    if (typeof chkVehActivo !== 'undefined') chkVehActivo.SetChecked(data.Activo == 1);
                    popupVehiculo.SetHeaderText('Editar Vehículo');
                    popupVehiculo.Show();
                } else {
                    showToast('error', result.message);
                }
            },
            error: function () { toastr.error('Error al cargar vehículo'); }
        });
    }

    function guardarVehiculo() {
        var residenteId = getHiddenFieldValue('hfResidenteId');
        if (!residenteId || residenteId == '0') {
            showToast('warning', 'Primero guarde el residente antes de agregar vehículos');
            return;
        }

        var vehiculoId = getHiddenFieldValue('hfVehiculoId') || 0;
        var datos = {
            id: parseInt(vehiculoId),
            residenteId: parseInt(residenteId),
            placas: txtVehPlacas.GetValue(),
            tipoVehiculo: cmbVehTipo.GetValue(),
            marca: txtVehMarca.GetValue(),
            modelo: txtVehModelo.GetValue(),
            anio: txtVehAnio.GetValue(),
            color: txtVehColor.GetValue(),
            numeroTarjeton: txtVehTarjeton.GetValue(),
            esPrincipal: chkVehPrincipal.GetChecked(),
            observaciones: txtVehObservaciones.GetValue(),
            activo: chkVehActivo.GetChecked()
        };

        $.ajax({
            type: 'POST',
            url: 'Residentes.aspx/GuardarVehiculo',
            data: JSON.stringify({ datos: datos }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                var result = response.d;
                if (result.success) {
                    showToast('success', result.message);
                    popupVehiculo.Hide();
                    gridVehiculos.Refresh();
                } else {
                    showToast('error', result.message);
                }
            },
            error: function () { toastr.error('Error al guardar vehículo'); }
        });
    }

    function eliminarVehiculo(id) {
        $.ajax({
            type: 'POST',
            url: 'Residentes.aspx/EliminarVehiculo',
            data: JSON.stringify({ id: id }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                var result = response.d;
                if (result.success) {
                    showToast('success', result.message);
                    gridVehiculos.Refresh();
                } else {
                    showToast('error', result.message);
                }
            },
            error: function () { toastr.error('Error al eliminar vehículo'); }
        });
    }

    // ========================================================================
    // GESTIÓN DE TAGS DE ACCESO
    // ========================================================================

    function onToolbarTagsClick(s, e) {
        var itemName = e.item.name;
        switch (itemName) {
            case 'btnNuevoTag':
                mostrarNuevoTag();
                break;
            case 'btnEditarTag':
                editarTagSeleccionado();
                break;
            case 'btnEliminarTag':
                eliminarTagSeleccionado();
                break;
        }
    }

    function mostrarNuevoTag() {
        limpiarFormularioTag();
        popupTag.SetHeaderText('Nuevo Tag de Acceso');
        popupTag.Show();
    }

    function editarTagSeleccionado() {
        var focusedRowIndex = gridTags.GetFocusedRowIndex();
        if (focusedRowIndex < 0) {
            showToast('warning', 'Seleccione un tag para editar');
            return;
        }
        var id = gridTags.GetRowKey(focusedRowIndex);
        cargarTag(id);
    }

    function eliminarTagSeleccionado() {
        var focusedRowIndex = gridTags.GetFocusedRowIndex();
        if (focusedRowIndex < 0) {
            showToast('warning', 'Seleccione un tag para eliminar');
            return;
        }
        var id = gridTags.GetRowKey(focusedRowIndex);
        var codigo = gridTags.GetRowValues(focusedRowIndex, 'CodigoTag');
        if (confirm('¿Está seguro de eliminar el tag "' + codigo + '"?')) {
            eliminarTag(id);
        }
    }

    function limpiarFormularioTag() {
        setHiddenFieldValue('hfTagId', '0');
        if (typeof txtTagCodigo !== 'undefined') txtTagCodigo.SetValue('');
        if (typeof cmbTagTipo !== 'undefined') cmbTagTipo.SetValue('RFID');
        if (typeof txtTagDescripcion !== 'undefined') txtTagDescripcion.SetValue('');
        if (typeof dteTagAsignacion !== 'undefined') dteTagAsignacion.SetDate(new Date());
        if (typeof dteTagVencimiento !== 'undefined') dteTagVencimiento.SetDate(null);
        if (typeof chkTagPeatonal !== 'undefined') chkTagPeatonal.SetChecked(true);
        if (typeof chkTagVehicular !== 'undefined') chkTagVehicular.SetChecked(false);
        if (typeof chkTagAreas !== 'undefined') chkTagAreas.SetChecked(false);
        if (typeof chkTagPrincipal !== 'undefined') chkTagPrincipal.SetChecked(false);
        if (typeof txtTagObservaciones !== 'undefined') txtTagObservaciones.SetValue('');
        if (typeof chkTagActivo !== 'undefined') chkTagActivo.SetChecked(true);
    }

    function cargarTag(id) {
        $.ajax({
            type: 'POST',
            url: 'Residentes.aspx/ObtenerTag',
            data: JSON.stringify({ id: id }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                var result = response.d;
                if (result.success) {
                    var data = result.data;
                    setHiddenFieldValue('hfTagId', data.Id);
                    if (typeof txtTagCodigo !== 'undefined') txtTagCodigo.SetValue(data.CodigoTag || '');
                    if (typeof cmbTagTipo !== 'undefined') cmbTagTipo.SetValue(data.TipoTag || 'RFID');
                    if (typeof txtTagDescripcion !== 'undefined') txtTagDescripcion.SetValue(data.Descripcion || '');
                    if (typeof dteTagAsignacion !== 'undefined' && data.FechaAsignacion) dteTagAsignacion.SetDate(new Date(data.FechaAsignacion));
                    if (typeof dteTagVencimiento !== 'undefined' && data.FechaVencimiento) dteTagVencimiento.SetDate(new Date(data.FechaVencimiento));
                    if (typeof chkTagPeatonal !== 'undefined') chkTagPeatonal.SetChecked(data.PermiteAccesoPeatonal == 1);
                    if (typeof chkTagVehicular !== 'undefined') chkTagVehicular.SetChecked(data.PermiteAccesoVehicular == 1);
                    if (typeof chkTagAreas !== 'undefined') chkTagAreas.SetChecked(data.PermiteAccesoAreas == 1);
                    if (typeof chkTagPrincipal !== 'undefined') chkTagPrincipal.SetChecked(data.EsPrincipal == 1);
                    if (typeof txtTagObservaciones !== 'undefined') txtTagObservaciones.SetValue(data.Observaciones || '');
                    if (typeof chkTagActivo !== 'undefined') chkTagActivo.SetChecked(data.Activo == 1);
                    popupTag.SetHeaderText('Editar Tag de Acceso');
                    popupTag.Show();
                } else {
                    showToast('error', result.message);
                }
            },
            error: function () { toastr.error('Error al cargar tag'); }
        });
    }

    function guardarTag() {
        var residenteId = getHiddenFieldValue('hfResidenteId');
        if (!residenteId || residenteId == '0') {
            showToast('warning', 'Primero guarde el residente antes de agregar tags');
            return;
        }

        var tagId = getHiddenFieldValue('hfTagId') || 0;
        var datos = {
            id: parseInt(tagId),
            residenteId: parseInt(residenteId),
            codigoTag: txtTagCodigo.GetValue(),
            tipoTag: cmbTagTipo.GetValue(),
            descripcion: txtTagDescripcion.GetValue(),
            fechaAsignacion: dteTagAsignacion.GetDate(),
            fechaVencimiento: dteTagVencimiento.GetDate(),
            permiteAccesoPeatonal: chkTagPeatonal.GetChecked(),
            permiteAccesoVehicular: chkTagVehicular.GetChecked(),
            permiteAccesoAreas: chkTagAreas.GetChecked(),
            esPrincipal: chkTagPrincipal.GetChecked(),
            observaciones: txtTagObservaciones.GetValue(),
            activo: chkTagActivo.GetChecked()
        };

        $.ajax({
            type: 'POST',
            url: 'Residentes.aspx/GuardarTag',
            data: JSON.stringify({ datos: datos }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                var result = response.d;
                if (result.success) {
                    showToast('success', result.message);
                    popupTag.Hide();
                    gridTags.Refresh();
                } else {
                    showToast('error', result.message);
                }
            },
            error: function () { toastr.error('Error al guardar tag'); }
        });
    }

    function eliminarTag(id) {
        $.ajax({
            type: 'POST',
            url: 'Residentes.aspx/EliminarTag',
            data: JSON.stringify({ id: id }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                var result = response.d;
                if (result.success) {
                    showToast('success', result.message);
                    gridTags.Refresh();
                } else {
                    showToast('error', result.message);
                }
            },
            error: function () { toastr.error('Error al eliminar tag'); }
        });
    }

    function getHiddenFieldValue(fieldId) {
        var field = document.querySelector('[id$="' + fieldId + '"]');
        return field ? field.value : null;
    }

    // API pública
    return {
        onToolbarResidentesClick: onToolbarResidentesClick,
        onRowDblClick: onRowDblClick,
        onEntidadChanged: onEntidadChanged,
        onSubEntidadChanged: onSubEntidadChanged,
        mostrarNuevoResidente: mostrarNuevoResidente,
        editarResidenteSeleccionado: editarResidenteSeleccionado,
        eliminarResidenteSeleccionado: eliminarResidenteSeleccionado,
        escanearINE: escanearINE,
        removeINEImage: removeINEImage,
        initINEScanner: initINEScanner,
        // Vehículos
        onToolbarVehiculosClick: onToolbarVehiculosClick,
        guardarVehiculo: guardarVehiculo,
        // Tags
        onToolbarTagsClick: onToolbarTagsClick,
        guardarTag: guardarTag
    };

})();

// Funciones globales para eventos del grid (requeridas por DevExpress)
function onToolbarResidentesClick(s, e) {
    ResidentesModule.onToolbarResidentesClick(s, e);
}

function onRowDblClick(s, e) {
    ResidentesModule.onRowDblClick(s, e);
}

function onEntidadChanged(s, e) {
    ResidentesModule.onEntidadChanged(s, e);
}

function onSubEntidadChanged(s, e) {
    ResidentesModule.onSubEntidadChanged(s, e);
}

// Funciones globales para escaneo de INE
function escanearINE() {
    ResidentesModule.escanearINE();
}

function removeINEImage() {
    ResidentesModule.removeINEImage();
}

// Funciones globales para vehículos
function onToolbarVehiculosClick(s, e) {
    ResidentesModule.onToolbarVehiculosClick(s, e);
}

function guardarVehiculo() {
    ResidentesModule.guardarVehiculo();
}

// Funciones globales para tags
function onToolbarTagsClick(s, e) {
    ResidentesModule.onToolbarTagsClick(s, e);
}

function guardarTag() {
    ResidentesModule.guardarTag();
}
