using Reports.Reports;

namespace App.Ioc
{
    public static class NativeInjectorConfig
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<RenderMessageReport>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowedCorsOrigins",
                builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
        }
    }
}
