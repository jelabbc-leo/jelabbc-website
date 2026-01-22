<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Ingreso.aspx.vb" Inherits="JelaWeb.Ingreso" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=5.0, user-scalable=yes" />
    <title>Ingreso Jela</title>
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" rel="stylesheet" />
    <link href="~/Content/Styles/login.css" rel="stylesheet" />
    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', function() {
            var inputs = document.querySelectorAll('.modern-input');
            inputs.forEach(function(input) {
                input.addEventListener('focus', function() {
                    this.closest('.input-group').classList.add('focused');
                });
                input.addEventListener('blur', function() {
                    this.closest('.input-group').classList.remove('focused');
                });
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-wrapper">
            <div class="login-container">
                <img src="<%= ResolveUrl("/Content/Images/LogoJelaBBC.png") %>" class="logo" alt="JELA Logo" />

                <div class="LabelError" id="errorContainer" runat="server" style="display: none;">
                    <asp:Label ID="lblError" runat="server" Text=""></asp:Label>
                </div>

                <div class="input-group">
                    <i class="fa fa-user icon"></i>
                    <asp:TextBox ID="txtUsername" runat="server" placeholder="Usuario" CssClass="modern-input" />
                </div>

                <div class="input-group">
                    <i class="fa fa-lock icon"></i>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="Contraseña" CssClass="modern-input" />
                </div>

                <asp:Button ID="btnLogin" runat="server" Text="Ingresar" CssClass="login-button" OnClick="btnLogin_Click" />

                <div class="copyright">
                    <p>&copy; <%= DateTime.Now.Year %> JELA. Todos los derechos reservados.</p>
                </div>

            </div>
        </div>
    </form>
</body>
</html>

