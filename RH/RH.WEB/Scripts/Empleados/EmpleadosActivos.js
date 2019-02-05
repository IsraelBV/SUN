$(document).ready(function () {
    var table = $("#tblEmpleados").DataTable({
        select: {
            style: 'multi'
        },
        scrollY: "60vh",
        paging: false,
        "language": {
            "sInfoFiltered": "(filtrado de un total de _MAX_ Empleados)",
            "sInfo": "Del _START_ al _END_ de un total de _TOTAL_ empleados",
            "sZeroRecords": "No se encontraron resultados",
            "infoEmpty": "No se encontraron registros",
            "sEmptyTable": "Ningún dato disponible en esta tabla",
            "sLoadingRecords": "Cargando...",
            "sLengthMenu": "Mostrar _MENU_ Empleados",
            "sSearch": "Buscar:",
            select: {
                rows: {
                    _: "Has seleccionado %d registros",
                    0: "Haz click en los registros para seleccionarlos"
                }
            }
        },
        "order": [[1, "asc"]],
        "aoColumnDefs": [
            { "bSortable": false, "aTargets": [0] },
            { "bSearchable": false, "aTargets": [0] }
        ]
    });

    table.on('select', function (e, dt, type, indexes) {
        var rowData = table.rows(indexes).ids();
        toolbarActivos();
        $("#input" + rowData[0]).prop("checked", true).change();

    });

    table.on('deselect', function (e, dt, type, indexes) {
        var rowData = table.rows(indexes).ids();
        toolbarActivos();
        $("#input" + rowData[0]).prop("checked", false).change();
    });

    $('a[data-toggle="tab"]').on('shown.bs.tab', function () {
        $("input[name='input-activos']").prop("checked", false);
        $("input[name='input-activos']").change();
        table.rows().deselect();
    });

    function toolbarActivos()
    {
        var seleccionados = table.rows(".selected").ids();
         //console.log(seleccionados.length);
        if ($(".btn-toolbar-hidden").is(":visible"))
        {
            if (seleccionados.length > 1) {
                $("#btn-toolbar-one").hide();
            }
            else if (seleccionados.length == 0) {
                $(".btn-toolbar-hidden").hide();
            }
            else {
                $("#btn-toolbar-one").show();
            }
        }
        else
        {
                if (seleccionados.length > 0)
                {
                    $(".btn-toolbar-hidden").show();
                
                }
        }


        //si se selecciona algun colaborador con una incapacidad, no se mostrara el boton de baja 
        var contIncapacidad = getIncapacidad();
        console.log(contIncapacidad.length);
            if (contIncapacidad > 0)
            {
                $("#btn-baja").hide();
            }
            else if (seleccionados.length > 0)
            {
                $("#btn-baja").show();
            }
    }
        
    });

    
//verifica cuantos colaboradores seleccionados tienen una incapacidad
function getIncapacidad() {
    var cont = 0;
    $("#tblEmpleados tbody tr").each(function () {
        if($(this).hasClass('selected'))
        if ($(this).find('#incap').hasClass('IncapacidadClass'))
        {
            cont++;
            console.log(cont);

        }

       
    });
    return cont;
}

