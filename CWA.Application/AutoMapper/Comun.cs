using CWA.Entities.Comun;
using CWA.Entities.Identity;
using CWA.Models.Comun;
using CWA.Models.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Application.AutoMapper
{
    public partial class MappingProfile
    {
        private void AddComunMapping()
        {
            // Usuarios
            CreateMap<AppUser, AppUserListing>();

            CreateMap<AppUser, SelectListItem>()
                .ForMember(d => d.Text, o => o.MapFrom(s => s.Nombre))
                .ForMember(d => d.Value, o => o.MapFrom(s => $"{s.Id}"));

            // Agentes
            CreateMap<Agente, AgenteListing>()
                .ForMember(d => d.Nombre, o => o.MapFrom(s => s.Organizacion.Nombre))
                .ForMember(d => d.TipoAgente, o => o.MapFrom(s => s.TipoAgente.Descripcion));

            CreateMap<Agente, SelectListItem>()
                .ForMember(d => d.Text, o => o.MapFrom(s => s.Organizacion.Nombre))
                .ForMember(d => d.Value, o => o.MapFrom(s => s.Id));

            // Agentes regionales
            CreateMap<AgenteRegional, AgenteRegionalListing>()
                .ForMember(d => d.Pais, o => o.MapFrom(s => s.Pais.Descripcion));

            CreateMap<AgenteRegional, SelectListItem>()
                .ForMember(d => d.Text, o => o.MapFrom(s => s.Nombre))
                .ForMember(d => d.Value, o => o.MapFrom(s => s.Id));

            // Responsables Grandes Clientes
            CreateMap<ResponsableGranCliente, ResponsableGranClienteListing>()
                .ForMember(d => d.ResponsableNombre, o => o.MapFrom(s => s.Responsable.Organizacion.Nombre))
                .ForMember(d => d.GranClienteNombre, o => o.MapFrom(s => s.GranCliente.Organizacion.Nombre));

            // Catálogos
            CreateMap<Catalogo, CatalogoListing>();

            CreateMap<Catalogo, SelectListItem>()
                .ForMember(d => d.Text, o => o.MapFrom(s => s.Descripcion))
                .ForMember(d => d.Value, o => o.MapFrom(s => s.Id));
        }
    }
}
