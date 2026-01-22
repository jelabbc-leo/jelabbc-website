'Public Class CRUDDTO
'    Public Property Campos As New Dictionary(Of String, Object)

'    Default Public Property Item(key As String) As Object
'        Get
'            If Campos.ContainsKey(key) Then Return Campos(key)
'            Return Nothing
'        End Get
'        Set(value As Object)
'            Campos(key) = value
'        End Set
'    End Property

'End Class

Public Class CampoConTipo
    Public Property Valor As Object
    Public Property Tipo As String
End Class

Public Class CRUDDTO
    Private _campos As New Dictionary(Of String, CampoConTipo)

    ' Acceso directo al diccionario completo
    Public ReadOnly Property Campos As Dictionary(Of String, CampoConTipo)
        Get
            Return _campos
        End Get
    End Property

    ' Indexador para acceder solo al valor
    Default Public Property Item(key As String) As Object
        Get
            If _campos.ContainsKey(key) Then Return _campos(key).Valor
            Return Nothing
        End Get
        Set(value As Object)
            Dim tipo = If(value Is Nothing, "null", value.GetType().Name.ToLower())
            _campos(key) = New CampoConTipo With {.Valor = value, .Tipo = tipo}
        End Set
    End Property

    ' Obtener tipo de campo
    Public Function TipoDe(key As String) As String
        If _campos.ContainsKey(key) Then Return _campos(key).Tipo
        Return "desconocido"
    End Function
End Class
