using MinhasTarefasAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinhasTarefasAPI.Repositories.Contracts
{
    public interface ITarefaRepository
    {
        List<Tarefa> Sicronizacao(List<Tarefa> tarefas);
        List<Tarefa> Restauracao(ApplicationUSER usuario,DateTime? DataUltimaSicronizacao);
    }
}
