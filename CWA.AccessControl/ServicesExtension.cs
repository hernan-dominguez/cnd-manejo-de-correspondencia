using CWA.AccessControl.Authorization.GrandesClientes;
using CWA.AccessControl.Authorization.Protecciones;
using CWA.AccessControl.Authorization.Sections;
using CWA.AccessControl.Authorization.ViabilidadContratos;
using CWA.AccessControl.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.AccessControl
{
    public static class ServicesExtension
    {
        public static IServiceCollection AddAccessControlLayerServices(this IServiceCollection Services)
        {
            // Application
            Services.AddScoped<AppAccessControlService>();
            Services.AddScoped<IAuthorizationHandler, SectionHandler>();

            // Grandes Clientes
            Services.AddScoped<GCAccessControlService>();
            Services.AddScoped<IAuthorizationHandler, GCOwnershipHandler>();
            Services.AddScoped<IAuthorizationHandler, GCRegistrarHandler>();
            Services.AddScoped<IAuthorizationHandler, GCRegistroHandler>();
            Services.AddScoped<IAuthorizationHandler, GCGeneralesHandler>();
            Services.AddScoped<IAuthorizationHandler, GCComercialHandler>();
            Services.AddScoped<IAuthorizationHandler, GCMedicionesHandler>();
            Services.AddScoped<IAuthorizationHandler, GCMedicionHandler>();
            Services.AddScoped<IAuthorizationHandler, GCMedidoresDistHandler>();
            Services.AddScoped<IAuthorizationHandler, GCMedidorDistHandler>();
            Services.AddScoped<IAuthorizationHandler, GCDocumentosHandler>();

            // Protecciones
            Services.AddScoped<PROTAccessControlService>();
            Services.AddScoped<IAuthorizationHandler, PROTOwnershipHandler>();
            Services.AddScoped<IAuthorizationHandler, PROTRegistrarHandler>();
            Services.AddScoped<IAuthorizationHandler, PROTRegistroHandler>();
            Services.AddScoped<IAuthorizationHandler, PROTGeneralesHandler>();
            Services.AddScoped<IAuthorizationHandler, PROTDocumentosHandler>();
            Services.AddScoped<IAuthorizationHandler, PROTPlantillasHandler>();

            // Viabilidad de contratos
            Services.AddScoped<VCAccessControlService>();
            Services.AddScoped<IAuthorizationHandler, VCRegionalesHandler>();
            Services.AddScoped<IAuthorizationHandler, VCRegistroHandler>();

            return Services;
        }
    }
}
