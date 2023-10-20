using AutoMapper;

namespace CWA.Application.AutoMapper
{
    public partial class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add mappings
            AddComunMapping();            
            AddGrandesClientesMapping();
            AddProteccionesMapping();
            AddViabilidadContratosMapping();
        }
    }
}
