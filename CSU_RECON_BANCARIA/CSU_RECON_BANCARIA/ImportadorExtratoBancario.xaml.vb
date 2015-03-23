Imports MahApps.Metro.Controls

Public Class ImportadorExtratoBancario : Inherits MetroWindow
    Dim xmlHelper As XmlHelper
    Public Sub Inicializar(tipoPlataforma As Integer, codEmpresa As String, codUsuario As String, password As String, connection As String)
        InitializeComponent()
        Me.importadorCrtl.Inicializar(tipoPlataforma, codEmpresa, codUsuario, password, connection)
        Me.Show()
    End Sub


    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        
    End Sub

    Public Sub InicializarPorXml()
        xmlHelper = New XmlHelper
        Me.importadorCrtl.Inicializar(xmlHelper.instancia.instancia, xmlHelper.instancia.empresa, xmlHelper.instancia.usuario,
                                     xmlHelper.instancia.password, xmlHelper.instancia.daConnectionString())
    End Sub

    Private Sub importadorCrtl_Loaded(sender As Object, e As RoutedEventArgs) Handles importadorCrtl.Loaded

    End Sub
End Class
