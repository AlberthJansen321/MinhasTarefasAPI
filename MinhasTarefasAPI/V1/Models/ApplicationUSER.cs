using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MinhasTarefasAPI.V1.Models
{
    public class ApplicationUSER : IdentityUser
    {
        public string FullName { get; set; }

        [ForeignKey("UsuarioID")]
        public virtual ICollection<Tarefa> Tarefas { get; set; }

        [ForeignKey("UsuarioID")]
        public virtual ICollection<Token> Tokens { get; set; }
    }
}
