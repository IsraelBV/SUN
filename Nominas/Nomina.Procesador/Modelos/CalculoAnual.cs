using RH.Entidades;
using System.Collections.Generic;
namespace Nomina.Procesador.Modelos
{
  public  class CalculoAnual
    {
      public decimal BaseGravable { get; set; }
        public decimal Exento { get; set; }
        public decimal Cuotafija { get; set; }
      public decimal LimiteInferior { get; set; }
      public decimal Base { get; set; }
      public decimal Tasa { get; set; }
      public decimal IsrAntesDeSub { get; set; }
      public decimal Subsidio { get; set; }
      public decimal ResultadoIsrOSubsidio { get; set; }
      public decimal marginal { get; set; }
      public decimal isrPagado { get; set; }
    }
}
