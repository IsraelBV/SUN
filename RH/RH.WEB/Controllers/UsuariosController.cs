using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RH.BLL;
using System.Data.Entity;
using RH.Entidades;
using FiltrosWeb;
using Common.Enums;
using Common.Helpers;
using System.IO;
using SYA.BLL;
namespace RH.WEB.Controllers
{
    public class UsuariosController : Controller
    {

        private ControlUsuario _ctx = new ControlUsuario();
        // GET: Usuarios
        public ActionResult Index()
        {
            var IdUser = SessionHelpers.GetIdUsuario();
            var modelo = _ctx.GetUsuarioById(IdUser);
            ViewBag.UserPicture = GetPicture(modelo.Fotografia);
            return View(modelo);
        }

        public RedirectToRouteResult UploadPicture(HttpPostedFileBase picture)
        {
            //Valida que se este recibiendo un archivo.
            if(picture != null)
            {
                // string nombreArchivo = picture.FileName;
                string extension = Path.GetExtension(picture.FileName);

                //Valida que el archivo sea una imagen jpg, jpeg o gif.
                if (extension == ".jpeg" || extension == ".jpg" || extension == ".gif")
                {
                 
                    //Valida que el tamaño del archivo sea menor o igual a 1mb.
                    if (picture.ContentLength <= 500000)

                    {
                        BinaryReader reader = new BinaryReader(picture.InputStream);
                        byte[] photo = reader.ReadBytes((int)picture.InputStream.Length);

                        reader.Close();
                        _ctx.UploadPicture(photo);
                    }
                }
          
            }
            return RedirectToAction("Index");
        }

        public String GetPicture(byte[] img)
        {
            if(img != null)
            {
                var picture = Convert.ToBase64String(img);
                var userpicture = String.Format("data:image/jpg; base64, {0}", picture);
                return userpicture;
            }
            return "../Images/profpic.png";
        }

    }
}
