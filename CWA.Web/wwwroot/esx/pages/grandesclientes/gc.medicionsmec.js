// manage tipoidentificacion from agente
cwa.page.identificacion = function () {
    var idagente = $("#cwa-agente-select").val();

    if (idagente) {
        $(cwa.page.identificaciones).each(function () {
            if (idagente == this.refVal1) {
                $("#cwa-identificacion-label").text(this.descripcion);
                $("#cwa-identificacion-hidden").val(this.id);
            }
        });
    }
    else {
        $("#cwa-identificacion-label").text(cwa.page.tituloCampo);
        $("#cwa-identificacion-hidden").val("");
    }
};

$("#cwa-agente-select").change(cwa.page.identificacion);

if (cwa.page.editing == true) cwa.page.identificacion();

// Update button
if (cwa.page.showUpdate == true) $("#cwa-update-button").click(function () {
    if (cwa.page.editing == false) {
        cwa.dialog.ask(cwa.page.strings.guardar, "#cwa-update-form", function () {            
            if (cwa.page.jsonResult.success == true) document.querySelector("#cwa-back-link").click();
        });
    }
    else {
        cwa.dialog.ask(cwa.page.strings.guardar, "#cwa-update-form", function () {
            if (cwa.page.jsonResult.success == true) document.querySelector("#cwa-back-link").click();
        });
    }
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