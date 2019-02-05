using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Enums
{
    public enum Tarifas
    {
        Diario = 1,
        Semanal = 2,
       // Docenal = 3,
        Quincenal = 4,
        Mensual = 5
    }

    public enum Incidencias
    {
        Inasistencias = 1,
        Incapacidades = 2,
        Permisos = 3,
        Vacaciones = 4
    }

    public enum TipoConcepto
    {
        Percepción = 1,
        Deducción = 2,
        Obligación = 0
    }

    public enum GenerarCfdiEstatus
    {
        Disponible = 0,
        GenerandoCfdi = 1,
        Generado = 2,
        Error = 3,
        Cancelado = 4
    }
}
