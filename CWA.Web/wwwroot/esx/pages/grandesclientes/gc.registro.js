// Update button
if (cwa.page.showUpdate == true) $("#cwa-update-button").click(function () { cwa.dialog.ask(cwa.page.strings.guardar, "#cwa-update-form"); });

// Notify button
if (cwa.page.showNotify == true) $("#cwa-notify-button").click(function () { cwa.dialog.ask(cwa.page.strings.notificar, "#cwa-notify-form"); });

// Executes right after Ajax return, but before dismissing the dialog
cwa.page.complete = (jsonResult) => {
    if (cwa.page.jsonResult.success == true) {
        if (cwa.page.jsonResult.content == "update") {
            if ($("#cwa-fecha-estatus").length == 0) {
                $("div.card-body div.row").append('<div id="cwa-fecha-estatus" class="col-auto col-md-4"></div>');
                $("#cwa-fecha-estatus").append('<small class= "d-block fw-bold">' + cwa.page.strings.condicion + '</small><span class="d-block">' + cwa.page.strings.programada + '</span>');
            }

            cwa.settext("#cwa-last-save", cwa.page.jsonResult.timeString);
        }

        if (cwa.page.jsonResult.content == "notify") $("#cwa-notify-button").remove();
    }
};