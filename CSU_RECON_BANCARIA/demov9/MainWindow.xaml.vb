Imports System.Reflection
Imports Interop.ErpBS900
Imports Interop.GcpBE900


Class MainWindow

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        On Error GoTo trataerro

        'Dim objmotor As Object = System.Activator.CreateInstance(System.Type.GetTypeFromProgID("ErpBS900.ErpBs"))
        Dim objmotor As Interop.ErpBS900.ErpBS = New Interop.ErpBS900.ErpBS
        Dim palavra As String
        'objmotor = CreateObject("ErpBS900.ErpBs")

        objmotor.AbreEmpresaTrabalho(1, "Acc", "Accsys", "Accsys2011")
        palavra = objmotor.Comercial.Clientes.DaNome("1001")
        MessageBox.Show(palavra)

        Exit Sub
trataerro:
        MsgBox(Err.Description)
    End Sub

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf CurrentDomain_AssemblyResolve
        DoTest()
        ' Add any initialization after the InitializeComponent() call.
        'Este handler tem que ser adicionado antes de existir qualquer referência para classes existentes nos Interop's,
        'isto é, no método Main() da aplicação NÃO PODERÁ EXISTIR DECLARAÇÕES DE VARIÁVEIS DE TIPOS EXISTENTES NOS INTEROPS.
        'Com este método, na pasta da aplicação não deverão existir os Interops e as referências para os mesmos deverão ser
        'adicionadas com Copy Local = False e Specific Version = false.

        'Chamar o método de teste.
        '' DoTest()
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
        Const PRIMAVERA_COMMON_FILES_FOLDER As String = "PRIMAVERA\\S900" ' pasta dos ficheiros comuns especifica da versão do ERP PRIMAVERA utilizada.
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

        Const PRIMAVERA_COMMON_FILES_FOLDER As String = "PRIMAVERA\\SG900"

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

    ''' <summary>
    ''' Testar os motores do ERP.
    ''' Ao iniciar a execução deste método são carregados os Interops e consequentemente é chamado o evento para resolução das assemblies.
    ''' </summary>
    Public Sub DoTest()
        Dim motor As ErpBS
        Dim docVenda As GcpBEDocumentoVenda
        motor = New ErpBS()
        Try
            'abre a empresa
            motor.AbreEmpresaTrabalho(Interop.StdBE900.EnumTipoPlataforma.tpProfissional, "Acc", "Accsys", "Accsys2011")
            'cria o documento de venda
            docVenda = New Interop.GcpBE900.GcpBEDocumentoVenda()
            docVenda.Serie = "2015"
            docVenda.Tipodoc = "FA"
            docVenda.TipoEntidade = "C"
            docVenda.Entidade = "1001"
            Try
                'preenche os dados relacionados do cabeçalho
                docVenda = motor.Comercial.Vendas.PreencheDadosRelacionados(docVenda)
                '....adicionar linhas...
                motor.Comercial.Vendas.AdicionaLinha(docVenda, "211.0001", 1, , , 100)
                'gravar o documento.
                motor.Comercial.Vendas.Actualiza(docVenda)
            Catch ex As Exception
                MsgBox(ex.Message)
            Finally
                motor.FechaEmpresaTrabalho()
            End Try
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
End Class
