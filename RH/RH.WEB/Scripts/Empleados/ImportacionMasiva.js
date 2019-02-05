$(document).ready(function () {

    //$("#fileInput").change(function () {
    //    var data = new FormData();
    //    var files = $("#fileInput").get(0).files;

    //    if (files.length > 0)
    //        data.append("UploadedImage", files[0]);

    //    var ajaxRequest = $.ajax({
    //        type: "POST",
    //        url: "UploadFile",
    //        contentType: false,
    //        processData: false,
    //        data: data
    //    });

    //    ajaxRequest.done(function (data, xhr) {
    //        $("#records").html(data);
    //        $(".panel-body").show();
    //        if ($("#countRecords").html() > 0) {
    //            $("#btnProcesar").attr("disabled", false);
    //        }
    //    });
    //});
    
    $("#fileInput").change(function (e) {
        $("#btnProcesar").prop("disabled", true);
        var files = e.target.files;
        //SI EXISTE UN DOCUMENTO 
        if (files.length > 0) {
            if (window.FormData !== undefined) {
                var data = new FormData();
                data.append("UploadedImage", files[0]);
                //MANDA LA INFORMACION DEL EXCEL AL CONTROLADOR
                $.ajax({
                    type: "POST",
                    url: "/Empleados/UploadFile",
                    contentType: false,
                    processData: false,
                    data: data
                }).done(function (data, textStatus, jqXHR) {
                    $("#records").html(data);

                    $(".panel-body").show();
                    //VERIFICA SI EXISTE EL ELEMENTO QUE SOLO ESTA DISPONIBLE SI LA INFROMACION ES CORRECTA
                    if ($("#countRecords").length > 0) {
                        $("#btnProcesar").prop("disabled", false).off().click(function () {
                            //SE HACE UN POST PARA VERIFICAR QUE LA INFORMACION SE REGISTRE CORRECTAMENTE
                            $.post("/Empleados/UploadRecords").done(function (data) {
                                utils.showMessage("ATENCION", data.msg, 4000, data.type);
                                $("#btnProcesar").hide();
                                //SE ESPERA A QUE SE PUEDA LEER EL MENSAJE Y SE REDIRECCIONA AL INDEX DE EMPLEADOS
                                setTimeout(function () {
                                    window.location.href = "/Empleados/Index";
                                }, 3500);
                            });
                        });
                    } else {
                        $("#fileInput").val(null);
                    }
                });
            } else {
                utils.showMessage("ATENCION", "Este navegador no soporta la carga de archivos.", 4000, "danger");
            }
        }

    });


    //$("#btnProcesar").click(function () {
    //    $("#btnProcesar").prop("disabled", true);
    //    window.location.href = "/Empleados/UploadRecords";
    //});
});