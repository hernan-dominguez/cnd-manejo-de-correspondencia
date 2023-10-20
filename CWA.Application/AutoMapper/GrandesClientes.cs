using AutoMapper;
using CWA.Entities.GrandesClientes;
using CWA.Models.GrandesClientes.Edit;
using CWA.Models.GrandesClientes.Validation;
using CWA.Models.GrandesClientes.View;
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
        private void AddGrandesClientesMapping()
        {
            // Grandes Clientes edit
            CreateMap<GCRegistro, GCRegistroEdit>();
            CreateMap<GCRegistroEdit, GCRegistro>();

            // Grandes Clientes view
            CreateMap<GCRegistro, GCRegistroView>()
                .ForMember(d => d.Tipo, o => o.MapFrom(s => s.TipoGranCliente.Descripcion))
                .ForMember(d => d.Registrante, o => o.MapFrom(s => s.Responsable == null ? s.Propietario.Nombre : s.Responsable.Codigo));

            // Grandes Clientes Generales view
            CreateMap<GCGenerales, GCGeneralesView>()
                .ForMember(d => d.Provincia, o => o.MapFrom(s => s.Provincia.Descripcion));

            // Grandes Clientes Generales edit
            CreateMap<GCGenerales, GCGeneralesEdit>();
            CreateMap<GCGeneralesEdit, GCGenerales>();

            // Grandes Clientes Mediciones view
            CreateMap<GCMedicion, GCMedicionesView>()
                .ForMember(d => d.Tipo, o => o.MapFrom(s => s.TipoConexion.Descripcion));

            // Grandes Clientes Medicion Edit
            CreateMap<GCMedicion, GCMedicionEdit>();

            CreateMap<GCMedicionEdit, GCMedicion>()
                .ForMember(d => d.Serie, o => o.MapFrom(s => s.Serie.ToUpper()));

            // Grandes Clientes Medicion View
            CreateMap<GCMedicion, GCMedicionView>()
                .ForMember(d => d.TipoConexion, o => o.MapFrom(s => s.TipoConexion.Descripcion))
                .ForMember(d => d.Tension, o => o.MapFrom(s => s.Tension.Descripcion))
                .ForMember(d => d.Distribuidora, o => o.MapFrom(s => s.Distribuidora.Codigo))
                .ForMember(d => d.TipoDistIdentificacion, o => o.MapFrom(s => s.TipoDistIdentificacion.Descripcion))
                .ForMember(d => d.Subestacion, o => o.MapFrom(s => s.Subestacion.Descripcion))
                .ForMember(d => d.Linea, o => o.MapFrom(s => s.Linea.Descripcion));

            // Grandes Clientes Documentos view
            CreateMap<GCDocumento, GCDocumentosView>()
                .ForMember(d => d.Descripcion, o => o.MapFrom(s => s.Mostrar));

            CreateMap<GCDocumento, GCDocumentoList>()
                .ForMember(d => d.Extensiones, o => o.MapFrom(s => s.TipoDocumento.RefVal1));

            // Grandes Clientes Comercial view
            CreateMap<GCComercial, GCComercialView>();

            // Grandes Clientes Comercial edit
            CreateMap<GCComercial, GCComercialEdit>();
            CreateMap<GCComercialEdit, GCComercial>();

            // Grandes Clientes Comercial validation
            CreateMap<GCComercial, GCComercialValidation>();

            // Grandes Clientes MedidoresDist view            
            CreateMap<GCMedicion, GCMedidoresDistView>()
                .ConstructUsing(d => new GCMedidoresDistView(d.Distribuidora.Codigo))
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Medidor.Id))
                .ForMember(d => d.Origen, o => o.Ignore())
                .ForMember(d => d.ModFecha, o => o.MapFrom(s => s.Medidor.ModFecha))
                .ForMember(d => d.FechaAprobacion, o => o.MapFrom(s => s.Medidor.FechaAtencion));

            // Grandes Clientes MedidorDist view
            CreateMap<GCMedidorDist, GCMedidorDistView>()
                .ForMember(d => d.Fabricante, o => o.MapFrom(s => s.Fabricante.Descripcion))
                .ForMember(d => d.Modelo, o => o.MapFrom(s => s.Modelo.Descripcion));

            CreateMap<GCMedidorCanal, GCMedidorCanalView>()
                .ForMember(d => d.Descripcion, o => o.MapFrom(s => s.Descripcion.Descripcion));

            // Grandes Clientes MedidoresDist edit
            CreateMap<GCMedidorDist, GCMedidorDistEdit>();
            CreateMap<GCMedidorDistEdit, GCMedidorDist>();

            CreateMap<GCMedidorCanal, GCMedidorCanalEdit>()
                .ForMember(d => d.Display, o => o.MapFrom(s => s.Descripcion.Descripcion));

            CreateMap<GCMedidorCanalEdit, GCMedidorCanal>();
        }
    }
}