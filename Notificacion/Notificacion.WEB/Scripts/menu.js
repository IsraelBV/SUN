$(document).ready(function () {
    var $sidebar = $("#side-bar");
    var $main = $("main");

    $("#btnCollapseSidebar").click(function () {
        if ($sidebar.is(":hidden")) {
            $sidebar.show('slide', 500);
            $(this).find(".material-icons").html("close");
            $(this).addClass("active");
        }
        else {
            $sidebar.hide('slide', 500);
            $(this).find(".material-icons").html("menu");
            $(this).removeClass("active");
        }
    });

    $(".btn-side-bar").click(function () {
        $("#side-bar").find(".liactive").removeClass("liactive");
        $(this).parent().addClass("liactive");
    });

    $(window).ready(function () {
        noTooltip();
    });

    $("#setting").sideNav({
        menuWidth: 300, // Default is 240
        edge: 'right', // Choose the horizontal origin
        closeOnClick: true // Closes side-nav on <a> clicks, useful for Angular/Meteor
    });

    $(".btnSearch").click(function () {
        $(".other-items-in-nav").hide();
        $("#search-form").show("fade");
        $("#search").focus();
    });

    $("#search").focusout(function () {
        $("#search-form").hide();
        $(".other-items-in-nav").show("fade");
    });

    $("#search-form").submit(function (e) {
        e.preventDefault();

        $(".liactive").removeClass("liactive");
        $("#btnInbox").parent().addClass("liactive");

        LoadPage(0);
    });


    //Botones de acción para notificaciones
    $(".btnSelectAll").click(function () {
        $(".notification").map(function () {
            if (!$(this).hasClass("selected")) {
                selectNotification($(this));
            }
        });
        actionBar();
    });

    $(".btnClearAll").click(function () {
        $(".notification").map(function () {
            if ($(this).hasClass("selected")) {
                unselectNotification($(this));
            }
        });
        actionBar();
    });

    $("#btnNavAtendida").click(function () {
        $(".selected").map(function () {
            var $IdNotificacion = $(this).attr("id");
            MarcarComoAtendida($IdNotificacion, $(this));
        });

        $(this).parent().hide();
        $("#liAbierta").show();

        var $modificadas = $(".selected").length;
        var $titulo = $modificadas + " notificación marcada como atendida";
        if ($modificadas > 1)
            $titulo = $modificadas + " notificaciones marcadas como atendidas";
            
        Materialize.toast($titulo, 4000)
    });

    $("#btnNavLeida").click(function () {
        $(".selected").map(function () {
            var $IdNotificacion = $(this).attr("id");
            MarcarComoLeida($IdNotificacion, $(this));
        });
        $(this).parent().hide();
        $("#liAtendida").show();

        var $modificadas = $(".selected").length;
        var $titulo = $modificadas + " notificación marcada como leída";
        if ($modificadas > 1)
            $titulo = $modificadas + " notificaciones marcadas como leídas";

        Materialize.toast($titulo, 4000)
    });

    $("#btnNavPrint").click(function () {
        $(this).hide();
        $("#liLoading").show();
        printNotifications();

        $("#liLoading").hide();
        $(this).show();
    });

});



function noTooltip() {
    if (screen.width < 999) {
            $('.tooltipped').tooltip('remove');
    }
}