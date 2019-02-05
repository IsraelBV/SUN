$(document).ready(function () {
    $("#tblEmpleados").DataTable({
        paging: false,
        bInfo: false,
        scrollY: "350px"
    });
});

$(document).on("shown.bs.modal", "#modalCalendar", function () {
    var selected = $("#tblEmpleados").find(".selected");
    var fechasAlta = [];
    var fechasBaja = [];

    selected.each(function () {
        var $fechaAlta = $(this).data("fecha-alta");
        var fechaAlta = new Date($fechaAlta);
        fechaAlta.setDate(fechaAlta.getDate() + 1);
        fechasAlta.push(fechaAlta);

        var $fechaBaja = $(this).data("fecha-baja");
        if ($fechaBaja != "") {
            var fechaBaja = new Date($fechaBaja);
            fechaBaja.setDate(fechaBaja.getDate() + 1);
            fechasBaja.push(fechaBaja);
        }
    });
    var fechaAlta = new Date(Math.max.apply(null, fechasAlta));
    var fechaBaja = new Date(Math.min.apply(null, fechasBaja));

    $("#Fecha").attr("min", dateToString(fechaAlta));
    $("#FechaFin").attr("min", dateToString(fechaAlta));
    if (!isNaN(fechaBaja)) {
        $("#Fecha").attr("max", dateToString(fechaBaja));
        $("#FechaFin").attr("max", dateToString(fechaBaja));
    }

    $("#IdTipoInasistencia").focus();
});

$(document).on("hidden.bs.modal", "#modalCalendar", function () {
    $("#Dias").val("");
    $("#FechaFin").val("");
    var date = new Date()
    var dateString = dateToString(date);
    $("#Fecha").val(dateString);
    $("input").removeClass("success");
    $("select").removeClass("success");
    $("#Fecha").attr("min", "");
    $("#Fecha").attr("max", "");
    $("#FechaFin").attr("min", "");
    $("#FechaFin").attr("max", "");
});

$(document).on("keyup", "#Dias", function () {
    var $Fecha = $("#Fecha").val();
    var $Dias = $(this).val();
    if ($Fecha != "" && ($Dias != "" && $Dias > 1)) {
        sumarDias($Fecha, $Dias);
    }
    else if($Fecha != "" && $Dias <= 1) {
        $("#FechaFin").val("");
    }
});

$(document).on("change", "#Fecha", function () {
    var $Fecha = $(this).val();
    var $Dias = $("#Dias").val();
    if ($Fecha != "" && ($Dias != "" && $Dias > 1)) {
        sumarDias($Fecha, $Dias);
    }
    else if ($Fecha != "" && $Dias <= 1) {
        $("#FechaFin").val("");
    }
});

$(document).on("change", "#FechaFin", function () {
    var $FechaFinal = $(this).val();
    var $Fecha = $("#Fecha").val();
    if ($Fecha != "" && $FechaFinal != "") {
        restarFechas($Fecha, $FechaFinal);
    }
});

function sumarDias($Fecha, $Dias) {
    var FechaInicio = new Date($Fecha);
    FechaInicio.setDate(FechaInicio.getDate() + parseInt($Dias));

    var FechaFinal = dateToString(FechaInicio);

    $("#FechaFin").val(FechaFinal);
}

function restarFechas($Fecha, $FechaFinal) {
    var FechaInicio = new Date($Fecha);
    var FechaFinal = new Date($FechaFinal);
    var dias = Math.floor((FechaFinal - FechaInicio) / (1000 * 60 * 60 * 24));

    $("#Dias").val(dias);
}

function dateToString(date) {
    var mm = date.getMonth();
    var mm = mm + 1;
    var dd = date.getDate();
    var yyyy = date.getFullYear();
    if (mm < 10) {
        mm = "0" + mm;
    }
    if (dd < 10) {
        dd = "0" + dd;
    }

    return yyyy + "-" + mm + "-" + dd;
}

$(document).on("click", "#tblEmpleados tbody tr", function () {
    var $tr = $(this);
    if ($tr.hasClass("selected")) {
        $tr.removeClass("selected");
    }
    else {
        $tr.addClass("selected");
    }
    checkPanel();
});

$(document).on("change", "#Fecha", function () {
    $("#FechaFin").attr("min", $(this).val());
});

function checkPanel() {
    var selected = $("#tblEmpleados").find(".selected");
    if (selected.length > 0) {
        if ($("#control-panel").is(":hidden")) {
            $("#control-panel").show()
            $(".table-info").show();
        }
        else {
            if (selected.length > 1) {
                $("#btnHistorial").hide();
                $("#numEmpleados").html(selected.length + " empleados seleccionados");
            }
            else {
                $("#btnHistorial").show();
                $("#numEmpleados").html("1 empleado seleccionado");
            }
        }
    }
    else {
        $(".table-info").hide();
        $("#control-panel").hide();
    }
}

$(document).on("click", "#btnGuardarCaptura", function () {
    $("form").submit();
});

var notiDuplicados;

$(document).on("submit", "form", function (e) {
    e.preventDefault();
    var selected = $("#tblEmpleados").find(".selected");
    var empleados = [];
    var i = 0;
    selected.each(function () {
        empleados[i] = parseInt($(this).data("idempleado"));
        i++;
    });

    var dias = $('#simpliest-usage').multiDatesPicker('getDates');

    var tipoInasistencia = $("#IdTipoInasistencia").val();

   $.ajax({
        url: "/CapturaInasistencias/RegistrarInasistencia",
        method: "POST",
        data: {
            fechas: dias,
            idEmpleados: empleados,
            IdTipoInasistencia: tipoInasistencia
        },
        beforeSend: function () {
            $("#modalCalendar").modal("hide");
        }

   }).done(function (data) {

        if (data.length > 0) {
            var message = "<ul>";
            for (var i = 0; i < data.length; i++) {
                if (data[i].FechaFinal && data[i].FechaFinal != data[i].Fecha)
                    message += "<li><b>" + data[i].Empleado + "</b> tiene 1 <b>" + data[i].Incidencia + "</b> del <b>" + data[i].Fecha + "</b> al <b>" + data[i].FechaFinal + "</b></li>";
                else
                    message += "<li><b>" + data[i].Empleado + "</b> tiene 1 <b>" + data[i].Incidencia + "</b> el <b>" + data[i].Fecha + "</b></li>";
            }

            var title = "<b>No se guardó " + data.length + " registro:</b> <br/>"
            if (data.length > 1)
                title = "<b>No se guardaron " + data.length + " registros:</b> <br/>";

            notiDuplicados = $.notify({
                title: title,
                message: message,
            }, {
                type: "danger",
                animate: {
                    enter: 'animated fadeInDown',
                    exit: 'animated fadeOutUp'
                },
                placement: { from: 'top', align: 'center' },
                offset: 55, timer: 60000
            });
        }
        if (data.length < empleados.length) {
            var registros = empleados.length - data.length;
            $.notify({
                title: "<b>Se guardaron " + registros + " registros exitosamente</b>",
                message: ""
            }, {
                type: "success",
                animate: {
                    enter: 'animated fadeInDown',
                    exit: 'animated fadeOutUp'
                },
                placement: { from: 'top', align: 'center' },
                offset: 55, timer: 10000
            });
        }
    });
});

$(document).on("click", "#btnHistorial", function () {
    var selected = $("#tblEmpleados").find(".selected");
    window.location.href = "/CapturaInasistencias/Detalle/" + selected.data("idempleado");
});