using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Models;
using Services;

namespace CompanyAPI
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
            services.AddControllers();
            // Setup the database connection
            services.AddDbContext<CompanyContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("CompanyDB")));

            services.AddTransient<IJobService, JobService>();
            services.AddTransient<IEmployeeService, EmployeeService>();
            services.AddTransient<IJobEmployeeService, JobEmployeeService>();
            
            // Setup the swagger generator
            services.AddSwaggerGen(context =>
                {
                    context.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Company API",
                        Description = "My First ASP.NET Core Web API",
                        Contact = new OpenApiContact
                        {
                            Name = "Biagio Pietro Capece",
                            Email = "biagiopietro.capece@student.unife.it"
                        },
                    });

                });
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

            app.UseAuthorization();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                // The instruction below shows the swagger page at the app's root; "http://<host>:<port>/"
                c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
