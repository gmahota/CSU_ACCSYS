
<ComClass(ConectorDll.ClassId, ConectorDll.InterfaceId, ConectorDll.EventsId)>
Public Class ConectorDll

    Public Const ClassId As String = "0DAD61F3-B0B8-4642-A49D-3B37D04BD856"
    Public Const InterfaceId As String = "701B0744-9452-430E-A074-03499961AAD8"
    Public Const EventsId As String = "2FE80C14-6B58-4DAD-AB52-D04ED0646E15"


    Public Sub inicializar(tipoPlataforma As Integer, codEmpresa As String, codUsuario As String, password As String, connection As String)
        Dim janela As CSU_RECON_BANCARIA.ImportadorExtratoBancario
        janela = New CSU_RECON_BANCARIA.ImportadorExtratoBancario
        janela.Inicializar(tipoPlataforma, codEmpresa, codUsuario, password, connection)
    End Sub

    Public Sub New()

    End Sub

    Public Sub inicializarImportadorFormatoMagnetico(tipoPlataforma As Integer, codEmpresa As String, codUsuario As String, password As String, connection As String)
        Dim janela As CSU_RECON_BANCARIA.ImportadorFormatoMagneticoView
        janela = New CSU_RECON_BANCARIA.ImportadorFormatoMagneticoView
        janela.Inicializar(tipoPlataforma, codEmpresa, codUsuario, password, connection)
    End Sub
End Class
