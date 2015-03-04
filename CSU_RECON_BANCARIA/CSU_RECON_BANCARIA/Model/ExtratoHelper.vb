Imports Interop.StdBE800
Imports Interop.ErpBS800
Imports Interop.GcpBE800
Imports System.Data.SqlClient
Imports System.Data
Imports System.Net.Mail
Imports System.Threading
Imports System.Globalization


Public Class ExtratoHelper
    Public tipoPlataforma As Integer
    Public codEmpresa As String
    Public codUsuario As String
    Public password As String
    'Public objmotor As ErpBS
    'Public objLista As StdBELista

    Dim i As Long
    Dim xlApp As Object
    Dim xlBook As Object
    Dim xlSheet As Object
    Dim Tipo As String


    'Declare the string variable 'connectionString' to hold the ConnectionString        
    Public connectionString As String = "Data Source=ACCPRI08\PRIMAVERAV810;Initial Catalog= PRICLONE;User Id= sa;Password=Accsys2011"

    Dim myConnection As SqlConnection
    Dim myCommand As SqlCommand
    Dim myAdapter As SqlDataAdapter


    Public Sub openExcell(caminhoficheiro As String)
        ' Excell file

        On Error GoTo Sair

        xlApp = CreateObject("Excel.Application")
        xlBook = xlApp.Workbooks.Open(caminhoficheiro)
        xlSheet = xlBook.Worksheets(1)

        Exit Sub

Sair:
        MsgBox(Err.Description, vbInformation, "erro: " & Err.Number)
    End Sub

    '    Public Sub incializarMotorPrimavera(tipoPlataforma As Integer, codEmpresa As String, codUsuario As String, password As String, con As String)
    '        On Error GoTo trataerro
    '        Me.tipoPlataforma = tipoPlataforma
    '        Me.codUsuario = codUsuario
    '        Me.codEmpresa = codEmpresa
    '        Me.password = password
    '        Me.connectionString = con

    '        'objmotor = CreateObject("ErpBS800.ErpBs")
    '        objmotor = New ErpBS
    '        objmotor.AbreEmpresaTrabalho(tipoPlataforma, codEmpresa, codUsuario, password)
    '        Exit Sub
    'trataerro:
    '        MsgBox(Err.Description)
    '    End Sub

    Public Function daListaBancos() As IEnumerable(Of Bancos)
        Dim lista As New List(Of Bancos)
       
        Dim dt = search_Query("select * from Bancos")

        For Each row As DataRow In dt.Rows
            lista.Add(New Bancos(row("Banco"), row("Descricao")))

        Next

        Return lista
    End Function

    Public Function daListaContasBancarias() As IEnumerable(Of ContasBancarias)
        Dim lista As New List(Of ContasBancarias)
        Dim dt = search_Query("select * from ContasBancarias ")

        For Each row As DataRow In dt.Rows
            lista.Add(New ContasBancarias(row("Conta").ToString(), row("NumConta").ToString(),
                                          row("Banco").ToString(), row("DescBanco").ToString(), row("Moeda").ToString()))

        Next

        Return lista
    End Function

    Public Function daListaContasBancarias(banco As String) As IEnumerable(Of ContasBancarias)
        Dim lista As New List(Of ContasBancarias)
        Dim dt = search_Query("select * from ContasBancarias where banco = '" & banco & "'")
        
        For Each row As DataRow In dt.Rows
            lista.Add(New ContasBancarias(row("Conta").ToString(), row("NumConta").ToString(), row("Banco").ToString(),
                                          row("DescBanco").ToString(), row("Moeda").ToString()))

        Next
    
        Return lista
    End Function

    Public Function daLinhasFormatoBancario(formato As String, ByRef tipoItem As String) As IEnumerable(Of LinhasFormatoBancario)
        Dim lista As New List(Of LinhasFormatoBancario)

        Dim dt = search_Query("select * from LinhasFormatosImportacao where formato = '" & formato & "'")

        For Each row As DataRow In dt.Rows
            lista.Add(
                New LinhasFormatoBancario(row("Formato").ToString(), row("TipoItem").ToString(), row("Campo").ToString(),
                                          row("Posicao").ToString(), row("Comprimento").ToString(), row("FormatoEspecial").ToString()))
        Next

        Return lista
    End Function

    Public Function daLinhasFormatoBancario(formatoBancario As FormatoBancario) As IEnumerable(Of LinhasFormatoBancario)
        Dim lista As New List(Of LinhasFormatoBancario)
        Dim linhasFormatoBancario As LinhasFormatoBancario

        Dim dt = search_Query("select * from LinhasFormatosImportacao where formato = '" & formatoBancario.Formato & "'")

        For Each row As DataRow In dt.Rows
            
            linhasFormatoBancario = New LinhasFormatoBancario(row("Formato").ToString(), row("TipoItem").ToString(),
                                                              row("Campo").ToString(),
                                          row("Posicao").ToString(), row("Comprimento").ToString(), row("FormatoEspecial").ToString())

            linhasFormatoBancario.FormatoBancario = formatoBancario
            lista.Add(linhasFormatoBancario)
        Next

        Return lista
    End Function

    Public Function daFormatoBancario() As IEnumerable(Of FormatoBancario)
        Dim lista As New List(Of FormatoBancario)
        Dim formatoBancario As FormatoBancario
        
        Dim dt = search_Query("select * from FormatosImportacao")

        For Each row As DataRow In dt.Rows

            formatoBancario = New FormatoBancario(row("Formato").ToString(), row("Descricao").ToString(),
                                                  row("SeparadorDecimal").ToString(),
                                          row("SeparadorMilhares").ToString(), row("SeparadorDatas").ToString())

            lista.Add(formatoBancario)
        Next

        Return lista
    End Function

    Public Function daListaCabecExtractoBancario(query As String) As IEnumerable(Of CabecExtractoBancario)
        Dim lista As New List(Of CabecExtractoBancario)
        Dim cabecExtratoBancario As CabecExtractoBancario

        Dim dt = search_Query(query)

        For Each row As DataRow In dt.Rows

            cabecExtratoBancario = New CabecExtractoBancario(row("Conta").ToString(), row("NumeroConta").ToString(),
                                                             row("NumeroExtracto").ToString(), row("Origem").ToString(),
                                                             Convert.ToDateTime(row("DataInicial")).Date,
                                                             Convert.ToDateTime(row("DataFinal")).Date, row("Id"))

            lista.Add(cabecExtratoBancario)
        Next

        Return lista
    End Function

    Public Function daCabecExtractoBancario(query As String) As CabecExtractoBancario
        Dim lista As New List(Of CabecExtractoBancario)
        Dim cabecExtratoBancario As CabecExtractoBancario

        Dim dt = search_Query(query)

        For Each row As DataRow In dt.Rows
            
            cabecExtratoBancario = New CabecExtractoBancario(row("Conta").ToString(), row("NumeroConta").ToString(),
                                                             row("NumeroExtracto").ToString(), row("Origem").ToString(),
                                                             Convert.ToDateTime(row("DataInicial")).Date,
                                                             Convert.ToDateTime(row("DataFinal")).Date, row("Id"))

            lista.Add(cabecExtratoBancario)
        Next
        If lista.Count > 0 Then Return lista(0)

        Return New CabecExtractoBancario()

    End Function

    Public Sub importarExtrato2(caminhoexcel As String, folhaexcel As Integer, linhaInicial As Integer, linhaFinal As Integer, banco As String, Conta As String, formatobanco As String, NumConta As String, NumExtrato As String, ByVal DataIniEx As Date, ByVal DataFimEx As Date, ByVal SaldoIni As String, ByVal SaldoFim As String)
        Try

            Dim i As Long
            Dim sqlstr As String
            Dim sqlstr1 As String
            Dim sqlstr2 As String
            Dim sqlstr3 As String
            Dim sqlstr4 As String

            Dim xlApp As Object
            Dim xlBook As Object
            Dim xlSheet As Object

            Dim IdCabec As String

            Dim IdLinhas As String
            Dim DataMovimEx As New String("")
            Dim DataValorEx As New String("")
            Dim Movimento As New String("")
            Dim Natureza As New String("")
            Dim MovBnc As New String("")
            Dim Numero As New String("")
            Dim Obs As New String("")

            Dim ValorMov As Double
            Dim ValorConta As Double
            Dim MoedaMov As String
            Dim MoedaConta As String
            Dim Ini As Integer
            Dim Fim As Integer
            Dim Valor As Double

            Dim linhasFormatoBancario As List(Of LinhasFormatoBancario)

            openExcell(caminhoexcel)

            'variavel temporaria
            Dim temp As LinhasFormatoBancario

            linhasFormatoBancario = daLinhasFormatoBancario(formatobanco, "")

            Thread.CurrentThread.CurrentCulture = New CultureInfo("pt-PT")

            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("pt-PT")

            'objMotorErp = objmotor
            Dim varConta As ContasBancarias
            varConta = daListaContasBancarias().Where(Function(x) x.Conta = Conta).First()

            If Not IsNothing(varConta) Then

                If Len(Trim(caminhoexcel)) > 0 Then

                    IdCabec = ""

                    'Verifica CabecExtrato
                    Dim lista As New List(Of ContasBancarias)
                    sqlstr2 = "SELECT * FROM CABECEXTRACTOBANCARIO where Conta ='" & Conta & "' and NumeroConta ='" & NumConta & "' and NumeroExtracto ='" & NumExtrato & "' and Origem='F' and DataInicial='" & DataIniEx.ToString("MM/dd/yyyy") & "' and DataFinal='" & DataFimEx.ToString("MM/dd/yyyy") & "'"

                    Dim dt2 = search_Query(sqlstr2)

                    Dim cbextrato = daCabecExtractoBancario(sqlstr2)
                    'objLista2 = objMotorErp.Consulta(sqlstr2)

                    'For Each row As DataRow In dt.Rows
                    'lista.Add(New ContasBancarias(row("Conta"), row("NumConta"), row("Banco"), row("DescBanco")))

                    'Next

                    Dim valor1, valor2 As String
                    valor1 = Replace(SaldoIni, ".", "")
                    valor2 = Replace(SaldoFim, ".", "")

                    If cbextrato.id = New Guid("{00000000-0000-0000-0000-000000000000}") Then

                        sqlstr = "INSERT INTO CabecExtractoBancario([Id],[DataInicial],[DataFinal],[Conta],[NumeroConta],[NumeroExtracto],[SaldoInicial],[SaldoFinal],[Origem]) VALUES (newid(), '" & DataIniEx.ToString("MM/dd/yyyy") & "', '" _
                            & DataFimEx.ToString("MM/dd/yyyy") & "', '" & Conta & "', '" & NumConta & "', '" & NumExtrato & "', '" & _
                            Replace(valor1, ",", ".") & "', '" & _
                            Replace(valor2, ",", ".") & "', 'F')"

                        insert_Query(sqlstr)

                        'Get CabecExtratoID
                        sqlstr3 = "SELECT * FROM CabecExtractoBancario where Conta ='" & Conta & "' and NumeroConta ='" & NumConta & "' and NumeroExtracto ='" & NumExtrato & "' and Origem='F' and DataInicial='" & DataIniEx.ToString("MM/dd/yyyy") & "' and DataFinal='" & DataFimEx.ToString("MM/dd/yyyy") & "'"
                        'objLista3 = objMotorErp.Consulta(sqlstr3) ''adLockReadOnly

                        cbextrato = daCabecExtractoBancario(sqlstr3)
                        'Dim dt3 = search_Query(sqlstr3)

                        IdCabec = cbextrato.id.ToString()

                        'Carrega Dados da folha de Excel

                        xlApp = CreateObject("Excel.Application")
                        xlBook = xlApp.Workbooks.Open(caminhoexcel)
                        xlSheet = xlBook.Worksheets(folhaexcel)

                        Ini = linhaInicial
                        Fim = linhaFinal
                        Dim valorDebito As Double = 0
                        Dim valorCredito As Double = 0
                        Dim formatoBancario As FormatoBancario
                        formatoBancario = daFormatoBancario().Where(Function(x) x.Formato = linhasFormatoBancario(i).Formato).First

                        For i = Ini To Fim

                            For Each linhas As LinhasFormatoBancario In linhasFormatoBancario

                                Select Case linhas.Campo

                                    Case "DataMovimento"
                                        If linhas.Coluna > 0 Then
                                            DataMovimEx = daData(daValorExcell(i, linhas.Coluna), linhas.FormatoEspecial)
                                        End If
                                    Case "DataValor"
                                        If linhas.Coluna > 0 Then
                                            DataValorEx = daData(daValorExcell(i, linhas.Coluna), linhas.FormatoEspecial)

                                        End If
                                    Case "ValorMov"
                                        If linhas.Coluna > 0 Then

                                            Select Case linhas.FormatoEspecial
                                                Case "D"
                                                    valorDebito = daDouble(daValorExcell(i, linhas.Coluna), formatoBancario.SeparadorMilhares, formatoBancario.SeparadorDecimal) '( IIf(daValorExcell(i, linhas.Coluna) > 0, daValorExcell(i, linhas.Coluna), daValorExcell(i, linhas.Coluna) * -1)
                                                    MovBnc = "DVD"
                                                    Natureza = "D"
                                                Case "C"
                                                    valorCredito = daDouble(daValorExcell(i, linhas.Coluna), formatoBancario.SeparadorMilhares, formatoBancario.SeparadorDecimal)
                                                    MovBnc = "DVC"
                                                    Natureza = "C"
                                                Case Else
                                                    Valor = daDouble(daValorExcell(i, linhas.Coluna), formatoBancario.SeparadorMilhares, formatoBancario.SeparadorDecimal)
                                                    MovBnc = IIf(daValorExcell(i, linhas.Coluna) > 0, "DVC", "DVD")
                                                    Natureza = IIf(daValorExcell(i, linhas.Coluna) > 0, "C", "D")

                                            End Select
                                        End If
                                    Case "Obs"
                                        If linhas.Coluna > 0 Then
                                            Obs = Left(daValorExcell(i, linhas.Coluna), 250)
                                        End If
                                    Case "Numero"
                                        If linhas.Coluna > 0 Then
                                            Numero = Left(daValorExcell(i, linhas.Coluna), 15)
                                        End If

                                End Select
                            Next


                            'If Left(Obs, 10) = "Pag. Serv." Then Numero = Right(Obs, 11)

                            Valor = IIf(valorCredito - valorDebito > 0, valorCredito, valorDebito)
                            MovBnc = IIf(valorCredito - valorDebito > 0, "DVD", "DVC")
                            Natureza = IIf(valorCredito - valorDebito > 0, "D", "C")

                            valorCredito = 0
                            valorDebito = 0

                            ValorMov = IIf(Valor > 0, Valor, Valor * -1)
                            ValorConta = IIf(Valor > 0, Valor, Valor * -1)
                            MoedaMov = varConta.Moeda
                            MoedaConta = varConta.Moeda

                            'Insere Linhas

                            'sqlstr4 = "INSERT INTO LINHASEXTRACTOBANCARIO([Id],[IdCabecExtractoBancario],[DataMovimento],[DataValor]," & _
                            '"[Movimento],[Natureza],[Numero],[Obs],[ValorMov],[ValorConta],[MoedaMov],[MoedaConta]) VALUES (newid(), '" _
                            '& IdCabec & "', '" & DataMovimEx & "', '" & DataValorEx & "', '" & MovBnc & "', '" & Natureza & "', '" & Numero _
                            '& "', '" & Obs & "', '" & Replace(ValorMov, ",", ".") & "', '" & Replace(ValorConta, ",", ".") & "', '" & MoedaMov & "', '" & MoedaConta & "')"


                            sqlstr4 = "INSERT INTO LINHASEXTRACTOBANCARIO([Id],[IdCabecExtractoBancario],[DataMovimento],[DataValor]," & _
                            "[Movimento],[Natureza],[Numero],[Obs],[ValorMov],[ValorConta],[MoedaMov],[MoedaConta]) VALUES (newid(), '" _
                            & IdCabec & "', '" & DataMovimEx & "', '" & DataValorEx & "', '" & MovBnc & "', '" & Natureza & "', '" & Numero _
                            & "', '" & Obs & "', '" & Replace(ValorMov, ",", ".") & "', '" & Replace(ValorConta, ",", ".") & "', '" & MoedaMov & "', '" & MoedaConta & "')"


                            insert_Query(sqlstr4) ''adLockReadOnly

                        Next i


                        MsgBox("Importação realizada com sucesso.", vbInformation, "Aviso")
                        xlBook.Close()
                        'Quit excel (automatically closes all workbooks)
                        xlApp.Quit()

                        xlApp = Nothing
                        xlBook = Nothing
                        xlSheet = Nothing



                        'rst3.Close()
                    Else
                        MsgBox(" O extracto já importado!", vbInformation, "Aviso")

                    End If

                Else

                    MsgBox("Seleccione p.f. o ficheiro Excel a reconciliar.", vbInformation, "Aviso")
                    Exit Sub
                End If

            Else

                MsgBox("Seleccione p.f. a conta bancária a reconciliar.", vbInformation, "Aviso")
                Exit Sub
            End If

        Catch ex As Exception
            MsgBox("Erro: " & Err.Number & " - " & Err.Description)
        End Try

        Exit Sub
    End Sub

    Public Function insert_Query(str_query As String) As String
        Dim numRows As Integer

        myConnection = New SqlConnection(connectionString)

        'str_query = "select * from artigo"
        myCommand = New SqlCommand(str_query, myConnection)
        myConnection.Open()

        myAdapter = New SqlDataAdapter(myCommand)
        myAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
        myAdapter.InsertCommand = New SqlCommand(str_query, myConnection)
        numRows = myAdapter.InsertCommand.ExecuteNonQuery()

        Return numRows.ToString()
    End Function

    Public Function search_Query(str_query As String) As DataTable

        Dim dt = New DataTable()
        myConnection = New SqlConnection(connectionString)

        'str_query = "select * from artigo"
        myCommand = New SqlCommand(str_query, myConnection)
        myConnection.Open()

        myAdapter = New SqlDataAdapter(myCommand)
        myAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey

        myAdapter.Fill(dt)

        Return dt
    End Function

    Private Function daValorExcell(linhas As Integer, coluna As Integer) As Object

        Return xlSheet.cells(linhas, coluna).Value
    End Function

    Private Function daData(data As Object, formatoEspecial As String) As String

        Return CDate(data).ToString("MM/dd/yyyy")
        ' DateTime.ParseExact(data.ToString(), formatoEspecial, CultureInfo.InvariantCulture).ToString("MM/dd/yyyy")
        'DateTime.ParseExact(data, formatoEspecial, CultureInfo.InvariantCulture).ToString("MM/dd/yyyy")
    End Function

    Private Function daDouble(valorExcell As Object, separadorMilhares As String, sepraradorDecimal As String) As Double

        Dim temp As String

        
        Try
            temp = Replace(valorExcell.ToString(), separadorMilhares, "")
            temp = Replace(temp, sepraradorDecimal, ",")
            temp = Replace(temp, "-", "")
            temp = Replace(temp, "+", "")
            Return IIf(Convert.ToDouble(temp) < 0, Convert.ToDouble(temp) * -1, Convert.ToDouble(temp))

        Catch ex As Exception
            Return 0
        End Try

    End Function

End Class

Public Class Bancos
    Public Property Banco As String
    Public Property Descricao As String

    Public Sub New()

    End Sub

    Public Sub New(ByVal banco As String,
                   ByVal descricao As String)
        Me.Banco = banco
        Me.Descricao = descricao
    End Sub
End Class

Public Class ContasBancarias
    Public Property Conta As String
    Public Property NumConta As String
    Public Property Banco As String
    Public Property Descricao As String
    Public Property Moeda As String


    Public Sub New()

    End Sub

    Public Sub New(ByVal conta As String,
                   ByVal numconta As String,
                   ByVal banco As String,
                   ByVal descricao As String, ByVal moeda As String)
        Me.Conta = conta
        Me.NumConta = numconta
        Me.Banco = banco
        Me.Descricao = descricao
        Me.Moeda = moeda
    End Sub

End Class

Public Class LinhasFormatoBancario
    Public Property Formato As String
    Public Property TipoItem As String
    Public Property Campo As String
    Public Property Coluna As Integer

    Public Property Linhas As Integer

    Public Property FormatoEspecial As String

    Public Property FormatoBancario As FormatoBancario

    Public Sub New()
    End Sub

    Public Sub New(ByVal formato As String,
                   ByVal tipoItem As String,
                   ByVal campo As String,
                   ByVal posicao As String, ByVal comprimento As String, ByVal formatoEspecial As String)
        Me.Formato = formato
        Me.TipoItem = tipoItem
        Me.Campo = campo
        Me.Coluna = Conversion.Int(posicao)
        Me.Linhas = Conversion.Int(comprimento)
        Me.FormatoEspecial = formatoEspecial
    End Sub
End Class

Public Class FormatoBancario
    Public Property Formato As String
    Public Property Descricao As String
    Public Property SeparadorDecimal As String
    Public Property SeparadorMilhares As String

    Public Property SeparadorDatas As String

    Public Property LinhasFormatosBancarios As List(Of LinhasFormatoBancario)

    Public Sub New()
    End Sub

    Public Sub New(ByVal formato As String,
                   ByVal descricao As String,
                   ByVal separadorDecimal As String,
                   ByVal separadorMilhares As String, ByVal separadorDatas As String)
        Me.Formato = formato
        Me.Descricao = descricao
        Me.SeparadorMilhares = separadorMilhares
        Me.SeparadorDecimal = separadorDecimal
        Me.SeparadorDatas = separadorDatas

    End Sub
End Class

Public Class CabecExtractoBancario

    Sub New()
        ' TODO: Complete member initialization 
    End Sub

    Public Property Conta As String
    Public Property NumeroConta As String
    Public Property NumeroExtracto As String
    Public Property Origem As String
    Public Property DataInicial As Date
    Public Property DataFinal As Date
    Public Property id As Guid

    Public Sub New(ByRef conta As String, ByRef NumeroConta As String, ByRef NumeroExtracto As String, ByRef Origem As String,
                   ByRef DataInicial As Date, ByRef DataFinal As Date, ByRef id As Guid)
        Me.Conta = conta
        Me.NumeroConta = NumeroConta
        Me.NumeroExtracto = NumeroExtracto
        Me.Origem = Origem
        Me.DataFinal = DataFinal
        Me.DataInicial = DataInicial
        Me.id = id
    End Sub



End Class