$(document).ready(function () {

    $(".autorizar").click(function () {
        //utils.confirmDialog("¿Autorizar Nomina?", null, null, null, function (confirmation) {
        //    if (confirmation) {
                autorizar();
            
        //    }
            
        //});
    });


    function Operacion() {
        var porcentajeServicio = $(C_Porcentaje_Servicio).val();
        var totalServicio = $(C_Total_Servicio).val();
        var percepciones = $(C_Percepciones).val();
        var cuottas = $(C_Cuotas_IMSS_Infonavit).val();
        var impuesto = $(C_Impuesto_Nomina).val();
        var rela = $(C_Relativos).val();
        var descuentos = $(C_Descuentos).val();
        var otros = $(C_Otros).val();
        var subtotal = $(C_Subtotal).val()
        var total = $(C_Total_Complemento).val();
        var iva = $(IVA).val();
        var totalIVA = $(C_Total_IVA).val();


        porcentajeServicio = (isNaN(parseFloat(porcentajeServicio))) ? 0 : parseFloat(porcentajeServicio);
        totalServicio = (isNaN(parseFloat(totalServicio))) ? 0 : parseFloat(totalServicio);
        percepciones = (isNaN(parseFloat(percepciones))) ? 0 : parseFloat(percepciones);
        cuottas = (isNaN(parseFloat(cuottas))) ? 0 : parseFloat(cuottas);
        impuesto = (isNaN(parseFloat(impuesto))) ? 0 : parseFloat(impuesto);
        rela = (isNaN(parseFloat(rela))) ? 0 : parseFloat(rela);
        descuentos = (isNaN(parseFloat(descuentos))) ? 0 : parseFloat(descuentos);
        otros = (isNaN(parseFloat(otros))) ? 0 : parseFloat(otros);
        subtotal = (isNaN(parseFloat(subtotal))) ? 0 : parseFloat(subtotal);
        total = (isNaN(parseFloat(total))) ? 0 : parseFloat(total);
        iva = (isNaN(parseFloat(iva))) ? 0 : parseFloat(iva);
        totalIVA = (isNaN(parseFloat(totalIVA))) ? 0 : parseFloat(totalIVA);

        porcentajeServicio = porcentajeServicio / 100;
        rela = cuottas + impuesto;

        totalServicio = percepciones * porcentajeServicio;
        subtotal = percepciones + rela + descuentos + otros + totalServicio
        iva = iva / 100;
        totalIVA = subtotal * iva;

        total = subtotal + totalIVA;

        $(C_Total_IVA).val(totalIVA);
        $(C_Subtotal).val(subtotal);
        $(C_Total_Servicio).val(totalServicio);
        $(C_Relativos).val(rela);
        $(C_Total_Complemento).val(total);
    }

});

function autorizar() {
    
    $.ajax({
        type: 'POST',            
        contentType: 'application/json',
        url: '/AutorizarNomina/AutorizarNomina/',
        beforeSend: function() {
            waitingDialog.show("Autorizando...");
        },
        success: function (result) {
            
            $('#noAutorizada').hide();
            $('#siAutorizada').show();
            $('#botonAutorizar').removeClass("btn-default");
            $('#botonAutorizar').removeClass("autorizar");
            $('#botonAutorizar').addClass("btn-danger");
            $('#botonAutorizar').text("Nomina Autorizada");
            //$('.auto').append(' <h4 class="pull-left label label-success" id="TituloAutorizar">Nomina Autorizada </h4>');
            //document.getElementById('Dispersion').style.display = 'block';
            waitingDialog.hide();
        },
        error: function (error) {
            // si hay un error lanzara el mensaje de error

        }
    })
}

