using AutoMapper;
using keesonGarmentApi.Common;
using keesonGarmentApi.Entities;
using keesonGarmentApi.Services.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace keesonGarmentApi.Services
{
    public class BaseService
    {
        protected readonly IMemoryCache Cache;
        protected readonly GarmentContext GarmentContext;
        protected readonly IMapper Mapper;

        private const string Connection = "Server=192.168.1.236;Database=AuthPlatform;User Id=sa;Password=Wms123456";
        private static readonly IFreeSql FreeSql = new FreeSql.FreeSqlBuilder()
            .UseConnectionString(global::FreeSql.DataType.SqlServer, Connection)
            .UseGenerateCommandParameterWithLambda(true).UseNoneCommandParameter(false)
            .UseLazyLoading(true).Build();

        protected BaseService(GarmentContext garmentContext, IMemoryCache cache, IMapper mapper)
        {
            Cache = cache;
            Mapper = mapper;
            GarmentContext = garmentContext;
            var user = HttpContextHelp.Current?.User;
            if (user != null)
            {
                //UserId = user.GetUserId();
                UserId = "SFD88888";
            }
        }

        protected string UserId { get; }

        protected List<string> GetRoleNames()
        {
            var list = new List<string>
            {
                "行政文员"
                //"部门文员"
            };
            return list;
        }
            
        protected string CoventPosition(string pos)
        {
            var managements = new List<string>()
            {
                "组长","主管"
            };
            
            if (managements.Contains(pos))
            {
                return "管理";
            }
            else
            {
                return "员工";
            }
        }

        protected int GetRoleState()
        {
            var roles = GetRoleNames();
            if (roles.Contains("部门文员"))
            {
                return 0;
            }
            else if (roles.Contains("人事文员"))
            {
                return 1;
            }
            else if (roles.Contains("行政文员"))
            {
                return 2;
            }
            return -1;
        }

        protected string GetRoleSelectDeparment()
        {
            var roles = GetRoleNames();
            if (roles.Contains("部门文员"))
            {
                return (from ge in GarmentContext.GEmployees
                       where ge.Code == UserId
                       select ge.Department).First();
            }
            else if (roles.Contains("人事文员") || roles.Contains("行政文员"))
            {
                return "ALL";
            }
            return "null";
        }

        //protected List<string> GetRoleNames() =>
        //    FreeSql.Ado.Query<string>(@"Select Postion from AspNetRoles
        //                             where Id in ( select RoleId from AspNetUserRoles where UserId = @uid )", new { uid = UserId });
    }
}
