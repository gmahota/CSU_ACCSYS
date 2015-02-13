Imports System.Threading.Tasks.TaskScheduler
Imports MahApps.Metro.Controls
Imports System.Data

Class MainWindow : Inherits MetroWindow
    Dim clienteHelper As New ClientesHelper

    Public Sub New()
        Dim ds As New DataSet
        ' This call is required by the designer.
        InitializeComponent()

        clienteHelper.instancia = "SERVERPRIMAVERA\PRIMAVERA" '"ACCPRI08\PRIMAVERAV810" ' 
        clienteHelper.passwordInstancia = "Accsys2011"
        'clienteHelper.actualizarconexao()

        ds = clienteHelper.listaEmpresas()
        ' Add any initialization after the InitializeComponent() call.
        ' clienteHelper.incializarMotorPrimavera(0, "RECNOVA", "accsys", "accsys2011")
        dgEmpresa.ItemsSource = ds.Tables("Empresas").DefaultView

    End Sub


    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        On Error GoTo trataerro

        If (dgEmpresa.Items.Count > 0) Then
            cbEmpresas.Items.Clear()
            Dim i As Integer
            For i = 0 To (dgEmpresa.Items.Count - 1)
                Dim selectedFile As System.Data.DataRowView
                selectedFile = dgEmpresa.Items(i)
                If (Convert.ToBoolean(selectedFile.Row.ItemArray(0))) Then
                    cbEmpresas.Items.Add(selectedFile.Row.ItemArray(2))

                End If
            Next i
        End If


        Exit Sub
trataerro:
        MsgBox("Erro: " & Err.Number & " - " & Err.Description)
    End Sub

    Private Sub cbEmpresas_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbEmpresas.SelectionChanged
        clienteHelper.actualizarconexao(cbEmpresas.SelectedItem)

        dgClientesPendentes.ItemsSource = clienteHelper.listaClientesComPendentes.Tables("Clientes").DefaultView
    End Sub

    Private Sub Button_Click_2(sender As Object, e As RoutedEventArgs)



        Dim dv As DataView
        Dim i As Integer
        dv = dgClientesPendentes.ItemsSource

        Try
            clienteHelper.actualizarconexao(cbEmpresas.SelectedItem)

            For i = 0 To dgClientesPendentes.Items.Count
                If dv.Item(i).Row("CDU_EnviaCobranca") = "True" Then
                    clienteHelper.enviaEmails(dv.Item(i).Row("Cliente"), txtAnexo1.Text, txtAnexo2.Text)
                End If
            Next i

        Catch
            MessageBox.Show("Erro durante a Operacao")
        End Try
        MessageBox.Show("Operacao terminda com Sucesso")


    End Sub

    Private Sub OnUnchecked(sender As Object, e As RoutedEventArgs)

        'Dim dv As DataView

        'dv = dgClientesPendentes.ItemsSource

        'Try

        '    totalFacturado = totalFacturado - dv.Item(dgClientesPendentes.SelectedIndex).Row("CDU_ValorContarto")
        '    txtTotal.Text = totalFacturado.ToString()

        'Catch
        'End Try
    End Sub

    Private Sub OnChecked(sender As Object, e As RoutedEventArgs)
        'Dim dataset As DataSet
        'Dim dv As DataView

        'dv = dgClientes.ItemsSource

        'Try

        '    totalFacturado = totalFacturado + dv.Item(dgClientes.SelectedIndex).Row("CDU_ValorContarto")
        '    txtTotal.Text = totalFacturado.ToString()

        'Catch
        'End Try
    End Sub

    Private Sub chkSelectAll_Unchecked(sender As Object, e As RoutedEventArgs)
        Dim dv As DataView
        Dim i As Integer
        dv = dgClientesPendentes.ItemsSource

        Try
            For i = 0 To dgClientesPendentes.Items.Count
                dv.Item(i).Row("CDU_EnviaCobranca") = "False"
                'totalFacturado = totalFacturado - dv.Item(i).Row("CDU_ValorContarto")
                'txtTotal.Text = totalFacturado.ToString()
            Next i

        Catch
        End Try
    End Sub

    Private Sub chkSelectAll_Checked(sender As Object, e As RoutedEventArgs)
        Dim dv As DataView
        Dim i As Integer
        dv = dgClientesPendentes.ItemsSource

        Try
            For i = 0 To dgClientesPendentes.Items.Count
                dv.Item(i).Row("CDU_EnviaCobranca") = "True"
                'totalFacturado = totalFacturado + dv.Item(i).Row("CDU_ValorContarto")
                'txtTotal.Text = totalFacturado.ToString()
            Next i

        Catch
        End Try
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim ficheiro As New Microsoft.Win32.OpenFileDialog()
        'ficheiro.Filter = "Excel files (*.xls)|*.xls|CVS Files (*.csv)|*.csv;"
        Dim result As Boolean
        result = ficheiro.ShowDialog()
        If (result = False) Then
            Return
        End If
        txtAnexo1.Text = ficheiro.FileName
    End Sub

    Private Sub Button_Click_3(sender As Object, e As RoutedEventArgs)
        Dim ficheiro As New Microsoft.Win32.OpenFileDialog()
        'ficheiro.Filter = "Excel files (*.xls)|*.xls|CVS Files (*.csv)|*.csv;"
        Dim result As Boolean
        result = ficheiro.ShowDialog()
        If (result = False) Then
            Return
        End If
        txtAnexo2.Text = ficheiro.FileName
    End Sub
End Class
