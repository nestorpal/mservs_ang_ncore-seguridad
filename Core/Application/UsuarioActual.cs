using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Servicios.Api.Seguridad.Core.DTO;
using Servicios.Api.Seguridad.Core.Entities;
using Servicios.Api.Seguridad.Core.JWTLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicios.Api.Seguridad.Core.Application
{
    public class UsuarioActual
    {
        public class UsuarioActualCommand : IRequest<UsuarioDTO> { } // los valores se obtienen del contexto (usrname) y del header (token)

        public class UsuarioActualHandler : IRequestHandler<UsuarioActualCommand, UsuarioDTO>
        {
            private readonly UserManager<Usuario> _usrManager;
            private readonly IUsuarioSesion _usrSesion;
            private readonly IJWTGenerator _jwtGenerator;
            private readonly IMapper _mapper;

            public UsuarioActualHandler(
                UserManager<Usuario> usrManager,
                IUsuarioSesion usrSesion,
                IJWTGenerator jwtGenerator,
                IMapper mapper
                )
            {
                this._usrManager = usrManager;
                this._usrSesion = usrSesion;
                this._jwtGenerator = jwtGenerator;
                this._mapper = mapper;
            }

            public async Task<UsuarioDTO> Handle(UsuarioActualCommand request, CancellationToken cancellationToken)
            {
                var usuario = await _usrManager.FindByNameAsync(_usrSesion.GetUsuarioSEsion());

                if (usuario != null)
                {
                    var usuarioDto = this._mapper.Map<Usuario, UsuarioDTO>(usuario);
                    usuarioDto.Token = this._jwtGenerator.CreateToken(usuario);

                    return usuarioDto;
                }

                throw new Exception("No se encontró el usuario");
            }
        }
    }
}
