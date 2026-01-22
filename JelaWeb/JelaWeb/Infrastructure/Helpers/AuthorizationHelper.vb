Imports System.Web

''' <summary>
''' Helper para validación de permisos y autorización
''' </summary>
Public NotInheritable Class AuthorizationHelper
    Private Sub New()
        ' Clase estática, no instanciable
    End Sub

    ''' <summary>
    ''' Verifica si un usuario tiene un permiso específico
    ''' </summary>
    Public Shared Function TienePermiso(userId As Integer?, permiso As String) As Boolean

        Try
            ' Si no hay usuario, no tiene permisos
            If Not userId.HasValue Then
                Return False
            End If

            ' Por ahora, retornar True para permitir acceso
            ' TODO: Implementar lógica de validación de permisos cuando esté disponible
            ' Esto debería consultar la base de datos o la sesión para verificar permisos
            Return True

        Catch ex As Exception
            Logger.LogError($"Error al validar permiso {permiso} para usuario {userId}", ex, "")
            Return False

        End Try
    End Function

    ''' <summary>
    ''' Verifica si el usuario actual tiene un permiso específico
    ''' </summary>
    Public Shared Function TienePermiso(permiso As String) As Boolean
        Dim userId = SessionHelper.GetUserId()

        Return TienePermiso(userId, permiso)
    End Function

End Class
