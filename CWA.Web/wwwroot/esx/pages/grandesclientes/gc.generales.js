// Update button
if (cwa.page.showUpdate == true) $("#cwa-update-button").click(function () { cwa.dialog.ask(cwa.page.strings.guardar, "#cwa-update-form"); });

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

            $("#cwa-approve-button").remove();
            $("#cwa-update-button").remove();
        }
    }
    else {
        if (cwa.page.jsonResult.content) cwa.setErrorMessages(cwa.page.jsonResult.content);
    }
};