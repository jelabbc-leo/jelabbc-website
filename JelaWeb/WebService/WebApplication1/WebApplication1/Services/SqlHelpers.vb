Imports MySql.Data.MySqlClient

Public Class SqlHelpers
    ' Helpers
    Public Shared conexionStr As String = ConfigurationManager.ConnectionStrings("ConexionMySQL").ConnectionString
    Public Shared Function EjecutarConsulta(query As String, parametros As Dictionary(Of String, Object)) As List(Of CRUDDTO)
        Dim lista As New List(Of CRUDDTO)

        Using conn As New MySqlConnection(conexionStr)
            conn.Open()

            Using cmd As New MySqlCommand(query, conn)
                For Each kvp In parametros
                    cmd.Parameters.AddWithValue(kvp.Key, kvp.Value)
                Next

                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim dto As New CRUDDTO()

                        For i As Integer = 0 To reader.FieldCount - 1
                            Dim nombre = reader.GetName(i)
                            Dim valor = reader.GetValue(i)
                            Dim tipo = reader.GetFieldType(i).FullName

                            dto.Campos(nombre) = New CampoConTipo With {
                            .Valor = valor,
                            .Tipo = tipo
                        }
                        Next

                        lista.Add(dto)
                    End While
                End Using
            End Using
        End Using

        Return lista

    End Function
    'Public Shared Function EjecutarConsulta(query As String, parametros As Dictionary(Of String, Object)) As List(Of CRUDDTO)
    '    Dim lista As New List(Of CRUDDTO)

    '    Using conn As New MySqlConnection(conexionStr)
    '        conn.Open()

    '        Using cmd As New MySqlCommand(query, conn)

    '            For Each kvp In parametros
    '                cmd.Parameters.AddWithValue(kvp.Key, kvp.Value)
    '            Next

    '            Using reader As MySqlDataReader = cmd.ExecuteReader()
    '                While reader.Read()
    '                    Dim dto As New CRUDDTO()

    '                    For i As Integer = 0 To reader.FieldCount - 1
    '                        dto(reader.GetName(i)) = reader.GetValue(i)
    '                    Next
    '                    lista.Add(dto)

    '                End While

    '            End Using
    '        End Using
    '    End Using

    '    Return lista

    'End Function

    Public Shared Sub EjecutarNoConsulta(query As String, parametros As Dictionary(Of String, Object))
        Using conn As New MySqlConnection(conexionStr)
            conn.Open()

            Using cmd As New MySqlCommand(query, conn)

                For Each kvp In parametros
                    cmd.Parameters.AddWithValue(kvp.Key, kvp.Value)
                Next

                cmd.ExecuteNonQuery()
            End Using

        End Using

    End Sub

    Public Shared Function EjecutarEscalar(query As String) As Object
        Dim resultado As Object = Nothing

        Using conexion As New MySqlConnection(conexionStr)
            Using comando As New MySqlCommand(query, conexion)
                Try
                    conexion.Open()
                    resultado = comando.ExecuteScalar()
                Catch ex As Exception
                    Throw New Exception("Error en EjecutarEscalar: " & ex.Message)
                End Try
            End Using
        End Using

        Return resultado
    End Function


End Class
