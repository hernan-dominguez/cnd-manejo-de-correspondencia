using CWA.Shared.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWA.Web.Extensions
{
    public static class ModelStateExtensions
    {
        public static List<string> GetErrorKeys(this ModelStateDictionary ModelStateValue)
        {
            var keys = new List<string>();

            ModelStateValue.Keys.Where(w => ModelStateValue[w].Errors.Count > 0)
                .ToList()
                .ForEach(k => keys.Add(k.Replace(".", "-").Replace("[", "-").Replace("]", "")));

            return keys;
        }

        public static object GetErrorMessages(this ModelStateDictionary ModelStateValue)
        {
            var errors = new List<object>();

            foreach (var item in ModelStateValue)
            {
                if (ModelStateValue[item.Key].Errors.Count > 0)
                {
                    errors.Add(new 
                    { 
                        Key = item.Key.Replace(".","-").Replace("[", "-").Replace("]", ""), 
                        Message = ModelStateValue[item.Key].Errors.First().ErrorMessage 
                    });
                }
            }

            return errors;
        }
    }

}