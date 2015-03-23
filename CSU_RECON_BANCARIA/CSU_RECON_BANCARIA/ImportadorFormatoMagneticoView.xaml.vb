Imports MahApps.Metro.Controls

Public Class ImportadorFormatoMagneticoView : Inherits MetroWindow
    Dim xmlHelper As XmlHelper

    

    Public Sub InicializarPorXml()
        xmlHelper = New XmlHelper
        Me.importadorCrtl.Inicializar(xmlHelper.instancia.instancia, xmlHelper.instancia.empresa, xmlHelper.instancia.usuario,
                                     xmlHelper.instancia.password, xmlHelper.instancia.daConnectionString())
    End Sub

    Public Sub Inicializar(tipoPlataforma As Integer, codEmpresa As String, codUsuario As String, password As String, connection As String)
        InitializeComponent()
        Me.importadorCrtl.Inicializar(tipoPlataforma, codEmpresa, codUsuario, password, connection)
        Me.Show()
    End Sub

End Class
