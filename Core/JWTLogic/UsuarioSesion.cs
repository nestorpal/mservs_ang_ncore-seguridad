using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicios.Api.Seguridad.Core.JWTLogic
{
    public class UsuarioSesion : IUsuarioSesion
    {
        private readonly IHttpContextAccessor _httpAcc;

        public UsuarioSesion(IHttpContextAccessor httpAcc)
        {
            this._httpAcc = httpAcc;
        }

        public string GetUsuarioSEsion()
        {
            var userName = _httpAcc.HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == "username")?.Value; // return username claims
            return userName;
        }
    }
}
