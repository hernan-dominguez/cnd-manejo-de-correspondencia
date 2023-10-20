using CWA.AccessControl.Constants;
using CWA.Data;
using CWA.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.AccessControl.Services
{
    public class PROTAccessControlService
    {
        private readonly AppAccessControlService _access;
        private readonly DataContext _data;

        public PROTAccessControlService(DataContext Data, AppAccessControlService Access)
        {
            _data = Data;
            _access = Access;
        }

        public async Task<bool> ValidRegistroOwnership(int RegistroId)
        {
            bool allow = false;

            var registro = await _data.PROTRegistros.Where(w => w.Id == RegistroId).FirstOrDefaultAsync();

            if (registro is not null)
            {
                if (await _access.HasClaimAsync(ClaimNames.UserGroup, GroupNames.ExternGroup))
                {
                    allow = registro.UsuarioId == _access.SessionUserId ? true : false;
                }
                else
                {
                    allow = await _access.IsInGroup(new string[] { GroupNames.AdminGroup, GroupNames.CndGroup });
                }
            }

            return allow;
        }

        public async Task<bool> ValidDocumentoOwnership(int UsuarioId)
        {
            bool allow = false;

            var registro = await _data.PROTRegistros.Where(w => w.UsuarioId == UsuarioId).FirstOrDefaultAsync();

            if (registro is not null)
            {
                if (await _access.HasClaimAsync(ClaimNames.UserGroup, GroupNames.ExternGroup))
                {
                    allow = registro.UsuarioId == _access.SessionUserId ? true : false;
                }
                else
                {
                    allow = await _access.IsInGroup(new string[] { GroupNames.AdminGroup, GroupNames.CndGroup });
                }
            }

            return allow;
        }

        public async Task<bool> AllowRegistrarGetPostAsync()
        {
            bool allow = await _access.HasClaimAsync(ClaimNames.PROTCreate);

            return allow;
        }

        public async Task<bool> AllowRegistroGetAsync()
        {
            bool allow = await _access.HasClaimAsync(ClaimNames.PROTView);

            return allow;
        }

        public async Task<bool> AllowRegistroPostAsync(int RegistroId, string HandlerName)
        {
            bool allow = false;

            string claim = HandlerName.ToUpper() == "UPDATE" ? ClaimNames.PROTUpdate : HandlerName.ToUpper() == "APPROVE" ? ClaimNames.PROTEnable : "";

            if (await _access.HasClaimAsync(claim))
            {
                if (claim == ClaimNames.PROTUpdate)
                {
                    allow = await _data.PROTRegistros.Where(w => w.Id == RegistroId).AnyAsync();
                }
                else
                {
                    // ToDo: add code to validate all data completed and approved
                }
            }

            return allow;
        }

        public async Task<bool> AllowGeneralesGetAsync(int RegistroId, int GeneralesId)
        {
            bool allow = false;

            if (await _access.HasClaimAsync(ClaimNames.PROTView)) allow = await _data.PROTRegistros.Where(w => w.Id == RegistroId && w.GeneralesId == GeneralesId).AnyAsync();

            return allow;
        }

        public async Task<bool> AllowGeneralesPostAsync(int RegistroId, int GeneralesId, string HandlerName)
        {
            bool allow = false;

            string claim = HandlerName.ToUpper() == PROTConstants.UPDATE ? ClaimNames.PROTUpdate : HandlerName.ToUpper() == PROTConstants.APPROVE ? ClaimNames.PROTApprove : "";

            if (await _access.HasClaimAsync(claim))
            {
                var registro = await _data.PROTRegistros.Where(w => w.Id == RegistroId && w.GeneralesId == GeneralesId).Include(i => i.Generales).FirstOrDefaultAsync();

                allow = registro is not null && !registro.FechaAprobacion.HasValue && !registro.Generales.FechaAprobacion.HasValue;
            }

            return allow;
        }

        public async Task<bool> AllowDocumentosGetAsync(int RegistroId, int? DocumentoId = null, string HandlerName = "")
        {
            bool allow = false;

            if (await _access.HasClaimAsync(ClaimNames.PROTView))
            {
                if (DocumentoId.HasValue && HandlerName.ToUpper() == PROTConstants.DOWNLOAD)
                {
                    var documento = await _data.PROTDocumentos.Where(w => w.Id == DocumentoId && w.RegistroId == RegistroId)
                        .Include(i => i.Registro)
                        .Include(i => i.TipoDocumento)
                        .FirstOrDefaultAsync();

                    if (documento is not null) allow = (documento.TipoDocumento.RefVal3 == "1");
                }
                else
                {
                    allow = !DocumentoId.HasValue && HandlerName.Empty();
                }
            }

            return allow;
        }

        public async Task<bool> AllowDocumentosPostAsync(int RegistroId, string HandlerName, int? DocumentoId = null)
        {
            bool allow = false;

            string claim = HandlerName.ToUpper() == PROTConstants.UPDATE ? ClaimNames.PROTUpdate : HandlerName.ToUpper() == PROTConstants.APPROVE ? ClaimNames.PROTApprove : "";

            if (await _access.HasClaimAsync(claim))
            {
                if (DocumentoId.HasValue)
                {
                    var documento = await _data.PROTDocumentos.Where(w => w.Id == DocumentoId && w.RegistroId == RegistroId)
                        .Include(i => i.Registro)
                        .Include(i => i.TipoDocumento)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

                    if (documento is not null)
                    {
                        //if (documento.TipoDocumento.RefVal3 == "1")
                        //{
                        //    allow = !documento.Registro.FechaAprobacion.HasValue && !documento.FechaAprobacion.HasValue;
                        //}
                        //else
                        //{
                        //    allow = !documento.Registro.FechaAprobacion.HasValue && !documento.FechaAprobacion.HasValue;
                        //}
                        //se define true siempre ya que el usuario puede actualizar documentos aun despues de aprovados.
                        allow = true;
                    }
                }
                else
                {
                    var registro = await _data.PROTRegistros.Where(w => w.Id == RegistroId)
                        .Include(i => i.Documentos)
                        .ThenInclude(t => t.TipoDocumento)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

                    if (!registro.FechaAprobacion.HasValue)
                    {
                        allow = registro.Documentos.Where(w => w.TipoDocumento.RefVal3 == "1" && !w.FechaAprobacion.HasValue).Any();
                    }
                }
            }

            return allow;
        }

        public async Task<bool> AllowPlantillasGetAsync(int? PlantillaId = null, string HandlerName = "")
        {
            bool allow = false;

            if (await _access.HasClaimAsync(ClaimNames.PROTView))
            {
                if (PlantillaId.HasValue && HandlerName.ToUpper() == PROTConstants.DOWNLOAD)
                {
                    var documento = await _data.PROTPlantilla.Where(w => w.Id == PlantillaId)
                        .Include(i => i.TipoDocumento)
                        .FirstOrDefaultAsync();

                    if (documento is not null) allow = (documento.TipoDocumento.RefVal3 == "1");
                }
                else
                {
                    allow = !PlantillaId.HasValue && HandlerName.Empty();
                }
            }

            return allow;
        }
    }
}
