$(document).ready(function () {
    var queryDate = $("#current-date").val(),
        dateParts = queryDate.match(/(\d+)/g);
    realDate = new Date(dateParts[0], dateParts[1] - 1, dateParts[2]);

    var today = new Date();

    $("#datepicker").datepicker(
        {
            maxDate: "+0D",
            dateFormat: "DD, d MM, yy",
            onClose: function (selectedDate) {
                var currentDate = $("#datepicker").datepicker("getDate");
                var days = parseInt((today - currentDate) / (1000 * 60 * 60 * 24));
                LoadDay(days);
            }
        });

    $("#datepicker").datepicker("setDate", realDate);
})