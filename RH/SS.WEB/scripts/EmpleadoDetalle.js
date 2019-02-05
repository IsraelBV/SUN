//botones del panel
var $btnGuardar = $("#btnGuardar");
var $btnEditar = $("#btnEditar");
var $btnCancelar = $("#btnCancelar");
var $btnGuardarFONACOT = $("#btnAgregarFonacot");
var $btnHistorialINFONAVIT = $("#btnHistorialInfonavit");

//paneles que contienen los botones
var $panelBotonDesactivado = $("#panel-boton-desactivado");
var $panelBotonActivado = $("#panel-boton-activado");
var $panelFonacot = $("#panel-fonacot");
var $panelInfonavit = $("#panel-infonavit");

var $listNss = $("#list-nss");
var $listFechaImss = $("#list-fecha-imss");

//campos del formulario del seguro social
var $nss = $("#nss");
var $fechaIMSS = $("#fecha-imss");

$(document).ready(function () {
    $btnGuardar.click(function () {
        $(".tab-pane.fade.active.in").find("form").submit();
    });

    $btnEditar.click(function () {
        enablePanel();
    });

    $btnCancelar.click(function () {
        disablePanel();
    });

    $btnGuardarFONACOT.click(function () {
        $("#form-fonacot").submit();
    });
    
    LoadInfonavit();
    
});

$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    var target = $(e.target).attr("href") // activated tab
    if (target == "#tabFONACOT") {
        $("#panel-infonavit").hide();
        $panelBotonDesactivado.hide();
        $panelFonacot.show();
    }

    else if (target == "#tabINFONAVIT") {
        $panelFonacot.hide();
        $panelBotonDesactivado.hide();
        $panelBotonActivado.hide();

        let $btnSave = $panelInfonavit.find(".btnSaveInfonavit");
        let $btnAdd = $panelInfonavit.find(".btnAgregarInfonavit");

        $panelInfonavit.show();

        let model = $(this).data("model");
        if (model) {
            let activos = parseInt($(this).data("activos"));
            if (activos > 0) 
                $btnAdd.hide();
            else 
                $btnAdd.show();
        }
        else {
            $btnAdd.hide();
            $btnSave.show();
        }
    }
    else {
        $panelInfonavit.hide();
        $panelFonacot.hide();
        $panelBotonDesactivado.show();
    }
});

$(document).on("click", ".btnAgregarInfonavit", function () {
    let activos = parseInt($("#tabLinkInfonavit").data("activos"));
    if(activos > 0) {
        $.notify({
                title: "Algo ha salido mal.",
                message: "El cliente solo puede tener un crédito INFONAVIT vigente."
            }, {
                type: "warning",
                animate: {
                    enter: 'animated fadeInDown',
                    exit: 'animated fadeOutUp'
                },
                placement: {
                    from: 'top',
                    align: 'center'
                },
                delay: 5000,
            });
    } else {
        let IdEmpleadoContrato = parseInt($("#IdEmpleadoContrato").html());
        let request = $.ajax({
            url: "/Empleados/NewInfonavitForm/",
            method: "GET",
            data: {
                IdEmpleadoContrato: IdEmpleadoContrato
            }
        }).done(function (data){
            let $modal = $("#modal-new-infonavit");
            $modal.find(".modal-body").html(data);
            $modal.modal();
            validateForm($("#frmNewInfonavit"));
        });
    }
    
});

$(document).on("click", ".btnDetalleInfonavit", function () {
    let IdInfonavit = parseInt($(this).data("idInfonavit"));
    $.ajax({
        url: "/Empleados/GetInfonavit/",
        method: "GET",
        data: {
            IdInfonavit: IdInfonavit
        }
    }).done(function (data) {
        let modalInfonavit = $("#modal-detalle-infonavit");
        modalInfonavit.find(".modal-body").html(data);
        modalInfonavit.modal();
        let form = modalInfonavit.find("form");
        validateForm(form);
    });
});

$(document).on("hidden.bs.modal", "#modal-detalle-infonavit", function () {
    $(this).find("modal-body").html("");
    $(".btn-save-edit-infonavit").addClass('hidden');
    $(".btn-edit-infonavit").show();
})

$(document).on("click", ".btn-edit-infonavit", function () {
    $('#infonavit-detail').find('.form-control').attr('disabled', false);
    $(this).hide();
    $(".btn-save-edit-infonavit").removeClass('hidden');
});

$(document).on("click", ".btnHistorialPagoInfonavit", function () {
    let IdInfonavit = parseInt($(this).data("idInfonavit"));
    $.ajax({
        url: "/Empleados/GetHistorialPagosInfonavit",
        method: "GET",
        data: {
            IdInfonavit: IdInfonavit
        }
    }).done(function (data) {
        let modalHistorial = $("#modal-historial-infonavit");
        modalHistorial.find(".modal-body").html(data);
        modalHistorial.modal();
    });
});

function LoadInfonavit() {
    let IdContrato = parseInt($("#IdEmpleadoContrato").html());
    var request = $.ajax({
        url: "/Empleados/CheckInfonavit/",
        method: "GET",
        data: {
            id: IdContrato
        }
    });

    request.done(function (data) {
        $("#tabLinkInfonavit").data("activos", data.activos);
        $("#tabLinkInfonavit").data("model", data.model);
        $.ajax({
            url: data.url,
            method: "GET",
            data: {
                IdEmpleadoContrato: IdContrato
            }
        }).done(function (data) {
            $("#tabINFONAVIT").html(data);
            let form = $("#tabINFONAVIT").find("#frmNewInfonavit");
            if(form.length > 0)
                validateForm(form);
        })
    });
}

function disablePanel() {
    //deshabilitar el botón guardar
    $btnGuardar.attr("disabled", false);

    //deshabilitar los campos del formulario activo
    $(".tab-pane.fade.active.in").find("input").attr("disabled", true);
    $(".tab-pane.fade.active.in").find("select").attr("disabled", true);

    //habilitar los tabs del panel
    $(".tabLink").removeClass("disabledTab");

    //esconder el botón de guardar y cancelar
    $panelBotonActivado.hide();

    //mostrar el botón de editar
    $panelBotonDesactivado.show();

    //resetear el validador de tooltip
    resetTooltipValidator();
}

function enablePanel() {
    //habilitar el botón guardar
    $btnGuardar.attr("disabled", false);

    //habilitar los campos del formulario activo
    $(".tab-pane.fade.active.in").find("input").attr("disabled", false);
    $(".tab-pane.fade.active.in").find("select").attr("disabled", false);

    //deshabilitar los tabs del panel
    $(".tabLink").addClass("disabledTab");

    //esconder el botón de editar
    $panelBotonDesactivado.hide();

    //mostrar los botones de guardar y cancelar
    $panelBotonActivado.show();
}

function resetTooltipValidator() {
    $("input").removeClass("error");
    $("input").removeClass("success");
    $("select").removeClass("error");
    $("select").removeClass("success");
}

function SSFormSuccess(response) {
    if (response == true) {
        setTimeout(function () {
            $.notify({
                message: "¡Los datos del seguro social han sido guardados con éxito!"
            }, {
                type: "success",
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
            $listNss.html($nss.val());
            $listFechaImss.html($fechaIMSS.val());
        }, 1000);
        disablePanel();
    } else {
        setTimeout(function () {
            $.notify({
                message: "No se detectaron cambios en los valores ingresados."
            }, {
                type: "danger",
                animate: {
                    enter: 'animated fadeInDown',
                    exit: 'animated fadeOutUp'
                },
                placement: {
                    from: 'top',
                    align: 'center'
                },
                timer: 1000
            });
        }, 1000);
    }
}

function InfonavitFormSuccess(response) {
    if (response > 0) {
        setTimeout(function () {
            $.notify({
                message: "¡Los datos del crédito INFONAVIT han sido guardados con éxito!"
            }, {
                type: "success",
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
        }, 1000);
        LoadInfonavit();
        $("#panel-infonavit").hide();    
    } else {

    }
}

function FonacotFormSuccess(response) {
    $("#fonacot-modal").modal("hide");
    if (response) {
        setTimeout(function () {
            $.notify({
                message: "¡Los datos del crédito FONACOT han sido guardados con éxito!"
            }, {
                type: "success",
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
            let IdEmpleadoContrato = parseInt($("#IdEmpleadoContrato").html());
            $.ajax({
                url: "/Empleados/GetFonacot/",
                data: {
                    IdEmpleadoContrato: IdEmpleadoContrato,
                },
                success: function (data) {
                    $("#tabFONACOT").html(data);
                }
            });
        }, 1000);
    } else {
        setTimeout(function () {
            $.notify({
                message: "¡Ocurrió un error inesperado!"
            }, {
                type: "danger",
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
        }, 1000);
    }
}

function OnFailure() {

}

function OnBegin() {
    $.notify({
        message: "Cargando...",
    }, {
        type: "info",
        animate: {
            enter: "animate zoomIn",
            exit: "animate zoomOut"
        },
        placement: {
            from: 'top',
            align: 'center'
        },
        offset: {
            y: 140,
        },
        showProgressbar: true,
        delay: 10
    });
}