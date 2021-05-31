using MinhasTarefasAPI.Database;
using MinhasTarefasAPI.V1.Models;
using MinhasTarefasAPI.V1.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinhasTarefasAPI.V1.Repositories
{
    public class TokenRepository : ITokenRepository
    {

        private readonly MinhasTarefasDBContext _banco;

        public TokenRepository(MinhasTarefasDBContext banco)
        {
            _banco = banco;
        }
        public Token Obter(string refreshToken)
        {
            return _banco.Token.FirstOrDefault( t => t.RefreshToken == refreshToken && t.Ultilizado == false);
        }
        public void Cadastrar(Token token)
        {
            _banco.Token.Add(token);
            _banco.SaveChanges();
        }
        public void Atualizar(Token token)
        {
            _banco.Token.Update(token);
            _banco.SaveChanges();
        }      
    }
}
