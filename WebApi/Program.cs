using Database.DocumentManagement;
using Database.DocumentManagement.Repositories;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

namespace WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        Env.TraversePath().Load(".env");
        var env = Environment.GetEnvironmentVariables();

        var builder = WebApplication.CreateBuilder(args);

        string connection =
            $"""
                 Host={env["DB_HOST"]};
                 Port={env["DB_PORT"]};
                 Database={env["DB_DATABASE"]};
                 Username={env["DB_USERNAME"]};
                 Password={env["DB_PASSWORD"]};
             """;
        builder.Services.AddDbContext<DocumentManagementDbContext>(options =>
            options.UseNpgsql(connection)
        );
        builder.Services.AddScoped<DocumentsRepository>();
        builder.Services.AddScoped<TaskItemsRepository>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
