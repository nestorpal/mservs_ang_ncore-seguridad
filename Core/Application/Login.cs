using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
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
    public class Login
    {
        public class UsuarioLoginCommand : IRequest<UsuarioDTO>
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class UsuarioLoginValidation : AbstractValidator<UsuarioLoginCommand>
        {
            public UsuarioLoginValidation()
            {
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
            }
        }

        public class UsuarioLoginHandler : IRequestHandler<UsuarioLoginCommand, UsuarioDTO>
        {
            private readonly SeguridadContexto _context;
            private readonly UserManager<Usuario> _usrManager;
            private readonly IMapper _mapper;
            private readonly IJWTGenerator _jwtGenerator;
            private readonly SignInManager<Usuario> _signManager;

            public UsuarioLoginHandler(
                SeguridadContexto context,
                UserManager<Usuario> usrManager,
                IMapper mapper,
                IJWTGenerator jwtGenerator,
                SignInManager<Usuario> signManager)
            {
                this._context = context;
                this._usrManager = usrManager;
                this._mapper = mapper;
                this._jwtGenerator = jwtGenerator;
                this._signManager = signManager;
            }

            public async Task<UsuarioDTO> Handle(UsuarioLoginCommand request, CancellationToken cancellationToken)
            {
                var usuario = await this._usrManager.FindByEmailAsync(request.Email);

                if (usuario == null)
                    throw new Exception("El usuario no existe");

                var resultado = await _signManager.CheckPasswordSignInAsync(usuario, request.Password, false);

                if (resultado.Succeeded)
                {
                    var usuarioDto = _mapper.Map<Usuario, UsuarioDTO>(usuario);
                    usuarioDto.Token = this._jwtGenerator.CreateToken(usuario);

                    return usuarioDto;
                }

                throw new Exception("Login incorrecto");
            }
        }
    }
}
