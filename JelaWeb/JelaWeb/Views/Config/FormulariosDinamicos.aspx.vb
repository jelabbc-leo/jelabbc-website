Imports DevExpress.Web

Public Class FormulariosDinamicos
    Inherits System.Web.UI.Page

    Private ReadOnly _formularioService As New FormularioService()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            CargarFormularios()
        End If
    End Sub

    Private Sub CargarFormularios()

        Try
            Dim dt = _formularioService.GetFormularios()

            gridFormularios.DataSource = dt
            gridFormularios.DataBind()

        Catch ex As Exception
            Logger.LogError("FormulariosDinamicos.CargarFormularios", ex.Message)

        End Try
    End Sub

    Protected Sub gridFormularios_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs)

        Try
            Dim args = e.Parameters.Split("|"c)
            Dim action = args(0)

            Select Case action

                Case "refresh"
                    CargarFormularios()

                Case "delete"
                    If args.Length > 1 Then
                        Dim formularioId = Integer.Parse(args(1))

                        _formularioService.DeleteFormulario(formularioId)
                        CargarFormularios()
                    End If

            End Select

        Catch ex As Exception
            Logger.LogError("FormulariosDinamicos.gridFormularios_CustomCallback", ex.Message)

        End Try
    End Sub

    ''' <summary>
    ''' Evento DataBound para aplicar FuncionesGridWeb
    ''' </summary>
    Protected Sub gridFormularios_DataBound(sender As Object, e As EventArgs) Handles gridFormularios.DataBound

        Try
            Dim tabla As DataTable = TryCast(gridFormularios.DataSource, DataTable)

            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridFormularios, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("FormulariosDinamicos.gridFormularios_DataBound", ex.Message)

        End Try
    End Sub
End Class