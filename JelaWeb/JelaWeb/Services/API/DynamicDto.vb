
Imports Newtonsoft.Json

Public Class CampoDto
    Public Property Valor As Object
    Public Property Tipo As String
End Class

Public Class DynamicDto
    <JsonProperty("Campos")>
    Public Property Campos As New Dictionary(Of String, CampoDto)

    ' Acceso directo al valor del campo
    Default Public Property Item(key As String) As Object
        Get
            If Campos.ContainsKey(key) Then
                Return Campos(key).Valor
            Else
                Return Nothing
            End If

        End Get

        Set(value As Object)
            Dim tipo = If(value Is Nothing, "null", value.GetType().FullName)

            Campos(key) = New CampoDto With {.Valor = value, .Tipo = tipo}
        End Set

    End Property

    ' Acceso directo al tipo del campo
    Public Function TipoDe(key As String) As String
        If Campos.ContainsKey(key) Then
            Return Campos(key).Tipo
        Else
            Return "desconocido"
        End If
    End Function
End Class
