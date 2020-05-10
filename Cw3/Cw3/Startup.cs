using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cw3.middlewares;
using Cw3.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cw3
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
            //AddSingleton, AddTransient, AddScoped
            services.AddScoped<IStudentDbService, SqlServerStudentDbService>();
            services.AddScoped<IStudentsDal, SqlServerDbDal>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IStudentDbService service)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<LoggingMiddleware>();

            //wlasny middleware
            app.Use(async (context, next) =>
            {
                if (!context.Request.Headers.ContainsKey("Index"))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Nie poda³eœ indeksu");
                    return;
                }

                string index = context.Request.Headers["Index"].ToString();
                var stud = service.GetStudent(index);
                if(stud == null)
                {
                    context.Response.StatusCode = StatusCodes.Status402PaymentRequired;
                    await context.Response.WriteAsync("Nie ma takiego indeksu w bazie");
                    return;
                }
                //check in db

                await context.Response.WriteAsync(stud.FirstName+" "+stud.LastName);
                await next();
            });


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
