using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MVS.Template.CSharp.Application.Behavior;
using MVS.Template.CSharp.Application.Command;
using MVS.Template.CSharp.Application.Validation;
using MVS.Template.CSharp.Infrastructure.Behaviors;
using Serilog;
using Serilog.Formatting.Compact;

namespace MVS.Template.CSharp.Infrastructure
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(new CompactJsonFormatter())
                .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddOptions();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "MVS.Template.CSharp HTTP API",
                    Version = "v1",
                    Description = "MVS.Template.CSharp HTTP API"
                });
            });
            services.AddControllers();
            
            services.AddMediatR(typeof(SolveCalculusCommandHandler));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
            services.AddTransient(typeof(IValidator<SolveCalculusCommand>), typeof(SolveCalculusCommandValidation));

            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseSwagger()
               .UseSwaggerUI(c =>
               {
                   c.SwaggerEndpoint("/swagger/v1/swagger.json", "MVS.Template.CSharp API V1");
                   c.RoutePrefix = string.Empty;
               }
            );
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("health");
                endpoints.MapControllers();
            });
        }
    }
    
}
