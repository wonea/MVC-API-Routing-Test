using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Test.API.Middleware;
using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.Extensions.Hosting;

namespace Test.API
{
    public class Startup
    {
        private IConfigurationRoot _configuration { get; set; }

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            // environmental variables
            builder.AddEnvironmentVariables();

            // get configuration
            _configuration = builder.Build();
        }

        /// <summary>
        /// ConfigureServices by the runtime. Use this method to add services to the container
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddApplicationInsightsTelemetry(_configuration);

            // memory cache
            services.AddMemoryCache();
            services.AddMvc()
            .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            // CORS
            var corsBuilder = new CorsPolicyBuilder();

            services.AddCors(builder =>
            {
                corsBuilder.AllowAnyHeader();
                corsBuilder.AllowAnyMethod();
                corsBuilder.AllowAnyOrigin();
                corsBuilder.WithOrigins("*");
                corsBuilder.AllowCredentials();
            });
        }

        /// <summary>
        /// Configure called by the runtime. Use this method to configure the HTTP request pipeline
        /// </summary>
        public void Configure(IApplicationBuilder app, IHostApplicationLifetime appLifetime)
        {
            appLifetime.ApplicationStarted.Register(OnStarted);
            appLifetime.ApplicationStopping.Register(OnStopping);
            appLifetime.ApplicationStopped.Register(OnStopped);

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                appLifetime.StopApplication();
                // Don't terminate the process immediately, wait for the Main thread to exit gracefully.
                eventArgs.Cancel = true;
            };

            app.UseRouting();
            app.UseSecurityMiddleware();
            app.UseUserValidation();

            // websockets
            var webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120)
            };
            app.UseWebSockets(webSocketOptions);
            app.UseWebSocketMiddleware();

            // put last so header configs like CORS or Cookies etc can fire
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// Perform post-startup activities here
        /// </summary>
        private void OnStarted()
        {
            Debug.WriteLine("Started Core API");
        }

        /// <summary>
        /// Perform on-stopping activities here
        /// </summary>
        private void OnStopping()
        {
            Debug.WriteLine("Stopping Core API");
        }

        /// <summary>
        /// Perform post-stopped activities here
        /// </summary>
        private void OnStopped()
        {
            Debug.WriteLine("Stopped Core API");
        }
    }

    #region Extension Method

    public static partial class RequestExtensions
    {
        public static IApplicationBuilder UseUserValidation(this IApplicationBuilder app)
        {
            app.UseMiddleware<UserValidatorMiddleware>();
            return app;
        }
    }

    #endregion
}