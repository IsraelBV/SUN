$(document).ready(function () {
    var mrNombreEmpleado = $("#mr-nombre-empleado");
    var modalRecontratacion = $("#modal-recontratacion");

    //este evento despliega el modal para recontratación al hacer clic en el botón de cada fila de empleados inactivos
    $("#btnModalRecontratacion").click(function () {
        var seleccionados = $("input[name='input-inactivos']:checked").map(function () {
            return $(this).val()
        }).get();

        if (seleccionados.length == 1) {
            
            var idEmpleado = seleccionados[0];
            //var idEmpleado = $(this).parent().parent().find(".tdId").html();
            //var row = $(this).closest("tr");
            $("#mr-id-empleado").val(idEmpleado);
            //var nombreEmpleado = $(this).parent().parent().find(".tdNombre").html();
            //mrNombreEmpleado.html(nombreEmpleado);
            $.ajax({
                type: "POST",
                url: "Empleados/ObtenerContrato",
                data: '{idEmpleado: '+idEmpleado+ '}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {     
                    $("#lidempf").html($("#formRecontratacion").find("#IdEmpresaFiscal option[value='" + response.IdEmpresaFiscal + "']").text());
                    $("#lidempc").html($("#formRecontratacion").find("#IdEmpresaComplemento option[value='" + response.IdEmpresaComplemento + "']").text());
                    $("#lidemps").html($("#formRecontratacion").find("#IdEmpresaSindicato option[value='" + response.IdEmpresaSindicato + "']").text());
                    $("#lidempa").html($("#formRecontratacion").find("#IdEmpresaAsimilado option[value='" + response.IdEmpresaAsimilado + "']").text());
                    $("#ltc").html($("#formRecontratacion").find("#TipoContrato option[value='" + response.TipoContrato + "']").text());
                    $("#lpuesto").html($("#formRecontratacion").find("#IdPuesto option[value='" + response.IdPuesto + "']").text());
                    $("#lfa").html('' + ToJavaScriptDate(response.FechaAlta) + '');
                    $("#lfr").html('' + ToJavaScriptDate(response.FechaReal) + '');
                    $("#lfi").html('' + ToJavaScriptDate(response.FechaIMSS) + '');
                    $("#lsr").html('$' + response.SalarioReal + '');
                    $("#lsd").html('$' + response.SD + '');
                    $("#lsdi").html('$' + response.SDI + '');
                },
                failure: function (response) {
                    alert("Error al buscar contrato anterior.");
                }
            });
            modalRecontratacion.modal();
        }
        else {

        }
    });

    var tblInactivos = $("#tblInactivos").DataTable({
        paging: false,
        "language": {
            "sInfoFiltered": "(filtrado de un total de _MAX_ Empleados)",
            "sInfo": "Del _START_ al _END_ de un total de _TOTAL_ empleados",
            "infoEmpty": "No se encontraron registros",
            "sZeroRecords": "No se encontraron resultados",
            "sEmptyTable": "Ningún dato disponible en esta tabla",
            "sLoadingRecords": "Cargando...",
            "sLengthMenu": "Mostrar _MENU_ Empleados",
            "sSearch": "Buscar:",
            select: {
                rows: {
                    _: "Has seleccionado %d registros",
                    0: "Haz click en los registros para seleccionarlos"
                }
            },
        },
        "order": [[1, "asc"]],
        "aoColumnDefs": [
            { "bSortable": false, "aTargets": [0] },
            { "bSearchable": false, "aTargets": [0] }
        ],
        select: {
            style: 'multi'
        },
        scrollY: "55vh",
    });

    tblInactivos.on('select', function (e, dt, type, indexes) {
        var rowData = tblInactivos.rows(indexes).ids();
        toolbarInactivos();
        $("#inactivo" + rowData[0]).prop("checked", true).change();
    });

    tblInactivos.on('deselect', function (e, dt, type, indexes) {
        var rowData = tblInactivos.rows(indexes).ids();
        toolbarInactivos();
        $("#inactivo" + rowData[0]).prop("checked", false).change();
    });

    $('a[data-toggle="tab"]').on('shown.bs.tab', function () {
        $("input[name='input-inactivos']").prop("checked", false);
        $("input[name='input-inactivos']").change();
        tblInactivos.rows().deselect();
    });

    function toolbarInactivos() {
        var seleccionados = tblInactivos.rows('.selected').ids();
        if (seleccionados.length > 0 && seleccionados.length < 2) {
            $(".btn-toolbar-inactivos-hidden").show();
        }
        else if (seleccionados.length > 1) {
            $(".btn-toolbar-inactivos-one").hide();
        }
        else {
            $(".btn-toolbar-inactivos-hidden").hide();
        }
        
    };
});

function ToJavaScriptDate(value) {
    var pattern = /Date\(([^)]+)\)/;
    var results = pattern.exec(value);
    var dt = new Date(parseFloat(results[1]));
    return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
}