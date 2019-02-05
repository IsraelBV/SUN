$(document).ready(cargarPagina(0));

$(document).on("click", ".navbar-toggle", toggleSideBar);

/*Sección de sidebar*/
$(document).on("click", ".btn-sidebar", function (e) {
	e.preventDefault();
	let $btn = $(this);
	
	$(".btn-sidebar.active").removeClass("active");
	$btn.addClass("active");

	cargarPagina(0);

	let title = $btn.html();
	$("#buzon-title").find("#bznTitle").html(title);
});

$(document).on("click", ".label-filter", function (e) {
    let $label = $(this);
    if (!e.ctrlKey) {
        $label.switchClass("label-inactive", "label-active", 0);
        $label.siblings(".label-active").switchClass("label-active", "label-inactive",0);
    }
    else {
        if ($label.hasClass("label-active"))
            $label.switchClass("label-active", "label-inactive", 0);
        else
            $label.switchClass("label-inactive", "label-active", 0);
    }
    cargarPagina(0);
});


/*Sección Lista de notificaciones*/

$(document).on("keyup", function (e) {
    if (e.keyCode == 40) {
        $(".notification.active").next(".notification").click();
    }
    if (e.keyCode == 38) {
        $(".notification.active").prev(".notification").click();
    }
});

$(document).on("click", "#buzon-title", function () {
    cargarPagina(0);
});

$(document).on("click", ".btn-pagination", function () {
    if (!$(this).attr("disabled")) {
        let pagina = parseInt($(this).data("pagina"));
        cargarPagina(pagina);
    }
});

$(document).on("keyup", "#keyword", function (e) {
    if ($(this).val() != "") {
        $("#clearKeyword").removeClass("disabled");
    }
    else {
        $("#clearKeyword").addClass("disabled");
    }
    if (e.keyCode == 13) {
        cargarPagina(0);
    }
    else if (e.keyCode == 27) {
        $(this).val("");
        $("#clearKeyword").addClass("disabled");
        cargarPagina(0);
    }
});

$(document).on("click", "#clearKeyword", function () {
    $("#keyword").val("");
    $("#clearKeyword").addClass("disabled");
    cargarPagina(0);
});

$(document).on("click", ".btn-selector", function (e) {
    e.stopImmediatePropagation();
    let $notification = $(this).parents(".notification");
    let IdNotificacion = parseInt($notification.data("idNotificacion"));
    if ($notification.hasClass("selected")) {
        removeFromNotificationsSelected(IdNotificacion);
        deselectNotification($notification);
    }
    else {
        addToNotificationSelected(IdNotificacion);
        selectNotification($notification);
    }
    toggleToolbar();
});

$(document).on("click", "#btnDeselectAll", function () {
    for (let i in NotificationsSelected) {
        let IdNotificacion = NotificationsSelected[i];
        let $notification = $(".notification").filterByData("idNotificacion", IdNotificacion);
        deselectNotification($notification);
    }
    NotificationsSelected = [];
    $(".toolbar").hide();
});

$(document).on("click", "#btnSelectAll", function () {
    let $notifs = $(".notification").not(".selected");
    $notifs.each(function () {
        let $notif = $(this);
        let IdNotif = parseInt($notif.data("idNotificacion"));
        addToNotificationSelected(IdNotif);
        selectNotification($notif);
    });
});

$(document).on("click", ".btn-toolbar#btnPrint", function () {
    let consulta = $.ajax({
        url: "/Home/GetDataForPrint/",
        method: "POST",
        data: {
            idNotificaciones: NotificationsSelected
        }
    }).done(function (data) {
        let doc = new jsPDF();
        for (var i = 0; i < data.length; i++) {
            printBody(data[i].Titulo, data[i].FechaString, data[i].Cliente, data[i].Usuario, data[i].TipoDescripcion, data[i].Datos, doc);
            if (i < data.length - 1)
                doc.addPage();
        }
        doc.save("Notificaciones.pdf");
    })
});

$(document).on("click", "#btnArchivar", function () {
    let request = archivarNotificaciones(NotificationsSelected);
    request.done(function (data) {
        console.log(data.length + " archivadas");
        if (data.length > 0) {
            tbSwitchToDesarchive(data);
            toast(data.length + " archivadas", "DESHACER", function (undo) {
                if (undo) {
                    desarchivarNotificaciones(data).done(function (data2) {
                        if (data2.length > 0)
                            tbSwitchToArchive(data2);
                    });
                }
            })
        }
    });
});

$(document).on("click", "#btnDesarchivar", function () {
    let request = desarchivarNotificaciones(NotificationsSelected);
    request.done(function (data) {
        console.log(data.length + " archivadas");
        if (data.length > 0) {
            tbSwitchToArchive(data);
            toast(data.length + " desarchivadas", "DESHACER", function (undo) {
                if (undo) {
                    archivarNotificaciones(data).done(function (data2) {
                        if (data2.length > 0)
                            tbSwitchToDesarchive(data2);
                    });
                }
            })
        }
    })
});

function tbSwitchToDesarchive(data) {
    $("#btnArchivar").hide();
    $("#btnDesarchivar").show();
    let idNotifActiva = parseInt($(".detail").data("idNotificacion"));
    if (idNotifActiva > 0) {
        let index = data.indexOf(idNotifActiva);
        if (index >= 0) {
            $(".btnArchiveNotification").data("status", 3);
            $(".btnArchiveNotification").find(".glyphicon").switchClass("glyphicon-folder-close", "glyphicon-folder-open", 0);
        }
    }
    $("#btnDeselectAll").click();
    let page = getPaginaActiva();
    cargarPagina(page);
}

function tbSwitchToArchive(data) {
    $("#btnDesarchivar").hide();
    $("#btnArchivar").show();
    let idNotifActiva = parseInt($(".detail").data("idNotificacion"));
    if (idNotifActiva > 0) {
        let index = data.indexOf(idNotifActiva);
        if (index >= 0) {
            $(".btnArchiveNotification").data("status", 2);
            $(".btnArchiveNotification").find(".glyphicon").switchClass("glyphicon-folder-open", "glyphicon-folder-close", 0);
        }
    }
    let page = getPaginaActiva();
    cargarPagina(page);
}

$(document).on("click", "#btnFavorite", function () {
    let request = agregarFavoritas(NotificationsSelected);
    request.done(function (data) {
        if (data.length > 0) {
            tbSwitchToUnfavorite(data);
            toast(data.length + " agregadas a favoritas", "DESHACER", function (undo) {
                if (undo) {
                    quitarFavoritas(data).done(function (data2) { if (data2.length > 0) tbSwitchToFavorite(data2); });
                }
            })
        }
    });
});

$(document).on("click", "#btnUnfavorite", function () {
    let request = quitarFavoritas(NotificationsSelected);
    request.done(function (data) {
        if (data.length > 0) {
            tbSwitchToFavorite(data);
            toast(data.length + " eliminadas de Favoritos", "DESHACER", function (undo) {
                if (undo) {
                    agregarFavoritas(data).done(function (data2) { if (data2.length > 0) tbSwitchToUnfavorite(data2); })
                }
            });
        }
    });
});

function tbSwitchToUnfavorite(data) {
    $(".toolbar").find("#btnFavorite").hide();
    $(".toolbar").find("#btnUnfavorite").show();
    let idNotifActiva = parseInt($(".detail").data("idNotificacion"));
    if (idNotifActiva > 0) {
        let index = data.indexOf(idNotifActiva);
        if (index >= 0) {
            $(".btnFavoriteNotification").data("favorita", 1);
            $(".btnFavoriteNotification").find(".glyphicon").switchClass("glyphicon-star-empty", "glyphicon-star", 0);
        }
    }

    let inbox = getBuzonActivo();
    if (inbox == 2) {
        let page = getPaginaActiva();
        cargarPagina(page);
    }
}

function tbSwitchToFavorite(data) {
    $(".toolbar").find("#btnUnfavorite").hide();
    $(".toolbar").find("#btnFavorite").show();
    let idNotifActiva = parseInt($(".detail").data("idNotificacion"));
    if (idNotifActiva > 0) {
        let index = data.indexOf(idNotifActiva);
        if (index >= 0) {
            $(".btnFavoriteNotification").data("favorita", 0);
            $(".btnFavoriteNotification").find(".glyphicon").switchClass("glyphicon-star", "glyphicon-star-empty", 0);
        }
    }

    let inbox = getBuzonActivo();
    if (inbox == 2) {
        let page = getPaginaActiva();
        cargarPagina(page);
    }
}

var NotificationsSelected = [];

function selectNotification($notification) {
    $notification.addClass("selected");
    $notification.find(".glyphicon").switchClass("glyphicon-unchecked", "glyphicon-check");
}

function addToNotificationSelected(IdNotificacion) {
    NotificationsSelected.push(IdNotificacion);
}

function deselectNotification($notification) {
    $notification.removeClass("selected");
    $notification.find(".glyphicon").switchClass("glyphicon-check", "glyphicon-unchecked");
}

function removeFromNotificationsSelected(IdNotificacion) {
    var index = NotificationsSelected.indexOf(IdNotificacion);
    if (index > -1)
        NotificationsSelected.splice(index, 1);
}

function toggleToolbar() {
    if (NotificationsSelected.length > 0) {
        if (NotificationsSelected.length == 1) {
            let id = NotificationsSelected[0];
            let $notif = $(".notification").filterByData("idNotificacion", id);
            let status = parseInt($notif.data("status"));
            if (status == 3){
                $("#btnArchivar").hide();
                $("#btnDesarchivar").show();
            }
            else {
                $("#btnDesarchivar").hide();
                $("#btnArchivar").show();
            }

            let fav = Boolean($notif.data("favorita"));
            if (fav) {
                $(".toolbar").find("#btnFavorite").hide();
                $(".toolbar").find("#btnUnfavorite").show();
            }
            else {
                $(".toolbar").find("#btnUnfavorite").hide();
                $(".toolbar").find("#btnFavorite").show();
            }
        }
        $(".toolbar").show();
    }
    else
        $(".toolbar").hide();
}

$(document).on("click", ".notification", function (e) {
    let $notification = $(this);
    if (e.ctrlKey) {
        $notification.find(".btn-selector").click();
    } else {
        $notification.siblings(".active").removeClass("active");
        $notification.addClass("active");

        let IdNotificacion = parseInt($notification.data("idNotificacion"));
        let request = cargarNotificacion(IdNotificacion);

        request.done(function () {
            if ($notification.data("status") == 1) {
                marcarComoLeida(IdNotificacion);
            }
        });    

        if (screen.width <= 600) {
            $(".notification-detail").show("slide", { 'direction': 'right' }, 500);
        }
    }
});

$(document).on("click", ".buttonToTop", function () {
    scrollToTopOfList();
});

/* Sección de Detalle de Notificaciones */

$(document).on("keypress keyup keydown change", "#comment-input", checkInputComment);

$(document).on("click", ".btnArchiveNotification", function () {
    let $btn = $(this);
    let status = parseInt($btn.data("status"));
    let IdNotificacion = parseInt($btn.data("idNotificacion"));
    var request;
    if (status == 3) {
        request = desarchivarNotificacion(IdNotificacion);
        request.done(function (data) {
            if(data){
                showBtnArchivar();
                toast("Desarchivada", "DESHACER", function (undo) {
                    if (undo) {
                        archivarNotificacion(IdNotificacion).done(function (data) { if (data) showBtnDesarchivar(); });
                    }
                })
            }
        });
    }
    else {
        request = archivarNotificacion(IdNotificacion);
        request.done(function (data) {
            if (data) {
                showBtnDesarchivar();
                toast("Archivada", "DESHACER", function (undo) {
                    if (undo) {
                        desarchivarNotificacion(IdNotificacion).done(function (data) { if(data) showBtnArchivar() });
                    }
                })
            }
        });
    }
});

function showBtnDesarchivar() {
    let $btn = $(".btnArchiveNotification");
    $btn.tooltip('hide').attr("data-original-title", "Desarchivar Notificación").tooltip("fixTitle").tooltip("show");
    $btn.find(".glyphicon").switchClass("glyphicon-folder-close", "glyphicon-folder-open", 0);
    $btn.data("status", 3);
    let page = getPaginaActiva();
    cargarPagina(page);
}

function showBtnArchivar() {
    let $btn = $(".btnArchiveNotification");
    $btn.tooltip('hide').attr("data-original-title", "Archivar Notificación").tooltip("fixTitle").tooltip("show");
    $btn.find(".glyphicon").switchClass("glyphicon-folder-open", "glyphicon-folder-close", 0);
    $btn.data("status", 2);
    let page = getPaginaActiva();
    cargarPagina(page);
}

$(document).on("click", ".btnFavoriteNotification", function () {
    var $btn = $(this);
    let IdNotificacion = $btn.data("idNotificacion");
    let dataF = $btn.data("favorita");
    let isFav = Boolean(dataF);
    var consulta;
    if (isFav) {
        consulta = quitarDeFavoritos(IdNotificacion);
        consulta.done(function (data) {
            if (data) {
                toast("Eliminada de Favoritos", "DESHACER", function (undo) {
                    if (undo) {
                        agregarAFavoritos(IdNotificacion);
                    }
                });
            }
        })
    }
    else {
        consulta = agregarAFavoritos(IdNotificacion);
        consulta.done(function (data) {
            if (data) {
                toast("Agregada a Favoritos", "DESHACER", function (undo) {
                    if (undo) {
                        quitarDeFavoritos(IdNotificacion);
                    }
                });
            }
        });
    }
});

$(document).on("click", ".btnPrintNotification", printNotification);

function checkInputComment() {
    let $this = $("#comment-input");
    let value = $this.val();
    let length = value.length;
    $(".counter").html(length);
    if (length > 120) {
        $(".new-comment").addClass("error");
        $(".btn-send").attr("disabled", true);
    }
    else {
        $(".new-comment").removeClass("error");
        $(".btn-send").attr("disabled", false);
    }
}

function cargarNotificacion(IdNotificacion) {
    request = $.ajax({
        url: "/Home/NotificationDetails/",
        method: "GET",
        data: {
            IdNotificacion: IdNotificacion
        },
    });

    request.done(function (data) {
        $(".empty-cover").hide();
        $(".notification-detail .detail").html(data);
        $(".notification-detail .detail").data("idNotificacion", IdNotificacion);
        $(".notification-detail .detail").show();
        $(".detail").find("[data-toggle='tooltip']").tooltip({
            "trigger": "hover"
        });
        setTimeout(function () {
            scrollToBottomOfComments();
        }, 1000);
    });

    return request;
}

function marcarComoLeida(IdNotificacion) {
    $.ajax({
        url: "/Home/MarcarComoLeida/",
        data: {
            IdNotificacion: IdNotificacion
        },
        method: "GET",
        success: function (data) {
            if (data) {
                let $notification = $(".notification").filterByData("idNotificacion", IdNotificacion);
                $notification.data("status", 2);
                $notification.removeClass("new");
            }
        }
    });
}

function archivarNotificacion(IdNotificacion) {
    return $.ajax({
        url: "/Home/ArchivarNotificacion",
        data: {
            IdNotificacion: IdNotificacion
        },
        method: "GET"
    });
}

function archivarNotificaciones(ids) {
    return $.ajax({
        url: "/Home/ArchivarNotificaciones/",
        data: {
            idsNotificacion: ids
        },
        method: "POST",
    });
}

function desarchivarNotificacion(IdNotificacion) {
    return $.ajax({
        url: "/Home/DesarchivarNotificacion",
        data: {
            IdNotificacion: IdNotificacion
        },
        method: "GET"
    });
}

function desarchivarNotificaciones(ids) {
    return $.ajax({
        url: "/Home/DesarchivarNotificaciones/",
        data: {
            idsNotificacion: ids
        },
        method: "POST",
    });
}

function agregarAFavoritos(IdNotificacion) {
    let consulta = $.ajax({
        url: "/Home/MarcarComoFavorita/",
        data: {
            IdNotificacion: IdNotificacion
        },
        method: "POST",
    });
    consulta.done(function (data) {
        if (data) {
            let $btn = $(".btnFavoriteNotification");
            $btn.tooltip('hide').attr("data-original-title", "Eliminar de Favoritos").tooltip("fixTitle").tooltip("show");
            $btn.find(".glyphicon").switchClass("glyphicon-star-empty", "glyphicon-star", 0);
            $btn.data("favorita", true);

            let bzn = getBuzonActivo();
            if (bzn == 2) {//si está en el buzón de favoritos 
                let page = getPaginaActiva();
                cargarPagina(page);
                setTimeout(function () {
                    $(".notification-list").find(".notification").filterByData("idNotificacion", IdNotificacion).addClass("active");
                }, 100);
            }
        }
    });

    return consulta;
}

function agregarFavoritas(ids) {
    return $.ajax({
        url: "/Home/AgregarFavoritas/",
        method: "POST",
        data: {
            notificaciones: ids
        }
    });
}

function quitarDeFavoritos(IdNotificacion) {
    let consulta = $.ajax({
        url: "/Home/QuitarFavorita/",
        data: {
            IdNotificacion: IdNotificacion
        },
        method: "POST",
    });
    consulta.done(function (data) {
        if (data) {
            let $btn = $(".btnFavoriteNotification");
            $btn.tooltip('hide').attr("data-original-title", "Agregar a Favoritos").tooltip("fixTitle").tooltip("show");
            $btn.find(".glyphicon").switchClass("glyphicon-star", "glyphicon-star-empty", 0);
            $btn.data("favorita", false);
            let bzn = getBuzonActivo();
            if (bzn == 2) {
                let notification = $(".notification-list").find(".notification").filterByData("idNotificacion", IdNotificacion);
                notification.hide("fade", 500);
                setTimeout(function () {
                    let page = getPaginaActiva();
                    cargarPagina(0);
                }, 500);
            }
        }
    });
    return consulta;
}

function quitarFavoritas(ids) {
    return $.ajax({
        url: "/Home/QuitarFavoritas/",
        data: {
            notificaciones: ids
        },
        method: "POST"
    });
}

function toggleSideBar() {
    let $sidebar = $(".sidebar");
    if ($sidebar.is(":hidden"))
        $sidebar.show("slide", 500);
    else
        $sidebar.hide("slide", 500);
}

function getBuzonActivo() {
    return parseInt($("#buzon-title").data("buzonActivo"));
}

function getPaginaActiva() {
    return parseInt($("#buzon-title").data("paginaActiva"));
}

function cargarPagina(numPag) {
    $('#load').css('visibility', 'visible');
    let $btnSidebar = $(".btn-sidebar.active");
    let bandeja = parseInt($btnSidebar.data("bandeja"));
    let keyword = $("#keyword").val();
    let $labels = $(".label-active");
    var filtros = [];
    if ($labels.length < 18) {
        $labels.each(function () {
            let filtro = parseInt($(this).data("filtro"));
			let filtro2 = parseInt($(this).data("filtro2"));
            let filtro3 = parseInt($(this).data("filtro3"));
			if(filtro != 0){
			filtros.push(filtro);				
			}
			if(filtro2 != 0){
			filtros.push(filtro2);				
			}
			if(filtro3 != 0){
			filtros.push(filtro3);				
			}
            
			
			
        });
    }

    let request = $.ajax({
        url: "/Home/GetListaNotificaciones/",
        data: {
            bandeja: bandeja,
            numPage: numPag,
            filtros: filtros,
            keyword: keyword,
        },
        method: "POST",
    });

    request.done(function (data) {
        let $list = $(".notification-list");
        $list.html(data);
        let numPagina = parseInt($list.find(".numPagina").val());
        let totalPaginas = parseInt($list.find(".totalPaginas").val());
        let totalRegistros = parseInt($list.find(".totalRegistros").val());
        let primerRegistro = parseInt($list.find(".primerRegistro").val());
        let ultimoRegistro = parseInt($list.find(".ultimoRegistro").val());

        let $pagination = $(".pagination-filter");
        $pagination.find(".primerRegistro").html(primerRegistro);
        $pagination.find(".ultimoRegistro").html(ultimoRegistro);
        $pagination.find(".totalRegistros").html(totalRegistros);

        $("#buzon-title").data("paginaActiva", numPagina);
        $("#buzon-title").data("buzonActivo", bandeja);

        if (numPagina == 0) 
            $("#btnPreviousPage").attr("disabled", true);
        else
            $("#btnPreviousPage").attr("disabled", false);

        if (numPagina >= (totalPaginas - 1) || totalPaginas == 0) 
            $("#btnNextPage").attr("disabled", true);
        else 
            $("#btnNextPage").attr("disabled", false);

        $("#btnPreviousPage").data("pagina", numPagina - 1);
        $("#btnNextPage").data("pagina", numPagina + 1);

        scrollToTopOfList();

        for (var i in NotificationsSelected) {
            let IdNotificacion = NotificationsSelected[i];
            let notificacion = $(".notification").filterByData("idNotificacion", IdNotificacion);
            selectNotification(notificacion);
        }
        $('#load').css('visibility', 'hidden');
    });
    
}

var timeoutCloseToast;

function toast(message, confirmTxt, cb) {
    let $toast = $(".toast");

    let timeout = 0;
    if (timeoutCloseToast != null) {
        clearTimeout(timeoutCloseToast);
        dismissToast();
        timeout = 600;
    }

    setTimeout(function () {
        $toast.find(".message").html(message);
        let $confirmText = $toast.find(".confirmTxt");
        

        if (confirmTxt) {
            $confirmText.html(confirmTxt);
            $confirmText.on("click", confirm);

            function confirm() {
                dismissToast();
                cb(true);
            }

        } else {
            confirmTxt = "";
            $confirmText.on("click", dismissToast);
        }

        showToast();

        timeoutCloseToast = setTimeout(dismissToast, 5000);

    }, timeout);
}

function showToast() {
    let $toast = $(".toast");
    $toast.show("slide", { direction: "down" }, 500);
}

function dismissToast() {
    let $toast = $(".toast");
    $toast.find(".confirmTxt").unbind();
    $toast.hide("slide", { direction: "down" }, 500);
}

function scrollToTopOfList() {
    $(".notification-list").animate({ scrollTop: 0 }, 700);
}

function scrollToBottomOfComments() {
    let $commentList = document.getElementsByClassName("comment-list");
    let height = $commentList[0].scrollHeight;
    $(".comment-list").animate({
        scrollTop: height
    }, 500);
}

function OnBeginCommenting() {
    let $commentInput = $("#comment-input");
    $commentInput.val("");
    checkInputComment();
}

function OnSuccessCommenting(data) {
    if (data > 0) {
        $.ajax({
            url: "/Home/GetComment/",
            data: {
                IdComentario: data
            },
            method: "GET",
        }).done(function (data) {
            $(".comment-list").append(data);
            $(".comment-hidden").hide();
            scrollToBottomOfComments();
            setTimeout(function () {
                $(".comment-hidden").show("fade", 500);
                $(".comment-hidden").removeClass("comment-hidden");
                scrollToBottomOfComments();
            }, 500);
        });
    }
}

function OnFailureCommenting() {

}

/* Utils */
$.fn.filterByData = function (prop, val) {
    return this.filter(function () { return $(this).data(prop) == val; })
}

/* PDF */
function printNotification() {
    var doc = new jsPDF();

    var title = $(".notification-detail").find(".notification-name").text();
    var fecha = $(".notification-detail").find(".notification-date").text();
    var cliente = $(".notification-detail").find(".notification-client").text();
    var user = $(".notification-detail").find(".remitent-name").text();
    var type = $(".notification-detail").find(".notification-type").text();
    var array = $(".notification-detail").find(".collection-item").map(function () {
        return $(this).text();
    }).get();
    printBody(title, fecha, cliente, user, type, array, doc);
    doc.save(type + " - " + title + '.pdf');
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

$(document).ready(function () {
    $("[data-toggle='tooltip']").tooltip({
        "viewport" : ".page-body"
    });
})