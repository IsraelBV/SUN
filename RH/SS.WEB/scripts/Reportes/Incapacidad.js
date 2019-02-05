var incapacidad = (function () {
    //recibe un elemento y en base a este busca el elemento padre(contenedor) con la clase: sendContiner
    //Busca dentro de este contenedor todos los elementos que tengan la clase: dataVldt y que no esten vacios
    //lanza una alerta en else caso de que esten vacios
    //devuelve false si existe uno o mas elemntos vacios
    var validate = function (btnElement) {
        var ret = true;
        var elements = btnElement.closest(".sendContiner").find(".dataVldt");
        elements.each(function () {

            if ($(this).val() == null || $(this).val() == "" || ($(this).val() == 0 && $(this)[0].nodeName == "SELECT")) {
                utils.showMessage("ATENCION", "El campo <strong>" + $(this).data('info') + "</strong> no puede estar vacio", 4000, "warning");
                ret = false;
            }
            
        });
        return ret;
    }

    //recibe un elemento y en base a este busca el elemento padre(contenedor) con la clase: sendContiner
    //Busca dentro de este contenedor todos los elementos que tengan la propiedad name
    //crea un objeto donde el nombre es el name y el valor es el value del elemento respectivamente
    //devuelve un json
    var objData = function (btnElement) {
        var elements = btnElement.closest(".sendContiner").find("[name]");
        var valOBJ = {}
        elements.each(function () {
            valOBJ[$(this).attr("name")] = $(this).val();
        });
        return valOBJ;
    }

    var init = {
        validacion: validate,
        data: objData
    }
    return init;
})();
$(document).ready(function () {

    $("#descargarReporteIncap").click(function () {

        if (incapacidad.validacion($(this))) {
            var xhr = $.ajax({
                url: '/Reportes/ReporteIncapacidad/',
                method: "POST",
                data: incapacidad.data($(this)),
                beforeSend: function (xhr) {
                    waitingDialog.show('Generando Reporte de Incapacidades...');
                },
                success: function (data) {
                    if (data.success) {
                        $("#downReporteInfo").attr("href", data.resultPath);
                        window.location = data.resultPath;
                    } else {
                        utils.showMessage("WAR", data.error, 8000);
                    }
                },
                error: function (data) {
                    alert(data.error);
                },
                complete: function (data) {
                    waitingDialog.hide();

                }
            });

        }

    });

});