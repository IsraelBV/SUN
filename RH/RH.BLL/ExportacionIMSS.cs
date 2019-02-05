using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RH.BLL
{
    public class ExportacionIMSS
    {

        public bool Write(string[] lines, string nombre)
        {
            try
            {
                System.IO.File.WriteAllLines(@"C:\\sites/RH/trunk/AlianzaCorp/SS.WEB/Files/" + nombre, lines);
                return true;
            }
            catch(System.IO.IOException)
            {
                return false;
            }
        }

        public int IDSEReingreso(int[] empleados, int IdEmpresa, string nombreArchivo)
        {
            Empleados emp = new Empleados();
            Empresas empr = new Empresas();
            List<string> datos = new List<string>();
            var empresa = empr.GetEmpresaById(IdEmpresa);
            foreach(var item in empleados)
            {
                var empleado = emp.GetEmpleadoById(item);
                var contrato = emp.ObtenerContratoEmpleadoPorId(item);
                //var nomina = emp.ObtenerEmpleadoNominaPorId(item);
                var rp = FormarCampo(empresa.RegistroPatronal, 11, '0', 1);

                var linea = rp;

                var nss = FormarCampo(empleado.NSS, 11, '0', 1);
                linea += empleado.NSS;

                var paterno = FormarCampo(empleado.APaterno, 27, ' ', 1);
                linea += paterno;

                var materno = FormarCampo(empleado.AMaterno, 27, ' ', 1);
                linea += materno;

                var nombre = FormarCampo(empleado.Nombres, 27, ' ', 1);
                linea += nombre;

                //string sdi = nomina.SDI.ToString();
                //remueve el punto
                //sdi = sdi.Replace(".", string.Empty);
                //sdi = FormarCampo(sdi, 6, '0', 2);
                //linea += sdi;

                var SalarioInfonavit = "000000";
                linea += SalarioInfonavit;

                var TipoTrabajador = contrato.TipoContrato.ToString();
                linea += TipoTrabajador;
                

                var TipoSalario = "0";
                linea += TipoSalario;

                var semana = "0";
                linea += semana;

                var fecha = contrato.FechaIMSS.Value.ToString("ddMMyyyy");
                linea += fecha;

                var umf = FormarCampo(contrato.UMF, 3, '0', 1);
                linea += umf;

                linea += "  ";

                linea += "08";

                linea += FormarCampo(empresa.Guia, 5, '0', 1);

                linea += FormarCampo("", 11, ' ', 1);

                var curp = FormarCampo(empleado.CURP, 18, '0', 1);
                linea += curp;

                linea += "9";
                if(linea.Length == 168)
                    datos.Add(linea);
                
            }


            if(datos.Count > 0)
            {
                string lineaFinal = LineaFinal(datos.Count, empresa.Guia);
                
                if(lineaFinal.Length == 168)
                    datos.Add(lineaFinal);

                var status = Write(datos.ToArray(), nombreArchivo);

                if (!status)
                    return -1;
            }

            return datos.Count - 1;
        }

        public int IDSEModificacionSalario(int[] empleados, int IdEmpresa, string nombreArchivo)
        {
            Empleados emp = new Empleados();
            Empresas empr = new Empresas();
            List<string> datos = new List<string>();
            var empresa = empr.GetEmpresaById(IdEmpresa);
            foreach(var item in empleados)
            {
                var empleado = emp.GetEmpleadoById(item);
                var contrato = emp.ObtenerContratoEmpleadoPorId(item);
                //var nomina = emp.ObtenerEmpleadoNominaPorId(item);
                var rp = FormarCampo(empresa.RegistroPatronal, 11, '0', 1);

                var linea = rp;
                var nss = FormarCampo(empleado.NSS, 11, '0', 1);
                linea += nss;

                var paterno = FormarCampo(empleado.APaterno, 27, ' ', 1);
                linea += paterno;

                var materno = FormarCampo(empleado.AMaterno, 27, ' ', 1);
                linea += materno;

                var nombres = FormarCampo(empleado.Nombres, 27, ' ', 1);
                linea += nombres;

                //string sdi = nomina.SDI.ToString();
                //remueve el punto
                //sdi = sdi.Replace(".", string.Empty);
                //sdi = FormarCampo(sdi, 6, '0', 2);
                //linea += sdi;

                var SalarioInfonavit = FormarCampo("", 6, '0', 1);
                linea += SalarioInfonavit;

                var TipoTrabajador = contrato.TipoContrato.ToString();
                linea += TipoTrabajador;

                var TipoSalario = "0";
                linea += TipoSalario;

                var semana = "0";
                linea += semana;

                var fecha = "00000000";
                if (contrato.FechaIMSS != null)
                    fecha = contrato.FechaIMSS.Value.ToString("ddMMyyyy");
                linea += fecha;

                linea += "     07";

                linea += FormarCampo(empresa.Guia, 5, '0', 1);

                linea += FormarCampo("", 11, ' ', 1);

                var curp = FormarCampo(empleado.CURP, 18, '0', 1);
                linea += curp;

                linea += "9";
                if (linea.Length == 168)
                    datos.Add(linea);
            }

            if (datos.Count > 0)
            {
                string lineaFinal = LineaFinal(datos.Count, empresa.Guia);

                if (lineaFinal.Length == 168)
                    datos.Add(lineaFinal);

                var status = Write(datos.ToArray(), nombreArchivo);

                if (!status)
                    return -1;
            }

            return datos.Count - 1;
        }

        private string LineaFinal(int numDatos, string guia)
        {
            string lineaFinal = FormarCampo("*************", 56, ' ', 1);
            lineaFinal += FormarCampo(numDatos.ToString(), 6, '0', 2);
            lineaFinal += FormarCampo("", 71, ' ', 1);
            lineaFinal += FormarCampo(guia, 34, ' ', 1);
            lineaFinal += "9";
            return lineaFinal;
        }

        private string FormarCampo(string text, int longitud, char relleno, int lado)
        {
            if(text == null)
            {
                return "";
            }

            if(text.Length < longitud)
            {
                while(text.Length != longitud)
                {
                    if (lado == 1)
                        text += relleno;
                    else
                        text = relleno + text;
                }
            }
            else if(text.Length > longitud)
            {
                text.Remove(longitud, text.Length);
            }
            return text;
        }
    }
}
