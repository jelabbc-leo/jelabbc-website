Imports System.Collections.Generic

''' <summary>
''' DTO para formulario dinámico
''' </summary>
Public Class FormularioDTO
    Public Property FormularioId As Integer
    Public Property IdEntidad As Integer
    Public Property NombreFormulario As String
    Public Property Descripcion As String
    Public Property Plataformas As String
    Public Property Estado As String
    Public Property Version As Integer
    Public Property RequiereFirma As Boolean
    Public Property RequiereFoto As Boolean
    Public Property TiempoEstimadoMinutos As Integer
    Public Property Instrucciones As String
    Public Property FechaCreacion As DateTime
    Public Property CreadoPor As Integer
End Class

''' <summary>
''' DTO para campo de formulario
''' </summary>
Public Class CampoFormularioDTO
    Public Property CampoId As Integer
    Public Property FormularioId As Integer
    Public Property NombreCampo As String
    Public Property EtiquetaCampo As String
    Public Property TipoCampo As String
    Public Property EsRequerido As Boolean
    Public Property EsVisible As Boolean
    Public Property PosicionOrden As Integer
    Public Property Placeholder As String
    Public Property LongitudMaxima As Integer?
    Public Property AnchoColumna As Integer
    Public Property AlturaCampo As Integer?
    Public Property Seccion As String
    Public Property Opciones As List(Of OpcionCampoDTO)

    Public Sub New()
        Opciones = New List(Of OpcionCampoDTO)()
    End Sub
End Class

''' <summary>
''' DTO para opción de campo dropdown/radio
''' </summary>
Public Class OpcionCampoDTO
    Public Property OpcionId As Integer
    Public Property CampoId As Integer
    Public Property ValorOpcion As String
    Public Property EtiquetaOpcion As String
    Public Property PosicionOrden As Integer
    Public Property EsDefault As Boolean
End Class

''' <summary>
''' DTO para formulario completo con campos y opciones
''' </summary>
Public Class FormularioCompletoDTO
    Public Property Formulario As FormularioDTO
    Public Property Campos As List(Of CampoFormularioDTO)

    Public Sub New()
        Campos = New List(Of CampoFormularioDTO)()
    End Sub
End Class

''' <summary>
''' DTO para campo extraído de PDF
''' </summary>
Public Class CampoExtraidoDTO
    Public Property NombreCampo As String
    Public Property EtiquetaCampo As String
    Public Property TipoCampo As String
    Public Property ValorEjemplo As String
    Public Property Seccion As String
End Class

''' <summary>
''' DTO para respuesta de formulario
''' </summary>
Public Class RespuestaFormularioDTO
    Public Property RespuestaId As Integer
    Public Property FormularioId As Integer
    Public Property FalloId As Integer
    Public Property UsuarioId As Integer
    Public Property FechaCaptura As DateTime
    Public Property Estado As String
    Public Property Latitud As Decimal?
    Public Property Longitud As Decimal?
    Public Property UrlFirma As String
    Public Property UrlFoto As String
    Public Property Respuestas As List(Of RespuestaCampoDTO)

    Public Sub New()
        Respuestas = New List(Of RespuestaCampoDTO)()
    End Sub
End Class

''' <summary>
''' DTO para respuesta de campo individual
''' </summary>
Public Class RespuestaCampoDTO
    Public Property RespuestaCampoId As Integer
    Public Property RespuestaId As Integer
    Public Property CampoId As Integer
    Public Property ValorTexto As String
    Public Property ValorNumerico As Decimal?
    Public Property ValorFecha As DateTime?
    Public Property ValorBooleano As Boolean?
    Public Property UrlArchivo As String
End Class
