$(document).ready(function () {
    $(".btnPaginador").click(function () {
        $keyword = $("#keyword").val();
        $numPagina = $(this).data("pagina");

        $.ajax({
            url: "/Home/SearchKeyword",
            method: "GET",
            data: {
                keyword: $keyword,
                numPage: $numPagina
            }
        }).done(function (data) {
            $("#notifications-section").hide();
            $("#notifications-section").html(data);
            $("#notifications-section").show("fade");
        });

        $("html, body").animate({
            scrollTop: 0
        }, 700);
    });
});