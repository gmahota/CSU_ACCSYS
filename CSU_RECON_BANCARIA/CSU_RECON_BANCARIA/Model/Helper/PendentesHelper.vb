Imports System.Data.SqlClient
Imports System.Data

Imports erpBS900 = Interop.ErpBS900
Imports eapBS900 = Interop.IEapBS900
Imports eapBE900 = Interop.EapBE900
Imports gcpBE900 = Interop.GcpBE900

Imports erpBS800 = Interop.ErpBS800
Imports eapBS800 = Interop.IEapBS800
Imports eapBE800 = Interop.EapBE800
Imports System.Reflection

Public Class PendentesHelper
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

    Public Const pastaConfig As String = "PRIMAVERA\\SG900"

    'Declare the string variable 'connectionString' to hold the ConnectionString        
    Public connectionString As String = "Data Source=ACCPRI08\PRIMAVERAV810;Initial Catalog= PRICLONE;User Id= sa;Password=Accsys2011"

    Dim myConnection As SqlConnection
    Dim myCommand As SqlCommand
    Dim myAdapter As SqlDataAdapter

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

    Public Function daListaPendentes() As IEnumerable(Of Pendentes)
        Dim lista As New List(Of Pendentes)

        Dim dt = search_Query("select * from Pendentes where Moeda not in ('MT','MZN','MZM')")

        For Each row As DataRow In dt.Rows
            lista.Add(
                New Pendentes(row("TipoEntidade"), row("Entidade"), row("TipoDoc").ToString() + " " + row("NumDocInt").ToString() + "/" + row("Serie").ToString(),
                    row("Moeda").ToString(), row("Cambio"), row("ValorTotal"), row("ValorPendente")
                )
            )

        Next

        Return lista
    End Function

    Public Function daListaPendentesPorMoeda(moeda As String) As IEnumerable(Of Pendentes)
        Dim lista As New List(Of Pendentes)

        Dim dt = search_Query("select * from Pendentes where Moeda not in ('MT','MZN','MZM') and moeda like '" + moeda + "%'")

        For Each row As DataRow In dt.Rows
            lista.Add(
                New Pendentes(row("TipoEntidade"), row("Entidade"), row("TipoDoc").ToString() + " " + row("NumDocInt").ToString() + "/" + row("Serie").ToString(),
                    row("Moeda").ToString(), row("Cambio"), row("ValorTotal"), row("ValorPendente")
                )
            )

        Next

        Return lista
    End Function

    Public Sub gravar(listaPendentes As IEnumerable(Of Pendentes))
        gravarV900(listaPendentes)
    End Sub

    Public Sub gravarV900(listaPendentes As IEnumerable(Of Pendentes))

        Dim motor As erpBS900.ErpBS
        Dim docPendente As gcpBE900.GcpBEPendente
        Dim docLinhaPendente As gcpBE900.GCPBELinhaPendente
        Dim valor As Double

        Dim i As Long

        motor = New erpBS900.ErpBS
        motor.AbreEmpresaTrabalho(tipoPlataforma, codEmpresa, codUsuario, password)


        For Each pendente In listaPendentes
            docPendente = New gcpBE900.GcpBEPendente

            valor = pendente.ValorActualizacao

            If valor > 0 Then
                docPendente.Tipodoc = "DCP"
            Else
                docPendente.Tipodoc = "DCN"
                valor = valor * -1
            End If


            docPendente.Serie = motor.Comercial.Series.DaSerieDefeito("M", "DCN", Today)
            docPendente.Entidade = pendente.Entidade
            docPendente.TipoEntidade = pendente.TipoEntidade

            motor.Comercial.Pendentes.PreencheDadosRelacionados(docPendente)
            docPendente.Moeda = motor.Contexto.MoedaBase

            docLinhaPendente = New gcpBE900.GCPBELinhaPendente

            With docLinhaPendente
                .Incidencia = valor
                .Total = valor
                .PercIvaDedutivel = 100

            End With

            docPendente.ValorTotal = valor
            docPendente.TotalIva = 0
            docPendente.ValorPendente = valor


            docPendente.Cambio = 1
            docPendente.CambioMBase = 1
            docPendente.CambioMAlt = 0.0

            motor.Comercial.Pendentes.Actualiza(docPendente)

            docPendente.EmModoEdicao = True

            docPendente.Cambio = 1
            docPendente.CambioMBase = 1
            docPendente.CambioMAlt = 0
            docPendente.CambioADataDoc = 0
            docPendente.ValorTotal = valor
            docPendente.TotalIva = 0
            docPendente.ValorPendente = valor
            motor.Comercial.Pendentes.Actualiza(docPendente)
        Next

    End Sub


    Public Function daListaMoedas() As IEnumerable(Of Moedas)
        Dim lista As New List(Of Moedas)

        Dim dt = search_Query("select * from Moedas where Moeda not in ('MT','MZN','MZM')")

        For Each row As DataRow In dt.Rows
            lista.Add(
                New Moedas(row("Moeda"), row("Descricao"), row("Compra"), row("Venda"), row("DataCambio"))
            )

        Next

        Return lista
    End Function

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

    Public Function search_Query_For_View(str_query As String) As DataTable

        Dim ds = New DataSet()
        myConnection = New SqlConnection(connectionString)

        'str_query = "select * from artigo"
        myCommand = New SqlCommand(str_query, myConnection)
        myConnection.Open()

        myAdapter = New SqlDataAdapter(myCommand)
        myAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey

        ds.EnforceConstraints = False
        myAdapter.Fill(ds, "View")

        Return ds.Tables("View")
    End Function

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


Public Class Pendentes

    Public Property TipoEntidade As String
    Public Property Entidade As String
    Public Property Documento As String
    Public Property Moeda As String
    Public Property Cambio As Double
    Public Property ValorTotal As Double
    Public Property ValorPendente As Double
    Public Property ValorTotalMT As Double
    Public Property ValorPendenteMT As Double
    Public Property ContraValor As Double

    Public Property ValorActualizacao As Double

    Public Sub New(ByRef TipoEntidade As String, ByRef Entidade As String, ByRef documento As String, ByRef moeda As String,
                   ByRef cambio As Double, ByRef valorTotal As Double, ByRef valorPendente As Double)
        Me.TipoEntidade = TipoEntidade
        Me.Entidade = Entidade
        Me.Documento = documento
        Me.Moeda = moeda
        Me.Cambio = cambio
        Me.ValorTotal = valorTotal
        Me.ValorPendente = valorPendente
        Me.ValorActualizacao = ValorActualizacao

        Me.ValorTotalMT = valorTotal * cambio
        Me.ValorPendenteMT = valorPendente * cambio
    End Sub



End Class

Public Class Moedas

    Public Property Moeda As String
    Public Property Descricao As String
    Public Property Compra As Double
    Public Property Venda As Double
    Public Property Data As Date

    Public Sub New(ByRef moeda As String, ByRef descricao As String, ByRef compra As Double, ByRef venda As Double,
                   ByRef data As Object)
        Me.Moeda = moeda
        Me.Descricao = descricao
        Me.Compra = compra
        Me.Venda = venda
        Try
            Me.Data = data
        Catch ex As Exception

        End Try


    End Sub

End Class