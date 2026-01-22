Imports System

''' <summary>
''' DTO para usuarios del sistema
''' </summary>
Public Class UsuarioDTO
    Public Property Id As Integer
    Public Property IdEntidad As Integer
    Public Property Username As String
    Public Property Nombre As String
    Public Property Email As String
    Public Property Activo As Boolean
    Public Property CreadoEn As DateTime

    ''' <summary>
    ''' Nombre de la entidad para visualización
    ''' </summary>
    Public Property EntidadNombre As String

    ''' <summary>
    ''' Lista de roles asignados al usuario
    ''' </summary>
    Public Property Roles As List(Of String)

    Public Sub New()
        Roles = New List(Of String)()
    End Sub
End Class

''' <summary>
''' DTO para login de usuario
''' </summary>
Public Class LoginDTO
    Public Property Username As String
    Public Property Password As String
End Class

''' <summary>
''' DTO para respuesta de login
''' </summary>
Public Class LoginResponseDTO
    Public Property Success As Boolean
    Public Property Message As String
    Public Property Usuario As UsuarioDTO
    Public Property Token As String
End Class

''' <summary>
''' DTO para información de sesión del usuario
''' </summary>
Public Class SesionUsuarioDTO
    Public Property UsuarioId As Integer
    Public Property IdEntidad As Integer
    Public Property Username As String
    Public Property Nombre As String
    Public Property EntidadNombre As String
    Public Property Roles As List(Of String)
    Public Property Permisos As List(Of String)

    Public Sub New()
        Roles = New List(Of String)()
        Permisos = New List(Of String)()
    End Sub
End Class
