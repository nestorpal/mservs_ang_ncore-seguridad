using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Servicios.Api.Seguridad.Core.DTO;
using Servicios.Api.Seguridad.Core.Entities;
using Servicios.Api.Seguridad.Core.JWTLogic;
using Servicios.Api.Seguridad.Core.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicios.Api.Seguridad.Core.Application
{
    public class Register
    {
        public class UsuarioRegistrarCommand : IRequest<UsuarioDTO>
        {
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class UsuarioRegisterValidation : AbstractValidator<UsuarioRegistrarCommand>
        {
            public UsuarioRegisterValidation()
            {
                RuleFor(x => x.Nombre).NotEmpty();
                RuleFor(x => x.Apellido).NotEmpty();
                RuleFor(x => x.UserName).NotEmpty();
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
            }
        }

        public class UsuarioRegisterHandler : IRequestHandler<UsuarioRegistrarCommand, UsuarioDTO>
        {
            private readonly SeguridadContexto _context;
            private readonly UserManager<Usuario> _usrManager;
            private readonly IMapper _mapper;
            private readonly IJWTGenerator _jwtGenerator;

            public UsuarioRegisterHandler(
                SeguridadContexto context
                , UserManager<Usuario> usrManager
                , IMapper mapper
                , IJWTGenerator jwtGenerator)
            {
                this._context = context;
                this._usrManager = usrManager;
                this._mapper = mapper;
                this._jwtGenerator = jwtGenerator;
            }

            public async Task<UsuarioDTO> Handle(
                UsuarioRegistrarCommand request
                , CancellationToken cancellationToken)
            {
                var existe = await _context.Users.Where(a => a.Email == request.Email).AnyAsync();
                if (existe)
                    throw new Exception("El email del usuario ya existe en la base de datos");

                existe = await _context.Users.Where(a => a.UserName == request.UserName).AnyAsync();
                if (existe)
                    throw new Exception("El username del usuario ya existe en la base de datos");

                var usuario = new Usuario
                {
                    Nombre = request.Nombre
                    ,
                    Apellido = request.Apellido
                    ,
                    Email = request.Email
                    ,
                    UserName = request.UserName
                };

                var resultado = await _usrManager.CreateAsync(usuario, request.Password);
                if (resultado.Succeeded)
                {
                    var usuarioDto = _mapper.Map<Usuario, UsuarioDTO>(usuario);
                    usuarioDto.Token = this._jwtGenerator.CreateToken(usuario);
                    return usuarioDto;
                }

                throw new Exception("No se pudo registrar el usuario");
            }
        }
    }
}
