using AutoMapper;
using CWA.AccessControl.Services;
using CWA.Application.Services.Bases;
using CWA.Data;
using CWA.Entities.Bases;
using CWA.Entities.Bases.GrandesClientes;
using CWA.Entities.Bases.ViabilidadContratos;
using CWA.Entities.Comun;
using CWA.Entities.ViabilidadContratos;
using CWA.Models.ViabilidadContratos.Edit;
using CWA.Shared.Extensions;
using CWA.Shared.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using DocumentFormat.OpenXml.InkML;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using DocumentFormat.OpenXml.Bibliography;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using CWA.Models.ManejoDeCorrespondencia.Carga;
using System.Drawing.Printing;

namespace CWA.Application.Services.ManejoDeCorrespondencia
{
    public partial class MCUploadService : BaseService
    {

        public MCUploadService(DataContext Data, AppAccessControlService Access, IMapper Mapper, ILogger<BaseService> Logger) : base(Data, Access, Mapper, Logger) { }

        public async Task<List<string>> GetListaValores(ClientContext context, string nombreLista)
        {
            Web web = context.Web;

            List<string> listaValores = new();

            CamlQuery query = CamlQuery.CreateAllItemsQuery();
            ListItemCollection items = web.Lists.GetByTitle(nombreLista).GetItems(query);

            context.Load(items);
            await context.ExecuteQueryAsync();

            foreach (ListItem listItem in items)
            {
                // We have all the list item data. For example, Title.
                listaValores.Add(listItem["Title"].ToString());
            }
            return listaValores;
        }

        public async Task<List<MCElementoLista>> GetListaValoresConId(ClientContext context, string nombreLista)
        {
            Web web = context.Web;

            List<MCElementoLista> listaValores = new();

            CamlQuery query = CamlQuery.CreateAllItemsQuery();
            ListItemCollection items = web.Lists.GetByTitle(nombreLista).GetItems(query);

            context.Load(items);
            await context.ExecuteQueryAsync();

            foreach (ListItem listItem in items)
            {
                // We have all the list item data. For example, Title.
                MCElementoLista element = new()
                {
                    Id = listItem["ID"].ToString(),
                    Valor = listItem["Title"].ToString()
                };

                var inactive = listItem["Inactivo"];

                if (inactive == null || inactive.ToString() == "false")
                {

                        listaValores.Add(element);
                
                }
                
            }
            return listaValores;
        }

        public async Task<List<MCElementoLista>> GetListaValoresConId(ClientContext context, string nombreLista, string nombreCampo)
        {
            Web web = context.Web;

            List<MCElementoLista> listaValores = new();

            CamlQuery query = CamlQuery.CreateAllItemsQuery();
            ListItemCollection items = web.Lists.GetByTitle(nombreLista).GetItems(query);

            context.Load(items);
            await context.ExecuteQueryAsync();

            foreach (ListItem listItem in items)
            {
                // We have all the list item data. For example, Title.
                MCElementoLista element = new()
                {
                    Id = listItem["ID"].ToString(),
                    Valor = listItem[nombreCampo]?.ToString() ?? "Vacío"
                };
                listaValores.Add(element);
            }
            return listaValores;
        }

        public async Task<List<MCNotaSaliente>> GetNotasSalientes(ClientContext context)
        {
            Web web = context.Web;

            List<MCNotaSaliente> listaValores = new();

            CamlQuery query = CamlQuery.CreateAllItemsQuery(5000);
            ListItemCollection items = web.Lists.GetByTitle("Saliente").GetItems(query);

            do
            {
                context.Load(items);
                await context.ExecuteQueryAsync();

                foreach (ListItem listItem in items)
                {
                    List<string> destinatarios = new();
                    FieldLookupValue[] lookupsDestinatarios = listItem.FieldValues["Destinatario"] as FieldLookupValue[];
                    foreach (FieldLookupValue lookup in lookupsDestinatarios ?? Array.Empty<FieldLookupValue>())
                    {
                        string lvalueDestinatario = lookup?.LookupValue ?? "Vacío";
                        destinatarios.Add(lvalueDestinatario);
                    }

                    // We have all the list item data. For example, Title.
                    MCNotaSaliente element = new()
                    {
                        Id = listItem["ID"].ToString(),
                        NumeroDeNota = listItem["N_x00fa_mero_x0020_de_x0020_Nota"]?.ToString() ?? "Vacío",
                        Destinatarios = destinatarios
                    };
                    listaValores.Add(element);
                }

                query.ListItemCollectionPosition = items.ListItemCollectionPosition;

            } while (query.ListItemCollectionPosition != null);
            
            return listaValores;
        }

        public List<string> GetNuevoDocRespuesta()
        {
            List<string> listaValores = new()
            {
                "Nuevo Documento",
                "Respuesta"
            };

            return listaValores;

        }

        public async Task<List<MCSubtema>> GetSubtemas(ClientContext context, List<MCElementoLista> temas)
        {
            Web web = context.Web;

            CamlQuery query = CamlQuery.CreateAllItemsQuery();
            ListItemCollection items = web.Lists.GetByTitle("Subtemas").GetItems(query);

            context.Load(items);
            await context.ExecuteQueryAsync();

            List<MCSubtema> listaValores = new();
            foreach (ListItem listItem in items)
            {
                FieldLookupValue lookupTema = listItem.FieldValues["Tema"] as FieldLookupValue;
                string nombreTema = lookupTema?.LookupValue ?? "Vacío";
                string idTema = temas.Where(t => t.Valor == nombreTema).FirstOrDefault()?.Id ?? "Vacío";
                MCSubtema mCSubtema = new()
                {
                    Tema = idTema,
                    IdSubtema = listItem["ID"].ToString(),
                    NombreSubtema = listItem["Title"].ToString()
                };
                listaValores.Add(mCSubtema);
            }
            return listaValores;
        }

        public async Task<List<MCAgente>> GetMCAgentes(ClientContext context)
        {
            Web site = context.Web;

            CamlQuery query = CamlQuery.CreateAllItemsQuery();
            ListItemCollection items = site.Lists.GetByTitle("Agentes").GetItems(query);

            context.Load(items);
            await context.ExecuteQueryAsync();

            List<MCAgente> listaMCUsuarios = new();
            foreach (ListItem listItem in items)
            {
                FieldLookupValue lookupAgente = listItem.FieldValues["Tp_x0020_de_x0020_Agente"] as FieldLookupValue;
                string lvalueAgente = lookupAgente?.LookupValue ?? "Vacío";

                // We have all the list item data. For example, Title.
                MCAgente mCUsuario = new()
                {
                    Codigo = listItem["field_0"]?.ToString() ?? "SINCODIGO",
                    Id = listItem["ID"].ToString(),
                    TipoAgente = lvalueAgente,
                    NombreAgente = listItem["Title"].ToString()
                };

                listaMCUsuarios.Add(mCUsuario);
                
            }
            return listaMCUsuarios;

        }

    }
}