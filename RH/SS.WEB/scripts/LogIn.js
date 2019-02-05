function OnSuccess(response) {
    if (response == 1) {
        $(".form-group").addClass("has-success");
        $.notify({
            title: "<strong>¡Bienvenido "+ $("#username").val() + "!</strong>",
            message: "En un momento se le redireccionará..."
        }, {
            type: 'success',
            animate: {
                enter: 'animated fadeInDown',
                exit: 'animated fadeOutUp'
            },
            placement: {
                from: 'top',
                align: 'center'
            }
        });
        setTimeout(function () { window.location.href = "../Home/Index" }, 2000);

    } else if (response == 3) {
        $.notify({
            title: "<strong>¡Error!</strong>",
            message: "Credenciales no válidas."
        }, {
            type: 'danger',
            animate: {
                enter: 'animated shake',
                exit: 'animated fadeOutUp'
            },
            placement: {
                from: 'top',
                align: 'center'
            }
        });
        $(".form-group").addClass("has-error");
        $("#username").focus();
    } else if (response == 2) {
        $.notify({
            title: "<strong>¡Error!</strong>",
            message: "No tiene permisos para ingresar a esta aplicación."
        }, {
            type: 'danger',
            animate: {
                enter: 'animated shake',
                exit: 'animated fadeOutUp'
            },
            placement: {
                from: 'top',
                align: 'center'
            }
        });
        $(".form-group").addClass("has-error");
        $("#username").focus();
    }
}

$(".form-group").change(function () {
    $(this).removeClass("has-error");
});