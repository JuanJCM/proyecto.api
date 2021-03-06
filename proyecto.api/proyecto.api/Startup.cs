using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
//using proyecto.api.Middlewares;
using proyecto.Core.Interfaces;
using proyecto.Core.IRepositories;
using proyecto.Core.Services;
using proyecto.Infrastructure.Repositories;
using proyecto.Infrastructure;
using proyecto.api.Middlewares;

namespace proyecto.api
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
            services.AddCors();

            services.AddControllers().AddNewtonsoftJson(x =>
                x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize);
            services.AddHttpContextAccessor();
            services.AddDbContext<ProyectoDbContext>((s, o) => o.UseSqlite("Data Source=data.db"));
            services.AddScoped(typeof(IRepository<>), typeof(EntityFrameworkRepository<>));
            services.AddScoped<IGroceryListService, GroceryListService>();
            services.AddScoped<IIngredientService, IngredientService>();
            services.AddScoped<IRecipeService, RecipeService>();
            services.AddScoped<IReminderService, ReminderService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IGroceryListRepository, GroceryListRepository>();
            services.AddScoped<IReminderRepository, ReminderRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseWhen(IsVerifyRequestNeeded, applicationBuilder => applicationBuilder.UseMiddleware<UserMiddleware>());

            app.UseCors(x => x.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private bool IsVerifyRequestNeeded(HttpContext context)
        {
            return !context.Request.Path.StartsWithSegments("/api/user");
        }
    }
}
