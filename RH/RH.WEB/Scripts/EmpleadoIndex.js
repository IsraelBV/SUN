$(document).ready(function () {
    var btnToggleModalBaja = $(".btnToggleBaja");
    var btnToggleModalRecontratacion = $(".btnToggleRecontratacion");
    var modalBaja = $("#ModalBajaEmpleado");
    var modalRecontratacion = $("#modal-recontratacion");
    var modalKardex = $("#modal-kardex");
    var inputIdEmpleado = $("#input-id-empleado");
    var btnBaja = $("#btnBaja");
    var fechaBaja = $("#fechaBaja");
    var mrNombreEmpleado = $("#mr-nombre-empleado");
    var btnRecontratar = $("#btnRecontratar");
    var btnSubmitRecontratarFrm = $("#btnSubmitRecontratarFrm");
    var btnKardex = $(".btnKardex");
    var btnReenvioAlerta = $("#btnReenvioAlerta");
    
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust();
    });


    //Obtener la fecha de hoy
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1;
    var yyyy = today.getFullYear();

    dd = dd < 10 ? '0' + dd : dd;
    mm = mm < 10 ? '0' + mm : mm;

    // /obtener la fecha de hoy

    // aplica la fecha actual a la clase hoy
    $(".hoy").val(yyyy + "-" + mm + "-" + dd);

    //setMaxFechaBaja();

    //function setMaxFechaBaja() {
    //    var fechaBaja = $("#fechaBaja").val();
    //    if (fechaBaja == "") {
    //        var maxDate = new Date(mm + "-" + dd + "-" + yyyy);
    //    }
    //    else {
    //        var trimDate = fechaBaja.split("-");
    //        var maxDate = new Date(trimDate[1] + "-" + trimDate[2] + "-" + trimDate[0]);
    //    }
    //    for (i = 0; i < 5; i++) {
    //        maxDate.setDate(maxDate.getDate() - 1);
    //        if (maxDate.getDate() == 0) {
    //            maxDate.setDate(maxDate.getDate() - 1);
    //        }
    //    }

    //    var date = concatDate(maxDate);

    //    $("#fechaBaja").attr("min", date);
    //    $("#fechaBaja").attr("max", fechaBaja);
    //}

    function concatDate(date) {
        var dd = date.getDate();
        var mm = date.getMonth() + 1;
        var yyyy = date.getFullYear();

        dd = dd < 10 ? '0' + dd : dd;
        mm = mm < 10 ? '0' + mm : mm;

        return yyyy + "-" + mm + "-" + dd;
    }

    $("#btnModalBaja").click(function () {
        $("#ModalBajaEmpleado").modal();
    });

    //acción para dar de baja un empleado
    btnBaja.click(function () {
        var empleados = GetEmpleadosActivosSeleccionados();
        var motivoBaja = $('select[name=MotivoBaja]').val()
        
        $.ajax({
            type: "POST",
            url: '/Empleados/EliminarRegistro/',
            data: {
                empleados: empleados,
                fecha: $("#fechaBaja").val(),
                MotivoBaja: motivoBaja
            },
            success: successAjax,
            error: error
        });

        function successAjax(data, status) {
            modalBaja.modal("hide");
            if (data > 0)
            {
                showMessage("¡Exito!", "Registros dados de baja: " + data, "success");
                RecargarEmpleados();
            }
            else {
                alert("Ocurrió un error inesperado");
            }
        }

        function error() {
            alert("Ocurrió un error inesperado");
        }
    });

    //boton de reenvio de alerta
    btnReenvioAlerta.click(function () {
        var empleados = GetEmpleadosActivosSeleccionados();
        

        $.ajax({
            type: "POST",
            url: '/Empleados/ReenvioAlerta/',
            data: {
                empleados: empleados,

            },
            success: successAjax,
            
        });

        function successAjax(data) {
            modalBaja.modal("hide");
            if (data = "true") {
                showMessage("¡Exito!", "Alertas Reenviadas con exito");
                RecargarEmpleados();
            }
            else {
                alert("Las Alertas no fueron enviadas ");
            }
        }

    });


    $("#btnDetalle").click(function () {
        if ($("#activos").is(":visible"))
            var empleado = $("input[name='input-activos']:checked").val();
        else
            var empleado = $("input[name='input-inactivos']:checked").val();
        window.location.href = "/Empleados/Detalle/" + empleado;
    });

    $("#btnKardex").click(function () {
        if ($("#activos").is(":visible")) {
            var empleado = $("input[name='input-activos']:checked");
        }
        else {
            var empleado = $("input[name='input-inactivos']:checked");
        }

        var idEmpleado = empleado.val();

        var nombreEmpleado = $(empleado).parent().parent().find(".tdNombre").html();
        $("#mk-nombre-empleado").html(nombreEmpleado);

        $.get("/Empleados/KardexEmpleado?empleado=" + idEmpleado, function (data) {
            $("#kardex-container").html(data);
            modalKardex.modal();
        });
    });

    $("#btnContrato").click(function () {
        $("#modal-contratos").modal();
    });

    $("#btnDownloadContrato").click(function () {
        var empleados = GetEmpleadosActivosSeleccionados();
        var idPlantilla = $("#Plantilla").val();
        $.ajax({
            url: "/Empleados/GenerarPlantillaContrato",
            method: "POST",
            data: {
                idPlantilla: idPlantilla,
                empleados: empleados
            }
        }).done(function (data) {
            if (data != null) {
                alert(data);
                var url = "/Empleados/GetPlantilla/?path=DATA";
                url = url.replace("DATA", data);
                alert(url);
                window.location.href = url;
            }
            else {
                showMessage("¡Error!", "Ocurrió un error inesperado.", "danger");
            }
        });
        $("#modal-contratos").modal("hide");
    });

    $("#btnPlantillaBaja").click(function () {
        $("#modal-plantillas-baja").modal();
    });
    $("#btnPlantillaBajaInact").click(function () {
        $("#modal-plantillas-bajaInact").modal();
    });

    //botón que permite descargar la plantilla de baja para empleados activos
    $("#btnDownloadBaja").click(function () {
        var empleados = GetEmpleadosActivosSeleccionados();
        var idPlantilla = $("#PlantillaBaja").val();

        $.ajax({
            url: "/Empleados/GenerarPlantillaBaja/",
            method: "POST",
            data: {
                IdPlantilla: idPlantilla,
                Empleados: empleados
            }
        }).done(function (data) {
            if (data != null) {
                var url = "/Empleados/GetPlantilla/?path=DATA";
                url = url.replace("DATA", data);
                window.location.href = url;
            } else {
                showMessage("¡Error!", "Ocurrió un error inesperado.", "danger");
            }
        })
        $("#modal-plantillas-baja").modal("hide");
    });

    //botón que permite descargar la plantilla de baja para empleados inactivos
    $("#btnDownloadBajaInact").click(function () {
       var empleados = GetEmpleadosInactivosSeleccionados();
       var idPlantilla = $("#PlantillaBajaInact").val();

        $.ajax({
            url: "/Empleados/GenerarPlantillaBaja/",
            method: "POST",
            data: {
                IdPlantilla: idPlantilla,
                Empleados: empleados
            }
        }).done(function (data) {
            if (data != null) {
                var url = "/Empleados/GetPlantilla/?path=DATA";
                url = url.replace("DATA", data);
                window.location.href = url;
            } else {
                showMessage("¡Error!", "Ocurrió un error inesperado.", "danger");
            }
        })
        $("#modal-plantillas-bajaInact").modal("hide");
    });

    $("#btnTransfer").click(function () {
        $("#modal-transfer").modal();
    });

    $("#btnTransferEmpresa").click(function () {
        var empleados = GetEmpleadosActivosSeleccionados();

        var IdEmpresaFiscal = $("#IdEmpresaFiscal").val();
        console.log(IdEmpresaFiscal);
        var IdEmpresaComplemento = $("#IdEmpresaComplemento").val();
        console.log(IdEmpresaComplemento);
        var IdEmpresaAsimilado = $("#IdEmpresaAsimilado").val();
        console.log(IdEmpresaAsimilado);
        var IdEmpresaSindicato = $("#IdEmpresaSindicato").val();
        console.log(IdEmpresaAsimilado);
        var fechaIMSS = $("#fechaIMSS").val();
        console.log(IdEmpresaAsimilado);

        $.ajax({
            url: "/Empleados/TransferEmployees/",
            method: "POST",
            data: {
                Empleados: empleados,
                IdEmpresaFiscal: IdEmpresaFiscal,
                IdEmpresaComplemento: IdEmpresaComplemento,
                IdEmpresaSindicato: IdEmpresaSindicato,
                IdEmpresaAsimilado: IdEmpresaAsimilado,
                fechaIMSS: fechaIMSS
            }
        }).done(function (data) {
            $("#modal-transfer").modal("hide");
            if (data > 0)
                showMessage("¡Exito!", data + " empleados han sido transferidos", "success");
            else
                showMessage("¡Error!", "Ha ocurrido un error inesperado", "danger");
        });
    });

    function GetEmpleadosActivosSeleccionados() {
        var empleados = $("#tblEmpleados").DataTable().rows('.selected').ids();
        var array = [];
        for (var i = 0; i < empleados.length; i++) {
            array[i] = empleados[i];
        }
        return array;
    }

    function GetEmpleadosInactivosSeleccionados() {
        var empleados = $("#tblInactivos").DataTable().rows('.selected').ids();
        var array = [];
        for (var i = 0; i < empleados.length; i++) {
            array[i] = empleados[i];
        }
        return array;
    }

    //TOOLTIPS
    $("[data-toggle='boton']").tooltip({ container: 'body' });

    function showMessage(title, message, type) {
        $.notify({
            title: title,
            message: message
        }, {
            type: type,
            animate: {
                enter: 'animated fadeInDown',
                exit: 'animated fadeOutUp'
            },
            placement: {
                from: 'top',
                align: 'center'
            },
            offset: 50,
            timer: 1000
        });
        
    }
});

var IdSucursal = $("#IdSucursal").val();

function RecargarEmpleados() {
    $.ajax({
        type: "GET",
        url: "/Empleados/EmpleadosActivos/" + IdSucursal,
        dataType: "html",
        success: function (data) {
            $("#activos").html(data);
        }
    });
    $.ajax({
        type: "GET",
        url: "/Empleados/EmpleadosInactivos/" + IdSucursal,
        dataType: "html",
        success: function (data) {
            $("#inactivos").html(data);
        }
    });
    $(".btn-toolbar-hidden").hide();
}

function CloseModal() {
    RecargarEmpleados();
    $("#modal-recontratacion").modal("hide");
    //$.ajax({
    //    url: "Empleados/FormRecontratacion"
    //}).done(function (data) {
    $('form').find('input[type=text], input[type=password], input[type=number], input[type=email], textarea,select').val('');
    //    $("#divFormRecontratacion").html(data);
    //});

}

