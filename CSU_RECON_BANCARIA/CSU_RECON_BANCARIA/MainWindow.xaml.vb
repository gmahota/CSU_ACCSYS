Imports MahApps.Metro.Controls
Partial Public Class MainWindow : Inherits MetroWindow
    Public xmlHelper As XmlHelper

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        Inicializar()

        xmlHelper = New XmlHelper
        homeCrl.mainWindows = Me
    End Sub

    Public Sub Inicializar()
        'Dim motor As ErpBS
        'motor = New ErpBS()

        'motor.AbreEmpresaTrabalho(1, "clone", "accsys", "accsys2011")
        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub importadorFormatoMagnetico()
        'Dim imp As New ImportadorFormatoMagneticoView
        'imp.InicializarPorXml()
        'imp.Show()

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

        importadorCrlFormatoMagnetico.Inicializar(xmlHelper.instancia.instancia, xmlHelper.instancia.empresa, xmlHelper.instancia.usuario,
                                     xmlHelper.instancia.password, xmlHelper.instancia.daConnectionString())
        item.Content = importadorCrlFormatoMagnetico

        tbMain.Items.Add(item)
    End Sub

    Private Sub importadoExtratoBancario()

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

        Dim importadorCrlExtratoBancario As New ImportExtBancoCrtl

        importadorCrlExtratoBancario.Inicializar(xmlHelper.instancia.instancia, xmlHelper.instancia.empresa, xmlHelper.instancia.usuario,
                                     xmlHelper.instancia.password, xmlHelper.instancia.daConnectionString())
        item.Content = importadorCrlExtratoBancario

        tbMain.Items.Add(item)
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

        tbMain.Items.Add(item)
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
End Class
