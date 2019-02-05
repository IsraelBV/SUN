$(document).ready(function () {
    $(".btnSearch").click(function () {
        $("#searchFrm").submit();
    });

    $("#btnCancel").click(function () {
        expandPanel();
    });

    $("#select-all").click(function () {
        $(".seleccion").prop("checked", true);
    });

    $("#deselect-all").click(function () {
        $(".seleccion").prop("checked", false);
    });

    $(".seleccion").change(function () {
        if ($(".seleccion:checked").length == $("seleccion").length) {
            $("#select-all").hide("blind", 500);
        } else {
            if ($("#select-all").is(":hidden")) {
                $("#select-all").show("blind", 500);
            }
        }
    });

    $("#btnDownload").click(function () {
        var empleados = $("input[name='seleccion']:checked").map(function () {
            return $(this).val()
        }).get();

        var today = getToday();
        var nombreArchivo = "File-" + $("#TipoArchivo").val() + "-" + $("#TipoMovimiento").val() + "-" + today + ".txt";

        if (empleados.length > 0) {
            $.ajax({
                url: "/Exportacion/CreateFile/",
                type: "POST",
                data: {
                    empleados: empleados,
                    IdEmpresa: $("#IdEmpresa").val(),
                    tipoArchivo: $("#TipoArchivo").val(),
                    tipoMovimiento: $("#TipoMovimiento").val(),
                    nombreArchivo: nombreArchivo
                },
            }).done(function (data) {
                if (data > 0) {
                    var url = '../Exportacion/Download/?nombreArchivo=PARAMETER';
                    window.location.href = url.replace("PARAMETER", nombreArchivo);
                    alert("Se descargaron " + data + " registros exitosamente!");
                }
                else {
                    alert("Ocurrió un error inesperado! Código de Error:" + data);
                }
            });
        }
        else
            console.log("Vacío");
    });
});

function getToday() {
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth()+1; //January is 0!

    var yyyy = today.getFullYear();
    if(dd<10){
        dd='0'+dd
    } 
    if(mm<10){
        mm='0'+mm
    } 
    var today = dd+'-'+mm+'-'+yyyy;
    return today
}

function OnBegin() {
    collapsePanel();
}

function OnFailure() {
    alert("Ha ocurrido un error inesperado!");
}

function OnSuccess(response) {
    if ($("#results-body").is(":visible")) {
        $("#results-body").hide("blind", 1000);
    }
    setTimeout(function () {
        $("#results-body").html(response);
        $("#results-body").show("blind", 1000);
    }, 5000);
}

function collapsePanel() {
    if (!$("#filtro").hasClass("col-md-4")) {
        $("#filtro").switchClass("col-md-12", "col-md-4", 2500);
        setTimeout(function () {
            $("#results").show("slide", 1000);
            $("#panel-not-collapsed").hide("slide", 500);
            setTimeout(function () {
                $("#panel-collapsed").show("slide", 500);
            }, 500);
        }, 2500);
    }
}

function expandPanel() {
    clearFields();
    if ($("#filtro").hasClass("col-md-4")) {
        $("#results").hide("slide", 1000);
        setTimeout(function () {
            $("#panel-collapsed").hide("slide", 500);
            setTimeout(function () {
                $("#panel-not-collapsed").show("slide", 500);
                $("#filtro").switchClass("col-md-4", "col-md-12", 2500);
            }, 500);
        }, 1000);
    }
}

function clearFields() {
    $("input").val("");
    $("#Empresa").val($("#Empresa option:first").val());
    $("#TipoMovimiento").val($("#TipoMovimiento option:first").val());
    $("#TipoArchivo").val($("#TipoArchivo option:first").val());
    $("input, select").removeClass("success");
    $("input, select").removeClass("error");
    
}