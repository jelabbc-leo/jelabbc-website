Imports System

''' <summary>
''' DTO para el catálogo de Roles
''' </summary>
Public Class RolDTO
    Public Property Id As Integer
    Public Property IdEntidad As Integer
    Public Property Nombre As String
    Public Property Descripcion As String
    Public Property Activo As Boolean
    Public Property FechaCreacion As DateTime
    Public Property FechaModificacion As DateTime

    ''' <summary>
    ''' Lista de IDs de permisos asignados a este rol
    ''' </summary>
    Public Property PermisosIds As List(Of Integer)

    ''' <summary>
    ''' Nombres de permisos para visualización
    ''' </summary>
    Public Property PermisosNombres As List(Of String)
End Class

''' <summary>
''' DTO para permisos del sistema
''' </summary>
Public Class PermisoDTO
    Public Property Id As Integer
    Public Property IdEntidad As Integer
    Public Property Modulo As String
    Public Property Nombre As String
    Public Property Descripcion As String
    Public Property Clave As String
    Public Property Activo As Boolean
End Class
