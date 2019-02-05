using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Common.Helpers;
using Common.Enums;
using SYA.BLL;
using RH.Entidades;

namespace Notificacion.BLL
{
    public class Notificaciones
    {
        RHEntities ctx = null;

        public Notificaciones()
        {
            ctx = new RHEntities();
        }

        public List<NotificacionDatos> GetNotifications(int[] ids)
        {
            return (from n in ctx.Notificaciones
                    join c in ctx.Cliente on n.IdCliente equals c.IdCliente
                    join u in ctx.SYA_Usuarios on n.UsuarioReg equals u.IdUsuario
                    join tn in ctx.C_NotificacionTipo on n.IdTipo equals tn.IdTipo
                    where ids.Contains(n.IdNotificacion)
                    select new NotificacionDatos
                    {
                        IdNotificacion = n.IdNotificacion,
                        Titulo = n.Titulo,
                        Fecha = n.Fecha,
                        Cliente = c.Nombre,
                        Usuario = u.Nombres + " " + u.ApPaterno,
                        TipoDescripcion = tn.Descripcion,
                        Contenido = n.Cuerpo
                    }
                    ).ToList();
        }

        public static List<int> GetTiposNotificacionDepartamento(SYA_Usuarios user)
        {
            using (var context = new RHEntities())
            {
                return user.IdPerfil == 1 ? context.C_NotificacionTipo.Select(x => x.IdTipo).ToList() : context.Notificacion_Departamento.Where(x => x.IdDepartamento == user.IdDepartamento).Select(x => x.IdTipo).ToList();
            }
            
        }

        //public List<NotificacionDatos> GetNotifications()
        //{
        //    var user = ControlAcceso.GetUsuarioEnSession();
        //    if (user.IdPerfil == 1)
        //    {
        //        return GetNotificationsPerron(user.IdUsuario);
        //    }
        //    var accesoATipos = GetTiposNotificacionDepartamento(user);
        //    if (user.Sucursales == "*")
        //    {
        //        return (from n in ctx.Notificaciones
        //                join t in ctx.C_NotificacionTipo on n.IdTipo equals t.IdTipo
        //                join su in ctx.SYA_Usuarios on n.UsuarioReg equals su.IdUsuario
        //                join cli in ctx.Cliente on n.IdCliente equals cli.IdCliente
        //                join sucu in ctx.Sucursal on cli.IdCliente equals sucu.IdCliente
        //                let fav = ctx.Notificaciones_Favoritas
        //                    .FirstOrDefault(x => x.IdNotificacion == n.IdNotificacion && x.IdUsuario == user.IdUsuario)
        //                let nu = ctx.Notificacion_Status
        //                        .Where(x => x.IdNotificacion == n.IdNotificacion && x.IdUsuario == user.IdUsuario)
        //                        .OrderByDescending(x => x.Fecha)
        //                        .FirstOrDefault()
        //                where su.IdUsuario == user.IdUsuario && accesoATipos.Contains(n.IdTipo)
        //                select new NotificacionDatos
        //                {
        //                    IdNotificacion = n.IdNotificacion,
        //                    Titulo = n.Titulo,
        //                    Tipo = n.IdTipo,
        //                    TipoDescripcion = t.Descripcion,
        //                    IdUsuario = n.UsuarioReg,
        //                    Contenido = n.Cuerpo,
        //                    Usuario = su.Nombres + " " + su.ApPaterno,
        //                    Fecha = n.Fecha,
        //                    Cliente = sucu.Ciudad,
        //                    Favorita = fav == null ? false : true,
        //                    Status = nu == null ? 1 : nu.Status
        //                }
        //                ).ToList();
        //    }
        //    else
        //    {
        //        var sucursales = user.Sucursales.Split(',');

        //        return (from n in ctx.Notificaciones
        //                join t in ctx.C_NotificacionTipo on n.IdTipo equals t.IdTipo
        //                join su in ctx.SYA_Usuarios on n.UsuarioReg equals su.IdUsuario
        //                join cli in ctx.Cliente on n.IdCliente equals cli.IdCliente
        //                join s in ctx.Sucursal on cli.IdCliente equals s.IdCliente
        //                let fav = ctx.Notificaciones_Favoritas
        //                    .FirstOrDefault(x => x.IdNotificacion == n.IdNotificacion && x.IdUsuario == user.IdUsuario)
        //                let nu = ctx.Notificacion_Status
        //                        .Where(x => x.IdNotificacion == n.IdNotificacion && x.IdUsuario == su.IdUsuario)
        //                        .OrderByDescending(x => x.Fecha)
        //                        .FirstOrDefault()
        //                where su.IdUsuario == user.IdUsuario && sucursales.Contains(s.IdSucursal.ToString()) && accesoATipos.Contains(n.IdTipo)
        //                select new NotificacionDatos
        //                {
        //                    IdNotificacion = n.IdNotificacion,
        //                    Titulo = n.Titulo,
        //                    Tipo = n.IdTipo,
        //                    Contenido = n.Cuerpo,
        //                    TipoDescripcion = t.Descripcion,
        //                    IdUsuario = n.UsuarioReg,
        //                    Usuario = su.Nombres + " " + su.ApPaterno,
        //                    Fecha = n.Fecha,
        //                    Cliente = s.Ciudad, // cli.Nombre,
        //                    Favorita = fav == null ? false : true,
        //                    Status = nu == null ? 1 : nu.Status,
        //                }
        //                )
        //                .ToList();
        //    }
        //}


        public List<NotificacionDatos> GetNotifications2( SYA_Usuarios usuario, int statusNotificacion = 0 )
        {
            //Obtenemos los tipos de noti que tiene acceso segun su departamento
            //ALta - Baja - IMSS - ETC.
            var accesoATipos = GetTiposNotificacionDepartamento(usuario);
            var arrayTipos = accesoATipos.ToArray();

            //val tipos de acceso
            if (arrayTipos.Length <= 0) return null;

            //Si el perfil es SU = 1 Perron
            int[] arrayIdSucursal = null;

            //Creamos la consulta
            using (var context = new RHEntities())
            {

                if (usuario.Sucursales == "*")
                {
                     arrayIdSucursal = context.Sucursal.Select(x => x.IdSucursal).ToArray();
                }
                else
                {
                    //Obtiene las sucursales a los que tiene acceso el usuario
                    var sucursalesAcceso = usuario.Sucursales.Split(',');
                    arrayIdSucursal = sucursalesAcceso.Select(int.Parse).ToArray();
                }

                if(arrayIdSucursal.Length <= 0) return null;
                

                var listaNotif = (from n in context.Notificaciones
                    join usu in context.SYA_Usuarios on n.UsuarioReg equals usu.IdUsuario
                    join cli in context.Cliente on n.IdCliente equals cli.IdCliente
                    where arrayIdSucursal.Contains(n.IdSucursal)
                    && arrayTipos.Contains(n.IdTipo)
                    select new NotificacionDatos
                    {
                        IdNotificacion = n.IdNotificacion,
                        Titulo = n.Titulo,
                        Tipo = n.IdTipo,
                        TipoDescripcion = ((TiposNotificacion) n.IdTipo).ToString().Replace("_"," "),
                        IdUsuario = n.UsuarioReg,
                        Contenido = n.Cuerpo,
                        Usuario = usu.Nombres + " " + usu.ApPaterno,
                        Fecha = n.Fecha,
                        Cliente = cli.Nombre,
                        Favorita = false,
                        Status = 1
                    }).ToList();


                //
                var arrayNotis = listaNotif.Select(x => x.IdNotificacion).ToArray();

                //Identificar si esta como favorita
                var listaFavDelUsuario = (from fav in context.Notificaciones_Favoritas
                    where arrayNotis.Contains(fav.IdNotificacion)
                          && fav.IdUsuario == usuario.IdUsuario
                    select fav).ToList();

                if (listaFavDelUsuario.Count > 0)
                {
                    foreach (var itemFav in listaFavDelUsuario)
                    {
                        //buscamos la notificacion
                        var itemnot = listaNotif.FirstOrDefault(x => x.IdNotificacion == itemFav.IdNotificacion);
                        if(itemnot== null)continue;
                        itemnot.Favorita = true;
                    }
                }

                //Actualizar estatus
                var listaEstatus = (
                    from st in context.Notificacion_Status
                    where arrayNotis.Contains(st.IdNotificacion)
                          && st.IdUsuario == usuario.IdUsuario
                    select st).ToList();

                if (listaEstatus.Count > 0)
                {
                    foreach (var itemSt in listaEstatus)
                    {
                        //buscamos la notificacion
                        var itemnot = listaNotif.FirstOrDefault(x => x.IdNotificacion == itemSt.IdNotificacion);
                        if (itemnot == null) continue;
                        itemnot.Status = itemSt.Status;
                    }
                }


                return listaNotif;
            }

        }


        public List<NotificacionDatos> GetFavoritas(SYA_Usuarios user)
        {
            var lista = GetNotifications2(user);
            return lista?.Where(x => x.Favorita == true).ToList();
        }

        private List<NotificacionDatos> GetNotificationsPerron(int IdUsuario)
        {
            //Nueva Colsulta
            using (var context = new RHEntities())
            {
                //Leer las notificaciones - asignar el tipo - el usuario - Scucursal - 
                var listaNoti = (from not in context.Notificaciones
                    join tipo in context.C_NotificacionTipo on not.IdTipo equals tipo.IdTipo
                    join usu in context.SYA_Usuarios on not.UsuarioReg equals usu.IdUsuario
                    join sucu in context.Sucursal on not.IdCliente equals sucu.IdSucursal
                    join cli in context.Cliente on sucu.IdCliente equals cli.IdCliente
                    select new NotificacionDatos
                    {
                        IdNotificacion = not.IdNotificacion,
                        Titulo = not.Titulo,
                        Tipo = not.IdTipo,
                        TipoDescripcion = tipo.Descripcion.Replace("_"," "),
                        IdUsuario = not.UsuarioReg,
                        Usuario = usu.Nombres + " " + usu.ApPaterno,
                        Contenido = not.Cuerpo,
                        Fecha = not.Fecha,
                        Favorita = false,
                        Cliente = sucu.Ciudad,
                        EmpresaCliente = cli.Nombre,
                        Status = 1
                    }).ToList();

               
            
                //Asignar el estatus
                var arrayNotis = listaNoti.Select(x => x.IdNotificacion).ToArray();

                var listaStatus = (from stado in context.Notificacion_Status
                    where arrayNotis.Contains(stado.IdNotificacion)
                          && stado.IdUsuario == IdUsuario
                    select stado).ToList();

                //ASignar los favoritos
                var listaFavoritos = (from fav in context.Notificaciones_Favoritas
                                  where arrayNotis.Contains(fav.IdNotificacion)
                                        && fav.IdUsuario == IdUsuario
                                  select fav).ToList();


                foreach (var itemNoti in listaNoti)
                {

                    var itemStatus = listaStatus.FirstOrDefault(x => x.IdNotificacion != itemNoti.IdNotificacion);
                    var itemFav = listaFavoritos.FirstOrDefault(x => x.IdNotificacion != itemNoti.IdNotificacion);

                    itemNoti.Status = itemStatus?.Status ?? 1;
                    itemNoti.Favorita = itemFav != null;

                }

                return listaNoti;
            }

            //return (from n in ctx.Notificaciones
            //        join t in ctx.C_NotificacionTipo on n.IdTipo equals t.IdTipo
            //        join su in ctx.SYA_Usuarios on n.UsuarioReg equals su.IdUsuario
            //        join cli in ctx.Cliente on n.IdCliente equals cli.IdCliente
            //        join suc in ctx.Sucursal on cli.IdCliente equals suc.IdCliente
            //        let fav = ctx.Notificaciones_Favoritas
            //                .FirstOrDefault(x => x.IdNotificacion == n.IdNotificacion && x.IdUsuario == IdUsuario)
            //        let nu = ctx.Notificacion_Status
            //                .Where(x => x.IdNotificacion == n.IdNotificacion && x.IdUsuario == IdUsuario)
            //                .OrderByDescending(x => x.Fecha)
            //                .FirstOrDefault()
            //        select new NotificacionDatos
            //        {
            //            IdNotificacion = n.IdNotificacion,
            //            Titulo = n.Titulo,
            //            Tipo = n.IdTipo,
            //            TipoDescripcion = t.Descripcion,
            //            IdUsuario = n.UsuarioReg,
            //            Usuario = su.Nombres + " " + su.ApPaterno,
            //            Contenido = n.Cuerpo,
            //            Fecha = n.Fecha,
            //            Favorita = fav == null ? false : true,
            //            Cliente = suc.Ciudad,
            //            Status = nu == null ? 1 : nu.Status
            //        }).ToList();



        }

        public List<NotificacionDatos> GetBandejaEntrada(SYA_Usuarios user)
        {
            var lista = GetNotifications2(user);
            return lista?.Where(x => x.Status != 3).ToList();
        }

        public List<NotificacionDatos> getNuevas(SYA_Usuarios user)
        {
            var lista = GetNotifications2(user);
            return lista?.Where(x => x.Status == 1 || x.Status == null).ToList();
        }

        public List<NotificacionDatos> GetNotificationsArchivadas(SYA_Usuarios user)
        {
            var lista = GetNotifications2(user);
            return lista?.Where(x => x.Status == 3).ToList();
        }

        public List<NotificacionDatos> GetNotificationsByStatus(int status, SYA_Usuarios user)
        {
            var IdUser = SessionHelpers.GetIdUsuario();
            if (status == 2)
            {
                return GetNotifications2(user).Where(x => x.Status == 2 || x.Status == null).ToList();
            }
            else
            {
                return GetNotifications2(user).Where(x => x.Status == status).ToList();
            }
        }

        public List<NotificacionDatos> GetNotificationsByType(int type, SYA_Usuarios user)
        {
            return GetNotifications2(user).Where(x => x.Tipo == type).ToList();
        }

        public NotificacionDatos GetNotificationDetails(int id)
        {
            var user = ControlAcceso.GetUsuarioEnSession();
            return (from n in ctx.Notificaciones
                    join t in ctx.C_NotificacionTipo on n.IdTipo equals t.IdTipo
                    join su in ctx.SYA_Usuarios on n.UsuarioReg equals su.IdUsuario
                    join cli in ctx.Cliente on n.IdCliente equals cli.IdCliente
                    let f = ctx.Notificaciones_Favoritas.Where(x => x.IdNotificacion == n.IdNotificacion).FirstOrDefault()
                    let ns = ctx.Notificacion_Status.Where(x => x.IdNotificacion == n.IdNotificacion && x.IdUsuario == user.IdUsuario).OrderByDescending(x => x.Id).FirstOrDefault()
                    where n.IdNotificacion == id
                    select new NotificacionDatos
                    {
                        IdNotificacion = n.IdNotificacion,
                        Usuario = su.Nombres + " " + su.ApPaterno,
                        Titulo = n.Titulo,
                        Contenido = n.Cuerpo,
                        Fecha = n.Fecha,
                        Cliente = cli.Nombre,
                        TipoDescripcion = t.Descripcion,
                        Favorita = f != null ? true : false,
                        Status = ns == null ? 1 : ns.Status
                    }
                    ).FirstOrDefault();
        }

        public bool AddFavorite(int id)
        {
            var user = ControlAcceso.GetUsuarioEnSession();
            //comprueba que no exista ya una notificación favorita
            var old = ctx.Notificaciones_Favoritas.Where(x => x.IdNotificacion == id && x.IdUsuario == user.IdUsuario).FirstOrDefault();
            if (old != null) return false;
            Notificaciones_Favoritas fav = new Notificaciones_Favoritas();
            fav.IdNotificacion = id;
            fav.IdUsuario = SessionHelpers.GetIdUsuario();
            ctx.Notificaciones_Favoritas.Add(fav);
            var status = ctx.SaveChanges();
            return status > 0 ? true : false;
        }

        public bool DeleteFavorite(int id)
        {
            var user = ControlAcceso.GetUsuarioEnSession();
            var fav = ctx.Notificaciones_Favoritas.Where(x => x.IdNotificacion == id && x.IdUsuario == user.IdUsuario).FirstOrDefault();
            if (fav != null)
            {
                ctx.Notificaciones_Favoritas.Remove(fav);
                var status = ctx.SaveChanges();
                return status > 0 ? true : false;
            }
            return false;
        }

        public List<ComentariosDatos> GetNotificationComments(int id)
        {
            var list = (from n in ctx.Notificacion_Comentarios
                        join us in ctx.SYA_Usuarios on n.IdUsuario equals us.IdUsuario
                        where n.IdNotificacion == id
                        select new ComentariosDatos
                        {
                            IdComentario = n.Id,
                            IdUsuario = n.IdUsuario,
                            Comentario = n.Comentario,
                            Fecha = n.Fecha,
                            Usuario = us.Nombres + " " + us.ApPaterno,
                        }
                    )
                    .OrderBy(x => x.Fecha)
                    .ToList();
            foreach (var item in list)
            {
                item.ProfPic = ControlUsuario.GetProfilePictureOfUser(item.IdUsuario);
            }
            return list;
        }

        public string GetNotificationBody(int id)
        {
            return ctx.Notificaciones.Where(x => x.IdNotificacion == id).Select(x => x.Cuerpo).FirstOrDefault();
        }

        public int AddComment(Notificacion_Comentarios comment)
        {
            comment.IdUsuario = SessionHelpers.GetIdUsuario();
            comment.Fecha = DateTime.Now;
            ctx.Notificacion_Comentarios.Add(comment);
            var status = ctx.SaveChanges();
            return comment.Id;
        }

        public ComentariosDatos GetComment(int id)
        {
            var comment = (from n in ctx.Notificacion_Comentarios
                           join us in ctx.SYA_Usuarios on n.IdUsuario equals us.IdUsuario
                           where n.Id == id
                           select new ComentariosDatos
                           {
                               IdComentario = n.Id,
                               IdUsuario = n.IdUsuario,
                               Usuario = us.Nombres,
                               Fecha = n.Fecha,
                               Comentario = n.Comentario,

                           }
                    ).FirstOrDefault();
            comment.ProfPic = ControlUsuario.GetProfilePictureOfUser(comment.IdUsuario);
            return comment;
        }

        public bool CountNewNotifications(int id)
        {
            var not = ctx.Notificacion_Status.Where(x => x.IdNotificacion == id).ToList();
            //si hay registros significa que no es nueva
            return not.Count > 0 ? true : false;
        }

        public bool MarcarComoLeida(int id)
        {
            //var idUser = SessionHelpers.GetIdUsuario();
            //ControlUsuario cu = new ControlUsuario();
            //var user = cu.GetUsuarioById(idUser);

            //if (user == null) return false;

            ////var not = new Notificacion_Status();
            ////not.IdNotificacion = id;
            ////not.IdUsuario = idUser;
            ////not.Fecha = DateTime.Now;
            ////not.Status = (int)StatusNotificaciones.Leida;
            ////ctx.Notificacion_Status.Add(not);
            ////var status = ctx.SaveChanges();
            //return status > 0 ? true : false;
            return true;
        }

        public bool ArchivarNotificacion(int id)
        {
            var idUser = SessionHelpers.GetIdUsuario();

            if (idUser <= 0) return false;

            var old = ctx.Notificacion_Status.Where(x => x.IdNotificacion == id && x.IdUsuario == idUser).OrderByDescending(x => x.Id).FirstOrDefault();
            if (old != null) if (old.Status == 3) return false;

            var not = new Notificacion_Status();
            not.IdNotificacion = id;
            not.IdUsuario = idUser;
            not.Fecha = DateTime.Now;
            not.Status = (int)StatusNotificaciones.Archivada;
            ctx.Notificacion_Status.Add(not);
            var status = ctx.SaveChanges();
            return status > 0 ? true : false;
        }

        public bool DesarchivarNotificacion(int id)
        {
            var idUser = SessionHelpers.GetIdUsuario();


            if (idUser <= 0) return false;
            var old = getLastStatus(id, idUser);
            if (old != null) if (old.Status == 2) return false;

            var not = new Notificacion_Status();
            not.IdNotificacion = id;
            not.IdUsuario = idUser;
            not.Fecha = DateTime.Now;
            not.Status = (int)StatusNotificaciones.Leida;
            ctx.Notificacion_Status.Add(not);
            var status = ctx.SaveChanges();
            return status > 0 ? true : false;
        }

        public Notificacion_Status getLastStatus(int IdNotificacion, int IdUser)
        {
            return ctx.Notificacion_Status.Where(x => x.IdNotificacion == IdNotificacion && x.IdUsuario == IdUser).OrderByDescending(x => x.Id).FirstOrDefault();
        }
    }

    public class NotificacionDatos
    {
        public int IdNotificacion { get; set; }
        public byte[] imageBytes { get; set; }
        public string image { get; set; }
        public string Titulo { get; set; }
        public string Usuario { get; set; }
        public int IdUsuario { get; set; }
        public string UserPicture { get; set; }
        public DateTime Fecha { get; set; }
        public string FechaString { get; set; }
        public string Cliente { get; set; }
        public string Empleado { get; set; }
        public int Tipo { get; set; }
        public string TipoDescripcion { get; set; }
        public string Contenido { get; set; }
        public int? Status { get; set; }
        public bool Favorita { get; set; }
        public List<string> Datos { get; set; }
        public List<ComentariosDatos> Comentarios { get; set; }
        public string EmpresaCliente { get; set; }
    }

    public class ComentariosDatos
    {
        public int IdComentario { get; set; }
        public int IdUsuario { get; set; }
        public string Usuario { get; set; }
        public DateTime Fecha { get; set; }
        public string Comentario { get; set; }
        public string ProfPic { get; set; }
    }
}