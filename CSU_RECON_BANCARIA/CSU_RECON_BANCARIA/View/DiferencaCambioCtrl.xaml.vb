Public Class DiferencaCambioCtrl

    Dim clienteshelper As New PendentesHelper

    Public Sub Inicializar(tipoPlataforma As Integer, codEmpresa As String, codUsuario As String, password As String, con As String)
        On Error GoTo trataerro
        ' This call is required by the designer.
        InitializeComponent()
        clienteshelper.tipoPlataforma = tipoPlataforma
        clienteshelper.codEmpresa = codEmpresa
        clienteshelper.codUsuario = codUsuario
        clienteshelper.password = password
        clienteshelper.connectionString = con
        
        Exit Sub
trataerro:

        MsgBox(Err.Description)
    End Sub



    

    Public Sub save()
        Try
            'Mouse.OverrideCursor = Cursors.Wait
            progressRing.IsActive = True
            'progressRing.Visibility = Windows.Visibility.Visible
            ' clienteshelper.importarExtrato2(txtFicheiroExcell.Text, cbFolhaExcel.SelectedIndex + 1, Conversion.Int(txtLinhaInical.Text), Conversion.Int(txtLinhaFinal.Text), cbBanco.Text, cbContaBancaria.Text, cbFormatoBanco.Text, txtNumConta.Text, txtNumExtrato.Text, dtInicio.SelectedDate, dtFim.SelectedDate, txtSaldoInicial.Text, txtSaldoFinal.Text)
            clienteshelper.gravar(dgPendentes.ItemsSource)
            progressRing.IsActive = False

            'progressRing.Visibility = Windows.Visibility.Hidden
        Catch ex As Exception
            progressRing.IsActive = False
            MessageBox.Show(ex.Message)
            'progressRing.Visibility = Windows.Visibility.Hidden

        End Try
    End Sub

    Public Sub update()
        dgPendentes.ItemsSource = clienteshelper.daListaPendentes()
        cbMoeda.Items.Clear()

        For Each moeda In clienteshelper.daListaMoedas()

            cbMoeda.Items.Add(moeda)
            cbMoeda.DisplayMemberPath = "Moeda"
        Next
    End Sub

    

    Private Sub cbMoeda_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim moeda As Moedas


        moeda = cbMoeda.SelectedItem

        dgPendentes.ItemsSource = clienteshelper.daListaPendentesPorMoeda(moeda.Moeda)
    End Sub

    Private Sub btupdate_Click(sender As Object, e As RoutedEventArgs)
        Dim i As Integer
        
        For i = 0 To dgPendentes.Items.Count - 1
            dgPendentes.Items(i).ValorActualizacao = txtCambio.Value
        Next
        dgPendentes.Items.Refresh()
    End Sub

    Private Sub NumericUpDown_TextInput(sender As Object, e As TextCompositionEventArgs)
        
    End Sub


    Private Sub txtCambio_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of System.Nullable(Of Double)))
        Try
            Dim i As Integer

            For i = 0 To dgPendentes.Items.Count - 1
                dgPendentes.Items(i).ContraValor = txtCambio.Value * dgPendentes.Items(i).ValorPendente
                dgPendentes.Items(i).ValorActualizacao = dgPendentes.Items(i).ContraValor - dgPendentes.Items(i).ValorPendenteMT
            Next
            dgPendentes.Items.Refresh()
        Catch ex As Exception

        End Try
        
    End Sub


End Class
