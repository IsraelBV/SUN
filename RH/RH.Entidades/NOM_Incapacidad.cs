//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RH.Entidades
{
    using System;
    using System.Collections.Generic;
    
    public partial class NOM_Incapacidad
    {
        public int Id { get; set; }
        public int IdNomina { get; set; }
        public int IdFiniquito { get; set; }
        public int IdTipoIncapacidad { get; set; }
        public int DiasIncapacidad { get; set; }
        public decimal Importe { get; set; }
    }
}
