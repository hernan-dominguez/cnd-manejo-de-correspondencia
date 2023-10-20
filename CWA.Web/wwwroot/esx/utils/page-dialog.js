// Common configuration
cwa.dialog.config = {
    position: "top",
    buttonsStyling: false,
    customClass: {
        confirmButton: "btn btn-sm btn-primary ms-1 me-1 cwa-dialog-button",
        cancelButton: "btn btn-sm btn-danger ms-1 me-1 cwa-dialog-button"
    },
    showClass: {
        backdrop: "swal2-noanimation"
    },
    hideClass: {
        backdrop: "swal2-noanimation"
    },
    confirmButtonText: cwa.dialog.parameters.confirmButtonText,
    cancelButtonText: cwa.dialog.parameters.cancelButtonText,
    showCancelButton: false,
    allowOutsideClick: false,
    allowEscapeKey: false
};

// Simple call configuration
cwa.dialog.simple = cwa.copy(cwa.dialog.config);

// Confirmation call configuration
cwa.dialog.confirm = cwa.copy(cwa.dialog.config);
cwa.dialog.confirm.icon = "question";
cwa.dialog.confirm.showCancelButton = true;

// Submitting result
cwa.dialog.complete = (xhr) => {
    swal.hideLoading();

    if (xhr.status == "200") {
        cwa.page.jsonResult = JSON.parse(xhr.responseText);

        swal.update({
            text: cwa.page.jsonResult.message,
            icon: (cwa.page.jsonResult.success == true ? "success" : "warning")
        });

        // On successful submit, call the page's complete function (if exists)
        if (cwa.page.complete) cwa.page.complete();
    }
    else {
        swal.update({
            text: cwa.dialog.parameters.errorText,
            icon: "error"
        });
    }
};

// Simple message function
cwa.dialog.show = (message, dialogIcon) => {
    cwa.dialog.simple.text = message;
    cwa.dialog.simple.icon = dialogIcon.toLowerCase();

    swal.fire(cwa.dialog.simple);
};

// Confirmation dialog
cwa.dialog.ask = (message, formName, onDismissed) => {
    cwa.dialog.confirm.text = message;

    swal.fire(cwa.dialog.confirm).then((proceed) => {
        if (proceed.isConfirmed) {
            swal.fire(cwa.dialog.config).then(function () { if (onDismissed) onDismissed(); });
            swal.showLoading();
            $(formName).submit();
        }
    });
};

// No confirmation
cwa.dialog.go = (formName, onDismissed) => {
    swal.fire(cwa.dialog.config).then(function () { if (onDismissed) onDismissed(); });
    swal.showLoading();
    $(formName).submit();
};