Imports Interop.ErpBS800

Public Class ImportadorExtratoBancario
    Public Sub Inicializar(tipoPlataforma As Integer, codEmpresa As String, codUsuario As String, password As String, connection As String)
        InitializeComponent()
        Me.importadorCrtl.Inicializar(tipoPlataforma, codEmpresa, codUsuario, password, connection)
        Me.Show()
    End Sub

End Class
