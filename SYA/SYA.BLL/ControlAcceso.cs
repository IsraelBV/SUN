using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;
using Common.Enums;
using Common.Helpers;
namespace SYA.BLL
{
    public class ControlAcceso
    {
        /// <summary>
        /// Metodo que obtiene el Usuario con el Id de la session activa
        /// </summary>
        /// <returns></returns>
        public static SYA_Usuarios GetUsuarioEnSession()
        {
            ControlUsuario cu = new ControlUsuario();

            var usuario = cu.GetUsuarioById(SessionHelpers.GetIdUsuario());

            return usuario;
        }

        public static bool AccesoFiltroNotificacion(TiposNotificacion tn, SYA_Usuarios user)
        {
            if (user == null) return false;

            if (user.IdPerfil == 1) return true;

            var noti = (int)tn;

            var ctx = new RHEntities();
            var array = ctx.Notificacion_Departamento.Where(x => x.IdDepartamento == user.IdDepartamento).Select(x => x.IdTipo).ToList();

            if (array.Contains(noti)) return true;
            else return false;
        }


        /// <summary>
        /// Metodo que valida si el usuario tiene configurada la aplicacion
        /// </summary>
        /// <param name="app"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool AccesoAplicacion(Aplicacion app, SYA_Usuarios user)
        {
            var result = false;

            if (user == null) return false;

            //Si el usuario tiene perfil SU se le concede acceso inmediato
            if (user.IdPerfil == 1)
                return true;

            //obtenemos el valor numerico del enumerado de aplicacion
            var idAplicacion = (int)app;

            //pasamos a un array las aplicaciones que tiene configurado el usuario
            var array = user.Aplicaciones.Split(',');

            if (array.Length == 1)
            {
                if (array[0].Trim() == "*")
                    return true;
            }

            //buscamos si la aplicacion esta dentro de la lista de aplicaciones
            // del usuario
            var encontrado = (from elemento in array
                              where elemento == idAplicacion.ToString()
                              select elemento).Count();

            //si esta disponible
            if (encontrado > 0)
            {
                result = true;
            }

            return result;

        }

        public static bool AccesoAplicacion(Aplicacion app, int IdUser)
        {
            var result = false;

            ControlUsuario cu = new ControlUsuario();
            var user = cu.GetUsuarioById(IdUser);

            //Si el usuario tiene perfil SU se le concede acceso inmediato
            if (user.IdPerfil == 1)
                return true;

            //obtenemos el valor numerico del enumerado de aplicacion
            var idAplicacion = (int)app;

            //pasamos a un array las aplicaciones que tiene configurado el usuario
            var array = user.Aplicaciones.Split(',');

            //buscamos si la aplicacion esta dentro de la lista de aplicaciones
            // del usuario
            var encontrado = (from elemento in array
                              where elemento == idAplicacion.ToString()
                              select elemento).Count();

            //si esta disponible
            if (encontrado > 0)
            {
                result = true;
            }

            return result;
        }


        /// <summary>
        /// Metodo para validar si el usuario tiene acceso al modulo
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public static bool AccesoModulo(Modulos mod, int idUser)
        {
            var result = false;

            var user = GetUsuarioEnSession();
            if (user != null)
                if (user.IdPerfil == 1) return true;

            ControlUsuario cu = new ControlUsuario();

            if (idUser <= 0) return false;

            //numero del enumerador
            var idMod = (int)mod;

            //buscamos si el modulo esta asignado al usuario
            var listaModulos = cu.GetModulosByUsuario(idUser);

            var encontrado = (from elemento in listaModulos
                              where elemento.IdModulo == idMod
                              select elemento).Count();

            //si fue encontrado
            if (encontrado > 0)
            {
                result = true;
            }

            return result;

        }



        /// <summary>
        /// Validar si tiene permiso para ejecutar la accion, consulta bd.
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="accion"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public static bool ValidarEjecutarAccion(Modulos mod, AccionCrud accion, int idUser)
        {
            var result = false;
            var cu = new ControlUsuario();

            if (idUser <= 0) return false;

            var user = GetUsuarioEnSession();
            if (user != null)
                if (user.IdPerfil == 1) return true;

            //numero del modulo
            var idMod = (int)mod;
            //var numAccion = (int)accion;

            //buscamos si el modulo esta asignado al usuario
            var reg = cu.GetModuloUsuarioById(idMod, idUser);

            if (reg != null)
            {
                result = ValidarAccion(accion, reg.Acciones);
            }

            return result;

        }



        /// <summary>
        /// Validar si tiene permiso para ejecutar la accion, sin consultar bd.
        /// </summary>
        /// <param name="accion"></param>
        /// <param name="arrayAcciones"></param>
        /// <returns></returns>
        public static bool ValidarAccion(AccionCrud accion, string arrayAcciones)
        {
            var result = false;

            //numero de la accion en el enumerador
            int numAccion = (int)accion;

            if (!string.IsNullOrEmpty(arrayAcciones))
            {
                var array = arrayAcciones.Split(',');

                var permitido = (from elemento in array
                                 where elemento == numAccion.ToString()
                                 select elemento).Count();

                if (permitido > 0)
                {
                    result = true;
                }
            }

            return result;


        }



        public static bool AccesoAccionModulo(Modulos mod, AccionCrud accion, int idUser)
        {

            // var cu = new ControlUsuario();

            if (idUser <= 0) return false;

            //numero del enumerador
            //var idMod = (int)mod;

            //buscamos si el usuario tiene acceso al modulo y la accion
            return ValidarEjecutarAccion(mod, accion, idUser);

        }


        /// <summary>
        /// Metodo para validar si el usuario tiene acceso a la sucursal idSucursal
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public static bool AccesoSucursal(int idSucursal)
        {

            var user = ControlAcceso.GetUsuarioEnSession();
            if (user == null) return false;

            if (user.IdPerfil == 1)
                return true;

            var cu = new ControlUsuario();
            var result = false;

            var usuario = cu.GetUsuarioById(user.IdUsuario);


            if (usuario == null) return false;
            if (usuario.Sucursales.Trim() == "*")
            {
                return true;
            }

            var arraySucursales = usuario.Sucursales.Split(',');

            var encontrado = (from elemento in arraySucursales
                              where elemento == idSucursal.ToString()
                              select elemento).Count();

            if (encontrado > 0)
                result = true;


            return result;


        }


        public static SYA_Usuarios UsuarioValido(string cuenta, string password)
        {
            var cu = new ControlUsuario();
            var usuario = cu.GetUsuarioByCuenta(cuenta, password);

            if (cuenta.Equals(usuario.Usuario) && password.Equals(usuario.Contraseña))
            {
                return usuario;
            }

            return null;
        }
    }
}
