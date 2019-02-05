$(document).ready(function () {
    var tblClientes;
    var tblPeriodos;
    var tblSubsidio;
    var tblISR;
    var empleadoCatalogo;
 

    /**/
    $(document).ready(function () {
        var tblAsignar;
        var tblCatEmpleado;

        $("#modal-catalogos").on("shown.bs.modal", function () {
            tblCatEmpleado = $("#empleados").DataTable({
                "language": {
                    "url": "../scripts/datatables-spanish.json"
                },

                scrollY: "180px",
                paging: false,
                bInfo: false,
                initComplete: function () {
                    this.api().columns('.select-filter').every(function () {
                        var column = this;
                        var select = $('<select><option value=""></option></select>')
                            .appendTo($(column.footer()).empty())
                            .on('change', function () { 
                                var val = $.fn.dataTable.util.escapeRegex(
                                    $(this).val()
                                );

                                column
                                    .search(val ? '^' + val + '$' : '', true, false)
                                    .draw();
                            });

                        column.data().unique().sort().each(function (d, j) {
                            select.append('<option value="' + d + '">' + d + '</option>')
                        });
                    });
                }

            });

        });
      


        $("#modal-AsignarConcepto").on("shown.bs.modal", function () {
            tblAsignar = $("#conceptos2").DataTable({
                "language": {"url": "../scripts/datatables-spanish.json"},

                scrollY: "400px",
                paging: false,
                bInfo: false,
           

            });
            $("#conceptos2 tbody").delegate("tr", "click", function (event) {
             
                var $row = $(event.target);
                if ($row[0].tagName !== "TR") $row = $row.parent();
                var a = $row.toggleClass("selected");

                if (event.ctrlKey === false) {
                    $row.siblings().removeClass("selected");

                }
            });


            var seleccionados = [];

        });

        $("#modal-AsignarConcepto").on("hidden.bs.modal", function () {
            $("#conceptos2").find(".selected").removeClass("selected");
            tblAsignar.destroy();
        });
     

        $(document).on("click", "#conceptos2 tbody tr", function () {
            
            var seleccionados = tblAsignar.rows(".selected").ids();

            if (seleccionados.length === 1) {
                var idConcepto = tblAsignar.rows(".selected").ids();
                var id = idConcepto[0];
                $.ajax({
                    type: 'POST',
                    data: '',
                    dataType: 'html',
                    url: '/AsignarConcepto/BusquedaCE/?id=' + id,
                    beforeSend: function() {
                      
                    },
                    success: function (result) {
                        $(".divemp").html(result);
                     
                    },
                    error: function (error) {
                        // si hay un error lanzara el mensaje de error

                    }
                });

            }
            else if (seleccionados.length > 0) {


                var datos = $('#concep').parent().find('td input[type=checkbox]').prop('checked', $(this).is(''));

                $('.sele').parent().parent().removeClass('selected');
               // console.log(datos);

            }

            return seleccionados;
        })

    
    });


   




    

   
    //--------------Modales de catalogos----------------->

    $(".modal").on("shown.bs.modal", function () {
        var tables = $(this).find(".DataTable");
        if (tables.length > 0) {
            tables.each(function () {
                initTable($(this));
            });
        }

        var tableSelect = $(this).find(".table-select");
        if (tableSelect.length > 0) {
            tableSelect.each(function () {
                initTableSelect($(this));
            })
        }

        var tableMultiSelect = $(this).find(".table-multiselect");
        if (tableMultiSelect.length > 0) {
            tableMultiSelect.each(function () {
                initTableSelect($(this));
            })
        }
    });

    $(".modal").on("hidden.bs.modal", function () {
        var tables = $(this).find(".DataTable");
        if (tables.length > 0) {
            tables.each(function () {
                destroyTable($(this));
            });
        }

        var tableSelect = $(this).find(".table-select");
        if (tableSelect.length > 0) {
            tableSelect.each(function () {
                destroyTableSelect($(this));
            })
        }

        var tableMultiSelect = $(this).find(".table-multiselect");
        if (tableMultiSelect.length > 0) {
            tableMultiSelect.each(function () {
                destroyTableSelect($(this));
            })
        }
    });

    $(".btnSideBar").click(function () {
        $(".active-page").removeClass("active-page");
        $(this).parent().addClass("active-page");
    });
    
    $(document).keydown(function (tecla) {

        var activar =0;
        if (tecla.ctrlKey && tecla.altKey) {
            if (tecla.keyCode == 65)
            {
                var compl = $("input[name = 'complemento']").val();
                if (compl == 0) {
                    activar= 1;
                    // SI ESTA EN 0 -- SE CONVIERTE EN 1 Y LE ASIGNAMOS 'FISCAL/COMPLEMENTO'
                    $("input[name= 'complemento']").val('1');
                    $("#titulocomplmento").text('-- Fiscal/Complemento');
                    utils.showMessage("modo Fiscal/Complemento activado", "", 5000);
             
                    location.reload();

                 

                } else {
                    activar =0;
                    $("input[name= 'complemento']").val('0');
                    $("#titulocomplmento").text('-- Fiscal');
                    utils.showMessage("Modo Fiscal activado", "", 5000);
                    location.reload();
                }
                
                var request = $.ajax({
                    url: "/Home/ActivarComplemento/",
                    method: "POST",
                    data: {
                        activar: activar
                    }
                });
            }
            else if (tecla.keyCode == 66)
            {
                $('input[type=search]').focus();
            }

            //if (tecla.keyCode == 66) {
            //    $(".main-content-body").find(".btnSearch").click();
            //}
        }
        else if (tecla.altKey) {            
            if (tecla.keyCode == 66) {            
                $('input[type=search]').focus();
            }
        }
    });

    $("#tblClientes").on("tableUniselect:double-select", function () {
        var $row = $(this).find(".double-select");
        if ($row.length == 0) $row = $(this).find(".selected");
        $("#btnSelectCliente").attr("disabled", true);
        var $IdSucursal = $row.data("idsucursal");
        var request = selectSucursal($IdSucursal);
        request.done(function () {
            $("#modal-clientes").modal("hide");
            location.reload();
        });
    });

    $("#tblClientes").on("tableUniselect:select", function () {
        $("#btnSelectCliente").attr("disabled", false);
    });

    $("#btnSelectCliente").click(function () {
     
        $(this).attr("disabled", true);
        var selected = $("#tblClientes").find(".selected");
  
        if (selected > 0) {
          
            var idsucursal = selected.data("idsucursal");
        } else {
          
            var idsucursal = $("#tblClientes").find(".selected").data("idsucursal");
        }
       
        var request = selectSucursal(idsucursal);
        request.done(function () {
            $("#modal-clientes").modal("hide");
            location.reload();
        });
    });

    $("#btnSelectPeriodo").click(function () {
        var IdPeriodo = $("#tblSeleccionPeriodo").find(".selected").data("idperiodo");

        if (IdPeriodo != null) {
            var request = selectPeriodoPago(IdPeriodo);
          
            if (request !== false) {
                request.done(function() {
                    $("#modal-periodos").modal("hide");
                    location.reload();
                });
            }

        }
    });

    $(document).on("click", ".toggle-modal-periodos", function () {
        var sucursal = GetSucursalEnSession();
        if (sucursal > 0) {
            var request = $.ajax({
                url: "/Home/GetPeriodosPago/",
                method: "POST",
                data: {
                    id: 0//sucursal
                }
            });

            request.done(function (data) {
                $("#modal-periodos-body").html(data);
                $("#modal-periodos").modal("show");
            });
        }
    });

    $(document).on("tableUniselect:double-select", "#tblSeleccionPeriodo", function () {
        var $row = $(this).find(".double-select");

        $("#btnSelectPeriodo").attr("disabled", true);

        var IdPeriodo = $row.data("idperiodo");

        var request = selectPeriodoPago(IdPeriodo);
   
        if (request !== false) {
            request.done(function() {
                $("#modal-periodos").modal("hide");
                location.reload();
            });

        }
    });

    $(document).on("tableUniselect:select", "#tblSeleccionPeriodo", function () {
        $("#btnSelectPeriodo").attr("disabled", false);
    });

    $(document).on("tableUniselect:deselect", "#tblSeleccionPeriodo", function () {
        $("#btnSelectPeriodo").attr("disabled", true);
    });
    

    $(".toggle-collapse").click(function () {
        var $collapsed = $(this).siblings(".collapsed");
        var others = $(".collapsed").not($collapsed);
        others.each(function () {
            if ($(this).is(":visible"))
                $(this).hide("blind");
        });
        
        setTimeout(function () {
            if ($collapsed.is(":hidden"))
                $collapsed.show("blind");
            else
                $collapsed.hide("blind");
        }, 500);
        
    });

	var $sidebar = $("#side-bar");

	$("#toggle-side-bar").click(function () {
		if($sidebar.is(":visible")) {
			$sidebar.hide("slide");
		}
		else {
			$sidebar.show("slide");
		}
	});
	
	$(".logout").click(function () {
	    utils.confirmDialog("¿Estás seguro de querer cerrar sesión?", null, null, null, function (confirmation) {
	        if (confirmation) {
	            utils.LogOut();
	        }
	    });
	});

});

function togglePeriodos(cb) {
    if (cb.checked) {
        $("#tblSeleccionPeriodo").find(".periodoInactivo").show();
    }
    else {
        $("#tblSeleccionPeriodo").find(".periodoInactivo").hide();
    }
    var table = $("#tblSeleccionPeriodo");
    countNumRows(table);
    
}

$(document).on("click", ".btn-ajax", function () {
    var controller = $(this).data("controller");
    var action = $(this).data("action");
    var target = $(this).data("target");
    var hasDataTable = $(this).data("tables");
    var param = $(this).data("id");

    utils.loadMainPage(controller, action, target, hasDataTable, param);
});

var ajaxTables;

function selectSucursal(IdSucursal) {
    
    var request = $.ajax({
        url: "/Home/SelectSucursal/",
        method: "POST",
        data: {
            id: IdSucursal
        }
    });

    request.done(function (data) {
        var text = data.Nombre + " / " + data.Ciudad;
        $("#client-actual").html(text);
        $("#sucursal-activa").data("sucursal", data.IdSucursal);
        $("#periodo-actual").html("Periodos de Pago");
        $("#LoadingProcess").switchClass("visible", "hidden");
        $(".toggle-modal-periodos").data("periodo", 0);
        utils.LoadActivePage();
    });

    return request;
}

function selectPeriodoPago(idPeriodoPago) {
    
    if (idPeriodoPago === undefined) return false;
    if (isNaN(idPeriodoPago)) return false;

    var request = $.ajax({
        url: "/Home/SelectPeriodo/",
        method: "POST",
        data: {id: idPeriodoPago}
    });

    request.done(function (data) {
        if (data.Procesando === true) {
            $("#LoadingProcess").switchClass("hidden", "visible");
        }
        else {
            $("#LoadingProcess").switchClass("visible", "hidden");
        }

        $("#periodo-actual").html(data.Descripcion);
        $(".toggle-modal-periodos").data("periodo", idPeriodoPago);
        utils.LoadActivePage();
    });

    return request;
}

$.ajaxSetup({
    beforeSend: function () {
        utils.showProgress();
    },
    complete: function () {
        utils.hideProgress();
    },
    error: function (jqXHR, textStatus, errorThrown) {
       // utils.showMessage("Algo salió mal... COD:" + jqXHR.status + " - " + errorThrown, "Por favor contacta al administrador.", 0, "error");
    }
});

function GetSucursalEnSession() {
    var sucursal = $("#sucursal-activa").data("sucursal");
    return sucursal;
}

function showModal(e) {

    $("#modal-configuracion").find(".modal-title").text($(e).text());
    $("#modal-configuracion").modal("show");
}
//modal para empleados
function catalogoshowModal(e) {

    $("#modal-catalogos").find(".modal-title").text($(e).text());
    $("#modal-catalogos").modal("show");
}
//modal para bancos e incidencias
function showModalBI(e) {

    $("#modal-catalogosBI").find(".modal-title").text($(e).text());
    $("#modal-catalogosBI").modal("show");
}
function showModalAsignar(e) {

    $("#modal-AsignarConcepto").find(".modal-title").text($(e).text());
    $("#modal-AsignarConcepto").modal("show");
}
function showModalDispersion(e) {

    $("#modal-Dispersion").find(".modal-title").text($(e).text());
    $("#modal-Dispersion").modal("show");
}

function showModalFactura(e) {

    $("#modal-Factura").find(".modal-title").text($(e).text());
    $("#modal-Factura").modal("show");
}

function showModalClaveClientes(e) {

    $("#Modal_ClavesClientes").find(".modal-title").text($(e).text());
    $("#Modal_ClavesClientes").modal("show");
}


function selector() {
    var x = document.getElementById("selector").value;
    $("#pvistaParcial").load("/Configuracion/TablaSubsidio/?id=" + x);
}

function selectorISR() {
    var x = document.getElementById("ISR").value;
    $("#pvistaParcial").load("/Configuracion/TablaISR/?id=" + x);
}






