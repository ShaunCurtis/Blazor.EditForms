/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazor.EditForms.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor.EditForms.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBlazorForms(this IServiceCollection services)
        {
            services.AddScoped<EditStateService>();
            return services;
        }
    }
}
