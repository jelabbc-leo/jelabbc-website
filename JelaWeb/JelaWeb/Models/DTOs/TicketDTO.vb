''' <summary>
''' DTO para representar un ticket del sistema
''' </summary>
Public Class TicketDTO
    Public Property Id As Integer
    Public Property Folio As String
    Public Property Titulo As String
    Public Property Descripcion As String
    Public Property CategoriaId As Integer
    Public Property CategoriaNombre As String
    Public Property Prioridad As String
    Public Property Estado As String
    Public Property UsuarioCreadorId As Integer
    Public Property UsuarioCreadorNombre As String
    Public Property EntidadId As Integer
    Public Property EntidadNombre As String
    Public Property SubEntidadId As Integer?
    Public Property SubEntidadNombre As String
    Public Property FechaCreacion As DateTime
    Public Property FechaModificacion As DateTime
End Class
