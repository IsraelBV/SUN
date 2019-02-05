$(document).ready(function () {
    var $tipo = $("#TipoPrestamo");
    var $btnSave = $("#btnSave");
    var $monto = $("#Monto");
    var $tipoNomina = $("#tipo-nomina");
    var $parcialidades = $("#Parcialidades");
    var $interes = $("#interes");
    var $descuento = $("#CantidadDescuento");
    var $aPagar = $("#MontoInteres");

    $tipo.change(function () {
        $monto.val("");
        $parcialidades.val("");
        $descuento.val("");
        $aPagar.val("");
        $interes.val("");

        if ($tipo.val() == 2) {
            $("#frm-group-interes").removeClass("hide");
            
            //$monto.attr("max", 5000);
            //var parcialidades = limitParcialidades($tipoNomina.html());
            //$parcialidades.attr("max", parcialidades);
            $descuento.attr("readonly", true);
            $aPagar.attr("readonly", true);
        }
        else {
            $("#frm-group-interes").addClass("hide");
            //$monto.removeAttr("max");
            //$parcialidades.removeAttr("max");
            $aPagar.attr("readonly", false);
            $descuento.attr("readonly", false);
        }
    });

    $parcialidades.keyup(function () {
        if ($tipo.val() == 2) {
            if ($monto.val() != "" && $parcialidades.val() != "") {
                var interes = calcularInteres($tipoNomina.html(), $parcialidades.val());
                $interes.val(interes);
                var total = calcularTotalAPagar($monto.val(), $interes.val());
                $aPagar.val(total);
                var descuento = calcularDescuento(total, $parcialidades.val());
                $descuento.val(descuento);
            }
        }
    });

    $monto.keyup(function () {
        if ($tipo.val() == 2) {
            if ($monto.val() != "" && $parcialidades.val() != "" && $interes.val() != "") {
                var total = calcularTotalAPagar($monto.val(), $interes.val());
                $aPagar.val(total);
                var descuento = calcularDescuento(total, $parcialidades.val());
                $descuento.val(descuento);
            }
        }
    });

    $btnSave.click(function () {
        $("form").submit();
    });

    $interes.keyup(function () {
        if ($tipo.val() == 2 && $monto.val() != "" && $parcialidades.val() != "") {
            var total = calcularTotalAPagar($monto.val(), $interes.val());
            $aPagar.val(total);
            var descuento = calcularDescuento(total, $parcialidades.val());
            $descuento.val(descuento);
        }
    });
});

//function limitParcialidades(n) {
//    var tipoNomina = parseInt(n);
//    switch (tipoNomina) {
//        case 1: //Quincenal
//            return 6;
//        case 2: //Semanal
//            return 12;
//        case 3: //Mensual
//            return 3;
//        case 4: //Catorcenal
//            return 6;
//        default:
//            return 6;
//    }
//}

function calcularInteres(a, b) {
    var parcialidades = parseInt(b);
    return parcialidades * 3;
}

function calcularTotalAPagar(monto, interes) {
    monto = parseFloat(monto);
    interes = parseFloat(interes);

    interes = interes / 100;
    interes = monto * interes;
    var total = monto + interes;

    return total.toFixed(2);
}

function calcularDescuento(monto, parcialidades) {
    var descuento = monto / parcialidades;
    return descuento.toFixed(2);
}