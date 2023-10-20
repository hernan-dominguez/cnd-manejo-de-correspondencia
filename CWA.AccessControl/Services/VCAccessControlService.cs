using CWA.AccessControl.Constants;
using CWA.Data;
using CWA.Entities.Bases;
using CWA.Entities.Bases.ViabilidadContratos;
using CWA.Entities.ViabilidadContratos;
using CWA.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.AccessControl.Services
{
    public class VCAccessControlService
    {
        private readonly AppAccessControlService _access;
        private readonly DataContext _data;

        public VCAccessControlService(DataContext Data, AppAccessControlService Access)
        {
            _data = Data;
            _access = Access;
        }

        public async Task<bool> AllowRegionalesGetAsync()
        {
            bool allow;

            if (!await _access.IsInGroup(new[] { GroupNames.AdminGroup, GroupNames.CndGroup }))
            {
                var organizacion = await _access.GetOrganizacion();
                allow = organizacion.AgenteId is not null && organizacion.TipoAgenteId == "TAG01";
            }
            else
            {
                allow = true;
            }

            return allow;
        }

        public async Task<bool> AllowRegistroGetAsync(string RequirementName, int? RegistroId = null)
        {
            bool allow = false;

            if (!RegistroId.HasValue)
            {
                allow = await _access.HasClaimAsync(ClaimNames.VCUpdate);
            }
            else
            {
                if (await _access.HasClaimAsync(ClaimNames.VCView))
                {
                    switch (RequirementName)
                    {
                        case VCConstants.NACIONAL:
                            allow = await AllowNacionalGetAsync(RegistroId.Value);
                            break;

                        case VCConstants.REGIONAL:
                            allow = await AllowRegionalGetAsync(RegistroId.Value);
                            break;

                        case VCConstants.ENMIENDA:
                            allow = await AllowEnmiendaGetAsync(RegistroId.Value);
                            break;
                    }
                }
            }

            return allow;
        }

        private async Task<bool> AllowNacionalGetAsync(int Id)
        {
            var registro = await _data.VCNacionales.Where(w => w.Id == Id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var ownership = false;

            if (registro is not null)
            {
                if (!(await _access.IsInGroup(new[] { GroupNames.AdminGroup, GroupNames.CndGroup })))
                {
                    var organizacion = await _access.GetOrganizacion();
                    ownership = organizacion.AgenteId.HasValue && (registro.VendedorId == organizacion.AgenteId.Value || registro.CompradorId == organizacion.AgenteId);
                }
                else
                {
                    ownership = true;
                }
            }

            return ownership;
        }

        private async Task<bool> AllowRegionalGetAsync(int Id)
        {
            var registro = await _data.VCRegionales.Where(w => w.Id == Id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var ownership = false;

            if (registro is not null)
            {
                if (!(await _access.IsInGroup(new[] { GroupNames.AdminGroup, GroupNames.CndGroup })))
                {
                    Console.WriteLine("Valuating");
                    var organizacion = await _access.GetOrganizacion();
                    ownership = organizacion.AgenteId.HasValue && registro.SolicitanteId == organizacion.AgenteId.Value ;
                }
                else
                {
                    ownership = true;
                }
            }

            return ownership;
        }

        private async Task<bool> AllowEnmiendaGetAsync(int Id)
        {
            var registro = await _data.VCEnmiendas.Where(w => w.Id == Id)
                .Include(i => i.Contrato)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var ownership = false;

            if (registro is not null)
            {
                if (!(await _access.IsInGroup(new[] { GroupNames.AdminGroup, GroupNames.CndGroup })))
                {
                    var organizacion = await _access.GetOrganizacion();
                    ownership = organizacion.AgenteId.HasValue && (registro.Contrato.VendedorId == organizacion.AgenteId.Value || registro.Contrato.CompradorId == organizacion.AgenteId);
                }
                else
                {
                    ownership = true;
                }
            }

            return ownership;
        }

        public async Task<bool> AllowDocumentoGetAsync(string RequirementName, int RegistroId, int? DocumentoId, string HandlerName)
        {
            bool allow = false;

            if (await _access.HasClaimAsync(ClaimNames.VCView))
            {
                if (DocumentoId.HasValue && HandlerName.ToUpper() == VCConstants.DOWNLOAD)
                {
                    switch (RequirementName)
                    {
                        case VCConstants.NACIONAL:
                            allow = await AllowDocNacionalGetAsync(RegistroId, DocumentoId.Value);
                            break;

                        case VCConstants.REGIONAL:
                            allow = await AllowDocRegionalGetAsync(RegistroId, DocumentoId.Value);
                            break;

                        case VCConstants.ENMIENDA:
                            allow = await AllowDocEnmiendaGetAsync(RegistroId, DocumentoId.Value);
                            break;
                    }
                }
            }

            return allow;
        }

        private async Task<bool> AllowDocNacionalGetAsync(int RegistroId, int Id)
        {
            var documento = await _data.VCDocsNacionales.Where(w => w.RegistroId == RegistroId && w.Id == Id)
                .Include(i => i.Registro)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            bool ownership = false;

            if (documento is not null)
            {
                if (!(await _access.IsInGroup(new[] { GroupNames.AdminGroup, GroupNames.CndGroup })))
                {
                    var organizacion = await _access.GetOrganizacion();
                    ownership = organizacion.AgenteId.HasValue && (documento.Registro.VendedorId == organizacion.AgenteId.Value || documento.Registro.CompradorId == organizacion.AgenteId);
                }
                else
                {
                    ownership = true;
                }
            }

            return ownership;
        }

        private async Task<bool> AllowDocRegionalGetAsync(int RegistroId, int Id)
        {
            var documento = await _data.VCDocsRegionales.Where(w => w.RegistroId == RegistroId && w.Id == Id)
                .Include(i => i.Registro)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            bool ownership = false;

            if (documento is not null)
            {
                if (!(await _access.IsInGroup(new[] { GroupNames.AdminGroup, GroupNames.CndGroup })))
                {
                    var organizacion = await _access.GetOrganizacion();
                    ownership = organizacion.AgenteId.HasValue && documento.Registro.SolicitanteId == organizacion.AgenteId.Value;
                }
                else
                {
                    ownership = true;
                }
            }

            return ownership;
        }

        private async Task<bool> AllowDocEnmiendaGetAsync(int RegistroId, int Id)
        {
            var documento = await _data.VCDocsEnmiendas.Where(w => w.RegistroId == RegistroId && w.Id == Id)
                .Include(i => i.Registro)
                .ThenInclude(t => t.Contrato)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            bool ownership = false;

            if (documento is not null)
            {
                if (!(await _access.IsInGroup(new[] { GroupNames.AdminGroup, GroupNames.CndGroup })))
                {
                    var organizacion = await _access.GetOrganizacion();
                    ownership = organizacion.AgenteId.HasValue && (documento.Registro.Contrato.VendedorId == organizacion.AgenteId.Value || documento.Registro.Contrato.CompradorId == organizacion.AgenteId);
                }
                else
                {
                    ownership = true;
                }
            }

            return ownership;
        }

        public async Task<bool> AllowRegistroPostAsync(string RequirementName, int? RegistroId = null, string HandlerName = "")
        {
            bool allow = false;

            if (!RegistroId.HasValue)
            {
                allow = await _access.HasClaimAsync(ClaimNames.VCUpdate);
            }
            else
            {
                if (await _access.HasClaimAsync(ClaimNames.VCApprove) && HandlerName.ToUpper() == VCConstants.APPROVE || HandlerName.ToUpper() == VCConstants.REJECT)
                {
                    switch (RequirementName)
                    {
                        case VCConstants.NACIONAL:
                            allow = await AllowAnyPostAsync<VCNacional>(RegistroId.Value);
                            break;

                        case VCConstants.REGIONAL:
                            allow = await AllowAnyPostAsync<VCRegional>(RegistroId.Value);
                            break;

                        case VCConstants.ENMIENDA:
                            allow = await AllowAnyPostAsync<VCEnmienda>(RegistroId.Value);
                            break;
                    }
                }
            }

            return allow;
        }

        private async Task<bool> AllowAnyPostAsync<T>(int Id) where T : class, IIdentidad<int>, IVCAprobacion
        {
            bool ownership = false;

            if (await _access.IsInGroup(new[] { GroupNames.AdminGroup, GroupNames.CndGroup }))
            {
                var registro = await _data.Set<T>().Where(w => w.Id.Equals(Id))
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                ownership = registro is not null && !registro.Aprobacion.HasValue;
            }

            return ownership;
        }
    }
}
