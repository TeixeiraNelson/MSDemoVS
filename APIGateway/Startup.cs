using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Authorization;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace APIGateway
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IConfiguration OcelotConfiguration { get; }

        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            Configuration = configuration;

            var builder = new ConfigurationBuilder();
            builder.SetBasePath(env.ContentRootPath)
                   .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
                   .AddEnvironmentVariables();

            OcelotConfiguration = builder.Build();
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            var client_id = "bms-demo-app";

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer("Bearer", o =>
            {
                o.RequireHttpsMetadata = false; // for dev env
                o.MetadataAddress = "http://localhost:8080/auth/realms/master/.well-known/openid-configuration";
                o.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidAudience = client_id
                };
                o.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = c =>
                    {
                        c.NoResult();
                        c.Response.StatusCode = 500;
                        c.Response.ContentType = "text/plain";
                        return c.Response.WriteAsync(c.Exception.ToString());
                    }
                };
            });

            services.AddOcelot(OcelotConfiguration);

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }       

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var configration = new OcelotPipelineConfiguration
            {
                AuthorizationMiddleware = async (ctx, next) =>
                {
                    if (this.Authorize(ctx))
                    {
                        await next.Invoke();
                    }
                    else
                    {
                        ctx.Items.SetError(new UnauthorizedError($"Fail to authorize"));
                    }
                }
            };

            app.UseOcelot(configration).Wait();
        }

        private bool Authorize(HttpContext ctx)
        {
            if (ctx.Items.DownstreamRoute().AuthenticationOptions.AuthenticationProviderKey == null) return true;
            else
            {

                bool auth = false;
                Claim[] claims = ctx.User.Claims.ToArray<Claim>();
                Dictionary<string, string> required = ctx.Items.DownstreamRoute().RouteClaimsRequirement;
                Regex reor = new Regex(@"[^,\s+$ ][^\,]*[^,\s+$ ]");
                MatchCollection matches;

                Regex reand = new Regex(@"[^&\s+$ ][^\&]*[^&\s+$ ]");
                MatchCollection matchesand;
                int cont = 0;
                foreach (KeyValuePair<string, string> claim in required)
                {
                    matches = reor.Matches(claim.Value);
                    foreach (Match match in matches)
                    {
                        matchesand = reand.Matches(match.Value);
                        cont = 0;
                        foreach (Match m in matchesand)
                        {
                            foreach (Claim cl in claims)
                            {
                                if (cl.Type == claim.Key)
                                {
                                    if (cl.Value == m.Value)
                                    {
                                        cont++;
                                    }
                                }
                            }
                        }
                        if (cont == matchesand.Count)
                        {
                            auth = true;
                            break;
                        }
                    }
                }
                return auth;
            }
        }
    }
}
