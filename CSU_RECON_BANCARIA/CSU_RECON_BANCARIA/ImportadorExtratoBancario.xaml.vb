Imports Interop.ErpBS800

Public Class ImportadorExtratoBancario
    Dim xmlHelper As XmlHelper
    Public Sub Inicializar(tipoPlataforma As Integer, codEmpresa As String, codUsuario As String, password As String, connection As String)
        InitializeComponent()
        Me.importadorCrtl.Inicializar(tipoPlataforma, codEmpresa, codUsuario, password, connection)
        Me.Show()
    End Sub


    Public Sub New()
        xmlHelper = New XmlHelper
        ' This call is required by the designer.
        InitializeComponent()
        Me.importadorCrtl.Inicializar(xmlHelper.instancia.instancia, xmlHelper.instancia.empresa, xmlHelper.instancia.usuario,
                                     xmlHelper.instancia.password, xmlHelper.instancia.daConnectionString())
    End Sub
End Class
