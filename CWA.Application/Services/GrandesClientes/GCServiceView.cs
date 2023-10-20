using AutoMapper;
using AutoMapper.QueryableExtensions;
using CWA.AccessControl;
using CWA.AccessControl.Authorization.GrandesClientes;
using CWA.AccessControl.Constants;
using CWA.AccessControl.Services;
using CWA.Application.Services.Bases;
using CWA.Data;
using CWA.Entities.Bases;
using CWA.Entities.GrandesClientes;
using CWA.Models.Comun;
using CWA.Models.GrandesClientes.View;
using CWA.Shared.Helpers;
using CWA.Shared.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Application.Services.GrandesClientes
{
    public partial class GCService
    { 
        public async Task<ViewPack<object>> ViewRegistroContextAsync(int RegistroId)
        {
            ViewPack<object> result = new();
                        
            var registro = await _data.GCRegistros.Where(w => w.Id == RegistroId).Include(i => i.Mediciones).FirstOrDefaultAsync();

            result.Add("registro", null);
            result.Add("nombre", registro.Nombre);
            result.Add("id", RegistroId);
            result.Add("generales", registro.GeneralesId);

            if (registro.FechaAtencion.HasValue)
            {
                result.Add("autorizado", registro.FechaAtencion);
            }

            if (registro.EstatusId == GCConstants.REQUISITOS || registro.EstatusId == GCConstants.AUTORIZADO)
            {
                if (registro.TipoGranClienteId == ACTIVO) result.Add("comercial", registro.ComercialId);                
                if (registro.Mediciones.Where(w => w.MedidorId.HasValue).Count() > 0) result.Add("medidores", null);
            }

            return result;
        }

        public async Task<ViewPack<object>> ViewRegistroContextAsync<K, TEntity>(int RegistroId, K ItemId) where K : IEquatable<K> where TEntity : class, IIdentidad<K>, IAtencion, IAuditoria
        {
            ViewPack<object> result = new();
                        
            var registro = await _data.GCRegistros.Where(w => w.Id == RegistroId).Include(i => i.Mediciones).FirstOrDefaultAsync();

            // Registro
            result.Add("item", null);
            result.Add("id", RegistroId);
            result.Add("nombre", registro.Nombre);
            result.Add("generales", registro.GeneralesId);

            if (registro.EstatusId == GCConstants.REQUISITOS || registro.EstatusId == GCConstants.AUTORIZADO)
            {
                if (registro.TipoGranClienteId == ACTIVO) result.Add("comercial", registro.ComercialId);                
                if (registro.Mediciones.Where(w => w.MedidorId.HasValue).Count() > 0) result.Add("medidores", null);
            }

            // Item
            var item = await _data.Set<TEntity>().Where(w => w.Id.Equals(ItemId)).FirstOrDefaultAsync();

            result.Add("modfecha", item.ModFecha);
            if (item.FechaAtencion.HasValue) result.Add("aprobado", item.FechaAtencion);

            if (item is GCMedidorDist)
            {                
                var medicion = await _data.GCMediciones.Where(w => w.MedidorId.HasValue && w.MedidorId.Equals(ItemId)).FirstOrDefaultAsync();
                result.Add("serie", medicion.Serie);
                result.Add("dist", medicion.Distribuidora.Codigo);
            }

            return result;
        }

        public async Task<List<GCRegistroView>> ViewRegistrosAsync()
        {
            var query = _data.GCRegistros.AsQueryable();
            
            if (!(await _access.IsInGroup(new[] { GroupNames.AdminGroup, GroupNames.CndGroup })))
            {
                // Lookup organization
                var lookup = await _data.UsuariosOrganizaciones.Where(w => w.UsuarioId == _access.SessionUserId)
                    .Include(i => i.Organizacion)
                    .ThenInclude(t => t.Agente)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (lookup is null)
                {
                    // Propietario
                    query = query.Where(w => w.PropietarioId == _access.SessionUserId);
                }
                else
                {
                    // Agente
                    query = query.Where(w => w.ResponsableId == lookup.Organizacion.Agente.Id);
                }
            }            

            return await query.ProjectTo<GCRegistroView>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
        }

        public async Task<GCRegistroView> ViewRegistroAsync(int RegistroId)
        {
            var query = await _data.GCRegistros.Where(w => w.Id == RegistroId)
                .Include(i => i.TipoGranCliente)
                .Include(i => i.Responsable)
                .Include(i => i.Propietario)
                .FirstOrDefaultAsync();

            return _mapper.Map<GCRegistroView>(query);
        }

        public async Task<TModel> ViewDatosAsync<K, TEntity, TModel>(K Id) where K : IEquatable<K> where TEntity : class, IIdentidad<K> where TModel : class, new()
        {
            var query = await _data.Set<TEntity>().Where(w => w.Id.Equals(Id)).FirstOrDefaultAsync();

            return (query is null) ? new TModel() : _mapper.Map<TModel>(query);
        }

        public async Task<List<GCMedicionesView>> ViewMedicionesAsync(int RegistroId)
        {
            var query = _data.GCMediciones.Where(w => w.RegistroId == RegistroId)
                .Include(i => i.Distribuidora)
                .Include(i => i.TipoConexion)
                .OrderBy(o => o.TipoConexionId)
                .ThenBy(t => t.Serie)
                .AsQueryable();

            return await query.ProjectTo<GCMedicionesView>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
        }

        public async Task<List<GCMedidoresDistView>> ViewMedidoresDistAsync(int RegistroId)
        {
            var query = (await _data.GCMediciones.Where(w => w.RegistroId == RegistroId && w.MedidorId.HasValue)
                .Include(i => i.Distribuidora)
                .Include(i => i.Medidor)
                .ToListAsync())
                .AsQueryable();

            return query.ProjectTo<GCMedidoresDistView>(_mapper.ConfigurationProvider).AsNoTracking().ToList();
        }

        public async Task<List<GCDocumentosView>> ViewDocumentosAsync(int RegistroId)
        {
            var query = (await _data.GCDocumentos.Where(w => w.RegistroId == RegistroId && w.Loaded)
                .OrderByDescending(o => o.FechaAtencion)
                .ThenByDescending(o => o.ModFecha)
                .AsNoTracking()
                .ToListAsync())
                .AsQueryable();

            return query.ProjectTo<GCDocumentosView>(_mapper.ConfigurationProvider).AsNoTracking().ToList();
        }

        public async Task<GCDocumentoDownload> ViewDocumentoAsync(int RegistroId, int DocumentoId, string RootFolder, string NameContent = "")
        {
            GCDocumentoDownload download = new() { FileName = "", FilePath = "" };

            var documento = await _data.GCDocumentos.Where(w => w.RegistroId == RegistroId && w.Id == DocumentoId)
                .Include(i => i.Registro)
                .Include(i => i.TipoDocumento)
                .FirstOrDefaultAsync();

            if (documento is null) return download;

            // Get the physical file path
            string filePath = Path.Combine(RootFolder, $"{documento.Registro.RegFecha:yyy}", $"GC{RegistroId}", documento.Archivo);

            if (!File.Exists(filePath)) return download;

            // Set the download properties
            NameContent = (!NameContent.Empty()) ? $" {NameContent}" : NameContent;
            download.FileName = $"{documento.TipoDocumento.Descripcion}{NameContent}{Path.GetExtension(documento.Archivo).ToLower()}";
            download.FilePath = filePath;

            return download;
        }

        public async Task<List<GCDocumentoList>> ViewDocumentosDisponiblesAsync(int RegistroId)
        {
            var registro = await _data.GCRegistros.Where(w => w.Id == RegistroId)
                .Include(i => i.Documentos)
                .ThenInclude(i => i.TipoDocumento)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (registro.EstatusId == GCConstants.INICIAL || registro.EstatusId == GCConstants.FECHA)
            {
                var query = registro.Documentos.Where(w => w.TipoDocumento.RefVal3 == "1" && !w.FechaAtencion.HasValue).OrderBy(o => o.Mostrar).AsQueryable();

                return query.ProjectTo<GCDocumentoList>(_mapper.ConfigurationProvider).ToList();
            }
            else
            {
                var query = registro.Documentos.Where(w => w.TipoDocumento.RefVal3 == "2" && !w.FechaAtencion.HasValue).OrderBy(o => o.Mostrar).AsQueryable();

                return query.ProjectTo<GCDocumentoList>(_mapper.ConfigurationProvider).ToList();
            }
        }

        public async Task<GCDocumentoDownload> ViewTemplateAsync(int RegistroId, string DocumentoTipoId, string TemplateFolder)
        {
            GCDocumentoDownload download = new() { FileName = "", FilePath = "" };

            var template = await _data.Catalogos.Where(w => w.Grupo == "GCDOCUMENTO" && w.Id == DocumentoTipoId).FirstOrDefaultAsync();
            var registro = await _data.GCRegistros.Where(w => w.Id == RegistroId).FirstOrDefaultAsync();

            if (template is null) return download;

            // Get the physical file path
            string filePath = Path.Combine(TemplateFolder, $"{registro.RegFecha:yyy}", $"GC{RegistroId}", $"{template.RefVal2}.{template.RefVal1}");

            if (!File.Exists(filePath)) return download;

            // Set the download properties
            download.FileName = $"{template.Descripcion}.{template.RefVal1}";
            download.FilePath = filePath;

            return download;
        }
    }
}
