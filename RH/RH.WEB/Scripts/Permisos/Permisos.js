

function respuestapermiso(data) {
    
    $('#modalCrear').modal('hide');
    if (data.resultado == true) {
        var title = "<b>No pudo guardar el registro </b> <br/>"
        var message = "Ya se registro un permiso o incidencias con la misma fecha";

        notiDuplicados = $.notify({
            title: title,
            message: message,
        }, {
            type: "danger",
            animate: {
                enter: 'animated fadeInDown',
                exit: 'animated fadeOutUp'
            },
            placement: { from: 'top', align: 'center' },
            offset: 55, timer: 60000
        });
    } else {

        $.notify({
            title: "<b>Se guardaron los datos registros exitosamente</b>",
            message: ""
        }, {
            type: "success",
            animate: {
                enter: 'animated fadeInDown',
                exit: 'animated fadeOutUp'
            },
            placement: { from: 'top', align: 'center' },
            offset: 55, timer: 10000
        });
    }
}

       
