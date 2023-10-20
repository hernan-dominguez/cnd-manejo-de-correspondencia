using AutoMapper;
using CWA.AccessControl.Services;
using CWA.Application.Services.Bases;
using CWA.Data;
using CWA.Entities.Bases;
using CWA.Shared.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Application.Services.GrandesClientes
{
    public partial class GCService
    {
        public async Task<bool> InvalidFechaEntradaAsync(int RegistroId, DateTime ModelData)
        {
            var registro = await _data.GCRegistros.Where(w => w.Id == RegistroId).FirstOrDefaultAsync();

            return (registro.FechaEntrada is null && ModelData < DateTime.Now.Date.AddDays(30)) || ModelData <= registro.FechaEntrada;
        }

        public async Task<bool> InvalidFileExtensionAsync(IFormFile FileName, int DocumentoId)
        {
            var documento = await _data.GCDocumentos.Where(w => w.Id == DocumentoId).Include(i => i.TipoDocumento).FirstOrDefaultAsync();

            if (documento is null) return false;

            List<string> allowedExtensions = documento.TipoDocumento.RefVal1.ToUpper().SplitList(',');
            string fileExtension = FileName.GetExtension(false).ToUpper();

            return (!(allowedExtensions.Contains(fileExtension)));
        }

        public async Task<bool> InvalidItemDataAsync<K, TEntity, TModel>(K id) where K : IEquatable<K> where TEntity : class, IIdentidad<K> where TModel : class
        {
            var item = await _data.Set<TEntity>().Where(w => w.Id.Equals(id)).FirstOrDefaultAsync();

            var vModel = _mapper.Map<TModel>(item);
            var vContext = new System.ComponentModel.DataAnnotations.ValidationContext(vModel);
            var vResults = new List<ValidationResult>();

            return !Validator.TryValidateObject(vModel, vContext, vResults, true);
        }                 
    }
}
