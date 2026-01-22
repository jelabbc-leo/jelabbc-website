function onToolbarClick(s, e) {
    switch (e.item.name) {
        case 'btnAgregar':
            popConceptos.Show();
            break;

        //case 'btnCancelDoc':
        //    CancelarDocumento();
        //    break;

        case 'btnExportar':
            // Dispara postback al servidor
            __doPostBack('ExportarGrid', '');
            break;

        //case 'btnLiberarDoc':
        //    LiberarTareas();
        //    break;
    }
}