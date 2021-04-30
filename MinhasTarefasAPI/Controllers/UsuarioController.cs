﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MinhasTarefasAPI.Models;
using MinhasTarefasAPI.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinhasTarefasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly SignInManager<ApplicationUSER> _signInManager;
        private readonly UserManager<ApplicationUSER> _userManager;

        public UsuarioController(IUsuarioRepository usuarioRepository, SignInManager<ApplicationUSER> signInManager, UserManager<ApplicationUSER> userManager)
        {
            _usuarioRepository = usuarioRepository;
            _signInManager = signInManager;
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
                    _signInManager.SignInAsync(usuario,false);
                
                    //retornar token (JWT)
                    return Ok(usuario);
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