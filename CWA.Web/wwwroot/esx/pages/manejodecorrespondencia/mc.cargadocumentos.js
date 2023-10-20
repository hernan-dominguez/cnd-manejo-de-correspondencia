// Manage tema/subtema
$("#cwa-tema-select").change(function () {
    const temaid = $(this).val();
    const defaultItem = '<option value="">Seleccione un valor...</option>';

    // Reset inputs
    $("#cwa-subtema-select").empty().append(defaultItem);

    if (temaid) {
        $(cwa.page.subtemas).each(function () {
            const spec = this;

            if (temaid == spec.tema) {

                $("#cwa-subtema-select").append('<option value="' + spec.idSubtema + '">' + spec.nombreSubtema + '</option>');
            }
        });
    }
});

// Manage Nuevo Doc/Respuesta
$("#cwa-nuevodocresp-select").change(function () {
    const nuevodocresp = $(this).val();
    const defaultItem = '<option value="">Seleccione un valor...</option>';

    // Reset inputs
    $("#cwa-docorig-select").empty().append(defaultItem);

    if (nuevodocresp && nuevodocresp == "Respuesta") {
        $(cwa.page.respnotasal).each(function () {
            const spec = this;
            $("#cwa-docorig-select").append('<option value="' + spec.id + '">' + spec.numeroDeNota + '</option>');
        });
    }
});

// Page controls
cwa.page.ct = {
    update: $("#cwa-update-button"),
    back: $("#cwa-back-link")
};

// Update button
if (cwa.page.showUpdate) $(cwa.page.ct.update).click(function () {
    cwa.dialog.ask(cwa.page.strings.guardar, "#cwa-update-form", function () {
        if (cwa.page.jsonResult.success == true) document.querySelector("#cwa-back-link").click();
    });
});

// Executes right after Ajax return, but before dismissing the dialog
cwa.page.complete = function () {
    if (cwa.page.jsonResult.success == true) {
        if (cwa.page.jsonResult.content == "update") {

            // Clear error messages
            $(".cwa-small").html("&nbsp;");
        }

        if (cwa.page.jsonResult.content == "approved") {
            cwa.settext("#cwa-approve-date", cwa.page.jsonResult.timeString);
            cwa.settext("#cwa-approved", cwa.page.strings.aprobado);

            if ($("#cwa-approved-icon").hasClass("bi-clock-fill")) {
                $("#cwa-approved-icon").removeClass("bi-clock-fill").removeClass("text-warning");
                $("#cwa-approved-icon").addClass("bi-check-circle-fill").addClass("text-success");
            }

            // clean
            $(cwa.page.ct.back).show();

            $(cwa.page.ct.rejection).remove();
            $(cwa.page.ct.approve).remove();
            $(cwa.page.ct.reject).remove();
            $(cwa.page.ct.update).remove();
            $(cwa.page.ct.text).remove();
        }

        if (cwa.page.jsonResult.content == "rejected") {
            cwa.settext("#cwa-approve-date", cwa.page.jsonResult.timeString);
            cwa.settext("#cwa-approved", cwa.page.strings.noaprobado);

            if ($("#cwa-approved-icon").hasClass("bi-clock-fill")) {
                $("#cwa-approved-icon").removeClass("bi-clock-fill").removeClass("text-warning");
                $("#cwa-approved-icon").addClass("bi-exclamation-circle-fill").addClass("text-danger");
            }

            // Append motivo
            cwa.settext("#cwa-motivo-span", $("textarea").val());
            $("#cwa-motivo-div").removeClass("d-none");

            // clean
            $(cwa.page.ct.back).show();

            $(cwa.page.ct.rejection).remove();
            $(cwa.page.ct.approve).remove();
            $(cwa.page.ct.reject).remove();
            $(cwa.page.ct.update).remove();
            $(cwa.page.ct.text).remove();
        }
    }
    else {
        if (cwa.page.jsonResult.content) cwa.setErrorMessages(cwa.page.jsonResult.content);
    }
};


