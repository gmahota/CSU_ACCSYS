Imports erpBS900 = Interop.ErpBS900
Imports gcpBE900 = Interop.GcpBE900

Imports erpBS800 = Interop.ErpBS800
Imports gcpBE800 = Interop.GcpBE800

Imports System.Data.SqlClient
Imports System.Data
Imports System.Net.Mail


Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports System.IO


Imports System.Linq

Imports System.Web

Namespace CSU_CRM_WEB.Models.Helper
    Public Class EmailHelper

        Private empresa As String
       

        Private objReport As ReportDocument
        Private paraValue As ParameterDiscreteValue
        Private currValue As ParameterValues

        Public Sub New(empresa As String)
            db = New PRIACCEntities()
            db.Database.Connection.Open()
            db.Database.Connection.ChangeDatabase(Convert.ToString("pri") & empresa)

            Me.empresa = empresa
            dbpriempre = New PRIEMPREEntities()
            empresadb = dbpriempre.View_Empresas.Where(Function(p) p.Codigo = empresa).First()

            objReport = New ReportDocument()
            paraValue = New ParameterDiscreteValue()
            currValue = New ParameterValues()
        End Sub

        Private pdfFile As String = "c:/Avisos/Av100012014.pdf"

        Public Sub enviaEmail(codigoCliente As String, files As IEnumerable(Of HttpPostedFileBase))
            Try

                Dim ds As New DataSet()
                Dim filename As String = Nothing
                Dim mailboy As String = Nothing

                Dim enviado As Boolean = False
                enviado = False


                pdfFile = (Convert.ToString("c:\Avisos\Extrato.") & codigoCliente) + "." + DateTime.Now.ToString("ddMMyyyy") + ".pdf"

                'objContactos = objmotor.CRM.Contactos.ListaContactosDaEntidade("C", codigoCliente);
                Dim listFacturasPendentes = db.View_Lista_Contactos_Pendentes.Where(Function(p) p.Cliente = codigoCliente).ToList()
                Dim objContacto As View_Lista_Contactos_Pendentes = listFacturasPendentes.First()

                'imprimirPdf(codigoCliente);

                Try
                    filename = System.Web.HttpContext.Current.Server.MapPath("~/Content/Reports/template.htm")
                    ' "~/Content/Reports/template.htm";
                    mailboy = System.IO.File.ReadAllText(filename)
                    mailboy = mailboy.Replace("##FirstName##", objContacto.Titulo + " " + objContacto.Nome)
                    mailboy = mailboy.Replace("##cliente##", objContacto.Nome)
                    'mailboy = mailboy.Replace("##quantidade##", ds.Tables[0].Rows[0]["Quantidade"].ToString());
                    mailboy = mailboy.Replace("##divida##", objContacto.ValorPendente.ToString())
                    mailboy = mailboy.Replace("##empresa##", empresadb.IDNome)

                    Dim Smtp_Server As New SmtpClient()
                    Dim e_mail As New MailMessage()

                    'Smtp_Server.UseDefaultCredentials = true;
                    'Smtp_Server.Credentials = new System.Net.NetworkCredential("gmahota@accsys.co.mz", "Accsys2011!");
                    'Smtp_Server.Port = 587;
                    'Smtp_Server.EnableSsl = true;
                    'Smtp_Server.Host = "smtp.gmail.com";



                    Smtp_Server.UseDefaultCredentials = False
                    Smtp_Server.Credentials = New System.Net.NetworkCredential("avisos@accsys.co.mz", "")
                    Smtp_Server.Port = 25
                    Smtp_Server.EnableSsl = False
                    Smtp_Server.Host = "192.168.3.14"

                    e_mail = New MailMessage()
                    e_mail.From = New MailAddress("avisos@accsys.co.mz")
                    'e_mail.To.Add("gmahota@accsys.co.mz");
                    e_mail.[To].Add(objContacto.Email)
                    e_mail.CC.Add("cmelo@accsys.co.mz")

                    e_mail.Subject = "Facturas Pendentes " + empresadb.IDNome

                    e_mail.IsBodyHtml = True

                    e_mail.Body = mailboy
                    e_mail.Attachments.Add(New System.Net.Mail.Attachment(imprimirPdf(objContacto.Cliente), "Extrato Pendentes.pdf"))

                    If files IsNot Nothing Then
                        For Each file As var In files
                            e_mail.Attachments.Add(New System.Net.Mail.Attachment(file.InputStream, Path.GetFileName(file.FileName)))
                        Next
                    End If




                    'if (!string.IsNullOrEmpty(anexo1))
                    '    e_mail.Attachments.Add(new System.Net.Mail.Attachment(anexo1));
                    'if (!string.IsNullOrEmpty(anexo2))
                    '    e_mail.Attachments.Add(new System.Net.Mail.Attachment(anexo2));

                    Smtp_Server.Send(e_mail)

                    enviado = True
                Catch error_t As Exception
                    'Interaction.MsgBox(error_t.ToString());
                    enviado = False
                End Try
            Catch ex As Exception
            End Try
        End Sub

        Private Function imprimirPdf(codigoCliente As String) As Stream
            Try
                Dim view_Cliente = db.View_Lista_Contactos_Pendentes.Where(Function(p) p.Cliente = codigoCliente).First()

                Dim dataSet As New ClientesDataSet()

                Dim row As DataRow = dataSet.Tables("Clientes").NewRow()
                row("Cliente") = view_Cliente.Cliente
                row("Nome") = view_Cliente.Nome
                row("Fac_Mor") = view_Cliente.Fac_Mor
                row("Fac_Local") = view_Cliente.Fac_Local
                row("Fac_Tel") = view_Cliente.Fac_Tel
                row("NumContrib") = view_Cliente.NumContrib
                row("Pais") = view_Cliente.Pais
                row("Moeda") = view_Cliente.Moeda
                row("PrimeiroNome") = view_Cliente.PrimeiroNome
                row("UltimoNome") = view_Cliente.UltimoNome
                row("Titulo") = view_Cliente.Titulo
                row("Email") = view_Cliente.Email
                row("EmailAssist") = view_Cliente.EmailAssist

                dataSet.Tables("Clientes").Rows.Add(row)

                Dim bancos = db.View_Bancos_Cobrancas.ToList()

                For Each banco As var In bancos
                    row = dataSet.Tables("Banco").NewRow()
                    row("Banco") = banco.Banco
                    row("Descricao") = banco.Descricao
                    row("NumConta") = banco.numconta
                    row("Nib") = banco.nib
                    row("Iban") = banco.IBAN
                    row("Swift") = banco.SWIFT
                    row("Moeda") = banco.Moeda

                    dataSet.Tables("Banco").Rows.Add(row)
                Next

                Dim pendentes = db.View_Pendentes_Doc_Clientes

                For Each pendente As var In pendentes
                    row = dataSet.Tables("Pendentes").NewRow()

                    row("TipoDoc") = pendente.TipoDoc
                    row("Serie") = pendente.Serie
                    row("TipoEntidade") = pendente.TipoEntidade
                    row("Entidade") = pendente.Entidade
                    row("DataDoc") = pendente.DataDoc
                    row("DataVenc") = pendente.DataVenc

                    row("NumDoc") = pendente.NumDoc
                    row("NumDocInt") = pendente.NumDocInt
                    row("ValorTotal") = pendente.ValorTotal
                    row("ValorPendente") = pendente.ValorPendente

                    row("NumDoc") = pendente.Moeda
                    row("NumDocInt") = pendente.Cambio

                    dataSet.Tables("Pendentes").Rows.Add(row)
                Next

                Return imprimirPdf(dataSet, view_Cliente)
            Catch
                Return Nothing
            End Try

        End Function

        Private Function imprimirPdf(dt As ClientesDataSet, objectoContacto As View_Lista_Contactos_Pendentes) As Stream
            Try
                'ExportOptions CrExportOptions = default(ExportOptions);
                'DiskFileDestinationOptions CrDiskFileDestinationOptions = new DiskFileDestinationOptions();
                'PdfRtfWordFormatOptions CrFormatTypeOptions = new PdfRtfWordFormatOptions();

                'object of table Log on info of Crystal report

                objReport.Load(System.Web.HttpContext.Current.Server.MapPath(("~\Content\Reports\ExtratoPendentes.rpt")))


                objReport.Database.Tables("Pendentes").SetDataSource(dt.Tables("Pendentes"))
                objReport.Database.Tables("Clientes").SetDataSource(dt.Tables("Clientes"))

                objReport.Subreports("ContasBancarias").SetDataSource(dt.Tables("Banco"))

                objReport.OpenSubreport("Pendentes").SetDataSource(dt.Tables("Pendentes"))
                objReport.DataDefinition.FormulaFields("NomeEmpresa").Text = "'" + empresadb.IDNome + "'"
                objReport.DataDefinition.FormulaFields("MoradaEmpresa").Text = "'" + empresadb.IDMorada + "'"
                objReport.DataDefinition.FormulaFields("LocalidadeEmpresa").Text = "'" + empresadb.IDLocalidade + "'"
                objReport.DataDefinition.FormulaFields("TelefoneEmpresa").Text = "'+ " + empresadb.IDIndicativoTelefone + empresadb.IDTelefone + "'"
                objReport.DataDefinition.FormulaFields("NuitEmpresa").Text = "' Nuit : " + empresadb.IFNIF + "'"
                objReport.DataDefinition.FormulaFields("EmailEmpresa").Text = "'Email: cmelo@accsys.co.mz'"
                objReport.DataDefinition.FormulaFields("Ao_Cuidado_de").Text = "' " + objectoContacto.Titulo + " " + objectoContacto.PrimeiroNome + " " + objectoContacto.UltimoNome + "'"
                'objReport.DataDefinition.FormulaFields("EmailEmpresa").Text = "' Email: " & objmotor.Contexto.IDEmail & "'"

                Dim banco As String = Nothing
                Dim descricaoBanco As String = Nothing
                Dim conta As String = Nothing
                Dim iban As String = Nothing
                Dim swift As String = Nothing
                'DataRow dr = null;
                'dr = dt.Tables["Clientes"].Rows[0];

                'banco = objmotor.Comercial.ContasBancarias.DaValorAtributo(dr["CDU_ContaRec"].ToString(), "Banco");
                'descricaoBanco = objmotor.Comercial.Bancos.DaValorAtributo(banco, "Descricao");
                'conta = objmotor.Comercial.ContasBancarias.DaValorAtributo(dr["CDU_ContaRec"].ToString(), "NumConta");
                'iban = objmotor.Comercial.ContasBancarias.DaValorAtributo(dr["CDU_ContaRec"].ToString(), "IBAN");
                'swift = objmotor.Comercial.ContasBancarias.DaValorAtributo(dr["CDU_ContaRec"].ToString(), "Swift");

                'objReport.DataDefinition.FormulaFields("BancoCliente").Text = "'" + descricaoBanco + "'";
                'objReport.DataDefinition.FormulaFields("ContaCliente").Text = "'" + conta + "'";
                'objReport.DataDefinition.FormulaFields("NibCliente").Text = "'" + objmotor.Contexto.IDEmail + "'";
                'objReport.DataDefinition.FormulaFields("IbanCliente").Text = "'" + objmotor.Contexto.IDEmail + "'";
                'objReport.DataDefinition.FormulaFields("SwiftCliente").Text = "'" + objmotor.Contexto.IDEmail + "'";

                objReport.Refresh()

                'CrDiskFileDestinationOptions.DiskFileName = pdfFile;
                'objReport.ExportOptions.DestinationOptions = CrDiskFileDestinationOptions;

                Return objReport.ExportToStream(ExportFormatType.PortableDocFormat)
            Catch ex As Exception
                'MessageBox.Show(ex.Message, "Aviso", MessageBoxButton.OK, MessageBoxImage.Error);
                Return Nothing
            End Try

        End Function



    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
