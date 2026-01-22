Imports Newtonsoft.Json.Linq

' Clase utilitaria para extraer mensajes de error desde respuestas JSON de API
Public NotInheritable Class ApiErrorHelper
    Private Sub New()
        ' Evitar instanciación
    End Sub

    ''' <summary>
    ''' Extrae un mensaje de error desde un contenido JSON devuelto por una API.
    ''' Soporta tanto "mensaje" como "ExceptionMessage".
    ''' </summary>
    ''' <param name="contenido">Texto de la respuesta del API (posiblemente JSON)</param>
    ''' <returns>Mensaje de error extraído o cadena vacía si no se pudo interpretar</returns>
    Public Shared Function ExtraerMensaje(contenido As String) As String

        Try
            Dim jsonStart = contenido.IndexOf("{"c)

            If jsonStart >= 0 Then
                Dim jsonSolo = contenido.Substring(jsonStart)
                Dim jObj = JObject.Parse(jsonSolo)
                Dim mensaje = jObj("mensaje")?.ToString()

                If Not String.IsNullOrWhiteSpace(mensaje) Then Return mensaje

                Dim exMsg = jObj("ExceptionMessage")?.ToString()

                If Not String.IsNullOrWhiteSpace(exMsg) Then Return exMsg
            End If

        Catch
            ' No es JSON válido o no se pudo parsear

        End Try
        Return String.Empty
    End Function
End Class
