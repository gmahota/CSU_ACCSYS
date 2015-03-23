Imports System.Data
Imports System.IO

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
        CreateCSVFile("c:\Teste.csv")

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
                    palavra = palavra & "," & linhaMovimentoBanco.Referencia 'referencia tipodoc, documento externo - REFERENCE
                    palavra = palavra & "," & linhaMovimentoBanco.Entidade 'Nome da Entidade - DESTINATION_NAME

                    palavra = palavra & "," & "00000221" 'linhaMovimentoBanco.Referencia ' Alterar por Referencia externa do Documento - DESTINATION_SORT_CODE
                    palavra = palavra & "," & entidade.NIB 'Nome da Entidade - DESTINATION_ACCOUNT_NUMBER
                    palavra = palavra & "," & linhaMovimentoBanco.Valor 'Nome da Entidade - AMOUNT

                    palavra = palavra & "," & "MZN" ' Alterar por Referencia externa do Documento - CURRENCY
                    palavra = palavra & "," & linhaMovimentoBanco.Referencia 'Nome da Entidade - NARRATIVE
                    palavra = palavra & "," & "vendas.tete@norco.co.mz" 'Nome da Entidade - DESTINATION_EMAIL

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
End Class
