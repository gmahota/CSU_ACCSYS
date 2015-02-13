Public Class Form1

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim t As New CSU_RECON_CONECTOR_DLL.ConectorDll
        t.inicializar(1, "CLONE", "accsys", "accsys2011", "Data Source=ACCPRI08\PRIMAVERAV810;Initial Catalog= PRICLONE;User Id= sa;Password=Accsys2011")
    End Sub
End Class
