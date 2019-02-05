using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FiltrosWeb;
using RH.BLL;
using RH.Entidades;
using Common.Enums;
using SYA.BLL;
namespace RH.WEB.Controllers
{
    [Autenticado]
    [AccesoAplicacion(App = Aplicacion.Rh)]
    public class HomeController : Controller
    {

        Sucursales bll = new Sucursales();


        public ActionResult Index()
        {
            ViewBag.paginas = bll.ObtenerNumeroSucursalesPaginador();
            return View();
        }

        public ActionResult Search(string keyword, int pagina = 0)
        {
            var model = bll.SearchSucursales(keyword);

            var paginas = (double)model.Count / (double)10;

            ViewBag.paginas = Math.Ceiling(paginas);

            var skip = pagina * 10;
            model = model.Skip(skip).Take(10).ToList();
            ViewBag.Keyword = keyword;
            return PartialView("_HomeIndexSucursales", model);
        }

        public ActionResult GetErrorHttp(int id)
        {
            ViewBag.errorCode = id;
            return View();
        }

        public ActionResult AccesoDenegado()
        {
            return View();
        }

        public ActionResult AccesoDenegadoSuc()
        {
            return View();
        }

        public PartialViewResult SucursalesMenu()
        {
            var model = bll.ObtenerSucursalByIds();

            return PartialView("_LayoutSucursales", model);
        }

        public PartialViewResult SucursalesIndex(int pagina)
        {
            var usuario = ControlAcceso.GetUsuarioEnSession();

            var total = 0;

            //var model = bll.ObtenerSucursalPaginado(10, pagina, out total, usuario.Sucursales);

            var model = bll.ObtenerSucursalPaginadoV2();
            ViewBag.paginas = total;
            ViewBag.PaginaActiva = pagina;
            ViewBag.Accion = "SucursalesPaginador";

            return PartialView("_HomeIndexSucursales", model);
        }

  
        public ActionResult SucursalesPaginador(int pagina)
        {
            var usuario = ControlAcceso.GetUsuarioEnSession();

            var total = 0;

            var model = bll.ObtenerSucursalPaginado(10, pagina, out total, usuario.Sucursales);
            ViewBag.paginas = total;
            ViewBag.PaginaActiva = pagina;

            return PartialView("_HomeIndexSucursales", model);
        }

        public ActionResult SucursalesRecientes()
        {
            var model = bll.GetSucursalesRecientes();
            return PartialView(model);
        }
    }
}
