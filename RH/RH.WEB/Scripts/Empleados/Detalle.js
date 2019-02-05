$(document).ready(function () {
    $(".form-disabled").attr("disabled", true);

    $("#btnEdit").click(function () {
        enablePanel();
    });

    $("#btnSave").click(function () {
        $(".tab-pane.active").find("form").submit();
    });
    $("#btnchsalario").click(function () {
        $('#formcambiosalario')[0].reset();
       
        var idempleado = $("#IdEmpleado").html();
        var nombre = $("#apemp").html() +" "+ $("#amemp").html() +" "+ $("#nombreemp").html();
        $("#spnombre").html(nombre);
        $.ajax({
            type: "POST",
            url: "/Empleados/ObtenerContrato",
            data: '{idEmpleado: ' + idempleado + '}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                $("#spfechaimss").html(ToJavaScriptDate(response.FechaIMSS));
                $("#cs_idContrato").val(response.IdContrato);
                $("#lsrc").html('$' + response.SalarioReal + '');
                $("#lsdc").html('$' + response.SD + '');
                $("#lsdic").html('$' + response.SDI + '');
            },
            failure: function (response) {
                alert("Error al buscar contrato anterior.");
            }
        });
        $("#modal-cambiosalario").modal();
        
    });



    $("#btnCancel").click(function () {
        if ($("#section1").is(":visible")) UpdateSection1();
        if ($("#section2").is(":visible")) UpdateSection2();
        disablePanel();
    });

    $("input[type='text']").change(function () {
        this.value = this.value.toUpperCase();
    });

    $("a[data-toggle='tab']").on("shown.bs.tab", function (e) {
        var target = $(e.target).attr("href");
        if (target == "#section3") {
            $("#panelDisabled").hide();
            $("#panelDocumentos").hide();
            $("#panelBancario").show();
        }
        else if (target == "#section4") {
            $("#panelDisabled").hide();
            $("#panelBancario").hide();
            $("#panelDocumentos").show();
        }
        else {
            $("#panelBancario").hide();
            $("#panelDocumentos").hide();
            $("#panelDisabled").show();
        }
    });
});

$(document).on("click", "#btnUploadDoc", function () {
    $(this).attr("disabled", true);
    var data = new FormData();
    var file = $("#document").get(0).files;
    if (file.length > 0)
        data.append("Document", file[0]);

    var IdTipoDocumento = $("#IdTipoDocumento").val();
    data.append("IdTipoDocumento", IdTipoDocumento);

    var IdEmpleado = parseInt($("#IdEmpleado").html());
    data.append("IdEmpleado", IdEmpleado);

    fileNotification = $.notify({
        title: "<strong>Cargando...</strong>",
        message: "Se está subiendo su documento.",
        icon: "glyphicon glyphicon-info-sign"
    }, {
        delay: 0,
        newest_on_top: true
    });

    var request = $.ajax({
        url: "/Empleados/UploadDoc",
        method: "POST",
        contentType: false,
        processData: false,
        data: data,
        beforeSend: function () {
            $("#newDocumentModal").modal("hide");
        }
    });

    request.done(function (data) {
        $.ajax({
            url: "/Empleados/GetDocumentosEmpleado",
            method: "GET",
            data: {
                IdEmpleado: IdEmpleado
            }
        }).done(function (data) {
            setTimeout(function () {
                $("#section4").html(data);
            }, 1000);
        });
        if (!data.status) {
            fileNotification.update("title", "<strong>Algo malo ha ocurrido...</strong>")
            fileNotification.update("message", data.message);
            fileNotification.update("icon", "glyphicon glyphicon-exclamation-sign")
            fileNotification.update("type", "danger");
        } else {
            fileNotification.update("title", "<strong>Carga Completa!</strong>");
            fileNotification.update("message", "Se ha guardado el documento exitosamente.");
            fileNotification.update("type", "success");
        }
        setTimeout(function () {
            fileNotification.close();
        }, 5000);
    });
});

$(document).on("click", ".btnDeleteDoc", function () {
    var IdDocumento = parseInt($(this).data("iddocumento"));
    var deleteDocNotif;
    if (confirm("Seguro que deseas eliminar este documento? NO HAY VUELTA ATRÁS, PIÉNSALO BIEN.")) {
        var request = $.ajax({
            url: "/Empleados/DeleteDocument",
            method: "POST",
            data: {
                id: IdDocumento
            },
            beforeSend: function () {
                deleteDocNotif = $.notify({
                    title: "<strong>Cargando...</strong>",
                    message: "Por favor espere.",
                    icon: "glyphicon glyphicon-info-sign"
                }, {
                    delay: 0,
                    newest_on_top: true
                });
            }
        });

        request.done(function (data) {


            if (data.status) {
                deleteDocNotif.update("title", "<strong>OK!</strong>");
                deleteDocNotif.update("message", data.message);
                deleteDocNotif.update("type", "success");
            }
            else {
                deleteDocNotif.update("title", "<strong>Algo malo ha ocurrido... </strong>");
                deleteDocNotif.update("message", data.message);
                deleteDocNotif.update("type", "danger");
                deleteDocNotif.update("icon", "glyphicon glyphicon-exclamation-sign");
            }
            setTimeout(function () {
                deleteDocNotif.close();
            }, 5000);
        });

        request.done(function () {
            var IdEmpleado = parseInt($("#IdEmpleado").html());
            $.ajax({
                url: "/Empleados/GetDocumentosEmpleado",
                method: "GET",
                data: {
                    IdEmpleado: IdEmpleado
                }
            }).done(function (data) {
                setTimeout(function () {
                    $("#section4").html(data);
                }, 1000);
            });
        })
    }
})

function ToJavaScriptDate(value) {
    var pattern = /Date\(([^)]+)\)/;
    var results = pattern.exec(value);
    var dt = new Date(parseFloat(results[1]));
    return dt.getDate() + "/" + (dt.getMonth()+1) + "/" + dt.getFullYear();
}

function enablePanel() {
    //habilitar los campos
    $(".tab-pane.active").find(".form-disabled").attr("disabled", false);

    //deshabilitar los tabs del panel
    $(".nav-tabs").find("li").not(".active").find(".tabLink").addClass("disabledTab")
    //$(".tabLink").not(".active").addClass("disabledTab");

    //Ocultar el botón editar
    $("#panelDisabled").hide();

    //Mostrar el botón guardar y cancelar
    $("#panelEnabled").show();

    $("#modo").html("Editar");

}

function disablePanel() {
    //deshabilitar los campos
    $(".tab-pane.active").find(".form-disabled").attr("disabled", "disabled");
    $(".tab-pane.active").find(".contrato-disabled").attr("disabled", "disabled");

    //habilitar los tabs del panel
    $(".tabLink").removeClass("disabledTab");

    //Ocultar el botón guardar y cancelar
    $("#panelEnabled").hide();

    //Mostrar el botón editar
    $("#panelDisabled").show();

    $("#modo").html("Detalle");
    resetTooltipValidator();
}

function resetTooltipValidator() {
    $("input").removeClass("error");
    $("input").removeClass("success");
    $("select").removeClass("error");
    $("select").removeClass("success");
}

var notify;

function UpdateSection1() {
    var IdEmpleado = parseInt($("#IdEmpleado").html());
    $.ajax({
        url: "/Empleados/GetEmpleado/"+IdEmpleado,
        method: "GET"
    }).done(function (data) {
        $("#section1").html(data);
        disablePanel();
        var nombre = $("#Nombres").val();
        var paterno = $("#APaterno").val();
        var materno = $("#AMaterno").val();
        $("#DatosImportantes").find(".Nombres").html(nombre);
        $("#DatosImportantes").find(".Paterno").html(paterno);
        $("#DatosImportantes").find(".Materno").html(materno);
    });
}

function UpdateSection2() {
    var IdEmpleado = parseInt($("#IdEmpleado").html());
    $.ajax({
        url: "/Empleados/GetContrato/" + IdEmpleado,
        method: "GET"
    }).done(function (data) {
        $("#section2").html(data);
        disablePanel();
    });
}

function OnBegin() {
    showProgressMessage();
    disablePanel();
}

function OnFailure() {
    showFailureMessage();
}

function showProgressMessage() {
    notify = $.notify({
        title: "<strong>Guardando</strong>",
        message: "Por favor espere...",
        icon: "glyphicon glyphicon-info-sign"
    }, {
        allow_dismiss: false,
        showProgressbar: true,
        delay: 0,
        newest_on_top: true
    });
}

function showFailureMessage() {
    notify.update("title", "<strong>¡Ups!</strong>");
    notify.update("message", "Ha ocurrido algo inesperado, por favor reportelo al administrador.");
    notify.update("type", "danger");
    notify.update("icon", "glyphicon glyphicon-exclamation-sign");
    setTimeout(function () {
        notify.close();
    }, 5000);
}

function showSuccessMessage() {
    notify.update("title", "<strong>¡Cambios guardados!</strong>");
    notify.update("message", "");
    notify.update("type", "success");
    notify.update("icon", "glyphicon glyphicon-ok-sign");
    setTimeout(function () {
        notify.close();
    }, 5000);
}

function showWarningMessage() {
    notify.update("title", "<strong>Operación Cancelada</strong>");
    notify.update("message", "No se actualizó ningún dato.");
    notify.update("type", "warning");
    setTimeout(function () {
        notify.close();
    }, 5000);
}

function OnSuccess(response) {
    if (response)
        showSuccessMessage();
    else
        showWarningMessage();
}

function OnSuccessDatosBancarios(response) {
    if (response > 0) {
        $.ajax({
            url: "/Empleados/GetDatosBancarios/" + response,
            dataType: "html"
        }).done(function (data) {
            $("#section3").html(data);
        })
    } else {
        alert("Error ingresando el registro a la base de datos");
    }
}

function OnSuccessUpdateBanco(response){
    if (response) {
        $.ajax({
            url: "/Empleados/GetDatosBancarios/" + response,
            dataType: "html"
        }).done(function (data) {
            $("#section3").html(data);
        })
    } else {
        alert("Error ingresando el registro a la base de datos");
    }
}