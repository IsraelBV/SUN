var boton2 = $(".boton2");
var modaledit = $("#modaledit");


boton2.click(function () {
    var Id = $(this).parent().parent().find(".tdId").html();
    $.get("../TiposInasistencia/viewEdit?id=" + Id, function (data) {
        $("#incidenciasEdit").html(data);
        modaledit.modal();
    });
});

function CloseModal() {
    modaledit.modal("hide");
    window.setTimeout(function () { location.reload() }, 1500);
}

$("#modaledit").on('hidden.bs.modal', function (e) {
    $('.modal-content').html("  <div id=\"incidenciasEdit\"></div>  ");
})