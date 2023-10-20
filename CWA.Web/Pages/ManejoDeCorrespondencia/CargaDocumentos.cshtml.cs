using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CWA.Application.Services.Comun;
using CWA.Models.Comun;
using CWA.Shared.Helpers;
using CWA.Web.Extensions;
using CWA.Web.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using CWA.Application.Services.ManejoDeCorrespondencia;
using Microsoft.Extensions.Configuration;
using CWA.Models.ManejoDeCorrespondencia.Carga;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CWA.Web.Pages.ManejoDeCorrespondencia
{

    public class CargaDocumentosModel: PageModel
    {
        //Configuration
        private readonly IConfiguration _config;
        private readonly IOptions<ApplicationSettings> _options;

        // Services
        private readonly MCUploadService _mcUploadService;
        private readonly AgenteService _agente;

        [BindProperty]
        public MCCargaDocumentos MCCargaDocumentosModel { get; set; }

        // Lists
        public AgenteListing Usuario = new();
        public List<string> NuevoDocRespuesta = new();
        public List<string> TiposDocumento = new();
        public List<MCElementoLista> DireccionesETESA = new();
        public List<MCElementoLista> Temas = new();
        public List<MCSubtema> Subtemas = new();
        public IEnumerable<MCNotaSaliente> RespuestaNotasSalientes;
        public List<MCAgente> MCAgentes = new();

        //Presentation
        public bool ShowUpdate = true;
        public string TipoAgente;

        public CargaDocumentosModel(MCUploadService MCUploadService, AgenteService Agente, IConfiguration Config, IOptions<ApplicationSettings> Options)
        {
            _mcUploadService = MCUploadService;
            _agente = Agente;
            _config = Config;
            _options = Options;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadLists();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            PageOperationResult result = new();

            Usuario = await _agente.GetByUserAsync();
            MCCargaDocumentosModel.CodigoUsuario = Usuario.Codigo;
            var sharepointAuthenticationObject = MCServiceHelpers.GetSharepointAuthenticationObject(_config);
            List<MCAgente> agentes = await _mcUploadService.GetMCAgentes(sharepointAuthenticationObject.ClientContext);
            MCCargaDocumentosModel.Agente = agentes.Where(w => w.Codigo == MCCargaDocumentosModel.CodigoUsuario).Select(s => s.Id).First();
            MCCargaDocumentosModel.TipoAgente = agentes.Where(w => w.Codigo == MCCargaDocumentosModel.CodigoUsuario).Select(s => s.TipoAgente).First();
            if (!MCCargaDocumentosModel.TipoAgente.Equals("ETESA")) MCCargaDocumentosModel.TipoDocumentoId = "Nota";

            bool documentoPrincipalMaxLengthError = false;
            bool adjunto1MaxLengthError = false;
            bool adjunto2MaxLengthError = false;
            bool adjunto3MaxLengthError = false;
            bool adjunto4MaxLengthError = false;
            bool adjunto5MaxLengthError = false;


            if (!ModelState.IsValid)
            {
                List<object> modelErrors = (List<object>)ModelState.GetErrorMessages();

                // Also check file extensions for submitted files
                if (MCCargaDocumentosModel.DocumentoPrincipal is not null)
                {
                    if (!MCCargaDocumentosModel.DocumentoPrincipal.FileName.ToLower().EndsWith(".pdf"))
                    {
                        modelErrors.Add(new
                        {
                            Key = $"MCCargaDocumentosModel-DocumentoPrincipal",
                            Message = _options.Value.ManejoDeCorrespondencia.Strings.ExtensionInvalidaDocumentoPrincipal
                        });
                    }

                    else if (MCCargaDocumentosModel.DocumentoPrincipal.FileName.Length > 50)
                    {
                        modelErrors.Add(new
                        {
                            Key = $"MCCargaDocumentosModel-DocumentoPrincipal",
                            Message = _options.Value.ManejoDeCorrespondencia.Strings.LongitudMaximaNombreArchivo
                        });
                        documentoPrincipalMaxLengthError = true;
                    }
                }

                if (MCCargaDocumentosModel.Adjunto1 is not null)
                {
                    if (MCCargaDocumentosModel.Adjunto1.FileName.Length > 50)
                    {
                        modelErrors.Add(new
                        {
                            Key = $"MCCargaDocumentosModel-Adjunto1",
                            Message = _options.Value.ManejoDeCorrespondencia.Strings.LongitudMaximaNombreArchivo
                        });
                        adjunto1MaxLengthError = true;
                    }

                }

                if (MCCargaDocumentosModel.Adjunto2 is not null)
                {
                    if (MCCargaDocumentosModel.Adjunto2.FileName.Length > 50)
                    {
                        modelErrors.Add(new
                        {
                            Key = $"MCCargaDocumentosModel-Adjunto2",
                            Message = _options.Value.ManejoDeCorrespondencia.Strings.LongitudMaximaNombreArchivo
                        });
                        adjunto2MaxLengthError = true;
                    }

                }

                if (MCCargaDocumentosModel.Adjunto3 is not null)
                {
                    if (MCCargaDocumentosModel.Adjunto3.FileName.Length > 50)
                    {
                        modelErrors.Add(new
                        {
                            Key = $"MCCargaDocumentosModel-Adjunto3",
                            Message = _options.Value.ManejoDeCorrespondencia.Strings.LongitudMaximaNombreArchivo
                        });
                        adjunto3MaxLengthError = true;
                    }

                }

                if (MCCargaDocumentosModel.Adjunto4 is not null)
                {
                    if (MCCargaDocumentosModel.Adjunto4.FileName.Length > 50)
                    {
                        modelErrors.Add(new
                        {
                            Key = $"MCCargaDocumentosModel-Adjunto4",
                            Message = _options.Value.ManejoDeCorrespondencia.Strings.LongitudMaximaNombreArchivo
                        });
                        adjunto4MaxLengthError = true;
                    }

                }

                if (MCCargaDocumentosModel.Adjunto5 is not null)
                {
                    if (MCCargaDocumentosModel.Adjunto5.FileName.Length > 50)
                    {
                        modelErrors.Add(new
                        {
                            Key = $"MCCargaDocumentosModel-Adjunto5",
                            Message = _options.Value.ManejoDeCorrespondencia.Strings.LongitudMaximaNombreArchivo
                        });
                        adjunto5MaxLengthError = true;
                    }

                }


                // Validar también campos obligatorios cuando el agente es de tipo ETESA
                if (MCCargaDocumentosModel.TipoAgente.Equals("ETESA"))
                {
                    if (MCCargaDocumentosModel.TipoDocumentoId is null)
                    {
                        modelErrors.Add(new
                        {
                            Key = $"MCCargaDocumentosModel-TipoDocumentoId",
                            Message = _options.Value.ManejoDeCorrespondencia.Strings.TipoDocumentoRequerido
                        });
                    }

                    if (MCCargaDocumentosModel.DireccionETESA is null)
                    {
                        modelErrors.Add(new
                        {
                            Key = $"MCCargaDocumentosModel-DireccionETESA",
                            Message = _options.Value.ManejoDeCorrespondencia.Strings.DireccionETESARequerida
                        });
                    }
                }

                // Validar que si la correspondencia es una Respuesta, se debe especificar la nota a la que se está respondiendo
                if (MCCargaDocumentosModel.RespuestaNotaSaliente is null && MCCargaDocumentosModel.NuevoDocRespuesta is not null && MCCargaDocumentosModel.NuevoDocRespuesta.Equals("Respuesta"))
                {
                    modelErrors.Add(new
                    {
                        Key = $"MCCargaDocumentosModel-RespuestaNotaSaliente",
                        Message = _options.Value.ManejoDeCorrespondencia.Strings.NotaSalienteRequerida
                    });
                }

                result.Success = false;
                result.Message = _options.Value.Strings.DatosInvalidos;
                if (documentoPrincipalMaxLengthError || adjunto1MaxLengthError
                    || adjunto2MaxLengthError || adjunto3MaxLengthError
                    || adjunto4MaxLengthError || adjunto5MaxLengthError)
                {
                    result.Message += " .................. La longitud del nombre del archivo principal y sus adjuntos debe ser menor a 50 caracteres.";
                }
                result.Content = modelErrors;

                return new JsonResult(result);
            }

            // Validate file extensions
            var extensionErrors = new List<object>();

            if (MCCargaDocumentosModel.DocumentoPrincipal is not null)
            {
                if (!MCCargaDocumentosModel.DocumentoPrincipal.FileName.ToLower().EndsWith(".pdf"))
                {
                    extensionErrors.Add(new
                    {
                        Key = $"MCCargaDocumentosModel-DocumentoPrincipal",
                        Message = _options.Value.ManejoDeCorrespondencia.Strings.ExtensionInvalidaDocumentoPrincipal
                    });
                }

                else if (MCCargaDocumentosModel.DocumentoPrincipal.FileName.Length > 50)
                {
                    extensionErrors.Add(new
                    {
                        Key = $"MCCargaDocumentosModel-DocumentoPrincipal",
                        Message = _options.Value.ManejoDeCorrespondencia.Strings.LongitudMaximaNombreArchivo
                    });
                    documentoPrincipalMaxLengthError = true;
                }

            }

            if (MCCargaDocumentosModel.Adjunto1 is not null)
            {
                if (MCCargaDocumentosModel.Adjunto1.FileName.Length > 50)
                {
                    extensionErrors.Add(new
                    {
                        Key = $"MCCargaDocumentosModel-Adjunto1",
                        Message = _options.Value.ManejoDeCorrespondencia.Strings.LongitudMaximaNombreArchivo
                    });
                    adjunto1MaxLengthError = true;
                }

            }

            if (MCCargaDocumentosModel.Adjunto2 is not null)
            {
                if (MCCargaDocumentosModel.Adjunto2.FileName.Length > 50)
                {
                    extensionErrors.Add(new
                    {
                        Key = $"MCCargaDocumentosModel-Adjunto2",
                        Message = _options.Value.ManejoDeCorrespondencia.Strings.LongitudMaximaNombreArchivo
                    });
                    adjunto2MaxLengthError = true;
                }

            }

            if (MCCargaDocumentosModel.Adjunto3 is not null)
            {
                if (MCCargaDocumentosModel.Adjunto3.FileName.Length > 50)
                {
                    extensionErrors.Add(new
                    {
                        Key = $"MCCargaDocumentosModel-Adjunto3",
                        Message = _options.Value.ManejoDeCorrespondencia.Strings.LongitudMaximaNombreArchivo
                    });
                    adjunto3MaxLengthError = true;
                }

            }

            if (MCCargaDocumentosModel.Adjunto4 is not null)
            {
                if (MCCargaDocumentosModel.Adjunto4.FileName.Length > 50)
                {
                    extensionErrors.Add(new
                    {
                        Key = $"MCCargaDocumentosModel-Adjunto4",
                        Message = _options.Value.ManejoDeCorrespondencia.Strings.LongitudMaximaNombreArchivo
                    });
                    adjunto4MaxLengthError = true;
                }

            }

            if (MCCargaDocumentosModel.Adjunto5 is not null)
            {
                if (MCCargaDocumentosModel.Adjunto5.FileName.Length > 50)
                {
                    extensionErrors.Add(new
                    {
                        Key = $"MCCargaDocumentosModel-Adjunto5",
                        Message = _options.Value.ManejoDeCorrespondencia.Strings.LongitudMaximaNombreArchivo
                    });
                    adjunto5MaxLengthError = true;
                }

            }

            // Validar también campos obligatorios cuando el agente es de tipo ETESA
            if (MCCargaDocumentosModel.TipoAgente.Equals("ETESA"))
            {
                if (MCCargaDocumentosModel.TipoDocumentoId is null)
                {
                    extensionErrors.Add(new
                    {
                        Key = $"MCCargaDocumentosModel-TipoDocumentoId",
                        Message = _options.Value.ManejoDeCorrespondencia.Strings.TipoDocumentoRequerido
                    });
                }

                if (MCCargaDocumentosModel.DireccionETESA is null)
                {
                    extensionErrors.Add(new
                    {
                        Key = $"MCCargaDocumentosModel-DireccionETESA",
                        Message = _options.Value.ManejoDeCorrespondencia.Strings.DireccionETESARequerida
                    });
                }
            }

            // Validar que si la correspondencia es una Respuesta, se debe especificar la nota a la que se está respondiendo
            if (MCCargaDocumentosModel.NuevoDocRespuesta.Equals("Respuesta") && MCCargaDocumentosModel.RespuestaNotaSaliente is null)
            {
                extensionErrors.Add(new
                {
                    Key = $"MCCargaDocumentosModel-RespuestaNotaSaliente",
                    Message = _options.Value.ManejoDeCorrespondencia.Strings.NotaSalienteRequerida
                });
            }

            if (extensionErrors.Count > 0)
            {
                result.Success = false;
                result.Message = _options.Value.Strings.DatosInvalidos;
                if (documentoPrincipalMaxLengthError || adjunto1MaxLengthError
                    || adjunto2MaxLengthError || adjunto3MaxLengthError
                    || adjunto4MaxLengthError || adjunto5MaxLengthError)
                {
                    result.Message += " .................. La longitud del nombre del archivo principal y sus adjuntos debe ser menor a 50 caracteres.";
                }
                result.Content = extensionErrors;

                return new JsonResult(result);
            }

            // Proceed
            MCCargaDocumentosModel.Fecha = DateTime.Now.ToString("yyyy-MM-dd");
            MCCargaDocumentosModel.Sistema = "Portal";
            MCCargaDocumentosModel.TempPath = _options.Value.ManejoDeCorrespondencia.DocumentsTemp;
            result = await _mcUploadService.SaveCorrespondenciaAsync(MCCargaDocumentosModel, sharepointAuthenticationObject);

            return new JsonResult(result);
        }

        private async Task LoadLists()
        {
            var sharepointAuthenticationObject = MCServiceHelpers.GetSharepointAuthenticationObject(_config);

            Usuario = await _agente.GetByUserAsync();
            string codigoUsuario = Usuario.Codigo;
            List<MCAgente> agentes = await _mcUploadService.GetMCAgentes(sharepointAuthenticationObject.ClientContext);            
            string nombreAgente = agentes.Where(w => w.Codigo == codigoUsuario).Select(s => s.NombreAgente).First();

            NuevoDocRespuesta = _mcUploadService.GetNuevoDocRespuesta();
            RespuestaNotasSalientes = (await _mcUploadService.GetNotasSalientes(sharepointAuthenticationObject.ClientContext)).Where(s => s.Destinatarios.Contains(nombreAgente) || s.Destinatarios.Contains("*TODOS*"));            
            Temas = await _mcUploadService.GetListaValoresConId(sharepointAuthenticationObject.ClientContext, "Temas");
            Subtemas = await _mcUploadService.GetSubtemas(sharepointAuthenticationObject.ClientContext, Temas);            
            TipoAgente = agentes.Where(w => w.Codigo == codigoUsuario).Select(s => s.TipoAgente).First();
            if (TipoAgente.Equals("ETESA"))
            {
                TiposDocumento = await _mcUploadService.GetListaValores(sharepointAuthenticationObject.ClientContext, "Tipos de Documento");
                DireccionesETESA = await _mcUploadService.GetListaValoresConId(sharepointAuthenticationObject.ClientContext, "Direcciones ETESA");
            }
        }
    }
}
