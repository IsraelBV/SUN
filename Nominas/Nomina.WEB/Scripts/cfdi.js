function onChangeTableCFDIs(selected) {
    var pending = selected.filter(".pending");
    if (pending.length > 0) {
        $("#btnGenerateCFDIs").attr("disabled", false);
    }
    else {
        $("#btnGenerateCFDIs").attr("disabled", true);
    }

    var ready = selected.filter(".ready");

    if (ready.length > 0) {
        $("#downloadRecibos").attr("disabled", false);
    }
    else {
        $("#downloadRecibos").attr("disabled", true);
    }
};

$(document).on("tableSelect:init", "#table-generar-cfdis", function (event, rows, selected, unselected) {
    onChangeTableCFDIs(selected);
});

$(document).on("tableSelect:change", "#table-generar-cfdis", function (event, rows, selected) {
    onChangeTableCFDIs(selected);
});

$(document).on("click", "#btnGenerateCFDIs", function () {
    var selected = $("#table-generar-cfdis").find(".selected.pending");
    var nominas = [];
    selected.each(function() {
        var idNomina = parseInt($(this).data("idnomina"));
        nominas.push(idNomina);
    });



    if (selected.length > 0) {
        utils.confirmDialog("¿DESEA CONTINUAR?", "SE GENERARÁN " + nominas.length + " CFDI's", "CONFIRMAR", "CANCELAR", function(confirm) {
            if (confirm) {
                var request = $.ajax({
                    url: "/Cfdi/GenerarCfdi",
                    method: "POST",
                    data: {
                        idNominas: nominas
                    },
                    beforeSend: OBGenerateCFDIs()
                });

                request.done(function(data) {
                    selected.each(function() {
                        var $row = $(this);
                        var IdNomina = parseInt($row.data("idnomina"));
                        var filter = $.grep(data, function(d) {
                            if (d.IdNomina === IdNomina) return d;
                        });

                        if (filter.length > 0) {
                            $row.removeClass("pending").addClass("ready");
                            $row.find("[data-column='uddi']").html(filter[0].Uddi);
                            $row.find("[data-column='fechaCert']").html(filter[0].FechaTimbrado);
                            $row.find(".label").html("GENERADO").switchClass("label-default", "label-success");
                            utils.showMessage("NÓMINA " + IdNomina, "Nómina procesada correctamente.", 3000);
                        } else {
                            $row.find(".label").html("ERROR").switchClass("label-default", "label-danger");
                            utils.showMessage("NÓMINA " + IdNomina, "La generación del CFDI tuvo un error inesperado.", 5000, "error");
                        }

                    });
                    onChangeTableCFDIs(selected);
                });

                request.always(OCGenerateCFDIs);
            }
        });
    } else {
        utils.showMessage("CFDI ", "Debe seleccionar los registros con estatus pendiente", 5000, "info");
    }
});

$(document).on("click", "#downloadRecibos", function () {
    var selected = $("#table-generar-cfdis").find(".selected.ready");

    var nominas = [];

    selected.each(function () {
        var IdNomina = parseInt($(this).data("idnomina"));
        nominas.push(IdNomina);
    });

    if (selected.length > 0) {
        utils.confirmDialog("Se descargarán " + selected.length + " recibos", "¿Desea continuar?", "SI", "NO", function(response) {
            if (response) {
                var form = document.createElement("form");
                form.setAttribute("method", "post");
                form.setAttribute("action", "/Cfdi/DownloadRecibos");

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

    } else {
        utils.showMessage("CFDI ", "Debe seleccionar los registros con estatus listo", 5000, "warning");
    }

});

function checkRecibos() {
    var selected = (".selected[data-cfdi-status='ready']");
    if (selected.length > 0) {
        $("#downloadRecibos").attr("disabled", false);
    }
    else {
        $("#downloadRecibos").attr("disabled", true);
    }
}

function OBGenerateCFDIs() {
    $("#table-generar-cfdis").switchClass("table-multiselect", "table-disabled");
    var btn = $("#btnGenerateCFDIs");
    btn.html("<span class='glyphicon glyphicon-hourglass fa-spin'></span> GENERANDO CFDI...");
    btn.attr("disabled", true);
    btn.attr("title","GENERANDO CFDIs, POR FAVOR ESPERE");
}

function OCGenerateCFDIs() {
    var btn = $("#btnGenerateCFDIs");
    btn.html("<span class='glyphicon glyphicon-download-alt'></span> GENERAR CFDIs");
    btn.attr("disabled", false);
}
