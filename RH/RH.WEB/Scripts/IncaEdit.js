var boton2 = $(".boton2");
var boton3 = $(".boton3");
var modaledit = $("#modaledit");


boton2.click(function () {
    var Id = $(this).parent().parent().find(".tdId").html();
    $.get("../viewEdit?id=" + Id, function (data) {
        $("#incaEdit").html(data);
        modaledit.modal();
    });
});

boton3.click(function () {
    var Id = $(this).parent().parent().find(".tdId").html();
    $.get("../viewDelete?id=" + Id, function (data) {
        $("#incaEdit").html(data);
        modaledit.modal();
    });
});

function CloseModal() {
    modaledit.modal("hide");
    window.setTimeout(function () { location.reload() }, 1500);
}

$("#modaledit").on('hidden.bs.modal', function (e) {
    $('.modal-content').html("  <div id=\"incaEdit\"></div>  ");
})