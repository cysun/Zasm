using AutoMapper;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zasm.Security.Constants;
using Zasm.Services;

namespace Zasm
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = 100 * 1024 * 1024; // 100MB (default 30MB)
            });

            services.AddControllersWithViews();

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie(opt =>
            {
                opt.AccessDeniedPath = "/Home/AccessDenied";
                opt.Cookie.MaxAge = TimeSpan.FromDays(90);
            })
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = Configuration["OIDC:Authority"];
                options.ClientId = Configuration["OIDC:ClientId"];
                options.ClientSecret = Configuration["OIDC:ClientSecret"];
                options.ResponseType = "code";
                options.Scope.Add("email");
                options.Scope.Add("zasm_claims");
                options.SaveTokens = true;

                options.Events = new OpenIdConnectEvents
                {
                    OnRemoteFailure = context =>
                    {
                        context.Response.Redirect("/");
                        context.HandleResponse();
                        return Task.FromResult(0);
                    }
                };

                if (Environment.IsDevelopment())
                {
                    options.RequireHttpsMetadata = false;
                }
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policy.IsAdmin, policyBuilder => policyBuilder.RequireClaim("zasm_admin", "true"));
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            services.AddAutoMapper(config => config.AddProfile<MapperProfile>());

            // services.AddRouting(options => options.LowercaseUrls = true);

            services.AddScoped<ClassService>();
            services.AddScoped<StudentService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                IdentityModelEventSource.ShowPII = true;
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseSerilogRequestLogging();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UsePathBase(Configuration["Application:PathBase"]);
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
