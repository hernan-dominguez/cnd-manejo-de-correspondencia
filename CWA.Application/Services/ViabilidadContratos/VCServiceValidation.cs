using CWA.Shared.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Application.Services.ViabilidadContratos
{
    public partial class VCService
    {
        public async Task<bool> InvalidFileExtensionAsync(IFormFile Submitted, string TipoId)
        {
            var tipo = await _data.Catalogos.Where(w => w.Id == TipoId).AsNoTracking().FirstOrDefaultAsync();

            if (tipo is null) return true;

            var allowedExtensions = tipo.RefVal1.ToLower().SplitList(',');
            string fileExtension = Submitted.GetExtension(false).ToLower();

            return (!(allowedExtensions.Contains(fileExtension)));
        }

        public async Task<int> InvalidDatesAsync(DateTime Inicio, DateTime Fin, bool ConsistencyOnly = false)
        {
            var SetDate = DateTime.Now.Date;

            // Consistency
            if (Inicio <= SetDate || Fin <= SetDate || Inicio > Fin) return -1;

            if (ConsistencyOnly) return 0;

            // Duration
            var days = (int)(Fin.AddDays(1) - Inicio).TotalDays;

            // Add 1 day to establish a whole anticipation days value
            SetDate = SetDate.AddDays(1);

            var validation = (await _data.Catalogos.Where(w => w.Grupo == "VCVALIDACION").AsNoTracking().ToListAsync())
                .Select(t => new { Min = int.Parse(t.RefVal1), Max = int.Parse(t.RefVal2), Before = int.Parse(t.RefVal3), Labor = (!t.RefVal4.Empty() && t.RefVal4 == "H"), Code = int.Parse(t.RefVal5) })
                .Where(w => w.Min <= days && days <= w.Max)
                .FirstOrDefault();

            if (!validation.Labor)
            {
                return ((int)(Inicio - SetDate).TotalDays >= validation.Before) ? 0 : validation.Code;
            }
            else
            {
                // Exclude saturday and sunday
                var current = SetDate;
                int weekDays = 0;

                while (current < Inicio)
                {
                    if (current.DayOfWeek != DayOfWeek.Sunday && current.DayOfWeek != DayOfWeek.Saturday) weekDays++;                    

                    current = current.AddDays(1);
                }

                // Filter out off days
                int offDays = await _data.DiasIrregulares.Where(w => w.Dia >= SetDate && w.Dia < Inicio).CountAsync();

                return (weekDays - offDays) >= validation.Before ? 0 : validation.Code;
            }
        }
    }
}
