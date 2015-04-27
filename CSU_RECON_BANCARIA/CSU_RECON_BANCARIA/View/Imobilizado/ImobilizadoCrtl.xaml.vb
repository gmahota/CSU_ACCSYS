Imports MahApps.Metro.Controls
Imports MahApps.Metro.Controls.Dialogs
Imports System.Threading

Public Class ImobilizadoCrtl
    Dim imobilizadoHelper As New ImobilizadoHelper
    Dim xlApp As Object
    Dim xlBook As Object
    Dim xlSheet As Object
    Public motor As Object
    Public connection As String

    Public Sub Inicializar(tipoPlataforma As Integer, codEmpresa As String, codUsuario As String, password As String, con As String)
        On Error GoTo trataerro
        ' This call is required by the designer.
        'InitializeComponent()

        imobilizadoHelper.connectionString = con

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

    Private Sub Button_Click_2(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        Try
            'Mouse.OverrideCursor = Cursors.Wait
            progressRing.IsActive = True
            'progressRing.Visibility = Windows.Visibility.Visible
            imobilizadoHelper.importarImobilizado(txtFicheiroExcell.Text, cbFolhaExcel.SelectedIndex + 1, Conversion.Int(txtLinhaInical.Text), Conversion.Int(txtLinhaFinal.Text))
            progressRing.IsActive = False

            'progressRing.Visibility = Windows.Visibility.Hidden
        Catch ex As Exception
            progressRing.IsActive = False

            'progressRing.Visibility = Windows.Visibility.Hidden

        End Try

    End Sub

    Private Sub btupdate_Click(sender As Object, e As RoutedEventArgs)

    End Sub
End Class
