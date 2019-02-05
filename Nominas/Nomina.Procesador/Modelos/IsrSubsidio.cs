using RH.Entidades;
using System.Collections.Generic;
namespace Nomina.Procesador.Modelos
{
  public  class IsrSubsidio
    {
      public decimal BaseGravable { get; set; }
      public decimal BaseGravableMensual { get; set; }
      public decimal LimiteInferior { get; set; }
      public decimal Base { get; set; }
      public decimal Tasa { get; set; }
      public decimal IsrAntesDeSub { get; set; }
      public decimal Subsidio { get; set; }
      public decimal ResultadoIsrOSubsidio { get; set; }
      public decimal NetoAPagar { get; set; }
      public int IdTablaIsr { get; set; }
      public int IdTablaSubsidio { get; set; }
      public bool EsISR { get; set; }

        public List<C_NOM_Tabla_ISR> Tablaisr { get; set; }
        public List<C_NOM_Tabla_Subsidio> Tablasubsidio { get; set; }
    }
}
