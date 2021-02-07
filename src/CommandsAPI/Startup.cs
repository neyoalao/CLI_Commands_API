using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandsAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using AutoMapper;
using Newtonsoft.Json.Serialization;
using Microsoft.OpenApi.Models;
using CommandsAPI.Models;

namespace CommandsAPI
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IConfiguration Configuration {get;}
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //int? Port = Convert.ToInt32(Configuration["DBPort"]);

            //var builder = new NpgsqlConnectionStringBuilder();
            //builder.ConnectionString = 
            //    //                               environment specified variable or hardcoded variable
            //    builder.Username = Configuration["UserID"] ?? "SA";
            //    builder.Password = Configuration["Password"] ?? "pa55w0rrd";
            //    builder.Database = Configuration["Database"] ?? "CommandsAPI";
            //    builder.Host = Configuration["DBServer"] ?? "localhost";
            //    builder.Port =  port ?? 5432;

            var username = Configuration["UserID"] ?? "postgres";
            var password = Configuration["Password"] ?? "pa55w0rrd";
            var database = Configuration["Database"] ?? "CommandsAPI";
            var host = Configuration["DBServer"] ?? "localhost";
            var port = Configuration["DBPort"] ?? "5432";

            services.AddDbContext<CommandsContext>(
                opt => opt.UseNpgsql($"Host={host};Port={port};Database={database};Pooling=true;User ID={username};Password={password};"));
            services.AddControllers().AddNewtonsoftJson(s => {
                s.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<ICommandsAPIRepo, SqlCommandsAPIRepo>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "ToDo API",
            Description = "ASP.NET Core web api for useful coding commands",
            TermsOfService = new Uri("https://example.com/terms"),
            Contact = new OpenApiContact
            {
                Name = "Olaniyi Alao",
                Email = string.Empty,
                Url = new Uri("https://github.com/neyoalao"),
            },
            License = new OpenApiLicense
            {
                Name = "Use under MIT Open Liecense",
                Url = new Uri("https://example.com/license"),
            }
            });
            });
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // it has to be called here before other functions
            app.UseCors(options =>
                options.WithOrigins("*")
                .AllowAnyHeader()
                .AllowAnyMethod());
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //creates migrations on the database
            PrepDB.CreateMigration(app);

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
    // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                // To serve the Swagger UI at the app's root (http://localhost:<port>/)
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
