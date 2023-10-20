// Manage solicitud/transaccion
$("#cwa-solicitud-select").change(function () {
    const tipoid = $(this).val();
    const defaultItem = '<option value="">Seleccione un valor...</option>';

    // Reset inputs
    $("#cwa-transaccion-select").empty().append(defaultItem);
    $(".cwa-doc-row").remove();

    if (tipoid) {
        $(cwa.page.tipos.solicitudTransacccion).each(function () {
            const spec = this;

            if (tipoid == spec.solicitud) {
                $(cwa.page.tipos.transacciones).each(function () {
                    if (spec.transaccion == this.id) $("#cwa-transaccion-select").append('<option value="' + this.id + '">' + this.descripcion + '</option>');
                });
            }
        });

        // Documentos
        var docIndex = 0;

        $(cwa.page.docs.doctipos).each(function () {
            const lookup = this;

            if (tipoid == lookup.solicitud) {
                $(cwa.page.docs.documentos).each(function () {
                    const docSpec = this;
                    var elementBefore = "cwa-documentos";

                    if (lookup.documento == docSpec.id) {
                        elementBefore = cwa.page.functions.createBinded(docSpec.id, docSpec.descripcion, docSpec.accepts, docIndex, elementBefore);
                        docIndex++;
                    }
                });
            }
        });
    }

    if ($("#cwa-transaccion-select option").length == 2) $("#cwa-transaccion-select option").first().remove();
});

// Manage pais/contraparte
$("#cwa-pais-select").change(function () {
    const paisid = $(this).val();
    const defaultItem = '<option value="">Seleccione un valor...</option>';

    // Reset select
    $("#cwa-contraparte-select").empty().append(defaultItem);

    if (paisid) {
        $(cwa.page.contrapartes).each(function () {
            const agente = this;

            if (paisid == agente.paisId) {
                $("#cwa-contraparte-select").append('<option value="' + agente.id + '">' + agente.nombre + '</option>');
            }
        });
    }
});

// Page specific functions
cwa.page.functions = {
    createBinded: function (tipo, descripcion, accepts, docIndex, elementLocation) {
        // Documento element binding 
        const divId = "cwa-documento-" + docIndex;

        const docId = "EditModel_Documentos_" + docIndex + "__DocumentUpload";
        const docName = "EditModel.Documentos[" + docIndex + "].DocumentUpload";

        const tipoId = "EditModel_Documentos_" + docIndex + "__TipoDocumentoId";
        const tipoName = "EditModel.Documentos[" + docIndex + "].TipoDocumentoId";

        const archivoInput = '<input class="form-control" type="file" data-val="true" data-val-required="El documento es requerido" id="' + docId + '" name="' + docName + '" accept="' + accepts + '" />';
        const tipoInput = '<input type="hidden" data-val="true" data-val-required="El tipo de documento es requerido." id="' + tipoId + '" name="' + tipoName + '" value="' + tipo + '" />'
        const errorSpan = '<span id="EditModel-Documentos-' + docIndex + '-DocumentUpload" class="text-danger cwa-small">&nbsp</span>';
        const template = '<div id="' + divId + '" class="row cwa-doc-row"><div class="col-12 mb-3"><label for="' + docId + '">' + descripcion + '</label>' + archivoInput + errorSpan + tipoInput + '</div>';

        $(template).insertAfter("#" + elementLocation);

        return divId;
    }
}
// Page controls
cwa.page.ct = {
    update: $("#cwa-update-button"),
    approve: $("#cwa-approve-button"),
    reject: $("#cwa-reject-button"),
    approval: $("#cwa-approval-button"),
    rejection: $("#cwa-rejection-button"),
    back: $("#cwa-back-link"),
    file: $("#cwa-adjunto-aprobacion"),
    text: $("#cwa-motivo-rechazo")
};


// Update button
if (cwa.page.showUpdate) $(cwa.page.ct.update).click(function () {
    cwa.dialog.ask(cwa.page.strings.guardar, "#cwa-update-form", function () {
        if (cwa.page.jsonResult.success == true) document.querySelector("#cwa-back-link").click();
    });
});

// Approve button
if (cwa.page.showApprove == true) {
    $(cwa.page.ct.approve).click(function () {
        cwa.dialog.ask(cwa.page.strings.aprobar, "#cwa-approve-form");
    });

    // Add collapse listener if required
    if (cwa.page.showFile == true) {
        $(cwa.page.ct.file).on("show.bs.collapse", function () {
            $(cwa.page.ct.approval).removeClass("btn-success").addClass("btn-danger").find("span").text("Cancelar");
            $(cwa.page.ct.approve).show();
            $(cwa.page.ct.rejection).hide();
            $(cwa.page.ct.back).hide();

        }).on("hide.bs.collapse", function () {
            $(cwa.page.ct.approval).removeClass("btn-danger").addClass("btn-success").find("span").text("Aprobación");
            $(cwa.page.ct.approve).hide();
            $(cwa.page.ct.rejection).show();
            $(cwa.page.ct.back).show();

        });
    }
}

// Reject button
if (cwa.page.showApprove == true) {    
    $(cwa.page.ct.reject).click(function () {
        cwa.dialog.ask(cwa.page.strings.rechazar, "#cwa-reject-form");
    });

    $(cwa.page.ct.text).on("show.bs.collapse", function () {
        $(cwa.page.ct.rejection).find("span").text("Cancelar");
        $(cwa.page.ct.reject).show();
        $(cwa.page.ct.approve).hide();
        $(cwa.page.ct.approval).hide();
        $(cwa.page.ct.back).hide();

    }).on("hide.bs.collapse", function () {
        $(cwa.page.ct.rejection).find("span").text("Rechazo");
        $(cwa.page.ct.reject).hide();        
        $(cwa.page.ct.approval).show();
        $(cwa.page.ct.back).show();

        if (cwa.page.showFile == false) $(cwa.page.ct.approve).show();

    }).on("shown.bs.collapse", function() {
        $("textarea").trigger("focus");
    });
}

cwa.page.complete = function () {
    if (cwa.page.jsonResult.success == true) {
        if (cwa.page.jsonResult.content == "update") {

            // Clear error messages
            $(".cwa-small").html("&nbsp;");
        }

        if (cwa.page.jsonResult.content.status == "approved") {
            cwa.settext("#cwa-approve-date", cwa.page.jsonResult.timeString);
            cwa.settext("#cwa-approved", cwa.page.strings.aprobado);

            if ($("#cwa-approved-icon").hasClass("bi-clock-fill")) {
                $("#cwa-approved-icon").removeClass("bi-clock-fill").removeClass("text-warning");
                $("#cwa-approved-icon").addClass("bi-check-circle-fill").addClass("text-success");
            }

            // Add approval attachment link if required
            if (cwa.page.jsonResult.content.docId) {
                const id = cwa.page.jsonResult.id;
                const docId = cwa.page.jsonResult.content.docId;
                const docName = cwa.page.jsonResult.content.docName;

                if (docId !== "0") {
                    const divCol = "<div class='col-12'><a class='cwa-no-underline' href='/ViabilidadContratos/Regional/" + id + "/Download/" + docId + "'>" + docName + "</a></div>";
                    $("#cwa-regional-docs").append(divCol);
                }
            }

            // clean
            $(cwa.page.ct.back).show();

            $(cwa.page.ct.approval).remove();
            $(cwa.page.ct.rejection).remove();
            $(cwa.page.ct.approve).remove();
            $(cwa.page.ct.reject).remove();
            $(cwa.page.ct.update).remove();
            $(cwa.page.ct.file).remove();
            $(cwa.page.ct.text).remove();
        }

        if (cwa.page.jsonResult.content.status == "rejected") {
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

            $(cwa.page.ct.approval).remove();
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