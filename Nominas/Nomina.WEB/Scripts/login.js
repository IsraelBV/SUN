$(document).ready(function () {
	$("#btnNext").click(function () {
		$("#frmUser").submit();
	});

	$("#btnBack").click(function () {
		backStep();
	});

	$("#username").keydown(function(e) {
		e.stopImmediatePropagation();
		if(e.keyCode == 13 || e.keyCode == 9) {
			e.preventDefault();
			$("#frmUser").submit();
		}
	});

	$("#btnLogin").click(function () {
		SubmitCredentials();
	});
	
	$("#password").keydown(function(e) {
		e.stopImmediatePropagation();
		if(e.keyCode == 13 || e.keyCode == 9) {
			e.preventDefault();
			SubmitCredentials();
		}
	});
});

function SubmitCredentials() {
	disableLogInBtn();
	var request = $.ajax({
		url: "/Acceso/AutenticarUsuario",
		data: {
			username: $("#username").val(),
			password: $("#password").val()
		},
		method: "POST",
		dataType: "JSON"
	});
	
	request.done(function (acceso) {
		if(acceso) {
			window.location.href = "../Home/";
		}
		else {
			$("#password-field").addClass("is-invalid");
			enableLogInBtn();
		}
	});
	
	request.fail(function () {
		console.log("Error al mandar la solicitud ajax");
	});
}

function disableBtn() {
    $("#btnNext").attr("disabled", true);
}

function enableBtn() {
    $("#btnNext").attr("disabled", false);
}

function disableLogInBtn() {
	$("#btnLogin").attr("disabled", true);
	
}

function enableLogInBtn() {
	$("#btnLogin").attr("disabled", false);
}



function nextStep(data) {
    if (data.Acceso) {
	    $("#form-content").switchClass("step1", "step2");
	    $("#footer").switchClass("step1", "step2");
	    $("#btnBack").show("drop");
	    $("#imgUser").attr("src", data.foto);
	    $("#imgUser").css("border-radius", "50%");
	    
	    $("#username-text").html(data.nombre);
		$("#username-text").show();
		$("#welcome").hide();
	    $("#instruction").html("Ingresa tu contraseña");
		
	    setTimeout(function () {
		    $("#password").focus();
	    }, 500);
    }
    else {
     
        if (data.Error === 1) {$("#errorMessage").html("¡Este usuario No tiene acceso a la app!");}
        else if (data.Error === 2) { $("#errorMessage").html("¡El nombre de usuario no existe en la bd!"); }
        else if (data.Error === 3) { $("#errorMessage").html("¡No se establecio conexión con el servidor!"); }

		$("#user-name-field").addClass("is-invalid");
    }
}

function backStep() {
	$("#form-content").switchClass("step2", "step1");
	$("#footer").switchClass("step2", "step1");
	$("#btnBack").hide("drop");
	$("#imgUser").attr("src", "../../Images/logo2.png");
	$("#password").val("");
	$("#password-field").removeClass("is-invalid");
	$("#imgUser").css("border-radius", "0");
	$("#username-text").hide();
	$("#welcome").show();
	$("#instruction").html("Ingresa tu nombre de usuario");
	$("#username").focus();
}