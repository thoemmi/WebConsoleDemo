using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace WebConsoleDemo.Server {
    public class Startup {
        public void ConfigureServices(IServiceCollection services) {
            services.AddLogging();
            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseFileServer();

            app.UseSignalR(routes => { routes.MapHub<Console>("/console"); });
        }
    }
}