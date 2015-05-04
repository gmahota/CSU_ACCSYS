Imports System.Data
Imports System.IO
Imports System.Globalization

Public Class ImportFormatoMagneticoCtrl
    Dim clienteshelper As New ExtratoHelper
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

    Private Sub cbBanco_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbBanco.SelectionChanged
        Dim banco As Bancos

        banco = cbBanco.SelectedItem
        dgContasBancarias.ItemsSource = clienteshelper.daListaContasBancarias(banco.Banco)

        inicializarValoresComponentes()
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

    Private Sub btGravar_Click(sender As Object, e As RoutedEventArgs) Handles btGravar.Click

        save()



    End Sub

    Public Sub CreateCSVFile(strFilePath As String)

        Try
            Dim linhasHistoricoSelecionado As HistoricoExpPS2


            linhasHistoricoSelecionado = dgHistorico.SelectedItem



            Dim sw As New StreamWriter(strFilePath, False)
            Dim listaEntidades As IEnumerable(Of EntidadeExportacao)
            Dim palavra As String

            listaEntidades = dgEntidades.ItemsSource

            sw.Write("VALUE_DATE,REFERENCE,DESTINATION_NAME,DESTINATION_SORT_CODE,DESTINATION_ACCOUNT_NUMBER,AMOUNT,CURRENCY,NARRATIVE,DESTINATION_EMAIL")
            sw.Write(sw.NewLine)

            For Each entidade As EntidadeExportacao In listaEntidades
                'palavra = linhasHistoricoSelecionado.DataExportacao

                For Each linhaMovimentoBanco As MovimentosBancos In entidade.listaMovimentos
                    palavra = linhasHistoricoSelecionado.DataExportacao.ToString("dd-MM-yyyy") '- VALUE_DATE
                    palavra = palavra & "," & Replace(linhaMovimentoBanco.Referencia, ",", " ") 'referencia tipodoc, documento externo - REFERENCE
                    palavra = palavra & "," & Replace(entidade.Nome, ",", " ") 'Nome da Entidade- DESTINATION_NAME

                    'palavra = palavra & "," & "00000221" 'linhaMovimentoBanco.Referencia ' Alterar por Referencia externa do Documento - DESTINATION_SORT_CODE
                    palavra = palavra & "," & Left(entidade.NIB, 8) 'Nome da Entidade  - DESTINATION_SORT_CODE
                    palavra = palavra & "," & Right(entidade.NIB, 13) 'Nome da Entidade - DESTINATION_ACCOUNT_NUMBER

                    palavra = palavra & "," & Replace(linhaMovimentoBanco.Valor.ToString("0.00"), ",", ".")  'Nome da Entidade - AMOUNT

                    palavra = palavra & "," & linhaMovimentoBanco.MoedaIso ' Alterar por Referencia externa do Documento - CURRENCY
                    palavra = palavra & "," & Replace(linhaMovimentoBanco.Referencia, ",", " ")  'Nome da Entidade - NARRATIVE
                    palavra = palavra & "," & entidade.Email 'Nome da Entidade - DESTINATION_EMAIL

                    sw.Write(palavra)
                    sw.Write(sw.NewLine)
                    palavra = ""
                Next


            Next

            sw.Close()

            MessageBox.Show("Exportação criada com sucesso, confirme o ficheiro : " + strFilePath, "Relatorio da Operação", MessageBoxButton.OK, MessageBoxImage.Information)
        Catch ex As Exception
            MessageBox.Show("Ocorreu um erro durante a Exportação para o ficheiro : " + strFilePath + vbNewLine + Err.Description, "Relatorio da Operação", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try



    End Sub

    Private Sub btActualizar_Click(sender As Object, e As RoutedEventArgs) Handles btActualizar.Click
        dgHistorico.ItemsSource = clienteshelper.daListaHistoricoExpPS2()
    End Sub

    Private Sub dgHistorico_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles dgHistorico.SelectionChanged
        On Error GoTo trataerro

        'Dim selectedFile As System.Data.DataRowView
        'selectedFile = dgHistorico.Items(dgHistorico.SelectedIndex)

        Dim linhasSelecionada As HistoricoExpPS2
        linhasSelecionada = dgHistorico.SelectedItem

        dgEntidades.ItemsSource = clienteshelper.daListaEntidadesExpPS2(linhasSelecionada.IdExportacao)



        Exit Sub
trataerro:
        MsgBox("Erro: " & Err.Number & " - " & Err.Description)
    End Sub

    Private Sub dgEntidades_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles dgEntidades.SelectionChanged
        On Error GoTo trataerro

        'Dim selectedFile As System.Data.DataRowView
        'selectedFile = dgHistorico.Items(dgHistorico.SelectedIndex)

        Dim linhasHistoricoSelecionado As HistoricoExpPS2
        Dim linhasEntidadeSelecionada As EntidadeExportacao

        linhasHistoricoSelecionado = dgHistorico.SelectedItem
        linhasEntidadeSelecionada = dgEntidades.SelectedItem

        dgDocumentos.ItemsSource = clienteshelper.daListaMovimentosPorEntidadesExpPS2(linhasHistoricoSelecionado.IdExportacao,
                                        linhasEntidadeSelecionada.TipoEntidade, linhasEntidadeSelecionada.Entidade)



        Exit Sub
trataerro:
        MsgBox("Erro: " & Err.Number & " - " & Err.Description)
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim ficheiro As New Microsoft.Win32.OpenFileDialog()
        'ficheiro.Filter = "CVS Files (*.csv)|*.csv Files ();"

        ficheiro.ValidateNames = False
        ficheiro.CheckFileExists = False
        ficheiro.CheckPathExists = True

        Dim result As Boolean
        result = ficheiro.ShowDialog()
        If (result = False) Then
            Return
        End If
        txtFile.Text = ficheiro.FileName

    End Sub

    Private Sub rbCsv_Checked(sender As Object, e As RoutedEventArgs) Handles rbCsv.Checked
        Try
            rbExcell.IsChecked = False
        Catch ex As Exception

        End Try


    End Sub

    Private Sub rbExcell_Checked(sender As Object, e As RoutedEventArgs) Handles rbExcell.Checked
        Try
            rbCsv.IsChecked = False
        Catch ex As Exception

        End Try

    End Sub

    Public Sub save()
        'get the file attributes for file or directory
        'Dim attr As FileAttributes = File.GetAttributes(txtFile.Text)

        'detect whether its a directory or file
        If (Directory.Exists(txtFile.Text)) Then
            CreateCSVFile(txtFile.Text + "\" + Now.ToString("dd_MM_yyyy_h_mm_ss.CSV"))
        Else
            CreateCSVFile(txtFile.Text)
        End If
    End Sub
End Class
