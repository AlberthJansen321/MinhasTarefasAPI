using MinhasTarefasAPI.Database;
using MinhasTarefasAPI.Models;
using MinhasTarefasAPI.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinhasTarefasAPI.Repositories
{
    public class TarefaRepository : ITarefaRepository
    {
        private readonly MinhasTarefasDBContext _banco;

        public TarefaRepository(MinhasTarefasDBContext banco)
        {
            _banco = banco;
        }

        public List<Tarefa> Restauracao(ApplicationUSER usuario, DateTime? DataUltimaSicronizacao)
        {
            var query = _banco.Tarefas.Where(u => u.UsuarioID == usuario.Id).AsQueryable();

            if(DataUltimaSicronizacao != null)
            {
                query.Where(condicao => condicao.Criado >= DataUltimaSicronizacao || condicao.Atualizado >= DataUltimaSicronizacao).AsQueryable();
            }

            return query.ToList<Tarefa>();
        }

        public List<Tarefa> Sicronizacao(List<Tarefa> tarefas)
        {

           

            var tarefasNovas = tarefas.Where(t => t.IdtarefaApi == 0).ToList();

            var tarefasExluidasAtualizadas = tarefas.Where(t => t.IdtarefaApi != 0).ToList();

            //cadastrar novos registros

            
            if (tarefasNovas.Count() > 0)
            {
                foreach(var tarefa in tarefasNovas)
                {
                    _banco.Tarefas.Add(tarefa);
                }
            }

            //atualizar novos registros

          

            if (tarefasExluidasAtualizadas.Count() > 0)
            {
                foreach (var tarefa in tarefasExluidasAtualizadas)
                {
                    _banco.Tarefas.Update(tarefa);
                }
            }
            //Salvar os dados no banco de dados
            _banco.SaveChanges();

            //retornar todas tarefas cadastradas

            return tarefasNovas.ToList();

        }
    }
}
