using System;
using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MVS.Template.CSharp.Application.Behavior;
using MVS.Template.CSharp.Application.Command;
using MVS.Template.CSharp.Application.Validation;
using MVS.Template.CSharp.Infrastructure.Behaviors;

namespace MVS.Template.CSharp.Infrastructure
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
            services.AddHealthChecks();
            services.AddOptions();
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "MVS.Template.CSharp HTTP API",
                    Version = "v1",
                    Description = "The Math Solver Service HTTP API",
                    TermsOfService = "Terms Of Service"
                });
            });
            services.AddMvc();
            services.AddMediatR(Assembly.LoadFrom("MVS.Template.CSharp.Application.dll"));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
            services.AddTransient(typeof(IValidator<SolveCalculusCommand>), typeof(SolveCalculusCommandValidation));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHealthChecks("/health");
            app.UseMvc();
            app.UseSwagger()
               .UseSwaggerUI(c =>
               {
                   c.SwaggerEndpoint("/swagger/v1/swagger.json", "MVS.Template.CSharp API V1");
                   c.RoutePrefix = string.Empty;
               }
            );
        }
    }
    
}
