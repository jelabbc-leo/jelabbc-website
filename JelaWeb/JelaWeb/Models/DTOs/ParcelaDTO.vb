Imports System

Public Class ParcelaDTO
    Public Property Id As Integer
    Public Property IdEntidad As Integer
    Public Property Nombre As String
    Public Property Descripcion As String
    Public Property Superficie As Decimal
    Public Property UnidadSuperficie As String
    Public Property Latitud As Decimal?
    Public Property Longitud As Decimal?
    Public Property EntidadId As Integer
    Public Property EntidadNombre As String
    Public Property Activo As Boolean
    Public Property FechaCreacion As DateTime
    Public Property FechaModificacion As DateTime
End Class
