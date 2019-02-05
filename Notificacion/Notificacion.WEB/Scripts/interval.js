$(document).ready(function () {

    $("#bells").click(function () {
        $("#bells").prop("hidden", true);
        $("#bell").prop("hidden", false);
        stopService();
        Materialize.toast("¡Notificaciones desactivadas!", 4000);
    });

    $("#bell").click(function () {
        $("#bell").prop("hidden", true);
        $("#bells").prop("hidden", false);
        initService();
        Materialize.toast("¡Notificaciones activadas!", 4000);
    });

    
    var service;
    $date = new Date();
    initService();


    function initService() {
        checkUpdates($date);
        service = setInterval(function () {
            checkUpdates($date);
            $date = new Date();
        }, 30000);
    }

    function stopService() {
        clearInterval(service);
    }

});


function checkUpdates(date) {
    var request = $.ajax({
        url: "/Home/GetLatest",
        data: {
            date: date.toISOString()
        }
    });

    request.done(function (data) {
        if (data.length > 0) {
            for (var i = 0; i < data.length; i++) {
                var IdNotificacion = data[i].IdNotificacion;
                Push.create(data[i].Titulo, {
                    body: data[i].TipoDescripcion,
                    icon: data[i].image,
                    timeout: 5000,
                    onClick: function () {
                        window.focus();
                        this.close();
                        openModalNotification(IdNotificacion);
                    }
                });
            }
            var $numPage = $(".pagination").data("pagina-activa");
            LoadPage($numPage);
        }
    });

    request.fail(function () {
        $("#bells").find("i").html("sync_problem");
    });
}