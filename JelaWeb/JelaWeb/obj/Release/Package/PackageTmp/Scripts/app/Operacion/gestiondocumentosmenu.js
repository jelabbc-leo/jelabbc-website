/**
 * Scripts para la página de Gestión de Documentos
 */

function navigateToDocumentos(type) {
    // Aquí puedes agregar la lógica de navegación según el tipo
    // Por ejemplo, redirigir a diferentes páginas o mostrar modales
    switch(type) {
        case 'Condominios':
            // window.location.href = 'DocumentosCondominios.aspx';
            alert('Navegando a Gestión de Documentos - Condominios');
            break;
        case 'ServiciosMunicipales':
            // window.location.href = 'DocumentosServiciosMunicipales.aspx';
            alert('Navegando a Gestión de Documentos - Servicios Municipales');
            break;
        case 'Agro':
            // window.location.href = 'DocumentosAgro.aspx';
            alert('Navegando a Gestión de Documentos - Agro');
            break;
        default:
            console.warn('Tipo de documento no reconocido:', type);
            break;
    }
}

