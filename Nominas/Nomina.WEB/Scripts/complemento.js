$(document).on("tableMultiselect:select", "#tblComplemento", function (evt, row) {
    let idEmpleado = parseInt(row.data("idEmpleado"));
    let request = $.ajax({
        url: "/EmpleadoComplemento/GetDetails/",
        method: "POST",
        data: {
            idEmpleado: idEmpleado
        },
        beforeSend: function () {
            $("#detailComplemento").empty();
        }
    });

    request.done(function (data) {
        $("#detailComplemento").html(data);
    });
});

$(document).on("tableMultiselect:multiselect tableSelect:selectAll", "#tblComplemento", function (evt, rows) {
    if (rows.length > 1) {
        var empleados = [];
        rows.each(function () {
            let idEmpleado = parseInt($(this).data("idEmpleado"));
            empleados.push(idEmpleado);
        });

        let request = $.ajax({
            url: "/EmpleadoComplemento/GetDetailsMixed/",
            method: "POST",
            data: {
                empleados: empleados
            },
            beforeSend: function () {
                $("#detailComplemento").empty();
            }
        });

        request.done(function (data) {
            $("#detailComplemento").html(data);
        });
    }
});

$(document).on("keydown", ".cantEmpleadoComplemento", function (evt) {
    let $input = $(this);
    if (evt.keyCode == 27) {
        let $row = $input.parent().parent("tr");
        let valor = parseFloat($row.data("valorAnterior"));
        $input.val(valor);
        $input.blur();
    }
    else if (evt.keyCode == 13) {
        evt.preventDefault();
        var inputs = $(".cantEmpleadoComplemento");
        var nextInput = inputs.get(inputs.index($input) + 1);
        if (nextInput) {
            nextInput.focus();
        }
        else {
            $input.blur();
        }
    }
});

$(document).on("focusout", ".cantEmpleadoComplementoSingle", function () {
    let $input = $(this);
    let $row = $input.parent().parent("tr");
    let cantidad = parseFloat($input.val());
    let cantidadAnterior = parseFloat($row.data("valorAnterior"));
    if (cantidad != cantidadAnterior) {
        let idEmpleadoComplento = parseFloat($(this).parent().parent("tr").data("idEmpleadoComplemento"));
        var notification;
        let request = $.ajax({
            url: "/EmpleadoComplemento/GuardarCantidadConcepto/",
            method: "POST",
            data: {
                IdEmpleadoComplemento: idEmpleadoComplento,
                Cantidad: cantidad
            },
            beforeSend: function () {
                notification = utils.showMessage("Cargando...", "Se están guardando sus cambios, por favor espere.", 0);
            }
        });

        request.done(function (data) {
            if (data.code > 0) {
                notification.update("title", "¡Datos guardados exitosamente!");
                $row.data("valorAnterior", cantidad);
            }
            else {
                notification.update("title", "Algo salió mal...");
            }
            notification.update("message", data.message);
            setTimeout(function () {
                notification.close();
            }, 5000);
        });

        request.fail(function () {
            $input.val(cantidadAnterior);
            notification.close();
        });
    }
});

$(document).on("focusout", ".cantEmpleadoComplementoMixed", function () {
    let $input = $(this);
    let cantidad = parseFloat($input.val());
    let $row = $input.parent().parent("tr");
    let valorAnterior = parseFloat($row.data("valorAnterior"));

    if (cantidad != valorAnterior) {
        let rows = $("#tblComplemento").find(".selected");
        let empleados = [];
        rows.each(function () {
            let idEmpleado = parseInt($(this).data("idEmpleado"));
            empleados.push(idEmpleado);
        });

        let concepto = parseInt($row.data("idConcepto"));
    
        var notification;
        let request = $.ajax({
            url: "/EmpleadoComplemento/GuardarCantidadConceptoMixed/",
            method: "POST",
            data: {
                idsEmpleado: empleados,
                IdConcepto: concepto,
                Cantidad: cantidad
            },
            beforeSend: function () {
                notification = utils.showMessage("Cargando...", "Se están guardando sus cambios, por favor espere.", 0);
            }
        });

        request.done(function (data) {
            if (data.code > 0) {
                notification.update("title", "¡Datos guardados exitosamente!");
                $row.data("valorAnterior", cantidad);
            }
            else {
                notification.update("title", "Algo salió mal...");
            }
            notification.update("message", data.message);
            setTimeout(function () {
                notification.close();
            }, 5000);
        });

        request.fail(function () {
            $input.val(valorAnterior);
            notification.close();
        });
    }
});