namespace Nomina.Procesador.Modelos
{
   public class PathsCertificado
    {
        public string PathArchivoCer { get; set; }
        public string PathArchivoKey { get; set; }
        public string NombreEmpresa { get; set; }
        public string PathArchivoXmlGenerado { get; set; }
        public string XmlStrSinTimbre { get; set; }
        public string XmlStrConTimbre { get; set; }
        public string Password { get; set; }
        public string CadenaOriginal { get; set; }
        public string TimbreRespuesta { get; set; }

        public string Version { get; set; }
        public string FechaTimbrado { get; set; }
        public string SelloCfd { get; set; }
        public string NoCertificadoSat { get; set; }
        public string SelloSat { get; set; }
        public string Uuid { get; set; }
    }
}
