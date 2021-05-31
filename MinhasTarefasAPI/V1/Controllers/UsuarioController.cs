using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MinhasTarefasAPI.V1.Models;
using MinhasTarefasAPI.V1.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MinhasTarefasAPI.V1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        //private readonly SignInManager<ApplicationUSER> _signInManager;
        private readonly ITokenRepository _tokenRepository;
        private readonly UserManager<ApplicationUSER> _userManager;

        public UsuarioController(IUsuarioRepository usuarioRepository, ITokenRepository tokenRepository,UserManager<ApplicationUSER> userManager)
        {
            _usuarioRepository = usuarioRepository;
            _tokenRepository = tokenRepository;
            //_signInManager = signInManager;
            _userManager = userManager;
        }
        [HttpPost("login")]
        public ActionResult Login([FromBody]UsuarioDTO usuarioDTO)
        {
            ModelState.Remove("ConfimacaoSenha");
            ModelState.Remove("Nome");

            if (ModelState.IsValid)
            {
                ApplicationUSER usuario = _usuarioRepository.obter(usuarioDTO.Email,usuarioDTO.Senha);

                if(usuario != null)
                {
                    //Login no identity
                    //_signInManager.SignInAsync(usuario,false);

                    //retornar token (JWT)
                    return GerarToken(usuario);

                }
                else
                {
                    return NotFound("Usuário não localizado");
                }
            }
            else
            {
                return UnprocessableEntity(ModelState);
            }
        }

        private ActionResult GerarToken(ApplicationUSER usuario)
        {
            var token = BuildToken(usuario);

            var TokenModel = new Token()
            {
                RefreshToken = token.RefreshToken,
                Expiration = token.Expiration,
                ExpirationRefreshToken = token.ExpirationRefreshToken,
                Usuario = usuario,
                Criado = DateTime.Now,
                Ultilizado = false
            };

            _tokenRepository.Cadastrar(TokenModel);

            return Ok(token);
        }

        [HttpPost("renovar")]
        public ActionResult Renovar(TokenDTO tokenDTO)
        {
           var RefreshTokenDB = _tokenRepository.Obter(tokenDTO.RefreshToken);

            if (RefreshTokenDB == null)
                return NotFound();

            //atualiar base dados - token será usado
            RefreshTokenDB.Atualizado = DateTime.Now;
            RefreshTokenDB.Ultilizado = true;
            _tokenRepository.Atualizar(RefreshTokenDB);
            //gerar um novo refresh token

            var usuario = _usuarioRepository.obter(RefreshTokenDB.UsuarioID);

            
            return GerarToken(usuario);

        }

        private TokenDTO BuildToken(ApplicationUSER usuario)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email,usuario.Email),
                new Claim(JwtRegisteredClaimNames.Sub,usuario.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("minha-api-aprendizado@"));
            var singn = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
            var exp = DateTime.UtcNow.AddHours(1);

            JwtSecurityToken token = new JwtSecurityToken(

                    issuer: null,
                    audience: null,
                    claims: claims,
                    expires: exp,
                    signingCredentials: singn
                );

            //String Token
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            
            //Data de Expiração do Refresh Token
            var expRefreshToken = DateTime.UtcNow.AddHours(2);

            //Refresh Token
            var refreshtoken = Guid.NewGuid().ToString();

            var TokenDTO = new TokenDTO{ Token = tokenString, Expiration = exp,RefreshToken =  refreshtoken,ExpirationRefreshToken = expRefreshToken};

            return TokenDTO;
        }

        [HttpPost("")]
        public ActionResult Cadastrar([FromBody] UsuarioDTO usuarioDTO)
        {
            if (ModelState.IsValid)
            {
                ApplicationUSER usuario = new ApplicationUSER();
                usuario.UserName = usuarioDTO.Email;
                usuario.FullName = usuarioDTO.Nome;
                usuario.Email = usuarioDTO.Email;

                var resultado =  _userManager.CreateAsync(usuario,usuarioDTO.Senha).Result;

                if (!resultado.Succeeded)
                {
                    List<string> erros = new List<string>();
                    foreach(var erro in resultado.Errors)
                    {
                        erros.Add(erro.Description);
                    }

                    return UnprocessableEntity(erros);
                }
                else
                {
                    return Ok(usuario);
                }
                
            }
            else
            {
                return UnprocessableEntity(ModelState);
            }
        }
    }
}
