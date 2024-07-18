using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components.Web;

namespace Application.Service
{
    public class RazorService : IRazorService
    {
        private readonly IServiceProvider _serviceProvider;
        public RazorService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<string> Render<T>(Dictionary<string, object?> dictionary) where T : IComponent
        {
            await using var htmlRenderer = new HtmlRenderer(_serviceProvider, _serviceProvider.GetRequiredService<ILoggerFactory>());

            var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
            {
                var parameters = ParameterView.FromDictionary(dictionary);
                var output = await htmlRenderer.RenderComponentAsync<T>(parameters);

                return output.ToHtmlString();
            });

            return html;
        }
    }
}
