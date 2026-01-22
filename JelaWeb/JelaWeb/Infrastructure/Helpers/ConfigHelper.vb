Public Class ConfigHelper
    Public Shared ReadOnly Property ApiBaseUrl As String
        Get
            Return ConfigurationManager.AppSettings("ApiBaseUrl")
        End Get
    End Property
End Class
