using MinhasTarefasAPI.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinhasTarefasAPI.V1.Repositories.Contracts
{
    public interface ITarefaRepository
    {
        List<Tarefa> Sicronizacao(List<Tarefa> tarefas);
        List<Tarefa> Restauracao(ApplicationUSER usuario,DateTime? DataUltimaSicronizacao);
    }
}
