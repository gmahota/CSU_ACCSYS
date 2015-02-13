Public Class InicializarPlaformaCrtl

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim imp As New ImportadorExtratoBancario
        imp.Inicializar(cbInstancia.SelectedIndex, txtCodEmp.Text, txtUser.Text, txtPassword.Text, _
            "Data Source=" & txtInstancia.Text & ";Initial Catalog=" & txtDasedeDados.Text & _
            ";User Id=" & txtUserSql.Text & ";Password=" & txtPasswordSql.Text)

    End Sub
End Class


