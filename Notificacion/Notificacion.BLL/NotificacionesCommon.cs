using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;
using Common.Enums;
using Newtonsoft.Json;

namespace Notificacion.BLL
{
    public static class NotificacionesCommon
    {

        public static void CreateNotification(string titulo, DateTime fecha, int usuarioReg, TiposNotificacion idTipo , int idCliente, CuerpoDatos datos, int idSucursal,int IdContrato, string DatoReporte)
        {
           // RHEntities bd = new RHEntities();
            
            RH.Entidades.Notificaciones notif = new RH.Entidades.Notificaciones
            {
                Titulo = titulo,
                Fecha = fecha,
                UsuarioReg = usuarioReg,
                IdCliente = idCliente,
                IdTipo = (int) idTipo,
                Cuerpo = "<ul class='collection'>",
                IdSucursal = idSucursal,
                IdContrato = IdContrato,
                DatoReporte = DatoReporte                
            };

            var dtype = datos.GetType();
            var dprop = dtype.GetProperties();
            foreach(var item in dprop)
            {
                var valor = item.GetValue(datos, null);
                if(valor != null)
                {
                    var valorTipo = valor.GetType();
                    if(valorTipo == typeof(Configuracion_Empresa))
                    {
                        var valorProp = valorTipo.GetProperties();
                        notif.Cuerpo += "<li class='collection-item'><b>" + item.Name.Replace('_', ' ') + "</b>";
                        foreach(var empresa in valorProp)
                        {
                            var nombre = empresa.GetValue(valor, null);
                            if(nombre != null)
                            {
                                notif.Cuerpo += "<li class='collection-item'><b>" + empresa.Name + ":</b><span class='secondary-content'>" + nombre + "</span></li>";
                            }
                        }
                        notif.Cuerpo += "</li>";
                    }
                    else if(valor.ToString() != "0")
                    {
                        notif.Cuerpo += "<li class='collection-item'><b>" + item.Name.Replace('_', ' ') + ":</b> ";
                        notif.Cuerpo += "<span class='secondary-content'>" +valor + "</span></li>";
                    }
                }
            }
            notif.Cuerpo += "</ul>";

            //Guarda la notificacion
            using (var context = new RHEntities())
            {
                context.Notificaciones.Add(notif);
                context.SaveChanges();
            }

            //bd.Notificaciones.Add(notif);
            //bd.SaveChanges();
        }

    }
}
