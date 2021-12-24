using keesonGarmentApi.Common;
using keesonGarmentApi.Services.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Net;

namespace keesonGarmentApi.Filters
{
    public class PermissionFilter : ActionFilterAttribute
    {
        private const string Connection = "Server=192.168.1.236;Database=AuthPlatform;User Id=sa;Password=Wms123456";
        private static readonly IFreeSql FreeSql = new FreeSql.FreeSqlBuilder()
            .UseConnectionString(global::FreeSql.DataType.SqlServer, Connection)
            .UseGenerateCommandParameterWithLambda(true).UseNoneCommandParameter(false)
            .UseLazyLoading(true).Build();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var metas = context.ActionDescriptor.EndpointMetadata.Where(x => x is PermissionAttribute).ToList();

            if (!metas.Any())
            {
                return;
            }

            var user = context.HttpContext.User;
            if (user.Identity is { IsAuthenticated: false })
            {
                context.Result = new ContentResult 
                { 
                    StatusCode = HttpStatusCode.Unauthorized.GetHashCode(),
                    Content = "您没有登录!"
                };
                return;
            }

            var isSuperAdmin = context.HttpContext.User.IsInRole("Super Admin");
            if (isSuperAdmin)
            {
                return;
            }

            var userId = context.HttpContext.User.GetUserId();
            var roleMenus = FreeSql.Ado.Query<string>(@"select MenuCode from AspNetRoleMenus
                                      where RoleId in (select RoleId from AspNetUserRoles where UserId = @UserId)", new { UserId = userId });

            foreach (var meta in metas)
            {
                if (meta is not PermissionAttribute permission) continue;
                if (roleMenus.Contains(permission.Code)) continue;
                context.Result = new ContentResult
                {
                    StatusCode = HttpStatusCode.Forbidden.GetHashCode(),
                    Content = JsonConvert.SerializeObject(new { message = "您没有访问该资源的权限!", status = HttpStatus.NotAccess, roles = roleMenus.ToList() })
                };
            }
        }
    }
}
