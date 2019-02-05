$(document).ready(function () {
    var input = $("#prox");

    $(".PicImage").click(function () {

        input.addClass("InputUpload");
        input.slideDown(800)
    });

    $(".upCancel").click(function () {

        input.addClass("InputUpload");
        input.slideUp(800)
    });
})