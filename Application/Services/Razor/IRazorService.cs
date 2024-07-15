

using Microsoft.AspNetCore.Components;

public interface IRazorService
{
    Task<string> Render<T>(Dictionary<string, object?> parameters) where T : IComponent;
}
