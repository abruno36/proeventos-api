using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProEventos.Application;
using ProEventos.Application.Contratos;
using ProEventos.Domain.Identity;
using ProEventos.Persistence;
using ProEventos.Persistence.Contextos;
using ProEventos.Persistence.Contratos;

namespace ProEventos.API
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
			services.AddDbContext<ProEventosContext>(
			    x => x.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
			);
			services.AddControllers()
				.AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling =
				    Newtonsoft.Json.ReferenceLoopHandling.Ignore
				);
			//Exigências da password abaixo, foram retiradas com o parâmetro false
			IdentityBuilder builder = services.AddIdentityCore<User>(options =>
			{
				options.Password.RequireDigit = false;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireLowercase = false;
				options.Password.RequireUppercase = false;
				options.Password.RequiredLength = 4;
			});

			//configurações de contexto e de roles, de usuário quem vai controlar o cadastro de usuário 
			builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
			builder.AddEntityFrameworkStores<ProEventosContext>();
			builder.AddRoleValidator<RoleValidator<Role>>();
			builder.AddRoleManager<RoleManager<Role>>();
			builder.AddSignInManager<SignInManager<User>>();

			//JWT
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			    .AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
				   		.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
						ValidateIssuer = false,
						ValidateAudience = false
					};
				}
			);
			
			//Politica de autenticação de todas as contollers
			services.AddAuthorization(auth =>
			{
				auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
				.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
				.RequireAuthenticatedUser().Build());
			});

			services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

			services.AddScoped<IEventoService, EventoService>();
			services.AddScoped<ILoteService, LoteService>();

			services.AddScoped<IGeralPersist, GeralPersist>();
			services.AddScoped<IEventoPersist, EventoPersist>();
			services.AddScoped<ILotePersist, LotePersist>();
			services.AddCors();
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProEventos.API", Version = "v1" });
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProEventos.API v1"));
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthentication();  
            		app.UseAuthorization();  

                        app.UseStaticFiles();
			app.UseStaticFiles(new StaticFileOptions(){
				FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
				RequestPath = new PathString("/Resources")
			});

			app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
