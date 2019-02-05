//Delete
var boton2 = $(".boton2");
var modaldelete = $("#modaldelete");


    boton2.click(function () {
        var idDepartamento = $(this).parent().parent().find(".tdId").html();
        $.get("../Departamentos/viewDelete?id=" + idDepartamento, function (data) {
            $("#depaDelete").html(data);
            modaldelete.modal();
        });
    });

    $("#modaldelete").on('hidden.bs.modal', function (e) {
        $('.modal-content').html("  <div id=\"depaDelete\"></div>  ");
    })