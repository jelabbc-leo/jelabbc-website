Imports System.Text
Imports System.Web

''' <summary>
''' Clase helper para construir queries SQL de forma segura
''' Previene SQL Injection mediante validación y escape de valores
''' </summary>
Public NotInheritable Class QueryBuilder
    Private Sub New()
        ' Clase estática, no instanciable
    End Sub

    ''' <summary>
    ''' Escapa un valor string para uso seguro en SQL
    ''' </summary>
    Public Shared Function EscapeSqlString(value As String) As String
        If String.IsNullOrEmpty(value) Then Return "''"

        ' Reemplazar comillas simples duplicándolas (estándar SQL)
        Dim escaped = value.Replace("'", "''")

        ' Validar que no contenga caracteres peligrosos
        Dim validation = InputValidator.ValidateSqlInput(value)

        If Not validation.IsValid Then
            Throw New ArgumentException($"Valor contiene caracteres SQL peligrosos: {validation.Message}")
        End If

        Return "'" & escaped & "'"
    End Function

    ''' <summary>
    ''' Convierte un entero a string seguro para SQL
    ''' </summary>
    Public Shared Function EscapeSqlInteger(value As Integer) As String
        Return value.ToString()
    End Function

    ''' <summary>
    ''' Convierte un entero nullable a string seguro para SQL
    ''' </summary>
    Public Shared Function EscapeSqlInteger(value As Integer?) As String
        If value.HasValue Then
            Return value.Value.ToString()
        End If
        Return "NULL"
    End Function

    ''' <summary>
    ''' Convierte una fecha a string seguro para SQL (formato SQL Server)
    ''' </summary>
    Public Shared Function EscapeSqlDate(value As Date) As String
        Return "'" & value.ToString("yyyy-MM-dd HH:mm:ss") & "'"
    End Function

    ''' <summary>
    ''' Convierte una fecha nullable a string seguro para SQL
    ''' </summary>
    Public Shared Function EscapeSqlDate(value As Date?) As String
        If value.HasValue Then
            Return EscapeSqlDate(value.Value)
        End If
        Return "NULL"
    End Function

    ''' <summary>
    ''' Convierte un decimal a string seguro para SQL
    ''' </summary>
    Public Shared Function EscapeSqlDecimal(value As Decimal) As String
        Return value.ToString(System.Globalization.CultureInfo.InvariantCulture)
    End Function

    ''' <summary>
    ''' Convierte un booleano a string seguro para SQL (1 o 0)
    ''' </summary>
    Public Shared Function EscapeSqlBoolean(value As Boolean) As String
        Return If(value, "1", "0")
    End Function

    ''' <summary>
    ''' Valida y sanitiza un nombre de columna o tabla
    ''' Solo permite caracteres alfanuméricos, guiones bajos y puntos
    ''' </summary>
    Public Shared Function ValidateIdentifier(identifier As String) As Boolean
        If String.IsNullOrEmpty(identifier) Then Return False

        ' Solo permitir caracteres alfanuméricos, guiones bajos y puntos
        Dim regex As New System.Text.RegularExpressions.Regex("^[a-zA-Z0-9_\.]+$")

        Return regex.IsMatch(identifier)
    End Function

End Class
