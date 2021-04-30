﻿using MinhasTarefasAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinhasTarefasAPI.Repositories.Contracts
{
    public interface IUsuarioRepository
    {
        //metodo para cadastrar um novo usuario no banco de dados
        void Cadastrar(ApplicationUSER usuario,string senha);
        //metodo para fazer login 
        ApplicationUSER obter(string email, string senha);
    }
}
