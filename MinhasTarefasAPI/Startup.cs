using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinhasTarefasAPI.Database;
using MinhasTarefasAPI.V1.Herlpers.Swagger;
using MinhasTarefasAPI.V1.Models;
using MinhasTarefasAPI.V1.Repositories;
using MinhasTarefasAPI.V1.Repositories.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinhasTarefasAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(opt =>
            {
                opt.SuppressModelStateInvalidFilter = true;
            });

            //banco de dados
            services.AddDbContext<MinhasTarefasDBContext>(config =>
           {
               config.UseSqlite("Data Source=Database/MinhasTarefas.db");
           });

            /*
             Repositories
             */
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<ITarefaRepository, TarefaRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            //

            //Swagger config
            services.AddApiVersioning(cfg =>
            {
                cfg.ReportApiVersions = true;
                cfg.AssumeDefaultVersionWhenUnspecified = true;
                cfg.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
            });

            services.AddSwaggerGen(cfg =>
            {
                cfg.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "Adicione o JSON Web Token(JWT) para autenticar.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"

                });


                cfg.AddSecurityRequirement( new OpenApiSecurityRequirement
                { 
                { new OpenApiSecurityScheme {
                 Reference = new OpenApiReference{
                Id = "Bearer", 
                Type = ReferenceType.SecurityScheme
                 }
                 },new List<string>()
                 }});

                cfg.ResolveConflictingActions(conflito => conflito.First());
                cfg.SwaggerDoc("v1.0", new OpenApiInfo()
                {
                    Title = "MinhasTarefasAPI - V1.0",
                    Version = "V1.0"
                });

                var CaminhoProjeto = PlatformServices.Default.Application.ApplicationBasePath;
                var NomeArquivo = $"{PlatformServices.Default.Application.ApplicationName}.xml";
                var ArquivodeComentario = Path.Combine(CaminhoProjeto, NomeArquivo);
                cfg.IncludeXmlComments(ArquivodeComentario);

                cfg.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var actionApiVersionModel = apiDesc.ActionDescriptor?.GetApiVersion();
                    // would mean this action is unversioned and should be included everywhere
                    if (actionApiVersionModel == null)
                    {
                        return true;
                    }
                    if (actionApiVersionModel.DeclaredApiVersions.Any())
                    {
                        return actionApiVersionModel.DeclaredApiVersions.Any(v => $"v{v.ToString()}" == docName);
                    }
                    return actionApiVersionModel.ImplementedApiVersions.Any(v => $"v{v.ToString()}" == docName);
                });

                cfg.OperationFilter<ApiVersionOperationFilter>();

            });




            //adicionar o service do identity via token
            services.AddIdentity<ApplicationUSER, IdentityRole>()
                        .AddEntityFrameworkStores<MinhasTarefasDBContext>()
                        .AddDefaultTokenProviders();

            //Concerta o erro de loop infinito json
            services.AddMvc(config =>
           {
               config.EnableEndpointRouting = false;
               config.ReturnHttpNotAcceptable = true;
               config.InputFormatters.Add(new XmlSerializerInputFormatter(config));
               config.OutputFormatters.Add(new XmlSerializerOutputFormatter());

           }).AddNewtonsoftJson(o =>
           {
               o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
           });


            //Redirecionamento de Login
            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;

                };
            });

            //
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("minha-api-aprendizado@"))
                };

            });
            //
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build());
            });


        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseStatusCodePages();
            app.UseAuthentication();
            app.UseHttpsRedirection();

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(cfg =>
            {

                cfg.SwaggerEndpoint("/swagger/v1.0/swagger.json", "MinhasTarefasAPI - V1.0");
                cfg.RoutePrefix = string.Empty;

            });
        }
    }
}
