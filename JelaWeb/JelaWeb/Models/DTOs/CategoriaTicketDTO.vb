Imports System

''' <summary>
''' DTO para el catálogo de Categorías de Tickets
''' </summary>
Public Class CategoriaTicketDTO
    Public Property Id As Integer
    Public Property IdEntidad As Integer
    Public Property Nombre As String
    Public Property Descripcion As String
    Public Property IconoClase As String
    Public Property Color As String
    Public Property Activo As Boolean
    Public Property FechaCreacion As DateTime
    Public Property FechaModificacion As DateTime

    ''' <summary>
    ''' Para jerarquía padre-hijo (opcional)
    ''' </summary>
    Public Property CategoriaPadreId As Integer?
    Public Property CategoriaPadreNombre As String

    ''' <summary>
    ''' Configuración de SLA asociada
    ''' </summary>
    Public Property TieneConfiguracionSLA As Boolean
End Class

''' <summary>
''' DTO para configuración de SLA por categoría
''' </summary>
Public Class ConfiguracionSLADTO
    Public Property Id As Integer
    Public Property IdEntidad As Integer
    Public Property CategoriaId As Integer
    Public Property Prioridad As String
    Public Property TiempoRespuestaMinutos As Integer
    Public Property TiempoResolucionMinutos As Integer
    Public Property Activo As Boolean

    ''' <summary>
    ''' Propiedades calculadas para visualización
    ''' </summary>
    Public ReadOnly Property TiempoRespuestaTexto As String
        Get
            Return FormatearTiempo(TiempoRespuestaMinutos)
        End Get
    End Property

    Public ReadOnly Property TiempoResolucionTexto As String
        Get
            Return FormatearTiempo(TiempoResolucionMinutos)
        End Get
    End Property

    Private Function FormatearTiempo(minutos As Integer) As String
        If minutos < 60 Then
            Return $"{minutos} min"
        ElseIf minutos < 1440 Then
            Dim horas = Math.Floor(minutos / 60)

            Return $"{horas} hrs"
        Else
            Dim dias = Math.Floor(minutos / 1440)

            Return $"{dias} días"
        End If
    End Function
End Class
