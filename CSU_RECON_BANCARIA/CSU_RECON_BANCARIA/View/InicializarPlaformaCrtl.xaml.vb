Imports MahApps.Metro.Controls
Imports System.Xml

Public Class InicializarPlaformaCrtl
    Inherits Flyout
    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        'Dim imp As New ImportadorExtratoBancario
        'imp.Inicializar(cbInstancia.SelectedIndex, txtCodEmp.Text, txtUser.Text, txtPassword.Text, _
        '    "Data Source=" & txtInstancia.Text & ";Initial Catalog=" & txtDasedeDados.Text & _
        '    ";User Id=" & txtUserSql.Text & ";Password=" & txtPasswordSql.Text)

        XMLData.Document.Save(DefaultData.Pasta_Config)

    End Sub

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        'XMLData.Document.Load("resources/Config.xml")

        ' Add any initialization after the InitializeComponent() call.

        Dim xmlDocument = New XmlDocument()

        xmlDocument.Load(DefaultData.Pasta_Config)
        XMLData.Document = xmlDocument

    End Sub
End Class


