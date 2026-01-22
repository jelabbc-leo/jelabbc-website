Imports System.Data
Imports System.Collections.Generic
Imports JelaWeb.Utilities

''' <summary>
''' Servicio para gestión de archivos en el visor
''' </summary>
Public Class VisorArchivoService

    ''' <summary>
    ''' Obtiene la imagen INE de un residente
    ''' </summary>
    Public Shared Function ObtenerImagenINEResidente(residenteId As Integer) As String

        Try
            Dim query As String = "SELECT ImagenINE FROM cat_residentes WHERE Id = " & residenteId
            Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)

            If dt.Rows.Count > 0 AndAlso Not IsDBNull(dt.Rows(0)("ImagenINE")) Then
                Return dt.Rows(0)("ImagenINE").ToString()
            End If

            Return Nothing

        Catch ex As Exception
            Logger.LogError("VisorArchivoService.ObtenerImagenINEResidente", ex)
            Return Nothing

        End Try
    End Function

    ''' <summary>
    ''' Obtiene la tarjeta de circulación de un vehículo
    ''' </summary>
    Public Shared Function ObtenerTarjetaCirculacionVehiculo(vehiculoId As Integer) As String

        Try
            Dim query As String = "SELECT TarjetaCirculacionBase64 FROM cat_vehiculos_unidad WHERE Id = " & vehiculoId
            Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)

            If dt.Rows.Count > 0 AndAlso Not IsDBNull(dt.Rows(0)("TarjetaCirculacionBase64")) Then
                Return dt.Rows(0)("TarjetaCirculacionBase64").ToString()
            End If

            Return Nothing

        Catch ex As Exception
            Logger.LogError("VisorArchivoService.ObtenerTarjetaCirculacionVehiculo", ex)
            Return Nothing

        End Try
    End Function

    ''' <summary>
    ''' Obtiene un archivo de residente por ID
    ''' </summary>
    Public Shared Function ObtenerArchivoResidente(archivoId As Integer) As Dictionary(Of String, String)

        Try
            Dim query As String = "SELECT ArchivoBase64, NombreArchivo, TipoMime FROM cat_residente_archivos WHERE Id = " & archivoId & " AND Activo = 1"
            Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)

            If dt.Rows.Count > 0 Then
                Dim resultado As New Dictionary(Of String, String)
                resultado("ArchivoBase64") = If(Not IsDBNull(dt.Rows(0)("ArchivoBase64")), dt.Rows(0)("ArchivoBase64").ToString(), "")
                resultado("NombreArchivo") = If(Not IsDBNull(dt.Rows(0)("NombreArchivo")), dt.Rows(0)("NombreArchivo").ToString(), "")
                resultado("TipoMime") = If(Not IsDBNull(dt.Rows(0)("TipoMime")), dt.Rows(0)("TipoMime").ToString(), "")
                Return resultado
            End If

            Return Nothing

        Catch ex As Exception
            Logger.LogError("VisorArchivoService.ObtenerArchivoResidente", ex)
            Return Nothing

        End Try
    End Function

    ''' <summary>
    ''' Obtiene un archivo de vehículo por ID
    ''' </summary>
    Public Shared Function ObtenerArchivoVehiculo(archivoId As Integer) As Dictionary(Of String, String)

        Try
            Dim query As String = "SELECT ArchivoBase64, NombreArchivo, TipoMime FROM cat_vehiculo_archivos WHERE Id = " & archivoId & " AND Activo = 1"
            Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)

            If dt.Rows.Count > 0 Then
                Dim resultado As New Dictionary(Of String, String)
                resultado("ArchivoBase64") = If(Not IsDBNull(dt.Rows(0)("ArchivoBase64")), dt.Rows(0)("ArchivoBase64").ToString(), "")
                resultado("NombreArchivo") = If(Not IsDBNull(dt.Rows(0)("NombreArchivo")), dt.Rows(0)("NombreArchivo").ToString(), "")
                resultado("TipoMime") = If(Not IsDBNull(dt.Rows(0)("TipoMime")), dt.Rows(0)("TipoMime").ToString(), "")
                Return resultado
            End If

            Return Nothing

        Catch ex As Exception
            Logger.LogError("VisorArchivoService.ObtenerArchivoVehiculo", ex)
            Return Nothing

        End Try
    End Function

    ''' <summary>
    ''' Obtiene un archivo de documento por ID
    ''' </summary>
    Public Shared Function ObtenerArchivoDocumento(archivoId As Integer) As Dictionary(Of String, String)

        Try
            Dim query As String = "SELECT ArchivoBase64, NombreArchivo, TipoMime FROM cat_documento_unidad_archivos WHERE Id = " & archivoId & " AND Activo = 1"
            Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)

            If dt.Rows.Count > 0 Then
                Dim resultado As New Dictionary(Of String, String)
                resultado("ArchivoBase64") = If(Not IsDBNull(dt.Rows(0)("ArchivoBase64")), dt.Rows(0)("ArchivoBase64").ToString(), "")
                resultado("NombreArchivo") = If(Not IsDBNull(dt.Rows(0)("NombreArchivo")), dt.Rows(0)("NombreArchivo").ToString(), "")
                resultado("TipoMime") = If(Not IsDBNull(dt.Rows(0)("TipoMime")), dt.Rows(0)("TipoMime").ToString(), "")
                Return resultado
            End If

            Return Nothing

        Catch ex As Exception
            Logger.LogError("VisorArchivoService.ObtenerArchivoDocumento", ex)
            Return Nothing

        End Try
    End Function

    ''' <summary>
    ''' Obtiene un documento de unidad por ID
    ''' </summary>
    Public Shared Function ObtenerDocumentoUnidad(documentoId As Integer) As Dictionary(Of String, Object)

        Try
            Dim registro As DataRow = DynamicCrudService.ObtenerPorId("cat_documentos_unidad", documentoId)

            If registro Is Nothing Then Return Nothing

            Dim datos As New Dictionary(Of String, Object)

            For Each col As DataColumn In registro.Table.Columns
                datos(col.ColumnName) = If(IsDBNull(registro(col)), Nothing, registro(col))

            Next

            Return datos

        Catch ex As Exception
            Logger.LogError("VisorArchivoService.ObtenerDocumentoUnidad", ex)
            Return Nothing

        End Try
    End Function

End Class
