var interval;

class Utils {    

    LoadActivePage() {
        $(".active-page").find(".btnSideBar").click();
    }

    IniciarServicioPeriodo() {
        interval = setInterval(function () {
            utils.PeriodoProcesando();
        }, 30000);
    }

    DetenerServicioPeriodo() {
        clearInterval(interval);
    }

    //Comprueba que el status del proceso del periodo
    PeriodoProcesando() {
        var request = $.ajax({
            url: "/Home/PeriodoProcesando",
            method: "GET",
        });
        request.done(function (data) {
            var loading = $("#LoadingProcess");
            var visible = loading.is(":visible");
            if (data) {
                if (!visible) {
                    $(".active-page").find(".btnSideBar").click();
                    loading.switchClass("hidden", "visible", 1000);
                }
            }
            else {
                if (visible) {
                    $(".active-page").find(".btnSideBar").click();
                    loading.switchClass("visible", "hidden", 1000);
                }
            }
        })
        return request;
    }

    ///Muestra el dialogo de confirmación
    confirmDialog(title, subtitle, positive, negative, cb) {
        $("#confirm-title h5").html(title);
        if (subtitle)
            $("#confirm-subtitle p").html(subtitle);

        if (positive)
            $("#confirm-positive h5").html(positive);

        if (negative)
            $("#confirm-negative h5").html(negative);

        $("#confirm").show("fade");

        $("#confirm-positive").on("click", confirmPositive);
        $("#confirm-negative").on("click", confirmNegative);

        function confirmPositive() {
            cb(true);
            restoreDialog();
        }

        function confirmNegative() {
            cb(false);
            restoreDialog();
        }

        function restoreDialog() {
            $("#confirm").hide("fade");
            $("#confirm-positive").unbind();
            $("#confirm-negative").unbind();
            setTimeout(function () {
                $("#confirm-title h5").html("");
                $("#confirm-subtitle p").html("");
                $("#confirm-positive h5").html("Si");
                $("#confirm-negative h5").html("Cancelar");
            }, 1000);
        }
    }

    showMessage(title, message, delay, tipo) {
        if (!tipo) {
            tipo = "info";
        }
        var notification = $.notify({
            title: title,
            message: message,
        }, {
            allow_dismiss: true,
            type: tipo,
            delay: delay,
            offset: {
                y: 55,
                x: 10
            },
            z_index: 10000,
            newest_on_top: true,
            template: '<div data-notify="container" class="col-xs-11 col-sm-3 alert alert-{0}" role="alert">' +
                        '<button type="button" aria-hidden="true" class="close" data-notify="dismiss" style="position: absolute; right: 10px; top: 5px; z-index: 10312;">×</button>' + 
		                '<span data-notify="title">{1}</span>' +
		                '<span data-notify="message">{2}</span>' +
	                    '</div>'
        });

        return notification;
    }

    loadMainPage(controller, action, target, hasDataTable, parametroOpcional) {
        var request = $.ajax({
            url: "/" + controller + "/" + action,
            dataType: "html",
            method: "GET",
            data: {
                id: parametroOpcional
            },
            beforeSend: function (xhr) {
                $(target).hide("fade", 100);
                utils.showProgress();
                //DataTables = [];
                $(target).empty();
            }
        });

        request.always(function () {
            utils.hideProgress();
        });

        request.done(function (data) {
            $(target).html(data);
            $(target).css("visibility", "hidden");
            $(target).show();

            var DataTables = $(target).find(".DataTable");
            if (hasDataTable) {
                DataTables.each(function () {
                    initTable($(this));
                });
            }

            var TableSelect = $(target).find(".table-select");
            if (TableSelect.length > 0) {
                TableSelect.each(function () {
                    var table = $(this);
                    initTableSelect($(this));
                });
            }

            var TableMultiSelect = $(target).find(".table-multiselect");
            if (TableMultiSelect.length > 0) {
                TableMultiSelect.each(function () {
                    var table = $(this);
                    initTableSelect($(this));
                });
            }
            $(target).hide();
            $(target).css("visibility", "visible");
            $(target).show("fade", 100);
        });

        return request;
    }

    dateToString(date) {
        var mm = date.getMonth();
        var mm = mm + 1;
        var dd = date.getDate();
        var yyyy = date.getFullYear();
        if (mm < 10) {
            mm = "0" + mm;
        }
        if (dd < 10) {
            dd = "0" + dd;
        }

        var dateToString = yyyy + "-" + mm + "-" + dd;

        return dateToString;
    }

    

    //Cierra sesión
    LogOut() {
        window.location.href = "/Acceso/LogOut/";
    }

    showProgress() {
        $("#progress").css("visibility", "visible");
    }

    hideProgress() {
        $("#progress").css("visibility", "hidden");
    }
}
var utils = new Utils();