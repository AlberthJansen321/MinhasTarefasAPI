﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MinhasTarefasAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinhasTarefasAPI.Database
{
    public class MinhasTarefasDBContext: IdentityDbContext<ApplicationUSER>
    {

        public MinhasTarefasDBContext(DbContextOptions<MinhasTarefasDBContext> options) : base(options)
        {

        }

        public DbSet<Tarefa> Tarefas { get; set; }

    }
}