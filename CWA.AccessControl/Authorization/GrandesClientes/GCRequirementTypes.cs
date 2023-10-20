using CWA.AccessControl.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.AccessControl.Authorization.GrandesClientes
{
    public class GCOwnershipRequirement : IAuthorizationRequirement { }

    public class GCRegistrarRequirement : IAuthorizationRequirement { }

    public class GCRegistroRequirement : IAuthorizationRequirement { }

    public class GCGeneralesRequirement : IAuthorizationRequirement { }

    public class GCComercialRequirement : IAuthorizationRequirement { }

    public class GCMedicionesRequirement : IAuthorizationRequirement { }

    public class GCMedicionRequirement : OperationAuthorizationRequirement { }

    public class GCMedidoresDistRequirement : IAuthorizationRequirement { }

    public class GCMedidorDistRequirement : IAuthorizationRequirement { }

    public class GCDocumentosRequirement : IAuthorizationRequirement { }
}
