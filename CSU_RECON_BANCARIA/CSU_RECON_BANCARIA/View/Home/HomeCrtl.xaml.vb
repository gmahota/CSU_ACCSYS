Imports MahApps.Metro.Controls

Public Class HomeCrtl
    Public mainWindows As MainWindow

    Private Sub Hyperlink_Click(sender As Object, e As RoutedEventArgs)
        Dim item As New MetroTabItem
        item.Header = "Importador para CSV"
        item.CloseButtonEnabled = True
        item.IsSelected = True

        Dim importadorCrlFormatoMagnetico As New ImportFormatoMagneticoCtrl

        importadorCrlFormatoMagnetico.Inicializar(mainWindows.xmlHelper.instancia.instancia, mainWindows.xmlHelper.instancia.empresa,
                                                  mainWindows.xmlHelper.instancia.usuario,
                                     mainWindows.xmlHelper.instancia.password, mainWindows.xmlHelper.instancia.daConnectionString())
        item.Content = importadorCrlFormatoMagnetico

        mainWindows.tbMain.Items.Add(item)
    End Sub

    Private Sub Hyperlink_Click_1(sender As Object, e As RoutedEventArgs)

    End Sub
End Class
