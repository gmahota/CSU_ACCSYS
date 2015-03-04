Imports MahApps.Metro.Controls
Partial Public Class MainWindow : Inherits MetroWindow
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        Inicializar()
    End Sub

    Public Sub Inicializar()
        'Dim motor As ErpBS
        'motor = New ErpBS()

        'motor.AbreEmpresaTrabalho(1, "clone", "accsys", "accsys2011")
        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim imp As New ImportadorExtratoBancario
        'imp.Inicializar(1, "clone", "accsys", "accsys2011", "Data Source=.\PRIMAVERAV810;Initial Catalog= PRIMINASREB;User Id= sa;Password=Accsys2011")

    End Sub

    Private Sub Button_Click2(sender As Object, e As RoutedEventArgs)
        'Dim imp As New InicializarView
        Dim imp As New ImportadorExtratoBancario
        imp.Show()

    End Sub

    Private Sub Image_MouseDown(sender As Object, e As MouseButtonEventArgs)
        System.Diagnostics.Process.Start("http://www.accsys.co.mz")
    End Sub
End Class
