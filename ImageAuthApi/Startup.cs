using ImageAuthApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageAuthApi;

public class Startup
{
    public Startup( IConfiguration configuration )
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }
    public void ConfigureServices( IServiceCollection services )
    {
        // Configuration de la base de données
        services.AddDbContext<HashDataContext>(opt => opt.UseInMemoryDatabase("HashData"));

        // Configuration de Swagger/OpenAPI
        services.AddSwaggerGen();

        // Ajoutez d'autres services d'injection de dépendances selon vos besoins.

        services.AddControllers();
    }
    public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }


}
