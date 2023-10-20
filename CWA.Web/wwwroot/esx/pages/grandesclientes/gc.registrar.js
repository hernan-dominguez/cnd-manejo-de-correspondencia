// Manage tipo select
$("#cwa-tipo-select").change(function () {
    const tipoid = $("#cwa-tipo-select").val();

    // Reset
    $("#cwa-responsable-select").prop("disabled", true);
    $("#cwa-responsable-select").val("");

    $("#cwa-propietario-select").prop("disabled", true);
    $("#cwa-propietario-select").val("");

    if (tipoid) {
        if (tipoid == cwa.page.tipos.pasivo) $("#cwa-responsable-select").prop("disabled", false);
        if (tipoid == cwa.page.tipos.activo) $("#cwa-propietario-select").prop("disabled", false);
    }
});

// Update button
$("#cwa-update-button").click(function () {
    cwa.dialog.ask(cwa.page.strings.guardar, "#cwa-update-form", function () {
        if (cwa.page.jsonResult.success == true) document.querySelector("#cwa-back-link").click();
    });
});

cwa.page.complete = function () {
    if (cwa.page.jsonResult.success == true) {
        $(".cwa-small").html("&nbsp;");
    }
    else {
        if (cwa.page.jsonResult.content) cwa.setErrorMessages(cwa.page.jsonResult.content);
    }
};