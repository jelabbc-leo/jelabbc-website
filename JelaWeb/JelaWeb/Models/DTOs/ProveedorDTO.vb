Imports System

''' <summary>
''' DTO para el catálogo de Proveedores
''' </summary>
Public Class ProveedorDTO
    Public Property Id As Integer
    Public Property IdEntidad As Integer
    Public Property RazonSocial As String
    Public Property NombreComercial As String
    Public Property RFC As String
    Public Property Telefono As String
    Public Property Email As String
    Public Property SitioWeb As String

    ' Dirección
    Public Property Calle As String
    Public Property NumeroExterior As String
    Public Property NumeroInterior As String
    Public Property Colonia As String
    Public Property CodigoPostal As String
    Public Property Ciudad As String
    Public Property Estado As String
    Public Property Pais As String

    ' Contacto principal
    Public Property NombreContacto As String
    Public Property TelefonoContacto As String
    Public Property EmailContacto As String
    Public Property CargoContacto As String

    ' Evaluación
    Public Property Calificacion As Decimal?
    Public Property NumeroEvaluaciones As Integer

    ' Control
    Public Property Activo As Boolean
    Public Property FechaCreacion As DateTime
    Public Property FechaModificacion As DateTime

    ''' <summary>
    ''' Dirección completa formateada
    ''' </summary>
    Public ReadOnly Property DireccionCompleta As String
        Get
            Dim partes As New List(Of String)

            If Not String.IsNullOrWhiteSpace(Calle) Then partes.Add(Calle)
            If Not String.IsNullOrWhiteSpace(NumeroExterior) Then partes.Add($"#{NumeroExterior}")
            If Not String.IsNullOrWhiteSpace(Colonia) Then partes.Add(Colonia)
            If Not String.IsNullOrWhiteSpace(Ciudad) Then partes.Add(Ciudad)
            If Not String.IsNullOrWhiteSpace(Estado) Then partes.Add(Estado)
            If Not String.IsNullOrWhiteSpace(CodigoPostal) Then partes.Add($"C.P. {CodigoPostal}")
            Return String.Join(", ", partes)
        End Get
    End Property
End Class
