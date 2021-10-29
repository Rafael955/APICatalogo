using APICatalogo.Context;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Exceptions;
using APICatalogo.Filters;
using APICatalogo.Logging;
using APICatalogo.Repository;
using APICatalogo.Repository.Interfaces;
using APICatalogo.Servicos;
using APICatalogo.Servicos.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICatalogo
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
            services.AddCors(options =>
            {
                options.AddPolicy("PermitirApiRequest",
                    builder => builder.WithOrigins("https://apirequest.io").WithMethods("GET")
                    );
                //options.AddPolicy("TudoLiberado",
                    //builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            });

            var mappingConfig = new MapperConfiguration(x =>
            {
                x.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            var mySqlConnection = Configuration.GetConnectionString("DefaultConnection");

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddDbContextPool<ApplicationDbContext>(options => options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));

            services.AddScoped<ApiLoggingFilter>();
            services.AddTransient<IMeuServico, MeuServico>();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //JWT
            //adiciona o manipulador de autentica��o e define o
            //esquema de autentica��o usado : Bearer
            //valida o emissor, a audiencia e a chave
            //usando a chave secreta valida a assinatura
            //Microsoft.AspNetCore.Authentication.JwtBearer - middleware que permite uma aplica��o receber um OpenID Connect bearer token
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidAudience = Configuration["TokenConfiguration:Audience"],
                        ValidIssuer = Configuration["TokenConfiguration:Issuer"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    });

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            loggerFactory.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
            {
                LogLevel = LogLevel.Information
            }));

            //adiciona o middleware de tratamento de erros
            app.ConfigureExceptionHandler();

            //adiciona o middleware para redirecionar para HTTPS
            app.UseHttpsRedirection();

            //adiciona o middleware de roteamento
            app.UseRouting();

            //adiciona o middleware de autentica��o
            app.UseAuthentication();

            //adiciona o middleware de autoriza��o(este deve vir sempre ap�s o de autentica��o).
            app.UseAuthorization();

            //app.UseCors(opt => opt
            //    .WithOrigins("https://apirequest.io")
            //    .WithMethods("GET")
            //    );

            app.UseCors();

            //Adiciona o middleware que executa o endpoint do request atual
            app.UseEndpoints(endpoints =>
            {
                //adiciona os endpoints para as Actions
                // dos controladores sem especificar rotas
                endpoints.MapControllers();
            });
        }
    }
}
