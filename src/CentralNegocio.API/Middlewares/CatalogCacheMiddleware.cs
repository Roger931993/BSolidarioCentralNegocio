using Core.Stamp.Application.Interfaces.Infraestructure;
using Core.Stamp.Domain.Models;
using Core.Stamp.Model.Entity;
using Core.Stamp.Persistence.Contexts;
using Core.Stamp.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Core.Stamp.API.Middlewares
{
    public class CatalogCacheMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<CatalogCacheMiddleware> _caching;
    private readonly IMemoryCacheLocalService _memoryCacheLocalService;
    private readonly IDbContextFactory<LoggDbContext> _dbContext;
    private readonly IConfiguration _configuration;

    public CatalogCacheMiddleware(RequestDelegate next, ILogger<CatalogCacheMiddleware> caching, IMemoryCacheLocalService memoryCacheLocalService, IDbContextFactory<LoggDbContext> dbContext, IConfiguration configuration)
    {
      _next = next;
      _caching = caching;
      _memoryCacheLocalService = memoryCacheLocalService;
      _dbContext = dbContext;
      this._configuration = configuration;
    }

    public async Task Invoke(HttpContext context)
    {
      string[] lstCatalogos = _configuration.GetSection("Settings")!.GetSection("CatalogosCache")!.Value?.Split(',')!;
      for (int i = 0; i < lstCatalogos.Length; i++)
      {
        string strNombreCatalogo = lstCatalogos[i];
        await CargarCatalogo(strNombreCatalogo);
      }
      await _next(context);
    }

    private async Task CargarCatalogo(string strNombreCatalogo)
    {
      object objCache = await _memoryCacheLocalService.GetCachedDataObject(strNombreCatalogo);
      if (objCache == null)
      {
        // Obtiene el tipo de la clase
        Assembly assembly = Assembly.GetAssembly(typeof(BaseEntity))!;
        // Obtiene el tipo de la clase desde la DLL cargada
        Type tipoClase = assembly.GetType($"{Generals.NameSpacesObject.NameSpacesObjectCatalogBilling}.{strNombreCatalogo}")!;
        if (tipoClase == null)
          return;

        Type tipoLista = typeof(List<>).MakeGenericType(tipoClase);
        List<ParametroSP> objParametros = new List<ParametroSP>
        {
          new ParametroSP() { Name = "@tabla_name", Value = strNombreCatalogo },
          new ParametroSP() { Name = "@condiciones", Value = string.Empty },
          new ParametroSP() { Name = "@top", Value = "5000" }
        };
        ResultModel<SpsBuscarTablaCatalogoResult> objResultadoBDD;
        using (var Dbcontext = _dbContext.CreateDbContext())
        {
          objResultadoBDD = await Dbcontext.ExecuteGetCatalogBillingAsync(Constantes.Procedures.sps_buscar_tabla_catalogo, objParametros);
        }

        if (objResultadoBDD.Success)
        {
          JToken objRes = JToken.Parse(objResultadoBDD.Data.FirstOrDefault()!.JsonData)!;
          object objToken = JsonConvert.DeserializeObject(objRes.ToString(), tipoLista)!;
          await _memoryCacheLocalService.SetCacheObject(strNombreCatalogo, objToken);
        }
      }
    }
  }
}
