using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class TarefaController : ControllerBase
    {
        private readonly ITarefaRepository _tarefaRepository;
        private readonly UserManager<ApplicationUSER> _userManager;

        public TarefaController(ITarefaRepository tarefaRepository, UserManager<ApplicationUSER> userManager)
        {
            _tarefaRepository = tarefaRepository;
            _userManager = userManager;
        }
        [Authorize]
        [HttpGet("restaurar")]
        public ActionResult Restaurar(DateTime data)
        {
            var usuario = _userManager.GetUserAsync(HttpContext.User).Result;

            return Ok(_tarefaRepository.Restauracao(usuario, data));
        }
        [Authorize]
        [HttpPost("sicronizar")]
        public ActionResult Sicronizar([FromBody] List<Tarefa> tarefas)
        {
            return Ok(_tarefaRepository.Sicronizacao(tarefas));

//            return Ok(tarefas);
        }
        [HttpGet("modelo")]
        public ActionResult modelo()
        {
            return Ok(new Tarefa());
        }


    }
}
