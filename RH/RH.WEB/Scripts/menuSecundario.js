$(document).ready(function () {
    $("#liCatalogos").addClass("active");




    $(document).keydown(function (tecla) {

        var activar = 0;
        if (tecla.ctrlKey && tecla.altKey) {
            if (tecla.keyCode == 65) {
                
            }
            else if (tecla.keyCode == 66) {
        
                $('input[type=search]').focus();
            }
        }
        else if (tecla.altKey) {

            if (tecla.keyCode == 66) {
           
                $('input[type=search]').focus();

            }
        }
    });



});