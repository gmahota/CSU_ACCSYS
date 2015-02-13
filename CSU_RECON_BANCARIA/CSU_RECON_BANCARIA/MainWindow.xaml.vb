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
        imp.Inicializar(1, "clone", "accsys", "accsys2011", "Data Source=ACCPRI08\PRIMAVERAV810;Initial Catalog= PRICLONE;User Id= sa;Password=Accsys2011")

    End Sub

    Private Sub Button_Click2(sender As Object, e As RoutedEventArgs)
        Dim imp As New InicializarView
        imp.show()

    End Sub
End Class
