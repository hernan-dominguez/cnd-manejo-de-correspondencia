using AutoMapper;
using CWA.AccessControl.Services;
using CWA.Application.Services.Bases;
using CWA.Data;
using CWA.Entities.Bases;
using CWA.Entities.Bases.GrandesClientes;
using CWA.Entities.Comun;
using CWA.Entities.GrandesClientes;
using CWA.Models.GrandesClientes.Edit;
using CWA.Models.GrandesClientes.View;
using CWA.Shared.Helpers;
using CWA.Shared.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CWA.AccessControl.Constants;
using CWA.AccessControl.Authorization.GrandesClientes;
using Microsoft.Extensions.Logging;

namespace CWA.Application.Services.GrandesClientes
{
    public partial class GCService : BaseService
    {
        public readonly string APPID = "PRGC";
        public readonly string PASIVO = "GCT01";
        public readonly string ACTIVO = "GCT02";
        public readonly string DOC_PROY = "GCD09";
        public readonly string DOC_SMEC = "GCD11";
        public readonly string DOC_CONT = "GCD12";

        public GCService(DataContext Data, AppAccessControlService Access, IMapper Mapper, ILogger<BaseService> Logger) : base(Data, Access, Mapper, Logger) { }

        public async Task<TModel> GetDatosAsync<K, TEntity, TModel>(K Id) where K : IEquatable<K> where TEntity : class, IIdentidad<K> where TModel : class, new()
        {
            var query = await _data.Set<TEntity>().Where(w => w.Id.Equals(Id)).FirstOrDefaultAsync();

            return (query is null) ? new TModel() : _mapper.Map<TModel>(query);
        }

        public async Task<PageOperationResult> SaveRegistroAsync(GCRegistroEdit ModelData, string NotificacionId, int? RegistroId = null)
        {
            PageOperationResult result = new() { Success = true, Message = "Datos guardados con éxito" };
            var registro = new GCRegistro();

            try
            {
                if (RegistroId.HasValue) registro = await _data.GCRegistros.Where(w => w.Id == RegistroId.Value).FirstOrDefaultAsync();

                registro = _mapper.Map(ModelData, registro);

                if (!RegistroId.HasValue)
                {
                    registro.EstatusId = GCConstants.INICIAL;
                    registro.FechaEditable = true;
                    registro.RegUsuarioId = _access.SessionUserId;
                    registro.ModUsuarioId = _access.SessionUserId;

                    // Create Generales and (if required) Comercial records
                    registro.Generales = new GCGenerales { RegUsuarioId = _access.SessionUserId, ModUsuarioId = _access.SessionUserId };

                    if (registro.TipoGranClienteId == ACTIVO) registro.Comercial = new GCComercial { RegUsuarioId = _access.SessionUserId, ModUsuarioId = _access.SessionUserId };

                    // Create Documentos
                    var tipogc = registro.TipoGranClienteId == ACTIVO ? "A" : "P";
                    var documentSpecs = await _data.Catalogos.Where(w => w.Grupo == "GCDOCUMENTO" && w.RefVal4 != "NOLIST" && w.RefVal5.Contains(tipogc)).AsNoTracking().ToListAsync();
                    registro.Documentos = new List<GCDocumento>();

                    foreach (var spec in documentSpecs)
                    {
                        registro.Documentos.Add(new GCDocumento
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
                var recipients = await _data.GCDestinatarios.Where(w => w.NotificacionId == NotificacionId).Include(i => i.Usuario).AsNoTracking().ToListAsync();
                var specs = await _data.Catalogos.Where(w => w.Id == NotificacionId).AsNoTracking().FirstOrDefaultAsync();

                // Internal
                foreach (var target in recipients)
                {
                    _data.CorreoMensajes.Add(new CorreoMensaje
                    {
                        AppId = APPID,
                        Estatus = 0,
                        Destinatario = target.Usuario.Email,
                        Asunto = specs.RefVal4.Replace("{:n}", registro.Nombre),
                        Previa = "Registro de Gran Cliente",
                        Saludo = $"Estimado(a) {target.Usuario.Nombre}",
                        Contenido = specs.RefVal1.Replace("{:n}", registro.Nombre),
                        RegUsuarioId = _access.SessionUserId,
                        ModUsuarioId = _access.SessionUserId
                    });
                }

                // Contact
                _data.CorreoMensajes.Add(new CorreoMensaje
                {
                    AppId = APPID,
                    Estatus = 0,
                    Destinatario = registro.ContactoCorreo,
                    Asunto = specs.RefVal4.Replace("{:n}", registro.Nombre),
                    Previa = "Registro de Gran Cliente",
                    Saludo = $"Estimado(a) {registro.ContactoNombre}",
                    Contenido = specs.RefVal1.Replace("{:n}", registro.Nombre),
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

        public async Task<PageOperationResult> SaveFechaEntradaAsync(int RegistroId, DateTime ModelData, string NotificacionId)
        {
            PageOperationResult result = new() { Success = true, Message = "Fecha actualizada con éxito" };
            DateTime fixedStamp = DateTime.Now;

            try
            {
                var registro = await _data.GCRegistros.Where(w => w.Id == RegistroId).FirstOrDefaultAsync();

                registro.FechaEntrada = ModelData.Date;
                registro.ModUsuarioId = _access.SessionUserId;
                registro.ModFecha = fixedStamp;

                if (registro.EstatusId == GCConstants.FECHA) registro.EstatusId = GCConstants.REQUISITOS;

                _data.Update(registro);

                // Add notifications
                var itemkey = $"{registro.Id}:{NotificacionId}";

                if (!(await _data.CorreoMensajes.Where(w => w.AppId == APPID && w.ItemKey == itemkey && w.Estatus == 0).AnyAsync()))
                {
                    var recipients = await _data.GCDestinatarios.Where(w => w.NotificacionId == NotificacionId).Include(i => i.Usuario).AsNoTracking().ToListAsync();
                    var specs = await _data.Catalogos.Where(w => w.Id == NotificacionId).AsNoTracking().FirstOrDefaultAsync();

                    // Internal
                    foreach (var target in recipients)
                    {
                        _data.CorreoMensajes.Add(new CorreoMensaje
                        {
                            AppId = APPID,
                            Estatus = 0,
                            ItemKey = itemkey,
                            Destinatario = target.Usuario.Email,
                            Asunto = specs.RefVal4,
                            Previa = "Registro de Gran Cliente",
                            Saludo = $"Estimado(a) {target.Usuario.Nombre}",
                            Contenido = specs.RefVal1.Replace("{:n}", registro.Nombre),
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

        public async Task<PageOperationResult> CreateMedicionAsync(int RegistroId, GCMedicionEdit ModelData, string NotificacionId)            
        {
            PageOperationResult result = new() { Success = true, Message = "Datos guardados con éxito" };

            try
            {
                var query = new GCMedicion();
                var registro = await ViewRegistroAsync(RegistroId);

                query = _mapper.Map(ModelData, query);
                query.RegistroId = RegistroId;
                query.RegUsuarioId = _access.SessionUserId;
                query.ModUsuarioId = _access.SessionUserId;

                _data.Update(query);

                // Add notifications
                var itemkey = $"{RegistroId}:{NotificacionId}";

                if (!(await _data.CorreoMensajes.Where(w => w.AppId == APPID && w.ItemKey == itemkey && w.Estatus == 0).AnyAsync()))
                {
                    var recipients = await _data.GCDestinatarios.Where(w => w.NotificacionId == NotificacionId).Include(i => i.Usuario).AsNoTracking().ToListAsync();
                    var specs = await _data.Catalogos.Where(w => w.Id == NotificacionId).AsNoTracking().FirstOrDefaultAsync();

                    // Internal
                    foreach (var target in recipients)
                    {
                        _data.CorreoMensajes.Add(new CorreoMensaje
                        {
                            AppId = APPID,
                            Estatus = 0,
                            ItemKey = itemkey,
                            Destinatario = target.Usuario.Email,
                            Asunto = specs.RefVal4,
                            Previa = "Registro de Gran Cliente",
                            Saludo = $"Estimado(a) {target.Usuario.Nombre}",
                            Contenido = specs.RefVal1.Replace("{:n}", registro.Nombre),
                            RegUsuarioId = _access.SessionUserId,
                            ModUsuarioId = _access.SessionUserId
                        });
                    }
                }

                await _data.SaveChangesAsync();
            }
            catch (Exception any)
            {
                result.Success = false;
                result.Message = any.Message;
            }

            return result;
        }

        public async Task<PageOperationResult> SaveDatosAsync<K, TModel, TEntity>(K Id, TModel ModelData, string NotificacionId, int RegistroId, bool NotifyOwner = false)
            where K : IEquatable<K> where TEntity : class, IIdentidad<K>, IAuditoria where TModel : class
        {
            PageOperationResult result = new() { Success = true, Message = "Datos guardados con éxito" };
            DateTime fixedStamp = DateTime.Now;

            try
            {
                var query = await _data.Set<TEntity>().Where(w => w.Id.Equals(Id)).FirstOrDefaultAsync();
                var registro = await ViewRegistroAsync(RegistroId);

                // Pre-mapping actions
                if (query is GCMedidorDist) foreach (var canal in (query as GCMedidorDist).Canales) _data.Entry(canal).State = EntityState.Deleted;

                query = _mapper.Map(ModelData, query);
                query.ModUsuarioId = _access.SessionUserId;
                query.ModFecha = fixedStamp;

                _data.Update(query);

                // Add notifications
                var specs = await _data.Catalogos.Where(w => w.Id == NotificacionId).AsNoTracking().FirstOrDefaultAsync();
                var itemkey = $"{RegistroId}:{NotificacionId}";

                if (!NotifyOwner)
                {
                    // Internal
                    var recipients = await _data.GCDestinatarios.Where(w => w.NotificacionId == NotificacionId).Include(i => i.Usuario).AsNoTracking().ToListAsync();
                    
                    if (!(await _data.CorreoMensajes.Where(w => w.AppId == APPID && w.ItemKey == itemkey && w.Estatus == 0 && w.Destinatario != registro.ContactoCorreo).AnyAsync()))
                    {
                        foreach (var target in recipients)
                        {
                            _data.CorreoMensajes.Add(new CorreoMensaje
                            {
                                AppId = APPID,
                                Estatus = 0,
                                ItemKey = itemkey,
                                Destinatario = target.Usuario.Email,
                                Asunto = specs.RefVal4,
                                Previa = "Registro de Gran Cliente",
                                Saludo = $"Estimado(a) {target.Usuario.Nombre}",
                                Contenido = specs.RefVal1.Replace("{:n}", registro.Nombre),
                                RegUsuarioId = _access.SessionUserId,
                                ModUsuarioId = _access.SessionUserId
                            });
                        }
                    }
                }
                else
                {
                    // Contact
                    if (!(await _data.CorreoMensajes.Where(w => w.AppId == APPID && w.ItemKey == itemkey && w.Estatus == 0 && w.Destinatario == registro.ContactoCorreo).AnyAsync()))
                    {
                        _data.CorreoMensajes.Add(new CorreoMensaje
                        {
                            AppId = APPID,
                            Estatus = 0,
                            ItemKey = itemkey,
                            Destinatario = registro.ContactoCorreo,
                            Asunto = specs.RefVal4,
                            Previa = "Registro de Gran Cliente",
                            Saludo = $"Estimado(a) {registro.ContactoNombre}",
                            Contenido = specs.RefVal1.Replace("{:n}", registro.Nombre),
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

        public async Task<PageOperationResult> SaveDocumentoAsync(int RegistroId, GCDocumentoEdit ModelData, string RootFolder, string TempFolder, string Nombre)
        {
            PageOperationResult result = new() { Success = true, Message = "Datos guardados con éxito" };
            DateTime fixedStamp = DateTime.Now;

            try
            {
                var documento = await _data.GCDocumentos.Where(w => w.Id == ModelData.DocumentoId.Value)
                    .Include(i => i.Registro)
                    .Include(i => i.TipoDocumento)
                    .FirstOrDefaultAsync();

                // File
                string fileName = documento.Loaded ? documento.Archivo : $"{documento.Archivo}{ModelData.DocumentUpload.GetExtension()}";
                string archivo = await SaveFileAsync(RegistroId, documento.Registro.RegFecha, ModelData.DocumentUpload, RootFolder, TempFolder, fileName.ToUpper());

                // Data
                documento.Archivo = fileName;
                documento.Loaded = true;
                documento.ModFecha = fixedStamp;
                documento.ModUsuarioId = _access.SessionUserId;

                // Check if file is auto-accepted
                if (documento.TipoDocumento.RefVal4 == "AUTO")
                {
                    documento.UsuarioAtencionId = 1;
                    documento.FechaAtencion = fixedStamp;
                }

                _data.Update(documento);

                // Add notifications                
                var specs = await _data.Catalogos.Where(w => w.Grupo == "GCNOTIFICACION" && w.RefVal3 == documento.TipoDocumento.Id).AsNoTracking().FirstOrDefaultAsync();
                var itemkey = $"{RegistroId}:{specs.Id}";

                if (!(await _data.CorreoMensajes.Where(w => w.AppId == APPID && w.ItemKey == itemkey && w.Estatus == 0).AnyAsync()))
                {
                    var recipients = await _data.GCDestinatarios.Where(w => w.NotificacionId == specs.Id).Include(i => i.Usuario).AsNoTracking().ToListAsync();

                    // Internal
                    foreach (var target in recipients)
                    {
                        _data.CorreoMensajes.Add(new CorreoMensaje
                        {
                            AppId = APPID,
                            Estatus = 0,
                            ItemKey = itemkey,
                            Destinatario = target.Usuario.Email,
                            Asunto = specs.RefVal4,
                            Previa = "Registro de Gran Cliente",
                            Saludo = $"Estimado(a) {target.Usuario.Nombre}",
                            Contenido = specs.RefVal1.Replace("{:d}", documento.Mostrar).Replace("{:n}", Nombre),
                            RegUsuarioId = _access.SessionUserId,
                            ModUsuarioId = _access.SessionUserId
                        });
                    }
                }

                await _data.SaveChangesAsync();

                result.Content = new GCDocumentoResult
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

        private static async Task<string> SaveFileAsync(int RegistroId, DateTime RegFecha, IFormFile FileData, string RootPath, string TempPath, string FileName)
        {
            // Write physical temp file            
            Directory.CreateDirectory(Path.Combine(RootPath, $"{RegFecha:yyy}", $"GC{RegistroId}"));

            string targetFile = Path.Combine(RootPath, $"{RegFecha:yyy}", $"GC{RegistroId}", FileName);
            string tempFile = await FileData.WriteTempAsync(TempPath);

            // Copy renamed to target directory
            using (var tempStream = new FileStream(tempFile, FileMode.Open))
            {
                using var targetStream = new FileStream(targetFile, FileMode.Create);
                await tempStream.CopyToAsync(targetStream);
            }

            return Path.GetFileName(targetFile);
        }

        public async Task<PageOperationResult> ApproveAsync<K, TEntity>(K Id, int RegistroId, string NotificacionId, bool AddMedidorDist = false, bool AddDocumentoSmec = false, bool IsDocumento = false)
            where K : IEquatable<K> where TEntity : class, IIdentidad<K>, IAtencion
        {
            PageOperationResult result = new() { Success = true, Message = "Datos aprobados con éxito" };
            DateTime fixedStamp = DateTime.Now;

            try
            {
                var query = await _data.Set<TEntity>().Where(w => w.Id.Equals(Id)).FirstOrDefaultAsync();

                query.UsuarioAtencionId = _access.SessionUserId;
                query.FechaAtencion = fixedStamp;

                // Create medidor if required                
                if (AddMedidorDist) (query as GCMedicion).Medidor = new GCMedidorDist { RegUsuarioId = _access.SessionUserId, ModUsuarioId = _access.SessionUserId };

                // Create documento SMEC if required
                if (AddDocumentoSmec)
                {
                    var spec = await _data.Catalogos.Where(w => w.Id == DOC_SMEC).FirstOrDefaultAsync();
                                        
                    (query as GCMedicion).SAS = new GCDocumento
                    {
                        RegistroId = RegistroId,
                        Loaded = false,
                        Archivo = $"{spec.RefVal2}_{Guid.NewGuid().GetString(0, 10, true)}",
                        Mostrar = $"{spec.Descripcion} ({(query as GCMedicion).Serie})",
                        TipoDocumentoId = spec.Id,
                        RegUsuarioId = _access.SessionUserId,
                        ModUsuarioId = _access.SessionUserId
                    };
                }

                _data.Update(query);

                // Add notifications
                var registro = await _data.GCRegistros.Where(w => w.Id == RegistroId).AsNoTracking().FirstOrDefaultAsync();
                var documentoMostrar = !IsDocumento ? "" : (query as GCDocumento).Mostrar;

                var specs = !IsDocumento
                    ? await _data.Catalogos.Where(w => w.Id == NotificacionId).AsNoTracking().FirstOrDefaultAsync()
                    : await _data.Catalogos.Where(w => w.Grupo == "GCNOTIFICACION" && w.RefVal3 == (query as GCDocumento).TipoDocumentoId).AsNoTracking().FirstOrDefaultAsync();

                // Contact
                _data.CorreoMensajes.Add(new CorreoMensaje
                {
                    AppId = APPID,
                    Estatus = 0,
                    Destinatario = registro.ContactoCorreo,
                    Asunto = specs.RefVal5,
                    Previa = "Registro de Gran Cliente",
                    Saludo = $"Estimado(a) {registro.ContactoNombre}",
                    Contenido = specs.RefVal2.Replace("{:n}", registro.Nombre).Replace("{:d}", documentoMostrar),
                    RegUsuarioId = _access.SessionUserId,
                    ModUsuarioId = _access.SessionUserId
                });

                await _data.SaveChangesAsync();

                // Process approvals
                await ProcessApprovalsAsync(RegistroId);

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

        private async Task ProcessApprovalsAsync(int RegistroId)
        {
            // Clean before validate
            _data.ChangeTracker.Clear();

            var registro = await _data.GCRegistros.Where(w => w.Id == RegistroId).AsNoTracking().FirstOrDefaultAsync();
            var generales = await _data.GCGenerales.Where(w => w.Id == registro.GeneralesId).AsNoTracking().FirstOrDefaultAsync();
            var comercial = await _data.GCComerciales.Where(w => w.Id == registro.ComercialId).AsNoTracking().FirstOrDefaultAsync();            
            var mediciones = await _data.GCMediciones.Where(w => w.RegistroId == RegistroId).Include(i => i.Medidor).AsNoTracking().ToListAsync();
            var documentos = await _data.GCDocumentos.Where(w => w.RegistroId == RegistroId).Include(i => i.TipoDocumento).AsNoTracking().ToListAsync();

            if (registro.EstatusId == GCConstants.INICIAL)
            {
                // Validate approvals
                bool proceed = generales.FechaAtencion.HasValue                    
                    && mediciones.Count > 0
                    && !mediciones.Where(w => !w.FechaAtencion.HasValue).Any()
                    && !documentos.Where(w => w.TipoDocumento.RefVal3 == "1" && !w.FechaAtencion.HasValue).Any();

                if (proceed)
                {
                    // Validate starting date
                    if (registro.FechaEntrada is not null && registro.FechaEntrada.Value.Date >= DateTime.Now.Date.AddDays(30))
                    {
                        registro.EstatusId = GCConstants.REQUISITOS;

                        // Add documentos
                        var distribuidoras = mediciones.Where(w => w.DistribuidoraId.HasValue).GroupBy(g => g.DistribuidoraId).Select(t => t.Key.Value).ToList();

                        foreach (var distribuidoraid in distribuidoras)
                        {
                            var distribuidora = await _data.Agentes.Where(w => w.Id == distribuidoraid).FirstOrDefaultAsync();
                            var spec = await _data.Catalogos.Where(w => w.Id == DOC_CONT).FirstOrDefaultAsync();

                            _data.Add(new GCDocumento
                            {
                                RegistroId = RegistroId,
                                Loaded = false,
                                Archivo = $"{spec.RefVal2}_{Guid.NewGuid().GetString(0, 10, true)}",
                                Mostrar = $"{spec.Descripcion} ({distribuidora.Codigo})",
                                TipoDocumentoId = spec.Id,
                                RegUsuarioId = _access.SessionUserId,
                                ModUsuarioId = _access.SessionUserId
                            });
                        }

                        // Add notification                        
                        var specs = await _data.Catalogos.Where(w => w.Id == "GCN91").AsNoTracking().FirstOrDefaultAsync();

                        // Contact
                        _data.CorreoMensajes.Add(new CorreoMensaje
                        {
                            AppId = APPID,
                            Estatus = 0,
                            Destinatario = registro.ContactoCorreo,
                            Asunto = specs.RefVal4,
                            Previa = "Registro de Gran Cliente",
                            Saludo = $"Estimado(a) {registro.ContactoNombre}",
                            Contenido = specs.RefVal1.Replace("{:n}", registro.Nombre),
                            RegUsuarioId = _access.SessionUserId,
                            ModUsuarioId = _access.SessionUserId
                        });
                    }
                    else
                    {
                        registro.EstatusId = GCConstants.FECHA;
                        registro.FechaEntrada = null;
                        registro.FechaEditable = true;
                        proceed = false;

                        // Add notification                        
                        var specs = await _data.Catalogos.Where(w => w.Id == "GCN92").AsNoTracking().FirstOrDefaultAsync();

                        // Contact
                        _data.CorreoMensajes.Add(new CorreoMensaje
                        {
                            AppId = APPID,
                            Estatus = 0,
                            Destinatario = registro.ContactoCorreo,
                            Asunto = specs.RefVal4,
                            Previa = "Registro de Gran Cliente",
                            Saludo = $"Estimado(a) {registro.ContactoNombre}",
                            Contenido = specs.RefVal1.Replace("{:n}", registro.Nombre),
                            RegUsuarioId = _access.SessionUserId,
                            ModUsuarioId = _access.SessionUserId
                        });
                    }

                    _data.Update(registro);
                    await _data.SaveChangesAsync();
                }
            }
            else
            {
                // Validate approvals
                bool proceed = (comercial is null || comercial.FechaAtencion.HasValue)                    
                    && !mediciones.Where(w => w.MedidorId.HasValue && !w.Medidor.FechaAtencion.HasValue).Any()
                    && !documentos.Where(w => w.TipoDocumento.RefVal3 == "2" && !w.FechaAtencion.HasValue).Any();

                if (proceed)
                {
                    registro.EstatusId = GCConstants.AUTORIZADO;
                    registro.FechaEditable = false;
                    registro.FechaAtencion = DateTime.Now.Date;
                    registro.UsuarioAtencionId = _access.SessionUserId;

                    // If required, modify FechaEntrada based on calculation using itself and current date plus 3 business days
                    var dateIn = registro.FechaEntrada.Value.Date;
                    var days = (dateIn - DateTime.Now.Date).Days;
                    var moveDate = false;

                    if (days < 3)
                    {
                        moveDate = true;
                    }
                    else
                    {
                        var current = DateTime.Now.Date.AddDays(-1);
                        var counted = 0;

                        while (current < dateIn)
                        {
                            current = current.AddDays(1);

                            // Check weekend
                            if (current.DayOfWeek == DayOfWeek.Saturday || current.DayOfWeek == DayOfWeek.Sunday) continue;

                            // Check irregular
                            if (await _data.DiasIrregulares.Where(w => w.Dia.Date == current).AnyAsync()) continue;

                            counted++;
                        }

                        moveDate = counted < 3;
                    }

                    // Move date if needed
                    if (moveDate)
                    {
                        var added = 0;

                        while (added < 3)
                        {
                            dateIn = dateIn.AddDays(1);

                            // Check weekend
                            if (dateIn.DayOfWeek == DayOfWeek.Sunday || dateIn.DayOfWeek == DayOfWeek.Saturday) continue;

                            // Check irregular
                            if (await _data.DiasIrregulares.Where(w => w.Dia.Date == dateIn).AnyAsync()) continue;

                            added++;
                        }

                        registro.FechaEntrada = dateIn;
                    }

                    // Add notification
                    var recipients = await _data.Users.Include(i => i.Claims).Where(w => w.Claims.Where(c => c.ClaimType == ClaimNames.GCEnable).Any()).ToListAsync();
                    var specs = await _data.Catalogos.Where(w => w.Id == "GCN93").AsNoTracking().FirstOrDefaultAsync();

                    // Internal
                    foreach (var target in recipients)
                    {
                        _data.CorreoMensajes.Add(new CorreoMensaje
                        {
                            AppId = APPID,
                            Estatus = 0,
                            Destinatario = target.Email,
                            Asunto = specs.RefVal4,
                            Previa = "Registro de Gran Cliente",
                            Saludo = $"Estimado(a) {target.Nombre}",
                            Contenido = specs.RefVal1.Replace("{:n}", registro.Nombre),
                            RegUsuarioId = _access.SessionUserId,
                            ModUsuarioId = _access.SessionUserId
                        });
                    }

                    _data.Update(registro);
                    await _data.SaveChangesAsync();
                }
            }
        }

        public async Task<PageOperationResult> NotifyAuthorizationAsync(int RegistroId)
        {
            PageOperationResult result = new() { Success = true, Message = "Se envió la notificación con éxito" };
            DateTime fixedStamp = DateTime.Now;

            try
            {
                var registro = await _data.GCRegistros.Where(w => w.Id == RegistroId).FirstOrDefaultAsync();
                var fecha = registro.FechaEntrada.Value.Date;

                // Add notification                        
                var specs = await _data.Catalogos.Where(w => w.Id == "GCN94").AsNoTracking().FirstOrDefaultAsync();
                var textDate = $"{fecha.Day} de {fecha.MonthName()} del {fecha.Year}";

                // Contact
                _data.CorreoMensajes.Add(new CorreoMensaje
                {
                    AppId = APPID,
                    Estatus = 0,
                    Destinatario = registro.ContactoCorreo,
                    Asunto = specs.RefVal4,
                    Previa = "Registro de Gran Cliente",
                    Saludo = $"Estimado(a) {registro.ContactoNombre}",
                    Contenido = specs.RefVal1.Replace("{:n}", registro.Nombre).Replace("{:d}", textDate),
                    RegUsuarioId = _access.SessionUserId,
                    ModUsuarioId = _access.SessionUserId
                });

                // Finalize
                registro.EstatusId = GCConstants.NOTIFICADO;
                await _data.SaveChangesAsync();

                result.Content = "notify";
            }
            catch (Exception any)
            {
                result.Success = false;
                result.Message = any.Message;
            }

            return result;
        }

    }
}
