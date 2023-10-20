using AutoMapper;
using CWA.AccessControl.Services;
using CWA.Application.Services.Bases;
using CWA.Data;
using CWA.Entities.Bases;
using CWA.Entities.Bases.GrandesClientes;
using CWA.Entities.Bases.ViabilidadContratos;
using CWA.Entities.Comun;
using CWA.Entities.ViabilidadContratos;
using CWA.Models.ViabilidadContratos.Edit;
using CWA.Shared.Extensions;
using CWA.Shared.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using DocumentFormat.OpenXml.InkML;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using DocumentFormat.OpenXml.Bibliography;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using CWA.Models.ManejoDeCorrespondencia.Carga;

namespace CWA.Application.Services.ManejoDeCorrespondencia
{
    public partial class MCUploadService : BaseService
    {

        public async Task<PageOperationResult> SaveCorrespondenciaAsync(MCCargaDocumentos ModelData, SharepointAuthenticationObject sharepointAuthenticationObject)
        {
            PageOperationResult result = new() { Success = true, Message = "Datos guardados con éxito" };

            try
            {
                var context = sharepointAuthenticationObject.ClientContext;
                string token = sharepointAuthenticationObject.Token;
                string site = sharepointAuthenticationObject.Site;

                string carpetaContenedora = ModelData.TipoDocumentoId.Equals("Nota") ? "Notas" : ModelData.TipoDocumentoId.Equals("Circular") ? "Circulares" : ModelData.TipoDocumentoId.Equals("Memo") ? "Memos" : "";
                string carpetaAnioMes = DateTime.Now.ToString("yyyy_MM");
                string rutaCargaSharepoint = $"/sites/ManejodeCorrespondencia/Documentos compartidos/Correspondencia CND/{ModelData.CodigoUsuario}/Entrante/{carpetaContenedora}/{carpetaAnioMes}/";

                string siEsDocumentoPrincipal = "True";
                string noEsDocumentoPrincipal = "False";

                await CargarArchivoMetadatosSharepoint(ModelData, ModelData.DocumentoPrincipal, siEsDocumentoPrincipal, context, token, site, rutaCargaSharepoint);

                if (ModelData.Adjunto1 is not null)
                {
                    await CargarArchivoMetadatosSharepoint(ModelData, ModelData.Adjunto1, noEsDocumentoPrincipal, context, token, site, rutaCargaSharepoint);
                }

                if (ModelData.Adjunto2 is not null)
                {
                    await CargarArchivoMetadatosSharepoint(ModelData, ModelData.Adjunto2, noEsDocumentoPrincipal, context, token, site, rutaCargaSharepoint);
                }

                if (ModelData.Adjunto3 is not null)
                {
                    await CargarArchivoMetadatosSharepoint(ModelData, ModelData.Adjunto3, noEsDocumentoPrincipal, context, token, site, rutaCargaSharepoint);
                }

                if (ModelData.Adjunto4 is not null)
                {
                    await CargarArchivoMetadatosSharepoint(ModelData, ModelData.Adjunto4, noEsDocumentoPrincipal, context, token, site, rutaCargaSharepoint);
                }

                if (ModelData.Adjunto5 is not null)
                {
                    await CargarArchivoMetadatosSharepoint(ModelData, ModelData.Adjunto5, noEsDocumentoPrincipal, context, token, site, rutaCargaSharepoint);
                }

                result.Content = "update";
            }
            catch (Exception any)
            {
                result.Success = false;
                result.Message = any.Message;
                _log.LogError($"{any.Message} {any.StackTrace}");
            }

            return result;
        }

        private async Task CargarArchivoMetadatosSharepoint(MCCargaDocumentos ModelData, IFormFile FileData, string esDocumentoPrincipal, ClientContext context, string token, string site, string rutaCargaSharepoint)
        {
            string rutaTemporalArchivo = await SaveFileAsync(FileData, ModelData.TempPath);

            int indiceExtension = FileData.FileName.LastIndexOf('.');
            string nombreArchivo;
            if (indiceExtension == -1)
            {
                nombreArchivo = FileData.FileName + " " + DateTime.Now.ToString("yyyyMMddHHmmsstt") + Guid.NewGuid().GetString(0, 8);
            } else
            {
                nombreArchivo = FileData.FileName.Substring(0, FileData.FileName.LastIndexOf('.')) + " " + DateTime.Now.ToString("yyyyMMddHHmmsstt") + Guid.NewGuid().GetString(0, 8) + FileData.FileName[FileData.FileName.LastIndexOf('.')..];
            }
            await UploadFileToSharePoint(site, token, rutaCargaSharepoint, rutaTemporalArchivo, nombreArchivo);
            string titleDocumento = nombreArchivo;
            await AddMetadataToFile(context, esDocumentoPrincipal, rutaCargaSharepoint, nombreArchivo, titleDocumento, ModelData);
        }

        private async Task<string> SaveFileAsync(IFormFile FileData, string TempPath)
        {
            // Create source
            string tempFile = await FileData.WriteTempAsync(TempPath);

            return tempFile;
        }

        private async Task UploadFileToSharePoint(string site, string token, string ruta, string archivo, string nombreArchivo)
        {
            using var client = new HttpClient();
            using var stream = System.IO.File.OpenRead(archivo);
            var file_content = new ByteArrayContent(new StreamContent(stream).ReadAsByteArrayAsync().Result);
            file_content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsync($"{site}/_api/web/GetFolderByServerRelativeUrl('{ruta}')/Files/Add(url='{nombreArchivo}', overwrite=true)", file_content);
            response.EnsureSuccessStatusCode();
        }

        private async Task AddMetadataToFile(ClientContext context, string esPrincipal, string ruta, string archivo, string title, MCCargaDocumentos ModelData)
        {
            Web web = context.Web;

            Microsoft.SharePoint.Client.File uploadedFile = web.GetFileByServerRelativeUrl(ruta + archivo);
            context.Load(uploadedFile);
            context.ExecuteQuery();

            Web site = context.Web;
            List docList = site.Lists.GetByTitle("Documentos");

            context.Load(docList);
            context.ExecuteQuery();

            context.Load(docList.Fields.GetByTitle("Título"));
            context.Load(docList.Fields.GetByTitle("Número de Nota"));
            context.Load(docList.Fields.GetByTitle("Descripción"));
            context.Load(docList.Fields.GetByTitle("Fecha"));
            context.Load(docList.Fields.GetByTitle("Tema"));
            context.Load(docList.Fields.GetByTitle("Subtema"));
            context.Load(docList.Fields.GetByTitle("Agente"));
            context.Load(docList.Fields.GetByTitle("Documento Principal"));
            context.Load(docList.Fields.GetByTitle("Tipo de Documento"));
            context.Load(docList.Fields.GetByTitle("Dirección Organizacional ETESA"));
            context.Load(docList.Fields.GetByTitle("Sistema"));
            context.Load(docList.Fields.GetByTitle("Nuevo Doc o Respuesta"));
            context.Load(docList.Fields.GetByTitle("Respuesta Nro Nota Saliente CND"));
            await context.ExecuteQueryAsync();

            //*********NEED TO GET THE INTERNAL COLUMN NAMES FROM SHAREPOINT************
            var tituloInternalName = docList.Fields.GetByTitle("Título").InternalName;
            var numeroNotaInternalName = docList.Fields.GetByTitle("Número de Nota").InternalName;
            var descripcionInternalName = docList.Fields.GetByTitle("Descripción").InternalName;
            var fechaInternalName = docList.Fields.GetByTitle("Fecha").InternalName;
            var temaInternalName = docList.Fields.GetByTitle("Tema").InternalName;
            var subtemaInternalName = docList.Fields.GetByTitle("Subtema").InternalName;
            var agenteInternalName = docList.Fields.GetByTitle("Agente").InternalName;
            var documentoPrincipalInternalName = docList.Fields.GetByTitle("Documento Principal").InternalName;
            var tipoDocumentoInternalName = docList.Fields.GetByTitle("Tipo de Documento").InternalName;
            var dirOrgETESAInternalName = docList.Fields.GetByTitle("Dirección Organizacional ETESA").InternalName;
            var sistemaInternalName = docList.Fields.GetByTitle("Sistema").InternalName;
            var nuevoDocRespInternalName = docList.Fields.GetByTitle("Nuevo Doc o Respuesta").InternalName;
            var respNroNotaSalienteInternalName = docList.Fields.GetByTitle("Respuesta Nro Nota Saliente CND").InternalName;

            //**********NOW SAVE THE METADATA TO SHAREPOINT**********
            uploadedFile.ListItemAllFields[tituloInternalName] = title;
            uploadedFile.ListItemAllFields[numeroNotaInternalName] = ModelData.NumeroDeNota;
            uploadedFile.ListItemAllFields[descripcionInternalName] = ModelData.Descripcion;
            uploadedFile.ListItemAllFields[fechaInternalName] = ModelData.Fecha;
            uploadedFile.ListItemAllFields[temaInternalName] = ModelData.Tema;
            uploadedFile.ListItemAllFields[subtemaInternalName] = ModelData.Subtema;
            uploadedFile.ListItemAllFields[agenteInternalName] = ModelData.Agente;
            uploadedFile.ListItemAllFields[documentoPrincipalInternalName] = esPrincipal;
            uploadedFile.ListItemAllFields[tipoDocumentoInternalName] = ModelData.TipoDocumentoId;
            uploadedFile.ListItemAllFields[dirOrgETESAInternalName] = ModelData.DireccionETESA;
            uploadedFile.ListItemAllFields[sistemaInternalName] = ModelData.Sistema;
            uploadedFile.ListItemAllFields[nuevoDocRespInternalName] = ModelData.NuevoDocRespuesta;
            uploadedFile.ListItemAllFields[respNroNotaSalienteInternalName] = ModelData.RespuestaNotaSaliente;

            uploadedFile.ListItemAllFields.Update();
            context.Load(uploadedFile);
            await context.ExecuteQueryAsync();
        }

    }
}