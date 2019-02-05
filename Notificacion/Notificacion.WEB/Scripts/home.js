$(document).ready(function () {
    if ($('#back-to-top').length) {
        var scrollTrigger = 100, // px
            backToTop = function () {
                var scrollTop = $(window).scrollTop();
                if (scrollTop > scrollTrigger) {
                    $('#back-to-top').addClass('show');
                } else {
                    $('#back-to-top').removeClass('show');
                }
            };
        backToTop();
        $(window).on('scroll', function () {
            backToTop();
        });
        $('#back-to-top').on('click', function (e) {
            e.preventDefault();
            $('html,body').animate({
                scrollTop: 0
            }, 700);
        });
    }

    function CambiarColor($this) {
        $(".linkactive").removeClass("linkactive");
        $this.addClass("linkactive");

        $(".active").removeClass("active");
        $this.parent().parent().addClass("active");
    }

    $("#btnAtendida").click(function () {
        var $this = $(this);
        var $id = $("#modal-notification-id").val();
        var $notification = $("#" + $id);
        MarcarComoAtendida($id, $notification);
        $this.hide();
        $("#btnReopen").show();
    });

    $("#btnReopen").click(function () {
        var $this = $(this);
        var $id = $("#modal-notification-id").val();
        var $notification = $("#" + $id);
        MarcarComoLeida($id, $notification);
        $this.hide();
        $("#btnAtendida").show();
    });

    $("#btnFavorite").click(function () {
        var $this = $(this);
        var $notification = $("#modal-notification-id").val();
        $.ajax({
            url: "/Home/AddFavorite/" + $notification
        }).done(function () {
            $this.hide();
            $("#btnUnfavorite").show();
        });
    });

    $("#btnUnfavorite").click(function () {
        var $this = $(this);
        var $notification = $("#modal-notification-id").val();
        $.ajax({
            url: "/Home/DeleteFavorite/" + $notification
        }).done(function () {
            $this.hide();
            $("#btnFavorite").show();
            if (Favorites()) {
                $("#" + $notification).parent().hide();
                $("#notification-details").closeModal();
            }
        });
    });

    $(".btn-side-bar").click(function () {
        LoadPage(0);
    });

    $("#btnComments").click(function () {
        showComments();
    });

    $("#btnCommentsh").click(function () {
        hideComments();
    });

    $("#new-comment-text").keypress(function () {
        if ($(this).hasClass("invalid"))
            $("#btnComment").prop("disabled", true);
        else if ($(this).val() == "")
            $("#btnComment").prop("disabled", true);
        else
            $("#btnComment").prop("disabled", false);
    });

    $("#btnPrint").click(function () {
        printNotification();
    });
});

$(document).on("click", ".notification", function () {
    var $id = $(this).attr("id");
    openModalNotification($id);
});

$(document).on("click", "#filter-title", function () {
    LoadPage(0)
});

$(document).on("click", ".user-img", function (e) {
    e.stopImmediatePropagation();
    console.log("Hola Mundo");
    var $notification = $(this).parent();
    var $status = $notification.data("status");

    if ($notification.hasClass("selected")) {
        unselectNotification($notification);
    }
    else {
        selectNotification($notification);
    }

    actionBar();
});

function printNotification() {
    var doc = new jsPDF();
    var idNotificacion = $("#modal-notification-id").val();
    var notificacion = $("#" + idNotificacion);
    var title = notificacion.find(".title").text();
    var fecha = notificacion.data("date");
    var cliente = notificacion.find(".client").text();
    var user = notificacion.find(".user-img").data("tooltip");
    var type = notificacion.data("type");    
    var array = $(".collection-item").map(function () {
        return $(this).text();
    }).get();
    printBody(title, fecha, cliente, user, type, array, doc);
    doc.save(type + " -" + title + '.pdf');
}

function printBody(title, fecha, cliente, user, type, datos, document) {
    document.setFontSize(20);
    document.text(title, 105, 20, 'center');
    var subtitle = fecha + " Cliente: " + cliente + " Generada por: " + user + " Tipo: " + type;
    document.setFontSize(10);
    document.text(subtitle, 105, 25, 'center');
    document.setLineWidth(0.5);
    document.line(15, 30, 200, 30);
    var height = 40;
    document.setFontSize(10);
    for (var i = 0; i < datos.length; i++) {
        document.text(20, height, datos[i]);
        height = height + 5;
    }
}

function printNotifications() {
    var $selected = $(".selected");
    

    var $notificaciones = $selected.map(function () {
        return parseInt($(this).attr("id"));
    }).get();

    console.log($notificaciones);

    var request = $.ajax({
        url: "/Home/GetNotificacionsForPrint",
        method: "POST",
        data: {
            ids: $notificaciones
        }
    });

    request.done(function (data) {
        if (data != null) {
            var doc = new jsPDF();
            for (var i = 0; i < data.length; i++) {
                var usuario = data[i].Usuario + " " + data[i].UsuarioPaterno + " " + data[i].UsuarioMaterno;
                printBody(data[i].Titulo, data[i].FechaString, data[i].Cliente, usuario, data[i].TipoDescripcion, data[i].Datos, doc);
                if (i < data.length - 1)
                    doc.addPage();
            }
            doc.save("Notificaciones.pdf");
        }


    });
}

function showComments() {
    var $IdNotificacion = $("#modal-notification-id").val();
    loadComments($IdNotificacion);
    $("#notification-content").switchClass("content-expanded", "content-collapsed", 1000);
    setTimeout(function () {
        $("#notification-comments").show("blind", 1000);
    }, 1200);
    $("#btnComments").hide("fade", 200);
    $("#btnCommentsh").show("fade", 200);
}

function hideComments() {
    setTimeout(function () {
        $("#notification-comments").hide("blind", 1000);
    }, 1200);
    $("#notification-content").switchClass("content-collapsed", "content-expanded", 1000);
    $("#notification-action-comment").show();
    $("#btnCommentsh").hide("fade", 200);
    $("#btnComments").show("fade", 200);
}

function Favorites() {
    var $enlace = $(".liactive").find(".btn-side-bar");
    var $filtro = $enlace.data("filter");
    if ($filtro == 1)
        return true;
    else
        return false;
}

function LoadPage(page) {
    $("#notifications-section").hide();
    $(".progress").removeClass("hide");
    var $enlace = $(".liactive").find(".btn-side-bar");
    var $filtro = $enlace.data("filter");
    var $extra = $enlace.data("filter-extra");
    var $filterTitle = $enlace.data("tooltip");
    var $keyword = $("#search").val();
    $(".user").tooltip("remove");

    if ($keyword) {
        $filterTitle = "Búsqueda: <b>" + $keyword + "</b>";
    }

    var request = $.ajax({
        url: "/Home/Notifications",
        method: "GET",
        data: {
            numPage: page,
            filtro: $filtro,
            extra: $extra,
            keyword: $keyword
        }
    });

    request.done(function (data) {
        $(".progress").addClass("hide");
        $("#notifications-section").html(data);
        $("#filter-title").html($filterTitle);
        var $color = $(".liactive").find("i").css("color");
        $("#pagination-header").css("border-bottom","2px solid " + $color);
        $("#notifications-section").show("fade");
        actionBar();
        $(".user-img").tooltip({ delay: 50 });
    });

    $("html, body").animate({
        scrollTop: 0
    }, 700);

    
}

function openModalNotification($idNotificacion) {
    loadData($idNotificacion);

    if (!$("#notification-details").hasClass("open")) {
        //Abre el modal
        $("#notification-details").openModal({
            complete: function () {
                $(".modal-status-buttons").hide();
                $("#modal-title").removeClass();
                $(".modal-favorite-buttons").hide();
                $("#modal-notification-id").val("");
                $("#notification-content").html("");
                $("#notification-comments").html("");
                hideComments();
            },
        });
    }

}

function loadData($idNotificacion) {
    //carga los datos de la notificación
    $.ajax({
        url: "/Home/NotificationDetails/" + $idNotificacion
    }).done(function (data) {
        if (data != null) {
            $("#notification-comments").html("");
            $(".modal-status-buttons").hide();
            $(".modal-favorite-buttons").hide();

            $("#modal-notification-title").empty();
            $("#modal-notification-title").html(data.Titulo);
            $("#modal-title").removeClass();
            $("#modal-title").addClass(data.TipoDescripcion);

            $("#notification-content").empty();
            $("#notification-content").append(data.Contenido);

            $("#modal-notification-id").val("");
            $("#modal-notification-id").val(data.IdNotificacion);

            if (data.Favorita) {
                $("#btnUnfavorite").show();
            }
            else {
                $("#btnFavorite").show();
            }

            changeStatus($idNotificacion);
        }
    });
}

function loadComments($idNotificacion) {
    //Carga los comentarios de la notificación
    $.ajax({
        url: "/Home/NotificationComments/" + $idNotificacion,
    }).done(function (data) {
        $("#notification-comments").html(data);
    });
}

function changeStatus($idNotificacion) {
    var $notification = $("#" + $idNotificacion);
    var $status = $notification.data("status");
    switch ($status) {
        case 1: //si es vista
            $("#btnAtendida").show("fade", 1000);
            break;
        case 2: //si es nueva
            MarkAsSeen($idNotificacion, $notification);
            break;
        case 3: //si es atendida
            $("#btnReopen").show("fade", 1000);
            break;
        default: //si es nueva
            MarkAsSeen($idNotificacion, $notification);
            break;
    }
}

function MarkAsSeen($IdNotificacion, $notificacion) {
    $.ajax({
        url: "/Home/MarkAsRead/" + $IdNotificacion
    }).done(function (data) {
        if (data) {
            var $chip = $notificacion.find(".status");
            var $icon = $chip.find(".material-icons").html("drafts");
            $icon.attr("class", "material-icons blue-grey-text text-lighten-4");
            $("#btnAtendida").show("fade", 1000);
            $notificacion.data("status", "1");
        }
    });
};

function MarcarComoLeida($IdNotificacion, $notificacion) {
    var $chip = $notificacion.find(".status");
    $.ajax({
        url: "/Home/MarkAsReopened/" + $IdNotificacion
    }).done(function (data) {
        if (data) {
            $notificacion.data("status", "1");
            var $icon = $chip.find(".material-icons").html("drafts");
            $icon.attr("class", "material-icons blue-grey-text text-lighten-4");
        }
    });
}

function MarcarComoAtendida($IdNotificacion, $notificacion) {
    var $chip = $notificacion.find(".status");
    $.ajax({
        url: "/Home/MarkAsDone/" + $IdNotificacion
    }).done(function (data) {
        if (data) {
            $notificacion.data("status", "3");
            $chip.attr("class", "status blue-grey-text text-lighten-4");
            $chip.find(".material-icons").html("work");
        }
    });
}

function CommentBegin() {
    $("#new-comment-text").val("");
}

function CommentSuccess(data) {
    if (data > 0) {
        $.ajax({
            url: "/Home/GetComment/" + data
        }).done(function (data) {
            $("#notification-comments").prepend(data);
            if ($("#notification-content").hasClass("content-expanded")) {
                showComments();
            }
        });
    }
    else {
        Materialize.toast('Error ingresando el comentario!', 4000);
    }
}

function CommentFailure(data) {
    Materialize.toast('Error ingresando el comentario!', 4000);
}

function selectNotification($notification) {
    var $img = $notification.find(".user-img");
    $notification.addClass("selected");
    $img.html("");
    $img.append("<i class='material-icons'>done</i>");
}

function unselectNotification($notification) {
    var $img = $notification.find(".user-img");
    $notification.removeClass("selected");
    var $data = $img.data("user");
    $img.html($data);
}

function actionBar() {
    var numSelected = $(".selected").length;
    var numNotifications = $(".notification").length;

    if (numSelected > 0) {
        if ($("#actionNav").is(":hidden")) {
            var $firstNotification = $(".selected:first");
            var $status = $firstNotification.data("status");
            switch ($status) {
                case 2: //Nueva
                    $("#liAbierta").show();
                    break;
                case 3: //Atendida
                    $("#liAbierta").show();
                    break;
                case 1: //Abierta
                    $("#liAtendida").show();
                    break;
                default:
                    $("#liAbierta").show();
                    break;
            }

            $("#defaultNav").hide("fade", 200);
            $("#btnInicio").addClass("actionBarActive");
            setTimeout(function () {
                $("#actionNav").show("fade", 200);
            }, 250);
        }
    }
    else {
        $("#actionNav").hide("fade", 200);
        setTimeout(function () {
            $("#defaultNav").show("fade", 200);
            $("#btnInicio").removeClass("actionBarActive");
        }, 250);
        $("#liAbierta").hide();
        $("#liAtendida").hide();
    }

    if (numSelected == numNotifications) {
        $(".liSelectAll").hide();
        $(".liClearAll").show();
    }
    else if (numSelected > 0) {
        $(".liSelectAll").show();
        $(".liClearAll").show();
    }
    else if (numSelected == 0) {
        $(".liSelectAll").show();
        $(".liClearAll").hide();
    }
}