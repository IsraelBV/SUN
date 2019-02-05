
function respuestavacaciones(data) {
    var idemp = $("input[name= 'idemp']").val();
    $('#modalGenerar').modal('hide');
    $('.modal-backdrop').remove();
    if (data.resultado == true) {
        var title = "<b>No pudo guardar el registro </b> <br/>"
        var message = "Ya se registraron unas vacaciones  o incidencias con la misma fecha";

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
        $("#divDetails").load("/Vacaciones/ViewDetails/?idContrato=" + idemp);
        $.notify({
            title: "<b>Se guardaron los datos registros exitfyuytosamente</b>",
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

