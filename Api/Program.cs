using App.Ioc;

namespace Relatorios_Cshtml
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.RegisterServices(builder.Configuration);

            builder.Services.AddSwaggerGen(); 
               
            var app = builder.Build();
            
            app.UseSwagger();

            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
