window.onToolbarClick = function (s, e) {
    switch (e.item.name) {
        case "Agregar":
            showAddForm();
            break;
        case "Editar":
            showEditForm();
            break;
        case "Eliminar":
            deleteEntity();
            break;
        case "Exportar":
            exportGrid();
            break;
        case "Refrescar":
            refreshGrid();
            break;
    }
};

// Mostrar formulario vacío
function showAddForm() {
    popupForm.Show();
    popupForm.GetMainElement ? setTimeout(()=> {
        const hf = document.getElementById('hfId');
        if (hf) hf.value = '';
    }, 50) : (document.getElementById('hfId') && (document.getElementById('hfId').value=''));
}

// Mostrar formulario con datos seleccionados desde el grid
function showEditForm() {
    var index = gridEntidades.GetFocusedRowIndex();
    if (index < 0) return;

    gridEntidades.GetRowValues(index,
        'Id;Clave;Alias;RazonSocial;IdCP;Calle;NoExterior;NoInterior;Colonia;Localidad;RFC;FechaAlta;FechaInicio;Activo;ClaveRegime;RegimenFiscal;ClaveMetodo;MetodoDePago;ClaveForma;FormaDePago;ClaveUso;UsoCFDI;Latitud;Longitud',
        function (values) {
            document.getElementById('hfId').value = values[0] || '';
            document.getElementById('txtClave').value = values[1] || '';
            document.getElementById('txtCIF').value = values[2] || '';
            document.getElementById('txtRazonSocial').value = values[3] || '';
            document.getElementById('txtCP').value = values[4] || '';
            document.getElementById('txtCalle').value = values[5] || '';
            document.getElementById('txtNoExterior').value = values[6] || '';
            document.getElementById('txtNoInterior').value = values[7] || '';
            document.getElementById('txtColonia').value = values[8] || '';
            document.getElementById('txtLocalidad').value = values[9] || '';
            document.getElementById('txtRFC').value = values[10] || '';
            document.getElementById('txtFechaAlta').value = values[11] || '';
            document.getElementById('txtFechaInicio').value = values[12] || '';
            document.getElementById('chkActivo').checked = (values[13] === true || values[13] === "True");
            document.getElementById('txtClaveRegime').value = values[14] || '';
            document.getElementById('txtRegimenFiscal').value = values[15] || '';
            document.getElementById('txtClaveMetodo').value = values[16] || '';
            document.getElementById('txtMetodoPago').value = values[17] || '';
            document.getElementById('txtClaveForma').value = values[18] || '';
            document.getElementById('txtFormaPago').value = values[19] || '';
            document.getElementById('txtClaveUso').value = values[20] || '';
            document.getElementById('txtUsoCFDI').value = values[21] || '';
            document.getElementById('txtLatitud').value = values[22] || '';
            document.getElementById('txtLongitud').value = values[23] || '';
            popupForm.Show();
        }
    );
}

// Guardar entidad (insertar o actualizar) usando CRUDDTO
function saveEntity() {
    // dispara el callback del panel que envuelve tu popup
    popUpCallBack.PerformCallback();
}

// Eliminar entidad seleccionada
function deleteEntity() {
    var index = gridEntidades.GetFocusedRowIndex();
    if (index < 0) return;
    var id = gridEntidades.GetRowValues(index, 'Id');

    fetch("/api/Crud/entidades/id/" + id, {
        method: "DELETE"
    }).then(response => {
        if (response.ok) {
            gridCallback.PerformCallback();
        }
    });
}

// Refrescar el grid
function refreshGrid() {
    gridCallback.PerformCallback();
}

// Exportar el grid a Excel
function exportGrid() {
    gridEntidades.ExportToXlsx();
}

function enviarPDF() {
    const loading = document.getElementById("loadingIndicator");
    const fileInput = document.getElementById("pdfInput");
    const file = fileInput?.files?.[0];

    if (!file) {
        alert("Selecciona un archivo PDF.");
        if (loading) loading.style.display = "none";
        return;
    }

    if (file.type !== "application/pdf") {
        alert("Por favor selecciona un archivo PDF válido.");
        if (loading) loading.style.display = "none";
        return;
    }

    if (loading) loading.style.display = "block";

    const formData = new FormData();
    formData.append("file", file);

    fetch("https://jela-n8n.azurewebsites.net/webhook/form-data", {
        method: "POST",
        body: formData
    })
        .then(res => {
            if (!res.ok) throw new Error("Error en la subida del PDF");
            return res.json();
        })
        .then(data => {
            console.log("JSON recibido:", data);
            mapearCamposDesdeAzure(data);
        })
        .catch(err => {
            console.error("Error al procesar PDF:", err);
            alert("Hubo un problema al procesar el PDF.");
        })
        .finally(() => {
            if (loading) loading.style.display = "none";
        });
}

// Disparar automáticamente enviarPDF() al seleccionar archivo
document.addEventListener("DOMContentLoaded", function () {
    var pdfInput = document.getElementById("pdfInput");
    if (pdfInput) {
        pdfInput.addEventListener("change", function () {
            const loading = document.getElementById("loadingIndicator");
            if (loading) loading.style.display = "block";

            if (this.files.length && this.files[0].type === "application/pdf") {
                enviarPDF();
            } else {
                alert("Por favor selecciona un archivo PDF válido.");
                if (loading) loading.style.display = "none";
            }
        });
    }
});

function quitarAcentos(s) {
    return (s || "").normalize("NFD").replace(/[\u0300-\u036f]/g, "");
}

function limpiarTexto(s) {
    return quitarAcentos(s).replace(/[^A-Za-z0-9]/g, "").trim();
}

function djb2HashBase36(s) {
    let hash = 5381;
    const str = limpiarTexto(s).toUpperCase();
    for (let i = 0; i < str.length; i++) {
        hash = ((hash << 5) + hash) + str.charCodeAt(i);
    }
    return Math.abs(hash).toString(36).toUpperCase();
}

function generarClaveCompacta(valores) {
    const rfc = valores.txtRFC || "";
    const cif = valores.txtCIF || "";
    const razon = valores.txtRazonSocial || "";

    // Limpiar y asegurar mayúsculas
    const cleanRFC = rfc.replace(/[^A-Z0-9]/gi, "").toUpperCase();
    const cleanCIF = cif.replace(/[^0-9]/g, "");
    const cleanRazon = razon.replace(/[^A-ZÁÉÍÓÚÑ0-9\s]/gi, "").toUpperCase();

    // Primeros 4 del RFC
    const rfcPart = cleanRFC.substring(0, 4);

    // Últimos 4 del CIF
    const cifPart = cleanCIF.slice(-4);

    // Hash corto de la razón social
    let hash = 5381;
    for (let i = 0; i < cleanRazon.length; i++) {
        hash = ((hash << 5) + hash) + cleanRazon.charCodeAt(i);
    }
    const hashPart = Math.abs(hash).toString(36).toUpperCase().slice(0, 4);

    // Construir clave final (RFC+CIF juntos, hash separado)
    const clave = `${rfcPart}${cifPart}-${hashPart}`;

    return clave;
}

// Mapeo de campos
function mapearCamposDesdeAzure(data) {
    const content = (Array.isArray(data) ? data[0]?.analyzeResult?.content : data.analyzeResult?.content || "")
        .replace(/\r?\n/g, " ")
        .replace(/\s+/g, " ")
        .trim();

    const valoresDetectados = {};

    // CIF
    const cifMatch = content.match(/idCIF:\s*(\d+)/i);
    if (cifMatch) valoresDetectados.txtCIF = cifMatch[1];

    // RFC
    const rfcMatch = content.match(/[A-ZÑ&]{3,4}\d{6}[A-Z0-9]{3}/);
    if (rfcMatch) valoresDetectados.txtRFC = rfcMatch[0];

    // Razón Social
    const razonMatch = content.match(/CONSTANCIA DE SITUACIÓN FISCAL\s+([A-ZÁÉÍÓÚÑ\s]+)\s+Nombre/i);
    if (razonMatch) valoresDetectados.txtRazonSocial = razonMatch[1].trim();

    // Fecha inicio de operaciones
    const fechaInicioMatch = content.match(/Fecha inicio de operaciones:\s*([0-9]{1,2}\sDE\s[A-ZÁÉÍÓÚÑ]+\sDE\s[0-9]{4})/i);
    if (fechaInicioMatch) valoresDetectados.txtFechaInicio = fechaInicioMatch[1];

    // Código Postal
    const cpMatch = content.match(/(C(Ó|O)DIGO\s+POSTAL|C\.?P\.?):?\s*(\d{5})/i);
    if (cpMatch) valoresDetectados.txtCP = cpMatch[3].trim();

    // Tipo Vialidad
    const tipoVialMatch = content.match(/Tipo\s+(de\s+)?Vialidad\s*[:\-]?\s*([A-ZÁÉÍÓÚÑ0-9\s]+)/i);
    if (tipoVialMatch) valoresDetectados.txtTipoVial = tipoVialMatch[2].trim();

    // Nombre Vialidad (Calle)
    const calleMatch = content.match(/Nombre\s+(de\s+)?Vialidad\s*[:\-]?\s*([A-ZÁÉÍÓÚÑ0-9\s]+)/i);
    if (calleMatch) valoresDetectados.txtCalle = calleMatch[2].trim();

    // Número Exterior
    const noExtMatch = content.match(/N[uú]mero\s+Exterior\s*[:\-]?\s*([A-Z0-9\s]+)/i);
    if (noExtMatch) valoresDetectados.txtNoExterior = noExtMatch[1].trim();

    // Número Interior
    const noIntMatch = content.match(/N[uú]mero\s+Interior\s*[:\-]?\s*([A-Z0-9\s]+)/i);
    if (noIntMatch) valoresDetectados.txtNoInterior = noIntMatch[1].trim();

    // Colonia
    const coloniaMatch = content.match(/Colonia\s*[:\-]?\s*([A-ZÁÉÍÓÚÑ0-9\s]+)/i);
    if (coloniaMatch) valoresDetectados.txtColonia = coloniaMatch[1].trim();

    // Localidad
    const localidadMatch = content.match(/Localidad\s*[:\-]?\s*([A-ZÁÉÍÓÚÑ0-9\s]+)/i);
    if (localidadMatch) valoresDetectados.txtLocalidad = localidadMatch[1].trim();

    // Municipio
    const municipioMatch = content.match(/Nombre\s+del\s+Municipio\s+o\s+Demarcación\s+Territorial\s*[:\-]?\s*([A-ZÁÉÍÓÚÑ0-9\s]+)/i);
    if (municipioMatch) valoresDetectados.txtMunicipio = municipioMatch[1].trim();

    // Estado
    const estadoMatch = content.match(/Estado\s*[:\-]?\s*([A-ZÁÉÍÓÚÑ0-9\s]+)/i);
    if (estadoMatch) valoresDetectados.txtEstado = estadoMatch[1].trim();

    // Entre Calle
    const entreMatch = content.match(/Entre\s+Calle\s*[:\-]?\s*([A-ZÁÉÍÓÚÑ0-9\s]+)/i);
    if (entreMatch) valoresDetectados.txtEntrecalle = entreMatch[1].trim();

    // Régimen Fiscal
    const regimenMatch = content.match(/Régimen Fiscal\s*[:\-]?\s*([A-ZÁÉÍÓÚÑ0-9\s]+)/i);
    if (regimenMatch) {
        valoresDetectados.txtClaveRegime = regimenMatch[1].trim();
        valoresDetectados.txtRegimenFiscal = regimenMatch[1].trim();
    }

    // Método de Pago
    const metodoMatch = content.match(/Método de Pago\s*[:\-]?\s*([A-ZÁÉÍÓÚÑ0-9\s]+)/i);
    if (metodoMatch) valoresDetectados.txtMetodoPago = metodoMatch[1].trim();

    // Forma de Pago
    const formaMatch = content.match(/Forma de Pago\s*[:\-]?\s*([A-ZÁÉÍÓÚÑ0-9\s]+)/i);
    if (formaMatch) valoresDetectados.txtFormaPago = formaMatch[1].trim();

    // Uso CFDI
    const usoMatch = content.match(/Uso CFDI\s*[:\-]?\s*([A-ZÁÉÍÓÚÑ0-9\s]+)/i);
    if (usoMatch) valoresDetectados.txtUsoCFDI = usoMatch[1].trim();

    // ✅ Asignar valores a los TextBox
    if (valoresDetectados.txtRFC) txtRFC.SetText(valoresDetectados.txtRFC);
    if (valoresDetectados.txtCIF) txtCIF.SetText(valoresDetectados.txtCIF);
    if (valoresDetectados.txtRazonSocial) txtRazonSocial.SetText(valoresDetectados.txtRazonSocial);
    if (valoresDetectados.txtFechaInicio) txtFechaInicio.SetText(valoresDetectados.txtFechaInicio);
    if (valoresDetectados.txtCP) txtCP.SetText(valoresDetectados.txtCP);
    if (valoresDetectados.txtTipoVial) txtTipoVial.SetText(valoresDetectados.txtTipoVial);
    if (valoresDetectados.txtCalle) txtCalle.SetText(valoresDetectados.txtCalle);
    if (valoresDetectados.txtNoExterior) txtNoExterior.SetText(valoresDetectados.txtNoExterior);
    if (valoresDetectados.txtNoInterior) txtNoInterior.SetText(valoresDetectados.txtNoInterior);
    if (valoresDetectados.txtColonia) txtColonia.SetText(valoresDetectados.txtColonia);
    if (valoresDetectados.txtLocalidad) txtLocalidad.SetText(valoresDetectados.txtLocalidad);
    if (valoresDetectados.txtMunicipio) txtMunicipio.SetText(valoresDetectados.txtMunicipio);
    if (valoresDetectados.txtEstado) txtEstado.SetText(valoresDetectados.txtEstado);
    if (valoresDetectados.txtEntrecalle) txtEntrecalle.SetText(valoresDetectados.txtEntrecalle);
    if (valoresDetectados.txtClaveRegime) txtClaveRegime.SetText(valoresDetectados.txtClaveRegime);
    if (valoresDetectados.txtRegimenFiscal) txtRegimenFiscal.SetText(valoresDetectados.txtRegimenFiscal);
    if (valoresDetectados.txtMetodoPago) txtMetodoPago.SetText(valoresDetectados.txtMetodoPago);
    if (valoresDetectados.txtFormaPago) txtFormaPago.SetText(valoresDetectados.txtFormaPago);
    if (valoresDetectados.txtUsoCFDI) txtUsoCFDI.SetText(valoresDetectados.txtUsoCFDI);

    // ✅ Generar clave compacta automáticamente
    const claveAuto = generarClaveCompacta(valoresDetectados);
    if (claveAuto) txtClave.SetText(claveAuto);

    console.log("Valores detectados automáticamente:", valoresDetectados);
    limpiarTextBoxes();

}

function limpiarTextBoxes() {
    // Etiquetas a eliminar (usa RegExp global e insensible)
    const etiquetas = [
        "Página",
        "Nombre de la Localidad",
        "Nombre del Municipio o Demarcación Territorial",
        "Nombre Comercial",
        "Nombre de la Colon",
        "Nombre",
        "Número Exterior",
        "  de la Entidad Federativa"
    ].map(e => new RegExp(e, "gi"));

    // Instancias cliente DevExpress (ClientInstanceName)
    const controls = [
        window.txtCalle,         // <dx:ASPxTextBox ClientInstanceName="txtCalle" ... />
        window.txtNoExterior,
        window.txtNoInterior,
        window.txtColonia,
        window.txtLocalidad,
        window.txtMunicipio,
        window.txtEstado,
        window.txtEntrecalle
    ];

    controls.forEach(ctrl => {
        if (ctrl && typeof ctrl.GetText === "function" && typeof ctrl.SetText === "function") {
            let texto = ctrl.GetText() || "";
            if (texto) {
                etiquetas.forEach(rx => { texto = texto.replace(rx, ""); });
                texto = texto.trim();
                ctrl.SetText(texto);
            }
        }
    });
}