using RH.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Helpers;

namespace SYA.BLL
{

    public class ControlUsuario
    {
        // readonly RHEntities _ctx;

        //Metodo Constructor
        public ControlUsuario()
        {
            //    _ctx = new RHEntities();
        }

        /// <summary>
        /// Obtiene la foto de perfil de un usuario
        /// </summary>
        /// <param name="IdUser"></param>
        /// <returns></returns>
        public string GetProfilePicture(int IdUser)
        {
            using (var context = new RHEntities())
            {
                var bytes =
                    context.SYA_Usuarios.Where(x => x.IdUsuario == IdUser).Select(x => x.Fotografia).FirstOrDefault();
                if (bytes != null)
                {
                    var pic = Convert.ToBase64String(bytes);
                    return $"data:image/jpg; base64, {pic}";
                }
                else return "../Images/profpic.jpg";
            }
        }

        public static string GetProfilePictureOfUser(int IdUser)
        {
            ControlUsuario cu = new ControlUsuario();
            return cu.GetProfilePicture(IdUser);
        }

        /// <summary>
        /// Obtiene la foto de perfil del usuario en sesión
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentUserProfilePicture()
        {
            var IdUser = SessionHelpers.GetIdUsuario();
            ControlUsuario cu = new ControlUsuario();
            return cu.GetProfilePicture(IdUser);
        }

        public void UploadProfilePicture(byte[] photo)
        {
            using (var conetxt = new RHEntities())
            {
                var IdUser = SessionHelpers.GetIdUsuario();
                var user = conetxt.SYA_Usuarios.FirstOrDefault(x => x.IdUsuario == IdUser);
                if (user != null)
                {
                    user.Fotografia = photo;
                    conetxt.SaveChanges();
                }
            }

        }

        /// <summary>
        /// Retorna un usuario, usando su clave y password
        /// </summary>
        /// <param name="cuenta"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public SYA_Usuarios GetUsuarioByCuenta(string cuenta, string password)
        {
            using (var context = new RHEntities())
            {
                var usuario = context.SYA_Usuarios.FirstOrDefault(x => x.Usuario == cuenta && x.Contraseña == password);

                return usuario;
            }
        }

        public SYA_Usuarios UserNameExists(string username)
        {
            try
            {
                using (var context = new RHEntities())
                {
                    return context.SYA_Usuarios.FirstOrDefault(x => x.Usuario == username);
                }
            }
            catch (Exception ex)
            {

                throw new InvalidOperationException("No se estableció conexion con el servidor de BD");
            }

        }

        /// <summary>
        /// Retorna un usuario, buscando por su id
        /// </summary>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public SYA_Usuarios GetUsuarioById(int idUser)
        {
            using (var context = new RHEntities())
            {
                var usuario = context.SYA_Usuarios.FirstOrDefault(x => x.IdUsuario == idUser);

                return usuario;
            }
        }


        /// <summary>
        /// Retorna una lista de modulos que tiene configurado el usuario
        /// </summary>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public List<SYA_Usuarios_Modulos> GetModulosByUsuario(int idUser)
        {
            using (var context = new RHEntities())
            {
                var lista = context.SYA_Usuarios_Modulos.Where(x => x.IdUsuario == idUser).ToList();

                return lista;
            }
        }

        /// <summary>
        /// Retorna un registros de confiuracion de modulos asignado al usuario
        /// </summary>
        /// <param name="idMod"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public SYA_Usuarios_Modulos GetModuloUsuarioById(int idMod, int idUser)
        {
            using (var context = new RHEntities())
            {
                var reg = context.SYA_Usuarios_Modulos.FirstOrDefault(x => x.IdModulo == idMod && x.IdUsuario == idUser);

                return reg;
            }
        }

        /// <summary>
        /// Actualiza la foto de perfil del usuario
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        public bool UploadPicture(byte[] photo)
        {
            using (var context = new RHEntities())
            {
                var IdUser = SessionHelpers.GetIdUsuario();
                var user = context.SYA_Usuarios.FirstOrDefault(x => x.IdUsuario == IdUser);
                user.Fotografia = photo;
                var response = context.SaveChanges();
                return response > 0 ? true : false;
            }
        }
        public int[] GetSucursalesUsuario(SYA_Usuarios user)
        {
            int[] sucursales = null;
            using (var context = new RHEntities())
            {
                if (user.Sucursales == "*")
                {
                    sucursales = context.Sucursal.Select(x => x.IdSucursal).ToArray();
                }
                else
                {
                    //Obtiene las sucursales a los que tiene acceso el usuario
                    var sucursalesAcceso = user.Sucursales.Split(',');
                    sucursales = sucursalesAcceso.Select(int.Parse).ToArray();
                }
            }
            return sucursales;
        }

        public List<Sucursal> GetSucusalesByIdUsuario(int idUsuario)
        {
            List<Sucursal> lista = new List<Sucursal>();
            using (var context = new RHEntities())
            {
                var itemUsuario = context.SYA_Usuarios.FirstOrDefault(x => x.IdUsuario == idUsuario);

                if (itemUsuario == null) return null;

                if (itemUsuario.Sucursales == "*")
                {
                    lista = context.Sucursal.ToList();
                }
                else
                {
                    var sucursalesAcceso = itemUsuario.Sucursales.Split(',');
                    var arraySucursales = sucursalesAcceso.Select(int.Parse).ToArray();
                    lista = (from s in context.Sucursal
                             where arraySucursales.Contains(s.IdSucursal)
                             select s).ToList();
                }
            }

            return lista;
        }
    }
}
