using Application.Services;
using Database.DocumentManagement;
using Database.DocumentManagement.Repositories;
using DotNetEnv;
using System.Text.Json.Serialization;

namespace WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        Env.TraversePath().Load(".env");
        var env = Environment.GetEnvironmentVariables();

        var builder = WebApplication.CreateBuilder(args);

        //If we need to use Postgres:
        //string connection =
        //    $"""
        //         Host={env["DB_HOST"]};
        //         Port={env["DB_PORT"]};
        //         Database={env["DB_DATABASE"]};
        //         Username={env["DB_USERNAME"]};
        //         Password={env["DB_PASSWORD"]};
        //     """;
        //builder.Services.AddDbContext<DocumentManagementDbContext>(options =>
        //    options.UseNpgsql(connection)
        //);

        builder.Services.AddDbContext<DocumentManagementDbContext>();

        builder.Services.AddScoped<DocumentsRepository>();
        builder.Services.AddScoped<TaskItemsRepository>();

        builder.Services.AddScoped<IDocumentsService, DocumentsService>();
        builder.Services.AddScoped<ITaskItemsService, TaskItemsService>();

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                }
            );

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
