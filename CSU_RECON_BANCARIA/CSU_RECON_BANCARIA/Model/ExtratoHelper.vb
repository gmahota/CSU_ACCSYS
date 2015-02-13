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
    Public objmotor As ErpBS
    Public objLista As StdBELista

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

    Public Sub incializarMotorPrimavera(tipoPlataforma As Integer, codEmpresa As String, codUsuario As String, password As String, con As String)
        On Error GoTo trataerro
        Me.tipoPlataforma = tipoPlataforma
        Me.codUsuario = codUsuario
        Me.codEmpresa = codEmpresa
        Me.password = password
        Me.connectionString = con

        'objmotor = CreateObject("ErpBS800.ErpBs")
        objmotor = New ErpBS
        objmotor.AbreEmpresaTrabalho(tipoPlataforma, codEmpresa, codUsuario, password)
        Exit Sub
trataerro:
        MsgBox(Err.Description)
    End Sub

    Public Function daListaBancos() As IEnumerable(Of Bancos)
        Dim lista As New List(Of Bancos)
        objLista = objmotor.Consulta("select * from Bancos")

        While Not (objLista.NoFim Or objLista.Vazia)
            lista.Add(New Bancos(objLista.Valor("Banco"), objLista.Valor("Descricao")))

            objLista.Seguinte()
        End While

        Return lista
    End Function


    Public Function daListaContasBancarias(banco As String) As IEnumerable(Of ContasBancarias)
        Dim lista As New List(Of ContasBancarias)
        objLista = objmotor.Consulta("select * from ContasBancarias where banco = '" & banco & "'")

        While Not (objLista.NoFim Or objLista.Vazia)
            lista.Add(New ContasBancarias(objLista.Valor("Conta"), objLista.Valor("NumConta"), objLista.Valor("Banco"), objLista.Valor("DescBanco")))

            objLista.Seguinte()
        End While

        Return lista
    End Function

    Public Function daLinhasFormatoBancario(formato As String, ByRef tipoItem As String) As IEnumerable(Of LinhasFormatoBancario)
        Dim lista As New List(Of LinhasFormatoBancario)
        objLista = objmotor.Consulta("select * from LinhasFormatosImportacao where formato = '" & formato & "'")

        While Not (objLista.NoFim Or objLista.Vazia)
            lista.Add(
                New LinhasFormatoBancario(objLista.Valor("Formato"), objLista.Valor("TipoItem"), objLista.Valor("Campo"),
                                          objLista.Valor("Posicao"), objLista.Valor("Comprimento"), objLista.Valor("FormatoEspecial")))

            objLista.Seguinte()
        End While

        Return lista
    End Function

    Public Function daLinhasFormatoBancario(formatoBancario As FormatoBancario) As IEnumerable(Of LinhasFormatoBancario)
        Dim lista As New List(Of LinhasFormatoBancario)
        Dim linhasFormatoBancario As LinhasFormatoBancario

        objLista = objmotor.Consulta("select * from LinhasFormatosImportacao where formato = '" & formatoBancario.Formato & "'")

        While Not (objLista.NoFim Or objLista.Vazia)
            linhasFormatoBancario = New LinhasFormatoBancario(objLista.Valor("Formato"), objLista.Valor("TipoItem"), objLista.Valor("Campo"),
                                          objLista.Valor("Posicao"), objLista.Valor("Comprimento"), objLista.Valor("FormatoEspecial"))
            linhasFormatoBancario.FormatoBancario = formatoBancario
            lista.Add(linhasFormatoBancario)

            objLista.Seguinte()
        End While

        Return lista
    End Function

    Public Function daFormatoBancario() As IEnumerable(Of FormatoBancario)
        Dim lista As New List(Of FormatoBancario)
        Dim formatoBancario As FormatoBancario
        objLista = objmotor.Consulta("select * from FormatosImportacao")

        While Not (objLista.NoFim Or objLista.Vazia)
            formatoBancario = New FormatoBancario(objLista.Valor("Formato"), objLista.Valor("Descricao"), objLista.Valor("SeparadorDecimal"),
                                          objLista.Valor("SeparadorMilhares"), objLista.Valor("SeparadorDatas"))
            'formatoBancario.LinhasFormatosBancarios = daLinhasFormatoBancario(formatoBancario)
            lista.Add(formatoBancario)

            objLista.Seguinte()
        End While

        Return lista
    End Function

    Public Sub importarExtrato2(caminhoexcel As String, folhaexcel As Integer, linhaInicial As Integer, linhaFinal As Integer, banco As String, Conta As String, formatobanco As String, NumConta As String, NumExtrato As String, ByVal DataIniEx As Date, ByVal DataFimEx As Date, ByVal SaldoIni As String, ByVal SaldoFim As String)
        On Error GoTo Erro

        Dim objLista1 As StdBELista
        Dim objLista2 As StdBELista
        Dim objLista3 As StdBELista
        Dim objLista4 As StdBELista

        Dim objMotorErp As ErpBS
        Dim objListaContas As GcpBEContaBancaria
        Dim objConta As GcpBEContaBancaria
        Dim objCabecExtrato As GcpBEExtractoBancario
        Dim objLinhaExtrato As GcpBELinhaExtractoBancario

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

        objMotorErp = objmotor

        If objMotorErp.Comercial.ContasBancarias.Existe(Conta) Then

            If Len(Trim(caminhoexcel)) > 0 Then

                IdCabec = ""

                'Verifica CabecExtrato

                sqlstr2 = "SELECT ID FROM CABECEXTRACTOBANCARIO where Conta ='" & Conta & "' and NumeroConta ='" & NumConta & "' and NumeroExtracto ='" & NumExtrato & "' and Origem='F' and DataInicial='" & DataIniEx.ToString("MM/dd/yyyy") & "' and DataFinal='" & DataFimEx.ToString("MM/dd/yyyy") & "'"
                objLista2 = objMotorErp.Consulta(sqlstr2)

                Dim valor1, valor2 As String
                valor1 = Replace(SaldoIni, ".", "")
                valor2 = Replace(SaldoFim, ".", "")

                If objLista2.Vazia = True Then

                    sqlstr = "INSERT INTO CabecExtractoBancario([Id],[DataInicial],[DataFinal],[Conta],[NumeroConta],[NumeroExtracto],[SaldoInicial],[SaldoFinal],[Origem]) VALUES (newid(), '" & DataIniEx.ToString("MM/dd/yyyy") & "', '" _
                        & DataFimEx.ToString("MM/dd/yyyy") & "', '" & Conta & "', '" & NumConta & "', '" & NumExtrato & "', '" & _
                        Replace(valor1, ",", ".") & "', '" & _
                        Replace(valor2, ",", ".") & "', 'F')"
                    'objMotorErp.Comercial.ExtractosBancarios.DaValorAtributo()
                    insert_Query(sqlstr) ''adLockReadOnly

                    'Get CabecExtratoID
                    sqlstr3 = "SELECT ID FROM CabecExtractoBancario where Conta ='" & Conta & "' and NumeroConta ='" & NumConta & "' and NumeroExtracto ='" & NumExtrato & "' and Origem='F' and DataInicial='" & DataIniEx.ToString("MM/dd/yyyy") & "' and DataFinal='" & DataFimEx.ToString("MM/dd/yyyy") & "'"
                    objLista3 = objMotorErp.Consulta(sqlstr3) ''adLockReadOnly

                    If objLista3.Vazia = False Then
                        IdCabec = objLista3.Valor("ID")
                    Else
                        IdCabec = ""
                    End If

                    'Carrega Dados da folha de Excel

                    xlApp = CreateObject("Excel.Application")
                    xlBook = xlApp.Workbooks.Open(caminhoexcel)
                    xlSheet = xlBook.Worksheets(folhaexcel)

                    Ini = linhaInicial
                    Fim = linhaFinal

                    For i = Ini To Fim

                        For Each linhas As LinhasFormatoBancario In linhasFormatoBancario

                            Select Case linhas.Campo
                               
                                Case "DataMovimento"
                                    If linhas.Coluna > 0 Then
                                        DataMovimEx = DateTime.ParseExact(daValorExcell(i, linhas.Coluna), "dd/MM/yyyy",
                                    CultureInfo.InvariantCulture).ToString("MM/dd/yyyy")
                                    End If
                                Case "DataValor"
                                    If linhas.Coluna > 0 Then
                                        DataValorEx = DateTime.ParseExact(daValorExcell(i, linhas.Coluna), "dd/MM/yyyy",
                                   CultureInfo.InvariantCulture).ToString("MM/dd/yyyy")
                                    End If
                                Case "ValorMov"
                                    If linhas.Coluna > 0 Then
                                        Valor = daValorExcell(i, linhas.Coluna)
                                        MovBnc = IIf(daValorExcell(i, linhas.Coluna) > 0, "DVC", "DVD")
                                        Natureza = IIf(daValorExcell(i, linhas.Coluna) > 0, "C", "D")
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

                        If Left(Obs, 10) = "Pag. Serv." Then Numero = Right(Obs, 11)

                        ValorMov = IIf(Valor > 0, Valor, Valor * -1)
                        ValorConta = IIf(Valor > 0, Valor, Valor * -1)
                        MoedaMov = objmotor.Comercial.ContasBancarias.Edita(Conta).Moeda
                        MoedaConta = objmotor.Comercial.ContasBancarias.Edita(Conta).Moeda

                        'Insere Linhas

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

                objLista2.Vazia()



            Else

                MsgBox("Seleccione p.f. o ficheiro Excel a reconciliar.", vbInformation, "Aviso")
                Exit Sub
            End If

        Else

            MsgBox("Seleccione p.f. a conta bancária a reconciliar.", vbInformation, "Aviso")
            Exit Sub
        End If

        'objMotorErp.FechaEmpresaTrabalho
        objMotorErp = Nothing
        Exit Sub

Erro:
        MsgBox("Erro: " & Err.Number & " - " & Err.Description)

    End Sub

    Public Sub importarExtrato(caminhoexcel As String, folhaexcel As Integer, linhaInicial As Integer, linhaFinal As Integer, banco As String, Conta As String, NumConta As String, NumExtrato As String, ByVal DataIniEx As Date, ByVal DataFimEx As Date, ByVal SaldoIni As String, ByVal SaldoFim As String)
        On Error GoTo Erro

        Dim objLista1 As StdBELista
        Dim objLista2 As StdBELista
        Dim objLista3 As StdBELista
        Dim objLista4 As StdBELista

        Dim objMotorErp As ErpBS
        Dim objListaContas As GcpBEContaBancaria
        Dim objConta As GcpBEContaBancaria
        Dim objCabecExtrato As GcpBEExtractoBancario
        Dim objLinhaExtrato As GcpBELinhaExtractoBancario

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
        Dim DataMovimEx As String
        Dim DataValorEx As String
        Dim Movimento As String
        Dim Natureza As String
        Dim MovBnc As String
        Dim Numero As String
        Dim Obs As String

        Dim ValorMov As Double
        Dim ValorConta As Double
        Dim MoedaMov As String
        Dim MoedaConta As String
        Dim Ini As Integer
        Dim Fim As Integer
        Dim Valor As Double


        If Not (objMotorErp Is Nothing) Then objMotorErp.FechaEmpresaTrabalho()
        objMotorErp = Nothing
        objMotorErp = objmotor

        'objMotorErp.AbreEmpresaTrabalho 0, REG_BD, REG_UTL, REG_UTLPWS, Nothing, "DEFAULT", False
        'objMotorErp.AbreEmpresaTrabalho 0, REG_BD, REG_UTL, REG_UTLPWS, Nothing, BSO.Contexto.Instancia, False


        'If objMotorErp.Comercial.ContasBancarias.Existe(Me.CmbCodConta.Text) Then
        If objMotorErp.Comercial.ContasBancarias.Existe(Conta) Then

            If Len(Trim(caminhoexcel)) > 0 Then

                IdCabec = ""

                'Verifica CabecExtrato

                sqlstr2 = "SELECT ID FROM CABECEXTRACTOBANCARIO where Conta ='" & Conta & "' and NumeroConta ='" & NumConta & "' and NumeroExtracto ='" & NumExtrato & "' and Origem='F' and DataInicial='" & DataIniEx.ToString("MM/dd/yyyy") & "' and DataFinal='" & DataFimEx.ToString("MM/dd/yyyy") & "'"
                objLista2 = objMotorErp.Consulta(sqlstr2)


                If objLista2.Vazia = True Then

                    sqlstr = "INSERT INTO CabecExtractoBancario([Id],[DataInicial],[DataFinal],[Conta],[NumeroConta],[NumeroExtracto],[SaldoInicial],[SaldoFinal],[Origem]) VALUES (newid(), '" & DataIniEx.ToString("MM/dd/yyyy") & "', '" & DataFimEx.ToString("MM/dd/yyyy") & "', '" & Conta & "', '" & NumConta & "', '" & NumExtrato & "', '" & SaldoIni & "', '" & SaldoFim & "', 'F')"
                    'objMotorErp.Comercial.ExtractosBancarios.DaValorAtributo()
                    insert_Query(sqlstr) ''adLockReadOnly



                    'Get CabecExtratoID
                    sqlstr3 = "SELECT ID FROM CabecExtractoBancario where Conta ='" & Conta & "' and NumeroConta ='" & NumConta & "' and NumeroExtracto ='" & NumExtrato & "' and Origem='F' and DataInicial='" & DataIniEx.ToString("MM/dd/yyyy") & "' and DataFinal='" & DataFimEx.ToString("MM/dd/yyyy") & "'"
                    objLista3 = objMotorErp.Consulta(sqlstr3) ''adLockReadOnly

                    If objLista3.Vazia = False Then
                        IdCabec = objLista3.Valor("ID")
                    Else
                        IdCabec = ""
                    End If



                    'Carrega Dados da folha de Excel

                    xlApp = CreateObject("Excel.Application")
                    xlBook = xlApp.Workbooks.Open(caminhoexcel)
                    xlSheet = xlBook.Worksheets(folhaexcel) '(Me.CmbFolha.ListIndex + 1)


                    Ini = linhaInicial
                    Fim = linhaFinal



                    For i = Ini To Fim

                        'Banco BCI
                        ' If Conta = "BCI" Then

                        Valor = xlSheet.cells(i, 6).Value
                        'Valor = Replace(Valor, ",", ".")

                        DataMovimEx = Convert.ToDateTime(xlSheet.cells(i, 1).Value)
                        DataValorEx = Convert.ToDateTime(xlSheet.cells(i, 4).Value)
                        MovBnc = IIf(xlSheet.cells(i, 6).Value > 0, "DVC", "DVD")
                        Natureza = IIf(xlSheet.cells(i, 6).Value > 0, "C", "D")

                        If Left(xlSheet.cells(i, 5).Value, 10) = "Pag. Serv." Then
                            Numero = Right(xlSheet.cells(i, 5).Value, 11)
                        Else
                            Numero = Left(xlSheet.cells(i, 2).Value, 15)
                        End If

                        Obs = Left(xlSheet.cells(i, 5).Value, 250)

                        ValorMov = IIf(Valor > 0, Valor, Valor * -1)
                        ValorConta = IIf(Valor > 0, Valor, Valor * -1)
                        MoedaMov = objmotor.Comercial.ContasBancarias.Edita(Conta).Moeda
                        MoedaConta = objmotor.Comercial.ContasBancarias.Edita(Conta).Moeda

                        'Insere Linhas

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

                objLista2.Vazia()



            Else

                MsgBox("Seleccione p.f. o ficheiro Excel a reconciliar.", vbInformation, "Aviso")
                Exit Sub
            End If

        Else

            MsgBox("Seleccione p.f. a conta bancária a reconciliar.", vbInformation, "Aviso")
            Exit Sub
        End If

        'objMotorErp.FechaEmpresaTrabalho
        objMotorErp = Nothing
        Exit Sub

Erro:
        MsgBox("Erro: " & Err.Number & " - " & Err.Description)

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

    Private Function daValorExcell(linhas As Integer, coluna As Integer) As Object

        Return xlSheet.cells(linhas, coluna).Value
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

    Public Sub New()

    End Sub

    Public Sub New(ByVal conta As String,
                   ByVal numconta As String,
                   ByVal banco As String,
                   ByVal descricao As String)
        Me.Conta = conta
        Me.NumConta = numconta
        Me.Banco = banco
        Me.Descricao = descricao
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