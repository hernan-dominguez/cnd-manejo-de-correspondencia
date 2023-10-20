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
using Microsoft.Extensions.Logging;

namespace CWA.Application.Services.ViabilidadContratos
{
    public partial class VCService : BaseService
    {
        private readonly string APPID = "PVCS";

        public VCService(DataContext Data, AppAccessControlService Access, IMapper Mapper, ILogger<BaseService> Logger) : base(Data, Access, Mapper, Logger) { }

        public async Task<PageOperationResult> SaveNacionalAsync(VCNacionalEdit ModelData)
        {
            PageOperationResult result = new() { Success = true, Message = "Datos guardados con éxito" };
            string fixedYear = $"{ DateTime.Now.Year}";
            string uniqueId = $"{Guid.NewGuid().GetString(0, 10, true)}";

            var registro = new VCNacional();

            try
            {
                registro = _mapper.Map(ModelData, registro);
                registro.RegUsuarioId = _access.SessionUserId;
                registro.ModUsuarioId = _access.SessionUserId;
                                
                // Documentos
                foreach (var documento in registro.Documentos)
                {
                    var fileData = ModelData.Documentos.Where(w => w.TipoDocumentoId == documento.TipoDocumentoId).First().DocumentUpload;
                    string fileName = $"{documento.TipoDocumentoId}{fileData.GetExtension()}".ToUpper();

                    documento.Archivo = await SaveFileAsync(fileData, fileName, ModelData.DocsPath, ModelData.TempPath, fixedYear, registro.TipoContratoId, uniqueId);
                    documento.RegUsuarioId = _access.SessionUserId;
                    documento.ModUsuarioId = _access.SessionUserId;
                }

                // Save
                _data.Update(registro);
                await _data.SaveChangesAsync();

                // Add notification
                await NacionalNotificationsAsync(registro, ModelData.NotificacionId, registro.Inicia, true);

                result.Content = "update";
            }
            catch (Exception any)
            {
                result.Success = false;
                result.Message = any.Message;
            }

            return result;
        }

        public async Task<PageOperationResult> SaveRegionalAsync(VCRegionalEdit ModelData)
        {
            PageOperationResult result = new() { Success = true, Message = "Datos guardados con éxito" };
            string fixedYear = $"{ DateTime.Now.Year}";
            string uniqueId = $"{Guid.NewGuid().GetString(0, 10, true)}";

            var registro = new VCRegional();

            try
            {
                registro = _mapper.Map(ModelData, registro);
                registro.RegUsuarioId = _access.SessionUserId;
                registro.ModUsuarioId = _access.SessionUserId;

                // Documentos
                foreach (var documento in registro.Documentos)
                {
                    var fileData = ModelData.Documentos.Where(w => w.TipoDocumentoId == documento.TipoDocumentoId).First().DocumentUpload;
                    string fileName = $"{documento.TipoDocumentoId}{fileData.GetExtension()}".ToUpper();

                    documento.Archivo = await SaveFileAsync(fileData, fileName, ModelData.DocsPath, ModelData.TempPath, fixedYear, registro.TipoSolicitudId, uniqueId);
                    documento.RegUsuarioId = _access.SessionUserId;
                    documento.ModUsuarioId = _access.SessionUserId;
                }

                // Save
                _data.Update(registro);
                await _data.SaveChangesAsync();

                // Add notifications
                await RegionalNotificationsAsync(registro, ModelData.NotificacionId, registro.Inicia, true);
            }
            catch (Exception any)
            {
                result.Success = false;
                result.Message = any.Message;
            }

            return result;
        }

        public async Task<PageOperationResult> SaveEnmiendaAsync(VCEnmiendaEdit ModelData)
        {
            PageOperationResult result = new() { Success = true, Message = "Datos guardados con éxito" };
            string fixedYear = $"{ DateTime.Now.Year}";
            string uniqueId = $"{Guid.NewGuid().GetString(0, 10, true)}";

            var registro = new VCEnmienda();

            try
            {
                var contrato = await _data.VCNacionales.Where(w => w.Id == ModelData.ContratoId)
                    .Include(i => i.Vendedor)
                    .ThenInclude(t => t.Organizacion)
                    .Include(i => i.Comprador)
                    .ThenInclude(t => t.Organizacion)
                    .Include(i => i.TipoContrato)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                registro = _mapper.Map(ModelData, registro);
                registro.RegUsuarioId = _access.SessionUserId;
                registro.ModUsuarioId = _access.SessionUserId;

                // Documentos
                foreach (var documento in registro.Documentos)
                {
                    var fileData = ModelData.Documentos.Where(w => w.TipoDocumentoId == documento.TipoDocumentoId).First().DocumentUpload;
                    string fileName = $"{documento.TipoDocumentoId}{fileData.GetExtension()}".ToUpper();

                    documento.Archivo = await SaveFileAsync(fileData, fileName, ModelData.DocsPath, ModelData.TempPath, fixedYear, contrato.TipoContratoId, uniqueId);
                    documento.RegUsuarioId = _access.SessionUserId;
                    documento.ModUsuarioId = _access.SessionUserId;
                }

                // Save
                _data.Update(registro);
                await _data.SaveChangesAsync();

                // Add notifications
                await EnmiendaNotificationsAsync(registro, contrato, ModelData.NotificacionId, registro.Inicia, true);
            }
            catch (Exception any)
            {
                result.Success = false;
                result.Message = any.Message;
            }

            return result;
        }

        private static async Task<string> SaveFileAsync(IFormFile FileData, string FileName, string DocsPath, string TempPath, string FixedYear, string TipoId, string UniqueId)
        {
            // Enable target directory
            string targetDir = Path.Combine(DocsPath, FixedYear, TipoId, UniqueId);
            Directory.CreateDirectory(targetDir);

            // Create source
            string tempFile = await FileData.WriteTempAsync(TempPath);

            // Copy source to target directory
            string targetFile = Path.Combine(targetDir, FileName);

            using (var tempStream = new FileStream(tempFile, FileMode.Open))
            {
                using var targetStream = new FileStream(targetFile, FileMode.Create);
                await tempStream.CopyToAsync(targetStream);
            }

            return string.Join('\\', FixedYear, TipoId, UniqueId, FileName);
        }

        public async Task<PageOperationResult> ApprovalNacionalAsync(VCAtencion ModelData)
        {
            PageOperationResult result = new() { Success = true, Message = "Se procesó la respuesta con éxito" };
            DateTime fixedStamp = DateTime.Now;

            try
            {
                var registro = await _data.VCNacionales.Where(w => w.Id == ModelData.RegistroId).FirstOrDefaultAsync();

                registro.UsuarioAtencionId = _access.SessionUserId;
                registro.FechaAtencion = fixedStamp;
                registro.Aprobacion = ModelData.Aprobacion;
                registro.MotivoRechazo = !ModelData.Aprobacion ? ModelData.Motivo : registro.MotivoRechazo;
                registro.ModUsuarioId = _access.SessionUserId;
                registro.ModFecha = fixedStamp;

                // Add notifications
                await NacionalNotificationsAsync(registro, ModelData.NotificacionId, registro.RegFecha);

                _data.Update(registro);
                await _data.SaveChangesAsync();

                result.TimeValue = fixedStamp;
                result.Content = ModelData.Aprobacion ? "approved" : "rejected";
            }
            catch (Exception any)
            {
                result.Success = false;
                result.Message = any.Message;
            }

            return result;
        }

        public async Task<PageOperationResult> ApprovalRegionalAsync(VCAtencion ModelData)
        {
            PageOperationResult result = new() { Success = true, Message = "Se procesó la respuesta con éxito" };
            DateTime fixedStamp = DateTime.Now;

            try
            {
                var registro = await _data.VCRegionales.Where(w => w.Id == ModelData.RegistroId).Include(i => i.Documentos).FirstOrDefaultAsync();

                registro.UsuarioAtencionId = _access.SessionUserId;
                registro.FechaAtencion = fixedStamp;
                registro.Aprobacion = ModelData.Aprobacion;
                registro.MotivoRechazo = !ModelData.Aprobacion ? ModelData.Motivo : registro.MotivoRechazo;
                registro.ModUsuarioId = _access.SessionUserId;
                registro.ModFecha = fixedStamp;

                string attachFile = "";
                string saveFile = "";
                string tipoId = "";
                string tipoName = "";
                int docId = 0;

                if (ModelData.Adjunto is not null)
                {
                    // Get specs
                    tipoId = (await _data.Catalogos.Where(w => w.Grupo == "VCADJUNTOXTIPO" && w.RefVal1 == registro.TipoSolicitudId).FirstOrDefaultAsync()).RefVal2;
                    var specs = await _data.Catalogos.Where(w => w.Id == tipoId).FirstOrDefaultAsync();
                    tipoName = specs.Descripcion;
                    string documentsPath = registro.Documentos.First().Archivo[..23];

                    // Create source and unique directory name for attachment
                    string tempFile = await ModelData.Adjunto.WriteTempAsync(ModelData.TempPath);
                    string dirName = $"{Guid.NewGuid().GetString(0, 10)}";

                    // Create files
                    Directory.CreateDirectory(Path.Combine(ModelData.MailPath, dirName));

                    attachFile = Path.Combine(ModelData.MailPath, dirName, $"{specs.Descripcion}{ModelData.Adjunto.GetExtension()}");
                    saveFile = Path.Combine(ModelData.DocsPath, documentsPath, $"{specs.Id}{ModelData.Adjunto.GetExtension()}");

                    using (var createStream = new FileStream(tempFile, FileMode.Open))
                    {
                        using var attachmentStream = new FileStream(attachFile, FileMode.Create);
                        await createStream.CopyToAsync(attachmentStream);
                    }

                    using (var createStream = new FileStream(tempFile, FileMode.Open))
                    {
                        using var saveStream = new FileStream(saveFile, FileMode.Create);
                        await createStream.CopyToAsync(saveStream);
                    }

                    // Update
                    registro.Documentos.Add(new VCDocRegional
                    {
                        Archivo = Path.Combine(documentsPath, $"{specs.Id}{ModelData.Adjunto.GetExtension()}"),
                        TipoDocumentoId = specs.Id,
                        RegUsuarioId = _access.SessionUserId,
                        ModUsuarioId = _access.SessionUserId
                    });
                }

                // Add notifications
                await RegionalNotificationsAsync(registro, ModelData.NotificacionId, registro.RegFecha, AdjuntoPath: attachFile);

                _data.Update(registro);
                await _data.SaveChangesAsync();

                // Set return content
                result.TimeValue = fixedStamp;
                result.Id = $"{registro.Id}";

                if (!tipoId.Empty()) docId = (await _data.VCDocsRegionales.Where(w => w.RegistroId == registro.Id && w.TipoDocumentoId == tipoId).FirstOrDefaultAsync()).Id;
                
                result.Content = ModelData.Aprobacion ? new { Status = "approved", docId = $"{docId}", docName = tipoName } : new { Status = "rejected" };
            }
            catch (Exception any)
            {
                Console.WriteLine(any.StackTrace);
                result.Success = false;
                result.Message = any.Message;
            }

            return result;
        }

        public async Task<PageOperationResult> ApprovalEnmiendaAsync(VCAtencion ModelData)
        {
            PageOperationResult result = new() { Success = true, Message = "Se procesó la respuesta con éxito" };
            DateTime fixedStamp = DateTime.Now;

            try
            {
                var registro = await _data.VCEnmiendas.Where(w => w.Id == ModelData.RegistroId).FirstOrDefaultAsync();

                registro.UsuarioAtencionId = _access.SessionUserId;
                registro.FechaAtencion = fixedStamp;
                registro.Aprobacion = ModelData.Aprobacion;
                registro.MotivoRechazo = !ModelData.Aprobacion ? ModelData.Motivo : registro.MotivoRechazo;
                registro.ModUsuarioId = _access.SessionUserId;
                registro.ModFecha = fixedStamp;

                // Add notifications
                var contrato = await _data.VCNacionales.Where(w => w.Id == registro.ContratoId)
                        .Include(i => i.Vendedor)
                        .ThenInclude(t => t.Organizacion)
                        .Include(i => i.Comprador)
                        .ThenInclude(t => t.Organizacion)
                        .Include(i => i.TipoContrato)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

                await EnmiendaNotificationsAsync(registro, contrato, ModelData.NotificacionId, registro.RegFecha);

                _data.Update(registro);
                await _data.SaveChangesAsync();

                result.TimeValue = fixedStamp;
                result.Content = ModelData.Aprobacion ? "approved" : "rejected";
            }
            catch (Exception any)
            {
                result.Success = false;
                result.Message = any.Message;
            }

            return result;
        }

        private async Task NacionalNotificationsAsync(VCNacional Registro, string Id, DateTime PresetDate, bool SendInternal = false)
        {
            // Get required data
            var vendedorRecipient = await _data.Agentes.Where(w => w.Id == Registro.VendedorId)
                .Include(i => i.Organizacion)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var compradorRecipient = await _data.Agentes.Where(w => w.Id == Registro.CompradorId)
                .Include(i => i.Organizacion)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var specs = await _data.Catalogos.Where(w => w.Id == Id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var tipo = await _data.Catalogos.Where(w => w.Id == Registro.TipoContratoId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            // Presets
            var textDate = $"{PresetDate.Day} de {PresetDate.MonthName()} del {PresetDate.Year}";
            var codigo = $"C-{tipo.RefVal2}-{vendedorRecipient.Codigo}-{compradorRecipient.Codigo}-{Registro.Inicia.Year}-{Registro.Id}";
            var subject = specs.RefVal3.Replace("{:i}", codigo);            
            var motivo = Registro.MotivoRechazo.Empty() ? "" : Registro.MotivoRechazo;
            var content = "";

            // Internal
            if (SendInternal)
            {
                var internalRecipients = await _data.VCDestinatarios.Where(w => w.NotificacionId == Id)
                    .Include(i => i.Usuario)
                    .AsNoTracking()
                    .ToListAsync();

                content = specs.RefVal1
                    .Replace("{:t}", tipo.Descripcion)
                    .Replace("{:f}", textDate)
                    .Replace("{:v}", vendedorRecipient.Organizacion.Nombre)
                    .Replace("{:c}", compradorRecipient.Organizacion.Nombre)
                    .Replace("{:i}", codigo);

                foreach (var target in internalRecipients)
                {
                    _data.CorreoMensajes.Add(new CorreoMensaje
                    {
                        AppId = APPID,
                        Estatus = 0,
                        Destinatario = target.Usuario.Email,
                        Asunto = subject,
                        Previa = "Viabilidad de Contratos",
                        Saludo = $"Estimado(a) {target.Usuario.Nombre}",
                        Contenido = content,
                        RegUsuarioId = _access.SessionUserId,
                        ModUsuarioId = _access.SessionUserId
                    });
                }
            }

            // External            
            content = Registro.Aprobacion.HasValue && motivo.Empty() ? specs.RefVal1 : specs.RefVal2;

            content = content
                .Replace("{:t}", tipo.Descripcion)
                .Replace("{:f}", textDate)
                .Replace("{:i}", codigo)
                .Replace("{:m}", motivo)
                .Replace("{:v}", vendedorRecipient.Organizacion.Nombre)
                .Replace("{:c}", compradorRecipient.Organizacion.Nombre.ToLower().StartsWith("i") 
                    ? $"e {compradorRecipient.Organizacion.Nombre}" : $"y {compradorRecipient.Organizacion.Nombre}");

            _data.CorreoMensajes.Add(new CorreoMensaje
            {
                AppId = APPID,
                Estatus = 0,
                Destinatario = vendedorRecipient.Organizacion.ContactoCorreo,
                Asunto = subject,
                Previa = "Viabilidad de Contratos",
                Saludo = "Estimado Agente",
                Contenido = content.Replace("{:p}", compradorRecipient.Organizacion.Nombre),
                RegUsuarioId = _access.SessionUserId,
                ModUsuarioId = _access.SessionUserId
            });

            _data.CorreoMensajes.Add(new CorreoMensaje
            {
                AppId = APPID,
                Estatus = 0,
                Destinatario = compradorRecipient.Organizacion.ContactoCorreo,
                Asunto = subject,
                Previa = "Viabilidad de Contratos",
                Saludo = "Estimado Agente",
                Contenido = content.Replace("{:p}", vendedorRecipient.Organizacion.Nombre),
                RegUsuarioId = _access.SessionUserId,
                ModUsuarioId = _access.SessionUserId
            });

            await _data.SaveChangesAsync();
        }

        private async Task RegionalNotificationsAsync(VCRegional Registro, string Id, DateTime PresetDate, bool SendInternal = false, string AdjuntoPath = "")
        {
            // Get required data
            var solicitanteRecipient = await _data.Agentes.Where(w => w.Id == Registro.SolicitanteId)
                .Include(i => i.Organizacion)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var contraparteRecipient = await _data.AgentesRegionales.Where(w => w.Id == Registro.ContraparteId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var specs = await _data.Catalogos.Where(w => w.Id == Id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var tipo = await _data.Catalogos.Where(w => w.Id == Registro.TipoSolicitudId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            // Presets
            var textDate = $"{PresetDate.Day} de {PresetDate.MonthName()} del {PresetDate.Year}";
            var codigo = $"R-{tipo.RefVal2}-{solicitanteRecipient.Codigo}-{contraparteRecipient.Codigo}-{Registro.Inicia.Year}-{Registro.Id}";
            var subject = specs.RefVal3.Replace("{:i}", codigo);
            var motivo = Registro.MotivoRechazo.Empty() ? "" : Registro.MotivoRechazo;
            var content = "";

            // Internal
            if (SendInternal)
            {
                var internalRecipients = await _data.VCDestinatarios.Where(w => w.NotificacionId == Id)
                    .Include(i => i.Usuario)
                    .AsNoTracking()
                    .ToListAsync();

                content = specs.RefVal1
                    .Replace("{:t}", tipo.Descripcion)
                    .Replace("{:f}", textDate)
                    .Replace("{:s}", solicitanteRecipient.Organizacion.Nombre)
                    .Replace("{:c}", contraparteRecipient.Nombre)
                    .Replace("{:i}", codigo);

                foreach (var target in internalRecipients)
                {
                    _data.CorreoMensajes.Add(new CorreoMensaje
                    {
                        AppId = APPID,
                        Estatus = 0,
                        Destinatario = target.Usuario.Email,
                        Asunto = subject,
                        Previa = "Viabilidad de Contratos",
                        Saludo = $"Estimado(a) {target.Usuario.Nombre}",
                        Contenido = content,
                        RegUsuarioId = _access.SessionUserId,
                        ModUsuarioId = _access.SessionUserId
                    });
                }
            }

            // External            
            content = Registro.Aprobacion.HasValue && motivo.Empty() ? specs.RefVal1 : specs.RefVal2;

            content = content
                .Replace("{:t}", tipo.Descripcion)
                .Replace("{:f}", textDate)
                .Replace("{:i}", codigo)
                .Replace("{:m}", motivo)
                .Replace("{:s}", solicitanteRecipient.Organizacion.Nombre)
                .Replace("{:c}", contraparteRecipient.Nombre.ToLower().StartsWith("i")
                    ? $"e {contraparteRecipient.Nombre}" : $"y {contraparteRecipient.Nombre}");

            if (!AdjuntoPath.Empty()) content += $"<attach>{AdjuntoPath}</attach>";

            _data.CorreoMensajes.Add(new CorreoMensaje
            {
                AppId = APPID,
                Estatus = 0,
                Destinatario = solicitanteRecipient.Organizacion.ContactoCorreo,
                Asunto = subject,
                Previa = "Viabilidad de Contratos",
                Saludo = "Estimado Agente",
                Contenido = content.Replace("{:p}", contraparteRecipient.Nombre),
                RegUsuarioId = _access.SessionUserId,
                ModUsuarioId = _access.SessionUserId
            });

            await _data.SaveChangesAsync();
        }

        private async Task EnmiendaNotificationsAsync(VCEnmienda Registro, VCNacional Contrato, string Id, DateTime PresetDate, bool SendInternal = false)
        {
            // Get required data
            var specs = await _data.Catalogos.Where(w => w.Id == Id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            // Presets
            var textDate = $"{Registro.Inicia.Day} de {Registro.Inicia.MonthName()} del {Registro.Inicia.Year}";
            var codigo = $"E-{Contrato.TipoContrato.RefVal2}-{Contrato.Vendedor.Codigo}-{Contrato.Comprador.Codigo}-{Registro.Inicia.Year}-{Registro.Id}";
            var subject = specs.RefVal3.Replace("{:i}", codigo);
            var motivo = Registro.MotivoRechazo.Empty() ? "" : Registro.MotivoRechazo;
            var content = "";

            // Internal
            if (SendInternal)
            {
                var internalRecipients = await _data.VCDestinatarios.Where(w => w.NotificacionId == Id)
                .Include(i => i.Usuario)
                .AsNoTracking()
                .ToListAsync();

                content = specs.RefVal1
                    .Replace("{:t}", Contrato.TipoContrato.Descripcion)
                    .Replace("{:f}", textDate)
                    .Replace("{:v}", Contrato.Vendedor.Organizacion.Nombre)
                    .Replace("{:c}", Contrato.Comprador.Organizacion.Nombre)
                    .Replace("{:i}", codigo);

                foreach (var target in internalRecipients)
                {
                    _data.CorreoMensajes.Add(new CorreoMensaje
                    {
                        AppId = APPID,
                        Estatus = 0,
                        Destinatario = target.Usuario.Email,
                        Asunto = subject,
                        Previa = "Viabilidad de Contratos",
                        Saludo = $"Estimado(a) {target.Usuario.Nombre}",
                        Contenido = content,
                        RegUsuarioId = _access.SessionUserId,
                        ModUsuarioId = _access.SessionUserId
                    });
                }
            }

            // External            
            content = Registro.Aprobacion.HasValue && motivo.Empty() ? specs.RefVal1 : specs.RefVal2;

            content = content
                .Replace("{:t}", Contrato.TipoContrato.Descripcion)
                .Replace("{:f}", textDate)
                .Replace("{:i}", codigo)
                .Replace("{:m}", motivo)
                .Replace("{:v}", Contrato.Vendedor.Organizacion.Nombre)
                .Replace("{:c}", Contrato.Comprador.Organizacion.Nombre.ToLower().StartsWith("i")
                    ? $"e {Contrato.Comprador.Organizacion.Nombre}" : $"y {Contrato.Comprador.Organizacion.Nombre}");

            _data.CorreoMensajes.Add(new CorreoMensaje
            {
                AppId = APPID,
                Estatus = 0,
                Destinatario = Contrato.Vendedor.Organizacion.ContactoCorreo,
                Asunto = subject,
                Previa = "Viabilidad de Contratos",
                Saludo = "Estimado Agente",
                Contenido = content,
                RegUsuarioId = _access.SessionUserId,
                ModUsuarioId = _access.SessionUserId
            });

            _data.CorreoMensajes.Add(new CorreoMensaje
            {
                AppId = APPID,
                Estatus = 0,
                Destinatario = Contrato.Comprador.Organizacion.ContactoCorreo,
                Asunto = subject,
                Previa = "Viabilidad de Contratos",
                Saludo = "Estimado Agente",
                Contenido = content,
                RegUsuarioId = _access.SessionUserId,
                ModUsuarioId = _access.SessionUserId
            });

            await _data.SaveChangesAsync();
        }
    }
}