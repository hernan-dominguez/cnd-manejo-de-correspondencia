using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace CWA.AccessControl.Authorization.ViabilidadContratos
{
    public class VCRegionalesRequirement : OperationAuthorizationRequirement { }

    public class VCRegistroRequirement : OperationAuthorizationRequirement { }
}
