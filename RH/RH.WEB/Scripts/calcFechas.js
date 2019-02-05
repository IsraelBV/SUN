var fechaInicio = $("#FechaInicio");
var fechaFin = $("#FechaFin");
var resultado = $("#Dias");


fechaInicio.change(function () {
    if (fechaFin.val() != "") {
        var dias = calcDias(fechaInicio.val(), fechaFin.val());
        resultado.val(dias);
    }
});

fechaFin.change(function () {
    if (fechaInicio.val() != "") {
        var dias = calcDias(fechaInicio.val(), fechaFin.val());
        resultado.val(dias);
    }
});

resultado.keyup(function () {
    setFecha_fin();
});


function calcDias(fechaInicio, fechaFin) {
    
    var fecha1= fechaInicio.split("-");
    var dia1= fecha1[2];
    var mes1= fecha1[1];
    var anyo1 = fecha1[0];
   

    var fecha2= fechaFin.split("-");
    var dia2= fecha2[2];
    var mes2= fecha2[1];
    var anyo2= fecha2[0];

    var nuevafecha1 = new Date(anyo1 + "," + mes1 + "," + (dia1-1));
    var nuevafecha2 = new Date(anyo2 + "," + mes2 + "," + dia2);

    var Dif= nuevafecha2.getTime() - nuevafecha1.getTime();
    var dias= Math.floor(Dif/(1000*24*60*60));

    return dias;
}

function setFecha_fin() {
    if ($("#FechaInicio").val() == "") {
        var fecha_inicio = mm + "-" + dd + "-" + yyyy;
        var f_fin = new Date(fecha_inicio);
    } else {
        var fecha_trim = $("#FechaInicio").val().split("-");

        //formato mm/dd/yyyy
        var f_fin = new Date(fecha_trim[1] + "-" + fecha_trim[2] + "-" + fecha_trim[0]);
    }
    var i = 0;

    while (i < $("#Dias").val()) {

        f_fin.setDate(f_fin.getDate() + 1);
        i++;
    }

    var date = concatDate(f_fin);
    $("#FechaFin").val(date);

}

//Recibe un objeto de tipo Date y retorna la fecha concatenada para introducir en un input type date
function concatDate(date) {
    var dd = date.getDate()-1;
    var mm = date.getMonth() + 1;
    var yyyy = date.getFullYear();

    dd = dd < 10 ? '0' + dd : dd;
    mm = mm < 10 ? '0' + mm : mm;

    return yyyy + "-" + mm + "-" + dd;
}