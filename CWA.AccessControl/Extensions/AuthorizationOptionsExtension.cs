using CWA.AccessControl.Authorization.GrandesClientes;
using CWA.AccessControl.Authorization.Protecciones;
using CWA.AccessControl.Authorization.Sections;
using CWA.AccessControl.Authorization.ViabilidadContratos;
using CWA.AccessControl.Constants;
using CWA.Entities.ViabilidadContratos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.AccessControl.Extensions
{
    public static class AuthorizationOptionsExtension
    {
        public static void AddPolicies(this AuthorizationOptions Options)
        {
            // Section access policies
            Options.AddPolicy(PolicyNames.AdminSectionPolicy, policy => policy.Requirements.Add(new SectionRequirement { Name = SectionNames.AdminSection }));
            Options.AddPolicy(PolicyNames.GCSectionPolicy, policy => policy.Requirements.Add(new SectionRequirement { Name = SectionNames.GCSection }));
            Options.AddPolicy(PolicyNames.PROTSectionPolicy, policy => policy.Requirements.Add(new SectionRequirement { Name = SectionNames.PROTSection }));
            Options.AddPolicy(PolicyNames.VCSectionPolicy, policy => policy.Requirements.Add(new SectionRequirement { Name = SectionNames.VCSection }));

            // Grandes Clientes policies
            Options.AddPolicy(PolicyNames.GCRegistrarPolicy, policy => policy.Requirements.Add(new GCRegistrarRequirement()));

            Options.AddPolicy(PolicyNames.GCRegistroPolicy, policy =>
            {
                policy.Requirements.Add(new GCOwnershipRequirement());
                policy.Requirements.Add(new GCRegistroRequirement());
            });

            Options.AddPolicy(PolicyNames.GCGeneralesPolicy, policy =>
            {
                policy.Requirements.Add(new GCOwnershipRequirement());
                policy.Requirements.Add(new GCGeneralesRequirement());
            });

            Options.AddPolicy(PolicyNames.GCComercialPolicy, policy =>
            {
                policy.Requirements.Add(new GCOwnershipRequirement());
                policy.Requirements.Add(new GCComercialRequirement());
            });

            Options.AddPolicy(PolicyNames.GCMedicionesPolicy, policy =>
            {
                policy.Requirements.Add(new GCOwnershipRequirement());
                policy.Requirements.Add(new GCMedicionesRequirement());
            });

            Options.AddPolicy(PolicyNames.GCMedicionPolicy, policy =>
            {
                policy.Requirements.Add(new GCOwnershipRequirement());
                policy.Requirements.Add(new GCMedicionRequirement());
            });
                        
            Options.AddPolicy(PolicyNames.GCMedidoresDistPolicy, policy =>
            {
                policy.Requirements.Add(new GCOwnershipRequirement());
                policy.Requirements.Add(new GCMedidoresDistRequirement());
            });

            Options.AddPolicy(PolicyNames.GCMedidorDistPolicy, policy =>
            {
                policy.Requirements.Add(new GCOwnershipRequirement());
                policy.Requirements.Add(new GCMedidorDistRequirement());
            });

            Options.AddPolicy(PolicyNames.GCDocumentosPolicy, policy =>
            {
                policy.Requirements.Add(new GCOwnershipRequirement());
                policy.Requirements.Add(new GCDocumentosRequirement());
            });

            // Protecciones policies
            Options.AddPolicy(PolicyNames.PROTRegistrarPolicy, policy => policy.Requirements.Add(new PROTRegistrarRequirement()));

            Options.AddPolicy(PolicyNames.PROTRegistroPolicy, policy =>
            {
                policy.Requirements.Add(new PROTOwnershipRequirement());
                policy.Requirements.Add(new PROTRegistroRequirement());
            });

            Options.AddPolicy(PolicyNames.PROTGeneralesPolicy, policy =>
            {
                policy.Requirements.Add(new PROTOwnershipRequirement());
                policy.Requirements.Add(new PROTGeneralesRequirement());
            });

            Options.AddPolicy(PolicyNames.PROTDocumentosPolicy, policy =>
            {
                policy.Requirements.Add(new PROTOwnershipRequirement());
                policy.Requirements.Add(new PROTDocumentosRequirement());
            });

            Options.AddPolicy(PolicyNames.PROTPlantillasPolicy, policy =>
            {
                policy.Requirements.Add(new PROTOwnershipRequirement());
                policy.Requirements.Add(new PROTPlantillasRequirement());
            });

            // Viabilidad de Contratos
            Options.AddPolicy(PolicyNames.VCRegionalesPolicy, policy => policy.Requirements.Add(new VCRegionalesRequirement()));

            Options.AddPolicy(PolicyNames.VCNacionalPolicy, policy => policy.Requirements.Add(new VCRegistroRequirement { Name = VCConstants.NACIONAL }));

            Options.AddPolicy(PolicyNames.VCRegionalPolicy, policy =>
            {
                policy.Requirements.Add(new VCRegionalesRequirement { Name = VCConstants.REGIONAL });
                policy.Requirements.Add(new VCRegistroRequirement { Name = VCConstants.REGIONAL });
            });

            Options.AddPolicy(PolicyNames.VCEnmiendaPolicy, policy => policy.Requirements.Add(new VCRegistroRequirement { Name = VCConstants.ENMIENDA }));

            // Fallback policy 
            Options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

            // Settings
            Options.InvokeHandlersAfterFailure = false;
        }
    }
}