using CSU_CRM_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CSU_CRM_WEB.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Statistics() {
            // most probably the values will come from a database
            // this is just a sample to show you 
            // that you can return an IEnumerable object
            // and it will be serialized properly
            var stats = new List<YearlyStat> {
                new YearlyStat { Year=2008, Value=20},
                new YearlyStat { Year=2009, Value=10},
            };
            return Json(stats,JsonRequestBehavior.AllowGet);
        }

        private PRIEMPREEntities db = new PRIEMPREEntities();
        private PRIACCEntities dbEmpresa = new PRIACCEntities();

        public ActionResult Pendentes()
        {
            // most probably the values will come from a database
            // this is just a sample to show you 
            // that you can return an IEnumerable object
            // and it will be serialized properly
            var pendentes = new List<Pendentes>();

            
            
            foreach (var empresa in db.View_Empresas.ToList())
            {
                dbEmpresa.Database.Connection.Open();
                dbEmpresa.Database.Connection.ChangeDatabase("pri" + empresa.Codigo);
                try
                {
                    foreach (var docPendente in dbEmpresa.View_Pendentes_Doc_Clientes.ToList())
                    {
                        pendentes.Add(new Pendentes()
                        {
                            data = docPendente.DataVenc.Value.ToString("yyyy/MM/dd"),
                            empresa = empresa.Codigo,
                            valor = docPendente.ValorPendente.Value,
                            cliente = docPendente.Entidade
                        });
                    }
                }
                catch
                {

                }

                
                dbEmpresa.Database.Connection.Close();
            }


            return Json(pendentes, JsonRequestBehavior.AllowGet);
        }
    }
}