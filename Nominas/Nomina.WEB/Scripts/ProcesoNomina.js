$(document).on("click", "#proceso-nomina .btnProcesar", function () {
    
    //var selected = $("#table-proceso-nomina").find(".selected");
    var selected = $("#table-proceso-nomina").find(".selected");

    if (selected.length > 0) {
        utils.confirmDialog("¿DESEA CONTINUAR?", "Se procesarán " + selected.length + " nóminas", "CONFIRMAR", "CANCELAR", function(response) {
            if (response) {
                var empleados = [];
                selected.each(function() {
                    var id = parseInt($(this).data("id-empleado"));
                    empleados.push(id);
                });
                var request = $.ajax({
                    url: "/ProcesoNomina/ProcesarNominas/",
                    method: "POST",
                    data: {
                        empleados: empleados
                    },
                    beforeSend: function() {
                        $("#table-proceso-nomina").switchClass("table-multiselect", "table-disabled");
                        btnProcesar.html("<span class='glyphicon glyphicon-hourglass fa-spin'></span> PROCESANDO...");
                        btnProcesar.attr("title", "PROCESANDO SU NÓMINA, POR FAVOR ESPERE...");
                    }
                });
                request.done(function (result) {
                    Console.log(result.status + " hi");
                    Console.log(result.nominas);

                    if (result.nominas !== 0) {
                        utils.showMessage("PROCESAMIENTO COMPLETADO", result.nominas + " " + result.status, 10000);
                    } else {
                        utils.showMessage("Procesado", result.status, 10000);
                    }


                    utils.LoadActivePage();
                });
            }
        });
    } else {
        utils.showMessage("WAR", "Debe seleccionar los registros a procesar.", 8000);
    }


});

$(document).on("tableSelect:init", "#table-proceso-nomina", function(event, totalRows, rowsSelected, rowsUnselected) {
    bloquearBtnDetalleNomina(rowsSelected);
});

function bloquearBtnDetalleNomina(selected) {
    if (selected.length === 1) {
        if (selected.data("procesado") === true) 
            $(".btnDetalleNomina").attr("disabled", false);
        else
            $(".btnDetalleNomina").attr("disabled", true);
    }
    else {
        $(".btnDetalleNomina").attr("disabled", true);
    }
}

$(document).on("tableSelect:change", "#table-proceso-nomina", function (event, totalRows, rowsSelected) {
    bloquearBtnDetalleNomina(rowsSelected);
    if (rowsSelected.length > 0) {
        $("#proceso-nomina .btnProcesar").attr("disabled", false);
    }
    else {
        $("#proceso-nomina .btnProcesar").attr("disabled", true);
    }
});

$(document).on("click", ".module#proceso-nomina .btnDetalleNomina", function () {
    var selected = $("#table-proceso-nomina").find(".selected");
    if (selected.length == 1) {
        var IdNomina = parseInt(selected.data("id-nomina"));
        if (IdNomina > 0) {
            utils.loadMainPage("ProcesoNomina", "DetalleNomina", "#main-content", true, IdNomina);
        }
        else {
            utils.showMessage("¡Oops!", "Esta nómina no está procesada", 5000);
        }
    }
    else {
        utils.showMessage("¡Algo inesperado sucedió!", "Por favor selecciona sólo un registro para ver el detalle", 5000);
    }
});

$(document).on("change", ".module#detalleNomina .filtro", function () {
    var filtro = parseInt($(this).val());
    var table = $(".module#detalleNomina").find("#table-detalle-nomina");
    var rows = table.find("tbody").find("tr");

    rows.each(function () {
        var tipoConcepto = $(this).data("tipo-concepto");
        if (filtro !== -1) {
            if (tipoConcepto !== filtro) $(this).hide();
            else $(this).show();
        }
        else {
            $(this).show();
        }
    });

    reconstructDataTable(table);
});


$(document).on("click", ".module#proceso-nomina .btnReciboNomina", function () {

    var selected = $("#table-proceso-nomina").find(".selected");
    var nominas = [];


    selected.each(function () {
        var id = parseInt($(this).data("id-nomina"));
        nominas.push(id);
    });


    if (nominas.length > 0) {
        var form = document.createElement("form");
        form.setAttribute("method", "post");
        form.setAttribute("action", "/ProcesoNomina/GetRecibos");

        form._submit_function_ = form.submit;
        for (var i = 0; i < nominas.length; i++) {
            var hiddenField = document.createElement("input");
            hiddenField.setAttribute("type", "hidden");
            hiddenField.setAttribute("name", "idNominas");
            hiddenField.setAttribute("value", nominas[i]);
            form.appendChild(hiddenField);
        }

        document.body.appendChild(form);
        form._submit_function_();

    }
});