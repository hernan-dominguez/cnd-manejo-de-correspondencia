// Manage comprador/vendedor
$("#cwa-tipo-select").change(function () {
    const tipoid = $(this).val();
    const defaultItem = '<option value="">Seleccione un valor...</option>';

    // Reset inputs
    $("#cwa-vendedor-select").empty().append(defaultItem);
    $("#cwa-comprador-select").empty().append(defaultItem);
    $(".cwa-doc-row").remove();

    if (tipoid) {
        $(cwa.page.tipos).each(function () {
            const spec = this;

            if (tipoid == spec.id) {
                // Add related                
                if (spec.refVal4 == cwa.page.targets.dist) {
                    // Vendedor
                    if (cwa.page.usuario.gen.id != 0) {
                        $("#cwa-vendedor-select").empty().append('<option value="' + cwa.page.usuario.gen.id + '">' + cwa.page.usuario.gen.nombre + '</option>');
                    }
                    else {
                        $(cwa.page.agentes.generadores).each(function () {
                            $("#cwa-vendedor-select").append('<option value="' + this.value + '">' + this.text + '</option>');
                        });
                    }

                    // Comprador
                    if (cwa.page.usuario.dist.id != 0) {
                        $("#cwa-comprador-select").empty().append('<option value="' + cwa.page.usuario.dist.id + '">' + cwa.page.usuario.dist.nombre + '</option>');
                    }
                    else {
                        $(cwa.page.agentes.distribuidoras).each(function () {
                            $("#cwa-comprador-select").append('<option value="' + this.value + '">' + this.text + '</option>');
                        });
                    }
                }

                if (spec.refVal4 == cwa.page.targets.gen) {
                    if (cwa.page.usuario.gen.id != 0) {
                        // Own
                        $("#cwa-vendedor-select").append('<option value="' + cwa.page.usuario.gen.id + '">' + cwa.page.usuario.gen.nombre + '</option>');
                        $("#cwa-comprador-select").append('<option value="' + cwa.page.usuario.gen.id + '">' + cwa.page.usuario.gen.nombre + '</option>');
                    }

                    // Vendedor
                    $(cwa.page.agentes.generadores).each(function () {
                        $("#cwa-vendedor-select").append('<option value="' + this.value + '">' + this.text + '</option>');
                    });

                    // Comprador
                    $(cwa.page.agentes.generadores).each(function () {
                        $("#cwa-comprador-select").append('<option value="' + this.value + '">' + this.text + '</option>');
                    });
                }

                //if (spec.refVal4 == cwa.page.targets.gc) {
                //    // Vendedor
                //    if (cwa.page.usuario.gen.id != 0) {
                //        $("#cwa-vendedor-select").empty().append('<option value="' + cwa.page.usuario.gen.id + '">' + cwa.page.usuario.gen.nombre + '</option>');
                //    }
                //    else {
                //        $(cwa.page.agentes.generadores).each(function () {
                //            $("#cwa-vendedor-select").append('<option value="' + this.value + '">' + this.text + '</option>');
                //        });
                //    }

                //    $(cwa.page.agentes.gclientes).each(function () {
                //        $("#cwa-comprador-select").append('<option value="' + this.value + '">' + this.text + '</option>');
                //    });
                //}

                // Documentos
                var docIndex = 0;

                $(cwa.page.docs.doctipos).each(function () {
                    const lookup = this;

                    if (spec.id == lookup.contrato) {

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
        });
    }
});

// Page specific functions
cwa.page.functions = {
    selectChanged: function (select, partner) {
        const tipoid = $("#cwa-tipo-select").val();
        const defaultItem = '<option value="">Seleccione un valor...</option>';

        if (tipoid) {
            $(cwa.page.tipos).each(function () {
                const selectedid = $("#" + select).val();
                const partnerid = $("#" + partner).val();
                const spec = this;

                if (tipoid == spec.id && spec.refVal4 == cwa.page.targets.gen) {

                    if (cwa.page.usuario.gen.id != 0) {
                        // Own
                        if (selectedid == cwa.page.usuario.gen.id) {
                            if (partnerid == cwa.page.usuario.gen.id) $("#" + partner).val('');
                        }
                        else {
                            // Other
                            if (selectedid != '') $("#" + partner).val(cwa.page.usuario.gen.id);
                        }
                    }
                }

                //if (tipoid == spec.id && spec.refVal4 == cwa.page.targets.gc) {
                //    if (cwa.page.usuario.gen.id == 0 && cwa.page.usuario.dist.id == 0 && select == "cwa-vendedor-select") {
                //        // Get children
                //        $("#" + partner).empty().append(defaultItem);

                //        $(cwa.page.agentes.responsables).each(function () {
                //            if (selectedid == this.rid) $("#" + partner).append('<option value="' + this.gid + '">' + this.gc + '</option>');
                //        });
                //    }
                //}
            });
        }
    },

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
};

// Function association
$("#cwa-vendedor-select").change(function () {
    cwa.page.functions.selectChanged("cwa-vendedor-select", "cwa-comprador-select");
});

$("#cwa-comprador-select").change(function () {
    cwa.page.functions.selectChanged("cwa-comprador-select", "cwa-vendedor-select");
});

// Page controls
cwa.page.ct = {
    update: $("#cwa-update-button"),
    approve: $("#cwa-approve-button"),
    reject: $("#cwa-reject-button"),
    rejection: $("#cwa-rejection-button"),
    back: $("#cwa-back-link"),
    text: $("#cwa-motivo-rechazo")
};

// Update button
if (cwa.page.showUpdate) $(cwa.page.ct.update).click(function () {
    cwa.dialog.ask(cwa.page.strings.guardar, "#cwa-update-form", function () {
        if (cwa.page.jsonResult.success == true) document.querySelector("#cwa-back-link").click();
    });
});

// Approve button
if (cwa.page.showApprove == true) $("#cwa-approve-button").click(function () {
    cwa.dialog.ask(cwa.page.strings.aprobar, "#cwa-approve-form");
});

// Reject button
if (cwa.page.showApprove == true) {
    $("#cwa-reject-button").click(function () {
        cwa.dialog.ask(cwa.page.strings.rechazar, "#cwa-reject-form");
    });

    $(cwa.page.ct.text).on("show.bs.collapse", function () {
        $(cwa.page.ct.rejection).find("span").text("Cancelar");
        $(cwa.page.ct.reject).show();
        $(cwa.page.ct.approve).hide();
        $(cwa.page.ct.back).hide();

    }).on("hide.bs.collapse", function () {
        $(cwa.page.ct.rejection).find("span").text("Rechazo");
        $(cwa.page.ct.reject).hide();
        $(cwa.page.ct.approve).show();
        $(cwa.page.ct.back).show();

    }).on("shown.bs.collapse", function () {
        $("textarea").trigger("focus");
    });
}

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