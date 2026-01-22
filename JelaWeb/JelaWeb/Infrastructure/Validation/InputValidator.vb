Imports System.Text.RegularExpressions

''' <summary>
''' Clase para validación y sanitización de entrada de datos
''' Previene SQL Injection, XSS y otros ataques comunes
''' </summary>
Public NotInheritable Class InputValidator
    Private Sub New()
        ' Clase estática, no instanciable
    End Sub

    ''' <summary>
    ''' Valida y sanitiza texto para prevenir XSS
    ''' </summary>
    Public Shared Function SanitizeText(input As String) As String
        If String.IsNullOrEmpty(input) Then Return String.Empty

        Dim sanitized = input.Trim()

        ' Remover scripts
        sanitized = Regex.Replace(sanitized, "<script[^>]*>.*?</script>", "", RegexOptions.IgnoreCase Or RegexOptions.Singleline)
        sanitized = Regex.Replace(sanitized, "<iframe[^>]*>.*?</iframe>", "", RegexOptions.IgnoreCase Or RegexOptions.Singleline)
        sanitized = Regex.Replace(sanitized, "<object[^>]*>.*?</object>", "", RegexOptions.IgnoreCase Or RegexOptions.Singleline)
        sanitized = Regex.Replace(sanitized, "<embed[^>]*>.*?</embed>", "", RegexOptions.IgnoreCase Or RegexOptions.Singleline)

        ' Remover eventos JavaScript
        sanitized = Regex.Replace(sanitized, "on\w+\s*=", "", RegexOptions.IgnoreCase)
        sanitized = Regex.Replace(sanitized, "javascript:", "", RegexOptions.IgnoreCase)
        sanitized = Regex.Replace(sanitized, "vbscript:", "", RegexOptions.IgnoreCase)

        ' Remover expresiones de estilo peligrosas
        sanitized = Regex.Replace(sanitized, "expression\s*\(", "", RegexOptions.IgnoreCase)

        Return sanitized
    End Function

    ''' <summary>
    ''' Valida que una cadena no contenga patrones SQL peligrosos
    ''' </summary>
    Public Shared Function ValidateSqlInput(input As String) As ValidationResult
        If String.IsNullOrEmpty(input) Then
            Return New ValidationResult With {.IsValid = True, .Message = ""}
        End If

        ' Lista de patrones peligrosos SQL
        Dim dangerousPatterns As New Dictionary(Of String, String)
        dangerousPatterns.Add("';", "Caracteres de terminación SQL no permitidos")
        dangerousPatterns.Add("--", "Comentarios SQL no permitidos")
        dangerousPatterns.Add("/*", "Comentarios SQL no permitidos")
        dangerousPatterns.Add("*/", "Comentarios SQL no permitidos")
        dangerousPatterns.Add("xp_", "Procedimientos almacenados extendidos no permitidos")
        dangerousPatterns.Add("sp_", "Procedimientos almacenados del sistema no permitidos")
        dangerousPatterns.Add("exec", "Comandos de ejecución no permitidos")
        dangerousPatterns.Add("execute", "Comandos de ejecución no permitidos")
        dangerousPatterns.Add("union", "Comandos UNION no permitidos")
        dangerousPatterns.Add("select", "Comandos SELECT no permitidos en este contexto")
        dangerousPatterns.Add("insert", "Comandos INSERT no permitidos")
        dangerousPatterns.Add("update", "Comandos UPDATE no permitidos")
        dangerousPatterns.Add("delete", "Comandos DELETE no permitidos")
        dangerousPatterns.Add("drop", "Comandos DROP no permitidos")
        dangerousPatterns.Add("create", "Comandos CREATE no permitidos")
        dangerousPatterns.Add("alter", "Comandos ALTER no permitidos")
        dangerousPatterns.Add("truncate", "Comandos TRUNCATE no permitidos")

        Dim inputLower = input.ToLower()

        For Each pattern In dangerousPatterns
            If inputLower.Contains(pattern.Key) Then
                Return New ValidationResult With {
                    .IsValid = False,
                    .Message = pattern.Value
                }
            End If

        Next

        Return New ValidationResult With {.IsValid = True, .Message = ""}
    End Function

    ''' <summary>
    ''' Valida formato de email
    ''' </summary>
    Public Shared Function ValidateEmail(email As String) As ValidationResult
        If String.IsNullOrEmpty(email) Then
            Return New ValidationResult With {.IsValid = False, .Message = "El email no puede estar vacío"}
        End If

        Try
            Dim emailRegex As New Regex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")

            If emailRegex.IsMatch(email) Then
                Return New ValidationResult With {.IsValid = True, .Message = ""}
            Else
                Return New ValidationResult With {.IsValid = False, .Message = "Formato de email inválido"}
            End If

        Catch
            Return New ValidationResult With {.IsValid = False, .Message = "Error al validar el email"}

        End Try
    End Function

    ''' <summary>
    ''' Valida formato de RFC mexicano
    ''' </summary>
    Public Shared Function ValidateRFC(rfc As String) As ValidationResult
        If String.IsNullOrEmpty(rfc) Then
            Return New ValidationResult With {.IsValid = False, .Message = "El RFC no puede estar vacío"}
        End If

        ' RFC puede ser de 12 o 13 caracteres
        ' Formato: XXXX######XXX (persona física) o XXX######XXX (persona moral)
        Dim rfcRegex As New Regex("^[A-ZÑ&]{3,4}\d{6}[A-Z0-9]{2,3}$")

        If rfcRegex.IsMatch(rfc.ToUpper()) Then
            Return New ValidationResult With {.IsValid = True, .Message = ""}
        Else
            Return New ValidationResult With {.IsValid = False, .Message = "Formato de RFC inválido"}
        End If
    End Function

    ''' <summary>
    ''' Valida que un número esté en un rango específico
    ''' </summary>
    Public Shared Function ValidateNumericRange(value As String, minValue As Double, maxValue As Double) As ValidationResult
        If String.IsNullOrEmpty(value) Then
            Return New ValidationResult With {.IsValid = False, .Message = "El valor no puede estar vacío"}
        End If

        Dim numValue As Double

        If Not Double.TryParse(value, numValue) Then
            Return New ValidationResult With {.IsValid = False, .Message = "El valor debe ser numérico"}
        End If

        If numValue < minValue Or numValue > maxValue Then
            Return New ValidationResult With {
                .IsValid = False,
                .Message = $"El valor debe estar entre {minValue} y {maxValue}"
            }
        End If

        Return New ValidationResult With {.IsValid = True, .Message = ""}
    End Function

    ''' <summary>
    ''' Valida longitud de cadena
    ''' </summary>
    Public Shared Function ValidateLength(input As String, minLength As Integer, maxLength As Integer) As ValidationResult
        If input Is Nothing Then input = String.Empty

        If input.Length < minLength Then
            Return New ValidationResult With {
                .IsValid = False,
                .Message = $"El texto debe tener al menos {minLength} caracteres"
            }
        End If

        If input.Length > maxLength Then
            Return New ValidationResult With {
                .IsValid = False,
                .Message = $"El texto no puede tener más de {maxLength} caracteres"
            }
        End If

        Return New ValidationResult With {.IsValid = True, .Message = ""}
    End Function

End Class

''' <summary>
''' Clase para resultados de validación
''' </summary>
Public Class ValidationResult
    Public Property IsValid As Boolean
    Public Property Message As String
End Class
