Imports Interop.StdBE800
Imports Interop.ErpBS800
Imports Interop.GcpBE800
Imports Interop.ICrmBS800
Imports Interop.CrmBE800
Imports System.Data.SqlClient
Imports System.Data
Imports System.Net.Mail


Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports System.IO

Public Class ClientesHelper
    Public tipoPlataforma As Integer = 0
    Public codEmpresa As String = "RECNOVA"
    Public codUsuario As String = "accsys"
    Public password As String = "Accsys2011"
    Public objmotor As ErpBS = New ErpBS

    'Dim cryRpt As New ReportDocument
    Dim pdfFile As String = "c:\Avisos\Av100012014.pdf"
    Dim objReport As New ReportDocument
    Dim paraValue As New ParameterDiscreteValue
    Dim currValue As ParameterValues

    'Declare the string variable 'connectionString' to hold the ConnectionString        
    Dim connectionString As String = "Data Source=ACCPRI08\PRIMAVERAV810;Initial Catalog= PRIRECNOVA;User Id= sa;Password=Accsys2011"
    Public instancia As String = "ACCPRI08\PRIMAVERAV810"
    Public empresaInstancia As String = "PRIRECNOVA" '"PRIEMPRE"
    Public utilizadorInstancia As String = "sa"
    Public passwordInstancia As String = "Accsys2011"


    'Conexao DB
    Dim myConnection As SqlConnection
    Dim myCommand As SqlCommand
    Dim myAdapter As SqlDataAdapter

    Public Sub actualizarconexao()
        Try
            'empresaInstancia = empresa
            connectionString = "Data Source=" + instancia + ";Initial Catalog= " + empresaInstancia + _
                ";User Id=" + utilizadorInstancia + ";Password=" + passwordInstancia

            incializarMotorPrimavera(tipoPlataforma, codEmpresa, codUsuario, password)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Aviso", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
        
    End Sub

    Public Sub actualizarconexao(NomeEmpresa As String)

        empresaInstancia = "PRIEMPRE"
        actualizarconexao()

        Dim ds As New DataSet

        Try
            'connectionString = connection

            myConnection = New SqlConnection(connectionString)

            'Declare the query
            Dim str_query As String

            str_query = "select * from [PRIEMPRE].[dbo].empresas where IDNome ='" & NomeEmpresa & "'"

            myCommand = New SqlCommand(str_query, myConnection)
            myConnection.Open()
            myAdapter = New SqlDataAdapter(myCommand)
            myAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
            myAdapter.Fill(ds, "Empresas")

            For Each emp As DataRow In ds.Tables(0).Rows
                empresaInstancia = "PRI" + emp("codigo").ToString()
                codEmpresa = emp("codigo").ToString()
            Next

            myConnection.Close()

            actualizarconexao()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Aviso", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
        

    End Sub

    ''' <summary>
    ''' Metodo para inicializar o motor do primavera
    ''' </summary>
    ''' <param name="tipoPlataforma"> 0 - Executiva, 1- Profissional</param>
    ''' <param name="codEmpresa"></param>
    ''' <param name="codUsuario"></param>
    ''' <param name="password"></param>
    ''' <remarks></remarks>
    Public Sub incializarMotorPrimavera(tipoPlataforma As Integer, codEmpresa As String, codUsuario As String, password As String)

        Try
            Me.tipoPlataforma = tipoPlataforma
            Me.codUsuario = codUsuario
            Me.codEmpresa = codEmpresa
            Me.password = password

            objmotor = New ErpBS

            objmotor.AbreEmpresaTrabalho(tipoPlataforma, codEmpresa, codUsuario, password)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Aviso", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try

        

    End Sub

    Public Sub incializarMotorPrimavera()
        Try
            If objmotor.Contexto.EmpresaAberta = True Then objmotor.FechaEmpresaTrabalho()

            objmotor.AbreEmpresaTrabalho(tipoPlataforma, codEmpresa, codUsuario, password)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Aviso", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
        

    End Sub

    Public Function listaClientesComPendentes() As ClientesDataSet

        Dim ds As New ClientesDataSet

        Try
            myConnection = New SqlConnection(connectionString)

            'Declare the query
            'Dim str_query As String = "select * from clientes where cliente in (select distinct (Entidade) from pendentes where tipoentidade = 'C')"
            Dim str_query As String = "select c.Cliente, c.Nome, c.Fac_Mor, c.Fac_Local,c.NumContrib,c.Pais,c.Fac_Tel,c.Moeda, " & _
            "c.CDU_ContaRec,c.CDU_EnviaCobranca,cont.PrimeiroNome, cont.UltimoNome, cont.Titulo ,cont.Email,cont.EmailAssist " & _
            "from clientes c   inner join LinhasContactoEntidades lce on lce.Entidade = c.Cliente and lce.TipoEntidade = 'C' " & _
            "inner join Contactos cont on cont.Id = lce.IDContacto " & _
            "where cliente in (select distinct (Entidade) from pendentes where tipoentidade = 'C')"

            myCommand = New SqlCommand(str_query, myConnection)
            myConnection.Open()
            myAdapter = New SqlDataAdapter(myCommand)
            myAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
            myAdapter.Fill(ds, "Clientes")

            myConnection.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Aviso", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try

        Return ds

    End Function

    Public Function listaPendentes(cliente As String) As ClientesDataSet
        Dim ds As New ClientesDataSet

        Try
            myConnection = New SqlConnection(connectionString)

            'Declare the query
            Dim str_query As String = "Select * from pendentes where tipoentidade = 'C' and entidade = '" & cliente & "' "


            myCommand = New SqlCommand(str_query, myConnection)
            myConnection.Open()
            myAdapter = New SqlDataAdapter(myCommand)
            myAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
            myAdapter.Fill(ds, "Pendentes")

            str_query = "Select Cliente,Nome,Fac_Mor,Fac_Local,NumContrib,Pais,Fac_Tel,Moeda," + _
                "ISNULL(Cdu_ContaRec,'CDO01') as Cdu_ContaRec from clientes"

            str_query = str_query + " where cliente = '" & cliente & "'"

            myCommand = New SqlCommand(str_query, myConnection)
            'myConnection.Open()
            myAdapter = New SqlDataAdapter(myCommand)
            myAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
            myAdapter.Fill(ds, "Clientes")

            myConnection.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Aviso", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try

        Return ds

    End Function

    Public Function listaBancos() As ClientesDataSet

        Dim ds As New ClientesDataSet

        Try
            myConnection = New SqlConnection(connectionString)

            'Declare the query
            Dim str_query As String = "select cb.tipoconta, b.Banco,b.Descricao, cb.numconta,cb.nib,cb.IBAN,cb.SWIFT,cb.Moeda " & _
                "from contasbancarias cb " & _
                "inner join bancos b on cb.Banco = b.Banco " & _
                "where cb.TipoConta = 0 and cb.cdu_tipoconta='CB'  and cb.CDU_NumeroOrdem is not null order by cb.CDU_NumeroOrdem "


            myCommand = New SqlCommand(str_query, myConnection)
            myConnection.Open()
            myAdapter = New SqlDataAdapter(myCommand)
            myAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey

            ds.Tables("Banco").Clear()
            ds.EnforceConstraints = False
            myAdapter.Fill(ds, "Banco")

            myConnection.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Aviso", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try

        Return ds

    End Function


    Public Function listaTotalPendentes(cliente As String) As DataSet
        Dim ds As New DataSet
        Try
            'connectionString = connection
            myConnection = New SqlConnection(connectionString)

            'Declare the query
            Dim str_query As String ' = "Select * from pendentes where entidade = '" & cliente & "'"

            'str_query = "SELECT dbo.Clientes.Cliente, dbo.Clientes.Nome, dbo.Pendentes.ValorPendente FROM dbo.Pendentes INNER JOIN dbo.Clientes ON dbo.Pendentes.Entidade = dbo.Clientes.Cliente where entidade = '" & cliente & "'"

            str_query = "SELECT Clientes.Cliente, Clientes.Nome, " + _
                "SUM(Pendentes.ValorPendente) AS ValorPendente, " + _
                "COUNT(Pendentes.ValorPendente) AS Quantidade " + _
                "FROM Pendentes INNER JOIN dbo.Clientes ON Pendentes.Entidade = Clientes.Cliente " + _
                "GROUP BY Clientes.Cliente, Clientes.Nome " + _
                "HAVING (Clientes.Cliente = N'" & cliente & "')"

            myCommand = New SqlCommand(str_query, myConnection)
            myConnection.Open()
            myAdapter = New SqlDataAdapter(myCommand)
            myAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
            myAdapter.Fill(ds, "Pendentes")

            myConnection.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Aviso", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try

        Return ds

    End Function


    Public Function listaEmpresas() As ClientesDataSet
        
        Dim ds As New ClientesDataSet

        Try
            
            myConnection = New SqlConnection(connectionString)

            'Declare the query
            Dim str_query As String

            str_query = "select * from [PRIEMPRE].[dbo].empresas where Categoria in (select Categoria from [PRIEMPRE].[dbo].categoriasempresas where descricao = 'GRUPO MERIDIAN')"

            myCommand = New SqlCommand(str_query, myConnection)
            myConnection.Open()
            myAdapter = New SqlDataAdapter(myCommand)
            myAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
            myAdapter.Fill(ds, "Empresas")

            myConnection.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Aviso", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try

        Return ds

    End Function

    Public Sub PrintToPdfWithStream(ByVal MyReport As CrystalDecisions.CrystalReports.Engine.ReportDocument)
        Try
            Dim MyExportOptions As New CrystalDecisions.Shared.ExportOptions
            MyExportOptions.ExportFormatType = CrystalDecisions.[Shared].ExportFormatType.PortableDocFormat
            Dim MyExportRequestContext As New CrystalDecisions.Shared.ExportRequestContext
            MyExportRequestContext.ExportInfo = MyExportOptions
            Dim MyStream As System.IO.Stream
            MyStream = MyReport.FormatEngine.ExportToStream(MyExportRequestContext)

            Dim MyBuffer(MyStream.Length) As Byte
            MyStream.Read(MyBuffer, 0, CType(MyStream.Length, Integer))
            Dim sr1 = New FileStream(pdfFile, FileMode.OpenOrCreate)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Aviso", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try

    End Sub


    Public Sub imprimirPdf(dt As ClientesDataSet, objectoContacto As CrmBEContacto)
        Try
            Dim CrExportOptions As ExportOptions
            Dim CrDiskFileDestinationOptions As New DiskFileDestinationOptions()
            Dim CrFormatTypeOptions As New PdfRtfWordFormatOptions()

            'object of table Log on info of Crystal report

            objReport.Load("Reports\ExtratoPendentes.rpt")


            objReport.Database.Tables("Pendentes").SetDataSource(dt.Tables("Pendentes"))
            objReport.Database.Tables("Clientes").SetDataSource(dt.Tables("Clientes"))

            objReport.Subreports("ContasBancarias").SetDataSource(listaBancos().Tables("Banco"))

            objReport.OpenSubreport("Pendentes").SetDataSource(dt.Tables("Pendentes"))
            objReport.DataDefinition.FormulaFields("NomeEmpresa").Text = "'" & objmotor.Contexto.IDNome & "'"
            objReport.DataDefinition.FormulaFields("MoradaEmpresa").Text = "'" & objmotor.Contexto.IDMorada & "'"
            objReport.DataDefinition.FormulaFields("LocalidadeEmpresa").Text = "'" & objmotor.Contexto.IDLocalidade & "'"
            objReport.DataDefinition.FormulaFields("TelefoneEmpresa").Text = "'+ " & objmotor.Contexto.IDIndicativoTelefone & objmotor.Contexto.IDTelefone & "'"
            objReport.DataDefinition.FormulaFields("NuitEmpresa").Text = "' Nuit : " & objmotor.Contexto.IFNIF & "'"
            objReport.DataDefinition.FormulaFields("EmailEmpresa").Text = "'Email: cmelo@accsys.co.mz'"
            objReport.DataDefinition.FormulaFields("Ao_Cuidado_de").Text = "' " & objectoContacto.Titulo & " " & objectoContacto.Nome & "'"
            'objReport.DataDefinition.FormulaFields("EmailEmpresa").Text = "' Email: " & objmotor.Contexto.IDEmail & "'"

            Dim banco, descricaoBanco, conta, iban, swift As String
            Dim dr As DataRow
            dr = dt.Tables("Clientes").Rows(0)

            banco = objmotor.Comercial.ContasBancarias.DaValorAtributo(dr("CDU_ContaRec").ToString(), "Banco")
            descricaoBanco = objmotor.Comercial.Bancos.DaValorAtributo(banco, "Descricao")
            conta = objmotor.Comercial.ContasBancarias.DaValorAtributo(dr("CDU_ContaRec").ToString(), "NumConta")
            iban = objmotor.Comercial.ContasBancarias.DaValorAtributo(dr("CDU_ContaRec").ToString(), "IBAN")
            swift = objmotor.Comercial.ContasBancarias.DaValorAtributo(dr("CDU_ContaRec").ToString(), "Swift")

            objReport.DataDefinition.FormulaFields("BancoCliente").Text = "'" & descricaoBanco & "'"
            objReport.DataDefinition.FormulaFields("ContaCliente").Text = "'" & conta & "'"
            objReport.DataDefinition.FormulaFields("NibCliente").Text = "'" & objmotor.Contexto.IDEmail & "'"
            objReport.DataDefinition.FormulaFields("IbanCliente").Text = "'" & objmotor.Contexto.IDEmail & "'"
            objReport.DataDefinition.FormulaFields("SwiftCliente").Text = "'" & objmotor.Contexto.IDEmail & "'"

            objReport.Refresh()

            CrDiskFileDestinationOptions.DiskFileName = pdfFile
            objReport.ExportOptions.DestinationOptions = CrDiskFileDestinationOptions

            objReport.ExportToDisk(ExportFormatType.PortableDocFormat, pdfFile)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Aviso", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try

    End Sub


    Public Sub enviaEmail(codigoCliente As String, ByRef anexo1 As String, ByRef anexo2 As String)

        Try
            Dim ds As New DataSet()
            Dim filename, mailboy As String
            Dim objContacto As CrmBEContacto
            Dim objContactos As CrmBEContactos
            Dim enviado As Boolean
            enviado = False


            pdfFile = "c:\Avisos\Extrato." + codigoCliente + "." + Today().ToString("ddMMyyyy") + ".pdf"

            objContactos = objmotor.CRM.Contactos.ListaContactosDaEntidade("C", codigoCliente)

            If (objContactos.NumItens > 0) Then


                For Each objContacto In objContactos
                    imprimirPdf(listaPendentes(codigoCliente), objContacto)
                    ds = listaTotalPendentes(codigoCliente)

                    If enviado = False Then
                        filename = "Reports/template.htm"
                        mailboy = System.IO.File.ReadAllText(filename)

                        mailboy = mailboy.Replace("##FirstName##", objContacto.Titulo + " " + objContacto.Nome)
                        mailboy = mailboy.Replace("##cliente##", ds.Tables(0).Rows(0)("Nome").ToString())
                        mailboy = mailboy.Replace("##quantidade##", ds.Tables(0).Rows(0)("Quantidade").ToString())
                        mailboy = mailboy.Replace("##divida##", ds.Tables(0).Rows(0)("ValorPendente").ToString())
                        mailboy = mailboy.Replace("##empresa##", objmotor.Contexto.IDNome)

                        Try
                            Dim Smtp_Server As New SmtpClient
                            Dim e_mail As New MailMessage()
                            Smtp_Server.UseDefaultCredentials = False

                            Smtp_Server.Credentials = New Net.NetworkCredential("avisos@meridian32.com", "")
                            Smtp_Server.Port = 25
                            Smtp_Server.EnableSsl = False
                            Smtp_Server.Host = "192.168.3.14"



                            e_mail = New MailMessage()
                            e_mail.From = New MailAddress("avisos@meridian32.com")
                            e_mail.To.Add(objContacto.Email)
                            e_mail.CC.Add("cmelo@accsys.co.mz")
                            
                            e_mail.Subject = "Facturas Pendentes " + objmotor.Contexto.IDNome

                            e_mail.IsBodyHtml = True 'false
                            'e_mail.Body = "Teste" + teste1.Tables(0).DataSet.GetXml
                            e_mail.Body = mailboy
                            e_mail.Attachments.Add(New Net.Mail.Attachment(pdfFile))

                            If anexo1 <> "" Then e_mail.Attachments.Add(New Net.Mail.Attachment(anexo1))
                            If anexo2 <> "" Then e_mail.Attachments.Add(New Net.Mail.Attachment(anexo2))

                            Smtp_Server.Send(e_mail)

                            enviado = True
                        Catch error_t As Exception
                            MsgBox(error_t.ToString)
                            enviado = False
                        End Try
                    End If

                Next

            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Aviso", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try

        

    End Sub

    Sub enviaEmails(codigo As String, ByRef anexo1 As String, ByRef anexo2 As String)

        enviaEmail(codigo, anexo1, anexo2)

    End Sub

    Public Sub New()

    End Sub
End Class
