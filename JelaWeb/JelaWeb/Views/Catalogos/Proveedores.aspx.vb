Imports System.Data
Imports DevExpress.Web
Imports System.Net.Http
Imports Newtonsoft.Json

Public Class Proveedores
    Inherits BasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Try
            If Not IsPostBack Then
                ' Suscribir evento DataBound
                AddHandler gridProveedores.DataBound, AddressOf gridProveedores_DataBound

                CargarProveedores()
            End If

        Catch ex As Exception
            Logger.LogError("Error en Page_Load de Proveedores", ex, "")

        End Try
    End Sub

    Private Sub CargarProveedores()

        Try
            ' Usar API CRUD dinámico para obtener proveedores
            Dim query As String = "SELECT * FROM cat_proveedores WHERE IdEntidad = " & SessionHelper.GetIdEntidadActual().GetValueOrDefault(1) & " ORDER BY RazonSocial"
            Dim dt As DataTable = EjecutarConsultaAPI(query)

            ' Generar columnas dinámicamente desde el DataTable
            GenerarColumnasDinamicas(gridProveedores, dt)

            ' Guardar DataTable en Session para FuncionesGridWeb
            Session("dtProveedores") = dt

            gridProveedores.DataSource = dt
            gridProveedores.DataBind()

        Catch ex As Exception
            Logger.LogError("Error al cargar proveedores", ex, "")

        End Try
    End Sub

    Protected Sub gridProveedores_DataBound(sender As Object, e As EventArgs) Handles gridProveedores.DataBound

        Try
            Dim tabla As DataTable = TryCast(Session("dtProveedores"), DataTable)
            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridProveedores, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("Proveedores.gridProveedores_DataBound", ex, "")

        End Try
    End Sub

    Protected Sub gridProveedores_RowInserting(sender As Object, e As DevExpress.Web.Data.ASPxDataInsertingEventArgs)

        Try
            ' Crear objeto con los valores del formulario
            Dim proveedorData As New Dictionary(Of String, Object)()
            proveedorData("RazonSocial") = e.NewValues("RazonSocial")?.ToString()
            proveedorData("NombreComercial") = e.NewValues("NombreComercial")?.ToString()
            proveedorData("RFC") = e.NewValues("RFC")?.ToString()?.ToUpper()
            proveedorData("Email") = e.NewValues("Email")?.ToString()
            proveedorData("Telefono") = e.NewValues("Telefono")?.ToString()
            proveedorData("Activo") = If(e.NewValues("Activo") IsNot Nothing, CBool(e.NewValues("Activo")), True)
            proveedorData("IdEntidad") = SessionHelper.GetIdEntidadActual().GetValueOrDefault(1)

            ' Usar API CRUD dinámico para insertar
            Dim result As Boolean = InsertarRegistroAPI("cat_proveedores", proveedorData)

            If result Then
                Logger.LogInfo($"Proveedor creado: {proveedorData("RazonSocial")}", SessionHelper.GetNombre())
                e.Cancel = True
                gridProveedores.CancelEdit()
                CargarProveedores()
            Else
                Throw New Exception("Error al crear el proveedor")
            End If

        Catch ex As Exception
            Logger.LogError("Error al insertar proveedor", ex, SessionHelper.GetNombre())
            e.Cancel = True

        End Try
    End Sub

    Protected Sub gridProveedores_RowUpdating(sender As Object, e As DevExpress.Web.Data.ASPxDataUpdatingEventArgs)

        Try
            Dim id As Integer = Convert.ToInt32(e.Keys("Id"))

            ' Crear objeto con los valores actualizados
            Dim proveedorData As New Dictionary(Of String, Object)()
            proveedorData("RazonSocial") = e.NewValues("RazonSocial")?.ToString()
            proveedorData("NombreComercial") = e.NewValues("NombreComercial")?.ToString()
            proveedorData("RFC") = e.NewValues("RFC")?.ToString()?.ToUpper()
            proveedorData("Email") = e.NewValues("Email")?.ToString()
            proveedorData("Telefono") = e.NewValues("Telefono")?.ToString()
            proveedorData("Activo") = If(e.NewValues("Activo") IsNot Nothing, CBool(e.NewValues("Activo")), True)

            ' Usar API CRUD dinámico para actualizar
            Dim result As Boolean = ActualizarRegistroAPI("cat_proveedores", id, proveedorData)

            If result Then
                Logger.LogInfo($"Proveedor actualizado: {proveedorData("RazonSocial")}", SessionHelper.GetNombre())
                e.Cancel = True
                gridProveedores.CancelEdit()
                CargarProveedores()
            Else
                Throw New Exception("Error al actualizar el proveedor")
            End If

        Catch ex As Exception
            Logger.LogError("Error al actualizar proveedor", ex, SessionHelper.GetNombre())
            e.Cancel = True

        End Try
    End Sub

    Protected Sub gridProveedores_RowDeleting(sender As Object, e As DevExpress.Web.Data.ASPxDataDeletingEventArgs)

        Try
            Dim id As Integer = Convert.ToInt32(e.Keys("Id"))

            ' Usar API CRUD dinámico para eliminar
            Dim result As Boolean = EliminarRegistroAPI("cat_proveedores", "Id", id.ToString())

            If result Then
                Logger.LogInfo($"Proveedor eliminado: {id}", SessionHelper.GetNombre())
                e.Cancel = True
                CargarProveedores()
            Else
                Throw New Exception("Error al eliminar el proveedor")
            End If

        Catch ex As Exception
            Logger.LogError("Error al eliminar proveedor", ex, SessionHelper.GetNombre())
            e.Cancel = True

        End Try
    End Sub

    ''' <summary>
    ''' Genera columnas dinámicamente para un grid basándose en las columnas del DataTable.
    ''' Preserva columnas personalizadas (GridViewCommandColumn, columnas con DataItemTemplate).
    ''' </summary>
    Private Sub GenerarColumnasDinamicas(grid As ASPxGridView, tabla As DataTable)
        Try
            If tabla Is Nothing OrElse tabla.Columns.Count = 0 Then Return

            ' Limpiar columnas previas (excepto columnas personalizadas)
            Dim columnasParaMantener As New List(Of GridViewColumn)

            ' Guardar columnas de acciones/botones personalizados y columnas con templates
            For Each col As GridViewColumn In grid.Columns
                Dim debeMantener As Boolean = False

                If TypeOf col Is GridViewCommandColumn Then
                    debeMantener = True
                ElseIf TypeOf col Is GridViewDataColumn Then
                    Dim dataCol = CType(col, GridViewDataColumn)
                    If dataCol.DataItemTemplate IsNot Nothing Then
                        debeMantener = True
                    End If
                End If

                If debeMantener Then
                    columnasParaMantener.Add(col)
                End If
            Next

            ' Limpiar solo las columnas de datos, no las personalizadas
            For i As Integer = grid.Columns.Count - 1 To 0 Step -1
                Dim col = grid.Columns(i)
                If Not TypeOf col Is GridViewCommandColumn AndAlso
                   Not (TypeOf col Is GridViewDataColumn AndAlso CType(col, GridViewDataColumn).DataItemTemplate IsNot Nothing) Then
                    grid.Columns.RemoveAt(i)
                End If
            Next

            ' Crear columnas dinámicamente desde el DataTable
            For Each col As DataColumn In tabla.Columns
                Dim nombreColumna = col.ColumnName

                ' Omitir columnas que ya existen con template personalizado
                Dim yaExistePersonalizada As Boolean = False
                For Each colMantener As GridViewColumn In columnasParaMantener
                    If TypeOf colMantener Is GridViewDataColumn Then
                        Dim dataCol = CType(colMantener, GridViewDataColumn)
                        If dataCol.FieldName = nombreColumna AndAlso dataCol.DataItemTemplate IsNot Nothing Then
                            yaExistePersonalizada = True
                            Exit For
                        End If
                    End If
                Next

                If yaExistePersonalizada Then Continue For

                ' Crear columna según el tipo de dato
                Dim gridCol As GridViewDataColumn = Nothing

                Select Case col.DataType
                    Case GetType(Boolean)
                        gridCol = New GridViewDataCheckColumn()
                        gridCol.Width = Unit.Pixel(80)
                    Case GetType(DateTime), GetType(Date)
                        gridCol = New GridViewDataDateColumn()
                        gridCol.Width = Unit.Pixel(150)
                        CType(gridCol, GridViewDataDateColumn).PropertiesDateEdit.DisplayFormatString = "dd/MM/yyyy"
                    Case GetType(Decimal), GetType(Double), GetType(Single)
                        gridCol = New GridViewDataTextColumn()
                        gridCol.Width = Unit.Pixel(100)
                        gridCol.PropertiesEdit.DisplayFormatString = "c2"
                    Case GetType(Integer), GetType(Long), GetType(Short)
                        gridCol = New GridViewDataTextColumn()
                        gridCol.Width = Unit.Pixel(100)
                        gridCol.PropertiesEdit.DisplayFormatString = "n0"
                    Case Else
                        gridCol = New GridViewDataTextColumn()
                        gridCol.Width = Unit.Pixel(150)
                End Select

                gridCol.FieldName = nombreColumna
                gridCol.Caption = nombreColumna ' FuncionesGridWeb.SUMColumn aplicará SplitCamelCase
                gridCol.ReadOnly = True
                gridCol.Visible = True

                ' Configurar filtros y agrupación según estándares
                gridCol.Settings.AllowHeaderFilter = True
                gridCol.Settings.AllowGroup = True

                ' Ocultar columna Id si existe
                If nombreColumna.Equals("Id", StringComparison.OrdinalIgnoreCase) Then
                    gridCol.Visible = False
                End If

                ' Agregar validaciones para campos requeridos
                If nombreColumna = "RazonSocial" Or nombreColumna = "RFC" Then
                    If TypeOf gridCol Is GridViewDataTextColumn Then
                        CType(gridCol, GridViewDataTextColumn).PropertiesTextEdit.ValidationSettings.RequiredField.IsRequired = True
                        CType(gridCol, GridViewDataTextColumn).PropertiesTextEdit.ValidationSettings.RequiredField.ErrorText =
                            $"El campo {nombreColumna} es requerido"
                    End If
                End If

                grid.Columns.Add(gridCol)
            Next

        Catch ex As Exception
            Logger.LogError("GenerarColumnasDinamicas", ex)
            Throw
        End Try
    End Sub

    ''' <summary>
    ''' Ejecuta una consulta SELECT usando el API CRUD dinámico
    ''' </summary>
    Private Function EjecutarConsultaAPI(query As String) As DataTable
        Try
            Dim apiUrl As String = ConfigurationManager.AppSettings("ApiBaseUrl")
            Dim fullUrl As String = apiUrl & "?strQuery=" & Uri.EscapeDataString(query)

            Using client As New HttpClient()
                ' Agregar token JWT si existe
                Dim token As String = Session("JWTToken")?.ToString()
                If Not String.IsNullOrEmpty(token) Then
                    client.DefaultRequestHeaders.Authorization = New Headers.AuthenticationHeaderValue("Bearer", token)
                End If

                Dim response As HttpResponseMessage = client.GetAsync(fullUrl).Result
                response.EnsureSuccessStatusCode()

                Dim jsonResponse As String = response.Content.ReadAsStringAsync().Result
                Return JsonConvert.DeserializeObject(Of DataTable)(jsonResponse)
            End Using

        Catch ex As Exception
            Logger.LogError("Error ejecutando consulta API", ex)
            Return New DataTable()
        End Try
    End Function

    ''' <summary>
    ''' Inserta un registro usando el API CRUD dinámico
    ''' </summary>
    Private Function InsertarRegistroAPI(tabla As String, data As Dictionary(Of String, Object)) As Boolean
        Try
            Dim apiUrl As String = ConfigurationManager.AppSettings("APIPost")
            Dim fullUrl As String = $"{apiUrl}/{tabla}"

            Using client As New HttpClient()
                ' Agregar token JWT si existe
                Dim token As String = Session("JWTToken")?.ToString()
                If Not String.IsNullOrEmpty(token) Then
                    client.DefaultRequestHeaders.Authorization = New Headers.AuthenticationHeaderValue("Bearer", token)
                End If

                Dim jsonData As String = JsonConvert.SerializeObject(data)
                Dim content As New StringContent(jsonData, Encoding.UTF8, "application/json")

                Dim response As HttpResponseMessage = client.PostAsync(fullUrl, content).Result
                Return response.IsSuccessStatusCode
            End Using

        Catch ex As Exception
            Logger.LogError("Error insertando registro API", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Actualiza un registro usando el API CRUD dinámico
    ''' </summary>
    Private Function ActualizarRegistroAPI(tabla As String, id As Integer, data As Dictionary(Of String, Object)) As Boolean
        Try
            Dim apiUrl As String = ConfigurationManager.AppSettings("APIPost")
            Dim fullUrl As String = $"{apiUrl}/{tabla}/{id}"

            Using client As New HttpClient()
                ' Agregar token JWT si existe
                Dim token As String = Session("JWTToken")?.ToString()
                If Not String.IsNullOrEmpty(token) Then
                    client.DefaultRequestHeaders.Authorization = New Headers.AuthenticationHeaderValue("Bearer", token)
                End If

                Dim jsonData As String = JsonConvert.SerializeObject(data)
                Dim content As New StringContent(jsonData, Encoding.UTF8, "application/json")

                Dim response As HttpResponseMessage = client.PutAsync(fullUrl, content).Result
                Return response.IsSuccessStatusCode
            End Using

        Catch ex As Exception
            Logger.LogError("Error actualizando registro API", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Elimina un registro usando el API CRUD dinámico
    ''' </summary>
    Private Function EliminarRegistroAPI(tabla As String, idField As String, idValue As String) As Boolean
        Try
            Dim apiUrl As String = ConfigurationManager.AppSettings("APIPost")
            Dim fullUrl As String = $"{apiUrl}/{tabla}?idField={idField}&idValue={idValue}"

            Using client As New HttpClient()
                ' Agregar token JWT si existe
                Dim token As String = Session("JWTToken")?.ToString()
                If Not String.IsNullOrEmpty(token) Then
                    client.DefaultRequestHeaders.Authorization = New Headers.AuthenticationHeaderValue("Bearer", token)
                End If

                Dim response As HttpResponseMessage = client.DeleteAsync(fullUrl).Result
                Return response.IsSuccessStatusCode
            End Using

        Catch ex As Exception
            Logger.LogError("Error eliminando registro API", ex)
            Return False
        End Try
    End Function

End Class
