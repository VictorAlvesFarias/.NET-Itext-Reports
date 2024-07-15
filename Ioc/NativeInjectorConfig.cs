
using Application.Service;
using Application.Services.Itext;
using Application.Services.Reports;

namespace App.Ioc
{
    public static class NativeInjectorConfig
    {
        //Injeção das dependecias
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Ele está adicionando o tipe expecificado ao escopo, ja que a interface TEntity não pode ser chamada aqui
            //services.AddScoped(typeof(IBaseRepository<>),typeof(BaseRepository<>));
            //services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IRazorService, RazorService>();
            services.AddScoped<IItextService, ItextService>();
            services.AddScoped<IReportsService, ReportsService>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowedCorsOrigins",
                builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            //services.AddAuthentication(configuration);
        }
    }
}
