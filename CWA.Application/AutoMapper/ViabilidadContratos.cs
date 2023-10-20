using CWA.Entities.ViabilidadContratos;
using CWA.Models.ViabilidadContratos.Edit;
using CWA.Models.ViabilidadContratos.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Application.AutoMapper
{
    public partial class MappingProfile
    {
        private void AddViabilidadContratosMapping()
        {
            // Nacionales edit
            CreateMap<VCNacional, VCNacionalEdit>();
            CreateMap<VCNacionalEdit, VCNacional>();
            CreateMap<VCDocumentoEdit, VCDocNacional>();

            // Nacionales view
            CreateMap<VCNacional, VCNacionalView>()
                .ForMember(d => d.Codigo, o => o.MapFrom(s => /*s.CodigoDisplay))*/ $"C-{s.TipoContrato.RefVal2}-{s.Vendedor.Codigo}-{s.Comprador.Codigo}-{s.Inicia.Year}-{s.Id}"))
                .ForMember(d => d.Tipo, o => o.MapFrom(s => s.TipoContrato.Descripcion))
                .ForMember(d => d.Vendedor, o => o.MapFrom(s => s.Vendedor.Codigo))
                .ForMember(d => d.Comprador, o => o.MapFrom(s => s.Comprador.Codigo))
                .ForMember(d => d.VendedorNombre, o => o.MapFrom(s => s.Vendedor.Organizacion.Nombre))
                .ForMember(d => d.CompradorNombre, o => o.MapFrom(s => s.Comprador.Organizacion.Nombre));
            
            CreateMap<VCDocNacional, VCDocumentoView>()
                .ForMember(d => d.Descripcion, o => o.MapFrom(s => s.TipoDocumento.Descripcion))
                .ForMember(d => d.SortOrder, o=> o.MapFrom(s => s.TipoDocumento.RefVal5));

            // Regionales edit
            CreateMap<VCRegional, VCRegionalEdit>();
            CreateMap<VCRegionalEdit, VCRegional>();
            CreateMap<VCDocumentoEdit, VCDocRegional>();

            // Regionales View
            CreateMap<VCRegional, VCRegionalView>()
                .ForMember(d => d.Codigo, o => o.MapFrom(s => $"R-{s.TipoSolicitud.RefVal2}-{s.Solicitante.Codigo}-{s.Contraparte.Codigo}-{s.Inicia.Year}-{s.Id}"))
                .ForMember(d => d.Solicitud, o => o.MapFrom(s => s.TipoSolicitud.Descripcion))
                .ForMember(d => d.Transaccion, o => o.MapFrom(s => s.TipoTransaccion.Descripcion))
                .ForMember(d => d.Solicitante, o => o.MapFrom(s => s.Solicitante.Organizacion.Nombre))
                .ForMember(d => d.Pais, o => o.MapFrom(s => s.Pais.Descripcion))
                .ForMember(d => d.Contraparte, o => o.MapFrom(s => s.Contraparte.Codigo))
                .ForMember(d => d.ContraparteNombre, o => o.MapFrom(s => s.Contraparte.Nombre));

            CreateMap<VCDocRegional, VCDocumentoView>()
                .ForMember(d => d.Descripcion, o => o.MapFrom(s => s.TipoDocumento.Descripcion))
                .ForMember(d => d.SortOrder, o => o.MapFrom(s => s.TipoDocumento.RefVal5));

            // Enmiendas Edit
            CreateMap<VCEnmienda, VCEnmiendaEdit>();
            CreateMap<VCEnmiendaEdit, VCEnmienda>();
            CreateMap<VCDocumentoEdit, VCDocEnmienda>();

            // Enmiendas View
            CreateMap<VCEnmienda, VCEnmiendaView>()
                .ForMember(d => d.Codigo, o => o.MapFrom(s => $"E-{s.Contrato.TipoContrato.RefVal2}-{s.Contrato.Vendedor.Codigo}-{s.Contrato.Comprador.Codigo}-{s.Inicia.Year}-{s.Id}"))                
                .ForMember(d => d.CodigoContrato, o => o.MapFrom(s => $"{s.Contrato.TipoContrato.RefVal1}-{s.Contrato.TipoContrato.RefVal2}-{s.Contrato.Vendedor.Codigo}-{s.Contrato.Comprador.Codigo}-{s.Inicia.Year}-{s.Id}"))
                .ForMember(d => d.ContratoId, o => o.MapFrom(s => s.ContratoId))
                .ForMember(d => d.Tipo, o => o.MapFrom(s => s.Contrato.TipoContrato.Descripcion))
                .ForMember(d => d.Vendedor, o => o.MapFrom(s => s.Contrato.Vendedor.Codigo))
                .ForMember(d => d.Comprador, o => o.MapFrom(s => s.Contrato.Comprador.Codigo))
                .ForMember(d => d.VendedorNombre, o => o.MapFrom(s => s.Contrato.Vendedor.Organizacion.Nombre))
                .ForMember(d => d.CompradorNombre, o => o.MapFrom(s => s.Contrato.Comprador.Organizacion.Nombre));

            CreateMap<VCDocEnmienda, VCDocumentoView>()
                .ForMember(d => d.Descripcion, o => o.MapFrom(s => s.TipoDocumento.Descripcion))
                .ForMember(d => d.SortOrder, o => o.MapFrom(s => s.TipoDocumento.RefVal5));
        }
    }
}
