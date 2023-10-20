// Manage accepted file extensions (accept attribute)
$("#cwa-documento-select").change(function () {
    const documentoid = $("#cwa-documento-select").val();

    $("#cwa-fileupload").replaceWith($("#cwa-fileupload").val("").clone(true));

    if (documentoid) {
        $("#cwa-fileupload").prop("disabled", false);

        $(cwa.page.documentosDisponibles).each(function () {
            if (documentoid == this.id) {
                $("#cwa-fileupload").prop("disabled", false);
                $("#cwa-fileupload").attr("accept", "." + this.extensiones.replace(",", ",."));
            }
        });
    }
    else {
        $("#cwa-fileupload").prop("disabled", true);
    }
});

// Update button
if (cwa.page.showUpdate == true) {
    $("#cwa-update-button").click(function () {
        cwa.page.complete = cwa.page.upload.complete;
        cwa.dialog.ask(cwa.page.strings.guardar, "#cwa-update-form");
    });
}

if (cwa.page.showApprove) {
    $("#cwa-approve-button").click(function () {
        cwa.page.complete = cwa.page.approve.complete;
        cwa.dialog.ask(cwa.page.strings.aprobar, "#cwa-approve-form");
    });
}

// // Executes right after Ajax return, but before dismissing the dialog
cwa.page.upload = {
    complete: function () {
        if (cwa.page.jsonResult.success == true) {
            var data = cwa.copy(cwa.page.jsonResult.content);
            var tr = $('tr[data-cwa-rid="' + data.id + '"]');

            // Clear error messages
            $(".cwa-small").html("&nbsp;");

            if ($(tr).length == 1) {
                // Update
                $(tr).find("td").eq(0).find("a").eq(0).html(data.descripcion);
                $(tr).find("td").eq(1).find("span").eq(0).text(data.actualizado);

                var estatus = $(tr).find("td").eq(2).find("span").eq(0);
                var aprobacion = $(tr).find("td").eq(3).find("span").eq(0);

                if (data.autoAccepted == true) {
                    estatus.replaceWith('<span>' + cwa.page.strings.aprobado + ' <i class="bi-check-circle-fill text-success"></i></span>');
                    aprobacion.text(data.aprobado);
                }
                else {
                    estatus.replaceWith('<span>' + cwa.page.strings.pendiente + ' <i class="bi-clock-fill text-warning"></i></span>');
                }

                $("#cwa-documento-table tbody").prepend($(tr));
            }
            else {
                // Add
                var route = $("#cwa-link-template").attr("href") + "/" + data.id;

                // Row
                var row = $('<tr data-cwa-rid="' + data.id + '">');

                // Cells
                $(row).append('<td class="align-middle"><a href="' + route + '">' + data.descripcion + '</a></td>');
                $(row).append('<td class="text-center align-middle"><span>' + data.actualizado + '</span></td>');

                if (data.autoAccepted == true) {
                    $(row).append('<td class="text-center align-middle"><span>' + cwa.page.strings.aprobado + ' <i class="bi-check-circle-fill text-success"></i></span></td>')
                    $(row).append('<td class="text-center align-middle"><span>' + data.aprobado + ' </span></td>');

                    if ($("#cwa-radio-th").length) {
                        $(row).append('<td><span>&nbsp;</span></td>');
                    }
                }
                else {
                    $(row).append('<td class="text-center align-middle"><span>' + cwa.page.strings.pendiente + ' <i class="bi-clock-fill text-warning"></i><span></td>');
                    $(row).append('<td class="text-center align-middle"><span>&nbsp;</span></td>');
                }

                // If the uploader has approving rights and the new item is not auto-accepted, add it to the selectables
                if (data.autoAccepted == false && cwa.page.userApprove) {

                    if ($("#cwa-radio-th").length == 0) {
                        // Add the table header
                        $("#cwa-documento-table thead tr").append('<th id="cwa-radio-th" class="text-center">Seleccionar</th>');

                        // Add the approve button
                        var approveButton = '<button id="cwa-approve-button" type="button" class="btn btn-sm btn-success me-1" style="width: 100px;"><span>Aprobar</span></button>';
                        $(approveButton).insertAfter("#cwa-update-button");

                        $("#cwa-approve-button").click(function () {
                            cwa.page.complete = cwa.page.approve.complete;
                            cwa.dialog.ask(cwa.page.strings.aprobar, "#cwa-approve-form");
                        });
                    }

                    // Add the radio element
                    $(row).append('<td class="text-center align-middle"><input class= "form-check-input" type="radio" value= "' + data.id + '" id="DocumentoId" name ="DocumentoId"></td>');

                    // Check if there is a previous item already approved
                    $("#cwa-documento-table tbody tr").each(function (index) {
                        if ($(this).find("td").length == 4) {
                            $(this).append('<td><span>&nbsp;</span></td>');
                        }
                    });
                }

                // Table
                $("#cwa-documento-table tbody").prepend(row);
            }

            // Clear form and disable upload
            cwa.clearForm("#cwa-update-form");
            $("#cwa-fileupload").prop("disabled", true);
        }
        else {
            if (cwa.page.jsonResult.content) {
                // Clear error messages
                $(".cwa-small").html("&nbsp;");

                // Update messages
                $(cwa.page.jsonResult.content).each(function () {
                    const id = "#" + this.key;
                    $(id).html(this.message);
                });
            }
        }
    }

};

// Executes right after Ajax return, but before dismissing the dialog
cwa.page.approve = {
    complete: function () {
        if (cwa.page.jsonResult.content) {

            var data = cwa.copy(cwa.page.jsonResult);
            var tr = $('tr[data-cwa-rid="' + data.id + '"]');

            if ($(tr).length == 1) {
                // Update presentation
                var descripcion = $(tr).find("td").eq(0).find("a").eq(0);
                var estatus = $(tr).find("td").eq(2).find("span").eq(0);
                var aprobacion = $(tr).find("td").eq(3).find("span").eq(0);
                var seleccion = $(tr).find("td").eq(4);

                estatus.replaceWith('<span>' + cwa.page.strings.aprobado + ' <i class="bi-check-circle-fill text-success"></i></span>');
                aprobacion.text(data.timeString);
                seleccion.html("<span>&nbsp;</span>");

                // If the approver has uploading rights and the new item was uploaded in the same page view, remove the option element from select control
                var option;

                $("#cwa-documento-select option").each(function (index) {
                    if ($(this).text() == descripcion.text()) {
                        option = this;
                    }
                });

                $(option).remove();

                // Remove the Approve related elements if all the items on table are approved
                if ($("i.text-warning").length == 0) {
                    $("#cwa-radio-th").remove();
                    $("#cwa-documento-table tbody td:last-child").remove();
                    $("#cwa-approve-button").remove();
                }

                // Remove the upload related elements if there isn't anything available to select
                if ($("#cwa-documento-select option").length == 1) {
                    $("#cwa-update-form").remove();
                    $("#cwa-update-button").remove();
                }
            }
        }
    }
};