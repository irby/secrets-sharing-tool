using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SecretSharingTool.Data.Database;
using SecretsSharingTool.Api.Data;
using SecretsSharingTool.Api.Middleware;
using SecretsSharingTool.Api.Pipeline;
using SecretsSharingTool.Api.Queue;
using SecretsSharingTool.Core;
using SecretsSharingTool.Core.Erase;

namespace SecretsSharingTool.Api
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
            
            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            services.AddMediatR(typeof(ISecretsSharingToolCore).GetTypeInfo().Assembly);

            AssemblyScanner.FindValidatorsInAssembly(typeof(ISecretsSharingToolCore).Assembly).ForEach(item => services.AddScoped(item.InterfaceType, item.ValidatorType));

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

            services.AddScoped<EraseSecretMessageService>();
            services.AddScoped<SecretInvalidationBackgroundService>();
            
            AddDatabase(services);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "SecretsSharingTool.Api", Version = "v1"});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
            IWebHostEnvironment env,
            ILogger<Startup> logger,
            IHostApplicationLifetime applicationLifetime,
            SecretInvalidationBackgroundService secretInvalidationBackgroundService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SecretsSharingTool.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseCors("CorsPolicy");

            app.UseRouting();

            app.UseMiddleware<IpAddressLoggingMiddleware>();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            
            applicationLifetime.ApplicationStarted.Register(() =>
            {
                logger.LogInformation("Starting background threads");
                
                StartQueue(secretInvalidationBackgroundService, true);
            });

            applicationLifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Stopping background threads");
                
                StopQueue(secretInvalidationBackgroundService, true);
            });
            
            void StartQueue(IBackgroundQueueService queue, bool flag)
            {
                if (flag)
                {
                    logger.LogInformation($"Starting queue: {queue.GetType()}");
                    queue.Start();
                    queue.Process();
                }
            }

            void StopQueue(IBackgroundQueueService queue, bool flag)
            {
                if (flag)
                {
                    logger.LogInformation($"Stopping queue: {queue.GetType()}");
                    queue.Stop();
                }
            }
        }

        public void AddDatabase(IServiceCollection serviceCollection)
        {
            var template = "server={0};database={1};user={2};password={3};default command timeout=180";
            var connectionString = string.Format(template, Configuration["DB_HOST"], Configuration["DB_CATALOG"],
                Configuration["DB_USER"], Configuration["DB_PASSWORD"]);
            DataServices.Setup(serviceCollection, connectionString);
        }
    }
}