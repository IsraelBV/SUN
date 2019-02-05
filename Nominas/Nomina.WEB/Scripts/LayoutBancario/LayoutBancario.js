var layoutBancario = (function () {
    //obtiene la vista de la tabla con la informacion de las nominas o finiquito y las muestra en pantalla
    var getTableLayout = function (empresa,idbanco) {
        //var empresa = $('select[id=empresa]').val();
        var request = $.ajax({
            url: "/LayoutBancario/tablaLayout",
            method: "POST",
            async: false,
            data:
            {
                idEmpresa: empresa,
                idBanco: idbanco
            },
            beforeSend: function () {
                waitingDialog.show('Procesando...');
            }
        });

        request.done(function (data) {
            waitingDialog.hide();
            $('#layout').html(data);
        });
    }

    //coloc el numero de empleados y el total del importe en el formilario
    //cambia el texto del boton
    var selectRow = function () {
        var count = $("#EmpleadosTesoreria tr.selected").length;
        $("#totalEmpleados").val(count);
        if (count != 0) {
            $("a.seleccionarall span").text("Deselecconar todos");
        } else {
            $("a.seleccionarall span").text("Selecconar todos");
        }

        var selected = $("#EmpleadosTesoreria").find("tbody tr.selected");

        var contador = 0;
        selected.each(function () {
            var importe = parseFloat($(this).data("importe"));
            contador = contador + importe;
        });

        var cont = contador.toFixed(2);
        $("#importeTotal").val(cont);

       
    }

    //inicializa el datatable para la tabla del layout
    var setDatatable = function () {
        $("table.EmpleadosTesoreria").DataTable({
            dom: 'Bfrtip',
            scrollY: '50vh',
            scrollX: true,
            scrollCollapse: true,
            paging: false,
            "language": { "url": "../scripts/datatables-spanish.json" },
            info: false,
            select: {
                style: 'multi'
            },
            "buttons": [ // lista de botones data table
                { 
                    className: 'seleccionarall btn btn-primary',
                    text: 'Seleccionar todos',
                    action: function () { // para seleccionar o quitar la seleccion de todos los botones

                        if ($("#EmpleadosTesoreria tr.selected").length != 0) {
                            $("#EmpleadosTesoreria tbody tr").removeClass("selected");
                        } else {
                            $("#EmpleadosTesoreria tbody tr").addClass("selected");
                        }

                        selectRow();
                    },
                    init: function (api, node, config) {
                        $(node).removeClass('dt-button')
                    },
                    key: 'a',
                },
                {
                    className: 'GetLayout btn btn-warning',
                    text: '<i class="fa fa-download"></i> Crear Layout',
                    //enabled: false,
                    action: function () { // enviar informacion al modelo para la creacion del layout
                        if (messageBtns($("#fecha, #EmpleadosTesoreria"))){ 
                            $(this).toggleClass('selected');
                            var selected = $("#EmpleadosTesoreria").find(".selected");
                            var Detalle = [];
                            selected.each(function () {
                                Detalle.push($(this).data("idempleado"));
                            });

                            var Encabezado = {};
                            Encabezado["TipoRegistro"] = $("#tipoRegistro").val();
                            Encabezado["ClaveServicio"] = $("#claveServicio").val();
                            Encabezado["Fecha"] = $("#fecha").val().replace(new RegExp('-', 'g'), "");
                            Encabezado["Consecutivo"] = $("#consecutivo").val();
                            Encabezado["RegistroDetalle"] = $("#registroDetalle").val();
                            Encabezado["Banco"] = $("#idBanco").val();

                            var request = $.ajax({
                                url: "/LayoutBancario/crearLayout",
                                method: "POST",
                                contentType: "application/json",
                                data: JSON.stringify({
                                    Encabezado: Encabezado,
                                    Detalle: Detalle,
                                    idEmpresa: $("#empresa").val()
                                }),
                                dataType: "json",
                                beforeSend: function () {
                                    waitingDialog.show('Generando Layout-......');
                                }
                            });
                            request.done(function (data) {
                                waitingDialog.hide();

                                var newdata2 = data.toString();
                                var rute = newdata2.split(",");

                                var contador = rute.length;
                                $("#consecutivo").val(contador);
                                for (var i = 0; i < contador; i++) {

                                    window.open('/LayoutBancario/descargarLayout?ruta=' + rute[i], '_blank');
                                }

                            })
                        }
                    },
                    init: function (api, node, config) {
                        $(node).removeClass('dt-button')
                    },
                },


            ],

        });
        $("a").removeClass("dt-button");
    }


    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> funcione dada de baja <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        //valida que exista un banco y una empresa seleccionados
        var getLayuotGridBtnOnOff = function ()
        {
            $("#empresa, #idBanco").change(function () {
                if ($("#empresa, #idBanco").val() == "") {
                    $("#btnGetList").prop("disabled", true);
                }
                else
                {
                    $("#btnGetList").prop('disabled', false);
                }
            });
        }

        //valida que exista una fecha seleccionada y porlo menos un empleado marcado
        var generarLayoutBtnOnOff = function () {
            
                if ($("#fecha").val() == "" || $("#EmpleadosTesoreria tr.selected").length == 0) {
                    $("a.GetLayout").addClass("disabled");
                }
                else {
                    $("a.GetLayout").removeClass('disabled');
                }
        }
    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>><<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


    var messageBtns = function (obj) {
        var ret = true;
        obj.each(function () {
            //$("#EmpleadosTesoreria tr.selected").length == 0
            if ($(this)[0].nodeName == "TABLE") {
                if ($(this).find("tr.selected").length == 0) {
                    utils.showMessage("ATENCION", "Seleccione al menos un empleado de la lista", 4000, "warning");
                    ret = false;
                }
            } else {
                if ($(this).val() == null || $(this).val() == "") {
                    utils.showMessage("ATENCION", "El campo <strong>" + $(this).data('info') + "</strong> no puede estar vacio", 4000, "warning");
                    ret = false;
                }
            }
        });
        return ret
    }

    var init = {
        tablaNomina: getTableLayout,
        seleccionarLinea: selectRow,
        tablaFormato: setDatatable,
       // disableGenLayBtn: generarLayoutBtnOnOff,
        messageBtn: messageBtns
    }

    return init;
})();


$(document).ready(function () {
    $("#empresa").change(function () {
        $("#layout").html("");
    });
    $("#btnGetList").click(function () {
        if (layoutBancario.messageBtn($("#empresa, #idBanco"))) {
            layoutBancario.tablaNomina($("#empresa").val(), $("#idBanco option[selected]").data("bancoid"));
            layoutBancario.tablaFormato();

            $('#EmpleadosTesoreria tbody').on('click', 'tr', function () {

                $(this).toggleClass('selected');
                layoutBancario.seleccionarLinea();
            });
        }
    });

});

