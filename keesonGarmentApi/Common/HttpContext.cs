namespace keesonGarmentApi.Common;

public static class HttpContextHelp
{
    private static IHttpContextAccessor _accessor;

    public static HttpContext Current
    {
        get
        {
            var ctx = _accessor?.HttpContext;
            return ctx;
        }
    }

    private static void Config(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }
     

    /// <summary>
    /// Can Use HttpContext.Current
    /// </summary>
    /// <param name="services"></param>
    public static void AddHttpContextCurrent(this IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    }

    /// <summary>
    /// Can Use HttpContext.Current
    /// </summary>
    /// <param name="app"></param>
    public static void UseHttpContextCurrent(this IApplicationBuilder app)
    {
        var httpContextAccessor = app.ApplicationServices.GetService<IHttpContextAccessor>();
        Config(httpContextAccessor);
    }
}