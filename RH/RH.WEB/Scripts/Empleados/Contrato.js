$(document).ready(function () {
    /* Función para generar el salario diario integrado */
    /* Toma el factor en el input oculto "factor" y lo multiplica por el salario diario SD */
    $("#SD").keyup(function () {
    	var sd = parseFloat($("#SD").val());
    	var factor = parseFloat($("#factor").val());
        var sdi = sd * factor;
        if (!isNaN(sdi)) {
        	$("#SDI").val(sdi.round(4));
        }
        else{
            $("#SDI").val(0);
        }
    });
    /* Función para generar el SBC salario base de cotizacion */
    /* Toma el factor en el input oculto "factor" y lo multiplica por el salario diario SD */
    $("#SD").keyup(function () {
        var sd = parseFloat($("#SD").val());
        var factor = parseFloat($("#factor").val());
        var sdi = sd * factor;
        if (!isNaN(sdi)) {
            $("#SBC").val(sdi.round(4));
        }
        else {
            $("#SBC").val(0);
        }
    });

    Number.prototype.round = function (places) {
        return +(Math.round(this + "e+" + places) + "e-" + places);
    }

    //Vigencia del contrato
    $("#DiasContrato").keyup(function () {
        setVigencia();
    });

    $("#FechaReal").change(function () {

        setVigencia();
        //if ($("#TipoContrato").val() === "2" ) {
        //    setVigencia();
        //}
    });

    $("input[name=DiaDescanso1]").change(function () {
        if ($("#TipoContrato").val() === "2") {
            setVigencia();
        }
    });

    function setVigencia() {
        if ($("#FechaReal").val() === "") {
            var fecha_inicio = mm + "-" + dd + "-" + yyyy;
            var vigencia = new Date(fecha_inicio);
        } else {
            var fecha_trim = $("#FechaReal").val().split("-");

            //formato mm/dd/yyyy
            var vigencia = new Date(fecha_trim[1] + "-" + fecha_trim[2] + "-" + fecha_trim[0]);
        }

        var i = 1;//antes era 0 //cambio de caro
        //Se obtienen los valores seleccionados de descanso
        //var descanso1 = getDiaDescanso($("input[name=DiaDescanso]:checked").val()); //se quito porque caro dijo que no se debe conciderar

        while (i < $("#DiasContrato").val()) {//se tomara en cuenta el primer dia - igual pedido por caro
           
            vigencia.setDate(vigencia.getDate() + 1);

            //if (vigencia.getDay() == descanso1) {
            //    vigencia.setDate(vigencia.getDate() + 1);
            //}
            i++;
        }

        var date = concatDate(vigencia);
        $("#Vigencia").val(date);

    }

    //Recibe un objeto de tipo Date y retorna la fecha concatenada para introducir en un input type date
    function concatDate(date) {
        var dd = date.getDate();
        var mm = date.getMonth() + 1;
        var yyyy = date.getFullYear();

        dd = dd < 10 ? '0' + dd : dd;
        mm = mm < 10 ? '0' + mm : mm;

        return yyyy + "-" + mm + "-" + dd;
    }

    //recibe un string y regresa un int dependiendo el día
    function getDiaDescanso(dia) {
        switch (dia) {
            case "Domingo":
                return 0;
            case "Lunes":
                return 1;
            case "Martes":
                return 2;
            case "Miércoles":
                return 3;
            case "Jueves":
                return 4;
            case "Viernes":
                return 5;
            case "Sábado":
                return 6;
            case "n/a":
                return 8;
            default:
                return 0;
        }
    }
});