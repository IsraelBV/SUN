using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nomina.WEB.Models
{
    public class LogIn
    {
        public string username { get; set; }
        public string password { get; set; }
        public bool Acceso { get; set; }
        public string nombre { get; set; }
        public string foto { get; set; }
        public int Error { get; set; }
    }
}