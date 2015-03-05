using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace CSU_CRM_WEB.Models.Helper
{
    public class EmailHelper
    {
        private PRIACCEntities db;
        private string empresa;
        View_Empresas empresadb;
        PRIEMPREEntities dbpriempre;

        ReportDocument objReport ;
        ParameterDiscreteValue paraValue;
        ParameterValues currValue;
        
        public EmailHelper (string empresa){
            db = new PRIACCEntities();
            db.Database.Connection.Open();
            db.Database.Connection.ChangeDatabase("pri" + empresa);

            this.empresa = empresa;
            dbpriempre = new PRIEMPREEntities();
            empresadb = dbpriempre.View_Empresas.Where(p => p.Codigo == empresa).First();

            objReport = new ReportDocument();
            paraValue = new ParameterDiscreteValue();
            currValue = new ParameterValues();
        }

        string pdfFile  = "c:/Avisos/Av100012014.pdf";

        public void enviaEmail(string codigoCliente, IEnumerable<HttpPostedFileBase> files)
        {
	        try 
            {
                
                DataSet ds = new DataSet();
		        string filename = null;
		        string mailboy = null;

		        bool enviado = false;
		        enviado = false;


		        pdfFile = "c:\\Avisos\\Extrato." + codigoCliente + "." + DateTime.Now.ToString("ddMMyyyy") + ".pdf";

		        //objContactos = objmotor.CRM.Contactos.ListaContactosDaEntidade("C", codigoCliente);
                var listFacturasPendentes = db.View_Lista_Contactos_Pendentes.Where(p=> p.Cliente == codigoCliente).ToList();
                View_Lista_Contactos_Pendentes objContacto = listFacturasPendentes.First();
                
                //imprimirPdf(codigoCliente);
                
                try 
                {
                    filename = System.Web.HttpContext.Current.Server.MapPath (@"~/Content/Reports/template.htm");// "~/Content/Reports/template.htm";

                    mailboy = System.IO.File.ReadAllText(filename);
                    mailboy = mailboy.Replace("##FirstName##", objContacto.Titulo + " " + objContacto.Nome);
                    mailboy = mailboy.Replace("##cliente##", objContacto.Nome);
                    //mailboy = mailboy.Replace("##quantidade##", ds.Tables[0].Rows[0]["Quantidade"].ToString());
                    mailboy = mailboy.Replace("##divida##", objContacto.ValorPendente.ToString());
                    mailboy = mailboy.Replace("##empresa##", empresadb.IDNome);
				    SmtpClient Smtp_Server = new SmtpClient();
				    MailMessage e_mail = new MailMessage();
				    

                    //Smtp_Server.UseDefaultCredentials = true;
                    //Smtp_Server.Credentials = new System.Net.NetworkCredential("guimaraesmahota@gmail.com", "Accsys2011!");
                    //Smtp_Server.Port = 587;
                    //Smtp_Server.EnableSsl = true;
                    //Smtp_Server.Host = "smtp.gmail.com";

                    Smtp_Server.UseDefaultCredentials = false;
                    Smtp_Server.Credentials = new System.Net.NetworkCredential("avisos@meridian32.com", "");
                    Smtp_Server.Port = 25;
                    Smtp_Server.EnableSsl = false;
                    Smtp_Server.Host = "192.168.3.14";
                    
				    e_mail = new MailMessage();
				    e_mail.From = new MailAddress("avisos@meridian32.com");
                    //e_mail.To.Add("gmahota@accsys.co.mz");
                    e_mail.To.Add(objContacto.Email);
				    e_mail.CC.Add("cmelo@accsys.co.mz");

				    e_mail.Subject = "Facturas Pendentes " + empresadb.IDNome;

				    e_mail.IsBodyHtml = true;
						
                    e_mail.Body = mailboy;
				    e_mail.Attachments.Add(new System.Net.Mail.Attachment(  imprimirPdf(objContacto.Cliente),"Extrato Pendentes.pdf"));

                    foreach (var file in files)
                    {
                        e_mail.Attachments.Add(new System.Net.Mail.Attachment(file.InputStream, Path.GetFileName(file.FileName)));
                    }                   



                    //if (!string.IsNullOrEmpty(anexo1))
                    //    e_mail.Attachments.Add(new System.Net.Mail.Attachment(anexo1));
                    //if (!string.IsNullOrEmpty(anexo2))
                    //    e_mail.Attachments.Add(new System.Net.Mail.Attachment(anexo2));

				    Smtp_Server.Send(e_mail);

				    enviado = true;
			    } catch (Exception error_t) {
				    //Interaction.MsgBox(error_t.ToString());
				    enviado = false;
			    }
            }catch (Exception ex) {
            }
        }

        private Stream imprimirPdf(string codigoCliente)
        {
            try
            {
                var view_Cliente = db.View_Lista_Contactos_Pendentes.Where(p => p.Cliente == codigoCliente).First();

                ClientesDataSet dataSet = new ClientesDataSet();

                DataRow row = dataSet.Tables["Clientes"].NewRow();
                row["Cliente"] = view_Cliente.Cliente;
                row["Nome"] = view_Cliente.Nome;
                row["Fac_Mor"] = view_Cliente.Fac_Mor;
                row["Fac_Local"] = view_Cliente.Fac_Local;
                row["Fac_Tel"] = view_Cliente.Fac_Tel;
                row["NumContrib"] = view_Cliente.NumContrib;
                row["Pais"] = view_Cliente.Pais;
                row["Moeda"] = view_Cliente.Moeda;
                row["PrimeiroNome"] = view_Cliente.PrimeiroNome;
                row["UltimoNome"] = view_Cliente.UltimoNome;
                row["Titulo"] = view_Cliente.Titulo;
                row["Email"] = view_Cliente.Email;
                row["EmailAssist"] = view_Cliente.EmailAssist;

                dataSet.Tables["Clientes"].Rows.Add(row);

                var bancos = db.View_Bancos_Cobrancas.ToList();

                foreach (var banco in bancos)
                {
                    row = dataSet.Tables["Banco"].NewRow();
                    row["Banco"] = banco.Banco;
                    row["Descricao"] = banco.Descricao;
                    row["NumConta"] = banco.numconta;
                    row["Nib"] = banco.nib;
                    row["Iban"] = banco.IBAN;
                    row["Swift"] = banco.SWIFT;
                    row["Moeda"] = banco.Moeda;

                    dataSet.Tables["Banco"].Rows.Add(row);
                }

                var pendentes = db.View_Pendentes_Doc_Clientes;

                foreach (var pendente in pendentes)
                {
                    row = dataSet.Tables["Pendentes"].NewRow();
                    
                    row["TipoDoc"] = pendente.TipoDoc;
                    row["Serie"] = pendente.Serie;
                    row["TipoEntidade"] = pendente.TipoEntidade;
                    row["Entidade"] = pendente.Entidade;
                    row["DataDoc"] = pendente.DataDoc;
                    row["DataVenc"] = pendente.DataVenc;

                    row["NumDoc"] = pendente.NumDoc;
                    row["NumDocInt"] = pendente.NumDocInt;
                    row["ValorTotal"] = pendente.ValorTotal;
                    row["ValorPendente"] = pendente.ValorPendente;

                    row["NumDoc"] = pendente.Moeda;
                    row["NumDocInt"] = pendente.Cambio;

                    dataSet.Tables["Pendentes"].Rows.Add(row);
                }

                return imprimirPdf(dataSet, view_Cliente);
            }
            catch
            {
                return null;
            }
            
        }

        private Stream imprimirPdf(ClientesDataSet dt, View_Lista_Contactos_Pendentes objectoContacto)
        {
            try
            {
                //ExportOptions CrExportOptions = default(ExportOptions);
                //DiskFileDestinationOptions CrDiskFileDestinationOptions = new DiskFileDestinationOptions();
                //PdfRtfWordFormatOptions CrFormatTypeOptions = new PdfRtfWordFormatOptions();

                //object of table Log on info of Crystal report

                objReport.Load(System.Web.HttpContext.Current.Server.MapPath ((@"~\Content\Reports\ExtratoPendentes.rpt")));


                objReport.Database.Tables["Pendentes"].SetDataSource(dt.Tables["Pendentes"]);
                objReport.Database.Tables["Clientes"].SetDataSource(dt.Tables["Clientes"]);

                objReport.Subreports["ContasBancarias"].SetDataSource(dt.Tables["Banco"]);

                objReport.OpenSubreport("Pendentes").SetDataSource(dt.Tables["Pendentes"]);
                objReport.DataDefinition.FormulaFields["NomeEmpresa"].Text = "'" + empresadb.IDNome + "'";
                objReport.DataDefinition.FormulaFields["MoradaEmpresa"].Text = "'" + empresadb.IDMorada + "'";
                objReport.DataDefinition.FormulaFields["LocalidadeEmpresa"].Text = "'" + empresadb.IDLocalidade + "'";
                objReport.DataDefinition.FormulaFields["TelefoneEmpresa"].Text = "'+ " + empresadb.IDIndicativoTelefone + empresadb.IDTelefone + "'";
                objReport.DataDefinition.FormulaFields["NuitEmpresa"].Text = "' Nuit : " + empresadb.IFNIF + "'";
                objReport.DataDefinition.FormulaFields["EmailEmpresa"].Text = "'Email: cmelo@accsys.co.mz'";
                objReport.DataDefinition.FormulaFields["Ao_Cuidado_de"].Text = "' " + objectoContacto.Titulo + " " + objectoContacto.Nome + "'";
                //objReport.DataDefinition.FormulaFields("EmailEmpresa").Text = "' Email: " & objmotor.Contexto.IDEmail & "'"

                string banco = null;
                string descricaoBanco = null;
                string conta = null;
                string iban = null;
                string swift = null;
                //DataRow dr = null;
                //dr = dt.Tables["Clientes"].Rows[0];

                //banco = objmotor.Comercial.ContasBancarias.DaValorAtributo(dr["CDU_ContaRec"].ToString(), "Banco");
                //descricaoBanco = objmotor.Comercial.Bancos.DaValorAtributo(banco, "Descricao");
                //conta = objmotor.Comercial.ContasBancarias.DaValorAtributo(dr["CDU_ContaRec"].ToString(), "NumConta");
                //iban = objmotor.Comercial.ContasBancarias.DaValorAtributo(dr["CDU_ContaRec"].ToString(), "IBAN");
                //swift = objmotor.Comercial.ContasBancarias.DaValorAtributo(dr["CDU_ContaRec"].ToString(), "Swift");

                //objReport.DataDefinition.FormulaFields("BancoCliente").Text = "'" + descricaoBanco + "'";
                //objReport.DataDefinition.FormulaFields("ContaCliente").Text = "'" + conta + "'";
                //objReport.DataDefinition.FormulaFields("NibCliente").Text = "'" + objmotor.Contexto.IDEmail + "'";
                //objReport.DataDefinition.FormulaFields("IbanCliente").Text = "'" + objmotor.Contexto.IDEmail + "'";
                //objReport.DataDefinition.FormulaFields("SwiftCliente").Text = "'" + objmotor.Contexto.IDEmail + "'";

                objReport.Refresh();

                //CrDiskFileDestinationOptions.DiskFileName = pdfFile;
                //objReport.ExportOptions.DestinationOptions = CrDiskFileDestinationOptions;
                return objReport.ExportToStream(ExportFormatType.PortableDocFormat);
                
            }
            catch (Exception ex)
            {
                return null;
                //MessageBox.Show(ex.Message, "Aviso", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        
		       
    }
}