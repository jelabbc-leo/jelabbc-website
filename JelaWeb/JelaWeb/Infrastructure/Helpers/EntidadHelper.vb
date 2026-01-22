Imports System.Web

''' <summary>
''' Helper para manejo automático de filtrado por entidad en sistema multi-entidad
''' Proporciona métodos para agregar filtros y validar pertenencia de registros
''' </summary>
Public NotInheritable Class EntidadHelper
    Private Sub New()
        ' Clase estática, no instanciable
    End Sub

    ''' <summary>
    ''' Obtiene el ID de la entidad actual o lanza excepción si no existe
    ''' </summary>
    ''' <returns>ID de la entidad actual</returns>
    ''' <exception cref="InvalidOperationException">Si no hay entidad seleccionada</exception>
    Public Shared Function GetIdEntidadActualOrThrow() As Integer
        Dim idEntidad = SessionHelper.GetIdEntidadActual()
        
        If Not idEntidad.HasValue Then
            Throw New InvalidOperationException("No hay entidad seleccionada en la sesión. El usuario debe seleccionar una entidad antes de realizar esta operación.")
        End If
        
        Return idEntidad.Value
    End Function

    ''' <summary>
    ''' Agrega filtro de entidad a una query SQL
    ''' Detecta automáticamente si la query ya tiene WHERE y agrega AND o WHERE según corresponda
    ''' </summary>
    ''' <param name="query">Query SQL original</param>
    ''' <returns>Query con filtro de entidad agregado</returns>
    Public Shared Function AgregarFiltroEntidad(query As String) As String
        If String.IsNullOrWhiteSpace(query) Then
            Throw New ArgumentException("La query no puede estar vacía", NameOf(query))
        End If
        
        Dim idEntidad = GetIdEntidadActualOrThrow()
        Dim queryUpper = query.ToUpper()
        
        ' Detectar si ya tiene WHERE
        If queryUpper.Contains(" WHERE ") Then
            ' Ya tiene WHERE, agregar con AND
            Return query & $" AND IdEntidad = {idEntidad}"
        Else
            ' No tiene WHERE, agregarlo
            Return query & $" WHERE IdEntidad = {idEntidad}"
        End If
    End Function

    ''' <summary>
    ''' Agrega el campo IdEntidad al diccionario de campos para INSERT/UPDATE
    ''' Solo lo agrega si no existe ya en el diccionario
    ''' </summary>
    ''' <param name="campos">Diccionario de campos a insertar/actualizar</param>
    Public Shared Sub AgregarCampoEntidad(ByRef campos As Dictionary(Of String, Object))
        If campos Is Nothing Then
            Throw New ArgumentNullException(NameOf(campos))
        End If
        
        Dim idEntidad = GetIdEntidadActualOrThrow()
        
        ' Solo agregar si no existe ya
        If Not campos.ContainsKey("IdEntidad") Then
            campos.Add("IdEntidad", idEntidad)
        End If
    End Sub

    ''' <summary>
    ''' Valida que un registro pertenezca a la entidad actual
    ''' Útil antes de UPDATE o DELETE para evitar modificar registros de otras entidades
    ''' </summary>
    ''' <param name="idRegistro">ID del registro a validar</param>
    ''' <param name="tabla">Nombre de la tabla donde está el registro</param>
    ''' <returns>True si el registro pertenece a la entidad actual, False si no</returns>
    Public Shared Function ValidarPerteneceAEntidadActual(idRegistro As Integer, tabla As String) As Boolean
        If String.IsNullOrWhiteSpace(tabla) Then
            Throw New ArgumentException("El nombre de la tabla no puede estar vacío", NameOf(tabla))
        End If
        
        Try
            Dim idEntidad = GetIdEntidadActualOrThrow()
            
            ' Construir query de validación
            Dim query As String = $"SELECT COUNT(*) AS Total FROM {tabla} WHERE Id = {idRegistro} AND IdEntidad = {idEntidad}"
            
            ' Ejecutar query usando DynamicCrudService
            Dim dt = DynamicCrudService.EjecutarConsulta(query)
            
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                Dim total = Convert.ToInt32(dt.Rows(0)("Total"))
                Return total > 0
            End If
            
            Return False
            
        Catch ex As Exception
            Logger.LogError($"EntidadHelper.ValidarPerteneceAEntidadActual - Tabla: {tabla}, Id: {idRegistro}", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Obtiene el nombre de la entidad actual para mostrar en UI
    ''' </summary>
    ''' <returns>Nombre de la entidad o string vacío si no hay entidad seleccionada</returns>
    Public Shared Function GetNombreEntidadActual() As String
        Return SessionHelper.GetEntidadActualNombre()
    End Function

    ''' <summary>
    ''' Verifica si el usuario actual puede gestionar múltiples entidades
    ''' </summary>
    ''' <returns>True si es AdministradorCondominios con múltiples entidades</returns>
    Public Shared Function PuedeGestionarMultiplesEntidades() As Boolean
        Return SessionHelper.IsAdministradorCondominios() AndAlso SessionHelper.TieneMultiplesEntidades()
    End Function

End Class
