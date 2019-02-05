using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FiltrosWeb;
using Common.Enums;
using Notificacion.BLL;
using RH.Entidades;
using SYA.BLL;
using Notificaciones = Notificacion.BLL.Notificaciones;

namespace SUNInbox.WEB.Controllers
{
    public class HomeController : Controller
    {
        [AccesoAplicacion(App = Aplicacion.Notificaciones)]
        [Autenticado]
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public PartialViewResult GetListaNotificaciones(int[] filtros, int bandeja = 1, int numPage = 0, string keyword = "")
        {
            Notificaciones ctx = new Notificaciones();
            List<NotificacionDatos> model = null;
            var records = numPage * 20;
            var userSession = Session["usuario"] as SYA_Usuarios;
            
            var user = ControlAcceso.GetUsuarioEnSession();

            switch (bandeja)
            {
                case 1: //Entrada
                    model = ctx.GetBandejaEntrada(user);
                    break;
                case 2: //Favoritas
                    model = ctx.GetFavoritas(user);
                    break;
                case 3: //Nuevas
                    model = ctx.getNuevas(user);
                    break;
                case 4: //Archivadas
                    model = ctx.GetNotificationsArchivadas(user);
                    break;
            }

            double paginas = 0;
            int totalRecors = 0;
            int firstRecord = 0;
            int lastRecord = 0;

            if (model != null)
            {

                if (keyword != "")
                {
                    keyword = keyword.ToUpper();
                    model = model.Where(x => x.Titulo.ToUpper().Contains(keyword) || x.Usuario.ToUpper().Contains(keyword) || x.Cliente.ToUpper().Contains(keyword)).ToList();
                }

                if (filtros != null)
                {
                    model = model.Where(x => filtros.Contains(x.Tipo)).ToList();
                }

                paginas = (double) model.Count / (double) 20;
                totalRecors =  model.Count;

                model = model.OrderByDescending(x => x.Fecha).ToList();
                model = model.Skip(records).Take(20).ToList();

                firstRecord = model.Count == 0 ? 0 : records + 1;
                lastRecord = records + model.Count;
            }


            ViewBag.NumPaginas = Math.Ceiling(paginas);

            ViewBag.PaginaActiva = numPage;
            ViewBag.TotalRecords = totalRecors;

            ViewBag.FirstRecord = firstRecord;
            ViewBag.LastRecord = lastRecord;

            return PartialView(model);
        }

        public PartialViewResult NotificationDetails(int IdNotificacion)
        {
            Notificaciones ctx = new Notificaciones();
            var notificacion = ctx.GetNotificationDetails(IdNotificacion);
            notificacion.Comentarios = ctx.GetNotificationComments(IdNotificacion);
            return PartialView(notificacion);
        }

        public JsonResult MarcarComoLeida(int IdNotificacion)
        {
            //Notificaciones bll = new Notificaciones();
            var response = true;//bll.MarcarComoLeida(IdNotificacion);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ArchivarNotificacion(int IdNotificacion)
        {
            Notificaciones bll = new Notificaciones();
            var response = bll.ArchivarNotificacion(IdNotificacion);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ArchivarNotificaciones(int[] idsNotificacion)
        {
            Notificaciones bll = new Notificaciones();
            var actualizados = new List<int>();
            foreach(var id in idsNotificacion)
            {
                var actualizado = bll.ArchivarNotificacion(id);
                if (actualizado) actualizados.Add(id);
            }
            return Json(actualizados, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DesarchivarNotificacion(int IdNotificacion)
        {
            Notificaciones bll = new Notificaciones();
            var response = bll.DesarchivarNotificacion(IdNotificacion);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DesarchivarNotificaciones(int[] idsNotificacion)
        {
            Notificaciones bll = new Notificaciones();
            var updated = new List<int>();
            foreach(var id in idsNotificacion)
            {
                var response = bll.DesarchivarNotificacion(id);
                if (response) updated.Add(id);
            }
            return Json(updated, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MarcarComoFavorita(int IdNotificacion)
        {
            Notificaciones bll = new Notificaciones();
            var response = bll.AddFavorite(IdNotificacion);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AgregarFavoritas(int[] notificaciones)
        {
            Notificaciones bll = new Notificaciones();
            var updated = new List<int>();
            foreach(var id in notificaciones)
            {
                var response = bll.AddFavorite(id);
                if (response) updated.Add(id);
            }
            return Json(updated, JsonRequestBehavior.AllowGet);
        }

        public JsonResult QuitarFavorita(int IdNotificacion)
        {
            Notificaciones bll = new Notificaciones();
            var response = bll.DeleteFavorite(IdNotificacion);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult QuitarFavoritas(int[] notificaciones)
        {
            Notificaciones bll = new Notificaciones();
            var updated = new List<int>();
            foreach(var id in notificaciones)
            {
                var response = bll.DeleteFavorite(id);
                if (response) updated.Add(id);
            }
            return Json(updated, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddComment(RH.Entidades.Notificacion_Comentarios comment)
        {
            Notificaciones bll = new Notificaciones();
            var id = bll.AddComment(comment);
            return Json(id, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult GetComment(int IdComentario)
        {
            Notificaciones bll = new Notificaciones();
            var comment = bll.GetComment(IdComentario);
            return PartialView(comment);
        }

        public JsonResult GetDataForPrint(int[] idNotificaciones)
        {
            Notificaciones ctx = new Notificaciones();
            var notifications = ctx.GetNotifications(idNotificaciones);
            foreach (var notification in notifications)
            {
                notification.FechaString = notification.Fecha.ToString("dd/MM/yyyy");
                var body = "";
                body = notification.Contenido.Replace("<ul class='collection'>", "");
                body = body.Replace("</ul>", "");
                string[] stringsep = new string[] { "</li>" };
                var data = body.Split(stringsep, StringSplitOptions.None);
                notification.Datos = new List<string>();
                foreach (var item in data)
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
        
        [HttpPost]
        public JsonResult GetLatest(DateTime date)
        {
            var userSession = Session["usuario"] as SYA_Usuarios;
            var user = ControlAcceso.GetUsuarioEnSession();

            Notificaciones bd = new Notificaciones();
            var list = bd.getNuevas(user).Where(x => x.Fecha >= date);

            foreach(var item in list)
            {
                item.image = ControlUsuario.GetProfilePictureOfUser(item.IdUsuario);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}