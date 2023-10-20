using CWA.Entities.Identity;
using CWA.Entities.GrandesClientes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CWA.Entities;
using CWA.Data.Extensions;
using CWA.Data.Extensions.GrandesClientes;
using CWA.Data.Extensions.Protecciones;
using CWA.Data.Extensions.Identity;
using CWA.Data.Extensions.Comun;
using CWA.Data.Extensions.ViabilidadContratos;
using CWA.Entities.Comun;
using CWA.Entities.Protecciones;
using CWA.Entities.ViabilidadContratos;

namespace CWA.Data
{
    public class DataContext : IdentityDbContext<AppUser, IdentityRole<int>, int, AppUserClaim, IdentityUserRole<int>, AppUserLogin, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        // Común
        public DbSet<Organizacion> Organizaciones { get; set; }
        public DbSet<UsuarioOrganizacion> UsuariosOrganizaciones { get; set; }
        public DbSet<Agente> Agentes { get; set; }
        public DbSet<AgenteRegional> AgentesRegionales { get; set; }
        public DbSet<ResponsableGranCliente> ResponsablesGrandesClientes { get; set; }
        public DbSet<Catalogo> Catalogos { get; set; }        
        public DbSet<DiaIrregular> DiasIrregulares { get; set; }
        public DbSet<CorreoMensaje> CorreoMensajes { get; set; }

        // Grandes Clientes
        public DbSet<GCRegistro> GCRegistros { get; set; }
        public DbSet<GCGenerales> GCGenerales { get; set; }
        public DbSet<GCComercial> GCComerciales { get; set; }
        public DbSet<GCDocumento> GCDocumentos { get; set; }
        public DbSet<GCMedicion> GCMediciones { get; set; }                
        public DbSet<GCMedidorDist> GCMedidoresDist { get; set; }
        public DbSet<GCMedidorCanal> GCMedidorCanales { get; set; }
        public DbSet<GCDestinatario> GCDestinatarios { get; set; }

        // Protecciones
        public DbSet<PROTRegistro> PROTRegistros { get; set; }
        public DbSet<PROTGenerales> PROTGenerales { get; set; }
        public DbSet<PROTDocumento> PROTDocumentos { get; set; }
        public DbSet<PROTBitacoraDocumentos> PROTBitacoraDocumentos { get; set; }
        public DbSet<PROTPlantilla> PROTPlantilla { get; set; }
        public DbSet<PROTExcelCellsEdit> PROTExcelCellsEdit { get; set; }
        public DbSet<PROTDestinatario> PROTDestinatarios { get; set; }

        // Viabilidad Contratos
        public DbSet<VCNacional> VCNacionales { get; set; }
        public DbSet<VCRegional> VCRegionales { get; set; }
        public DbSet<VCEnmienda> VCEnmiendas { get; set; }
        public DbSet<VCDocNacional> VCDocsNacionales { get; set; }
        public DbSet<VCDocRegional> VCDocsRegionales { get; set; }
        public DbSet<VCDocEnmienda> VCDocsEnmiendas { get; set; }
        public DbSet<VCDestinatario> VCDestinatarios { get; set; }

        public DataContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure entities
            builder.Configure<AppUser>();
            builder.Configure<Organizacion>();
            builder.Configure<UsuarioOrganizacion>();
            builder.Configure<Agente>();
            builder.Configure<AgenteRegional>();
            builder.Configure<ResponsableGranCliente>();
            builder.Configure<Catalogo>();     
            builder.Configure<DiaIrregular>();
            builder.Configure<CorreoMensaje>();

            // Grandes Clientes
            builder.Configure<GCRegistro>();
            builder.Configure<GCGenerales>();
            builder.Configure<GCComercial>();
            builder.Configure<GCDocumento>();
            builder.Configure<GCMedicion>();
            builder.Configure<GCMedidorDist>();
            builder.Configure<GCMedidorCanal>();
            builder.Configure<GCDestinatario>();

            // Viabilidad de Contratos
            builder.Configure<VCNacional>();
            builder.Configure<VCRegional>();
            builder.Configure<VCEnmienda>();
            builder.Configure<VCDocNacional>();
            builder.Configure<VCDocRegional>();
            builder.Configure<VCDocEnmienda>();
            builder.Configure<VCDestinatario>();

            // Protecciones
            builder.Configure<PROTRegistro>();
            builder.Configure<PROTGenerales>();
            builder.Configure<PROTDocumento>();
            builder.Configure<PROTBitacoraDocumentos>();
            builder.Configure<PROTPlantilla>();
            builder.Configure<PROTExcelCellsEdit>();

            // Set everything to uppercase to make Oracle happy =)
            builder.SetNamesUppercase();

            // Automatic leading/trailing blanks trimming on String properties
            builder.SetStringPropertiesAutoTrim();
        }
    }
}