Imports System

Public Class TipoSensorDTO
    Public Property Id As Integer
    Public Property IdEntidad As Integer
    Public Property Nombre As String
    Public Property Descripcion As String
    Public Property UnidadMedida As String
    Public Property SimboloUnidad As String
    Public Property UmbralMinimo As Decimal?
    Public Property UmbralMaximo As Decimal?
    Public Property Activo As Boolean
    Public Property FechaCreacion As DateTime
    Public Property FechaModificacion As DateTime
End Class
