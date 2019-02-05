using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Nomina.Procesador.Modelos.Nomina12
{
    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]/*http://www.sat.gob.mx/nomina*/
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.sat.gob.mx/nomina12", IsNullable = false)]/*http://www.sat.gob.mx/nomina*/
    public partial class Nomina
    {

        private NominaEmisor emisorField;

        private NominaReceptor receptorField;

        private NominaPercepciones percepcionesField;

        private NominaDeducciones deduccionesField;

        private NominaOtroPago[] otrosPagosField;

        private NominaIncapacidad[] incapacidadesField;

        private string versionField;

        private string tipoNominaField;

        private System.DateTime fechaPagoField;

        private System.DateTime fechaInicialPagoField;

        private System.DateTime fechaFinalPagoField;

        private decimal numDiasPagadosField;

        private decimal totalPercepcionesField;

        private bool totalPercepcionesFieldSpecified;

        private decimal totalDeduccionesField;

        private bool totalDeduccionesFieldSpecified;

        private decimal totalOtrosPagosField;

        private bool totalOtrosPagosFieldSpecified;

        private string schemaLocationFieldSpecified;
        public Nomina()
        {
            this.versionField = "1.2";
        }

        /// <comentarios/>
        public NominaEmisor Emisor
        {
            get
            {
                return this.emisorField;
            }
            set
            {
                this.emisorField = value;
            }
        }

        /// <comentarios/>
        public NominaReceptor Receptor
        {
            get
            {
                return this.receptorField;
            }
            set
            {
                this.receptorField = value;
            }
        }

        /// <comentarios/>
        public NominaPercepciones Percepciones
        {
            get
            {
                return this.percepcionesField;
            }
            set
            {
                this.percepcionesField = value;
            }
        }

        /// <comentarios/>
        public NominaDeducciones Deducciones
        {
            get
            {
                return this.deduccionesField;
            }
            set
            {
                this.deduccionesField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlArrayItemAttribute("OtroPago", IsNullable = false)]
        public NominaOtroPago[] OtrosPagos
        {
            get
            {
                return this.otrosPagosField;
            }
            set
            {
                this.otrosPagosField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Incapacidad", IsNullable = false)]
        public NominaIncapacidad[] Incapacidades
        {
            get
            {
                return this.incapacidadesField;
            }
            set
            {
                this.incapacidadesField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TipoNomina
        {
            get
            {
                return this.tipoNominaField;
            }
            set
            {
                this.tipoNominaField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "date")]
        public System.DateTime FechaPago
        {
            get
            {
                return this.fechaPagoField;
            }
            set
            {
                this.fechaPagoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "date")]
        public System.DateTime FechaInicialPago
        {
            get
            {
                return this.fechaInicialPagoField;
            }
            set
            {
                this.fechaInicialPagoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "date")]
        public System.DateTime FechaFinalPago
        {
            get
            {
                return this.fechaFinalPagoField;
            }
            set
            {
                this.fechaFinalPagoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal NumDiasPagados
        {
            get
            {
                return this.numDiasPagadosField;
            }
            set
            {
                this.numDiasPagadosField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalPercepciones
        {
            get
            {
                return this.totalPercepcionesField;
            }
            set
            {
                this.totalPercepcionesField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TotalPercepcionesSpecified
        {
            get
            {
                return this.totalPercepcionesFieldSpecified;
            }
            set
            {
                this.totalPercepcionesFieldSpecified = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalDeducciones
        {
            get
            {
                return this.totalDeduccionesField;
            }
            set
            {
                this.totalDeduccionesField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TotalDeduccionesSpecified
        {
            get
            {
                return this.totalDeduccionesFieldSpecified;
            }
            set
            {
                this.totalDeduccionesFieldSpecified = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalOtrosPagos
        {
            get
            {
                return this.totalOtrosPagosField;
            }
            set
            {
                this.totalOtrosPagosField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TotalOtrosPagosSpecified
        {
            get
            {
                return this.totalOtrosPagosFieldSpecified;
            }
            set
            {
                this.totalOtrosPagosFieldSpecified = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string schemaLocation
        {
            get { return this.schemaLocationFieldSpecified; }
            set { this.schemaLocationFieldSpecified = value; }
        }
    
}

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
    public partial class NominaEmisor
    {

        private NominaEmisorEntidadSNCF entidadSNCFField;

        private string curpField;

        private string registroPatronalField;

        private string rfcPatronOrigenField;

        /// <comentarios/>
        public NominaEmisorEntidadSNCF EntidadSNCF
        {
            get
            {
                return this.entidadSNCFField;
            }
            set
            {
                this.entidadSNCFField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Curp
        {
            get
            {
                return this.curpField;
            }
            set
            {
                this.curpField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RegistroPatronal
        {
            get
            {
                return this.registroPatronalField;
            }
            set
            {
                this.registroPatronalField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RfcPatronOrigen
        {
            get
            {
                return this.rfcPatronOrigenField;
            }
            set
            {
                this.rfcPatronOrigenField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
    public partial class NominaEmisorEntidadSNCF
    {

        private string origenRecursoField;

        private decimal montoRecursoPropioField;

        private bool montoRecursoPropioFieldSpecified;

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string OrigenRecurso
        {
            get
            {
                return this.origenRecursoField;
            }
            set
            {
                this.origenRecursoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal MontoRecursoPropio
        {
            get
            {
                return this.montoRecursoPropioField;
            }
            set
            {
                this.montoRecursoPropioField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool MontoRecursoPropioSpecified
        {
            get
            {
                return this.montoRecursoPropioFieldSpecified;
            }
            set
            {
                this.montoRecursoPropioFieldSpecified = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
    public partial class NominaReceptor
    {

        private NominaReceptorSubContratacion[] subContratacionField;

        private string curpField;

        private string numSeguridadSocialField;

        private System.DateTime fechaInicioRelLaboralField;

        private bool fechaInicioRelLaboralFieldSpecified;

        private string antigüedadField;

        private string tipoContratoField;

        private NominaReceptorSindicalizado sindicalizadoField;

        private bool sindicalizadoFieldSpecified;

        private string tipoJornadaField;

        private string tipoRegimenField;

        private string numEmpleadoField;

        private string departamentoField;

        private string puestoField;

        private string riesgoPuestoField;

        private string periodicidadPagoField;

        private string bancoField;

        private string cuentaBancariaField;

        private bool cuentaBancariaFieldSpecified;

        private decimal salarioBaseCotAporField;

        private bool salarioBaseCotAporFieldSpecified;

        private decimal salarioDiarioIntegradoField;

        private bool salarioDiarioIntegradoFieldSpecified;

        private string claveEntFedField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute("SubContratacion")]
        public NominaReceptorSubContratacion[] SubContratacion
        {
            get
            {
                return this.subContratacionField;
            }
            set
            {
                this.subContratacionField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Curp
        {
            get
            {
                return this.curpField;
            }
            set
            {
                this.curpField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NumSeguridadSocial
        {
            get
            {
                return this.numSeguridadSocialField;
            }
            set
            {
                this.numSeguridadSocialField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "date")]
        public System.DateTime FechaInicioRelLaboral
        {
            get
            {
                return this.fechaInicioRelLaboralField;
            }
            set
            {
                this.fechaInicioRelLaboralField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool FechaInicioRelLaboralSpecified
        {
            get
            {
                return this.fechaInicioRelLaboralFieldSpecified;
            }
            set
            {
                this.fechaInicioRelLaboralFieldSpecified = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Antigüedad
        {
            get
            {
                return this.antigüedadField;
            }
            set
            {
                this.antigüedadField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TipoContrato
        {
            get
            {
                return this.tipoContratoField;
            }
            set
            {
                this.tipoContratoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public NominaReceptorSindicalizado Sindicalizado
        {
            get
            {
                return this.sindicalizadoField;
            }
            set
            {
                this.sindicalizadoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SindicalizadoSpecified
        {
            get
            {
                return this.sindicalizadoFieldSpecified;
            }
            set
            {
                this.sindicalizadoFieldSpecified = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TipoJornada
        {
            get
            {
                return this.tipoJornadaField;
            }
            set
            {
                this.tipoJornadaField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TipoRegimen
        {
            get
            {
                return this.tipoRegimenField;
            }
            set
            {
                this.tipoRegimenField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NumEmpleado
        {
            get
            {
                return this.numEmpleadoField;
            }
            set
            {
                this.numEmpleadoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Departamento
        {
            get
            {
                return this.departamentoField;
            }
            set
            {
                this.departamentoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Puesto
        {
            get
            {
                return this.puestoField;
            }
            set
            {
                this.puestoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RiesgoPuesto
        {
            get
            {
                return this.riesgoPuestoField;
            }
            set
            {
                this.riesgoPuestoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string PeriodicidadPago
        {
            get
            {
                return this.periodicidadPagoField;
            }
            set
            {
                this.periodicidadPagoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Banco
        {
            get
            {
                return this.bancoField;
            }
            set
            {
                this.bancoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CuentaBancaria
        {
            get
            {
                return this.cuentaBancariaField;
            }
            set
            {
                this.cuentaBancariaField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CuentaBancariaSpecified
        {
            get
            {
                return this.cuentaBancariaFieldSpecified;
            }
            set
            {
                this.cuentaBancariaFieldSpecified = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal SalarioBaseCotApor
        {
            get
            {
                return this.salarioBaseCotAporField;
            }
            set
            {
                this.salarioBaseCotAporField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SalarioBaseCotAporSpecified
        {
            get
            {
                return this.salarioBaseCotAporFieldSpecified;
            }
            set
            {
                this.salarioBaseCotAporFieldSpecified = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal SalarioDiarioIntegrado
        {
            get
            {
                return this.salarioDiarioIntegradoField;
            }
            set
            {
                this.salarioDiarioIntegradoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SalarioDiarioIntegradoSpecified
        {
            get
            {
                return this.salarioDiarioIntegradoFieldSpecified;
            }
            set
            {
                this.salarioDiarioIntegradoFieldSpecified = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ClaveEntFed
        {
            get
            {
                return this.claveEntFedField;
            }
            set
            {
                this.claveEntFedField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
    public partial class NominaReceptorSubContratacion
    {

        private string rfcLaboraField;

        private decimal porcentajeTiempoField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RfcLabora
        {
            get
            {
                return this.rfcLaboraField;
            }
            set
            {
                this.rfcLaboraField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal PorcentajeTiempo
        {
            get
            {
                return this.porcentajeTiempoField;
            }
            set
            {
                this.porcentajeTiempoField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
    public enum NominaReceptorSindicalizado
    {

        /// <comentarios/>
        Sí,

        /// <comentarios/>
        No,
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
    public partial class NominaPercepciones
    {

        private NominaPercepcionesPercepcion[] percepcionField;

        private NominaPercepcionesJubilacionPensionRetiro jubilacionPensionRetiroField;

        private NominaPercepcionesSeparacionIndemnizacion separacionIndemnizacionField;

        private decimal totalSueldosField;

        private bool totalSueldosFieldSpecified;

        private decimal totalSeparacionIndemnizacionField;

        private bool totalSeparacionIndemnizacionFieldSpecified;

        private decimal totalJubilacionPensionRetiroField;

        private bool totalJubilacionPensionRetiroFieldSpecified;

        private decimal totalGravadoField;

        private decimal totalExentoField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute("Percepcion")]
        public NominaPercepcionesPercepcion[] Percepcion
        {
            get
            {
                return this.percepcionField;
            }
            set
            {
                this.percepcionField = value;
            }
        }

        /// <comentarios/>
        public NominaPercepcionesJubilacionPensionRetiro JubilacionPensionRetiro
        {
            get
            {
                return this.jubilacionPensionRetiroField;
            }
            set
            {
                this.jubilacionPensionRetiroField = value;
            }
        }

        /// <comentarios/>
        public NominaPercepcionesSeparacionIndemnizacion SeparacionIndemnizacion
        {
            get
            {
                return this.separacionIndemnizacionField;
            }
            set
            {
                this.separacionIndemnizacionField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalSueldos
        {
            get
            {
                return this.totalSueldosField;
            }
            set
            {
                this.totalSueldosField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TotalSueldosSpecified
        {
            get
            {
                return this.totalSueldosFieldSpecified;
            }
            set
            {
                this.totalSueldosFieldSpecified = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalSeparacionIndemnizacion
        {
            get
            {
                return this.totalSeparacionIndemnizacionField;
            }
            set
            {
                this.totalSeparacionIndemnizacionField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TotalSeparacionIndemnizacionSpecified
        {
            get
            {
                return this.totalSeparacionIndemnizacionFieldSpecified;
            }
            set
            {
                this.totalSeparacionIndemnizacionFieldSpecified = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalJubilacionPensionRetiro
        {
            get
            {
                return this.totalJubilacionPensionRetiroField;
            }
            set
            {
                this.totalJubilacionPensionRetiroField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TotalJubilacionPensionRetiroSpecified
        {
            get
            {
                return this.totalJubilacionPensionRetiroFieldSpecified;
            }
            set
            {
                this.totalJubilacionPensionRetiroFieldSpecified = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalGravado
        {
            get
            {
                return this.totalGravadoField;
            }
            set
            {
                this.totalGravadoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalExento
        {
            get
            {
                return this.totalExentoField;
            }
            set
            {
                this.totalExentoField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
    public partial class NominaPercepcionesPercepcion
    {

        private NominaPercepcionesPercepcionAccionesOTitulos accionesOTitulosField;

        private NominaPercepcionesPercepcionHorasExtra[] horasExtraField;

        private string tipoPercepcionField;

        private string claveField;

        private string conceptoField;

        private decimal importeGravadoField;

        private decimal importeExentoField;

        /// <comentarios/>
        public NominaPercepcionesPercepcionAccionesOTitulos AccionesOTitulos
        {
            get
            {
                return this.accionesOTitulosField;
            }
            set
            {
                this.accionesOTitulosField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute("HorasExtra")]
        public NominaPercepcionesPercepcionHorasExtra[] HorasExtra
        {
            get
            {
                return this.horasExtraField;
            }
            set
            {
                this.horasExtraField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TipoPercepcion
        {
            get
            {
                return this.tipoPercepcionField;
            }
            set
            {
                this.tipoPercepcionField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Clave
        {
            get
            {
                return this.claveField;
            }
            set
            {
                this.claveField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Concepto
        {
            get
            {
                return this.conceptoField;
            }
            set
            {
                this.conceptoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal ImporteGravado
        {
            get
            {
                return this.importeGravadoField;
            }
            set
            {
                this.importeGravadoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal ImporteExento
        {
            get
            {
                return this.importeExentoField;
            }
            set
            {
                this.importeExentoField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
    public partial class NominaPercepcionesPercepcionAccionesOTitulos
    {

        private decimal valorMercadoField;

        private decimal precioAlOtorgarseField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal ValorMercado
        {
            get
            {
                return this.valorMercadoField;
            }
            set
            {
                this.valorMercadoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal PrecioAlOtorgarse
        {
            get
            {
                return this.precioAlOtorgarseField;
            }
            set
            {
                this.precioAlOtorgarseField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
    public partial class NominaPercepcionesPercepcionHorasExtra
    {

        private int diasField;

        private string tipoHorasField;

        private int horasExtraField;

        private decimal importePagadoField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int Dias
        {
            get
            {
                return this.diasField;
            }
            set
            {
                this.diasField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TipoHoras
        {
            get
            {
                return this.tipoHorasField;
            }
            set
            {
                this.tipoHorasField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int HorasExtra
        {
            get
            {
                return this.horasExtraField;
            }
            set
            {
                this.horasExtraField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal ImportePagado
        {
            get
            {
                return this.importePagadoField;
            }
            set
            {
                this.importePagadoField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
    public partial class NominaPercepcionesJubilacionPensionRetiro
    {

        private decimal totalUnaExhibicionField;

        private bool totalUnaExhibicionFieldSpecified;

        private decimal totalParcialidadField;

        private bool totalParcialidadFieldSpecified;

        private decimal montoDiarioField;

        private bool montoDiarioFieldSpecified;

        private decimal ingresoAcumulableField;

        private decimal ingresoNoAcumulableField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalUnaExhibicion
        {
            get
            {
                return this.totalUnaExhibicionField;
            }
            set
            {
                this.totalUnaExhibicionField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TotalUnaExhibicionSpecified
        {
            get
            {
                return this.totalUnaExhibicionFieldSpecified;
            }
            set
            {
                this.totalUnaExhibicionFieldSpecified = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalParcialidad
        {
            get
            {
                return this.totalParcialidadField;
            }
            set
            {
                this.totalParcialidadField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TotalParcialidadSpecified
        {
            get
            {
                return this.totalParcialidadFieldSpecified;
            }
            set
            {
                this.totalParcialidadFieldSpecified = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal MontoDiario
        {
            get
            {
                return this.montoDiarioField;
            }
            set
            {
                this.montoDiarioField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool MontoDiarioSpecified
        {
            get
            {
                return this.montoDiarioFieldSpecified;
            }
            set
            {
                this.montoDiarioFieldSpecified = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal IngresoAcumulable
        {
            get
            {
                return this.ingresoAcumulableField;
            }
            set
            {
                this.ingresoAcumulableField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal IngresoNoAcumulable
        {
            get
            {
                return this.ingresoNoAcumulableField;
            }
            set
            {
                this.ingresoNoAcumulableField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
    public partial class NominaPercepcionesSeparacionIndemnizacion
    {

        private decimal totalPagadoField;

        private int numAñosServicioField;

        private decimal ultimoSueldoMensOrdField;

        private decimal ingresoAcumulableField;

        private decimal ingresoNoAcumulableField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalPagado
        {
            get
            {
                return this.totalPagadoField;
            }
            set
            {
                this.totalPagadoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int NumAñosServicio
        {
            get
            {
                return this.numAñosServicioField;
            }
            set
            {
                this.numAñosServicioField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal UltimoSueldoMensOrd
        {
            get
            {
                return this.ultimoSueldoMensOrdField;
            }
            set
            {
                this.ultimoSueldoMensOrdField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal IngresoAcumulable
        {
            get
            {
                return this.ingresoAcumulableField;
            }
            set
            {
                this.ingresoAcumulableField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal IngresoNoAcumulable
        {
            get
            {
                return this.ingresoNoAcumulableField;
            }
            set
            {
                this.ingresoNoAcumulableField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
    public partial class NominaDeducciones
    {

        private NominaDeduccionesDeduccion[] deduccionField;

        private decimal totalOtrasDeduccionesField;

        private bool totalOtrasDeduccionesFieldSpecified;

        private decimal totalImpuestosRetenidosField;

        private bool totalImpuestosRetenidosFieldSpecified;

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute("Deduccion")]
        public NominaDeduccionesDeduccion[] Deduccion
        {
            get
            {
                return this.deduccionField;
            }
            set
            {
                this.deduccionField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalOtrasDeducciones
        {
            get
            {
                return this.totalOtrasDeduccionesField;
            }
            set
            {
                this.totalOtrasDeduccionesField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TotalOtrasDeduccionesSpecified
        {
            get
            {
                return this.totalOtrasDeduccionesFieldSpecified;
            }
            set
            {
                this.totalOtrasDeduccionesFieldSpecified = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalImpuestosRetenidos
        {
            get
            {
                return this.totalImpuestosRetenidosField;
            }
            set
            {
                this.totalImpuestosRetenidosField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TotalImpuestosRetenidosSpecified
        {
            get
            {
                return this.totalImpuestosRetenidosFieldSpecified;
            }
            set
            {
                this.totalImpuestosRetenidosFieldSpecified = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
    public partial class NominaDeduccionesDeduccion
    {

        private string tipoDeduccionField;

        private string claveField;

        private string conceptoField;

        private decimal importeField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TipoDeduccion
        {
            get
            {
                return this.tipoDeduccionField;
            }
            set
            {
                this.tipoDeduccionField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Clave
        {
            get
            {
                return this.claveField;
            }
            set
            {
                this.claveField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Concepto
        {
            get
            {
                return this.conceptoField;
            }
            set
            {
                this.conceptoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal Importe
        {
            get
            {
                return this.importeField;
            }
            set
            {
                this.importeField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
    public partial class NominaOtroPago
    {

        private NominaOtroPagoSubsidioAlEmpleo subsidioAlEmpleoField;

        private NominaOtroPagoCompensacionSaldosAFavor compensacionSaldosAFavorField;

        private string tipoOtroPagoField;

        private string claveField;

        private string conceptoField;

        private decimal importeField;

        /// <comentarios/>
        public NominaOtroPagoSubsidioAlEmpleo SubsidioAlEmpleo
        {
            get
            {
                return this.subsidioAlEmpleoField;
            }
            set
            {
                this.subsidioAlEmpleoField = value;
            }
        }

        /// <comentarios/>
        public NominaOtroPagoCompensacionSaldosAFavor CompensacionSaldosAFavor
        {
            get
            {
                return this.compensacionSaldosAFavorField;
            }
            set
            {
                this.compensacionSaldosAFavorField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TipoOtroPago
        {
            get
            {
                return this.tipoOtroPagoField;
            }
            set
            {
                this.tipoOtroPagoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Clave
        {
            get
            {
                return this.claveField;
            }
            set
            {
                this.claveField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Concepto
        {
            get
            {
                return this.conceptoField;
            }
            set
            {
                this.conceptoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal Importe
        {
            get
            {
                return this.importeField;
            }
            set
            {
                this.importeField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
    public partial class NominaOtroPagoSubsidioAlEmpleo
    {

        private decimal subsidioCausadoField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal SubsidioCausado
        {
            get
            {
                return this.subsidioCausadoField;
            }
            set
            {
                this.subsidioCausadoField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
    public partial class NominaOtroPagoCompensacionSaldosAFavor
    {

        private decimal saldoAFavorField;

        private short añoField;

        private decimal remanenteSalFavField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal SaldoAFavor
        {
            get
            {
                return this.saldoAFavorField;
            }
            set
            {
                this.saldoAFavorField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public short Año
        {
            get
            {
                return this.añoField;
            }
            set
            {
                this.añoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal RemanenteSalFav
        {
            get
            {
                return this.remanenteSalFavField;
            }
            set
            {
                this.remanenteSalFavField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
    public partial class NominaIncapacidad
    {

        private int diasIncapacidadField;

        private string tipoIncapacidadField;

        private decimal importeMonetarioField;

        private bool importeMonetarioFieldSpecified;

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int DiasIncapacidad
        {
            get
            {
                return this.diasIncapacidadField;
            }
            set
            {
                this.diasIncapacidadField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TipoIncapacidad
        {
            get
            {
                return this.tipoIncapacidadField;
            }
            set
            {
                this.tipoIncapacidadField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal ImporteMonetario
        {
            get
            {
                return this.importeMonetarioField;
            }
            set
            {
                this.importeMonetarioField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ImporteMonetarioSpecified
        {
            get
            {
                return this.importeMonetarioFieldSpecified;
            }
            set
            {
                this.importeMonetarioFieldSpecified = value;
            }
        }
    }
}
