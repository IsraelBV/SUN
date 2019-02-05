$(document).ready(function () {

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
        },
		method: "POST",
    });

    request.done(function (data) {
        if (data.length > 0) {
            for (var i = 0; i < data.length; i++) {
                var IdNotificacion = data[i].IdNotificacion;
                Push.create(data[i].Titulo, {
                    body: data[i].TipoDescripcion,
                    icon: data[i].image,
                    timeout: 15000,
                    onClick: function () {
                        window.focus();
                        this.close();
                        cargarNotificacion(IdNotificacion);
                        marcarComoLeida(IdNotificacion);
                    }
                });
            }
            let page = getPaginaActiva();
            cargarPagina(page);
        }
    });

    request.fail(function () {
        console.log("Error de conexión");
    });
}