Imports Interop.ErpBS800
Imports MahApps.Metro.Controls
Imports MahApps.Metro.Controls.Dialogs
Imports System.Threading

Public Class ImportExtBancoCrtl

    Dim clienteshelper As New ExtratoHelper
    Dim xlApp As Object
    Dim xlBook As Object
    Dim xlSheet As Object
    Public motor As Object
    Public connection As String


    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        dtInicio.SelectedDate = Today
        dtFim.SelectedDate = Today

    End Sub

    Public Sub Inicializar(tipoPlataforma As Integer, codEmpresa As String, codUsuario As String, password As String, con As String)
        On Error GoTo trataerro
        ' This call is required by the designer.
        'InitializeComponent()

        dtInicio.SelectedDate = Today
        dtFim.SelectedDate = Today
        ' Add any initialization after the InitializeComponent() call.
        clienteshelper.connectionString = con
        'clienteshelper.incializarMotorPrimavera(tipoPlataforma, codEmpresa, codUsuario, password, con)

        cbBanco.Items.Clear()

        For Each banco As Bancos In clienteshelper.daListaBancos()

            cbBanco.Items.Add(banco)
            cbBanco.DisplayMemberPath = "Banco"
        Next

        cbFormatoBanco.Items.Clear()

        Exit Sub
trataerro:

        MsgBox(Err.Description)
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim ficheiro As New Microsoft.Win32.OpenFileDialog()
        ficheiro.Filter = "Excel files (*.xls;*.xlsx)|*.xls;*.xlsx|CVS Files (*.csv)|*.csv Files ();"
        Dim result As Boolean
        result = ficheiro.ShowDialog()
        If (result = False) Then
            Return
        End If
        txtFicheiroExcell.Text = ficheiro.FileName
        preencherExcell(ficheiro.FileName)

    End Sub

    Private Sub preencherExcell(caminhoficheiro As String)
        Dim i As Long
        Dim Tipo As String

        On Error GoTo Sair

        xlApp = CreateObject("Excel.Application")
        xlBook = xlApp.Workbooks.Open(caminhoficheiro)
        xlSheet = xlBook.Worksheets(1)

        Me.cbFolhaExcel.Items.Clear()

        If Len(caminhoficheiro) > 0 Then
            For i = 1 To xlBook.Worksheets.Count
                Me.cbFolhaExcel.Items.Add(xlBook.Worksheets(i).Name)
            Next i
            'Me.CmbFolha.ListIndex = 0
        End If

        xlBook.Close()
        'Quit excel (automatically closes all workbooks)
        xlApp.Quit()

        xlApp = Nothing
        xlBook = Nothing
        xlSheet = Nothing

        Exit Sub

Sair:
        MsgBox(Err.Description, vbInformation, "erro: " & Err.Number)

    End Sub

    Private Sub cbBanco_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbBanco.SelectionChanged
        Dim banco As Bancos


        banco = cbBanco.SelectedItem


        txtBanco.Text = banco.Descricao

        cbContaBancaria.Items.Clear()

        For Each contabancaria As ContasBancarias In clienteshelper.daListaContasBancarias(banco.Banco)

            cbContaBancaria.Items.Add(contabancaria)
            cbContaBancaria.DisplayMemberPath = "Conta"

        Next

        inicializarValoresComponentes()



    End Sub

    Private Sub cbContaBancaria_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbContaBancaria.SelectionChanged
        On Error GoTo trataerro

        Dim conta As ContasBancarias


        conta = cbContaBancaria.SelectedItem

        txtContaBancaria.Text = conta.Descricao
        txtNumConta.Text = conta.NumConta

        Exit Sub
trataerro:

    End Sub

    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        Try
            'Mouse.OverrideCursor = Cursors.Wait
            progressRing.IsActive = True
            'progressRing.Visibility = Windows.Visibility.Visible
            clienteshelper.importarExtrato2(txtFicheiroExcell.Text, cbFolhaExcel.SelectedIndex + 1, Conversion.Int(txtLinhaInical.Text), Conversion.Int(txtLinhaFinal.Text), cbBanco.Text, cbContaBancaria.Text, cbFormatoBanco.Text, txtNumConta.Text, txtNumExtrato.Text, dtInicio.SelectedDate, dtFim.SelectedDate, txtSaldoInicial.Text, txtSaldoFinal.Text)
            progressRing.IsActive = False

            'progressRing.Visibility = Windows.Visibility.Hidden
        Catch ex As Exception
            progressRing.IsActive = False

            'progressRing.Visibility = Windows.Visibility.Hidden

        End Try


        
        'clienteshelper.importarExtrato(txtFicheiroExcell.Text, cbFolhaExcel.SelectedIndex + 1, Conversion.Int(txtLinhaInical.Text), Conversion.Int(txtLinhaFinal.Text), cbBanco.Text, cbContaBancaria.Text, txtNumConta.Text, txtNumExtrato.Text, Today, Today, 0, 0)
    End Sub

    Private Sub cbFormatoBanco_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbFormatoBanco.SelectionChanged
        On Error GoTo trataerro

        txtFormatoBanco.Text = cbFormatoBanco.SelectedItem.Descricao

        validaExtratoBancario()
trataerro:

    End Sub

    Private Sub inicializarValoresComponentes()
        Dim banco As Bancos


        banco = cbBanco.SelectedItem

        For Each formatoBanco As FormatoBancario In clienteshelper.daFormatoBancario()

            cbFormatoBanco.Items.Add(formatoBanco)
            cbFormatoBanco.DisplayMemberPath = "Formato"
            If formatoBanco.Formato = banco.Formato Then '"STD"
                cbFormatoBanco.SelectedItem = formatoBanco
            End If

        Next
    End Sub

    Private Sub validaExtratoBancario()
        Dim linhasFormatoBancario As List(Of LinhasFormatoBancario)

        txtNumExtrato.Text = ""
        txtSaldoInicial.Text = "0,0"
        txtSaldoFinal.Text = "0,0"
        dtInicio.SelectedDate = Today
        dtFim.SelectedDate = Today
        txtNumConta.Text = ""



        linhasFormatoBancario = clienteshelper.daLinhasFormatoBancario(cbFormatoBanco.SelectedItem)

        For Each formatoBanco As LinhasFormatoBancario In clienteshelper.daLinhasFormatoBancario(cbFormatoBanco.SelectedItem)

            Select Case formatoBanco.Campo
                Case "NumeroExtracto"
                    If formatoBanco.Coluna > 0 Then
                        txtNumExtrato.Text = daValorExcell(formatoBanco.Linhas, formatoBanco.Coluna)
                    End If

                Case "SaldoInicial"
                    If formatoBanco.Coluna > 0 Then
                        txtSaldoInicial.Text = daValorExcell(formatoBanco.Linhas, formatoBanco.Coluna)
                    End If
                Case "SaldoFinal"
                    If formatoBanco.Coluna > 0 Then
                        txtSaldoFinal.Text = daValorExcell(formatoBanco.Linhas, formatoBanco.Coluna)
                    End If
                Case "DataInicial"
                    If formatoBanco.Coluna > 0 Then
                        dtInicio.SelectedDate = Convert.ToDateTime(daValorExcell(formatoBanco.Linhas, formatoBanco.Coluna))
                    End If
                Case "DataFinal"
                    If formatoBanco.Coluna > 0 Then
                        dtFim.SelectedDate = Convert.ToDateTime(daValorExcell(formatoBanco.Linhas, formatoBanco.Coluna))
                    End If
                Case "NumeroConta"
                    If formatoBanco.Coluna > 0 Then
                        txtNumConta.Text = daValorExcell(formatoBanco.Linhas, formatoBanco.Coluna)
                    End If

            End Select

        Next

    End Sub

    Private Function daValorExcell(linhas As Integer, coluna As Integer) As Object
        xlSheet = xlBook.Worksheets(cbFolhaExcel.SelectedIndex + 1)

        Return xlSheet.cells(linhas, coluna).Value
    End Function

    Private Sub Button_Click_2(sender As Object, e As RoutedEventArgs)
        'xlApp.Quit()
        Application.Current.Shutdown()
    End Sub

    Private Sub btValidado_Click(sender As Object, e As RoutedEventArgs)

        ImageValidado.Source = New BitmapImage(New Uri("/CSU_RECON_BANCARIA;component/Resources/Images/validar_certo.jpg", UriKind.Relative))
    End Sub

    Private Sub btupdate_Click(sender As Object, e As RoutedEventArgs)

    End Sub
End Class
