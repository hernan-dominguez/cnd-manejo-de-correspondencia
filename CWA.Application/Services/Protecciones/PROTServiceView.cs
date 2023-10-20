using AutoMapper;
using AutoMapper.QueryableExtensions;
using CWA.AccessControl.Constants;
using CWA.AccessControl.Services;
using CWA.Application.Services.Bases;
using CWA.Data;
using CWA.Entities.Bases;
using CWA.Entities.Bases.Protecciones;
using CWA.Entities.Identity;
using CWA.Models.Protecciones.View;
using CWA.Shared.Extensions;
using CWA.Shared.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CWA.Application.Services.Protecciones
{
    public partial class PROTService : BaseService
    {
        protected UserManager<AppUser> _userManager { get; }

        public PROTService(DataContext Data, AppAccessControlService Access, IMapper Mapper, UserManager<AppUser> userManager, ILogger<BaseService> Logger) : base(Data, Access, Mapper, Logger)
        {
            _userManager = userManager;
        }

        public async Task<ViewPack<object>> ViewInfoAsync<K, TData>(int RegistroId, K Id) where K : IEquatable<K> where TData : class, IIdentidad<K>, IPROTAprobacion, IAuditoria
        {
            ViewPack<object> result = new();

            var registro = await _data.PROTRegistros.Where(w => w.Id == RegistroId).OrderBy(o => o.Id)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
            var generales = await _data.PROTGenerales.Where(w => w.Id == RegistroId).OrderBy(o => o.Id)
                .AsSplitQuery()
                .FirstOrDefaultAsync();

            var request = await _data.Set<TData>().Where(w => w.Id.Equals(Id)).FirstOrDefaultAsync();

            result.Add("razonsocial", registro.RazonSocial);
            result.Add("registro", false);
            result.Add("modfecha", request.ModFecha);
            result.Add("fechaaprobacion", request.FechaAprobacion);

            var aprovado = false;
            aprovado = generales.FechaAprobacion != null ? true : false;

            result.Add("generalaprovado", aprovado);

            // For navigation            
            result.Add("registroid", RegistroId);
            result.Add("generalesid", registro.GeneralesId);

            return result;
        }

        public async Task<ViewPack<object>> ViewInfoAsync(int RegistroId)
        {
            ViewPack<object> result = new();

            var registro = await _data.PROTRegistros.Where(w => w.Id == RegistroId).OrderBy(o => o.Id)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
            var usuario = await _userManager.Users.Where(w => w.Id == registro.RegUsuarioId).OrderBy(o => o.Id)
                .FirstOrDefaultAsync();
            var general = await _data.PROTGenerales.Where(w => w.Id == RegistroId).OrderBy(o => o.Id)
                .AsSplitQuery()
                .FirstOrDefaultAsync();

            result.Add("razonsocial", registro.RazonSocial);
            result.Add("registro", true);
            result.Add("fechaaprobacion", registro.FechaAprobacion);
            result.Add("regusuario", usuario.UserName);

            //se extrae el usuario que cargo el archivo para asi sacar el link para descargarlo
            var usuarioreg = await _userManager.Users.Where(w => w.Id == registro.UsuarioId).OrderBy(o => o.Id)
                .FirstOrDefaultAsync();
            result.Add("usuariopath", usuarioreg.UserName.ToUpper() + "\\" + registro.RazonSocial.ToUpper());

            var aprovado = false;
            aprovado = general.FechaAprobacion != null ? true : false;

            result.Add("generalaprovado", aprovado);

            // For navigation
            result.Add("registroid", RegistroId);
            result.Add("generalesid", registro.GeneralesId);

            return result;
        }

        public async Task<List<PROTRegistroView>> ViewRegistrosAsync()
        {
            var query = _data.PROTRegistros.AsQueryable();

            if (!await _access.IsInGroup(new string[] { GroupNames.AdminGroup, GroupNames.CndGroup }))
            {
                var agentes = await _data.PROTRegistros.Where(w => w.UsuarioId == _access.SessionUserId).Select(s => s.UsuarioId).ToListAsync();

                query = query.Where(w => agentes.Contains(w.UsuarioId)).OrderByDescending(o => o.ModFecha);
            }

            return await query.ProjectTo<PROTRegistroView>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
        }

        public async Task<PROTRegistroView> ViewRegistroAsync(int RegistroId)
        {
            var query = await _data.PROTRegistros.Where(w => w.Id == RegistroId)
                .Include(i => i.Tipo)
                .FirstOrDefaultAsync();

            return _mapper.Map<PROTRegistroView>(query);
        }

        public async Task<TModel> ViewDatosAsync<K, TData, TModel>(K Id) where K : IEquatable<K> where TData : class, IIdentidad<K> where TModel : class, new()
        {
            var query = await _data.Set<TData>().Where(w => w.Id.Equals(Id)).FirstOrDefaultAsync();

            return (query is null) ? new TModel() : _mapper.Map<TModel>(query);
        }

        public async Task<List<PROTDocumentosView>> ViewDocumentosAsync(int RegistroId)
        {
            var query = (await _data.PROTDocumentos.Where(w => w.RegistroId == RegistroId && w.Loaded)
                .OrderByDescending(o => o.FechaAprobacion)
                .ThenByDescending(o => o.ModFecha)
                .AsNoTracking()
                .ToListAsync())
                .AsQueryable();

            return query.ProjectTo<PROTDocumentosView>(_mapper.ConfigurationProvider).AsNoTracking().ToList();
        }

        public async Task<List<PROTPlantillasView>> ViewPlantillasAsync()
        {
            var query = (await _data.PROTPlantilla.Where(w => w.Loaded)
                .OrderByDescending(o => o.Id)
                .AsNoTracking()
                .ToListAsync())
                .AsQueryable();

            return query.ProjectTo<PROTPlantillasView>(_mapper.ConfigurationProvider).AsNoTracking().ToList();
        }

        public async Task<PROTDocumentoDownload> ViewDocumentoAsync(string pathUsuario, int RegistroId, int DocumentoId, string RootFolder, string NameContent = "")
        {
            PROTDocumentoDownload download = new() { FileName = "", FilePath = "" };

            var documento = await _data.PROTDocumentos.Where(w => w.RegistroId == RegistroId && w.Id == DocumentoId)
                .Include(i => i.TipoDocumento)
                .FirstOrDefaultAsync();

            if (documento is null) return download;

            // Get the physical file path
            string filePath = Path.Combine(RootFolder, pathUsuario, documento.Archivo);

            if (!File.Exists(filePath)) return download;

            // Set the download properties
            NameContent = (!NameContent.Empty()) ? $" {NameContent}" : NameContent;
            download.FileName = $"{documento.TipoDocumento.Descripcion}{NameContent}{Path.GetExtension(documento.Archivo).ToLower()}";
            download.FilePath = filePath;

            return download;
        }

        public async Task<List<SelectListItem>> ViewDocumentosDisponiblesAsync(int RegistroId)
        {
            var registro = await _data.PROTRegistros.Where(w => w.Id == RegistroId)
                .Include(i => i.Documentos)
                .ThenInclude(i => i.TipoDocumento)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var query = registro.Documentos.Where(w => w.TipoDocumento.RefVal5 == "PROT" /*&& !w.FechaAprobacion.HasValue*/).OrderBy(o => o.Mostrar).AsQueryable();

            return query.ProjectTo<SelectListItem>(_mapper.ConfigurationProvider).ToList();
        }

        public async Task<PROTPlantillaDownload> ViewPlantillaAsync(int DocumentoId, string RootFolder, string NameContent = "")
        {
            PROTPlantillaDownload download = new() { FileName = "", FilePath = "" };

            var documento = await _data.PROTPlantilla.Where(w => w.Id == DocumentoId)
                .Include(i => i.TipoDocumento)
                .FirstOrDefaultAsync();

            if (documento is null) return download;

            // Get the physical file path
            string filePath = Path.Combine(RootFolder, documento.TipoDocumentoId, documento.Archivo);

            if (!File.Exists(filePath)) return download;

            // Set the download properties
            NameContent = (!NameContent.Empty()) ? $" {NameContent}" : NameContent;
            download.FileName = $"{documento.TipoDocumento.Descripcion}{NameContent}{Path.GetExtension(documento.Archivo).ToLower()}";
            download.FilePath = filePath;

            return download;
        }
    }
}
