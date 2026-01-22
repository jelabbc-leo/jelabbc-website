/**
 * Scripts para la página IOT
 */

function navigateToIOT(type) {
    // Aquí puedes agregar la lógica de navegación según el tipo
    // Por ejemplo, redirigir a diferentes páginas o mostrar modales
    switch(type) {
        case 'Home':
            // window.location.href = 'IOTHome.aspx';
            alert('Navegando a IOT Home');
            break;
        case 'Condominio':
            // window.location.href = 'IOTCondominio.aspx';
            alert('Navegando a IOT Condominio');
            break;
        case 'Invernadero':
            // window.location.href = 'IOTInvernadero.aspx';
            alert('Navegando a IOT Invernadero');
            break;
    }
}

