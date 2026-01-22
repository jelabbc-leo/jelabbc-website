Imports System

Public Class FitosanitarioDTO
    Public Property Id As Integer
    Public Property IdEntidad As Integer
    Public Property Nombre As String
    Public Property NombreComercial As String
    Public Property TipoProducto As String
    Public Property Fabricante As String
    Public Property IngredienteActivo As String
    Public Property Concentracion As String
    Public Property DosisRecomendada As String
    Public Property UnidadDosis As String
    Public Property TiempoCarencia As Integer?
    Public Property Toxicidad As String
    Public Property Stock As Decimal
    Public Property UnidadStock As String
    Public Property Activo As Boolean
    Public Property FechaCreacion As DateTime
    Public Property FechaModificacion As DateTime
End Class
