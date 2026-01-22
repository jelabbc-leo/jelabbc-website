Imports System.IO
Imports System.Text
Imports System.Data
Imports JelaWeb.Infrastructure.Helpers
Imports JelaWeb.Services
Imports JelaWeb.Utilities

''' <summary>
''' Página visor para mostrar archivos almacenados en Base64
''' Soporta PDF, JPG, PNG
''' </summary>
Public Class VisorArchivo
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try
            ' Obtener parámetros de la URL
            Dim tipo As String = Request.QueryString("tipo") ' residente, vehiculo, documento
            Dim idStr As String = Request.QueryString("id")
            Dim id As Integer = 0

            If String.IsNullOrEmpty(tipo) OrElse String.IsNullOrEmpty(idStr) OrElse Not Integer.TryParse(idStr, id) Then
                MostrarError("Parámetros inválidos. Se requiere tipo e id.")
                Return
            End If

            ' Obtener el archivo en Base64 desde la base de datos
            Dim archivoBase64 As String = Nothing
            Dim tipoArchivo As String = Nothing
            Dim nombreArchivo As String = Nothing

            ' Obtener archivoId si existe (para múltiples archivos)
            Dim archivoIdStr As String = Request.QueryString("archivoId")
            Dim archivoId As Integer = 0

            Select Case tipo.ToLower()

                Case "residente"
                    If Not String.IsNullOrEmpty(archivoIdStr) AndAlso Integer.TryParse(archivoIdStr, archivoId) Then
                        ' Obtener archivo específico desde cat_residente_archivos
                        Dim datosArchivo = ObtenerArchivoResidente(archivoId)

                        If datosArchivo IsNot Nothing Then
                            archivoBase64 = datosArchivo("ArchivoBase64")
                            nombreArchivo = datosArchivo("NombreArchivo")
                            tipoArchivo = ObtenerTipoArchivoPorExtension(nombreArchivo)
                        Else
                            MostrarError("No se encontró el archivo solicitado.")
                            Return
                        End If
                    Else
                        ' Retrocompatibilidad: buscar en cat_residentes.ImagenINE si existe
                        archivoBase64 = ObtenerImagenINEResidente(id)
                        tipoArchivo = "image"
                        nombreArchivo = "INE_Residente_" & id.ToString()
                    End If

                Case "vehiculo"
                    If Not String.IsNullOrEmpty(archivoIdStr) AndAlso Integer.TryParse(archivoIdStr, archivoId) Then
                        ' Obtener archivo específico desde cat_vehiculo_archivos
                        Dim datosArchivo = ObtenerArchivoVehiculo(archivoId)

                        If datosArchivo IsNot Nothing Then
                            archivoBase64 = datosArchivo("ArchivoBase64")
                            nombreArchivo = datosArchivo("NombreArchivo")
                            tipoArchivo = ObtenerTipoArchivoPorExtension(nombreArchivo)
                        Else
                            MostrarError("No se encontró el archivo solicitado.")
                            Return
                        End If
                    Else
                        ' Retrocompatibilidad: buscar en cat_vehiculos_unidad.TarjetaCirculacionBase64
                        archivoBase64 = ObtenerTarjetaCirculacionVehiculo(id)
                        tipoArchivo = "image"
                        nombreArchivo = "Tarjeta_Vehiculo_" & id.ToString()
                    End If

                Case "documento"
                    If Not String.IsNullOrEmpty(archivoIdStr) AndAlso Integer.TryParse(archivoIdStr, archivoId) Then
                        ' Obtener archivo específico desde cat_documento_unidad_archivos
                        Dim datosArchivo = ObtenerArchivoDocumento(archivoId)

                        If datosArchivo IsNot Nothing Then
                            archivoBase64 = datosArchivo("ArchivoBase64")
                            nombreArchivo = datosArchivo("NombreArchivo")
                            tipoArchivo = ObtenerTipoArchivoPorExtension(nombreArchivo)
                        Else
                            MostrarError("No se encontró el archivo solicitado.")
                            Return
                        End If
                    Else
                        ' Retrocompatibilidad: buscar en cat_documentos_unidad.ArchivoBase64
                        Dim datos = ObtenerDocumento(id)

                        If datos IsNot Nothing AndAlso datos.ContainsKey("ArchivoBase64") Then
                            archivoBase64 = datos("ArchivoBase64")
                            tipoArchivo = ObtenerTipoArchivoPorExtension(datos("NombreArchivo"))
                            nombreArchivo = datos("NombreArchivo")
                        Else
                            MostrarError("No se encontró el archivo solicitado.")
                            Return
                        End If
                    End If

                Case Else
                    MostrarError("Tipo de archivo no válido. Use: residente, vehiculo o documento")
                    Return

            End Select

            If String.IsNullOrEmpty(archivoBase64) Then
                MostrarError("No se encontró el archivo solicitado.")
                Return
            End If

            ' Determinar si es PDF o imagen
            Dim esPDF As Boolean = tipoArchivo = "pdf" OrElse nombreArchivo.ToLower().EndsWith(".pdf")

            ' Convertir Base64 a bytes
            Dim bytesArchivo As Byte() = Convert.FromBase64String(archivoBase64)

            ' Determinar el Content-Type
            Dim contentType As String = "application/octet-stream"

            If esPDF Then
                contentType = "application/pdf"
            ElseIf nombreArchivo.ToLower().EndsWith(".jpg") OrElse nombreArchivo.ToLower().EndsWith(".jpeg") Then
                contentType = "image/jpeg"
            ElseIf nombreArchivo.ToLower().EndsWith(".png") Then
                contentType = "image/png"
            End If

            ' Establecer headers para la respuesta
            Response.Clear()
            Response.ContentType = contentType
            Response.AddHeader("Content-Disposition", "inline; filename=" & nombreArchivo)
            Response.AddHeader("Content-Length", bytesArchivo.Length.ToString())
            Response.BinaryWrite(bytesArchivo)
            Response.End()

        Catch ex As Exception
            Logger.LogError("VisorArchivo.Page_Load", ex)
            MostrarError("Error al cargar el archivo: " & ex.Message)

        End Try
    End Sub

    ''' <summary>
    ''' Obtiene la imagen INE de un residente desde el servicio
    ''' </summary>
    Private Function ObtenerImagenINEResidente(residenteId As Integer) As String
        Return VisorArchivoService.ObtenerImagenINEResidente(residenteId)
    End Function

    ''' <summary>
    ''' Obtiene la tarjeta de circulación de un vehículo desde el servicio
    ''' </summary>
    Private Function ObtenerTarjetaCirculacionVehiculo(vehiculoId As Integer) As String
        Return VisorArchivoService.ObtenerTarjetaCirculacionVehiculo(vehiculoId)
    End Function

    ''' <summary>
    ''' Obtiene un documento desde el servicio
    ''' </summary>
    Private Function ObtenerDocumento(documentoId As Integer) As Dictionary(Of String, Object)
        Return VisorArchivoService.ObtenerDocumentoUnidad(documentoId)
    End Function

    ''' <summary>
    ''' Determina el tipo de archivo por la extensión del nombre
    ''' </summary>
    Private Function ObtenerTipoArchivoPorExtension(nombreArchivo As String) As String
        If String.IsNullOrEmpty(nombreArchivo) Then Return "image"

        Dim extension As String = Path.GetExtension(nombreArchivo).ToLower()

        Select Case extension

            Case ".pdf"
                Return "pdf"

            Case ".jpg", ".jpeg"
                Return "image"

            Case ".png"
                Return "image"

            Case Else
                Return "image"

        End Select
    End Function

    ''' <summary>
    ''' Obtiene un archivo específico de residente desde el servicio
    ''' </summary>
    Private Function ObtenerArchivoResidente(archivoId As Integer) As Dictionary(Of String, Object)
        Dim resultado = VisorArchivoService.ObtenerArchivoResidente(archivoId)
        If resultado Is Nothing Then Return Nothing

        ' Convertir Dictionary(Of String, String) a Dictionary(Of String, Object)
        Dim datos As New Dictionary(Of String, Object)
        For Each kvp In resultado
            datos(kvp.Key) = kvp.Value
        Next
        Return datos
    End Function

    ''' <summary>
    ''' Obtiene un archivo específico de vehículo desde el servicio
    ''' </summary>
    Private Function ObtenerArchivoVehiculo(archivoId As Integer) As Dictionary(Of String, Object)
        Dim resultado = VisorArchivoService.ObtenerArchivoVehiculo(archivoId)
        If resultado Is Nothing Then Return Nothing

        ' Convertir Dictionary(Of String, String) a Dictionary(Of String, Object)
        Dim datos As New Dictionary(Of String, Object)
        For Each kvp In resultado
            datos(kvp.Key) = kvp.Value
        Next
        Return datos
    End Function

    ''' <summary>
    ''' Obtiene un archivo específico de documento desde el servicio
    ''' </summary>
    Private Function ObtenerArchivoDocumento(archivoId As Integer) As Dictionary(Of String, Object)
        Dim resultado = VisorArchivoService.ObtenerArchivoDocumento(archivoId)
        If resultado Is Nothing Then Return Nothing

        ' Convertir Dictionary(Of String, String) a Dictionary(Of String, Object)
        Dim datos As New Dictionary(Of String, Object)
        For Each kvp In resultado
            datos(kvp.Key) = kvp.Value
        Next
        Return datos
    End Function

    ''' <summary>
    ''' Muestra un mensaje de error como HTML
    ''' </summary>
    Private Sub MostrarError(mensaje As String)
        Response.Clear()
        Response.ContentType = "text/html"
        Response.Write("<!DOCTYPE html><html><head><title>Error</title></head><body style=""font-family:Arial;padding:20px;""><h2>Error</h2><p>" & Server.HtmlEncode(mensaje) & "</p><button onclick=""window.close();"">Cerrar</button></body></html>")
        Response.End()
    End Sub

End Class
