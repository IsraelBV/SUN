$(".tooltipForm").each(function () {
    $(this).validate({
        showErrors: function (errorMap, errorList) {

            //Quitar tooltips cuando el elemento es válido
            $.each(this.validElements(), function (index, element) {
                var $element = $(element);
                $element.data("title", "")
                    .removeClass("error")
                    .addClass("success")
                    .tooltip("destroy");
            });

            //agregar tooltip cuando el elemento sea inválido
            $.each(errorList, function (index, error) {
                var $element = $(error.element);

                $element.tooltip("destroy")
                    .data("title", error.message)
                    .removeClass("success")
                    .addClass("error")
                    .tooltip();
            });
        }
    });
});

function validateForm($form) {
    $form.validate({
        showErrors: function (errorMap, errorList) {

            //Quitar tooltips cuando el elemento es válido
            $.each(this.validElements(), function (index, element) {
                var $element = $(element);
                $element.data("title", "")
                    .removeClass("error")
                    .addClass("success")
                    .tooltip("destroy");
            });

            //agregar tooltip cuando el elemento sea inválido
            $.each(errorList, function (index, error) {
                var $element = $(error.element);

                $element.tooltip("destroy")
                    .data("title", error.message)
                    .removeClass("success")
                    .addClass("error")
                    .tooltip();
            });
        }
    });
}