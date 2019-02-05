using System.Collections.Generic;
using System.Linq;
using RH.Entidades;
using Common.Helpers;

namespace RH.BLL
{
    public class ControlUsuarioxx
    {
        readonly RHEntities _ctx;

        public ControlUsuarioxx()
        {
            _ctx = new RHEntities();
        }

        public byte[] GetPicture(int IdUser)
        {
            return _ctx.SYA_Usuarios.Where(x => x.IdUsuario == IdUser).Select(x => x.Fotografia).FirstOrDefault();
        }

        /// <summary>
        /// Actualiza la foto de perfil del usuario
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        public bool UploadPicture(byte[] photo)
        {
            var IdUser = SessionHelpers.GetIdUsuario();
            var user = _ctx.SYA_Usuarios.Where(x => x.IdUsuario == IdUser).FirstOrDefault();
            user.Fotografia = photo;
            var response = _ctx.SaveChanges();
            return response > 0 ? true : false;
        }


        /// <summary>
        /// Retorna un usuario, usando su clave y password
        /// </summary>
        /// <param name="cuenta"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public SYA_Usuarios GetUsuarioByCuenta(string cuenta, string password)
        {
            var usuario = _ctx.SYA_Usuarios.FirstOrDefault(x => x.Usuario == cuenta && x.Contraseña == password);

            return usuario;
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
            var lista = _ctx.SYA_Usuarios_Modulos.Where(x => x.IdUsuario == idUser).ToList();

            return lista;
        }

        /// <summary>
        /// Retorna un registros de confiuracion de modulos asignado al usuario
        /// </summary>
        /// <param name="idMod"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public SYA_Usuarios_Modulos GetModuloUsuarioById(int idMod, int idUser)
        {
            var reg = _ctx.SYA_Usuarios_Modulos.FirstOrDefault(x => x.IdModulo == idMod && x.IdUsuario == idUser);

            return reg;
        }
    }
}
