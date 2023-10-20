using Microsoft.Extensions.Configuration;

namespace CWA.Application.Services.ManejoDeCorrespondencia
{
    public static class MCServiceHelpers
    {

        public static SharepointAuthenticationObject GetSharepointAuthenticationObject(IConfiguration _config)
        {
            return new SharepointAuthenticationObject(_config);
        }
    }
}