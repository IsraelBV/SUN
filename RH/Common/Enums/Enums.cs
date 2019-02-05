using System.ComponentModel;

namespace Common.Enums
{
    public enum AccionCrud
    {
        Consultar = 1,
        Agregar = 2,
        Editar = 3,
        Eliminar = 4
    }

    public enum Aplicacion
    {
        [Description("Recursos Humanos")]
        Rh = 1,
        [Description("Notificaciones")]
        Notificaciones = 2,
        [Description("Nominas")]
        Nominas = 3,
        [Description("Seguro Social")]
        SeguroSocial = 4,
        [Description("Seguridad")]
        Seguridad = 5
    }

    public enum Esquemas
    {
        Fiscal = 1,
        Complemento = 2,
        Sindicato = 3,
        Asimilado = 4
    }

    public enum Modulos
    {
        RHEmpresas = 1,
        RHClientes = 2,
        RHEmpleados = 3,
        RHInasistencias = 4,
        RHIncapacidades = 5,
        RHPermisos = 6,
        RHVacaciones = 7,
        RHValidadorRFC = 8,
        RHCumpleañosIMSS = 9,
        RHDepartamentos = 10,
        RHFDI = 11,
        RHTiposDeIncidencias = 12,
        RHPlantillas = 13,
        NomSelecCliente = 14,
        NomSelecPeriodo = 15,
        NomConfiguracion = 16,
        NomCatalogos = 17,
        NomCrearLayout = 18,
        NomDatosBancos = 19,
        NomPeriodoNomina = 20,
        NomAsignarConceptos = 21,
        NomIncidencias = 22,
        NomComplemento = 23,
        NomAjusteDeNomina = 24,
        NomProcesarNomina = 25,
        NomAutorizarNomina = 26,
        NomGenerarCFDI = 27,
        NomAcumulado = 28,
        NomGenerarReporteCC = 29,
        NomConfigConceptos = 30,
        NomConfigClientes = 31,
        SSEmpleados = 32,
        RHPrestamos = 33


    }

    public enum TipoCreditoInfonavit
    {
        Cuota_Fija = 1,
        Porcentaje = 2,
        VSM = 3,
        VSM_UMA = 4
    }

    public enum StatusNotificaciones
    {
        Nueva = 1,
        Leida = 2,
        Archivada = 3
    }

    public enum TiposNotificacion
    {
        //Alta = 1,//se eliminara
        //Baja = 2,//se eliminara
        //IMSS = 3,//se eliminara
        //Salarios = 4,//se eliminara
        //Puesto = 5,
        //Empresa = 6,//se eliminara
        //Reingresos = 7,//se eliminara
        //Cumple_IMSS = 8,
        //INFONAVIT = 9,
        //Vacaciones = 10,
        //Incapacidades = 11,
        //Permisos = 12,
        //BajaIMSS = 13,
        Alta_General = 1,
        Baja_General = 2,
        Alta_Fiscal = 3,
        SDI = 4,
        Puesto = 5,
        Transferencia = 6,
        Reingresos = 7,
        Cumple_IMSS = 8,
        INFONAVIT = 9,
        Vacaciones = 10,
        Incapacidades = 11,
        Permisos = 12,
        Baja_Fiscal = 13,
        SD = 14,
        Salario_Real = 15,
        Tranferencia_General = 16,
        Reingreso_General = 17,
        Cambio_Fecha_IMSS= 18,


    }

    public enum Paises
    {
        México = 1,
        EU = 2,
        Guatemala = 3
    }

    public enum ClasesRiesgo
    {
        I = 1,
        II = 2,
        III = 3,
        IV = 4,
        V = 5,
        NA = 99
    }

    public enum TipoKardex
    {
        Alta = 1,
        Baja = 2,
        Recontratacion = 3,
        Puesto = 4,
        SR = 5,
        SD = 6,
        Cumple_Imss=11,
        SDI = 12,
        EmpresaFiscal=13,
        EmpresaComplemento=14,
        EmpresaAsimilado=15,
        EmpresaSindicato=16
    }

    public enum TipoPagoIncidencias
    {
        Dias = 1,
        Horas = 2
    }

    public enum TipoPlantilla
    {
        Contrato = 1,
        Vacaciones = 2,
        Incapacidades = 3,
        Baja = 4,
        Permisos = 5,
        Prestamos = 6,
        MovimientoPersonal = 7,
        CartaAntiguedad = 8,
        GastosMedicosMenores = 9,
        Sindicato = 10
    }

    public enum DiasSemana
    {
        Domingo = 0,
        Lunes = 1,
        Martes = 2,
        Miércoles = 3,
        Jueves = 4,
        Viernes = 5,
        Sábado = 6
    }

    public enum Turnos
    {
        Administrativo = 1,
        Matutino = 2,
        Vespertino = 3,
        Nocturno = 4
    }

    public enum TipoSalario
    {
        Fijo = 1,
        Mixto = 2,
        Variable = 3
    }

    public enum Perfiles
    {
        SU = 1,
        Admin = 2,
        Mortal = 3
    }

    public enum ParametrosDeConfiguracion
    {
        [Description("Impuesto Sobre Nomina")]
        ISN = 1,
        [Description("Factor Salario Minimo Vigente")]
        FSMGV = 2,
        [Description("Activar Complemento")]
        AC =4,
        [Description("Iva")]
        IVA = 5,
        [Description("Activar calculo con UMA")]
        AUMA = 6
  

        
    }

    public enum TiposPrestamo
    {
        PRESTAMO_EMPRESA = 1,
        FONACOT = 2,
        INFONAVIT = 3,
    }

    public enum Mes
    {
        Enero = 1,
        Febrero = 2,
        Marzo = 3,
        Abril = 4,
        Mayo = 5,
        Junio = 6,
        Julio = 7,
        Agosto = 8,
        Septiembre = 9,
        Octubre = 10,
        Noviembre = 11,
        Diciembre = 12
    }
}
