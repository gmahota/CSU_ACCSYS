Imports MahApps.Metro.Controls
Imports System.Reflection

Partial Public Class MainWindow : Inherits MetroWindow
    Public xmlHelper As XmlHelper
    Public Const pastaConfig As String = "PRIMAVERA\\SG900"


    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        Inicializar()

        homeCrl.mainWindows = Me

        AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf CurrentDomain_AssemblyResolve
        ' DoTest()
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
        Const PRIMAVERA_COMMON_FILES_FOLDER As String = pastaConfig ' pasta dos ficheiros comuns especifica da versão do ERP PRIMAVERA utilizada.
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

        Const PRIMAVERA_COMMON_FILES_FOLDER As String = pastaConfig

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

    Public Sub Inicializar()
        'Dim motor As ErpBS
        'motor = New ErpBS()

        'motor.AbreEmpresaTrabalho(1, "clone", "accsys", "accsys2011")
        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim imp As New ImportadorExtratoBancario
        imp.InicializarPorXml()
        'imp.Inicializar(1, "clone", "accsys", "accsys2011", "Data Source=.\PRIMAVERAV810;Initial Catalog= PRIMINASREB;User Id= sa;Password=Accsys2011")

    End Sub

    Private Sub Button_Click2(sender As Object, e As RoutedEventArgs)
        'Dim imp As New InicializarView
        'Dim imp As New ImportadorExtratoBancario
        'imp.InicializarPorXml()
        'imp.Show()

        'Dim item As New MetroTabItem
        'item.Header = "Importador de Extrato Bancario"
        'item.CloseButtonEnabled = True
        'item.IsSelected = True

        'Dim importadorCrlExtratoBancario As New ImportExtBancoCrtl

        'importadorCrlExtratoBancario.Inicializar(xmlHelper.instancia.instancia, xmlHelper.instancia.empresa, xmlHelper.instancia.usuario,
        '                             xmlHelper.instancia.password, xmlHelper.instancia.daConnectionString())
        'item.Content = importadorCrlExtratoBancario

        'tbMain.Items.Add(item)
        importadoExtratoBancario()


    End Sub

    Private Sub Image_MouseDown(sender As Object, e As MouseButtonEventArgs)
        System.Diagnostics.Process.Start("http://www.accsys.co.mz")
    End Sub

    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        importadorFormatoMagnetico()
    End Sub

    Private Sub MetroTabControl_TabItemClosingEvent(sender As Object, e As BaseMetroTabControl.TabItemClosingEventArgs)
        If e.ClosingTabItem.Header.ToString().StartsWith("sizes") Then
            e.Cancel = True
        End If
    End Sub

    Private Sub HomeMenu_Click(sender As Object, e As RoutedEventArgs)
        janelaHome()
    End Sub

    Private Sub ExtratoMenu_Click(sender As Object, e As RoutedEventArgs)
        importadoExtratoBancario()
    End Sub

    Private Sub PagamentoBancoMenu_Click(sender As Object, e As RoutedEventArgs)
        importadorFormatoMagnetico()
    End Sub

    Private Sub MenuItem_Click(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub ParametrosMenu_Click(sender As Object, e As RoutedEventArgs)
        parametrosCtrl.IsOpen = Not parametrosCtrl.IsOpen
    End Sub

    Private Sub ImobilizadoMenu_Click(sender As Object, e As RoutedEventArgs)
        importadorImobilizado()
    End Sub

    Private Sub DiferencaCambioMenu_Click(sender As Object, e As RoutedEventArgs)
        diferencaCambio()
    End Sub

    Private Sub janelaHome()

        For Each janela As MetroTabItem In tbMain.Items
            If janela.Header = "Pagina Inicial" Then
                janela.IsSelected = True
                Exit Sub
            End If
        Next

        Dim item As New MetroTabItem
        item.Header = "Pagina Inicial"

        item.CloseButtonEnabled = True
        item.IsSelected = True

        Dim paginaInicial As New HomeCrtl

        item.Content = paginaInicial
        item.SetValue(ControlsHelper.HeaderFontSizeProperty, 18.0)
        tbMain.Items.Add(item)
    End Sub

    Private Sub importadorImobilizado()
        btSave.IsEnabled = True

        For Each janela As MetroTabItem In tbMain.Items
            If janela.Header = "Importador de Imobilizado" Then
                janela.IsSelected = True
                Exit Sub
            End If
        Next

        Dim item As New MetroTabItem
        item.Header = "Importador de Imobilizado"
        item.CloseButtonEnabled = True
        item.IsSelected = True
        xmlHelper = New XmlHelper
        Dim importadorCrlImobilizado As New ImobilizadoCrtl
        importadorCrlImobilizado.toolbar.Visibility = Windows.Visibility.Hidden
        importadorCrlImobilizado.Inicializar(xmlHelper.instancia.instancia, xmlHelper.instancia.empresa, xmlHelper.instancia.usuario,
                                     xmlHelper.instancia.password, xmlHelper.instancia.daConnectionString())
        item.Content = importadorCrlImobilizado
        item.SetValue(ControlsHelper.HeaderFontSizeProperty, 18.0)
        tbMain.Items.Add(item)
    End Sub

    Private Sub importadorFormatoMagnetico()

        For Each janela As MetroTabItem In tbMain.Items
            If janela.Header = "Importador para CSV" Then
                janela.IsSelected = True
                Exit Sub
            End If
        Next

        Dim item As New MetroTabItem
        item.Header = "Importador para CSV"
        item.CloseButtonEnabled = True
        item.IsSelected = True

        Dim importadorCrlFormatoMagnetico As New ImportFormatoMagneticoCtrl
        xmlHelper = New XmlHelper
        importadorCrlFormatoMagnetico.toolbar.Visibility = Windows.Visibility.Hidden
        importadorCrlFormatoMagnetico.Inicializar(xmlHelper.instancia.instancia, xmlHelper.instancia.empresa, xmlHelper.instancia.usuario,
                                     xmlHelper.instancia.password, xmlHelper.instancia.daConnectionString())
        item.Content = importadorCrlFormatoMagnetico
        item.SetValue(ControlsHelper.HeaderFontSizeProperty, 18.0)
        tbMain.Items.Add(item)
    End Sub

    Private Sub importadoExtratoBancario()

        btSave.IsEnabled = True

        For Each janela As MetroTabItem In tbMain.Items
            If janela.Header = "Importador de Extrato Bancario" Then
                janela.IsSelected = True
                Exit Sub
            End If
        Next

        Dim item As New MetroTabItem

        item.Header = "Importador de Extrato Bancario"
        item.CloseButtonEnabled = True
        item.IsSelected = True
        xmlHelper = New XmlHelper
        Dim importadorCrlExtratoBancario As New ImportExtBancoCrtl
        importadorCrlExtratoBancario.toolbar.Visibility = Windows.Visibility.Hidden

        importadorCrlExtratoBancario.Inicializar(xmlHelper.instancia.instancia, xmlHelper.instancia.empresa, xmlHelper.instancia.usuario,
                                     xmlHelper.instancia.password, xmlHelper.instancia.daConnectionString())
        item.Content = importadorCrlExtratoBancario

        
        item.SetValue(ControlsHelper.HeaderFontSizeProperty, 18.0)

        tbMain.Items.Add(item)
    End Sub

    Private Sub diferencaCambio()

        btSave.IsEnabled = True

        For Each janela As MetroTabItem In tbMain.Items
            If janela.Header = "Diferença de Cambios" Then
                janela.IsSelected = True
                Exit Sub
            End If
        Next

        Dim item As New MetroTabItem

        item.Header = "Diferença de Cambios"
        item.CloseButtonEnabled = True
        item.IsSelected = True

        Dim diferencaCambioCtrl As New DiferencaCambioCtrl
        'diferencaCambioCtrl.toolbar.Visibility = Windows.Visibility.Hidden
        xmlHelper = New XmlHelper
        diferencaCambioCtrl.Inicializar(xmlHelper.instancia.instancia, xmlHelper.instancia.empresa, xmlHelper.instancia.usuario,
                                     xmlHelper.instancia.password, xmlHelper.instancia.daConnectionString())

        item.Content = diferencaCambioCtrl

        item.SetValue(ControlsHelper.HeaderFontSizeProperty, 18.0)

        tbMain.Items.Add(item)
    End Sub

    Private Sub tbMain_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles tbMain.SelectionChanged
        Try
            Dim header = tbMain.Items(tbMain.SelectedIndex).Header
            Select Case header
                Case "Pagina Inicial"
                    btPrint.IsEnabled = False
                    btRemove.IsEnabled = False
                    btSave.IsEnabled = False
                    btupdate.IsEnabled = False
                Case "Importador de Extrato Bancario"
                    btPrint.IsEnabled = False
                    btRemove.IsEnabled = False
                    btSave.IsEnabled = True
                    btupdate.IsEnabled = False
                Case "Importador para CSV"
                    btPrint.IsEnabled = False
                    btRemove.IsEnabled = False
                    btSave.IsEnabled = True
                    btupdate.IsEnabled = True
                Case "Importador de Imobilizado"
                    btPrint.IsEnabled = False
                    btRemove.IsEnabled = False
                    btSave.IsEnabled = True
                    btupdate.IsEnabled = False
                Case "Diferença de Cambios"
                    btPrint.IsEnabled = False
                    btRemove.IsEnabled = False
                    btSave.IsEnabled = True
                    btupdate.IsEnabled = True
            End Select
        Catch ex As Exception

        End Try


        

    End Sub

    Private Sub btupdate_Click(sender As Object, e As RoutedEventArgs)
        update()
    End Sub

    Private Sub update()
        Try
            Dim header = tbMain.Items(tbMain.SelectedIndex).Header
            Select Case header
                Case "Pagina Inicial"
                    btPrint.IsEnabled = False
                    btRemove.IsEnabled = False
                    btSave.IsEnabled = False
                    btupdate.IsEnabled = False
                Case "Importador de Extrato Bancario"
                    
                Case "Importador para CSV"
                    Dim importadorFormatoMagnetico As New ImportFormatoMagneticoCtrl
                    importadorFormatoMagnetico = tbMain.Items(tbMain.SelectedIndex).Content
                    importadorFormatoMagnetico.update()
                Case "Importador de Imobilizado"
                    
                Case "Diferença de Cambios"

            End Select
        Catch ex As Exception

        End Try
    End Sub
    Private Sub save()
        Try
            Dim header = tbMain.Items(tbMain.SelectedIndex).Header
            Select Case header
                Case "Pagina Inicial"
                    btPrint.IsEnabled = False
                    btRemove.IsEnabled = False
                    btSave.IsEnabled = False
                    btupdate.IsEnabled = False
                Case "Importador de Extrato Bancario"
                    Dim importadorCrlExtratoBancario As New ImportExtBancoCrtl
                    importadorCrlExtratoBancario = tbMain.Items(tbMain.SelectedIndex).Content
                    importadorCrlExtratoBancario.save()
                Case "Importador para CSV"
                    Dim importadorFormatoMagnetico As New ImportFormatoMagneticoCtrl
                    importadorFormatoMagnetico = tbMain.Items(tbMain.SelectedIndex).Content
                    importadorFormatoMagnetico.save()
                Case "Importador de Imobilizado"
                    Dim imobilizadoCtrl As New ImobilizadoCrtl
                    imobilizadoCtrl = tbMain.Items(tbMain.SelectedIndex).Content
                    imobilizadoCtrl.save()
                Case "Diferença de Cambios"
                    Dim diferencaCtrl As New DiferencaCambioCtrl
                    diferencaCtrl = tbMain.Items(tbMain.SelectedIndex).Content
                    diferencaCtrl.save()
            End Select
        Catch ex As Exception

        End Try
    End Sub
    Private Sub btSave_Click(sender As Object, e As RoutedEventArgs)
        save()
    End Sub

    Private Sub btRemove_Click(sender As Object, e As RoutedEventArgs)
        Try
            Dim header = tbMain.Items(tbMain.SelectedIndex).Header
            Select Case header
                Case "Pagina Inicial"
                    btPrint.IsEnabled = False
                    btRemove.IsEnabled = False
                    btSave.IsEnabled = False
                    btupdate.IsEnabled = False
                Case "Importador de Extrato Bancario"
                    btPrint.IsEnabled = False
                    btRemove.IsEnabled = False
                    btSave.IsEnabled = True
                    btupdate.IsEnabled = False
                Case "Importador para CSV"
                    btPrint.IsEnabled = False
                    btRemove.IsEnabled = False
                    btSave.IsEnabled = True
                    btupdate.IsEnabled = False
                Case "Importador de Imobilizado"
                    btPrint.IsEnabled = False
                    btRemove.IsEnabled = False
                    btSave.IsEnabled = True
                    btupdate.IsEnabled = False
            End Select
        Catch ex As Exception

        End Try
    End Sub

    Private Sub btPrint_Click(sender As Object, e As RoutedEventArgs)
        Try
            Dim header = tbMain.Items(tbMain.SelectedIndex).Header
            Select Case header
                Case "Pagina Inicial"
                    btPrint.IsEnabled = False
                    btRemove.IsEnabled = False
                    btSave.IsEnabled = False
                    btupdate.IsEnabled = False
                Case "Importador de Extrato Bancario"
                    btPrint.IsEnabled = False
                    btRemove.IsEnabled = False
                    btSave.IsEnabled = True
                    btupdate.IsEnabled = False
                Case "Importador para CSV"
                    btPrint.IsEnabled = False
                    btRemove.IsEnabled = False
                    btSave.IsEnabled = True
                    btupdate.IsEnabled = False
                Case "Importador de Imobilizado"
                    btPrint.IsEnabled = False
                    btRemove.IsEnabled = False
                    btSave.IsEnabled = True
                    btupdate.IsEnabled = False
            End Select
        Catch ex As Exception

        End Try
    End Sub

    
    Private Sub btProperties_Click(sender As Object, e As RoutedEventArgs)
        parametrosCtrl.IsOpen = Not parametrosCtrl.IsOpen
    End Sub

    Private Sub TextBlock_MouseLeave(sender As Object, e As MouseEventArgs)
        parametrosCtrl.IsOpen = Not parametrosCtrl.IsOpen
    End Sub

    
End Class
