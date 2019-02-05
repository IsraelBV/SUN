var section1 = $("#section1");
var section2 = $("#section2");
var section3 = $("#section3");
var allSections = $(".section");
var btnAnterior = $("#btnAnterior");
var btnCancelar = $("#btnCancelar");
var btnGuardar = $("#btnGuardar");
var btnContinuar = $("#btnContinuar");
var ptDatosPersonales = $("#pt_datos_personales");
var ptDatosContratacion = $("#pt_datos_contratacion");
var ptDatosNomina = $("#pt_datos_nomina");

$("input[type='text']").change(function () {
    this.value = this.value.toUpperCase();
});

function OnBegin() {
    $(".section").hide();
    $("#loadingImg").show();
}

var terminar = false;

function OnSuccessContrato() {
    if (terminar)
        window.location.href = "/Empleados/TerminarProcesoNuevoEmpleado";
    else 
        section2_to_section3();
}

function OnFailure() {
    alert("Algo inesperado sucedió!");
}

function OnSuccessDatosBancarios(response) {
    window.location.href = "/Empleados/TerminarProcesoNuevoEmpleado";
}

/* Navegación del panel */
btnContinuar.click(function () {
    if (section2.is(":visible")) {
        $("#form-contrato").submit();
    }
    if (section1.is(":visible")) {
        $("#form-empleado").submit();
    }
});

btnAnterior.click(function () {
    if (section2.is(":visible")) {
        section2_to_section1();
    }
    if (section3.is(":visible")) {
        section3_to_section2();
    }
});

btnGuardar.click(function () {
    if (section3.is(":visible")) {
        $("#DatosBancariosForm").submit();
    }
    else {
        terminar = true;
        $("#form-contrato").submit();
    }
});

function section1_to_section2() {
    $("#loadingImg").hide();
    section2.show();
    btnAnterior.attr("disabled", false);
    //btnGuardar.show();

    ptDatosPersonales.removeClass("active-section");
    ptDatosPersonales.addClass("completed-section");
    ptDatosContratacion.removeClass("inactive-section");
    ptDatosContratacion.addClass("active-section");
}

function section2_to_section3() {
    $("#loadingImg").hide();
    section3.show();
    btnContinuar.attr("disabled", true);
    btnGuardar.show();
    ptDatosContratacion.removeClass("active-section");
    ptDatosContratacion.addClass("completed-section");
    ptDatosNomina.removeClass("inactive-section");
    ptDatosNomina.addClass("active-section");
}

function section3_to_section2() {
    section3.hide();
    section2.show();
    btnContinuar.attr("disabled", false);

    ptDatosNomina.removeClass("active-section");
    ptDatosNomina.addClass("inactive-section");
    ptDatosContratacion.removeClass("completed-section");
    ptDatosContratacion.addClass("active-section");
}

function section2_to_section1() {
    section2.hide();
    section1.show();
    btnAnterior.attr("disabled", true);
    btnGuardar.hide();

    ptDatosContratacion.removeClass("active-section");
    ptDatosContratacion.addClass("inactive-section");
    ptDatosPersonales.removeClass("completed-section");
    ptDatosPersonales.addClass("active-section");
}