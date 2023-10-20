using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CWA.Application.Services.Bases;
using AutoMapper;
using CWA.AccessControl.Services;
using CWA.Data;
using Microsoft.Extensions.Logging;
using CWA.Shared.Helpers;
using CWA.Models.ManejoDeCorrespondencia.Descarga;
using System.Web;
using Microsoft.Extensions.Configuration;
using CWA.Shared.Extensions;
using CWA.Models.ManejoDeCorrespondencia.Carga;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Newtonsoft.Json;
//using DocumentFormat.OpenXml.Wordprocessing;

namespace CWA.Application.Services.ManejoDeCorrespondencia
{
    public class MCDownloadService : BaseService
    {
        public MCDownloadService(DataContext Data, AppAccessControlService Access, IMapper Mapper, ILogger<BaseService> Logger) : base(Data, Access, Mapper, Logger) { }

        private async Task<List<string>> GetAllFolders(string startFolder, string token, string siteUrl, string tipoFolder)
        {
            var foldersList = new List<string>();

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                client.DefaultRequestHeaders.Add("Accept", $"application/json;odata=verbose");

                HttpResponseMessage response = await client.GetAsync($"{siteUrl}/_api/web/GetFolderByServerRelativeUrl('/sites/ManejodeCorrespondencia{startFolder}{tipoFolder}')/Folders");

                if (response.IsSuccessStatusCode)
                {
                    System.Net.Http.HttpContent content = response.Content;
                    var contentStream = await content.ReadAsStringAsync();

                    var data = JsonConvert.DeserializeObject<MCRaizResultadoRestArchivos>(contentStream);

                    foreach (var item in data.Result.ArchivosCollection)
                    {
                        foldersList.Add(item.Name);
                    }
                }
                else
                {
                    throw new FileNotFoundException(response.StatusCode.ToString());
                }

            }
            catch (Exception any) 
            {
                _log.LogError($"{any.Message} {any.StackTrace}");
            }
            return foldersList;
        }

        private async Task<string> GetNumeroDeNota(string token, string siteUrl, string ubicacion, string fileName)
        {
            var numeroDeNota = "";

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                client.DefaultRequestHeaders.Add("Accept", $"application/json;odata=verbose");

                HttpResponseMessage response = await client.GetAsync($"{siteUrl}/_api/web/GetFolderByServerRelativeUrl('{ubicacion}')/Files('{fileName}')/Properties");

                if (response.IsSuccessStatusCode)
                {
                    System.Net.Http.HttpContent content = response.Content;
                    var contentStream = await content.ReadAsStringAsync();

                    var data = JsonConvert.DeserializeObject<MCRaizResultadoRestPropiedadesArchivo>(contentStream);

                    numeroDeNota = data.Result.NumeroDeNota;
                }
                else
                {
                    throw new FileNotFoundException(response.StatusCode.ToString());
                }

            }
            catch (Exception any)
            {
                _log.LogError($"{any.Message} {any.StackTrace}");
            }
            return numeroDeNota;

        }

        private async Task<List<MCArchivoDescarga>> GetAllFilesUnderTipoDocumento(string token, string siteUrl, string tipoDocumento, string startFolder)
        {
            var fileList = new List<MCArchivoDescarga>();

            try
            {
                var foldersList = await GetAllFolders(startFolder, token, siteUrl, tipoDocumento);

                foreach (string folder in foldersList)
                {
                    using var client = new HttpClient();

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}"); 
                    client.DefaultRequestHeaders.Add("Accept", $"application/json;odata=verbose");
                    HttpResponseMessage response = await client.GetAsync($"{siteUrl}/_api/web/GetFolderByServerRelativeUrl('/sites/ManejodeCorrespondencia{startFolder}{tipoDocumento}/{folder}')/Files");

                    if (response.IsSuccessStatusCode)
                    {
                        System.Net.Http.HttpContent content = response.Content;
                        var contentStream = await content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<MCRaizResultadoRestArchivos>(contentStream);

                        foreach (var item in data.Result.ArchivosCollection)
                        {
                            string ubicacion = $"/sites/ManejodeCorrespondencia{startFolder}{tipoDocumento}/{folder}";
                            string nombreCompleto = ubicacion + "/" + item.Name;
                            string nombre = item.Name;

                            fileList.Add(new MCArchivoDescarga
                            {
                                Id = nombreCompleto,
                                Nombre = nombre,
                                NumeroDeNota = await GetNumeroDeNota(token, siteUrl, ubicacion, nombre),
                                Ubicacion = ubicacion,
                                Fecha = DateTime.Parse(item.TimeCreated).ToLocalTime().ToString("yyyy/MM/dd H:mm:ss")
                            });

                        }
                    }
                    else
                    {
                        throw new FileNotFoundException(response.StatusCode.ToString());
                    }
                }
            }
            catch (Exception any)
            {
                _log.LogError($"{any.Message} {any.StackTrace}");
            }

            return fileList;
        }

        public async Task<List<MCArchivoDescarga>> GetAllFilesUnderFolder(string startFolder, SharepointAuthenticationObject sharepointAuthenticationObject, string codigoUsuario)
        {
            var fileList = new List<MCArchivoDescarga>();

            try
            {
                string token = sharepointAuthenticationObject.Token;
                string siteUrl = sharepointAuthenticationObject.Site;

                fileList.AddRange(await GetAllFilesUnderTipoDocumento(token, siteUrl, "Notas", startFolder));

                if (codigoUsuario.Equals("ETESA"))
                {
                    fileList.AddRange(await GetAllFilesUnderTipoDocumento(token, siteUrl, "Circulares", startFolder));
                    fileList.AddRange(await GetAllFilesUnderTipoDocumento(token, siteUrl, "Memos", startFolder));
                }
            }
            catch (Exception any)
            {
                _log.LogError($"{any.Message} {any.StackTrace}");
            }
            return fileList;
        }

        public async Task<MCDocumentoDownload> DownloadFileFromSharePoint(SharepointAuthenticationObject sharepointAuthenticationObject, string spFileName, string spFilePath, string localTempPath, string codigoUsuario)
        {
            MCDocumentoDownload result = new() { FileName = "", FilePath = "" };
            try
            {
                using var client = new HttpClient();

                string token = sharepointAuthenticationObject.Token;
                string siteUrl = sharepointAuthenticationObject.Site;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}"); //if any
                //client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/pdf"));
                HttpResponseMessage response = await client.GetAsync($"{siteUrl}/_api/web/GetFolderByServerRelativeUrl('{spFilePath}')/Files('{spFileName}')/$value");

                if (response.IsSuccessStatusCode)
                {
                    System.Net.Http.HttpContent content = response.Content;
                    using var contentStream = await content.ReadAsStreamAsync();
                    string safeName = codigoUsuario + IFormFileExtensions.SafeFileName(spFileName);
                    using var fs = new FileStream(@$"{localTempPath}\{safeName}", FileMode.CreateNew);
                    await contentStream.CopyToAsync(fs);
                    // get the actual content stream
                    result.FileName = spFileName;
                    result.FilePath = $"{localTempPath}/{safeName}";

                }
                else
                {
                    throw new FileNotFoundException();
                }

            }
            catch (Exception any)
            {

                _log.LogError($"{any.Message} {any.StackTrace}");
            }
            return result;
        }
    }
}
