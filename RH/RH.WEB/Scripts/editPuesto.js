var boton1 = $(".boton1")
var boton2 = $(".boton2");
var modaledit = $("#modaledit");


boton2.click(function () {
    var Id = $(this).parent().parent().find(".tdId").html();
    $.get("../viewEdit?id=" + Id, function (data) {
        $("#puestoEdit").html(data);
        modaledit.modal();
    });
});

boton1.click(function () {
   
    var Id = $(this).data("sucursal");
   

    $.get("../viewCreate?id=" + Id, function (data) {
        $("#puestoEdit").html(data);
        modaledit.modal();
    });
});

function CloseModal() {
    modaledit.modal("hide");
    window.setTimeout(function () { location.reload() }, 1500);
}

$("#modaledit").on('hidden.bs.modal', function (e) {
    $('.modal-content').html("  <div id=\"puestoEdit\"></div>  ");
})