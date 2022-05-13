using Servicios.Api.Seguridad.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicios.Api.Seguridad.Core.JWTLogic
{
    public interface IJWTGenerator
    {
        string CreateToken(Usuario usuario);
    }
}
