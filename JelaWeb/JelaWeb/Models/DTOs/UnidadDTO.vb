Imports System

Public Class UnidadDTO
    Public Property Id As Integer
    Public Property Codigo As String
    Public Property Nombre As String
    Public Property EntidadId As Integer
    Public Property EntidadNombre As String
    Public Property Torre As String
    Public Property Edificio As String
    Public Property Piso As String
    Public Property Numero As String
    Public Property Superficie As Decimal?
    Public Property NumeroResidentes As Integer
    Public Property Activo As Boolean
    Public Property FechaCreacion As DateTime
    Public Property FechaModificacion As DateTime

    Public ReadOnly Property NombreCompleto As String
        Get
            Dim partes As New List(Of String)

            If Not String.IsNullOrWhiteSpace(Torre) Then partes.Add($"Torre {Torre}")
            If Not String.IsNullOrWhiteSpace(Edificio) Then partes.Add($"Edificio {Edificio}")
            If Not String.IsNullOrWhiteSpace(Numero) Then partes.Add($"Unidad {Numero}")
            Return String.Join(" - ", partes)
        End Get
    End Property
End Class
