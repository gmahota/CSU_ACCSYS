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
        public PRIEmpresasEntities db;
        public string empresa;
        public Empresas empresadb;
        public AspNetUsers user;
        //CRM_MITEntities bdEmpresa;

        public string pdfFile = "";

        ReportDocument objReport ;
        ParameterDiscreteValue paraValue;
        ParameterValues currValue;
        
        public EmailHelper (string empresa){
            db = new PRIEmpresasEntities();
            db.Database.Connection.Open();
            db.Database.Connection.ChangeDatabase("pri" + empresa);

            this.empresa = empresa;
            //dbpriempre = new PRIEMPREEntities();
            //empresadb = dbpriempre.View_Empresas.Where(p => p.Codigo == empresa).First();

            objReport = new ReportDocument();
            paraValue = new ParameterDiscreteValue();
            currValue = new ParameterValues();
        }

        

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
                    mailboy = mailboy.Replace("##empresa##", empresadb.CodEmpresaPri);

                    SmtpClient Smtp_Server = new SmtpClient();
                    MailMessage e_mail = new MailMessage();

                    //Smtp_Server.UseDefaultCredentials = true;
                    //Smtp_Server.Credentials = new System.Net.NetworkCredential("gmahota@accsys.co.mz", "Accsys2011!");
                    //Smtp_Server.Port = 587;
                    //Smtp_Server.EnableSsl = true;
                    //Smtp_Server.Host = "smtp.gmail.com";



                    Smtp_Server.UseDefaultCredentials = empresadb.UseDefaultCredentials.Value;
                    Smtp_Server.Credentials = new System.Net.NetworkCredential( empresadb.Email, empresadb.Credentials);
                    Smtp_Server.Port = empresadb.Port.Value;
                    Smtp_Server.EnableSsl = empresadb.EnableSsl.Value;
                    Smtp_Server.Host = empresadb.Host;
                    
				    e_mail = new MailMessage();
                    e_mail.From = new MailAddress(empresadb.Email);
                    //e_mail.To.Add("gmahota@accsys.co.mz");
                    e_mail.To.Add(objContacto.Email);
				    e_mail.CC.Add(user.Email);

				    e_mail.Subject = "Facturas Pendentes " + empresadb.NomeEmpresa;

				    e_mail.IsBodyHtml = true;
						
                    e_mail.Body = mailboy;
				    e_mail.Attachments.Add(new System.Net.Mail.Attachment(  imprimirPdf(objContacto.Cliente),"Extrato Pendentes.pdf"));

                    if (files != null)
                    {
                        foreach (var file in files)
                        {
                            e_mail.Attachments.Add(new System.Net.Mail.Attachment(file.InputStream, Path.GetFileName(file.FileName)));
                        }  
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

                var pendentes = db.View_Pendentes_Doc_Clientes.Where(p => p.Entidade == codigoCliente);

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
                ExportOptions CrExportOptions = default(ExportOptions);
                DiskFileDestinationOptions CrDiskFileDestinationOptions = new DiskFileDestinationOptions();
                PdfRtfWordFormatOptions CrFormatTypeOptions = new PdfRtfWordFormatOptions();

                

                //object of table Log on info of Crystal report
                objReport.FileName = System.Web.HttpContext.Current.Server.MapPath((@"~\Content\Reports\ExtratoPendentes.rpt"));
                objReport.Load(objReport.FileName);

                objReport.SetDataSource(dt.Tables["Pendentes"]);
                objReport.Database.Tables["Pendentes"].SetDataSource(dt.Tables["Pendentes"]);
                objReport.Database.Tables["Clientes"].SetDataSource(dt.Tables["Clientes"]);

                objReport.Subreports["ContasBancarias"].SetDataSource(dt.Tables["Banco"]);

                objReport.OpenSubreport("Pendentes").SetDataSource(dt.Tables["Pendentes"]);
                objReport.DataDefinition.FormulaFields["NomeEmpresa"].Text = "'" + empresadb.CodEmpresaPri + "'";
                objReport.DataDefinition.FormulaFields["MoradaEmpresa"].Text = "'" + empresadb.MoradaEmpresa + "'";
                objReport.DataDefinition.FormulaFields["LocalidadeEmpresa"].Text = "'" + empresadb.LocalidadeEmpresa + "'";
                objReport.DataDefinition.FormulaFields["TelefoneEmpresa"].Text = "'+ " + empresadb.TelefoneEmpresa  + "'";
                objReport.DataDefinition.FormulaFields["NuitEmpresa"].Text = "' Nuit : " + empresadb.NuitEmpresa+ "'";
                objReport.DataDefinition.FormulaFields["EmailEmpresa"].Text = "'"+ user.Email +"'";
                objReport.DataDefinition.FormulaFields["Ao_Cuidado_de"].Text = "' " + objectoContacto.Titulo + " " + objectoContacto.PrimeiroNome+ " "+ objectoContacto.UltimoNome + "'";
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

                CrDiskFileDestinationOptions.DiskFileName = System.Web.HttpContext.Current.Server.MapPath((@"~\Content\Avisos\Teste.pdf"));
                objReport.ExportOptions.DestinationOptions = CrDiskFileDestinationOptions;

                Stream stream = objReport.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                
                return stream;
                
            }
            catch (Exception ex)
            {
                return null;
                //MessageBox.Show(ex.Message, "Aviso", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
                
        internal void enviaEmailComRelatorio(string codigoCliente, IEnumerable<HttpPostedFileBase> files, string tipoExtrato, string p)
        {
            try
            {

                DataSet ds = new DataSet();
                string filename = null;
                string mailboy = null;

                bool enviado = false;
                enviado = false;


                
                //objContactos = objmotor.CRM.Contactos.ListaContactosDaEntidade("C", codigoCliente);
                var listFacturasPendentes = db.View_Lista_Contactos_Pendentes.Where(a => a.Cliente == codigoCliente).ToList();
                View_Lista_Contactos_Pendentes objContacto = listFacturasPendentes.First();

                //imprimirPdf(codigoCliente);

                try
                {
                    filename = System.Web.HttpContext.Current.Server.MapPath(@"~/Content/Reports/template.htm");// "~/Content/Reports/template.htm";

                    mailboy = System.IO.File.ReadAllText(filename);
                    mailboy = mailboy.Replace("##FirstName##", objContacto.Titulo + " " + objContacto.PrimeiroNome + " " + objContacto.UltimoNome);
                    mailboy = mailboy.Replace("##cliente##", objContacto.Nome);
                    //mailboy = mailboy.Replace("##quantidade##", ds.Tables[0].Rows[0]["Quantidade"].ToString());
                    mailboy = mailboy.Replace("##divida##", objContacto.ValorPendente.ToString());
                    mailboy = mailboy.Replace("##empresa##", empresadb.CodEmpresaPri);

                    SmtpClient Smtp_Server = new SmtpClient();
                    MailMessage e_mail = new MailMessage();

                    //Smtp_Server.UseDefaultCredentials = true;
                    //Smtp_Server.Credentials = new System.Net.NetworkCredential("gmahota@accsys.co.mz", "Accsys2011!");
                    //Smtp_Server.Port = 587;
                    //Smtp_Server.EnableSsl = true;
                    //Smtp_Server.Host = "smtp.gmail.com";



                    Smtp_Server.UseDefaultCredentials = empresadb.UseDefaultCredentials.Value;
                    Smtp_Server.Credentials = new System.Net.NetworkCredential(empresadb.Email, empresadb.Credentials);
                    Smtp_Server.Port = empresadb.Port.Value;
                    Smtp_Server.EnableSsl = empresadb.EnableSsl.Value;
                    Smtp_Server.Host = empresadb.Host;

                    e_mail = new MailMessage();
                    e_mail.From = new MailAddress(empresadb.Email);
                    //e_mail.To.Add("gmahota@accsys.co.mz");
                    e_mail.To.Add(objContacto.Email);
                    e_mail.CC.Add(user.Email);

                    e_mail.Subject = "Facturas Pendentes " + empresadb.NomeEmpresa;

                    e_mail.IsBodyHtml = true;

                    e_mail.Body = mailboy;
                    e_mail.Attachments.Add(new System.Net.Mail.Attachment(imprimirPdf(objContacto.Cliente), "Extrato Pendentes.pdf"));

                    if (files != null)
                    {
                        foreach (var file in files)
                        {
                            e_mail.Attachments.Add(new System.Net.Mail.Attachment(file.InputStream, Path.GetFileName(file.FileName)));
                        }
                    }
                    
                    //if (!string.IsNullOrEmpty(anexo1))
                    //    e_mail.Attachments.Add(new System.Net.Mail.Attachment(anexo1));
                    //if (!string.IsNullOrEmpty(anexo2))
                    //    e_mail.Attachments.Add(new System.Net.Mail.Attachment(anexo2));

                    Smtp_Server.Send(e_mail);

                    enviado = true;
                }
                catch (Exception error_t)
                {
                    //Interaction.MsgBox(error_t.ToString());
                    enviado = false;
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}