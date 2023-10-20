using CWA.Entities.Identity;
using CWA.Entities.Comun;
using CWA.Shared.Extensions;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CWA.Entities.Protecciones;
using CWA.Data.Seed;

namespace CWA.Data.Seed
{
    public static class Seed
    {
        public static async Task<int> SeedAsync(UserManager<AppUser> Manager, DataContext Context)
        {
            // Común
            var result = await Comun.Seed.ExecuteAsync(Manager, Context);

            return result;
        }

        public static async Task SeedPROTPlantilla(DataContext context)
        {
            if (await context.PROTPlantilla.AnyAsync()) return;

            var admin = await context.Users.Where(w => w.NormalizedUserName == "APPADM").FirstOrDefaultAsync();

            DateTime fixedStamp = DateTime.Now;

            PROTPlantilla plantilla = new()
            {
                Loaded = true,
                Archivo = "Resumen de Protecciones.xlsx",
                Mostrar = "Resumen de Protecciones",
                TipoDocumentoId = "PPE001",
                RegFecha = fixedStamp,
                ModFecha = fixedStamp,
                RegUsuarioId = admin.Id,
                ModUsuarioId = admin.Id
            };

            context.PROTPlantilla.Add(plantilla);

            await context.SaveChangesAsync();
        }

        public static async Task SeedPROTExcel(DataContext context)
        {
            if (await context.PROTExcelCellsEdit.AnyAsync()) return;

            var plantilla = await context.PROTPlantilla.FirstOrDefaultAsync();
            var admin = await context.Users.Where(w => w.NormalizedUserName == "APPADM").FirstOrDefaultAsync();

            DateTime fixedStamp = DateTime.Now;

            PROTExcelCellsEdit excel = new()
            {
                Editable = true,
                sentencia = "SELECT * FROM APPDEV.PROTRegistros WHERE Id = {0}",
                Sheet = "INVENTARIO DE PROTECCIONES",
                Column = 2,
                Row = 5,
                RegPlantillaId = plantilla.Id,
                RegFecha = fixedStamp,
                RegUsuarioId = admin.Id,
                ModUsuarioId = admin.Id

            };

            context.PROTExcelCellsEdit.Add(excel);

            await context.SaveChangesAsync();
        }
    }
}
