Imports System.Web.Http
Imports MySql.Data.MySqlClient
Imports System.Configuration
Imports WebApplication1.SqlHelpers

Imports System.Net

Public Class CrudController
    Inherits ApiController

    Private ReadOnly conexionStr As String = ConfigurationManager.ConnectionStrings("ConexionMySQL").ConnectionString

    ' GET: Leer registros dinámicamente
    <HttpGet>
    Public Function GetRegistros(strQuery As String) As IHttpActionResult
        Try
            If String.IsNullOrWhiteSpace(strQuery) OrElse Not strQuery.Trim().ToLower().StartsWith("select") Then
                Return BadRequest("Solo se permiten consultas SELECT.")
            End If

            Dim datos = EjecutarConsulta(strQuery, New Dictionary(Of String, Object))
            Return Json(datos)

        Catch ex As Exception
            Return Content(HttpStatusCode.InternalServerError,
                           New With {.mensaje = "Error al ejecutar la consulta: " & ex.Message})
        End Try
    End Function

    ' POST: Insertar registro dinámicamente
    <HttpPost>
    Public Function Post(table As String, dto As CRUDDTO) As IHttpActionResult
        Try
            Dim campos = String.Join(",", dto.Campos.Keys.Select(Function(k) $"`{k}`"))
            Dim valores = String.Join(",", dto.Campos.Keys.Select(Function(k) $"@{k}"))
            Dim query = $"INSERT INTO `{table}` ({campos}) VALUES ({valores})"

            Dim parametros = dto.Campos.ToDictionary(Function(kvp) $"@{kvp.Key}", Function(kvp) kvp.Value.Valor)

            EjecutarNoConsulta(query, parametros)

            Dim idInsertado = ObtenerUltimoIdInsertado()
            Return Ok(New With {.id = idInsertado, .mensaje = "Registro insertado correctamente."})

        Catch ex As Exception
            Return Content(HttpStatusCode.InternalServerError,
                           New With {.mensaje = "Error al insertar registro: " & ex.Message})
        End Try
    End Function

    ' PUT: Actualizar registro dinámicamente
    <Route("api/Crud/{table}/{id}")>
    <HttpPut>
    Public Function Actualizar(table As String, id As Integer, <FromBody> dto As CRUDDTO) As IHttpActionResult
        Try
            Dim asignaciones = String.Join(",", dto.Campos.Keys.Select(Function(k) $"`{k}` = @{k}"))
            Dim query = $"UPDATE `{table}` SET {asignaciones} WHERE id = @id"

            Dim parametros = dto.Campos.ToDictionary(
                Function(kvp) $"@{kvp.Key}",
                Function(kvp) kvp.Value.Valor
            )

            parametros.Add("@id", id)

            EjecutarNoConsulta(query, parametros)

            Return Ok("Registro actualizado correctamente.")
        Catch ex As Exception
            Return Content(HttpStatusCode.InternalServerError,
                           New With {.mensaje = "Error al actualizar registro: " & ex.Message})
        End Try
    End Function

    ' DELETE: Eliminar dinámicamente por ID
    <HttpDelete>
    Public Function Delete(table As String, idField As String, idValue As Object) As IHttpActionResult
        Try
            Dim query = $"DELETE FROM `{table}` WHERE `{idField}` = @id"
            Dim parametros = New Dictionary(Of String, Object) From {{"@id", idValue}}
            EjecutarNoConsulta(query, parametros)
            Return Ok("Registro eliminado correctamente.")
        Catch ex As Exception
            Return Content(HttpStatusCode.InternalServerError,
                           New With {.mensaje = "Error al eliminar registro: " & ex.Message})
        End Try
    End Function

    Private Function ObtenerUltimoIdInsertado() As Integer
        Dim query = "SELECT LAST_INSERT_ID()"
        Return Convert.ToInt32(EjecutarEscalar(query))
    End Function


End Class