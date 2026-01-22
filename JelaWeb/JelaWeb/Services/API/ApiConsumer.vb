'=================================================================================
'  Autor: Josué Luis Solís Gómez                                                 =
'  Fecha: 15/06/2024                                                             =
'  Descripción: Clase para consumir APIs REST creadas por Josué Luis Solís Gómez.=
'  Y convertir los datos a DataTable                                             =
'=================================================================================

Imports System.Data
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Threading.Tasks
Imports DevExpress.XtraPrinting.Native.WebClientUIControl
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class ApiConsumer
    Private ReadOnly cliente As HttpClient

    Public Sub New()
        ' Usar HttpClientHelper para reutilizar la instancia
        cliente = HttpClientHelper.Client
    End Sub

    ''' <summary>
    ''' Agrega el header de autorización JWT si hay un token válido
    ''' </summary>
    Private Function CrearRequestConAuth(url As String, method As HttpMethod) As HttpRequestMessage
        Dim request = New HttpRequestMessage(method, url)
        Dim token = JwtTokenService.Instance.GetToken()

        If Not String.IsNullOrEmpty(token) Then
            request.Headers.Authorization = New AuthenticationHeaderValue("Bearer", token)
        End If

        Return request
    End Function

    Public Function ObtenerDatos(url As String) As List(Of DynamicDto)

        Try
            If String.IsNullOrWhiteSpace(url) Then
                Throw New ArgumentException("La URL no puede estar vacía")
            End If

            Dim respuesta As HttpResponseMessage = Nothing

            Try
                ' Crear request con header de autorización JWT
                Dim request = CrearRequestConAuth(url, HttpMethod.Get)

                ' Usar Task.Run para ejecutar en thread pool separado y evitar deadlocks
                Dim taskRespuesta = Task.Run(Async Function()
                                                 Return Await cliente.SendAsync(request).ConfigureAwait(False)
                                             End Function)
                respuesta = taskRespuesta.Result

            Catch ex As Exception
                Logger.LogError("Error al realizar GetAsync en ObtenerDatos: " & ex.Message, ex, "")
                Throw New Exception("Error al realizar la petición HTTP: " & ex.Message, ex)
            End Try

            If respuesta Is Nothing Then
                Throw New Exception("La respuesta HTTP es Nothing")
            End If

            Dim contenido As String = Nothing
            Try
                ' Usar Task.Run para ejecutar en thread pool separado y evitar deadlocks
                Dim taskContenido = Task.Run(Async Function()

                                                 Return Await respuesta.Content.ReadAsStringAsync().ConfigureAwait(False)
                                             End Function)
                contenido = taskContenido.Result

            Catch ex As Exception
                Logger.LogError("Error al leer contenido de respuesta en ObtenerDatos: " & ex.Message, ex, "")
                Throw New Exception("Error al leer el contenido de la respuesta: " & ex.Message, ex)
            End Try

            If Not respuesta.IsSuccessStatusCode Then
                ' Intentar extraer mensaje del cuerpo JSON
                Dim mensajeError = ApiErrorHelper.ExtraerMensaje(contenido)

                If String.IsNullOrWhiteSpace(mensajeError) Then
                    ' Si no hay mensaje, usar StatusCode y contenido crudo (limitado a 500 caracteres)
                    Dim contenidoLimitado = If(contenido IsNot Nothing AndAlso contenido.Length > 500, contenido.Substring(0, 500) & "...", contenido)

                    Throw New Exception("Error al consumir la API: " & respuesta.StatusCode & " - " & contenidoLimitado)
                Else
                    ' Incluir mensaje + StatusCode
                    Throw New Exception("Error al consumir la API: " & mensajeError & " (HTTP " & respuesta.StatusCode & ")")
                End If
            End If

            ' Validar que el contenido no esté vacío
            If String.IsNullOrWhiteSpace(contenido) Then
                Return New List(Of DynamicDto)()
            End If

            ' Limpiar el contenido (remover espacios en blanco)
            Dim contenidoLimpio = contenido.Trim()

            ' Si el contenido es "null", "[]" o está vacío después de limpiar, retornar lista vacía
            If contenidoLimpio = "null" OrElse contenidoLimpio = "[]" OrElse contenidoLimpio = String.Empty Then
                Return New List(Of DynamicDto)()
            End If

            ' Configurar JSON settings para evitar problemas de deserialización
            Dim jsonSettings As New JsonSerializerSettings()

            jsonSettings.NullValueHandling = NullValueHandling.Ignore
            jsonSettings.MissingMemberHandling = MissingMemberHandling.Ignore
            jsonSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            jsonSettings.MaxDepth = 32 ' Limitar profundidad máxima

            Try
                Dim resultado = JsonConvert.DeserializeObject(Of List(Of DynamicDto))(contenidoLimpio, jsonSettings)

                ' Si el resultado es Nothing, retornar lista vacía en lugar de Nothing
                Return If(resultado Is Nothing, New List(Of DynamicDto)(), resultado)

            Catch jsonEx As JsonException

                Logger.LogError("Error al deserializar JSON en ObtenerDatos. Contenido (primeros 200 chars): " & If(contenidoLimpio.Length > 200, contenidoLimpio.Substring(0, 200), contenidoLimpio), jsonEx, "")
                ' Si hay error de deserialización pero el contenido parece ser un array vacío o null, retornar lista vacía
                If contenidoLimpio = "[]" OrElse contenidoLimpio = "null" Then
                    Return New List(Of DynamicDto)()
                End If
                Throw New Exception("Error al procesar la respuesta JSON de la API: " & jsonEx.Message, jsonEx)

            End Try

        Catch ex As AggregateException

            ' Desenvolver AggregateException para obtener la excepción real
            Dim innerEx = ex.InnerException

            If innerEx IsNot Nothing Then
                Logger.LogError("Error en ObtenerDatos (AggregateException): " & innerEx.Message, innerEx, "")
                Throw New Exception("Error al consumir la API: " & innerEx.Message, innerEx)
            Else
                Logger.LogError("Error en ObtenerDatos (AggregateException): " & ex.Message, ex, "")
                Throw New Exception("Error al consumir la API: " & ex.Message, ex)
            End If

        Catch ex As Exception
            Logger.LogError("Error en ObtenerDatos: " & ex.Message, ex, "")
            Throw

        End Try

    End Function

    ''' <summary>
    ''' Versión asíncrona para obtener datos de la API
    ''' </summary>
    Public Async Function ObtenerDatosAsync(url As String) As Task(Of List(Of DynamicDto))
        Dim request = CrearRequestConAuth(url, HttpMethod.Get)
        Dim respuesta = Await cliente.SendAsync(request)
        Dim contenido = Await respuesta.Content.ReadAsStringAsync()

        If Not respuesta.IsSuccessStatusCode Then
            Dim mensajeError = ApiErrorHelper.ExtraerMensaje(contenido)

            If String.IsNullOrWhiteSpace(mensajeError) Then
                Throw New Exception("Error al consumir la API: " & respuesta.StatusCode & " - " & contenido)
            Else
                Throw New Exception("Error al consumir la API: " & mensajeError & " (HTTP " & respuesta.StatusCode & ")")
            End If
        End If

        Return JsonConvert.DeserializeObject(Of List(Of DynamicDto))(contenido)
    End Function

    Public Function ConvertirADatatable(listaDtos As List(Of DynamicDto)) As DataTable
        Dim tabla As New DataTable()

        ' Paso 1: Detectar todas las claves y su tipo más representativo
        Dim tiposPorCampo As New Dictionary(Of String, Type)

        For Each dto In listaDtos
            For Each kvp In dto.Campos
                Dim clave = kvp.Key
                Dim campo = kvp.Value
                Dim tipoDetectado = ObtenerTipoDesdeTexto(campo.Tipo)

                If Not tiposPorCampo.ContainsKey(clave) Then
                    tiposPorCampo(clave) = tipoDetectado
                Else
                    tiposPorCampo(clave) = ObtenerTipoCompatible(tiposPorCampo(clave), tipoDetectado)
                End If

            Next

        Next

        ' Paso 2: Crear columnas con tipo correcto

        For Each kvp In tiposPorCampo
            tabla.Columns.Add(kvp.Key, kvp.Value)

        Next

        ' Paso 3: Agregar filas

        For Each dto In listaDtos
            Dim fila As DataRow = tabla.NewRow()

            For Each kvp In tiposPorCampo
                Dim clave = kvp.Key

                If dto.Campos.ContainsKey(clave) Then
                    Dim valor = dto.Campos(clave).Valor

                    fila(clave) = If(valor IsNot Nothing, Convert.ChangeType(valor, kvp.Value), DBNull.Value)
                Else
                    fila(clave) = DBNull.Value
                End If

            Next

            tabla.Rows.Add(fila)

        Next

        Return tabla

    End Function

    Private Function ObtenerTipoDesdeTexto(tipoTexto As String) As Type

        Try
            Dim tipo = Type.GetType(tipoTexto, throwOnError:=False)

            Return If(tipo IsNot Nothing, tipo, GetType(Object))

        Catch
            Return GetType(Object)

        End Try
    End Function

    Private Function InferirTipo(valor As Object) As Type
        If valor Is Nothing OrElse valor Is DBNull.Value Then Return GetType(String)

        Dim tipo = valor.GetType()

        If tipo = GetType(String) Then
            Dim texto = valor.ToString()

            If Integer.TryParse(texto, Nothing) Then Return GetType(Integer)
            If Double.TryParse(texto, Nothing) Then Return GetType(Double)
            If Decimal.TryParse(texto, Nothing) Then Return GetType(Decimal)
            If Date.TryParse(texto, Nothing) Then Return GetType(Date)
            If Single.TryParse(texto, Nothing) Then Return GetType(Single)
            If Int16.TryParse(texto, Nothing) Then Return GetType(Int16)
            If Int32.TryParse(texto, Nothing) Then Return GetType(Int32)
            If Int64.TryParse(texto, Nothing) Then Return GetType(Int64)
            If Boolean.TryParse(texto, Nothing) Then Return GetType(Boolean)
            Return GetType(String)
        End If

        Return tipo
    End Function

    Private Function ObtenerTipoCompatible(tipoA As Type, tipoB As Type) As Type
        If tipoA = tipoB Then Return tipoA

        ' Si hay conflicto, usar String como tipo más general
        Return GetType(String)
    End Function

End Class
