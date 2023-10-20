using CWA.Application.AutoMapper;
using CWA.Application.Services.Bases;
using CWA.Application.Services.Comun;
using CWA.Application.Services.GrandesClientes;
using CWA.Application.Services.Protecciones;
using CWA.Application.Services.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CWA.Application.Services.ViabilidadContratos;
using CWA.Application.Services.ManejoDeCorrespondencia;

namespace CWA.Application
{
    public static class ServicesExtension
    {
        public static IServiceCollection AddApplicationLayerServices(this IServiceCollection Services)
        {
            // Application services
            Services.AddScoped<AppUserService>();
            Services.AddScoped<AgenteService>();
            Services.AddScoped<AgenteRegionalService>();
            Services.AddScoped<CatalogoService>();
            Services.AddScoped<GCService>();
            Services.AddScoped<PROTService>();
            Services.AddScoped<VCService>();
            Services.AddScoped<MCUploadService>();
            Services.AddScoped<MCDownloadService>();

            // Third party services
            Services.AddAutoMapper(typeof(MappingProfile).Assembly);

            return Services;
        }
    }
}
