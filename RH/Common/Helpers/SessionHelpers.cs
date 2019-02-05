using System;
using System.Web;
using System.Web.Security;

namespace Common.Helpers
{
   public class SessionHelpers
    {
        /// <summary>
        /// Quita el ticket de auntentificacioin del explorador
        /// </summary>
        public static void CerrarSession()
        {
            FormsAuthentication.SignOut();
        }

        /// <summary>
        /// Metodo para indicar si el usuario esta autenticado
        /// </summary>
        /// <returns></returns>
       public static bool IsAutenticado()
       {
           return HttpContext.Current.User.Identity.IsAuthenticated;
       }

        /// <summary>
        /// Retorna el Id del usuario de la session activa
        /// </summary>
        /// <returns></returns>
       public static int GetIdUsuario()
       {
            var idUsuario = 0;

           if (HttpContext.Current.User == null || !(HttpContext.Current.User.Identity is FormsIdentity))
               return idUsuario;

           FormsAuthenticationTicket ticket = ((FormsIdentity)HttpContext.Current.User.Identity).Ticket;

           if (ticket != null)
           {
               idUsuario = Convert.ToInt32(ticket.UserData);
           }

           return idUsuario;
        } 

        /// <summary>
        /// Agrega el Id del usuario a la Session
        /// </summary>
        /// <param name="nombreUsuario"></param>
        /// <param name="idUsuario"></param>
       public static void IniciarSession(string nombreUsuario, string idUsuario)
       {
            var cookieUsuario = FormsAuthentication.GetAuthCookie(nombreUsuario, true);

            cookieUsuario.Name = FormsAuthentication.FormsCookieName;
            cookieUsuario.Expires = DateTime.Now.AddDays(7);

            var ticket = FormsAuthentication.Decrypt(cookieUsuario.Value);
        
            var nuevoTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration, ticket.IsPersistent, idUsuario);

            cookieUsuario.Value = FormsAuthentication.Encrypt(nuevoTicket);

            HttpContext.Current.Response.Cookies.Add(cookieUsuario);
        }
    }
}
