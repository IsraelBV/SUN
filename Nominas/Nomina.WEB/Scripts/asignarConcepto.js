

   
  


function GuardarDatos() {
    var seleccionados = $("#conceptos2").DataTable().rows(".selected").ids();
    var array = [];
    var DATA = [];
    for (i = 0; i < seleccionados.length; i++) {
        array[i] = seleccionados[i];

    }

    console.log(array);

    var arrayEm = [];

 

    $('#tblEmpleados tbody tr').each(function () {

        //console.log($(this).find('input[name=checkedFiscal]').prop('checked') + " Complemento: " + $(this).find('input[name=checkedComplemento]').prop('checked'));
        //console.log($(this).find('.checkedFiscal').prop('checked') + " Complemento: " + $(this).find('.checkedComplemento').prop('checked'));

        if (($(this).find('.checkedFiscal').prop('checked') == true && $(this).find('.checkedComplemento').prop('checked') == true)) {
            //do something
            item = {};
            item["checkvalue"] = true;
            item["ide"] = parseInt($(this).data("id-empleado"));;
            item["isFiscal"] = true;
            item["isComplemento"] = true;
       

            
            arrayEm.push(item);
        } else if (($(this).find('.checkedFiscal').prop('checked') == true) && ($(this).find('.checkedComplemento').prop('checked') == false))
            {
            item = {};
            item["checkvalue"] = true;
            item["ide"] = parseInt($(this).data("id-empleado"));;
            item["isFiscal"] = true;
            
            item["isComplemento"] = false;
        
            arrayEm.push(item);
        } else if (($(this).find('.checkedFiscal').prop('checked') == false) && ($(this).find('.checkedComplemento').prop('checked') == true)) {
            item = {};
            item["checkvalue"] = true;
            item["ide"] = parseInt($(this).data("id-empleado"));;
            item["isFiscal"] = false;
            item["isComplemento"] = true;
         
            arrayEm.push(item);

        } else if (($(this).find('.checkedFiscal').prop('checked') == false) && ($(this).find('.checkedComplemento').prop('checked') == false)) {
            item = {};
            item["checkvalue"] = false;
            item["ide"] = parseInt($(this).data("id-empleado"));;
            item["isFiscal"] = false;
            item["isComplemento"] = false;
         
            arrayEm.push(item);
        }
    });



 
   
    
    var dataAccion = { arrayC: array, arrayE: arrayEm };
    var notification;
    $.ajax({
        type: 'POST',
        data: JSON.stringify(dataAccion),
        contentType: 'application/json',
        url: '/AsignarConcepto/GuardarCon_Empl/',
        success: function (result) {
            notification= utils.showMessage("Guardando Datos", "¡Datos guardados exitosamente!",1000,"");

        },
        error: function (error) {
            // si hay un error lanzara el mensaje de error

        }
    }).done(function (result) {

    });

    }
