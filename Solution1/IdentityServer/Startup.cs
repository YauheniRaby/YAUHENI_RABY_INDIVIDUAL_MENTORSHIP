using DataAccessLayer.Configuration;
using DataAccessLayer.Repositories;
using DataAccessLayer.Repositories.Abstract;
using IdentityServer.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentityServer()
                .AddInMemoryApiResources(IdentityServerSettings.ApiResources)
                .AddInMemoryIdentityResources(IdentityServerSettings.IdentityResources)
                .AddInMemoryApiScopes(IdentityServerSettings.ApiScopes)
                .AddInMemoryClients(IdentityServerSettings.Clients(Configuration.GetSection("CorsUrls").Get<string[]>()))
                .AddDeveloperSigningCredential()
                .AddCustomUserStore();

            var connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            services.AddDbContext<RepositoryContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Singleton);
            services.AddSingleton<IUserRepository, UserRepository>();            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseIdentityServer();
            app.UseEndpoints(endpoints =>
            {
                
            });
        }
    }
}
