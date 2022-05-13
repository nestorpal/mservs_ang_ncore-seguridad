using Microsoft.AspNetCore.Identity;
using Servicios.Api.Seguridad.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicios.Api.Seguridad.Core.Persistence
{
    public class SeguridadData
    {
        public static async Task InsertarUsuario(
            SeguridadContexto context
            , UserManager<Usuario> usrManager)
        {
            if (!usrManager.Users.Any())
            {
                var usuario = new Usuario()
                {
                    Nombre = "Nestor",
                    Apellido = "Panu",
                    Direccion = "Av. Mi Casa",
                    UserName = "nestor.panu",
                    Email = "nestor.panu@gmail.com"
                };

                await usrManager.CreateAsync(usuario, "Password!123");
            }
        }
    }
}
