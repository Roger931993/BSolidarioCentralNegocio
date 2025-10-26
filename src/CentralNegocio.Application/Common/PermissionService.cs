using CentralNegocio.Application.DTOs;
using CentralNegocio.Application.Interfaces;
using CentralNegocio.Application.Interfaces.Infraestructure;
using Microsoft.Extensions.Configuration;

namespace CentralNegocio.Application.Common
{
    public class PermissionService : IPermissionService
    {
        private readonly IRedisCache _redisCache;
        private readonly IConfiguration _configuration;        

        public PermissionService(IRedisCache redisCache, IConfiguration configuration)
        {
            _redisCache = redisCache;
            this._configuration = configuration;            
        }

        public async Task<bool> HasPermissionAsync(string idSession, string Verb, string permission)
        {
            bool blnFlagAuth = _configuration["AuthenticationSettings:UseNoValidation"] == "True" ? true : false;
            if (blnFlagAuth)
                return true;

            if (string.IsNullOrEmpty(idSession))
                return false;

            List<adm_permissionDto> userPermissions = await _redisCache.GetAsync<List<adm_permissionDto>>($"{idSession}-adm_permission_backend");           
            if (!userPermissions.Any())
                return false;

            return userPermissions.Any(x => x.action == Verb && x.name == permission);
        }
    }
}
