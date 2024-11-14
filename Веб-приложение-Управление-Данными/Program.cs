using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Веб_приложение_Управление_Данными
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Добавление необходимых сервисов
            builder.Services.AddControllers();

            // Добавление Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Настройка статических файлов
             // Это позволяет использовать статические файлы из wwwroot

            // Включение Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty; // Чтобы открыть Swagger UI на корневом URL
            });

            // Настройка конечной точки для корневого URL
            app.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to the API!");
            });

            app.MapControllers();

            app.Run();
        }
    }
}