using System;
using System.Net;
using System.Text;
using Deeproxio.Infrastructure.Runtime;
using Kubernetes.Configuration.Extensions.Configmap;
using Kubernetes.Configuration.Extensions.Secret;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Prometheus;

namespace Deeproxio.UserManagement.API
{
    public class Startup : IServerStartup
    {
        private string secretKey;
        private SymmetricSecurityKey signingKey;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile($"secrets/appsettings.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsProduction() && Environment.GetEnvironmentVariable("KUBE") == "true")
            {
                builder
                    .AddKubernetesConfigmap("dpioUserManagement.API=config", reloadOnChange: true)
                    .AddKubernetesSecret("dpioUserManagement.API=secret", reloadOnChange: true);
            }

            Configuration = builder.Build();

            secretKey = Configuration.GetValue("JwtIssuerOptions::SecretKey", "iNivDmHLpUA223sqsfhqGbMRdRj1PVkH");
            signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            services.TryAddTransient<IHttpContextAccessor, HttpContextAccessor>();



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHealthChecks("/ready", new HealthCheckOptions()
            {
                // The following StatusCodes are the default assignments for
                // the HealthCheckStatus properties.
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                },
                // The default value is false.
                AllowCachingResponses = false
            });

            app.UseMetricServer();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }


            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseExceptionHandler(
                builder =>
                {
                    builder.Run(
                        async context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                            var error = context.Features.Get<IExceptionHandlerFeature>();
                            if (error != null)
                            {
                                await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(false);
                            }
                        });
                });


            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
