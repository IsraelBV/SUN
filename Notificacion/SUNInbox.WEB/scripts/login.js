$(document).ready(function () {
    window.setTimeout(showCard, 2000);
    $('#imgLoad').hide();
    $("input").focus(function () {
        $(this).parent().find(".prefix.active").css("color", "#03a9f4");
        $(this).parent().find("label").css("color", "#03a9f4");
    });

    $("input").focusout(function () {
        if ($(this).hasClass("valid")) {
            $(this).parent().find(".prefix.active").css("color", "#4CAF50");
            $(this).parent().find("label").css("color", "#4CAF50");
        }
        else if ( $(this).val() == "" ) {
            $(this).parent().find(".prefix.active").css("color", "grey");
            $(this).parent().find("label").css("color", "grey");
        }
    });
});

function showCard() {
	$("#welcome-message").hide("slide", { direction: "right" }, 1000);
	setTimeout(function () {
		$("#login-form").show("fade", 200);
	}, 1000);
}

function showMessage(title, name, secondname) {
    var img = name.charAt(0) + secondname.charAt(0);
    $("#user-image").html(img.toUpperCase());
    $("#message-text-title").html(title);
    $("#message-text-subtitle").html(name);
    var width = $("#message").width();
    var window = $(document).width();
    var newWidth = (window - width) / 2;
    $("#message").css("display", "none");
    $("#message").css("visibility", "visible");
    $("#message").css("left", newWidth);
    $("#message").show("slide", { direction: "down" }, 1000);
}

function Loading () {
    $('#btnLogin').prop("disabled", true);
    $('#in').prop("hidden", true);
    $('#imgLoad').show();
};

function OnSuccess(data) {
    if (data[0] == 1) {
        showMessage("Bienvenido,", data[1], data[2]);
        setTimeout(function () {
            window.location.href = "/Home/Index";
        }, 4000);
    }
    else if (data[0] == 2) { //el usuario no tiene acceso a la app
        $(".validate").removeClass("valid");
        $(".validate").addClass("invalid");
        $('#btnLogin').prop("disabled", false);
        $('#in').prop("hidden", false);
        $('#imgLoad').hide();
        Materialize.toast('El usuario no tiene acceso a la aplicación!', 4000)
    }
    else {
        $(".validate").removeClass("valid");
        $(".validate").addClass("invalid");

        //cambiar de color el icono
        $(".input-field .prefix").css("color", "red");

        //cambiar de color el label
        $(".input-field label").css("color", "red");

        //Habilita el botón nuevamente
        $('#btnLogin').prop("disabled", false);
        $('#in').prop("hidden", false);
        $('#imgLoad').hide();

        Materialize.toast('Credenciales incorrectas!', 4000)
    }
}