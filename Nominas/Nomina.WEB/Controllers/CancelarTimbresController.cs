using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nomina.BLL;
using RH.Entidades;
using Nomina.WEB.Filtros;
using Common.Helpers;
using System.Threading.Tasks;
using RH.BLL;
using RH.Entidades.GlobalModel;
using FiltrosWeb;

namespace Nomina.WEB.Controllers
{
    public class CancelarTimbresController : Controller
    {
        readonly FacturaElectronica _fe = new FacturaElectronica();

        // GET: CancelarTimbres
        public ActionResult Index()
        {
            var listaEjercicios = _fe.GetEjercicioFiscales();

            var selectListEjercicios = listaEjercicios.Select(x => new SelectListItem()
            {
                Value = x.IdEjercicio.ToString(),
                Text = x.Anio,
            }).ToList();

            var itemNew = new SelectListItem()
            {
                Value = "0",
                Text = "Seleccione un ejercicio",
                Selected = true
            };
            selectListEjercicios.Insert(0, itemNew);
            ViewBag.ListaEjercicios = selectListEjercicios;

            List<NotificationSummary> listSummary = new List<NotificationSummary>();

            if (TempData["summary"] != null)
            {
                listSummary = TempData["summary"] as List<NotificationSummary>;
            }

            ViewBag.Summary = listSummary;

            return View();
        }
        public JsonResult GetPeriodos(int id)
        {
            if (Session["sucursal"] != null)
            {
                var sucursal = Session["sucursal"] as SucursalDatos;

                //var periodoPago = Session["periodo"] as NOM_PeriodosPago;

                var listaPeriodos = _fe.GetNomPeriodosPagosBySucursal(sucursal.IdSucursal, id);
                
                var selectListEjercicios = listaPeriodos.Select(x => new SelectListItem()
                {
                    Value = x.IdPeriodoPago.ToString(),
                    Text = x.Descripcion,
                }).ToList();

                if (listaPeriodos.Count <= 0)
                {
                    SelectListItem item = new SelectListItem()
                    {
                        Value = "0",
                        Text = "No se encontró periodos"
                    };

                    selectListEjercicios.Add(item);
                }


                return Json(selectListEjercicios, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = "--" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTimbrados(int id)
        {
            var listaTimbrados = _fe.GetTimbradosByIdPeriodo(id);

            ViewBag.IdPeriodo = id;

            List<NotificationSummary> listSummary = new List<NotificationSummary>();

            if (TempData["summary"] != null)
            {
                listSummary = TempData["summary"] as List<NotificationSummary>;
            }

            ViewBag.Summary = listSummary;


            return PartialView(listaTimbrados);
        }
        public async Task<JsonResult> CancelarTimbres(int[] idTimbrados = null, int[] idEmisores = null)
        {
            string summaryMsj = "";
            //Validacion
            if (idEmisores == null)
            {
                return Json(new { status = "Datos Emisores = null" }, JsonRequestBehavior.AllowGet);
            }

            if (idTimbrados == null)
            {
                return Json(new { status = "Datos Timbrados = null" }, JsonRequestBehavior.AllowGet);
            }

            //Validar el archivo Certificado .pfx
            var resultValidacion = false;

            int[] EmisoresDif = idEmisores.Distinct().ToArray();
            int idEmisor = EmisoresDif[0];

            resultValidacion = _fe.ValidarArchivoPfx(idEmisor, out summaryMsj);

            //Ejecutar la cancelacion por ASYNC
            if (resultValidacion)
            {
                var idusuario = SessionHelpers.GetIdUsuario();
                var summary = await _fe.CancelarTimbresAsync(idTimbrados, idusuario);

                if (summary != null)
                {
                    TempData["summary"] = summary;
                }
            }

            //recargar la pagina

            return Json(new { status = "Proceso Cancelacion terminado" }, JsonRequestBehavior.AllowGet);
        }

    }
}