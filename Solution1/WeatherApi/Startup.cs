using DataAccessLayer.Configuration;
using DataAccessLayer.Enums;
using FluentValidation.AspNetCore;
using Hangfire;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using WeatherApi.Configuration;
using WeatherApi.Extensions;
using WeatherApi.Infrastructure.Hangfire;

namespace WeatherApi
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
            var connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            services.AddDbContext<RepositoryContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Singleton);
            services.Configure<AppConfiguration>(Configuration.GetSection(nameof(AppConfiguration)));
            services.Configure<BackgroundJobConfiguration>(Configuration.GetSection(nameof(BackgroundJobConfiguration)));
            services.Configure<WeatherApiConfiguration>(Configuration.GetSection(nameof(WeatherApiConfiguration)));
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            services.AddSingleton<ExceptionHangfireFilter>();
            services.AddHangfire((provider, config) => config
                .UseSqlServerStorage(connectionString)
                .UseFilter(provider.GetService<ExceptionHangfireFilter>()));
            services.AddHangfireServer();

            services.AddStartupFilters();
            services.AddServices();
            services.AddRepositories();
            services.AddValidators();
            services.AddLogging(opt => opt.AddSimpleConsole());

            services.AddControllers()
                .AddFluentValidation();

            var authority = Configuration["IdentityUrl"];
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WeatherAPI", Version = "v1" });
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri($"{authority}/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "weather_api", "WeatherAPI" },
                            },
                        },
                    }
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2",
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    },
                });
            });            
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            .AddIdentityServerAuthentication(options =>
            {
                options.Authority = authority;
                options.ApiName = "weather_api";
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Constants.AllUsersPolicy, policy => policy.RequireRole(nameof(Role.Admin), nameof(Role.User)));
                options.AddPolicy(Constants.AdminPolicy, policy => policy.RequireRole(nameof(Role.Admin)));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();                
                app.UseHangfireDashboard("/dashboard");                
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WeatherApi v1");
                c.OAuthAppName("Swagger API");
                c.OAuthClientId("swagger_api");
            });

            app.UseHttpStatusExceptionHandler();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}