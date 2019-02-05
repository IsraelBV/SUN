$(document).on("click", ".data-selector .btnSelectAll", function () {
    var table = $(this).data("table-select");

    var rows = $(table).find(".unselected");
    rows.each(function () {
        var row = $(this);
        selectRow(row);
    });
    rows = $(table).find(".selected");
    $(table).trigger("tableSelect:selectAll", [rows]);
    checkTablePanel($(table));
});

$(document).on("click", ".data-selector .btnDeselectAll", function () {
    var table = $(this).data("table-select");

    var rows = $(table).find(".selected");
    rows.each(function () {
        var row = $(this);
        deselectRow(row);
    });
    rows = $(table).find(".unselected");
    $(table).trigger("tableSelect:deselectAll", [rows]);
    checkTablePanel($(table));
});

/* Selector de Tabla Multiples Filas */
$(document).on("click", ".table-select .unselected", function () {
    var table = $(this).parents(".table-select");
    selectRow($(this));

    table.trigger("tableSelect:selected", [$(this)]);

    checkTablePanel(table);
});

$(document).on("click", ".table-select .selected", function () {
    var table = $(this).parents(".table-select");
    deselectRow($(this));

    table.trigger("tableSelect:unselected", [$(this)]);

    checkTablePanel(table);
});

function selectRow($row) {
    $row.removeClass("unselected");
    $row.addClass("selected");
    $row.find(".iconSelect").switchClass("glyphicon-unchecked", "glyphicon-check", 0);
}

function deselectRow($row) {
    $row.removeClass("selected");
    $row.addClass("unselected");
    $row.find(".iconSelect").switchClass("glyphicon-check", "glyphicon-unchecked", 0);
}

function checkTablePanel(table) {
    var selected = table.find(".selected");
    var rows = table.find("tbody").find("tr");

    var tableId = table.attr("id");
    var btnDeselectAll = $(".btnDeselectAll[data-table-select='#" + tableId + "']");
    var btnSelectAll = $(".btnSelectAll[data-table-select='#" + tableId + "']");

    if (selected.length < rows.length) {
        if (btnDeselectAll.is(":visible"))
            btnDeselectAll.hide();
        if (btnSelectAll.is(":hidden"))
            btnSelectAll.show();
    }
    else if (selected.length == rows.length) {
        if (btnSelectAll.is(":visible")) {
            btnSelectAll.hide();
        }
        if(btnDeselectAll.is(":hidden"))
            btnDeselectAll.show();
    }
    var numRows = $(".table-select-counter[data-table-select='#" + tableId + "']").find(".numRows");
    var text;
    if (selected.length == rows.length) {
        text = "TODOS";
    }
    else {
        text = selected.length;
    }
    numRows.html(text);
    table.trigger("tableSelect:change", [rows, selected]);
}

/* Selector de Tabla Unifila */
$(document).on("click", ".table-uniselect .selected", function () {
    var $row = $(this);
    
    $row.removeClass("selected");
    $row.removeClass("double-select");
    $row.addClass("unselected");

    $row.parent().parent().trigger("tableUniselect:deselect");
});

$(document).on("click", ".table-uniselect .unselected", function () {
    var $row = $(this);
    var $table = $row.parent().parent();
    
    $table.find(".double-select").removeClass("selected").removeClass("double-select").addClass("unselected");
    $table.find(".selected").removeClass("selected").addClass("unselected");
    $row.removeClass("unselected");
    $row.addClass("double-select"); //Para accionar el evento de doble select

    setTimeout(function () {
        $row.removeClass("double-select");
        if(!$row.hasClass("unselected")){
            $row.addClass("selected")
        }
    }, 2000);

    $table.trigger("tableUniselect:select");
});

$(document).on("click", ".table-uniselect .double-select", function () {
    var $row = $(this);
    $row.parent().parent().trigger("tableUniselect:double-select");
});

/* Selector de tablas unifila y multifila con la tecla "CTRL" */
$(document).on("click", ".table-multiselect .selected", function (e) {
    var row = $(this);
    var table = row.parent().parent()

    if (!e.ctrlKey) {
        var otherRows = row.siblings(".selected");
        if (otherRows.length > 0) {
            otherRows.each(function () {
                deselectRow($(this));
            });
            table.trigger("tableMultiselect:select", [row]);
        }
        else {
            deselectRow(row);
        }
        table.trigger("tableMultiselect:unselect");
    }
    else {
        deselectRow(row);
        var selected = table.find(".selected");
        table.trigger("tableMultiselect:multiselect", [selected]);
    }

    checkTablePanel(table);
});

$(document).on("click", ".table-multiselect .unselected", function (e) {
    var row = $(this);
    var table = row.parent().parent();
    selectRow(row);

    if (!e.ctrlKey) {
        var otherRows = row.siblings(".selected");
        otherRows.each(function () {
            deselectRow($(this));
        })
        table.trigger("tableMultiselect:select", [row]);
    }
    else {
        var selected = table.find(".selected");
        table.trigger("tableMultiselect:multiselect", [selected]);
    }

    checkTablePanel(table);
});

function initTableSelect(table) {
    var tableId = table.attr("id");
    var removeButtons = table.data("select-buttons");
    var removeCounter = table.data("select-counter");
    var removeIcons = table.data("select-icons");
    
    var totalRows = table.find("tbody").find("tr");
    var rowsSelected = table.find(".selected");
    var rowsUnselected = table.find(".unselected");

    if (totalRows.length != (rowsSelected.length + rowsUnselected.length)) {
        totalRows.each(function () {
            var row = $(this)
            var rowSelected = row.hasClass("selected");
            var rowUnselected = row.hasClass("unselected");
            if (!rowSelected) {
                if (!rowUnselected) {
                    row.addClass("unselected");
                }
            }
            else if (!rowUnselected) {
                if (!rowSelected) {
                    row.addClass("unselected");
                }
            }
        });
    }

    rowsSelected = table.find(".selected");
    rowsUnselected = table.find(".unselected");

    if (removeIcons != false) {
        table.find("thead").find("tr").prepend("<th class='data-selector'></th>");
        rowsUnselected.each(function () {
            var row = $(this);
            row.prepend("<td><span class='glyphicon glyphicon-unchecked iconSelect'></span></td>");
        });

        rowsSelected.each(function () {
            var row = $(this);
            row.prepend("<td><span class='glyphicon glyphicon-check iconSelect'></span></td>");
        });
    }

    if (removeButtons != false) {
        var btnSelect = "<a class='btnSelectAll' title='SELECCIONAR TODO' data-table-select='#" + tableId + "'><span class='glyphicon glyphicon-unchecked'></span></a>";
        var btnDeselect = "<a class='btnDeselectAll' title='DESELECCIONAR TODO' data-table-select='#" + tableId + "'><span class='glyphicon glyphicon-check'></span></a>";
        var buttons =  btnSelect + btnDeselect;
        table.find(".data-selector").append(buttons);
        if (rowsSelected.length == totalRows.length) {
            table.find(".btnSelectAll").hide();
        }
        else {
            table.find(".btnDeselectAll").hide();
        }
    }

    if (removeCounter != false) {
        var text;
        if (rowsSelected.length == totalRows.length)
            text = "TODOS";
        else
            text = rowsSelected.length;

        var tableFooter = table.next(".table-footer");
        var counter = "<div class='table-select-counter' data-table-select='#" + tableId + "'> REGISTROS SELECCIONADOS: <span class='numRows'>" + text + "</span></div>"
        if (tableFooter.length == 0) {
            counter = "<div class='table-footer'>" + counter + "</div>";
            table.after(counter);
        }
        else {
            tableFooter.append(counter);
        }
    }

    table.trigger("tableSelect:init", [totalRows, rowsSelected, rowsUnselected]);
}

function destroyTableSelect(table) {
    var tableId = table.attr("id");
    var removeButtons = table.data("select-buttons");
    var removeCounter = table.data("select-counter");
    var removeIcons = table.data("select-icons");

    var totalRows = table.find("tbody").find("tr");
    var rowsSelected = table.find(".selected");
    var rowsUnselected = table.find(".unselected");

    totalRows.each(function () {
        var $row = $(this);
        $row.removeClass("unselected").removeClass("selected");
        if (removeIcons != false) {
            $row.first("td").remove();
        }
    });

    if (removeButtons != false) {
        table.find(".data-selector").remove();
    }

    if (removeCounter != false) {
        table.next(".table-footer").remove();
    }
}

var DataTables = [];

function initTable(table) {
    var tableData = [];
    var TableId = table.attr("id");
    var tablePanel = table.prev(".table-panel");
    var input = "<div class='search-filter'><a class='btnSearch'><span class='glyphicon glyphicon-search'></span> BUSCAR</a> <input type='search' placeholder='BÚSQUEDA' class='table-filter' data-table='" + TableId + "'/></div>";
    if (tablePanel.length == 0) {
        input = "<div class='table-panel'>" + input + "</div>";
        table.before(input);
    }
    else {
        tablePanel.prepend(input);
    }
    var rows = table.find("tbody").find("tr");
    var rowsVisibles = 0;
    rows.each(function () {
        var $row = $(this);
        if ($row.is(":visible")) {
            var rowData = [];
            var columns = $row.find("td");

            $row.addClass("tableRow");
            $row.data("row", rowsVisibles);
            rowData.push(rowsVisibles); //inserta el número de fila para el algoritmo de ordenamiento

            columns.each(function () {
                var data = $(this).text();
                data = isNaN(data) ? data.toUpperCase() : parseFloat(data);
                rowData.push(data);
            });

            tableData.push(rowData);
            rowsVisibles++;
        } else {
            $row.removeClass("tableRow");
        }
    });

    DataTables[TableId] = tableData;

    var counter = "<p class='DataTableNumRegistros " + TableId + " '><span class='numRegistros'>" + rowsVisibles + " </span> <span class='textRegistros'>REGISTROS EN TOTAL</span></p>";
    if (table.data("count")) {
        var parentCount = table.data("count");
        $(parentCount).prepend(counter);
    }
    else {
        var tableFooter = table.next(".table-footer");
        if (tableFooter.length == 0) {
            tableFooter = "<div class='table-footer'>" + counter + "</div>";
            table.after(tableFooter);
        }
        else {
            tableFooter.after(counter);
        }
    }

}

function destroyTable(table) {
    var rows = table.find("tbody").find("tr");
    rows.each(function () {
        rows.show();
    });
    var TableId = table.attr("id");
    var tablePanel = table.prev(".table-panel");
    var DataTableName = tablePanel.find(".table-filter").data("table");
    tablePanel.remove();
    $(".DataTableNumRegistros." + TableId).remove();
    delete DataTables[DataTableName];
}

function searchInTable(value, table) {
    var tableData = DataTables[table];
    var numRegistros = 0;
    for (var row = 0; row < tableData.length; row++) {
        var numColumns = tableData[row].length;
        var found = false;
        for (var column = 1; column < numColumns; column++) {
            var data = tableData[row][column];
            data = data.toString();
            if (data.indexOf(value) != -1) {
                found = true;
                break;
            }
        }
        var element = $("#" + table).find(".tableRow").get(row);
        if (found) {
            numRegistros++;
            element.removeAttribute("hidden");
        }
        else {
            element.setAttribute("hidden", true);
        }
    }
    var $NumRegistros = $(".DataTableNumRegistros." + table);
    $NumRegistros.find(".numRegistros").html(numRegistros);
    var text = numRegistros == tableData.length ? "REGISTROS EN TOTAL" : "REGISTROS ENCONTRADOS";
    $NumRegistros.find(".textRegistros").html(text);
}

function countNumRows(table) {
    var rows = table.find("tbody").find("tr");
    var tableId = table.attr("id");
    var rowsVisible = rows.length;
    rows.each(function () {
        if ($(this).is(":hidden")) {
            rowsVisible--;
        }
    });

    var $NumRegistros = $(".DataTableNumRegistros." + tableId);
    $NumRegistros.find(".numRegistros").html(rowsVisible);
    $NumRegistros.find(".textRegistros").html("REGISTROS EN TOTAL");
}

function reconstructDataTable(table) {
    var TableId = table.attr("id");
    delete DataTables[TableId];

    var tableData = [];
    var rows = table.find("tbody").find("tr");
    var rowsVisibles = 0;
    rows.each(function () {
        if ($(this).is(":visible")) {
            $(this).addClass("tableRow");
            rowsVisibles++;
            var rowData = [];
            var columns = $(this).find("td");
            columns.each(function () {
                var data = $(this).text();
                if (!isNaN(data)) //valida si el dato es un número
                    data = parseFloat(data);
                rowData.push(data.toUpperCase());
            });

            tableData.push(rowData);
        } else {
            $(this).removeClass("tableRow");
        }
    });
    DataTables[TableId] = tableData;
    countNumRows(table);
}

$(document).on("click", ".btnSearch", function () {
    if (!$(this).attr("disabled")) {
        $(this).hide();
        $(this).next().show("fade").focus();
        $(this).attr("disabled", true);
    }
});

$(document).on("keyup", ".table-filter", function (e) {
    if (e.keyCode == 27) {
        $(this).hide();
        var btn = $(this).prev().show("fade", 1000);
        setTimeout(function () {
            btn.attr("disabled", false);
        }, 1000);
        var keyword = "";
    }
    else {
        var keyword = $(this).val().toUpperCase();
    }
    var table = $(this).data("table");
    searchInTable(keyword, table);
});

//$(document).on("focusout", ".table-filter", function (e) {
//    $(this).hide();
//    var btn = $(this).prev().show("fade", 1000);
//    setTimeout(function () {
//        btn.attr("disabled", false);
//    }, 1000);
//    var keyword = "";
//    $(this).val("");
//    var table = $(this).data("table");
//    searchInTable(keyword, table);
//});









function sortTable($table, column, order) {
    let TableId = $table.attr("id");
    var table = DataTables[TableId];
    
    column = column + 1;
    if (order == 1)
        table = table.sort(sortTableAsc(column));
    else
        table = table.reverse();

    orderTable($table, table);
    DataTables[TableId] = table;
}

function orderTable($table, tableArray) {
    let $tBody = $table.find("tbody");
    let $rows = $table.find(".tableRow");
    for (let i = 0; i < tableArray.length; i++) {
        let row = tableArray[i];
        let $row = $rows.filterByData("row", row[0]);
        $tBody.append($row);
    }
}

//function sortTableDesc($rows, column) {
//    return function (a, b) {
//        let $row = $rows.filterByData('row', a[0]);
//        let $next = $rows.filterByData("row", b[0]);

//        if(a[column] === b[column]) {
//            if (a[0] < b[0]) return 1;
//            else return -1;
//        } 
//        else {
//            if (a[column] < b[column]) {
//                $next.after($row);
//                return 1;
//            }
//            else return -1;
//        }
//    }
//}

function sortTableAsc(column) {
    return function (a, b) {
        //let $rowA = $rows.filterByData('row', a[0]);
        //let $rowB = $rows.filterByData("row", b[0]);

        if (a[column] === b[column]) return 0;
        else {
            if (a[column] > b[column]) {
                //$rowA.before($rowB);
                return 1;
            }
            else {
                //$rowB.before($rowA);
                return -1;
            }
        }
    }
}

$.fn.filterByData = function (prop, val) {
    return this.filter(function () { return $(this).data(prop) == val; })
}