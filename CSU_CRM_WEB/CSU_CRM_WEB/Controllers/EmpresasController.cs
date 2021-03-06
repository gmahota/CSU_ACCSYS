﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CSU_CRM_WEB.Models;
using CSU_CRM_WEB.Models.Helper;
using Microsoft.AspNet.Identity;

namespace CSU_CRM_WEB.Controllers
{
    [Authorize]
    public class EmpresasController : Controller
    {
        private CRM_MITEntities db = new CRM_MITEntities();
        private PRIEmpresasEntities dbEmpresaPrimavera = new PRIEmpresasEntities();
        
        private void abreEmpresaPrimavera()
        {
            
            string codEmpresaPrimavera;

            codEmpresaPrimavera = Session["EmpresaCodigoPrimavera"].ToString();
            try
            {
                dbEmpresaPrimavera.Database.Connection.Open();
                dbEmpresaPrimavera.Database.Connection.ChangeDatabase(codEmpresaPrimavera);
            }
            catch
            {
                dbEmpresaPrimavera.Database.Connection.ChangeDatabase(codEmpresaPrimavera);
            }
            
        }

        #region dados gerais
        // GET: Empresas
        public ActionResult Index()
        {
            return View(db.Empresas.ToList());
        }

        // GET: Empresas/Details/5
        public ActionResult Details(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresas empresas = db.Empresas.Find(id);
            if (empresas == null)
            {
                return HttpNotFound();
            }
            return View(empresas);
        }

        

        // GET: Empresas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Empresas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CodEmpresa,CodEmpresaPri,NomeEmpresa,LogoTipo,Conexao,EmpresaPrimavera,Credentials,Email,EnableSsl,Host,Port,UseDefaultCredentials")] Empresas empresas)
        {
            if (ModelState.IsValid)
            {
                
                db.Empresas.Add(empresas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(empresas);
        }

        // GET: Empresas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresas empresas = db.Empresas.Find(id);
            if (empresas == null)
            {
                return HttpNotFound();
            }
            return View(empresas);
        }

        // POST: Empresas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CodEmpresa,CodEmpresaPri,NomeEmpresa,LogoTipo,Conexao,EmpresaPrimavera,Credentials,Email,EnableSsl,Host,Port,UseDefaultCredentials")] Empresas empresas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(empresas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(empresas);
        }

        // GET: Empresas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresas empresas = db.Empresas.Find(id);
            if (empresas == null)
            {
                return HttpNotFound();
            }
            return View(empresas);
        }

        // POST: Empresas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Empresas empresas = db.Empresas.Find(id);
            db.Empresas.Remove(empresas);
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

        // GET: View_Empresas
        public ActionResult ListaEmpresas()
        {
            return PartialView("_ListaEmpresas", db.Empresas.ToList());
        }
        #endregion
        
        #region Dashboard
        
        public ActionResult Dashboard(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresas empresas = db.Empresas.Find(id);
            if (empresas == null)
            {
                return HttpNotFound();
            }
            Session["Empresa"] = empresas.CodEmpresa;
            Session["EmpresaNome"] = empresas.NomeEmpresa;
            Session["EmpresaCodigoPrimavera"] = empresas.CodEmpresaPri;

            abreEmpresaPrimavera();

            var facturasPendentes = 0;
            dbEmpresaPrimavera.Database.SqlQuery<int>("select  count (DISTINCT IdHistorico) from Pendentes where Modulo = 'V' ").ToList().First();

            ViewBag.FacturasPendentes = facturasPendentes;

            return View(empresas);
        }
        #endregion
        
        #region Extrato Pendentes de Clientes

        public ActionResult ListaExtratoClientes()
        {
            string codEmpresaPrimavera;

            if (Session["Empresa"] != null)
            {
                codEmpresaPrimavera = Session["EmpresaCodigoPrimavera"].ToString();
                dbEmpresaPrimavera.Database.Connection.Open();
                dbEmpresaPrimavera.Database.Connection.ChangeDatabase(codEmpresaPrimavera);

                var temp = dbEmpresaPrimavera.View_Lista_Contactos_Pendentes;
                try
                {
                    return PartialView(temp.ToList());
                }
                catch
                {
                    return PartialView(new List<View_Lista_Contactos_Pendentes>());
                }
                
            }
            else
            {

                return RedirectToAction("Index", "Home", null);
            }

        }

        public ActionResult ListaPendentesClientes()
        {
            string codEmpresaPrimavera;

            if (Session["Empresa"] != null)
            {
                codEmpresaPrimavera = Session["EmpresaCodigoPrimavera"].ToString();
                dbEmpresaPrimavera.Database.Connection.Open();
                dbEmpresaPrimavera.Database.Connection.ChangeDatabase(codEmpresaPrimavera);
                abreEmpresaPrimavera();

                var temp = dbEmpresaPrimavera.View_Lista_Contactos_Pendentes;
                try
                {
                    return PartialView( temp.ToList());
                }
                catch
                {
                    return PartialView( new List<View_Lista_Contactos_Pendentes>());
                }
                
            }
            else
            {

                return RedirectToAction("Index", "Home", null);
            }

        }

        [HttpPost]
        public ActionResult ListaPendentesClientes(FormCollection frm,string tipoExtrato, IEnumerable<string> CDU_EnviaCobranca, IEnumerable<HttpPostedFileBase> files)
        {
            string codEmpresaPrimavera;
            
            if (Session["Empresa"] != null)
            {
                codEmpresaPrimavera = Session["EmpresaCodigoPrimavera"].ToString();
                dbEmpresaPrimavera.Database.Connection.Open();
                dbEmpresaPrimavera.Database.Connection.ChangeDatabase(codEmpresaPrimavera);
                abreEmpresaPrimavera();

                var temp = dbEmpresaPrimavera.View_Lista_Contactos_Pendentes;
                try
                {
                    EmailHelper envia_email = new EmailHelper(Session["Empresa"].ToString());
                    envia_email.db = dbEmpresaPrimavera;
                    envia_email.empresadb = db.Empresas.ToList().Where(p => p.CodEmpresa == Session["Empresa"].ToString()).First();
                    
                    
                    foreach (string cliente in CDU_EnviaCobranca)
                    {
                        
                        AspNetUsers currentUser = db.AspNetUsers.FirstOrDefault(x => x.UserName == User.Identity.Name);
                        envia_email.user = currentUser;
                        envia_email.enviaEmailComRelatorio(cliente, files, tipoExtrato, currentUser.Email);
                        
                    }
                    return RedirectToAction("Index", "Home");
                }
                catch
                {
                    return RedirectToAction("Index", "Home");
                }
                
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}
