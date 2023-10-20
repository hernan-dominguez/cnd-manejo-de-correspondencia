using AutoMapper.QueryableExtensions;
using CWA.AccessControl.Constants;
using CWA.Entities.Bases;
using CWA.Entities.Bases.ViabilidadContratos;
using CWA.Entities.ViabilidadContratos;
using CWA.Models.ViabilidadContratos.Edit;
using CWA.Models.ViabilidadContratos.View;
using CWA.Shared.Extensions;
using CWA.Shared.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Application.Services.ViabilidadContratos
{
    public partial class VCService
    {
        public async Task<ViewPack<object>> ViewRegistroContext<TEntity>(int? RegistroId = null) where TEntity : class, IIdentidad<int>, IAtencion
        {
            ViewPack<object> result = new();

            // Get Registro data if required
            if (RegistroId.HasValue)
            {
                var registro = await _data.Set<TEntity>().Where(w => w.Id.Equals(RegistroId.Value))
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (registro is not null && registro.FechaAtencion.HasValue)
                {
                    result.Add("fecha", registro.FechaAtencion.Value);
                    result.Add("aprobacion", 
                        (registro is VCNacional) ? (registro as VCNacional).Aprobacion.Value 
                        : (registro is VCRegional) ? (registro as VCRegional).Aprobacion.Value 
                        : (registro as VCEnmienda).Aprobacion.Value);
                }
            }

            // Get Organizacion data
            var org = await _data.UsuariosOrganizaciones.Where(w => w.UsuarioId == _access.SessionUserId)
                .Include(i => i.Organizacion)
                .ThenInclude(t => t.Agente)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (org.Organizacion is not null && org.Organizacion.Agente is not null)
            {
                result.Add("org-id", org.Organizacion.Id);
                result.Add("org-nombre", org.Organizacion.Nombre);
                result.Add("agt-id", org.Organizacion.Agente.Id);
                result.Add("agt-tipo", org.Organizacion.Agente.TipoAgenteId);
                result.Add("agt-codigo", org.Organizacion.Agente.Codigo);
                result.Add("regionales", org.Organizacion.Agente.TipoAgenteId == "TAG01");
            }
            else
            {
                result.Add("org-id", 0);
                result.Add("org-nombre", "");
                result.Add("agt-id", 0);
                result.Add("agt-tipo", "");
                result.Add("agt-codigo", "");
                result.Add("regionales", await _access.IsInGroup(new[] { GroupNames.AdminGroup, GroupNames.CndGroup }));
            }

            return result;
        }

        public async Task<List<VCNacionalView>> ViewNacionalesAsync(int? AgenteId = null, bool? Aprobados = false)
        {
            var query = _data.VCNacionales.AsQueryable();

            if (!(await _access.IsInGroup(new string[] { GroupNames.AdminGroup, GroupNames.CndGroup })) && AgenteId.HasValue)
            {
                query = query.Where(w => w.VendedorId == AgenteId.Value || w.CompradorId == AgenteId.Value);                
            }

            if (Aprobados.HasValue && Aprobados.Value)
            {
                query = query.Where(w => w.Aprobacion.HasValue && w.Aprobacion.Value);
            }

            return await query.ProjectTo<VCNacionalView>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
        }

        public async Task<VCNacionalView> ViewNacionalAsync(int Id)
        {
            var lookup = await _data.VCNacionales
                .Where(w => w.Id == Id)
                .Include(i => i.Comprador)
                .ThenInclude(t => t.Organizacion)
                .Include(i => i.Vendedor)
                .ThenInclude(i => i.Organizacion)
                .Include(i => i.Documentos)
                .ThenInclude(t => t.TipoDocumento)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return (lookup is null) ? new() : _mapper.Map<VCNacionalView>(lookup);
        }

        public async Task<List<VCRegionalView>> ViewRegionalesAsync(int? AgenteId = null)
        {
            var query = _data.VCRegionales.AsQueryable();

            if (!(await _access.IsInGroup(new string[] { GroupNames.AdminGroup, GroupNames.CndGroup })))
            {
                query = query.Where(w => w.SolicitanteId == AgenteId);
            }

            return await query.ProjectTo<VCRegionalView>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
        }

        public async Task<VCRegionalView> ViewRegionalAsync(int Id)
        {
            var lookup = await _data.VCRegionales
                .Where(w => w.Id == Id)
                .Include(i => i.Solicitante)
                .ThenInclude(t => t.Organizacion)
                .Include(i => i.Documentos)
                .ThenInclude(t => t.TipoDocumento)
                .Include(i => i.Contraparte)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return (lookup is null) ? new() : _mapper.Map<VCRegionalView>(lookup);
        }

        public async Task<List<VCEnmiendaView>> ViewEnmiendasAsync(int? AgenteId = null)
        {
            var query = _data.VCEnmiendas.AsQueryable();

            if (!(await _access.IsInGroup(new string[] { GroupNames.AdminGroup, GroupNames.CndGroup })) && AgenteId.HasValue)
            {
                query = query.Include(i => i.Contrato);
                query = query.Where(w => w.Contrato.VendedorId == AgenteId.Value || w.Contrato.CompradorId == AgenteId.Value);
            }

            return await query.ProjectTo<VCEnmiendaView>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
        }

        public async Task<VCEnmiendaView> ViewEnmiendaAsync(int Id)
        {
            var lookup = await _data.VCEnmiendas
                .Where(w => w.Id == Id)
                .Include(i => i.Documentos)
                .ThenInclude(t => t.TipoDocumento)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var contrato = await _data.VCNacionales
                .Where(w => w.Id == lookup.ContratoId)
                .Include(i => i.Comprador)
                .ThenInclude(t => t.Organizacion)
                .Include(i => i.Vendedor)
                .ThenInclude(t => t.Organizacion)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            lookup.Contrato = contrato;

            return (lookup is null) ? new() : _mapper.Map<VCEnmiendaView>(lookup);
        }

        public async Task<VCDocumentoDownload> ViewDocumentoAsync<TDocumento, TRegistro>(int RegistroId, int DocumentoId, string RootFolder, string NameContent = "") 
            where TDocumento : class, IIdentidad<int>, IVCDocumento<TRegistro>
            where TRegistro : class, IAuditoria
        {
            VCDocumentoDownload download = new() { FileName = "", FilePath = "" };

            var documento = await _data.Set<TDocumento>().Where(w => w.RegistroId == RegistroId && w.Id.Equals(DocumentoId))
                .Include(i => i.TipoDocumento)
                .AsNoTracking()
                .FirstOrDefaultAsync();            

            if (documento is null) return download;

            // Get the physical file path
            string filePath = Path.Combine(RootFolder, documento.Archivo);

            if (!File.Exists(filePath)) return download;

            // Set the download properties
            NameContent = (!NameContent.Empty()) ? $" {NameContent}" : NameContent;
            download.FileName = $"{documento.TipoDocumento.Descripcion}{NameContent}{Path.GetExtension(documento.Archivo).ToLower()}";
            download.FilePath = filePath;

            return download;
        }
    }
}
