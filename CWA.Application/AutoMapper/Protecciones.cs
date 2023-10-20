using CWA.Entities.Protecciones;
using CWA.Models.Protecciones.Edit;
using CWA.Models.Protecciones.View;
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
        private void AddProteccionesMapping()
        {
            // Protecciones edit
            CreateMap<PROTRegistro, PROTRegistroEdit>();
            CreateMap<PROTRegistroEdit, PROTRegistro>();

            // Protecciones view
            CreateMap<PROTRegistro, PROTRegistroView>()
                .ForMember(d => d.Tipo, o => o.MapFrom(s => s.Tipo.Descripcion))
                .ForMember(d => d.RegUsuarioID, o => o.MapFrom(s => s.RegUsuarioId));

            // Protecciones Generales view
            CreateMap<PROTGenerales, PROTGeneralesView>()
                .ForMember(d => d.Provincia, o => o.MapFrom(s => s.Provincia.Descripcion));

            // Protecciones Generales edit
            CreateMap<PROTGenerales, PROTGeneralesEdit>();
            CreateMap<PROTGeneralesEdit, PROTGenerales>();

            // Protecciones Documentos view
            CreateMap<PROTDocumento, PROTDocumentosView>()
                .ForMember(d => d.Descripcion, o => o.MapFrom(s => s.Mostrar));

            CreateMap<PROTDocumento, SelectListItem>()
                .ForMember(d => d.Value, o => o.MapFrom(s => $"{s.Id}"))
                .ForMember(d => d.Text, o => o.MapFrom(s => s.Mostrar));

            // Protecciones Plantilla view
            CreateMap<PROTPlantilla, PROTPlantillasView>()
                .ForMember(d => d.Descripcion, o => o.MapFrom(s => s.Mostrar));

            CreateMap<PROTPlantilla, SelectListItem>()
                .ForMember(d => d.Value, o => o.MapFrom(s => $"{s.Id}"))
                .ForMember(d => d.Text, o => o.MapFrom(s => s.Mostrar));
        }
    }
}
