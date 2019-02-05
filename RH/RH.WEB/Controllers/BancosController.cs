using System.Web.Mvc;
using Common.Enums;
using FiltrosWeb;
using RH.BLL;
using RH.Entidades;


namespace RH.WEB.Controllers
{
    [Autenticado]
    [AccesoAplicacion(App = Aplicacion.Rh)]
    public class BancosController : Controller
    {
        readonly Bancos _bnc = new Bancos();

        // GET: Bancos

        [AccesoModuloAttribute(Modulo = Modulos.NomDatosBancos, Accion = AccionCrud.Consultar)]
        public ActionResult Index()
        {
            var modelo = _bnc.GetBancos();

            return View(modelo);
        }

        // GET: Bancos/Details/5
        [AccesoModuloAttribute(Modulo = Modulos.NomDatosBancos, Accion = AccionCrud.Consultar)]
        public ActionResult Details(int id)
        {
            var modelo = _bnc.GetBancoById(id);

            return View(modelo);
        }

        // GET: Bancos/Create
        [AccesoModuloAttribute(Modulo = Modulos.NomDatosBancos, Accion = AccionCrud.Agregar)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Bancos/Create
        [AccesoModuloAttribute(Modulo = Modulos.NomDatosBancos, Accion = AccionCrud.Agregar)]
        [HttpPost]
        public ActionResult Create(C_Banco_SAT collection)
        {
            try
            {
                 _bnc.CreateBanco(collection);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Bancos/Edit/5
        [AccesoModuloAttribute(Modulo = Modulos.NomDatosBancos, Accion = AccionCrud.Editar)]
        public ActionResult Edit(int id)
        {
            var modelo = _bnc.GetBancoById(id);

            return View(modelo);
        }

        // POST: Bancos/Edit/5
        [AccesoModuloAttribute(Modulo = Modulos.NomDatosBancos, Accion = AccionCrud.Editar)]
        [HttpPost]
        public ActionResult Edit(int id, C_Banco_SAT collection)
        {
            try
            {
                 _bnc.UpdateBanco(id, collection);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Bancos/Delete/5
        [AccesoModuloAttribute(Modulo = Modulos.NomDatosBancos, Accion = AccionCrud.Eliminar)]
        public ActionResult Delete(int id)
        {
            var modelo = _bnc.GetBancoById(id);
            return View(modelo);
        }

        // POST: Bancos/Delete/5
        [AccesoModuloAttribute(Modulo = Modulos.NomDatosBancos, Accion = AccionCrud.Eliminar)]
        [HttpPost]
        public ActionResult Delete(int id, C_Banco_SAT collection)
        {
            try
            {
               //  _bnc.DeleteBanco(id, collection);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }



        public ActionResult DeleteVP(int id)
        {
            var modelo = _bnc.GetBancoById(id);
            return PartialView(modelo);
        }

        [HttpPost]
        public ActionResult DeleteVP(int id, C_Banco_SAT collection)
        {
            try
            {
                //_bnc.DeleteBanco(id, collection);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }



        // GET: Bancos/Create
        [AccesoModuloAttribute(Modulo = Modulos.NomDatosBancos, Accion = AccionCrud.Agregar)]
        public ActionResult CrearVP()
        {
            return View();
        }

        // POST: Bancos/Create
        [AccesoModuloAttribute(Modulo = Modulos.NomDatosBancos, Accion = AccionCrud.Agregar)]
        [HttpPost]
        public ActionResult CrearVP(C_Banco_SAT collection)
        {
            try
            {
                _bnc.CreateBanco(collection);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }




    }
}
