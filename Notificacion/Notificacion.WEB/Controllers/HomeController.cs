using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FiltrosWeb;
using Notificacion.BLL;
using SYA.BLL;
using System.Globalization;
using Common.Enums;
using RH.Entidades;

namespace Notificacion.WEB.Controllers
{
    public class HomeController : Controller
    {
        [AccesoAplicacion(App = Aplicacion.Notificaciones)]
        [Autenticado]
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult Notifications(int numPage = 0, int filtro = 0, int extra = 0, string keyword = "")
        {
            Notificacion.BLL.Notificaciones ctx = new Notificacion.BLL.Notificaciones();
            List<NotificacionDatos> model = null;

            var records = numPage * 20;

            switch (filtro)
            {
                case 0: //bandeja de entrada
                    model = ctx.GetNotifications();
                    break;
                case 1: //favoritas
                    model = ctx.GetFavoritas();
                    break;
                case 2: //status
                    model = ctx.GetNotificationsByStatus(extra);
                    break;
                case 3: //tipo
                    model = ctx.GetNotificationsByType(extra);
                    break;
                default:
                    model = ctx.GetNotifications();
                    break;
            }

            if(keyword != "")
            {
                keyword = keyword.ToUpper();
                model = model.Where(x => x.Titulo.ToUpper().Contains(keyword) || x.Contenido.ToUpper().Contains(keyword) || x.Cliente.ToUpper().Contains(keyword)).ToList();
            }

            var paginas = (double)model.Count / (double)20;
           
            ViewBag.NumPaginas = Math.Ceiling(paginas);
            


            ViewBag.PaginaActiva = numPage;
            ViewBag.TotalRecords = model.Count;
            model = model.OrderByDescending(x => x.Fecha).ToList();
            model = model.Skip(records).Take(20).ToList();
            ViewBag.FirstRecord = records + 1;
            ViewBag.LastRecord = records + model.Count;

            return PartialView(model);
        }

        //public JsonResult GetLatest(DateTime date)
        //{
        //    Notificacion.BLL.Notificaciones bd = new Notificacion.BLL.Notificaciones();
        //    var list = bd.GetLatest(date);
        //    foreach(var item in list)
        //    {
        //        item.image = ControlUsuario.GetProfilePictureOfUser(item.IdUsuario);
        //    }
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        public void AddFavorite(int id)
        {
            Notificacion.BLL.Notificaciones n = new Notificacion.BLL.Notificaciones();
            n.AddFavorite(id);
        }

        public void DeleteFavorite(int id)
        {
            Notificacion.BLL.Notificaciones n = new Notificacion.BLL.Notificaciones();
            n.DeleteFavorite(id);
        }

        public JsonResult NotificationDetails(int id)
        {
            Notificacion.BLL.Notificaciones ctx = new Notificacion.BLL.Notificaciones();
            var not = ctx.GetNotificationDetails(id);
            return Json(not, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNotificationBody(int id)
        {
            Notificacion.BLL.Notificaciones ctx = new Notificacion.BLL.Notificaciones();
            var lista = new List<string>();
            var cuerpo = ctx.GetNotificationBody(id);
            var list = new List<string>();
            cuerpo = cuerpo.Replace("<ul class=\"collection\">", "");
            cuerpo = cuerpo.Replace("</ul>", "");
            string[] stringsep = new string[] { "</li>" };
            var array = cuerpo.Split(stringsep, StringSplitOptions.None);
            foreach(var item in array)
            {
                var dato = "";
                dato = item.Replace("<li class='collection-item'><b>", "");
                dato = dato.Replace("</b> <span class='secondary-content'>", "");
                dato = dato.Replace("</span></li>", "");
                list.Add(dato);
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNotificacionsForPrint(int[] ids)
        {
            Notificacion.BLL.Notificaciones ctx = new Notificacion.BLL.Notificaciones();
            var notifications = ctx.GetNotifications(ids);
            foreach(var notification in notifications)
            {
                notification.FechaString = notification.Fecha.ToString("dd/MM/yyyy");
                var body = "";
                body = notification.Contenido.Replace("<ul class=\"collection\">", "");
                body = body.Replace("</ul>", "");
                string[] stringsep = new string[] { "</li>" };
                var data = body.Split(stringsep, StringSplitOptions.None);
                notification.Datos = new List<string>();
                foreach(var item in data)
                {
                    var value = item;
                    value = value.Replace("<li class='collection-item'><b>", "");
                    value = value.Replace("</b> <span class='secondary-content'>", "");
                    value = value.Replace("</span>", "");
                    notification.Datos.Add(value);
                }
            }
            return Json(notifications, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult NotificationComments(int id)
        {
            Notificacion.BLL.Notificaciones n = new Notificacion.BLL.Notificaciones();
            var comments = n.GetNotificationComments(id);
            return PartialView(comments);
        }

        public JsonResult AddComment(Notificacion_Comentarios comment)
        {
            Notificacion.BLL.Notificaciones n = new Notificacion.BLL.Notificaciones();
            var result = n.AddComment(comment);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult GetComment(int id)
        {
            Notificacion.BLL.Notificaciones n = new Notificacion.BLL.Notificaciones();
            var comment = n.GetComment(id);
            return PartialView(comment);
        }

        public JsonResult MarkAsRead(int id)
        {
            Notificacion.BLL.Notificaciones ctx = new Notificacion.BLL.Notificaciones();
            var status = ctx.MarcarComoLeida(id);
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult MarkAsDone(int id)
        //{
        //    Notificacion.BLL.Notificaciones ctx = new Notificacion.BLL.Notificaciones();
        //    var status = ctx.MarcarComoAtendida(id);
        //    return Json(status, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult MarkAsReopened(int id)
        //{
        //    Notificacion.BLL.Notificaciones ctx = new Notificacion.BLL.Notificaciones();
        //    var status = ctx.MarcarComoReabierta(id);
        //    return Json(status, JsonRequestBehavior.AllowGet);
        //}
    }
}