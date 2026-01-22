Imports System

''' <summary>
''' DTO para Tickets v2 (Sistema tipo Klarna con IA)
''' </summary>
Public Class TicketV2DTO
    Public Property Id As Integer
    Public Property IdEntidad As Integer
    Public Property TicketIdExterno As String
    Public Property AsuntoCorto As String
    Public Property MensajeOriginal As String
    Public Property ResumenIA As String
    Public Property Canal As String
    Public Property NombreCompleto As String
    Public Property EmailCliente As String
    Public Property TelefonoCliente As String
    Public Property IdCliente As Integer?
    Public Property CategoriaAsignada As String
    Public Property SubcategoriaAsignada As String
    Public Property SentimientoDetectado As String
    Public Property PrioridadAsignada As String
    Public Property UrgenciaAsignada As String
    Public Property PuedeResolverIA As Boolean
    Public Property RespuestaIA As String
    Public Property Estado As String
    Public Property IdAgenteAsignado As Integer?
    Public Property NombreAgenteAsignado As String
    Public Property FechaAsignacionAgente As DateTime?
    Public Property FechaResolucion As DateTime?
    Public Property TiempoResolucionMinutos As Integer?
    Public Property SatisfaccionCliente As Integer?
    Public Property ComentarioSatisfaccion As String
    Public Property IdUsuarioCreacion As Integer
    Public Property FechaCreacion As DateTime
    Public Property FechaUltimaActualizacion As DateTime
    Public Property Activo As Boolean
End Class

''' <summary>
''' DTO para conversación de ticket
''' </summary>
Public Class TicketConversacionDTO
    Public Property Id As Integer
    Public Property IdTicket As Integer
    Public Property TipoMensaje As String
    Public Property Mensaje As String
    Public Property EsRespuestaIA As Boolean
    Public Property IdUsuarioEnvio As Integer?
    Public Property NombreUsuarioEnvio As String
    Public Property FechaEnvio As DateTime
    Public Property Leido As Boolean
    Public Property FechaLectura As DateTime?
End Class

''' <summary>
''' DTO para archivos adjuntos de ticket
''' </summary>
Public Class TicketArchivoDTO
    Public Property Id As Integer
    Public Property IdTicket As Integer
    Public Property IdMensajeConversacion As Integer?
    Public Property NombreArchivo As String
    Public Property NombreArchivoServidor As String
    Public Property RutaArchivo As String
    Public Property TipoMime As String
    Public Property TamanioBytes As Long?
    Public Property IdUsuarioSubida As Integer?
    Public Property FechaSubida As DateTime
    Public Property Activo As Boolean
End Class

''' <summary>
''' DTO para acciones/historial de ticket
''' </summary>
Public Class TicketAccionDTO
    Public Property Id As Integer
    Public Property IdTicket As Integer
    Public Property TipoAccion As String
    Public Property Descripcion As String
    Public Property ValorAnterior As String
    Public Property ValorNuevo As String
    Public Property IdUsuarioAccion As Integer?
    Public Property NombreUsuarioAccion As String
    Public Property EsAccionIA As Boolean
    Public Property FechaAccion As DateTime
    Public Property Metadata As String
End Class
