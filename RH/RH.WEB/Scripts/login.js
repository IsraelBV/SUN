//Se declaran las variables para lo que se va a utilizar.
$(document).ready(function () {
    var user = $("#Usuario");
    var password = $("#Contraseña");
    var log_status = $(".log-status");
    var alerta1 = $(".alerta1");
    var alerta2 = $(".alerta2");

    //Se da la acción de poder entrar a la aplicación con la tecla "Enter".
    $(".form-control").keyup(function (e) {
        if (e.keyCode == 13)
        {
            $(".log-btn").click();
        }
    });

    //Se hace la sentencia del Ajax para inicio de sesión.
    $(".log-btn").click(function () {
        $.ajax({
            type: "POST",
            url: "LogInAJAX/?user=" + user.val() + "&password=" + password.val(),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: successFunc,
            error: errorFunc
        });
        $(".log-btn").prop("disabled", true);
        $(".log-btn").addClass("logbtnBlock");

        //En caso de regresar un valor success del ajax.
        function successFunc(data)
        {
            //Si el dato que se regresó es 2 aparece alerta de usuario incorrecto.
            if(data == 3)
            {
                log_status.addClass("wrong-entry");
                alerta1.addClass("alerta-visible");
                alerta1.slideDown(1000)
                /*$(".alert").fadeIn(500);
                setTimeout("$('.alert').fadeOut(1500);", 3000);*/
                $(".log-btn").removeClass("logbtnBlock");
                $(".log-btn").prop("disabled", false);
            }
            else
            {
                //Si el dato que se regresó es 1 redirecciona al home del sitio.
                if (data == 1)
                {
                    window.location.href = "../Home/Index";
                }
                //Si el dato que se regresa es 3, aparece alerta de usuario sin permisos.
                else {
                    log_status.addClass("wrong-entry");
                    alerta2.addClass("alerta-visible");
                    alerta2.slideDown(1000)
                    $(".log-btn").removeClass("logbtnBlock");
                    $(".log-btn").prop("disabled", false);
                }
            }
            
        }
        //Si el ajax devolvió un valor erróneo se despliega una alerta.
        function errorFunc()
        {
            alert("Algo inesperado sucedió");
        }
    });

    //Función para deshacer las alertas al editar los campos de texto.
    $('.form-control').keypress(function () {
        $('.log-status').removeClass('wrong-entry');
        $('.alerta1').slideUp(1000);
        $('.alerta2').slideUp(1000);
    });
    
})