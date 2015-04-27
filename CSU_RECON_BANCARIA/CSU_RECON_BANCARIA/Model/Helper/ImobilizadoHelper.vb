Imports System.Data.SqlClient
Imports System.Data
Imports System.Globalization
Imports System.Reflection

Imports Interop.ErpBS900
Imports Interop.IEapBS900
Imports Interop.EapBE900

Public Class ImobilizadoHelper
    Public listaImobilizado As List(Of Imobilizado)

    Public tipoPlataforma As Integer
    Public codEmpresa As String
    Public codUsuario As String
    Public password As String
    Public Const pastaConfig As String = "PRIMAVERA\\SG900"

    'Public objmotor As ErpBS
    'Public objLista As StdBELista

    Dim i As Long
    Dim xlApp As Object
    Dim xlBook As Object
    Dim xlSheet As Object
    Dim Tipo As String

    'Declare the string variable 'connectionString' to hold the ConnectionString        
    Public connectionString As String = "Data Source = ACCPRI08\PRIMAVERAV810;Initial Catalog= PRICLONE;User Id= sa;Password=Accsys2011"

    Dim myConnection As SqlConnection
    Dim myCommand As SqlCommand
    Dim myAdapter As SqlDataAdapter

    Public Sub importarImobilizado(caminhoexcel As String, folhaexcel As Integer, linhaInicial As Integer, linhaFinal As Integer)

        Dim i As Long
        Dim imobilizado As Imobilizado
        listaImobilizado = New List(Of Imobilizado)
        Try

            openExcell(caminhoexcel, folhaexcel)

            For i = linhaInicial To linhaFinal
                imobilizado = New Imobilizado()
                imobilizado.id = daValorExcell(i, 1)
                imobilizado.Codigo = daValorExcell(i, 2)
                imobilizado.dataAquisicao = daData(daValorExcell(i, 3), "pt-PT")
                imobilizado.dataUtilizacao = daData(daValorExcell(i, 4), "pt-PT")
                imobilizado.nome = daValorExcell(i, 5)
                imobilizado.tipoImobilizado = daValorExcell(i, 6)
                imobilizado.diploma = daValorExcell(i, 7)
                imobilizado.codigoFiscal = daValorExcell(i, 8)
                imobilizado.contaPOS = daValorExcell(i, 9)
                imobilizado.valorActual = daDouble(daValorExcell(i, 10), ".", ",")
                imobilizado.exercicio = daValorExcell(i, 11)
                imobilizado.nrElementos = daValorExcell(i, 12)
                imobilizado.ValorAquisicao = daDouble(daValorExcell(i, 13), ".", ",")

                listaImobilizado.Add(imobilizado)
            Next i

            importarAoErp()
        Catch ex As Exception
            If Not xlBook Is Nothing Then
                xlBook.Close()
                'Quit excel (automatically closes all workbooks)
                xlApp.Quit()

                xlApp = Nothing
                xlBook = Nothing
                xlSheet = Nothing
            End If

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

    Public Function delete_Query(str_query As String) As String
        Dim numRows As Integer

        myConnection = New SqlConnection(connectionString)

        'str_query = "select * from artigo"
        myCommand = New SqlCommand(str_query, myConnection)
        myConnection.Open()

        myAdapter = New SqlDataAdapter(myCommand)
        myAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
        myAdapter.DeleteCommand = New SqlCommand(str_query, myConnection)
        numRows = myAdapter.DeleteCommand.ExecuteNonQuery()

        Return numRows.ToString()
    End Function

    Public Sub openExcell(caminhoficheiro As String, folhaexcel As Integer)
        ' Excell file

        On Error GoTo Sair

        xlApp = CreateObject("Excel.Application")
        xlBook = xlApp.Workbooks.Open(caminhoficheiro)
        xlSheet = xlBook.Worksheets(folhaexcel)

        Exit Sub

Sair:
        MsgBox(Err.Description, vbInformation, "erro: " & Err.Number)
    End Sub

    Private Function daValorExcell(linhas As Integer, coluna As Integer) As Object

        Return xlSheet.cells(linhas, coluna).Value
    End Function

    Private Function daData(data As Object, cultura As String) As Date
        'Return Format(data, formatoEspecial)
        'Return CDate(data).ToString("MM/dd/yyyy")
        Return DateTime.Parse(data, New CultureInfo(cultura))
        'DateTime.ParseExact(data, formatoEspecial, CultureInfo.InvariantCulture)
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

    Private Sub importarAoErp()

        Dim motor As ErpBS
        Dim ficha As ImoBEFicha
        Dim valor As ImoBEFichaPlanoDepreciacao
        Dim conta As ImoBEFichaConta
        Dim centro As ImoBEFichaCentroCusto
        Dim reav As ImoBEReavaliacao
        Dim aquis As ImoBEAquisicao
        Dim objLinha As ImoBELinhaAquisicao
        Dim i As Long
        Dim j As Long

        motor = New ErpBS
        Dim linha As String

        motor.AbreEmpresaTrabalho(tipoPlataforma, codEmpresa, codUsuario, password)

        On Error GoTo erro

        For Each iventario In listaImobilizado
            ' Passos para criar a ficha do bem
            ficha = New ImoBEFicha

            ficha.Ficha = iventario.Codigo
            ficha.Activo = True
            ficha.CodBarras = iventario.Codigo
            ficha.DataAquisicao = iventario.dataAquisicao
            ficha.DataUtilizacao = iventario.dataUtilizacao
            ficha.Descricao = iventario.nome
            ficha.NElementos = iventario.nrElementos
            ficha.TipoImo = iventario.tipoImobilizado
            ficha.NumeroInventario = iventario.Codigo
            ficha.DataInventario = iventario.dataAquisicao
            ficha.Diploma = iventario.diploma
            ficha.CodFiscal = iventario.codigoFiscal



            ' Passos para criar a conta do bem
            j = Year(iventario.dataAquisicao)
            Do While j <= 2015
                conta = New ImoBEFichaConta
                conta.Ficha = iventario.Codigo
                conta.Exercicio = j
                conta.Conta = iventario.contaPOS
                ficha.ContasInvestimento.Insere(conta)
                j = j + 1
            Loop


            'Passos para criar os planos de depreciação
            'Plano Fiscal
            valor = New ImoBEFichaPlanoDepreciacao
            valor.Plano = "001"
            valor.Ficha = iventario.Codigo
            valor.ValorAquisicao = iventario.ValorAquisicao
            valor.ValorContabilistico = iventario.valorActual

            ficha.PlanosDepreciacao.Insere(valor)

            motor.Equipamentos.Fichas.Actualiza(ficha)


            ' Passos para criar o centro de custo
            '     Set centro = New EapBE700.ImoBEFichaCentroCusto
            '    centro.Bem = Cells(i, 2)
            '    centro.CentroCusto = Cells(i, 13)
            '    centro.Exercicio = Cells(i, 11)
            '    centro.Percentagem = "100"
            '    centro.Principal = True
            '    centro.Fixa = True
            '    motor.Equipamentos.FichaCentrosCusto.Actualiza centro

            ficha = motor.Equipamentos.Fichas.Edita(iventario.Codigo)

            aquis = New ImoBEAquisicao

            aquis.Tipo = "AQ"
            aquis.Alteracao = "001"
            aquis.Exercicio = Year(iventario.dataUtilizacao)
            aquis.Periodo = Month(iventario.dataUtilizacao)
            aquis.Dia = Day(iventario.dataUtilizacao)
            aquis.ExercicioDoc = Year(iventario.dataAquisicao)
            aquis.PeriodoDoc = 1
            aquis.Modulo = "E"


            objLinha = New ImoBELinhaAquisicao

            objLinha.Exercicio = aquis.Exercicio
            objLinha.Dia = aquis.Dia
            objLinha.Periodo = aquis.Periodo
            objLinha.Cambio = 1
            objLinha.Dedutivel = 100
            objLinha.Ficha = ficha.Ficha
            objLinha.Iva = "00"
            objLinha.IvaDedutivel = 0
            objLinha.Linha = 1
            objLinha.Plano = "001"
            objLinha.ValorAquisicao = iventario.ValorAquisicao

            aquis.Linhas.Insere(objLinha)

            motor.Equipamentos.Aquisicoes.Actualiza(aquis)

        Next

        MsgBox("Bens criados com sucesso!!!")
        Exit Sub
erro:
        MsgBox(Err.Description, vbOKOnly)

    End Sub

    Public Sub New()
        AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf CurrentDomain_AssemblyResolve

    End Sub

    ''' <summary>
    ''' Método para resolução das assemblies.
    ''' </summary>
    ''' <param name="sender">Application</param>
    ''' <param name="args">Resolving Assembly Name</param>
    ''' <returns>Assembly</returns>
    Public Function CurrentDomain_AssemblyResolve1(sender As Object, args As ResolveEventArgs) As System.Reflection.Assembly
        Dim assemblyFullName As String
        Dim assemblyName As System.Reflection.AssemblyName
        Const PRIMAVERA_COMMON_FILES_FOLDER As String = pastaConfig '"PRIMAVERA\\S800" ' pasta dos ficheiros comuns especifica da versão do ERP PRIMAVERA utilizada.
        assemblyName = New System.Reflection.AssemblyName(args.Name)
        assemblyFullName = System.IO.Path.Combine(System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86), PRIMAVERA_COMMON_FILES_FOLDER), assemblyName.Name + ".dll")
        If (System.IO.File.Exists(assemblyFullName)) Then
            Return System.Reflection.Assembly.LoadFile(assemblyFullName)
        Else
            Return Nothing
        End If
    End Function

    ''' <summary> 
    '''     Módulo de exemplo de carregamento de Interops da pasta de ficheiros comuns. 
    ''' </summary> 
    ''' <remarks></remarks> 
    <Runtime.InteropServices.ComVisible(False)> _
    Public Function CurrentDomain_AssemblyResolve(ByVal sender As Object, ByVal args As ResolveEventArgs) As Assembly

        Const PRIMAVERA_COMMON_FILES_FOLDER As String = pastaConfig  '"PRIMAVERA\\SG800"

        Dim outAssembly, objExeAssembly As Assembly
        Dim strTempAssemblyPath As String = ""
        Dim strArgToLoad As String

        objExeAssembly = Assembly.GetExecutingAssembly
        Dim arrRefAssemblyNames() As AssemblyName = objExeAssembly.GetReferencedAssemblies

        strArgToLoad = args.Name.Substring(0, args.Name.IndexOf(","))

        For Each strAName As AssemblyName In arrRefAssemblyNames

            If strAName.FullName.Substring(0, strAName.FullName.IndexOf(",")) = strArgToLoad Then

                strTempAssemblyPath = System.IO.Path.Combine(
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86), PRIMAVERA_COMMON_FILES_FOLDER),
                    strArgToLoad + ".dll"
                )

                Exit For
            End If

        Next

        'Valida nome do assembly
        If String.IsNullOrEmpty(strTempAssemblyPath) Then
            outAssembly = Nothing
        Else
            outAssembly = Assembly.LoadFrom(strTempAssemblyPath)
        End If

        Return outAssembly

    End Function
End Class
