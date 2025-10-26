using CentralNegocio.Application.Interfaces;
using CentralNegocio.Shared.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CentralNegocio.API.Filters
{
    public class PermissionFilter : IAsyncActionFilter
  {
    private readonly IPermissionService _permissionService;
    private readonly string _requiredPermission;

    public PermissionFilter(IPermissionService permissionService, string requiredPermission)
    {
      _permissionService = permissionService;
      _requiredPermission = requiredPermission;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      string idSession = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == Generals.Claims.idSession)?.Value!;
      string isAdmin = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == Generals.Claims.isAdmin)?.Value!;
      if (isAdmin == "true" || string.IsNullOrEmpty(isAdmin))
        await next();
      else
      {
        string httpMethod = context.HttpContext.Request.Method;
        if (idSession == null || !await _permissionService.HasPermissionAsync(idSession, httpMethod, _requiredPermission))
        {
          context.Result = new ForbidResult();
          return;
        }
        await next();
      }
    }
  }
  public class PermissionAttribute : TypeFilterAttribute
  {
    /// <summary>
    /// Atributo de Permiso
    /// </summary>
    /// <param name="permission">Nombre de permiso</param>
    public PermissionAttribute(string permission) : base(typeof(PermissionFilter))
    {
      Arguments = new object[] { permission };
    }
  }
}
