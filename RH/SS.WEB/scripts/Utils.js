var interval;
class Utils {
    showMessage(title, message, delay, tipo) {
        if (!tipo) {
            tipo = "info";
        }
        var notification = $.notify({
                title: title,
                message: message,
            },
            {
                allow_dismiss: true,
                type: tipo,
                delay: delay,
                placement: {
                    from: 'top',
                    align: 'center'
                },
                z_index: 10000,
                newest_on_top: true
            });

        return notification;


    }
}
var utils = new Utils();

function bajaReporte(ruta) {
    if (ruta.length > 0) {
        var form = document.createElement("form");
        form.setAttribute("method", "post");
        form.setAttribute("action", "/Reportes/DescargarArchivo");

        form._submit_function_ = form.submit;        
            var hiddenField = document.createElement("input");
            hiddenField.setAttribute("type", "hidden");
            hiddenField.setAttribute("name", "ruta");
            hiddenField.setAttribute("value",ruta);
            form.appendChild(hiddenField);
        

        document.body.appendChild(form);
        form._submit_function_();

    }
};

function bajarTXT(uri, name) {
    var link = document.createElement("a");
    link.download = name;
    link.href = uri;
    link.click();
}