using IdentityServer4.AccessTokenValidation;
using keesonGarmentApi.Common;
using keesonGarmentApi.Services.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);
var identityClient = builder.Configuration.GetSection("IdentityClientOptions").Get<IdentityClientOptions>();

//Add DbContext
builder.Services.AddDbContext<GarmentContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

// Add Controller
builder.Services.AddControllers().AddNewtonsoftJson(option =>
                {
                    //忽略循环引用
                    //option.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    option.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                    option.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                }
            );
//builder.Services.AddControllers();

//Add MemoryCache
builder.Services.AddMemoryCache();

//Add Service
builder.Services.AddCommonOptions(builder.Configuration).AddHelpers().AddApiServices();
builder.Services.AddHttpContextCurrent();

//Add AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//Add Ids4 TokenValidation
builder.Services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
    .AddIdentityServerAuthentication(config =>
    {
        config.Authority = identityClient.Authority;
        config.RequireHttpsMetadata = identityClient.UseHttps;
        config.SaveToken = identityClient.SaveToken;
        config.ApiName = identityClient.ApiName;
        config.ApiSecret = identityClient.ApiSecret;
    });

//Add Cors
//解决跨域问题
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("Any", policy =>
    { policy.SetIsOriginAllowed(_ => true).AllowAnyHeader().AllowAnyMethod().AllowCredentials(); });
});

#region Add Swagger

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo {Title = "Keeson.Garment.Api", Version = "v1"});
    opt.UseInlineDefinitionsForEnums();
    opt.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "keesonGarmentApi.xml"));
    opt.OperationFilter<AddResponseHeadersFilter>();
    opt.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
    opt.OperationFilter<SecurityRequirementsOperationFilter>();
    opt.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{identityClient.Authority}/connect/authorize"),
                TokenUrl = new Uri($"{identityClient.Authority}/connect/token"),
                Scopes = new Dictionary<string, string> {{"garment.api", "Access keeson garment api."}}
            }
        }
    });
});

#endregion

builder.Services.AddEndpointsApiExplorer();
var app = builder.Build();

app.UseHttpContextCurrent();
app.UseCors("Any");

app.UseSwagger();
app.UseDeveloperExceptionPage();
app.UseSwaggerUI(opt =>
{
    opt.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    opt.OAuthClientId("garment.api.swagger");
    opt.RoutePrefix = string.Empty;
});
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();