using Amazon.S3;
using Books.Core.Interfaces;
using Books.Core.Services;
using Books.Infrastructure.Data;
using Books.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace Books.API;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
            
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            
        services.AddAWSService<IAmazonS3>(Configuration.GetAWSOptions());

        services.AddTransient<ICoverImageService, CoverImageService>();
        services.AddTransient<IBookService, BookService>();
        services.AddScoped<IBookRepository, BookRepository>();
            
        services.AddAutoMapper(typeof(Startup));

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Books", Version = "v1", Description = "A simple example of .NET 7 Web API. To build on Docker, please run the following command: `docker-compose up --build`. Then use the following URL to access the API: http://localhost:5017/ to access the API docs." });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Books"));
        }

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}