//$(document).ready(calcDescuento());

//si el colaborador gana menos del salario mínimo entonces  
function validarSalarioMinimo() {
    var $tipoCredito = $("#TipoCredito");
    var $cantidad = $("#FactorDescuento");

    if ($tipoCredito.val() == 1) {
        if (parseFloat($sdi.html()) < parseFloat($sm.html())) {
            var max = 20;
            $cantidad.attr("max", max);
        }
    }
    else {
        $cantidad.removeAttr("max");
    }
};

//validarSalarioMinimo();

$(document).on("keyup", ".FactorDescuento", function () {
    let form = $(this).parents(".frmInfonavit");
    calcDescuento(form);
});

$(document).on("change", ".TipoCredito", function () {
    let form = $(this).parents(".frmInfonavit");
    //validarSalarioMinimo();
    calcDescuento(form);
});

$(document).on("change", ".FechaInicio", function () {
    let form = $(this).parents(".frmInfonavit");
    calcDescuento(form);
});





//Fechas de bimestres: 
function getDiasBimestre(fecha) {
    var fecha = fecha.split("-");
    var date = new Date(fecha[1] + "-" + fecha[2] + "-" + fecha[0]);

    var fechaActual = new Date();
    // si la fecha actual es mayor a la fecha de Inicio, el cálculo se realiza con la fecha actual
    if (fechaActual > date) { 
        date = fechaActual;
    }

    let mes = 0;
    let bimestre = "";

    //si el mes es Enero - Febrero
    if (date.getMonth() < 2) {
        bimestre = "Enero - Febrero";
        mes = 0;
    }

    //si el mes es Marzo - Abril
    else if (date.getMonth() >= 2 && date.getMonth() < 4) {
        bimestre = "Marzo - Abril";
        mes = 2;
    }

    //si el mes es Mayo - Junio
    else if (date.getMonth() >= 4 && date.getMonth() < 6) {
        bimestre = "Mayo - Junio";
        mes = 4;
    }
    
    //si es Julio - Agosto
    else if (date.getMonth() >= 6 && date.getMonth() < 8) {
        bimestre = "Julio - Agosto";
        mes = 6;
    }

    //si es Septiembre - Octubre
    else if (date.getMonth() >= 8 && date.getMonth() < 10) {
        bimestre = "Septiembre - Octubre";
        mes = 8;
    }

    //si es noviembre - diciembre
    else if (date.getMonth() >= 10) {
        bimestre = "Noviembre - Diciembre";
        mes = 10;
    }

    var fechaBimestre = new Date(date.getFullYear(), mes, 1);
    var fechaFinBimestre = new Date(date.getFullYear(), mes + 2, 0);
    let oneDay = 24 * 60 * 60 * 1000; // hours*minutes*seconds*milliseconds
    let diasBim = Math.round(Math.abs((fechaBimestre.getTime() - fechaFinBimestre.getTime()) / (oneDay))) + 1;

    return { diasBimestre: diasBim, bimestreAplicado: bimestre }
}

function CalcPorcentaje(cantidad, sdi) {
    //calcula el porcentaje
    var porcentaje = cantidad / 100;

    var descuentoDiario = sdi * porcentaje;
    if (isNaN(descuentoDiario))
        return 0;
    else
        return descuentoDiario;
}

function calcPesos(cantidad) {
    var bimestral = (parseFloat(cantidad) * 2) + 15;
    return bimestral;
}

function calcVSM(SM, cantidad) {
    let bim = (SM * cantidad) * 2 + 15; //bimestre
    return bim;
}