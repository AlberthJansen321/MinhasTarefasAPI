using Microsoft.AspNetCore.Identity;
using MinhasTarefasAPI.V1.Models;
using MinhasTarefasAPI.V1.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinhasTarefasAPI.V1.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly UserManager<ApplicationUSER> _userManager;

        public UsuarioRepository(UserManager<ApplicationUSER> userManager)
        {
            _userManager = userManager;
        }
        public ApplicationUSER obter(string email, string senha)
        {
            var usuario = _userManager.FindByEmailAsync(email).Result;
            if (_userManager.CheckPasswordAsync(usuario, senha).Result)
            {
                return usuario;
            }
            else
            {
                throw new Exception("Usuario não localizado!");
            }

        }
        public void Cadastrar(ApplicationUSER usuario,string senha)
        {

            var result = _userManager.CreateAsync(usuario,senha).Result;
            if (result.Succeeded)
            {
                StringBuilder sb = new StringBuilder();
                foreach(var erro in result.Errors)
                {
                    sb.Append(erro.Description);
                }

                throw new Exception($"Usuario não cadastrado! {sb.ToString()}");

            }
            else
            {

            }

        }

        public ApplicationUSER obter(string id)
        {
            return _userManager.FindByIdAsync(id).Result;
        }
    }
}
