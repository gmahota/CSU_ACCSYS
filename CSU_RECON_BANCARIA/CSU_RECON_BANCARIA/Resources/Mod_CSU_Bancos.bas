Attribute VB_Name = "Mod_CSU_Bancos"
Sub Mod_Importador_Reconsiliacoes()
    'Dim janela As CSU_RECON_CONECTOR_DLL.ConectorDll
    'Set janela = New CSU_RECON_CONECTOR_DLL.ConectorDll
    Dim janela As Object 'CSU_RECON_CONECTOR_DLL.ConectorDll
    Set janela = CreateObject("CSU_RECON_CONECTOR_DLL.ConectorDll")
    janela.inicializar 1, "", "", "", "Data Source=.\PRIMAVERAV810;Initial Catalog= PRIMINAS2015C;User Id= sa;Password=accsys2011!"
    
End Sub

Sub Mod_Importador_Magnetico()
    Dim janela As Object 'CSU_RECON_CONECTOR_DLL.ConectorDll
    Set janela = CreateObject("CSU_RECON_CONECTOR_DLL.ConectorDll")
    janela.inicializarImportadorFormatoMagnetico 1, "", "", "", "Data Source=.\PRIMAVERAV810;Initial Catalog= PRIMINAS2015C;User Id= sa;Password=accsys2011!"
    
End Sub

