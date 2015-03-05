using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CSU_CRM_WEB.Models;

using System.Data;
using System.Net.Mail;


using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CSU_CRM_WEB.Models.Helper;

namespace CSU_CRM_WEB.Controllers
{
    public class View_Lista_Contactos_PendentesController : Controller
    {
        private PRIACCEntities db = new PRIACCEntities();
        private string empresadb;

        // GET: View_Lista_Contactos_Pendentes
        public ActionResult Index()
        {
            return View(db.View_Lista_Contactos_Pendentes.ToList());
        }

        // GET: View_Lista_Contactos_Pendentes
        public ActionResult Lista_Empresas_Pendentes(string empresa,string conexaoPriempre)
        {
            //db = new PRIACCEntities(empresa,conexaoPriempre)
            //db.Database.Connection.Close();
            db.Database.Connection.Open();
            db.Database.Connection.ChangeDatabase("pri" + empresa);
            ViewBag.empresabd = empresa;
            empresadb = empresa;
            //db.Database.ExecuteSqlCommand() 
                //.View_Lista_Contactos_Pendentes
            return PartialView("_Lista_Empresas_Pendentes", db.View_Lista_Contactos_Pendentes.ToList());
        }

        [HttpPost]
        public ActionResult Lista_Empresas_Pendentes(FormCollection frm, string empresa, IEnumerable<string> CDU_EnviaCobranca, IEnumerable<HttpPostedFileBase> files)
        {
            this.empresadb = empresa;

            foreach (string cliente in CDU_EnviaCobranca)
            {
                Envia_Email(cliente, files, empresa);
            }
                       
            
            //string[] chequed =new string[]{} ;
            //string[] cliente = new string[] { }; 
            //string temp ;
            //foreach (string key in frm.AllKeys)
            //{
            //    Response.Write("Key" + key);
            //    Response.Write(frm[key]);
            //    if (key.Contains("item.CDU_EnviaCobranca.Value"))
            //    {
            //        chequed = (string[])frm.GetValue(key.ToString()).RawValue;
            //    }

            //    if (key.Contains("item.Cliente"))
            //    {
            //        cliente = (string[])frm.GetValue(key.ToString()).RawValue;
                    
            //    }                   
            //}
            
            //string file1 = "", file2 = "";

            //for (int i = 0; i < chequed.Length; i++)
            //{
            //    temp = chequed [i];
            //    if (temp == "true") 
            //    {
            //        string client = cliente[i];
            //        Envia_Email(client, ref file1, ref file2, empresa);
            //    }
            //}

            return RedirectToAction("Index", "Home");

        }
        
        private void Envia_Email(string codigoCliente,  IEnumerable<HttpPostedFileBase> files, string empresab)
        {
            EmailHelper envia_email = new EmailHelper(empresab);
            envia_email.enviaEmail(codigoCliente, files);
        }
        

        // GET: View_Lista_Contactos_Pendentes/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            View_Lista_Contactos_Pendentes view_Lista_Contactos_Pendentes = db.View_Lista_Contactos_Pendentes.Find(id);
            if (view_Lista_Contactos_Pendentes == null)
            {
                return HttpNotFound();
            }
            return View(view_Lista_Contactos_Pendentes);
        }

        // GET: View_Lista_Contactos_Pendentes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: View_Lista_Contactos_Pendentes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Cliente,Nome,Fac_Mor,Fac_Local,NumContrib,Pais,Fac_Tel,Moeda,CDU_ContaRec,CDU_EnviaCobranca,PrimeiroNome,UltimoNome,Titulo,Email,EmailAssist,tipoContacto,ValorPendente,ValorTotal")] View_Lista_Contactos_Pendentes view_Lista_Contactos_Pendentes)
        {
            if (ModelState.IsValid)
            {
                db.View_Lista_Contactos_Pendentes.Add(view_Lista_Contactos_Pendentes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(view_Lista_Contactos_Pendentes);
        }

        // GET: View_Lista_Contactos_Pendentes/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            View_Lista_Contactos_Pendentes view_Lista_Contactos_Pendentes = db.View_Lista_Contactos_Pendentes.Find(id);
            if (view_Lista_Contactos_Pendentes == null)
            {
                return HttpNotFound();
            }
            return View(view_Lista_Contactos_Pendentes);
        }

        // POST: View_Lista_Contactos_Pendentes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Cliente,Nome,Fac_Mor,Fac_Local,NumContrib,Pais,Fac_Tel,Moeda,CDU_ContaRec,CDU_EnviaCobranca,PrimeiroNome,UltimoNome,Titulo,Email,EmailAssist,tipoContacto,ValorPendente,ValorTotal")] View_Lista_Contactos_Pendentes view_Lista_Contactos_Pendentes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(view_Lista_Contactos_Pendentes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(view_Lista_Contactos_Pendentes);
        }

        // GET: View_Lista_Contactos_Pendentes/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            View_Lista_Contactos_Pendentes view_Lista_Contactos_Pendentes = db.View_Lista_Contactos_Pendentes.Find(id);
            if (view_Lista_Contactos_Pendentes == null)
            {
                return HttpNotFound();
            }
            return View(view_Lista_Contactos_Pendentes);
        }

        // POST: View_Lista_Contactos_Pendentes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            View_Lista_Contactos_Pendentes view_Lista_Contactos_Pendentes = db.View_Lista_Contactos_Pendentes.Find(id);
            db.View_Lista_Contactos_Pendentes.Remove(view_Lista_Contactos_Pendentes);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
