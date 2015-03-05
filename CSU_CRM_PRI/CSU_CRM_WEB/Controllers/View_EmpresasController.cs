using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CSU_CRM_WEB.Models;

namespace CSU_CRM_WEB.Controllers
{
    public class View_EmpresasController : Controller
    {
        private PRIEMPREEntities db = new PRIEMPREEntities();

        // GET: View_Empresas
        public ActionResult Index()
        {
            return View(db.View_Empresas.ToList());
        }

        // GET: View_Empresas
        public ActionResult ListaEmpresas()
        {
            return PartialView("_ListaEmpresas", db.View_Empresas.ToList());
        }


        // GET: View_Empresas/Details/5
        public ActionResult Details(string id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            View_Empresas view_Empresas = db.View_Empresas.Where(p => p.Codigo == id).First();
            if (view_Empresas == null)
            {
                return HttpNotFound();
            }

            ViewBag.ConexaoPri = db.Database.Connection.ConnectionString;
            return View(view_Empresas);
        }

        // GET: View_Empresas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: View_Empresas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Codigo,IDNome,IDMorada,Categoria")] View_Empresas view_Empresas)
        {
            if (ModelState.IsValid)
            {
                db.View_Empresas.Add(view_Empresas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(view_Empresas);
        }

        // GET: View_Empresas/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            View_Empresas view_Empresas = db.View_Empresas.Find(id);
            if (view_Empresas == null)
            {
                return HttpNotFound();
            }
            return View(view_Empresas);
        }

        // POST: View_Empresas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Codigo,IDNome,IDMorada,Categoria")] View_Empresas view_Empresas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(view_Empresas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(view_Empresas);
        }

        // GET: View_Empresas/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            View_Empresas view_Empresas = db.View_Empresas.Find(id);
            if (view_Empresas == null)
            {
                return HttpNotFound();
            }
            return View(view_Empresas);
        }

        // POST: View_Empresas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            View_Empresas view_Empresas = db.View_Empresas.Find(id);
            db.View_Empresas.Remove(view_Empresas);
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
