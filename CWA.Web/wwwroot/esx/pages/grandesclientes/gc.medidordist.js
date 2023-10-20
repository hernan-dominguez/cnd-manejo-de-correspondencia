// console.log(cwa.page);

// Manage fabricante/modelo
$("#cwa-fabricante-select").change(function () {
    const fabricanteid = $(this).val();
    const defaultItem = '<option value="">Seleccione un valor...</option>';

    // Reset select
    $("#cwa-modelo-select").empty().append(defaultItem);

    if (fabricanteid) {
        $(cwa.page.modelos).each(function () {
            if (fabricanteid == this.fabricanteid) $("#cwa-modelo-select").append('<option value="' + this.id + '">' + this.descripcion + '</option>');
        });
    }
});

// Manage canales
cwa.page.addCanal = function () {
    var numero = $("#cwa-canal-numero").val()
    var descripcion = $("#cwa-descripcion-select").val().trim();
    var display = $("#cwa-descripcion-select option:selected").text();
    var descripcionCompare = $("#cwa-descripcion-select option:selected").text().toLowerCase();

    if (!numero || !descripcion) {
        cwa.dialog.show(cwa.page.strings.canales, "warning");
    }
    else {
        // duplicated numero or descripcion
        var existsNumero = false;
        var existsDescripcion = false;

        $("tr[data-cwa-rid]").each(function () {
            var listaNumero = $(this).find("td").eq(0).text();
            var listaDescripcion = $(this).find("td").eq(1).text();
            listaDescripcion = listaDescripcion.toLowerCase();

            existsNumero = (numero == listaNumero) || existsNumero;
            existsDescripcion = (descripcionCompare == listaDescripcion) || existsDescripcion;
        });

        if (existsNumero == true) {
            cwa.dialog.show(cwa.page.strings.numero, "warning");
        }
        else {
            if (existsDescripcion == true) {
                cwa.dialog.show(cwa.page.strings.descripcion, "warning");
            }
            else {
                // add to table
                $("#cwa-canal-table tbody")
                    .append('<tr data-cwa-rid="">'
                        + '<td>' + numero + '</td>'
                        + '<td>' + display + '</td>'
                        + '<td class="text-end">'
                        + '<a href="#quitar" data-cwa-cid=""><i class="bi-trash"></i></a>'
                        + '<input type="hidden" id="" name="" value="' + numero + '" />'
                        + '<input type="hidden" id="" name="" value="' + descripcion + '" />'
                        + '</td > ');

                // reflow index
                var lastIndex = cwa.page.indexCanal();

                // update the latest entry
                $('[data-cwa-cid="' + lastIndex + '"]').click(cwa.page.removeCanal);

                // clean up and focus
                $("#cwa-descripcion-select").val("");
                $("#cwa-canal-numero").val("").focus();
            }
        }
    }
};

cwa.page.removeCanal = function () {
    $("tr[data-cwa-rid='" + $(this).attr("data-cwa-cid") + "']").remove();

    // reflow index
    cwa.page.indexCanal();
};

cwa.page.indexCanal = function () {
    var newIndex = 0;

    $("tr[data-cwa-rid]").each(function () {
        $(this).attr("data-cwa-rid", newIndex);
        $(this).find("a[data-cwa-cid]").attr("data-cwa-cid", newIndex);
        $(this).find("input").eq(0).prop("id", "EditModel_Canales_" + newIndex + "__Numero");
        $(this).find("input").eq(0).prop("name", "EditModel.Canales[" + newIndex + "].Numero");
        $(this).find("input").eq(1).prop("id", "EditModel_Canales_" + newIndex + "__DescripcionId");
        $(this).find("input").eq(1).prop("name", "EditModel.Canales[" + newIndex + "].DescripcionId");
        newIndex++;
    });

    return newIndex - 1;
}

$("#cwa-canal-add").click(cwa.page.addCanal);
$("[data-cwa-cid]").click(cwa.page.removeCanal);

// Update button
if (cwa.page.showUpdate == true) $("#cwa-update-button").click(function () {
    cwa.dialog.ask(cwa.page.strings.guardar, "#cwa-update-form", function () {
        if (cwa.page.jsonResult.success == true) document.querySelector("#cwa-back-link").click();
    });
});

// Approve button
if (cwa.page.showApprove == true) $("#cwa-approve-button").click(function () {
    if (cwa.page.showUpdate == false) {
        cwa.dialog.ask(cwa.page.strings.aprobar, "#cwa-approve-form");
    }
    else {
        cwa.dialog.ask(cwa.page.strings.aprobar, "#cwa-approve-form", function () { if (cwa.page.showUpdate == true) location.reload(); });
    }
});

// Executes right after Ajax return, but before dismissing the dialog
cwa.page.complete = function () {
    if (cwa.page.jsonResult.success == true) {
        if (cwa.page.jsonResult.content == "update") {
            cwa.settext("#cwa-last-save", cwa.page.jsonResult.timeString);

            // Clear error messages
            $(".cwa-small").html("&nbsp;");
        }

        if (cwa.page.jsonResult.content == "approve") {
            cwa.settext("#cwa-approve-date", cwa.page.jsonResult.timeString);
            cwa.settext("#cwa-approved", cwa.page.strings.aprobado);

            if ($("#cwa-approved-icon").hasClass("bi-clock-fill")) {
                $("#cwa-approved-icon").removeClass("bi-clock-fill").removeClass("text-warning");
                $("#cwa-approved-icon").addClass("bi-check-circle-fill").addClass("text-success");
            }

            // clean
            $("#cwa-approve-button").remove();
            $("#cwa-update-button").remove();
        }
    }
    else {
        if (cwa.page.jsonResult.content) cwa.setErrorMessages(cwa.page.jsonResult.content);
    }
};