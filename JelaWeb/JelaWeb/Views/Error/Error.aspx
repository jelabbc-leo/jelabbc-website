<%@ Page Title="Error" Language="vb" AutoEventWireup="false" CodeBehind="Error.aspx.vb" Inherits="JelaWeb.ErrorPage" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Error</title>
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" rel="stylesheet" />
    <link href="~/Content/Styles/error.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="error-container">
            <i id="errorIcon" runat="server" class="error-icon"></i>
            <h1 id="errorCode" runat="server" class="error-code">Error</h1>
            <h2 id="errorMessage" runat="server" class="error-message">Ha ocurrido un error</h2>
            <p id="errorDescription" runat="server" class="error-description">
                Por favor, intente nuevamente más tarde.
            </p>
            <a id="btnAction" runat="server" href="~/Views/Inicio.aspx" class="btn-action">
                <i id="btnIcon" runat="server"></i> <span id="btnText" runat="server">Volver al inicio</span>
            </a>
        </div>
    </form>
</body>
</html>

