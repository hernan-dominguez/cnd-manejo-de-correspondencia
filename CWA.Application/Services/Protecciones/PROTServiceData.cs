using AutoMapper;
using ClosedXML.Excel;
using CWA.AccessControl.Constants;
using CWA.AccessControl.Services;
using CWA.Application.Services.Bases;
using CWA.Data;
using CWA.Entities.Bases;
using CWA.Entities.Bases.Protecciones;
using CWA.Entities.Comun;
using CWA.Entities.Protecciones;
using CWA.Models.Protecciones.Edit;
using CWA.Models.Protecciones.View;
using CWA.Shared.Extensions;
using CWA.Shared.Helpers;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Application.Services.Protecciones
{
    public partial class PROTService
    {
        public async Task<TModel> GetDatosAsync<K, TEntity, TModel>(K Id) where K : IEquatable<K> where TEntity : class, IIdentidad<K> where TModel : class, new()
        {
            var query = await _data.Set<TEntity>().Where(w => w.Id.Equals(Id)).FirstOrDefaultAsync();

            return (query is null) ? new TModel() : _mapper.Map<TModel>(query);
        }

        public async Task<PageOperationResult> SaveRegistroAsync(PROTRegistroEdit ModelData, string NotificacionId, int? RegistroId = null)
        {
            PageOperationResult result = new() { Success = true, Message = "Datos guardados con éxito" };
            var registro = new PROTRegistro();

            try
            {
                if (RegistroId.HasValue) registro = await _data.PROTRegistros.Where(w => w.Id == RegistroId.Value).FirstOrDefaultAsync();

                registro = _mapper.Map(ModelData, registro);

                if (!RegistroId.HasValue)
                {
                    registro.EstatusId = PROTConstants.INICIAL;
                    registro.RegUsuarioId = _access.SessionUserId;
                    registro.ModUsuarioId = _access.SessionUserId;
                    registro.Generales = new PROTGenerales { RegUsuarioId = _access.SessionUserId, ModUsuarioId = _access.SessionUserId };

                    var documentSpecs = await _data.Catalogos.Where(w => w.Grupo == "PROTDOCUMENTO" && w.RefVal4 != "NO"/* && w.RefVal5.Contains(tipoprot)*/).AsNoTracking().ToListAsync();
                    registro.Documentos = new List<PROTDocumento>();

                    foreach (var spec in documentSpecs)
                    {
                        registro.Documentos.Add(new PROTDocumento
                        {
                            Loaded = false,
                            Archivo = $"{spec.RefVal2}_{Guid.NewGuid().GetString(0, 10, true)}",
                            Mostrar = $"{spec.Descripcion}",
                            TipoDocumentoId = spec.Id,
                            RegUsuarioId = _access.SessionUserId,
                            ModUsuarioId = _access.SessionUserId
                        });
                    }
                }
                else
                {
                    registro.ModUsuarioId = _access.SessionUserId;
                    registro.ModFecha = DateTime.Now;
                }

                _data.Update(registro);

                // Add notification
                var recipients = await _data.PROTDestinatarios.Where(w => w.NotificacionId == NotificacionId).Include(i => i.Usuario).AsNoTracking().ToListAsync();
                var specs = await _data.Catalogos.Where(w => w.Id == NotificacionId).AsNoTracking().FirstOrDefaultAsync();

                // Internal
                foreach (var target in recipients)
                {
                    _data.CorreoMensajes.Add(new CorreoMensaje
                    {
                        AppId = "PRPT",
                        Estatus = 0,
                        Destinatario = target.Usuario.Email,
                        Contenido = specs.RefVal1.Replace("{:n}", registro.RazonSocial),
                        RegUsuarioId = _access.SessionUserId,
                        ModUsuarioId = _access.SessionUserId
                    });
                }

                // Contact
                _data.CorreoMensajes.Add(new CorreoMensaje
                {
                    AppId = "PRPT",
                    Estatus = 0,
                    Destinatario = registro.ContactoCorreo,
                    Contenido = specs.RefVal1.Replace("{:n}", registro.RazonSocial),
                    RegUsuarioId = _access.SessionUserId,
                    ModUsuarioId = _access.SessionUserId
                });

                await _data.SaveChangesAsync();
            }
            catch (Exception any)
            {
                result.Success = false;
                result.Message = any.Message;
            }

            return result;
        }

        public async Task<PageOperationResult> SaveDatosAsync<K, TModel, TEntity>(K Id, TModel ModelData, string NotificacionId, string Nombre, int RegistroId)
            where K : IEquatable<K> where TEntity : class, IIdentidad<K>, IAuditoria where TModel : class
        {
            PageOperationResult result = new() { Success = true, Message = "Datos guardados con éxito" };
            DateTime fixedStamp = DateTime.Now;

            try
            {
                var query = await _data.Set<TEntity>().Where(w => w.Id.Equals(Id)).FirstOrDefaultAsync();

                query = _mapper.Map(ModelData, query);
                query.ModUsuarioId = _access.SessionUserId;
                query.ModFecha = fixedStamp;

                _data.Update(query);

                // Add notifications
                var itemkey = $"{RegistroId:000000}:{NotificacionId}";

                if (!(await _data.CorreoMensajes.Where(w => w.AppId == "PRPT" && w.ItemKey == itemkey && w.Estatus == 0).AnyAsync()))
                {
                    var recipients = await _data.PROTDestinatarios.Where(w => w.NotificacionId == NotificacionId).Include(i => i.Usuario).AsNoTracking().ToListAsync();
                    var specs = await _data.Catalogos.Where(w => w.Id == NotificacionId).AsNoTracking().FirstOrDefaultAsync();

                    // Internal
                    foreach (var target in recipients)
                    {
                        _data.CorreoMensajes.Add(new CorreoMensaje
                        {
                            AppId = "PRTP",
                            Estatus = 0,
                            ItemKey = itemkey,
                            Destinatario = target.Usuario.Email,
                            Contenido = specs.RefVal1.Replace("{:n}", Nombre),
                            RegUsuarioId = _access.SessionUserId,
                            ModUsuarioId = _access.SessionUserId
                        });
                    }
                }

                await _data.SaveChangesAsync();

                result.TimeValue = fixedStamp;
                result.Content = "update";
            }
            catch (Exception any)
            {
                result.Success = false;
                result.Message = any.Message;
            }

            return result;
        }

        public async Task<PageOperationResult> SaveDocumentoAsync(string pathUsuario, PROTDocumentoEdit ModelData, string RootFolder, string TempFolder)
        {
            PageOperationResult result = new() { Success = true, Message = "Datos guardados con éxito" };
            DateTime fixedStamp = DateTime.Now;

            try
            {
                var documento = await _data.PROTDocumentos.Where(w => w.Id == ModelData.DocumentoId).Include(i => i.TipoDocumento).FirstOrDefaultAsync();

                // File
                string fileName = documento.Loaded ? documento.Archivo : $"{documento.Archivo}{ModelData.DocumentUpload.GetExtension()}";
                string archivo = await SaveFileAsync(pathUsuario, ModelData.DocumentUpload, RootFolder, TempFolder, fileName.ToUpper());

                // Data
                documento.Archivo = fileName;
                documento.Loaded = true;
                documento.ModFecha = fixedStamp;
                documento.ModUsuarioId = _access.SessionUserId;
                documento.FechaAprobacion = (DateTime?)null;
                documento.UsuarioAprueba = null;

                // Check if file is auto-accepted
                if (documento.TipoDocumento.RefVal4 == "AUTO")
                {
                    documento.UsuarioApruebaId = 1;
                    documento.FechaAprobacion = fixedStamp;
                }

                documento.Bitacora = new PROTBitacoraDocumentos
                { 
                    RegistroId = documento.RegistroId,
                    Archivo = documento.Archivo,
                    TipoDocumentoId = documento.TipoDocumentoId,
                    RegFecha = fixedStamp
                };

                _data.Update(documento);
                await _data.SaveChangesAsync();

                result.Content = new PROTDocumentoResult
                {
                    Id = $"{documento.Id}",
                    Descripcion = documento.Mostrar,
                    AutoAccepted = (documento.TipoDocumento.RefVal4 == "AUTO"),
                    SaveTime = fixedStamp
                };
            }
            catch (Exception any)
            {
                result.Success = false;
                result.Message = any.Message;
            }

            return result;
        }

        public static async Task<string> SaveFileAsync(string pathUsuario, IFormFile FileData, string RootPath, string TempPath, string FileName)
        {
            // Write physical temp file            
            Directory.CreateDirectory(Path.Combine(RootPath, pathUsuario));

            string targetFile = Path.Combine(RootPath, pathUsuario, FileName);
            string tempFile = await FileData.WriteTempAsync(TempPath);

            // Copy renamed to target directory
            using (var tempStream = new FileStream(tempFile, FileMode.Open))
            {
                using var targetStream = new FileStream(targetFile, FileMode.Create);
                await tempStream.CopyToAsync(targetStream);
            }

            //File.Delete(TempPath);

            return Path.GetFileName(targetFile);
        }

        public async Task<PageOperationResult> ApproveAsync<K, TEntity>(K Id, int RegistroId, string NotificacionId, bool IsDocumento = false)
            where K : IEquatable<K> where TEntity : class, IIdentidad<K>, IPROTAprobacion
        {
            PageOperationResult result = new() { Success = true, Message = "Datos aprobados con éxito" };
            DateTime fixedStamp = DateTime.Now;

            try
            {
                var query = await _data.Set<TEntity>().Where(w => w.Id.Equals(Id)).FirstOrDefaultAsync();

                query.UsuarioApruebaId = _access.SessionUserId;
                query.FechaAprobacion = fixedStamp;

                _data.Update(query);

                // Add notifications
                var registro = await _data.PROTRegistros.Where(w => w.Id == RegistroId).AsNoTracking().FirstOrDefaultAsync();
                var documentoMostrar = !IsDocumento ? "" : (query as PROTDocumento).Mostrar;

                var specs = !IsDocumento
                    ? await _data.Catalogos.Where(w => w.Id == NotificacionId).AsNoTracking().FirstOrDefaultAsync()
                    : await _data.Catalogos.Where(w => w.Grupo == "PROTNOTIFICACION" && w.RefVal3 == (query as PROTDocumento).TipoDocumentoId).AsNoTracking().FirstOrDefaultAsync();

                // Contact
                _data.CorreoMensajes.Add(new CorreoMensaje
                {
                    AppId = "PRPT",
                    Estatus = 0,
                    Destinatario = registro.ContactoCorreo,
                    Contenido = specs.RefVal2.Replace("{:n}", registro.RazonSocial).Replace("{:d}", documentoMostrar),
                    RegUsuarioId = _access.SessionUserId,
                    ModUsuarioId = _access.SessionUserId
                });

                await _data.SaveChangesAsync();

                // Process approvals
                //await ProcessApprovalsAsync(RegistroId);

                result.TimeValue = fixedStamp;
                result.Content = "approve";
                result.Id = Id.ToString();
            }
            catch (Exception any)
            {
                result.Success = false;
                result.Message = any.Message;
            }

            return result;
        }

        public async Task<PageOperationResult> SaveEmailAsync(string RegistroId)
        {
            PageOperationResult result = new() { Success = true, Message = "Datos guardados con éxito" };

            try
            {
                var generales = await _data.PROTGenerales.Where(w => w.Id == Int32.Parse(RegistroId)).FirstOrDefaultAsync();
                DateTime fixedStamp = DateTime.Now;

                var correo = new CorreoMensaje
                {
                    Destinatario = generales.RProteccionCorreo,
                    Contenido = "Su registro ha sido aprovado con exito.",
                    AppId = "PRPT",
                    Estatus = 0,
                    RegFecha = fixedStamp,
                    RegUsuarioId = _access.SessionUserId,
                    ModFecha = fixedStamp,
                    ModUsuarioId = _access.SessionUserId
                };

                _data.Update(correo);
                await _data.SaveChangesAsync();
            }
            catch (Exception any)
            {
                result.Success = false;
                result.Message = any.Message;
            }

            return result;
        }

        public async Task<PROTPlantillaDownload> EditPlantillaAsync(int DocumentoId, string RootFolder, string TempFolder, int RegistroId, string NameContent = "")
        {
            PROTPlantillaDownload download = new() { FileName = "", FilePath = "" };


            var documento = await _data.PROTPlantilla.Where(w => w.Id == DocumentoId)
                .Include(i => i.TipoDocumento)
                .FirstOrDefaultAsync();

            if (documento is null) return download;

            var FileName = $"{documento.TipoDocumento.Descripcion}{NameContent}{Path.GetExtension(documento.Archivo).ToLower()}";

            // Get the physical file path
            string filePath = Path.Combine(RootFolder, documento.TipoDocumentoId, documento.Archivo);

            if (!File.Exists(filePath)) return download;

            NameContent = (!NameContent.Empty()) ? $" {NameContent}" : NameContent;

            var uniqueName = Guid.NewGuid().GetString(0, 8) + "_" + FileName;
            var tempFile = Path.Combine(TempFolder, uniqueName);

            // Copy renamed to target directory
            using (var tempStream = new FileStream(filePath, FileMode.Open))
            {
                using var targetStream = new FileStream(tempFile, FileMode.Create);
                await tempStream.CopyToAsync(targetStream);
            }

            var celdas = _data.PROTExcelCellsEdit.Where(w => w.RegPlantillaId == documento.Id && w.Editable == true)
                .ToList();

            var res = EditExcel(tempFile, RegistroId, celdas);
            // Set the download properties
            download.FileName = FileName;
            download.FilePath = tempFile;

            return download;
        }

        public bool EditExcel(string tempFile, int Id, IEnumerable<PROTExcelCellsEdit> ModelData)
        {
            using (var workbook = new XLWorkbook(tempFile))
            {
                foreach(var item in ModelData)
                {
                    try
                    {
                        var blogs = _data.PROTRegistros.FromSqlRaw<PROTRegistro>(item.sentencia, Id)
                                   .Include(b => b.Generales)
                                   .Include(b => b.Documentos)
                                   .FirstOrDefault();
                        var worksheet = workbook.Worksheet(item.Sheet);
                        worksheet.Cell(item.Row, item.Column).Value = blogs.RazonSocial;
                    }
                    catch (Exception)
                    {
                        return false;
                    }

                }
                workbook.SaveAs(tempFile);
            }
            return true;
        }

        private async Task ProcessApprovalsAsync(int RegistroId)
        {
            // Clean before validate
            _data.ChangeTracker.Clear();

            var registro = await _data.PROTRegistros.Where(w => w.Id == RegistroId).AsNoTracking().FirstOrDefaultAsync();
            var generales = await _data.PROTGenerales.Where(w => w.Id == registro.GeneralesId).AsNoTracking().FirstOrDefaultAsync();
            var documentos = await _data.PROTDocumentos.Where(w => w.RegistroId == RegistroId).Include(i => i.TipoDocumento).AsNoTracking().ToListAsync();

            if (registro.EstatusId == PROTConstants.INICIAL)
            {
                // Validate approvals
                bool proceed = generales.FechaAprobacion.HasValue
                    && !documentos.Where(w => w.TipoDocumento.RefVal3 == "1" && !w.FechaAprobacion.HasValue).Any();

                if (proceed)
                {
                    registro.EstatusId = GCConstants.FECHA;
                    proceed = false;

                    // Add notification                        
                    var specs = await _data.Catalogos.Where(w => w.Id == "PROTN92").AsNoTracking().FirstOrDefaultAsync();

                    // Contact
                    _data.CorreoMensajes.Add(new CorreoMensaje
                    {
                        AppId = "PRPT",
                        Estatus = 0,
                        Destinatario = registro.ContactoCorreo,
                        Contenido = specs.RefVal1.Replace("{:n}", registro.RazonSocial),
                        RegUsuarioId = _access.SessionUserId,
                        ModUsuarioId = _access.SessionUserId
                    });

                    _data.Update(registro);
                    await _data.SaveChangesAsync();
                }
            }
            else
            {
                // Validate approvals
                bool proceed = !documentos.Where(w => w.TipoDocumento.RefVal3 == "2" && !w.FechaAprobacion.HasValue).Any();

                if (proceed)
                {
                    // Add notification
                    var recipients = await _data.Users.Include(i => i.Claims).Where(w => w.Claims.Where(c => c.ClaimType == ClaimNames.PROTEnable).Any()).ToListAsync();
                    var specs = await _data.Catalogos.Where(w => w.Id == "PROTN93").AsNoTracking().FirstOrDefaultAsync();

                    // Internal
                    foreach (var target in recipients)
                    {
                        _data.CorreoMensajes.Add(new CorreoMensaje
                        {
                            AppId = "PRPT",
                            Estatus = 0,
                            Destinatario = target.Email,
                            Contenido = specs.RefVal1.Replace("{:n}", registro.RazonSocial),
                            RegUsuarioId = _access.SessionUserId,
                            ModUsuarioId = _access.SessionUserId
                        });
                    }

                    _data.Update(registro);
                    await _data.SaveChangesAsync();
                }
            }
        }
    }
}
