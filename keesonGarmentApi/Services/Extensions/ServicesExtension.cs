using System.Security.Claims;
using keesonGarmentApi.Common;

namespace keesonGarmentApi.Services.Extensions;

public static class ServicesExtension
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddScoped<GarmentService>();
        services.AddScoped<GarmentIssuingService>();
        //services.AddScoped<GarmentSizeService>();
        services.AddScoped<GEmployeeService>();
        services.AddScoped<GarmentAssignedLogService>();
        services.AddScoped<GarmentIssuingRuleService>();
        services.AddScoped<SelectorService>();
        return services;
    }
    
    public static IServiceCollection AddCommonOptions(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions();
        services.Configure<IdentityClientOptions>(config.GetSection("IdentityClientOptions"));
        return services;
    }
    
    
    public static IServiceCollection AddHelpers(this IServiceCollection services)
    {
        return services;
    }
    
  
    public static string? GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.Claims.FirstOrDefault(p => p.Type == "sub")?.Value;
    }
    
}