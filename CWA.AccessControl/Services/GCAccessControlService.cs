using CWA.AccessControl.Authorization.GrandesClientes;
using CWA.AccessControl.Constants;
using CWA.Data;
using CWA.Entities.Bases;
using CWA.Entities.Bases.GrandesClientes;
using CWA.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.AccessControl.Services
{
    public class GCAccessControlService
    {
        private readonly AppAccessControlService _access;
        private readonly DataContext _data;

        public GCAccessControlService(DataContext Data, AppAccessControlService Access)
        {
            _data = Data;
            _access = Access;
        }

        public async Task<bool> ValidRegistroOwnership(int RegistroId)
        {
            bool allow = false;

            var registro = await _data.GCRegistros.Where(w => w.Id == RegistroId).FirstOrDefaultAsync();

            if (registro is not null)
            {
                // Check integrity
                if ((registro.PropietarioId.HasValue && registro.ResponsableId.HasValue) || (!registro.PropietarioId.HasValue && !registro.ResponsableId.HasValue)) return false;

                // Lookup owner
                if (await _access.HasClaimAsync(ClaimNames.UserGroup, GroupNames.ExternGroup))
                {
                    if (registro.PropietarioId.HasValue)
                    {
                        // Propietario
                        allow = registro.PropietarioId == _access.SessionUserId;
                    }
                    else 
                    {
                        // Agente
                        var lookup = await _data.UsuariosOrganizaciones
                            .Include(i => i.Organizacion)
                            .ThenInclude(t => t.Agente)
                            .Where(w => w.UsuarioId == _access.SessionUserId && (w.Organizacion != null && w.Organizacion.Agente.Id == registro.ResponsableId))
                            .FirstOrDefaultAsync();

                        allow = lookup is not null;
                    }
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
            bool allow = await _access.HasClaimAsync(ClaimNames.GCCreate);

            return allow;
        }

        public async Task<bool> AllowRegistroGetAsync()
        {
            bool allow = await _access.HasClaimAsync(ClaimNames.GCView);

            return allow;
        }

        public async Task<bool> AllowRegistroPostAsync(int RegistroId, string HandlerName)
        {
            bool allow = false;

            string claim = HandlerName.ToUpper() == "UPDATE" ? ClaimNames.GCUpdate : HandlerName.ToUpper() == "NOTIFY" ? ClaimNames.GCEnable : "";

            if (await _access.HasClaimAsync(claim))
            {
                if (claim == ClaimNames.GCUpdate)
                {
                    allow = await _data.GCRegistros.Where(w => w.Id == RegistroId && w.FechaEditable).AnyAsync();
                }
                else
                {
                    allow = await _data.GCRegistros.Where(w => w.Id == RegistroId && w.FechaAtencion.HasValue && w.EstatusId == GCConstants.AUTORIZADO).AnyAsync();
                }
            }

            return allow;
        }

        public async Task<bool> AllowGeneralesGetAsync(int RegistroId, int GeneralesId)
        {
            bool allow = false;

            if (await _access.HasClaimAsync(ClaimNames.GCView)) allow = await _data.GCRegistros.Where(w => w.Id == RegistroId && w.GeneralesId == GeneralesId).AnyAsync();

            return allow;
        }

        public async Task<bool> AllowGeneralesPostAsync(int RegistroId, int GeneralesId, string HandlerName)
        {
            bool allow = false;

            string claim = HandlerName.ToUpper() == GCConstants.UPDATE ? ClaimNames.GCUpdate : HandlerName.ToUpper() == GCConstants.APPROVE ? ClaimNames.GCApprove : "";

            if (await _access.HasClaimAsync(claim))
            {
                var registro = await _data.GCRegistros.Where(w => w.Id == RegistroId && w.GeneralesId == GeneralesId).Include(i => i.Generales).FirstOrDefaultAsync();

                allow = registro is not null && !registro.FechaAtencion.HasValue && registro.EstatusId == GCConstants.INICIAL && !registro.Generales.FechaAtencion.HasValue;
            }

            return allow;
        }

        public async Task<bool> AllowComercialGetAsync(int RegistroId, int ComercialId)
        {
            bool allow = false;

            if (await _access.HasClaimAsync(ClaimNames.GCView))
            {
                var registro = await _data.GCRegistros.Where(w => w.Id == RegistroId && w.ComercialId == ComercialId).FirstOrDefaultAsync();

                allow = registro is not null && (registro.EstatusId == GCConstants.REQUISITOS || registro.EstatusId == GCConstants.AUTORIZADO);
            }

            return allow;
        }

        public async Task<bool> AllowComercialPostAsync(int RegistroId, int ComercialId, string HandlerName)
        {
            bool allow = false;

            var registro = await _data.GCRegistros.Where(w => w.Id == RegistroId && w.ComercialId == ComercialId).Include(i => i.Comercial).FirstOrDefaultAsync();

            if (HandlerName.ToUpper() == GCConstants.UPDATE)
            {
                if (await _access.HasClaimAsync(ClaimNames.GCUpdate) || await _access.HasClaimAsync(ClaimNames.GCApprove))
                {
                    allow = registro is not null && !registro.FechaAtencion.HasValue && registro.EstatusId == GCConstants.REQUISITOS && !registro.Comercial.FechaAtencion.HasValue;
                }
            }

            if (HandlerName.ToUpper() == GCConstants.APPROVE)
            {
                if (await _access.HasClaimAsync(ClaimNames.GCApprove))
                {
                    allow = registro is not null && !registro.FechaAtencion.HasValue && registro.EstatusId == GCConstants.REQUISITOS && !registro.Comercial.FechaAtencion.HasValue;
                }
            }

            return allow;
        }

        public async Task<bool> AllowMedicionesGetAsync()
        {
            bool allow = await _access.HasClaimAsync(ClaimNames.GCView);

            return allow;
        }
        
        public async Task<bool> AllowMedicionGetAsync(int RegistroId, int? MedicionId = null)
        {
            bool allow = false;

            if (!MedicionId.HasValue)
            {
                if (await _access.HasClaimAsync(ClaimNames.GCUpdate))
                {
                    var registro = await _data.GCRegistros.Where(w => w.Id == RegistroId).FirstOrDefaultAsync();

                    allow = !registro.FechaAtencion.HasValue && registro.EstatusId == GCConstants.INICIAL;
                }
            }
            else
            {
                if (await _access.HasClaimAsync(ClaimNames.GCView)) allow = await _data.GCMediciones.Where(w => w.Id == MedicionId.Value && w.RegistroId == RegistroId).AnyAsync();
            }

            return allow;
        }

        public async Task<bool> AllowMedicionPostAsync(int RegistroId, int? MedicionId = null, string HandlerName = "")
        {
            bool allow = false;

            if (!MedicionId.HasValue)
            {
                if (await _access.HasClaimAsync(ClaimNames.GCUpdate))
                {
                    var registro = await _data.GCRegistros.Where(w => w.Id == RegistroId).FirstOrDefaultAsync();

                    allow = !registro.FechaAtencion.HasValue && registro.EstatusId == GCConstants.INICIAL;
                }
            }
            else
            {
                string claim = HandlerName.ToUpper() == GCConstants.UPDATE ? ClaimNames.GCUpdate : HandlerName.ToUpper() == GCConstants.APPROVE ? ClaimNames.GCApprove : "";

                if (await _access.HasClaimAsync(claim))
                {
                    var medicion = await _data.GCMediciones.Where(w => w.Id == MedicionId.Value && w.RegistroId == RegistroId).Include(i => i.Registro).FirstOrDefaultAsync();

                    allow = medicion is not null && !medicion.Registro.FechaAtencion.HasValue
                        && medicion.Registro.EstatusId == GCConstants.INICIAL && !medicion.FechaAtencion.HasValue;
                }
            }

            return allow;
        }

        public async Task<bool> AllowMedidoresDistGetAsync(int RegistroId)
        {
            bool allow = false;

            if (await _access.HasClaimAsync(ClaimNames.GCView))
            {
                var registro = await _data.GCRegistros.Where(w => w.Id == RegistroId).Include(i => i.Mediciones).FirstOrDefaultAsync();

                allow = registro.Mediciones.Where(w => w.MedidorId.HasValue).ToList().Count > 0 && (registro.EstatusId == GCConstants.REQUISITOS || registro.EstatusId == GCConstants.AUTORIZADO);
            }

            return allow;
        }

        public async Task<bool> AllowMedidorDistGetAsync(int RegistroId, int MedidorId)
        {
            bool allow = false;

            if (await _access.HasClaimAsync(ClaimNames.GCView))
            {                
                var medicion = await _data.GCMediciones.Where(w => w.RegistroId == RegistroId && w.MedidorId.HasValue && w.MedidorId == MedidorId)
                    .Include(i => i.Registro)
                    .FirstOrDefaultAsync();

                allow = medicion is not null && (medicion.Registro.EstatusId == GCConstants.REQUISITOS || medicion.Registro.EstatusId == GCConstants.AUTORIZADO);
            }

            return allow;
        }

        public async Task<bool> AllowMedidorDistPostAsync(int RegistroId, int MedidorId, string HandlerName)
        {
            bool allow = false;

            string claim = HandlerName.ToUpper() == GCConstants.UPDATE ? ClaimNames.GCUpdate : HandlerName.ToUpper() == GCConstants.APPROVE ? ClaimNames.GCApprove : "";

            if (await _access.HasClaimAsync(claim))
            {
                var medicion = await _data.GCMediciones.Where(w => w.RegistroId == RegistroId && w.MedidorId.HasValue && w.MedidorId == MedidorId)
                    .Include(i => i.Registro)
                    .Include(i => i.Medidor)
                    .FirstOrDefaultAsync();

                allow = medicion is not null && !medicion.Registro.FechaAtencion.HasValue
                    && medicion.Registro.EstatusId == GCConstants.REQUISITOS && !medicion.Medidor.FechaAtencion.HasValue;
            }

            return allow;
        }

        public async Task<bool> AllowDocumentosGetAsync(int RegistroId, int? DocumentoId = null, string HandlerName = "")
        {
            bool allow = false;

            if (await _access.HasClaimAsync(ClaimNames.GCView))
            {
                if (DocumentoId.HasValue && HandlerName.ToUpper() == GCConstants.DOWNLOAD)
                {
                    var documento = await _data.GCDocumentos.Where(w => w.Id == DocumentoId && w.RegistroId == RegistroId)
                        .Include(i => i.Registro)
                        .Include(i => i.TipoDocumento)
                        .FirstOrDefaultAsync();

                    if (documento is not null) allow = (documento.Registro.EstatusId == GCConstants.INICIAL && documento.TipoDocumento.RefVal3 == "1")
                            || documento.Registro.EstatusId == GCConstants.REQUISITOS
                            || documento.Registro.EstatusId == GCConstants.AUTORIZADO;
                }
                else
                {
                    allow = !DocumentoId.HasValue && (HandlerName.ToUpper() == GCConstants.TEMPLATE || HandlerName.Empty());
                }
            }

            return allow;
        }

        public async Task<bool> AllowDocumentosPostAsync(int RegistroId, string HandlerName, int? DocumentoId = null)
        {
            bool allow = false;

            string claim = HandlerName.ToUpper() == GCConstants.UPDATE ? ClaimNames.GCUpdate : HandlerName.ToUpper() == GCConstants.APPROVE ? ClaimNames.GCApprove : "";

            if (await _access.HasClaimAsync(claim))
            {
                if (DocumentoId.HasValue)
                {
                    var documento = await _data.GCDocumentos.Where(w => w.Id == DocumentoId && w.RegistroId == RegistroId)
                        .Include(i => i.Registro)
                        .Include(i => i.TipoDocumento)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

                    if (documento is not null)
                    {
                        if (documento.TipoDocumento.RefVal3 == "1")
                        {
                            allow = !documento.Registro.FechaAtencion.HasValue && documento.Registro.EstatusId == GCConstants.INICIAL && !documento.FechaAtencion.HasValue;
                        }
                        else
                        {
                            allow = !documento.Registro.FechaAtencion.HasValue && documento.Registro.EstatusId == GCConstants.REQUISITOS && !documento.FechaAtencion.HasValue;
                        }
                    }
                }
                else
                {
                    var registro = await _data.GCRegistros.Where(w => w.Id == RegistroId)
                        .Include(i => i.Documentos)
                        .ThenInclude(t => t.TipoDocumento)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

                    if (!registro.FechaAtencion.HasValue)
                    {
                        if (registro.EstatusId == GCConstants.INICIAL) allow = registro.Documentos.Where(w => w.TipoDocumento.RefVal3 == "1" && !w.FechaAtencion.HasValue).Any();

                        if (registro.EstatusId == GCConstants.REQUISITOS) allow = registro.Documentos.Where(w => w.TipoDocumento.RefVal3 == "2" && !w.FechaAtencion.HasValue).Any();
                    }
                }
            }

            return allow;
        }
    }
}
