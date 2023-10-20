// Set the application's scripting namespaces, therefore objects should be referenced as cwa.page.*, cwa.dialog.*, etc. 
window.cwa = {
    // Scripting scoped to the current page
    page: {},

    // Configuration of Dialog control (if enabled)
    dialog: {}
};

// Display errors on fields
cwa.setErrorMessages = function (errorContent) {
    const errorIcon = '<i class="bi-exclamation-circle"></i>';

    // Clear error messages
    $(".cwa-small").html("&nbsp;");

    // Update messages
    $(errorContent).each(function () {
        const id = "#" + this.key;
        $(id).html(errorIcon);
    });
}

// Empty all control fields on the specified form element
cwa.clearForm = function (formName) {

    var frm_elements = document.querySelector(formName).elements;

    for (i = 0; i < frm_elements.length; i++) {

        var field_type = frm_elements[i].type.toLowerCase();

        switch (field_type) {
            case "text":
            case "email":
            case "password":
            case "textarea":
                frm_elements[i].value = "";
                break;

            case "radio":
            case "checkbox":
                if (frm_elements[i].checked) {
                    frm_elements[i].checked = false;
                }
                break;

            case "select-one":
            case "select-multi":
                frm_elements[i].selectedIndex = 0;
                break;

            case "file":
                var filecontrol = $(frm_elements[i]);
                filecontrol.replaceWith(filecontrol.val("").clone(true));
                break;

            default:
                break;
        }

    }
};

// Set text to an element
cwa.settext = function (elementName, textString) {
    $(elementName).text(textString);
};

// Copy object to another >>>USE WITH CAUTION<<<
cwa.copy = function (source) {
    return $.extend({}, source);
};

// Initialize Bootstrap tooltips
cwa.bstooltips = function () {
    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));
};