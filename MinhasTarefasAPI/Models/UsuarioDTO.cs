using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MinhasTarefasAPI.Models
{
    public class UsuarioDTO
    {
        [Required(ErrorMessage = "Campo Nome é Obrigatório")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "Campo Email é Obrigatório")]
        [EmailAddress(ErrorMessage = "Email Inválido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Campo Senha é Obrigatório")]
        public string Senha { get; set; }
        [Required(ErrorMessage = "Campo Confirmação de Senha é Obrigatório")]
        [Compare("Senha")]
        public string ConfimacaoSenha { get; set; }
       
    }
}
