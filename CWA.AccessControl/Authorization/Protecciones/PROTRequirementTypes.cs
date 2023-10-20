using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.AccessControl.Authorization.Protecciones
{
    public class PROTOwnershipRequirement : IAuthorizationRequirement { }

    public class PROTRegistrarRequirement : IAuthorizationRequirement { }

    public class PROTRegistroRequirement : IAuthorizationRequirement { }

    public class PROTGeneralesRequirement : IAuthorizationRequirement { }

    public class PROTDocumentosRequirement : IAuthorizationRequirement { }

    public class PROTPlantillasRequirement : IAuthorizationRequirement { }
}
